using MauiLaConcordia.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLaConcordia.Shared.Repository
{
    public interface IAccountsRepository
    {
        Task<UserToken> Login(UserLogin userInfo);
        Task<UserToken> Register(UserEditDTO userInfo);
        Task<UserToken> RenewToken();
    }
}
