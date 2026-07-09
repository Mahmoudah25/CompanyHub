using CompanyHub.Application.Role;
using CompanyHub.Application.Role.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService roleService;
        public RoleController(IRoleService roleService) 
        {
            this.roleService = roleService;
        }

        [Authorize(Policy = "Role.Read")]
        [HttpGet]
        public async Task <IActionResult> GetAllRoles()
        {
            var roles = await roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [Authorize(Policy = "Role.Read")]
        [HttpGet("/{Id}")]
        public async Task <IActionResult> GetById(Guid Id)
        {
            var role = await roleService.GetRolesByIdAsync(Id);
            return Ok(role);
        }

        [Authorize(Policy = "Role.Read")]
        [HttpGet("/{Name:alpha}")]
        public async Task <IActionResult> GetRoleByName(string Name)
        {
            var role = await roleService.GetRolesByNameAsync(Name);
            return Ok(role);
        }

        [Authorize(Policy = "Role.Create")]
        [HttpPost]
        public async Task <IActionResult> AddRole(CreateRoleRequest request)
        {
            var NewRole = await roleService.CreateRole(request);
            return Ok(NewRole);
        }

        [Authorize(Policy = "Role.Update")]
        [HttpPut("/{Id}")]
        public async Task <IActionResult> PutRole(Guid Id ,UpdateRoleRequest request) 
        {
            var role = await roleService.UpdateRoleAsync(Id,request);
            return Ok(role);
        }

        [Authorize(Policy = "Role.Delete")]
        [HttpDelete("{Id}")]
        public async Task <IActionResult> Delete(Guid Id) 
        {
            var role = await roleService.DeleteRoleAsync(Id);
            return NoContent();
        }

        // assign Permission
        [Authorize(Policy = "Role.AssignPermission")]
        [HttpPost("{RoleId}/Permisson")]
        public async Task <IActionResult> AssignPermissionToRole(Guid RoleId, AssignPermissionsRequest request)
        {
             await roleService.AssignPermissionsToRole(RoleId,request);
            return Ok();
        }
    }
}
