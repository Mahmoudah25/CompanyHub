using CompanyHub.Application.Role.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Role
{
    public interface IRoleService
    {
        Task<List<RoleResponse>> GetAllRolesAsync();
        Task<RoleResponse> GetRolesByIdAsync(Guid id);
        Task<RoleResponse> GetRolesByNameAsync(string Name);
        Task<Guid> CreateRole(CreateRoleRequest request);
        Task<bool> UpdateRoleAsync(Guid Id, UpdateRoleRequest request);
        Task<bool> DeleteRoleAsync(Guid Id);
        Task AssignPermissionsToRole(Guid RoleId, AssignPermissionsRequest request);
    }
}
