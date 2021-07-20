using AppRRHH.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppRRHH.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AppRRHH.Models.Department> Department { get; set; }
        public DbSet<AppRRHH.Models.Employee> Employee { get; set; }
    }
}
