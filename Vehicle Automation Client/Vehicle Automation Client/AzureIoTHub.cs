using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

class AzureIoTHub
{
    private static void CreateClient()
    {
        if (deviceClient == null)
        {
            // create Azure IoT Hub client from embedded connection string
            deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
        }
    }

    static DeviceClient deviceClient = null;

    //
    // Note: this connection string is specific to the device "IOT". To configure other devices,
    // see information on iothub-explorer at http://aka.ms/iothubgetstartedVSCS
    //
    const string deviceConnectionString = "HostName=VEHICLE84.azure-devices.net;DeviceId=DB410C;SharedAccessKey=+chjWtUyCUtwzaUWPJp2XUY5lIyWSNOWx2jyGY+T9RI=";


    //
    // To monitor messages sent to device "kraaa" use iothub-explorer as follows:
    //    iothub-explorer monitor-events --login HostName=DRAGONBOARD.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=XwSSLiIXIBAzpVXVD+z8reDK35/qjZB2a9NdB+lWr4Y= "IOT"
    //

    

    public static async Task SendDeviceToCloudMessageAsync()
    {
        CreateClient();
#if WINDOWS_UWP
        var str = "{\"deviceId\":\"DB410C\",\"messageId\":1,\"text\":\"Hello, Cloud from a UWP C# app!\"}";
#else
        var str = "{\"deviceId\":\"DB410C\",\"messageId\":1,\"text\":\"Hello, Cloud from a C# app!\"}";
#endif
        var message = new Message(Encoding.ASCII.GetBytes(str));

        await deviceClient.SendEventAsync(message);
    }

    public static async Task<string> ReceiveCloudToDeviceMessageAsync()
    {
        CreateClient();

        while (true)
        {
            var receivedMessage = await deviceClient.ReceiveAsync();

            if (receivedMessage != null)
            {
                var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                await deviceClient.CompleteAsync(receivedMessage);
                return messageData;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

    
}
