using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDataAPI.Models;

namespace UserDataAPI.ServiceInterface
{
    public interface IUserService
    {
        public Task<List<User>> GetUsers();

        public Task<User> AddUser(User user);

        public Task<string> GetToken();

        public Task<string> GetSecret();
    }
}
