﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppRRHH.Models
{
    public class AppUser : IdentityUser
    {
        public string NumberOrg { get; set; }
        public string Name { get; set; }
    }
}
