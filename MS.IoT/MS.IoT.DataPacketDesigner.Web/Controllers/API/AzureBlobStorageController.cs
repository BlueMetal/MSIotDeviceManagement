using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MS.IoT.Common;
using MS.IoT.DataPacketDesigner.Web.Helpers;
using MS.IoT.Domain.Interface;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Options;
using MS.IoT.DataPacketDesigner.Web.Models;
using MS.IoT.Repositories;
using Microsoft.Extensions.Logging;

namespace MS.IoT.DataPacketDesigner.Web.Controllers.API
{
    /// <summary>
    /// AzureBlobStorageController
    /// Service to communicate with Azure and retrieve data from Azure
    /// </summary>
    [Authorize]
    [Route("api/blob")]
    public class AzureBlobStorageController : BaseApiController
    {
        //Service member
        public readonly IBlobStorageRepository _blobRepo;
        private readonly ILogger<AzureBlobStorageController> logger;
        private readonly CosmosDbOptions cosmostOptions;
        private readonly IoTHubOptions iotOptions;
        private readonly BlobOptions blobOptions;

        /// <summary>
        /// Main Controller
        /// </summary>
        /// <param name="userProfile">User Service</param>
        /// <param name="blobRepo">Blob Repository</param>
        public AzureBlobStorageController(IUserProfileService userProfile, 
                                          IBlobStorageRepository blobRepo,
                                          IOptionsSnapshot<IoTHubOptions> iotOptions,
                                          IOptionsSnapshot<BlobOptions> blobOptions,
                                          IOptionsSnapshot<CosmosDbOptions> cosmostOptions,
                                          ILogger<AzureBlobStorageController> logger)
            : base(userProfile)
        {
            _blobRepo = blobRepo;
            this.logger = logger;
            this.cosmostOptions = cosmostOptions.Value;
            this.iotOptions = iotOptions.Value;
            this.blobOptions = blobOptions.Value;
        }

        /// <summary>
        /// End point to download the simulator
        /// </summary>
        /// <param name="folderpath">Path of the simulator</param>
        /// <returns></returns>
        [HttpGet]
        [Route("download/{folderpath}")]
        public async Task<IActionResult> DownloadSimulatorZip(string folderpath)
        {
            Stream response = null; //The stream must not be closed prior to sending the file to the view

            try
            {
                // if the path is simulator then create a config.xml file of appsettings
                if (folderpath.Equals("simulator"))
                {
                    using (StringWriterWithEncoding sw = new StringWriterWithEncoding(new UTF8Encoding(false)))
                    {
                        CreateXMLConfig(sw);
                        response = await _blobRepo.DownloadBlobZip(
                                                  blobOptions.SimulateToolsContainerName,
                                                  folderpath,
                                                  sw.ToString());
                            return File(response, "application/octet-stream", $"{folderpath}.zip");
                    }
                }
                else
                {
                    response = await _blobRepo.DownloadBlobZip(
                                              blobOptions.SimulateToolsContainerName,
                                              folderpath,
                                              null);
                    return File(response, "application/octet-stream", $"{folderpath}.zip");
                }            
            }
            catch (Exception e)
            {                
                logger.LogError(e, "Download Blob error {error}", e.Message);
                response.Dispose();
                throw;
            }         
        }

        /// <summary>
        /// Create the XML Config file of the simulator automatically
        /// </summary>
        /// <param name="tw"></param>
        public void CreateXMLConfig(TextWriter tw)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = new UTF8Encoding(true);

                using (XmlWriter writer = XmlWriter.Create(tw, settings))
                {
                    writer.WriteStartElement("appsettings");             
                    writer.WriteElementString("CosmosDBEndPoint", cosmostOptions.Endpoint);
                    writer.WriteElementString("CosmosDBAuthKey", cosmostOptions.AuthKey);
                    writer.WriteElementString("IoTHubHostName", iotOptions.HostName);
                    writer.WriteElementString("IoTHubConnectionString", iotOptions.ConnectionString);
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
                Log.Error("Download Blob error {error}: ", e.Message);             
            }
        }
    }

    // to encode XML in utf-8
    public sealed class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;

        public StringWriterWithEncoding(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return encoding; }
        }
    }
}