using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTI619_Lab5.Models.Home
{
    public class MenuModel
    {
        public bool IsAdmin { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}