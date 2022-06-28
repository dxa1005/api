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
//using System.Web.Script.Serialization;

namespace UserDataAPI.Repo
{
    public class UserRepo: IUserRepo
    {
        private readonly UserDbContext _userContext;

        public UserRepo(UserDbContext userContext)
        {
            _userContext = userContext;
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
            var connectionString = "Data Source=migrationpoc1.database.windows.net; Initial Catalog=migrationpoc; Authentication=Active Directory Default";


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

                KeyVaultSecret secret = client.GetSecret("migrationsecret");

                secretValue = secret.Value;
                return secretValue;
            }
            catch (Exception ex)
            {
                return secretValue = ex.ToString();
            }
        }
    }

}
