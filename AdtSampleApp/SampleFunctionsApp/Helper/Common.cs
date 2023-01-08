using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleFunctionsApp.Helper
{
    public class Common
    {
        public static async Task CreateCSVFileConsole(DataTable table, string fileName)
        {
            Console.WriteLine(" :===: Task : Creating CSV file :===: ");
            try
            {
                StringBuilder sb = new StringBuilder();

                IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in table.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }

                await File.WriteAllTextAsync(fileName, sb.ToString());
                Console.WriteLine(" :===:  Task : Creating CSV file has Completed :===: ");
            }
            catch (Exception ex)
            {
                Console.WriteLine(" :===: Task :  Task : Creating CSV file is Failed with  :===: " + ex);
            }
        }
        public static async Task CreateFile(DataTable table, string fileName, ILogger log=null)
        {
            log.LogInformation(" :===: Task : PerformOperation --> 1.1 CreateFile has Started :===: ");
            try
            {
                StringBuilder sb = new StringBuilder();

                IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in table.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }

                await File.WriteAllTextAsync(fileName, sb.ToString());
                log.LogInformation(" :===: Task : PerformOperation --> 1.1 CreateFile has Completed :===: ");
            }
            catch (Exception ex)
            {
                log.LogError(" :===: Task : PerformOperation Failed --> 1.1 CreateFile Failed with  :===: " + ex);
            }
        }

        public static async Task SaveFileToBlobConsole( string fileName, string filepath=null, ILogger log=null )
        {

            string blobconnection = "DefaultEndpointsProtocol=https;AccountName=sagreenhouse;AccountKey=7/aG8qXX5USnLw5OP0fpY26Itg9DnwUIMyIIiyQTR3dtNF0nKw2USF63j9K4h6iOktAyfoTRm802BtlnIX9joA==;EndpointSuffix=core.windows.net";
            string containerName = "job";
            //string blobconnection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            //string containerName = Environment.GetEnvironmentVariable("ContainerName");
            Console.WriteLine(" :===: Task : Upload to Azure blob storage  has Started :===: ");
            //string blobconnection = "DefaultEndpointsProtocol=https;AccountName=storageaccountazarc;AccountKey=wKQKBjJdcwH+9t0/Q5ZKaUlihxyRo7wQf8RAqZ479cu4zxr4k5AF5Gh+sHKiDV1cjRX0y4nmHYKz+AStKWJ2Lg==;EndpointSuffix=core.windows.net";
            try
            {

                // START : Code to upload to blob 
                BlobServiceClient blobServiceClient = new BlobServiceClient(blobconnection);

                bool bContainerExists = false;
                Azure.Storage.Blobs.BlobContainerClient containerClient = null;
                Azure.Storage.Blobs.Models.BlobContainerItem containerItem = null;

                // Create the container and return a container client object
                foreach (Azure.Storage.Blobs.Models.BlobContainerItem
                blobContainerItem in blobServiceClient.GetBlobContainers())
                {
                    if (blobContainerItem.Name == containerName)
                    {
                        bContainerExists = true;
                        containerItem = blobContainerItem;
                        break;
                    }
                }

                // Create or use existing Azure container as client.
                if (!bContainerExists)
                {
                    containerClient = blobServiceClient.CreateBlobContainer(
                 containerName);
                }
                else
                    containerClient = blobServiceClient.GetBlobContainerClient(
                 containerName);
                // Get a reference to a blob
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                Console.WriteLine(" Uploading to Blob storage as blob:\n\t {0}\n :===: ", blobClient.Uri);

                // Upload data from the local file
                //await blobClient.UploadAsync(filepath, true);
                await blobClient.UploadAsync(fileName, true);
                Console.WriteLine(":===: Task : Upload to Azure blob storage   has Completed :===: ");
            }
            catch (Exception ex)
            {
                Console.WriteLine(":===: Task : Upload to Azure blob storage   has failed :===:  " + ex);
            }

        }
        //private static async Task SaveFileToBlob(string filepath, ILogger log, string fileName)
        //{
        //    string blobconnection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        //    string containerName = Environment.GetEnvironmentVariable("ContainerName");
        //    //log.LogInformation(" :===: Task : PerformOperation --> 1.2 SaveFileToBlob has Started :===: ");
        //    //string blobconnection = "DefaultEndpointsProtocol=https;AccountName=storageaccountazarc;AccountKey=wKQKBjJdcwH+9t0/Q5ZKaUlihxyRo7wQf8RAqZ479cu4zxr4k5AF5Gh+sHKiDV1cjRX0y4nmHYKz+AStKWJ2Lg==;EndpointSuffix=core.windows.net";
        //    try
        //    {

        //        // START : Code to upload to blob 
        //        BlobServiceClient blobServiceClient = new BlobServiceClient(blobconnection);

        //        bool bContainerExists = false;
        //        Azure.Storage.Blobs.BlobContainerClient containerClient = null;
        //        Azure.Storage.Blobs.Models.BlobContainerItem containerItem = null;

        //        // Create the container and return a container client object
        //        foreach (Azure.Storage.Blobs.Models.BlobContainerItem
        //        blobContainerItem in blobServiceClient.GetBlobContainers())
        //        {
        //            if (blobContainerItem.Name == containerName)
        //            {
        //                bContainerExists = true;
        //                containerItem = blobContainerItem;
        //                break;
        //            }
        //        }

        //        // Create or use existing Azure container as client.
        //        if (!bContainerExists)
        //        {
        //            containerClient = blobServiceClient.CreateBlobContainer(
        //         containerName);
        //        }
        //        else
        //            containerClient = blobServiceClient.GetBlobContainerClient(
        //         containerName);
        //        // Get a reference to a blob
        //        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        //        log.LogInformation(" Uploading to Blob storage as blob:\n\t {0}\n :===: ", blobClient.Uri);

        //        // Upload data from the local file
        //        await blobClient.UploadAsync(filepath, true);
        //        //log.LogInformation(" :===: Task : PerformOperation --> 1.2 SaveFileToBlob has Completed :===: ");
        //    }
        //    catch (Exception ex)
        //    {
        //        //log.LogError(" :===: Task : PerformOperation Failed --> 1.2 SaveFileToBlob Failed with  :===: " + ex);
        //    }

        //}
    }
}
