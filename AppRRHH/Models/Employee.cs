using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppRRHH.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Seniority { get; set; }
        [Display(Name = "Gross Salary")]
        public double GrossSalary { get; set; }
        [Display(Name = "Net Salary")]
        public double NetSalary { get; set; }
        [Display(Name = "Photo")]
        public string Image { get; set; }
        
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

    }
}
