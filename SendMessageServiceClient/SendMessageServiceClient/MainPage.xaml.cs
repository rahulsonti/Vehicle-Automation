using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Azure.Devices;
using System.Threading.Tasks;
using System.Text;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SendMessageServiceClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static ServiceClient serviceClient;
        const string connectionString = "HostName=VEHICLE84.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=sfUda3XGnkCMtdb29XK9xy9Xd2WsTnewOQogv2t5rvk=";

        public MainPage()
        {
            this.InitializeComponent();
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

        }


        private void forward_Click(object sender, RoutedEventArgs e)
        {
            SendCloudToDeviceMessageAsync("DB410C", "forward");
        }

        private void backward_Click(object sender, RoutedEventArgs e)
        {
            SendCloudToDeviceMessageAsync("DB410C", "backward");
        }

        private void right_Click(object sender, RoutedEventArgs e)
        {
            SendCloudToDeviceMessageAsync("DB410C", "right");
        }

        private void left_Click(object sender, RoutedEventArgs e)
        {
            SendCloudToDeviceMessageAsync("DB410C", "left");
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            SendCloudToDeviceMessageAsync("DB410C", "stop");
        }
        public async static Task SendCloudToDeviceMessageAsync(string destination, string message)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            await serviceClient.SendAsync(destination, commandMessage);
        }

        
    }
}
