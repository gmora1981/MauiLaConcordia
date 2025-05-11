using MauiLaConcordia.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLaConcordia.Shared.Repository
{
    public interface IUsersRepository
    {

        //Task AssignRole(EditRoleDTO editRole);
        // Task<List<RoleDTO>> GetRoles();
        Task<List<UserEditDTO>> GetUsuarios();
        Task<PaginatedResponse<List<UserDTO>>> GetUsers(PaginationDTO paginationDTO);
        // Task RemoveRole(EditRoleDTO editRole);
    }
}
