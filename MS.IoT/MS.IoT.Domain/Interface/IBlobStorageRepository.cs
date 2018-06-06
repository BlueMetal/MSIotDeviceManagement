using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Interface
{
    public interface IBlobStorageRepository
    {
        Task<Stream> DownloadBlobZip(string containerName,string folderpath,string configXml);
    }
}
