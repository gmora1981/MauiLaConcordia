using MauiLaConcordia.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLaConcordia.Shared.Auth
{
    public interface ILoginService
    {

        Task Login(UserToken userToken);
        Task Logout();
        Task TryRenewToken();
    }
}
