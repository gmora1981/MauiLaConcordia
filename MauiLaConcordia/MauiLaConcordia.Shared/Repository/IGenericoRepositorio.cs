using MauiLaConcordia.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLaConcordia.Shared.Repository
{
    public interface IGenericoRepositorio
    {
        Task<HttpResponseWrapper<T>> GetRegistrados<T>(string url);
    }
}
