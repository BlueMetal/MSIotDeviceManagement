using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using System.Diagnostics;
using System.Security.Cryptography;
using RestSharp;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace MS.IoT.Repositories
{
    public class BlobStorageRepository : IBlobStorageRepository
    {
        private static string _blobConnectionString;

        public BlobStorageRepository(string blobConnectionString)
        {
            _blobConnectionString = blobConnectionString;
        }

        public async Task<System.Web.HttpResponse> DownloadBlobZip(string containerName,string folderpath,string configXml)
        {
            try
            {
                var container = GetBlobContainer(containerName);
                System.Web.HttpResponse Response = HttpContext.Current.Response;
                using (var zipOutputStream = new ZipOutputStream(Response.OutputStream))
                {
                    var blobs = container.ListBlobs(useFlatBlobListing: true);
                    var blobNames = blobs.OfType<CloudBlockBlob>().Select(b => b.Name).ToList();
                    foreach (var blobName in blobNames)
                    {
                        var blobPathNameSplit = blobName.Split('/');
                        if (blobPathNameSplit.First().Equals(folderpath))
                        {
                            zipOutputStream.SetLevel(0);
                            var blob = container.GetBlockBlobReference(blobName);
                            var entry = new ZipEntry(blobPathNameSplit.Last());
                            zipOutputStream.PutNextEntry(entry);
                            blob.DownloadToStream(zipOutputStream);
                        }                      
                    }
                   
                    // add config file for simulator
                    if (configXml!=null)
                    {
                        var configEntry = new ZipEntry("Config.xml");
                        zipOutputStream.PutNextEntry(configEntry);
                        byte[] toBytes = Encoding.ASCII.GetBytes(configXml);
                        zipOutputStream.Write(toBytes, 0, toBytes.Length);
                    }                 

                    zipOutputStream.Finish();
                    zipOutputStream.Close();
                }
                Response.BufferOutput = false;
                Response.AddHeader("Content-Disposition", "attachment; filename=" + folderpath + ".zip");
                Response.ContentType = "application/octet-stream";
                Response.Flush();
                Response.End();
                return Response;
            }
            catch (Exception e)
            {
                throw e;
            }            
        }


        public static CloudBlobContainer GetBlobContainer(string containerName)
        {
            // Create blob client and return reference to the container
            var blobStorageAccount = CloudStorageAccount.Parse(_blobConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }
    }
}
