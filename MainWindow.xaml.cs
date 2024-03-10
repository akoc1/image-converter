using System.Windows;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Diagnostics;

namespace ImageConverter
{
    public partial class MainWindow : Window
    {
        enum ConvertTypes
        {
            PNGtoJPG,
            JPGtoPNG
        }

        ConvertTypes convertType = ConvertTypes.PNGtoJPG;

        private string? selectedFile;

        public MainWindow()
        {
            InitializeComponent();

            ComboBox.SelectedIndex = 0;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] filePath = (string[])e.Data.GetData(DataFormats.FileDrop);

            selectedFile = filePath[0];

            switch (Path.GetExtension(selectedFile))
            {
                case ".png":
                    convertType = ConvertTypes.PNGtoJPG;
                    ComboBox.SelectedIndex = 0;
                    break;
                case ".jpeg" or ".jpg":
                    convertType = ConvertTypes.JPGtoPNG;
                    ComboBox.SelectedIndex = 1;
                    break;
            }

            SetPreviewImage();
        }

        private void SetPreviewImage()
        {
            if (!File.Exists(selectedFile))
                return;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(selectedFile);
            bitmap.EndInit();

            ImageViewer.Source = bitmap;
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(selectedFile))
            {
                MessageBox.Show("File not selected.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            switch (convertType)
            {
                case ConvertTypes.PNGtoJPG:
                    Image png = Image.FromFile(selectedFile);

                    png.Save(desktopPath + @"/" + Path.GetFileNameWithoutExtension(selectedFile) + ".jpeg", ImageFormat.Jpeg);
                    png.Dispose();

                    break;
                case ConvertTypes.JPGtoPNG:
                    Image jpg = Image.FromFile(selectedFile);
                    
                    jpg.Save(desktopPath + @"/" + Path.GetFileNameWithoutExtension(selectedFile) + ".png", ImageFormat.Png);
                    jpg.Dispose();
                    
                    break;
            }

            InfoTextBlock.Text = "File saved to desktop!";
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ImageViewer.Source = null;
            selectedFile = null;
            InfoTextBlock.Text = "Select a file to convert";
        }

        private void ImageViewer_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            //if (ImageViewer.Source != null)
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Select file";

            switch (convertType)
            {
                case ConvertTypes.PNGtoJPG:
                    dialog.Filter = "Image file(*.png)|*.png";
                    break;
                case ConvertTypes.JPGtoPNG:
                    dialog.Filter = "Image file(*.jpg;*.jpeg)|*.jpg;*.jpeg";
                    break;
            }

            Nullable<bool> result = dialog.ShowDialog(this);

            if (result == true)
            {
                selectedFile = dialog.FileName;

                InfoTextBlock.Text = Path.GetFileNameWithoutExtension(selectedFile);
                SetPreviewImage();
            }
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox.SelectedIndex == 0)
                convertType = ConvertTypes.PNGtoJPG;
            else
                convertType = ConvertTypes.JPGtoPNG;
        }
    }
}