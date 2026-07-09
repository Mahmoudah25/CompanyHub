using CompanyHub.Application.User;
using CompanyHub.Application.User.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [Authorize(Policy = "User.Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            var user = await userService.CreateUser(request);
            return Ok(user);
        }

        [Authorize(Policy = "User.Read")]
        [HttpGet]
        public async Task<IActionResult> getAll()
        {
            var users = await userService.GetAllUsers();
            return Ok(users);
        }

        [Authorize(Policy = "User.Read")]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetUserById(Guid Id)
        {
            var user = await userService.GetUsersById(Id);
            return Ok(user);
        }

        [Authorize(Policy = "User.Read")]
        [HttpGet("{Email:alpha}")]
        public async Task<IActionResult> GetUserByEmail(string Email)
        {
            var user = await userService.getUserByEmail(Email);
            return Ok(user);
        }

        [Authorize(Policy = "User.Update")]
        [HttpPut("{Id}")]
        public async Task <IActionResult> Update(Guid Id, UpdateUserRequest update)
        {
            var user = await userService.UpdateUser(Id,update);
            return NoContent();
        }

        [Authorize(Policy = "User.Delete")]
        [HttpDelete("{Id}")]
        public async Task <IActionResult> Delete(Guid Id)
        {
            var user = await userService.Delete(Id);
            return NoContent();
        }

        [Authorize(Policy = "User.Update")]
        [HttpPost("{UserId}/roles")]
        public async Task <IActionResult> assignRole(Guid UserId, AssignRoleToUserRequest request) 
        {
            await userService.AssignRoleToUser(UserId, request);
            return Ok();
        }

        [Authorize(Policy = "User.Delete")]
        [HttpDelete("{UserId}/roles/{RoleId}")]
        public async Task <IActionResult> RemoveRoleFromUSer(Guid UserId,Guid RoleId)
        {
            await userService.RemoveRoleFromUser(UserId, RoleId);
            return NoContent();
        }

        [Authorize(Policy = "User.Read")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchUserRequest request)
        {
            var res = await userService.SearchUser(request);
            return Ok(res);
        }
    }
}
