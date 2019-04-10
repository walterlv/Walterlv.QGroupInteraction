using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Walterlv.EasiPlugins.Configurations;

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
        private CancellationTokenSource _sendSource;
        private CancellationTokenSource _whitelistSource;
        private readonly List<string> _whiteList = new List<string>();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _current = QQChat.Find().FirstOrDefault();
            GroupNameTextBlock.Text = _current?.Name;

            var whiteList = DefaultConfiguration.FromFile("configs.txt")["WhiteList"].ToString()
                .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            _whiteList.AddRange(whiteList);
        }

        private async void WhiteListButton_Click(object sender, RoutedEventArgs e)
        {
            if (_whitelistSource != null)
            {
                _whitelistSource.Cancel();
                _whitelistSource = null;
            }
            else
            {
                SingleSendButton.IsEnabled = false;
                MultipleSendButton.IsEnabled = false;
                _whitelistSource = new CancellationTokenSource();
                try
                {
                    await EditWhiteListAsync(MessageTextBox.Text, _whitelistSource.Token);
                }
                finally
                {
                    SingleSendButton.IsEnabled = true;
                    MultipleSendButton.IsEnabled = true;
                }
            }
        }

        private async Task EditWhiteListAsync(string text, CancellationToken token)
        {
            MessageTextBox.Text = string.Join(Environment.NewLine, _whiteList);

            while (!token.IsCancellationRequested)
            {
                var whiteList = MessageTextBox.Text
                    .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).ToList();
                var current = QQChat.Find().FirstOrDefault();
                if (current is null)
                {
                    await Task.Delay(200);
                }
                else if (whiteList.Find(x => x == current.Name) == null)
                {
                    whiteList.Add(current.Name);
                    MessageTextBox.Text = string.Join(Environment.NewLine, whiteList);
                    await Task.Delay(200, token);
                }
                else
                {
                    await Task.Delay(200);
                }
            }

            _whiteList.Clear();
            _whiteList.AddRange(MessageTextBox.Text.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries));
            DefaultConfiguration.FromFile("configs.txt")["WhiteList"] = string.Join("\n", _whiteList);
            MessageTextBox.Text = "";
        }

        private void SendSingleButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageTextBox.Text.Trim().Length <= 0)
            {
                return;
            }

            _current?.SendMessageAsync(MessageTextBox.Text);
        }

        private async void SendMultipleButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageTextBox.Text.Trim().Length <= 0)
            {
                return;
            }

            RootPanel.IsEnabled = false;
            SendingPanel.Visibility = Visibility.Visible;
            try
            {
                _sendSource = new CancellationTokenSource();
                await MultipleSendAsync(MessageTextBox.Text, _sendSource.Token);
            }
            finally
            {
                RootPanel.IsEnabled = true;
                SendingPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            _sendSource?.Cancel();
        }

        private async Task MultipleSendAsync(string text, CancellationToken token)
        {
            var sentChats = new List<string>();
            while (!token.IsCancellationRequested)
            {
                var current = QQChat.Find().FirstOrDefault();
                if (current is null)
                {
                    await Task.Delay(200);
                }
                else if (sentChats.Find(x => x == current.Name) == null
                         && (_whiteList.Count == 0 || _whiteList.Contains(current.Name)))
                {
                    sentChats.Add(current.Name);
                    CurrentSendingRun.Text = current.Name;
                    HasSentRun.Text = (int.Parse(HasSentRun.Text) + 1).ToString();
                    ToSendRun.Text = (int.Parse(ToSendRun.Text) + 1).ToString();
                    await current.SendMessageAsync(text);
                }
                else
                {
                    await Task.Delay(200);
                }
            }
        }
    }
}