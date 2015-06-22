using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageExplorer
{
    public class Blob
    {
        public string Name { get; set; }

        public Uri PrimaryUri { get; set; }
        public Uri SecondaryUri { get; set; }

        public BlobContainerPermissions Permissions { get; set; }
    }
}
