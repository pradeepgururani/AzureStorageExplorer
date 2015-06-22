using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;

namespace AzureStorageExplorer
{
    [Serializable]
    public class ExtendedStorageView
    {
        public string AccountName { get; set; }

        //public CloudBlobClient blobClient = null;
        //public CloudTableClient tableClient = null;
        //public CloudQueueClient queueClient = null;

        public ServiceProperties BlobProperties { get; set; }
        //public CloudBlobContainer LogContainer { get; set; }

        public List<Blob> Containers { get; set; }

        public List<Queue> Queues { get; set; }

        public List<Table> Tables { get; set; }

        public void LoadAccountDetails(AzureAccount azureAccount)
        {
            CloudStorageAccount account = OpenStorageAccount(azureAccount);

            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudQueueClient queueClient = account.CreateCloudQueueClient();

            #region ServiceProperties
            try
            {
                BlobProperties = blobClient.GetServiceProperties();
            }
            catch (Exception)
            {
                // Disallowed for developer storage account.
            }
            #endregion ServiceProperties

            #region Blob
            try
            {
                // Check for $logs container and add it if present ($logs is not included in the general ListContainers call).

                //LogContainer = blobClient.GetContainerReference("$logs");

                List<CloudBlobContainer> CloudContainers = blobClient.ListContainers().ToList();
                Containers = new List<Blob>();

                CloudContainers.ForEach((cc) => {
                    Containers.Add(new Blob { Name = cc.Name, PrimaryUri = cc.StorageUri.PrimaryUri,
                        SecondaryUri = cc.StorageUri.SecondaryUri, Permissions = cc.GetPermissions() });
                });
            }
            catch (Exception ex)
            {
                //ShowError("Error enumering blob containers in the storage account: " + ex.Message);
            }
            #endregion Blob

            #region Queues
            try
            {
                List<CloudQueue> CloudQueues = queueClient.ListQueues().ToList();
                Queues = new List<Queue>();

                CloudQueues.ForEach((cq) => {
                    
                    Queues.Add(new Queue { Name = cq.Name, PrimaryUri = cq.StorageUri.PrimaryUri, SecondaryUri = cq.StorageUri.SecondaryUri });
                });
            }
            catch (Exception ex)
            {
                //ShowError("Error enumering queues in storage account: " + ex.Message);
            }
            #endregion Queues

            // OData version number occurs here:
            // Could not load file or assembly 'Microsoft.Data.OData, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' or one of its dependencies. The system cannot find the file specified.

            #region Tables
            try
            {
                List<CloudTable> CloudTables = tableClient.ListTables().ToList();
                Tables = new List<Table>();

                CloudTables.ForEach((ct) => {
                    Tables.Add(new Table { Name = ct.Name, PrimaryUri = ct.StorageUri.PrimaryUri, SecondaryUri = ct.StorageUri.SecondaryUri });
                });
            }
            catch (Exception ex)
            {
                //ShowError("Error enumerating tables in storage account: " + ex.Message);
            }
            #endregion Tables
        }

        private CloudStorageAccount OpenStorageAccount(AzureAccount azureAccount)
        {
            CloudStorageAccount account = null;

            if (azureAccount.IsDeveloperAccount)
            {
                account = CloudStorageAccount.DevelopmentStorageAccount;
            }
            else
            {
                account = new CloudStorageAccount(new StorageCredentials(azureAccount.Name, azureAccount.Key), azureAccount.EndpointDomain, azureAccount.UseSSL);
            }

            return account;
        }
    }
}
