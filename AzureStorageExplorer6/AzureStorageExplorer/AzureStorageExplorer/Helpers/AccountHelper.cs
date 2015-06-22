using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageExplorer.Helpers
{
    public class AccountHelper
    {
        public static List<ExtendedStorageView> LoadAccountData()
        {
            List<ExtendedStorageView> accountData = null;

            string fileName = System.Windows.Forms.Application.UserAppDataPath + "\\AccountData.dt1";

            if (File.Exists(fileName))
            {
                //accountData = Deserialize<List<ExtendedStorageView>>(fileName);

                string fileContent = File.ReadAllText(fileName);

                if (!string.IsNullOrWhiteSpace(fileContent))
                {
                    accountData = JsonConvert.DeserializeObject<List<ExtendedStorageView>>(fileContent);
                }
            }
            else
            { accountData = new List<ExtendedStorageView>(); }

            return accountData;
        }

        public static void SaveAccountData(List<ExtendedStorageView> accountData)
        {
            string fileName = System.Windows.Forms.Application.UserAppDataPath + "\\AccountData.dt1";

            var storageContent = JsonConvert.SerializeObject(accountData);

            File.WriteAllText(fileName, storageContent);

            //SerializeAndWrite(accountData, fileName);
        }

        public static ExtendedStorageView GetCloudAccountDetails(string accountName)
        {
            var azureAccount = GetAzureAccountByName(accountName);

            ExtendedStorageView storage = null;

            if (azureAccount != null)
            {
                storage = new ExtendedStorageView();

                storage.AccountName = accountName;
                storage.LoadAccountDetails(azureAccount);
            }

            return storage;
        }

        public static AzureAccount GetAzureAccountByName(string accountName)
        {
            var accountList = LoadAccountList();

            AzureAccount azureAccount = null;

            if (accountList != null && accountList.Count() > 0)
            {
                azureAccount = accountList.Find(al => al.Name.Equals(accountName, StringComparison.OrdinalIgnoreCase));
            }

            return azureAccount;
        }

        //*********************
        //*                   *
        //*  LoadAccountList  *
        //*                   *
        //*********************
        // Load the account list combo box.

        public static List<AzureAccount> LoadAccountList()
        {
            string cipherKey = "lkjsojkweu798ynfgs";
            List<AzureAccount> accounts = new List<AzureAccount>();

            String filename = System.Windows.Forms.Application.UserAppDataPath + "\\AzureStorageExplorer6.dt1";

            if (File.Exists(filename))
            {
                using (TextReader reader = File.OpenText(filename))
                {
                    reader.ReadLine();  // version

                    String line;
                    String[] items;
                    while ((line = reader.ReadLine()) != null)
                    {
                        try
                        {
                            line = StringCipher.Decrypt(line, cipherKey);

                            items = line.Split('|');
                            if (items.Length >= 4)
                            {
                                AzureAccount account = new AzureAccount()
                                {
                                    Name = items[0],
                                    Key = items[1],
                                    IsDeveloperAccount = (items[2] == "1"),
                                    UseSSL = (items[3] == "1")
                                };
                                if (items.Length >= 5)
                                {
                                    account.EndpointDomain = items[4];
                                }
                                else
                                {
                                    account.EndpointDomain = "core.windows.net";
                                }
                                accounts.Add(account);
                            }
                        }
                        catch (Exception)
                        {
                            // If something is wrong in the account data file, don't let that stop the rest from loading.
                        }
                    }
                }
            }

            return accounts;
        }


        //*********************
        //*                   *
        //*  SaveAccountList  *
        //*                   *
        //*********************
        // Save the account list to disk.

        public static void SaveAccountList(List<AzureAccount> accounts)
        {
            string cipherKey = "lkjsojkweu798ynfgs";
            // Sort account list.

            accounts = accounts.OrderBy(o => o.Name).ToList();

            // Save account list, encrypted.

            String filename = System.Windows.Forms.Application.UserAppDataPath + "\\AzureStorageExplorer6.dt1";

            using (TextWriter writer = File.CreateText(filename))
            {
                writer.WriteLine("v6.0-1");
                foreach (AzureAccount account in accounts)
                {
                    String line = account.Name + "|";
                    line = line + account.Key + "|";

                    if (account.IsDeveloperAccount)
                    {
                        line = line + "1|";
                    }
                    else
                    {
                        line = line + "0|";
                    }

                    if (account.UseSSL)
                    {
                        line = line + "1|";
                    }
                    else
                    {
                        line = line + "0|";
                    }

                    line = line + account.EndpointDomain + "|";
                    line = StringCipher.Encrypt(line, cipherKey);

                    writer.WriteLine(line);
                }
            }
        }

        public static void SerializeAndWrite<T>(T obj, string fileName)
        {
            //Create the stream to add object into it.
            System.IO.Stream stream = File.OpenWrite(fileName);

            //Format the object as Binary
            BinaryFormatter formatter = new BinaryFormatter();
            
            formatter.Serialize(stream, obj);
            stream.Flush();
            stream.Close();
            stream.Dispose();
        }

        public static T Deserialize<T>(string fileName) where T : class
        {
            BinaryFormatter formatter = new BinaryFormatter();
            T obj = default(T);

            FileStream stream = File.Open(fileName, FileMode.Open);

            if (stream.Length > 0)
            { 
                obj = formatter.Deserialize(stream) as T;
            }

            stream.Flush();
            stream.Close();
            stream.Dispose();

            return obj;
        }
    }
}
