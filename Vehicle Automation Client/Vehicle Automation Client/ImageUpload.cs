using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using Windows.Storage;



namespace Vehicle_Automation_Client
{
   static class ImageUpload
    {
        private static readonly StorageCredentials cred = new StorageCredentials("hotwheel", "BWH1YqsdsFSxHLrwEl75FOXme1EMapiYDopjDnUNjGpvGlPM5ZKrD28fL03D1+0iEOqK/Ja4whv3y2l8TfrMwQ==");
        public static readonly CloudBlobContainer container = new CloudBlobContainer(new Uri("https://hotwheel.blob.core.windows.net/vehicleimage"), cred);

        public static async Task UploadToAzure(StorageFile file, string blobname)
        {
            // Create the container if it doesn't already exist.
            await container.CreateIfNotExistsAsync();

            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobname);
            await blockBlob.UploadFromFileAsync(file);

        }

    }
}
