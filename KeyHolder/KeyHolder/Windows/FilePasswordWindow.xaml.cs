using KeyHolder.Utils;
using System.Windows;

namespace KeyHolder.Windows
{
    /// <summary>
    /// Interaction logic for FilePasswordWindow.xaml
    /// </summary>
    public partial class FilePasswordWindow : Window
    {
        public FilePasswordWindow()
        {
            InitializeComponent();
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            TechOperations.FilePass = this.PassBox.Password;
            this.Close();
        }
    }
}
