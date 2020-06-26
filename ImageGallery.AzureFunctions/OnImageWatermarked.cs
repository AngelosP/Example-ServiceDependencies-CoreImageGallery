using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using ImageGallery.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ImageGallery.AzureFunctions
{
    public class OnImageWatermarked
    {
        private BlobServiceClient _blobClient;
        public OnImageWatermarked(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        [FunctionName("OnImageWatermarked")]
        public async Task Run([BlobTrigger(Config.WatermarkContainer + "/{name}",
            Connection = "AzureWebJobsStorage")]Stream myBlob,
            string name,
            ILogger log)
        {
            var container = _blobClient.GetBlobContainerClient(Config.WatermarkContainer);
            var originalImage = container.GetBlobClient(name);
            await originalImage.DeleteIfExistsAsync();

            log.LogInformation("Deleted original: {0}", name);
        }
    }
}
