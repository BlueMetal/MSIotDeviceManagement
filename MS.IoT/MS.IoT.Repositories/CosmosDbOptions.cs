using System;
using System.Collections.Generic;
using System.Text;

namespace MS.IoT.Repositories
{
    public class CosmosDbOptions
    {
        public string Endpoint { get; set; }
        public string AuthKey { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }


        public CosmosDbCollections Collections { get; set; }

        public string TemplatesBase { get; set; }
    }

    public class CosmosDbCollections
    {
        public string Templates { get; set; }   
        public string Messages { get; set; }
        public string Devices { get; set; }
        public string Groups { get; set; }
    }
}
