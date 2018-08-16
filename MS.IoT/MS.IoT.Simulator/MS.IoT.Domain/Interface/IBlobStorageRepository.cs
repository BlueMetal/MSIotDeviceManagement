using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MS.IoT.Domain.Interface
{
    public interface IBlobStorageRepository
    {
        Task<System.Web.HttpResponse> DownloadBlobZip(string containerName,string folderpath,string configXml);
    }
}
