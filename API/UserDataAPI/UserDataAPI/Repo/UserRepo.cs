using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDataAPI.Models;
using UserDataAPI.RepoInterface;
using System.Net;
using System.IO;
using Microsoft.Data.SqlClient;
using Dapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace UserDataAPI.Repo
{
    public class UserRepo: IUserRepo
    {
        private readonly UserDbContext _userContext;
         private readonly IConfiguration Configuration;

        public UserRepo(UserDbContext userContext, IConfiguration configuration)
        {
            _userContext = userContext;
            Configuration = configuration;
        }
        public async Task<List<User>> GetUsers()
        {
                        return await _userContext.Users.ToListAsync(); ;
        }

        public async Task<User> AddUser(User user)
        {
            _userContext.Users.Add(user);
            await _userContext.SaveChangesAsync();

            return user;
        }

        public async Task<string> GetToken()
        {           
          
            string res=null;
            var connectionString = Configuration["ConnectionStrings:UsersDbConnectionString"];


            using (SqlConnection con = new SqlConnection(connectionString))
            {
              // con.Open();
                try
                {
                    await using var connection = new SqlConnection(connectionString);
                    var count = await connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM [dbo].[User]");
                    res = count.ToString();
                }
                catch(Exception e)
                {
                   res = e.ToString();
                }
                return res;
                //return await _userContext.Users.ToListAsync(); ;
            }       
            
            
        }

        public async Task<string> GetSecret()
        {
            string secretValue = null;
            try
            {
                var secretName = Configuration["KeyVault:SecretName"];
                SecretClientOptions options = new SecretClientOptions()
                {
                    Retry =        
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }             
                };
                var client = new SecretClient(new Uri("https://client-keyvault.vault.azure.net/"), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret(secretName);

                secretValue = secret.Value;
                return secretValue;
            }
            catch (Exception ex)
            {
                return secretValue = ex.ToString();
            }
        }
        
         public async Task<string> GetBlobs()
        {
            string res = null;
            try
            {
                
                string blobContainter = Configuration["Blob:Container"]; 
                string accountName = Configuration["Blob:StorageAccount"];
                int count = 0;
                string containerEndpoint = string.Format("https://{0}.blob.core.windows.net/{1}",
                              accountName, blobContainter);
                BlobContainerClient containerClient
                      = new BlobContainerClient(new Uri(containerEndpoint),
                                                new DefaultAzureCredential());
                //BlobContainerClient containerClient = new BlobContainerClient(blobConString, blobContainter);
                string blobNames = null;
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    blobNames = blobNames+blobItem.Name+", ";
                    count++;
                }
                string response = null;
                response = "No. of blobs: " + count.ToString() + (char)10 + "Blob Names: " + blobNames ;
                res = response.ToString();                
            }
            catch (Exception ex)
            {
                res = ex.ToString();
            }
            return res;
        }
    }

}
