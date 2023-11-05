using System;
using System.IO;
using System.Text;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string folderPath = dialog.SelectedPath;
                    FolderPathText.Text = folderPath;
                }
            }
        }

        private void ProcessFiles(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Console.WriteLine(file);
                string extension = Path.GetExtension(file);
                if (extension == ".c" || extension == ".h")
                {
                    ConvertToUtf8(file);
                }
            }

            OutputText.Text = "File encoding conversion completed.";
        }
        private void ConvertToUtf8(string filePath)
        {
            string tempFile = Path.GetTempFileName();
            try
            {

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                byte[] content = File.ReadAllBytes(filePath);
                content = Encoding.Convert(Encoding.GetEncoding("GB18030"), Encoding.GetEncoding("UTF-8"), content);
                File.WriteAllBytes(tempFile, content);
                File.Delete(filePath);
                File.Move(tempFile, filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting file to UTF-8: {ex.Message}");
                OutputText.Text = $"Error converting file to UTF-8: {ex.Message}";
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

            static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
            {
                // Get information about the source directory
                var dir = new DirectoryInfo(sourceDir);

                // Check if the source directory exists
                if (!dir.Exists)
                    throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

                // Cache directories before we start copying
                DirectoryInfo[] dirs = dir.GetDirectories();

                // Create the destination directory
                Directory.CreateDirectory(destinationDir);

                // Get the files in the source directory and copy to the destination directory
                foreach (FileInfo file in dir.GetFiles())
                {
                    string targetFilePath = Path.Combine(destinationDir, file.Name);
                    file.CopyTo(targetFilePath);
                }

                // If recursive and copying subdirectories, recursively call this method
                if (recursive)
                {
                    foreach (DirectoryInfo subDir in dirs)
                    {
                        string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                        CopyDirectory(subDir.FullName, newDestinationDir, true);
                    }
                }
            }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FolderPathText.Text))
            {
                ProcessFiles(FolderPathText.Text);
            }
            else
            {
                System.Windows.MessageBox.Show("路径不能为空！");
            }
        }
        private void SaveButtonAnotherPlace_Click(object sender, RoutedEventArgs e)
        {
            string folderPath=null;
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    folderPath = dialog.SelectedPath;
                    CopyDirectory(FolderPathText.Text, folderPath,true);
                    ConvertToUtf8(folderPath);
                }
            }
           
        }
    }
}