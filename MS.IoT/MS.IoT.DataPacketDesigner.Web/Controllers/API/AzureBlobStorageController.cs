using MS.IoT.Common;
using MS.IoT.DataPacketDesigner.Web.Helpers;
using MS.IoT.Domain.Interface;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;

namespace MS.IoT.DataPacketDesigner.Web.Controllers.API
{
    /// <summary>
    /// AzureBlobStorageController
    /// Service to communicate with Azure and retrieve data from Azure
    /// </summary>
    [Authorize]
    [RoutePrefix("api/blob")]
    public class AzureBlobStorageController : BaseApiController
    {
        //Service member
        public readonly IBlobStorageRepository _blobRepo;

        /// <summary>
        /// Main Controller
        /// </summary>
        /// <param name="userProfile">User Service</param>
        /// <param name="blobRepo">Blob Repository</param>
        public AzureBlobStorageController(IUserProfileService userProfile, IBlobStorageRepository blobRepo)
            : base(userProfile)
        {
            _blobRepo = blobRepo;
        }

        /// <summary>
        /// End point to download the simulator
        /// </summary>
        /// <param name="folderpath">Path of the simulator</param>
        /// <returns></returns>
        [HttpGet]
        [Route("download/{folderpath}")]
        public async Task<IHttpActionResult> DownloadSimulatorZip(string folderpath)
        {
            try
            {
                // if the path is simulator then create a config.xml file of appsettings
                if (folderpath.Equals("simulator"))
                {
                    using (StringWriterWithEncoding sw = new StringWriterWithEncoding(new UTF8Encoding(false)))
                    {
                        CreateXMLConfig(sw);
                        var response = await _blobRepo.DownloadBlobZip(
                            AppConfig.ConfigurationItems.SimulateToolsContainerName, folderpath, sw.ToString());
                        return Ok(response);
                    }
                }
                else
                {
                    var response = await _blobRepo.DownloadBlobZip(
                    AppConfig.ConfigurationItems.SimulateToolsContainerName, folderpath,null);
                    return Ok(response);
                }            
            }
            catch (Exception e)
            {                
                Log.Error("Download Blob error {error}: ",e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
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
                    writer.WriteElementString("CosmosDBEndPoint", AppConfig.ConfigurationItems.EndPoint);
                    writer.WriteElementString("CosmosDBAuthKey", AppConfig.ConfigurationItems.AuthKey);
                    writer.WriteElementString("IoTHubHostName", AppConfig.ConfigurationItems.IoTHubHostName);
                    writer.WriteElementString("IoTHubConnectionString", AppConfig.ConfigurationItems.IoTHubConnectionString);
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