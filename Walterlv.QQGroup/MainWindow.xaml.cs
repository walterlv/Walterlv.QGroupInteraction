using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private QQChat _current;
        private CancellationTokenSource _cancellationTokenSource;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _current = QQChat.Find().FirstOrDefault();
            GroupNameTextBlock.Text = _current?.Name;
        }

        private void SendSingleButton_Click(object sender, RoutedEventArgs e)
        {
            _current?.SendMessageAsync(MessageTextBox.Text);
        }

        private async void SendMultipleButton_Click(object sender, RoutedEventArgs e)
        {
            RootPanel.IsEnabled = false;
            SendingPanel.Visibility = Visibility.Visible;
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                await MultipleSendAsync(MessageTextBox.Text, _cancellationTokenSource.Token);
            }
            finally
            {
                RootPanel.IsEnabled = true;
                SendingPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task MultipleSendAsync(string text, CancellationToken token)
        {
            var sentChats = new List<string>();
            while (!token.IsCancellationRequested)
            {
                var current = QQChat.Find().FirstOrDefault();
                if (current is null)
                {
                    await Task.Delay(200, token);
                }
                else if (sentChats.Find(x => x == current.Name) == null)
                {
                    sentChats.Add(current.Name);
                    CurrentSendingRun.Text = current.Name;
                    HasSentRun.Text = (int.Parse(HasSentRun.Text) + 1).ToString();
                    ToSendRun.Text = (int.Parse(ToSendRun.Text) + 1).ToString();
                    await current.SendMessageAsync(text);
                }
            }
        }
    }
}