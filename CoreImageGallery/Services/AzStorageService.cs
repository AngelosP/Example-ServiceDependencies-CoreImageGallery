using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ImageGallery.Model;
using CoreImageGallery.Data;
using CoreImageGallery.Extensions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;

namespace CoreImageGallery.Services
{
    public class AzStorageService : IStorageService
    {
        private static bool ResourcesInitialized { get; set; } = false;

        private const string ImagePrefix = "img_";
        private BlobContainerClient _uploadContainer;
        private BlobContainerClient _watermarkContainer;

        private ApplicationDbContext _dbContext;

        public AzStorageService(IConfiguration config, ApplicationDbContext dbContext, BlobServiceClient client)
        {
            _uploadContainer = client.GetBlobContainerClient(Config.UploadContainer);
            _watermarkContainer = client.GetBlobContainerClient(Config.WatermarkContainer);

            // OLD SDK
            //_uploadContainer = client.GetContainerReference(Config.UploadContainer);
            //_publicContainer = client.GetContainerReference(Config.WatermarkedContainer);

            _dbContext = dbContext;
        }

        public async Task AddImageAsync(Stream stream, string originalName, string userName)
        {
            await InitializeResourcesAsync();

            UploadUtilities.GetImageProperties(originalName, userName, out string uploadId, out string fileName, out string userId);
            
            _uploadContainer.UploadBlob(fileName, stream);
            var imageBlob = _uploadContainer.GetBlobClient(fileName);
            
            // OLD SDK
            //var imageBlob = _uploadContainer.GetBlockBlobReference(fileName);
            //await imageBlob.UploadFromStreamAsync(stream);

            await UploadUtilities.RecordImageUploadedAsync(_dbContext, uploadId, fileName, imageBlob.Uri.ToString(), userId);
        }

        public async Task<IEnumerable<UploadedImage>> GetImagesAsync()
        {
            await InitializeResourcesAsync();

            var imageList = new List<UploadedImage>();

            foreach (BlobItem blob in _watermarkContainer.GetBlobs(prefix: ImagePrefix))
            {
                var blobClient = _watermarkContainer.GetBlobClient(blob.Name);
                var image = new UploadedImage { ImagePath = blobClient.Uri.ToString() };
                imageList.Add(image);
            }

            // OLD SDK
            //var token = new BlobContinuationToken();
            //var blobList = await _publicContainer.ListBlobsSegmentedAsync(ImagePrefix, true, BlobListingDetails.All, 100, token, null, null);

            //foreach (var blob in blobList.Results)
            //{
            //    var image = new UploadedImage
            //    {
            //        ImagePath = blob.Uri.ToString()
            //    };

            //    imageList.Add(image);
            //}

            return imageList;
        }

        private async Task InitializeResourcesAsync()
        {
            if (!ResourcesInitialized)
            {
                _watermarkContainer.CreateIfNotExists(PublicAccessType.Blob);
                _uploadContainer.CreateIfNotExists();

                // OLD SDK
                //var permissions = await _publicContainer.GetPermissionsAsync();
                //if (permissions.PublicAccess == BlobContainerPublicAccessType.Off || permissions.PublicAccess == BlobContainerPublicAccessType.Unknown)
                //{
                //    // If blob isn't public, we can't directly link to the pictures
                //    await _publicContainer.SetPermissionsAsync(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
                //}

                ResourcesInitialized = true;
            }

        }

    }
}
