using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDataAPI.Models;
using UserDataAPI.ServiceInterface;

namespace UserDataAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                List<User> UsersList = new List<User>();
                UsersList = await _userService.GetUsers();
                return Ok(UsersList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            } 
        }

        [HttpGet]
        [Route("Count")]
        public async Task<ActionResult<string>> GetToken()
        {
            try
            {
                string Token = await _userService.GetToken();
                return Ok(Token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("Secret")]
        public async Task<ActionResult<string>> GetSecret()
        {
            try
            {
                string Secret = await _userService.GetSecret();
                return Ok(Secret);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody]User user)
        {
            try
            {
                User addedUser;
                addedUser = await _userService.AddUser(user);
                return Ok(addedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
    }
}
