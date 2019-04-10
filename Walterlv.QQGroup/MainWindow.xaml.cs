using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace Walterlv
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var groups = QQGroup.Find();
            GroupNameTextBlock.Text = groups.FirstOrDefault()?.Name;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}