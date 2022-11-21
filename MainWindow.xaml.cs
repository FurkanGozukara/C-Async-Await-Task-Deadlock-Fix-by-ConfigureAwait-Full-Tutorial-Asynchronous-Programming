using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace async_programming
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnTestAsyncDeadLock_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("inside btnTestAsyncDeadLock_Click " + Thread.CurrentThread.ManagedThreadId);
            txtBlockSourceCode.Text = returnSourceCode("https://www.monstermmorpg.com", true).Result;
        }

        private async Task<string> returnSourceCode(string srUrl, bool blConfigureAwait = false)
        {
            Debug.WriteLine("inside returnSourceCode " + Thread.CurrentThread.ManagedThreadId);
            using (HttpClient client = new HttpClient())
            {
                Debug.WriteLine("inside using HttpClient client " + Thread.CurrentThread.ManagedThreadId);

                using (var responseOfRequest = await client.GetAsync(srUrl).ConfigureAwait(blConfigureAwait))
                {
                    Debug.WriteLine("inside  using (var responseOfRequest " + Thread.CurrentThread.ManagedThreadId);

                    return await responseOfRequest.Content.ReadAsStringAsync();
                }
            }
        }

        private void btnConfigureAwaitAsyncCode_Click(object sender, RoutedEventArgs e)
        {
            txtBlockSourceCode.Text = returnSourceCode("https://www.monstermmorpg.com", false).Result;
        }

        private async void btnResponsiveAsyncClick_Click(object sender, RoutedEventArgs e)
        {
            txtBlockSourceCode.Text = await returnSourceCode("https://www.monstermmorpg.com", false);
        }

        private void btnTaskExample_Click(object sender, RoutedEventArgs e)
        {
            string source = "";
            var task = Task.Factory.StartNew(() =>
            {
                Debug.WriteLine("inside btnTaskExample_Click " + Thread.CurrentThread.ManagedThreadId);
                source = returnSourceCode("https://www.monstermmorpg.com").Result;
            });

            task.Wait();

            txtBlockSourceCode.Text = source;
        }

        private void btnTaskVersion2_Click(object sender, RoutedEventArgs e)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Debug.WriteLine("inside btnTaskExample_Click " + Thread.CurrentThread.ManagedThreadId);
                return returnSourceCode("https://www.monstermmorpg.com").Result;
            }).ContinueWith((t1) =>
            {
                txtBlockSourceCode.Dispatcher.BeginInvoke(new Action(() =>
                {
                    txtBlockSourceCode.Text = t1.Result;
                }));
            });
        }
    }
}
