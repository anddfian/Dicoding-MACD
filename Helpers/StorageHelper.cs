using BooksCatalogueAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
 
 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
 
 
namespace BooksCatalogueAPI.Helpers
{
    public static class StorageHelper
    {
 
 
        public static bool IsImage(IFormFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }
 
 
            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };
 
 
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
 
 
        public static async Task<string> UploadFileToStorage(Stream fileStream, string fileName, AzureStorageConfig _storageConfig)
        {
            // Membuat object storage credentials dengan nilai dari berkas konfigurasi (appsettings.json)
            StorageCredentials storageCredentials = new StorageCredentials(_storageConfig.AccountName, _storageConfig.AccountKey);
 
 
            // Membuat cloud storage account berdasarkan storage credentials
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
 
 
            // Membuat client blob
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
 
 
            // Mendapatkan reference blob container berdasarkan nama pada konfigurasi (appsettings.json)
            CloudBlobContainer container = blobClient.GetContainerReference(_storageConfig.ImageContainer);
 
 
            // Mendapatkan reference block blob dari container
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.Properties.ContentType = GetFileContentType(fileName);
 
 
            // Mengunggah berkas
            await blockBlob.UploadFromStreamAsync(fileStream);
 
 
            return await Task.FromResult(blockBlob.Uri.AbsoluteUri);
        }
 
 
        public static string GetFileContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType = String.Empty;
 
 
            if(!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
 
 
            return contentType;
        }
    }
}