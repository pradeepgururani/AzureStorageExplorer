using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageExplorer
{
    public class Table
    {
        public string Name { get; set; }
        public Uri PrimaryUri { get; set; }
        public Uri SecondaryUri { get; set; }
    }
}
