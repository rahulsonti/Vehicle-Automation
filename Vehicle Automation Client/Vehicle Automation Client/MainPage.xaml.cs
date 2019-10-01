using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Devices.Gpio;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Vehicle_Automation_Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaCapture mediaCapture = null;
        private StorageFile photoFile;
        private readonly string PHOTO_FILE_NAME = "photo.jpg";
        private const int forwardpinnumber = 12;
        private const int backwardpinnumber = 69;
        private const int rightpinnumber = 25;
        private const int leftpinnumber = 34;

        private GpioPin forward;
        private GpioPin backward;
        private GpioPin right;
        private GpioPin left;

        private GpioPinValue forwardpinvalue;
        private GpioPinValue backwardpinvalue;
        private GpioPinValue rightpinvalue;
        private GpioPinValue leftpinvalue;
        public MainPage()
        {
            this.InitializeComponent();
            InitializeCamera();
             InitGPIO(); 
            RecieveAlert();
        }
        private async void onClick(object sender, RoutedEventArgs e)  
        {
            await AzureIoTHub.SendDeviceToCloudMessageAsync();
        }
        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                // GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            forward = gpio.OpenPin(forwardpinnumber);
            forward.SetDriveMode(GpioPinDriveMode.Output);
            forwardpinvalue = GpioPinValue.Low;
            forward.Write(forwardpinvalue);

            backward = gpio.OpenPin(backwardpinnumber);
            backward.SetDriveMode(GpioPinDriveMode.Output);
            backwardpinvalue = GpioPinValue.Low;
            backward.Write(backwardpinvalue);

            right = gpio.OpenPin(rightpinnumber);
            right.SetDriveMode(GpioPinDriveMode.Output);
            rightpinvalue = GpioPinValue.Low;
            right.Write(rightpinvalue);

            left = gpio.OpenPin(leftpinnumber);
            left.SetDriveMode(GpioPinDriveMode.Output);
            leftpinvalue = GpioPinValue.Low;
            left.Write(leftpinvalue);


            //GpioStatus.Text = "GPIO pins initialized correctly.";
        }

        public async Task RecieveAlert()
        {
            string value = await AzureIoTHub.ReceiveCloudToDeviceMessageAsync();
            RecieveMessageStatus.Text = value;
            if (value.Equals("Capture"))
            {
                ImageCapture();
            }
           if (value.Equals("forward"))
            {
              moveforward();
            }
            if (value.Equals("backward"))
            {
              movebackward();
            }
            if (value.Equals("right"))
            {
              moveright();
            }
            if (value.Equals("left"))
            {
              moveleft();
            }
            if (value.Equals("stop"))
            {
              stop();
            }
            await RecieveAlert();
        }


        public async void InitializeCamera()
        {
            status.Text = "Initializing camera to capture Image";
            try
            {
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();


                // To  get Camera Preview on UI               
                previewElement.Source = mediaCapture;
                await mediaCapture.StartPreviewAsync();
                status.Text = "Camera preview succeeded";


            }
            catch (Exception ex)
            {
                status.Text = "Unable to initialize camera : " + ex.Message;
            }
        }

        public async void ImageCapture()
        {
            try
            {
                captureImage.Source = null;

                photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync(
                    PHOTO_FILE_NAME, CreationCollisionOption.GenerateUniqueName);
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);

                status.Text = "Take Photo succeeded: " + photoFile.Path;
                //To preview capture image on UI
                IRandomAccessStream photoStream = await photoFile.OpenReadAsync();
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(photoStream);
                captureImage.Source = bitmap;

                //To send Image to cloud
                await ImageUpload.UploadToAzure(photoFile, "mypic");


            }
            catch (Exception ex)
            {
                status.Text = ex.Message;

            }




        }
        public void movebackward()
        {
            forwardpinvalue = GpioPinValue.High;
            forward.Write(forwardpinvalue);
            backwardpinvalue = GpioPinValue.Low;
            backward.Write(backwardpinvalue);
            rightpinvalue = GpioPinValue.High;
            right.Write(rightpinvalue);
            leftpinvalue = GpioPinValue.Low;
            left.Write(leftpinvalue);
        }
        public void moveforward()
        {
            forwardpinvalue = GpioPinValue.Low;
            forward.Write(forwardpinvalue);
            backwardpinvalue = GpioPinValue.High;
            backward.Write(backwardpinvalue);
            rightpinvalue = GpioPinValue.Low;
            right.Write(rightpinvalue);
            leftpinvalue = GpioPinValue.High;
            left.Write(leftpinvalue);
        }
        public void moveright()
        {
            forwardpinvalue = GpioPinValue.Low;
            forward.Write(forwardpinvalue);
            backwardpinvalue = GpioPinValue.High;
            backward.Write(backwardpinvalue);
            rightpinvalue = GpioPinValue.Low;
            right.Write(rightpinvalue);
            leftpinvalue = GpioPinValue.Low;
            left.Write(leftpinvalue);

        }
        public void moveleft()
        {
            forwardpinvalue = GpioPinValue.Low;
            forward.Write(forwardpinvalue);
            backwardpinvalue = GpioPinValue.Low;
            backward.Write(backwardpinvalue);
            rightpinvalue = GpioPinValue.Low;
            right.Write(rightpinvalue);
            leftpinvalue = GpioPinValue.High;
            left.Write(leftpinvalue);
        }
        public void stop()
        {
            forwardpinvalue = GpioPinValue.Low;
            forward.Write(forwardpinvalue);
            backwardpinvalue = GpioPinValue.Low;
            backward.Write(backwardpinvalue);
            rightpinvalue = GpioPinValue.Low;
            right.Write(rightpinvalue);
            leftpinvalue = GpioPinValue.Low;
            left.Write(leftpinvalue);
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            await AzureIoTHub.SendDeviceToCloudMessageAsync();
            ImageCapture();
        }
    }
}
