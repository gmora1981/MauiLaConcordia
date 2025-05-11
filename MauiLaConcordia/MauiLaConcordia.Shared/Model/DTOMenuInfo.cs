using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLaConcordia.Shared.Model
{
    public class DTOMenuInfo
    {
        public int Menuid { get; set; }
        public int? ParentMenuid { get; set; }
        public string? PageName { get; set; }
        public string? MenuName { get; set; }
        public string? IcoName { get; set; }
    }
}
