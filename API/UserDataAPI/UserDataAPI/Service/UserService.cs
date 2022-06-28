using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDataAPI.Models;
using UserDataAPI.RepoInterface;
using UserDataAPI.ServiceInterface;

namespace UserDataAPI.Service
{
    public class UserService:IUserService
    {

        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<List<User>> GetUsers()
        {
            List<User> UsersList = new List<User>();
            UsersList = await _userRepo.GetUsers();
            return UsersList;
        }

        public async Task<User> AddUser(User user)
        {
            User addedUser;
            addedUser = await _userRepo.AddUser(user);
            return addedUser;
        }

        public async Task<string> GetToken()
        {            
            string token = await _userRepo.GetToken();
            return token;
        }

        public async Task<string> GetSecret()
        {
            string secret = await _userRepo.GetSecret();
            return secret;
        }
    }
}
