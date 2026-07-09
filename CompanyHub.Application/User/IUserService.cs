using CompanyHub.Application.Role.DTOs;
using CompanyHub.Application.User.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.User
{
    public interface IUserService
    {
        Task AssignRoleToUser(Guid UserId, AssignRoleToUserRequest request);
        Task RemoveRoleFromUser(Guid UserId, Guid RoleId);
        Task<Guid> CreateUser(CreateUserRequest request);
        Task<List<UserResponse>> GetAllUsers();
        Task<UserResponse> GetUsersById(Guid Id);
        Task<UserResponse> getUserByEmail(string email);
        Task<bool> UpdateUser(Guid Id, UpdateUserRequest request);
        Task<bool> Delete(Guid Id);
        Task<List<UserResponse>> SearchUser(SearchUserRequest request);

    }
}
