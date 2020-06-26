using System;
using System.IO;
using ImageGallery.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ImageGallery.AzureFunctions
{
    public static class OnImageUploaded
    {
        const string WaterMarkText = "(c) CoreImageGallery";

        [FunctionName("OnImageUploaded")]
        public static void Run([BlobTrigger(Config.UploadContainer + "/{name}")] Stream inputBlob,
                               [Blob(Config.WatermarkContainer + "/{name}", FileAccess.Write)] Stream outputBlob,
                               string name,
                               ILogger log)
        {
            try
            {
                WaterMarker.WriteWatermark(WaterMarkText, inputBlob, outputBlob);
                log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {inputBlob.Length} Bytes");
            }
            catch (Exception e)
            {
                log.LogError($"Watermaking failed {e.Message}");
            }
        }
    }
}
