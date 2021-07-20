using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppRRHH.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Name")]
        public string Namespace { get; set; }
        public string Description { get; set; }

        public List<Employee> Employees { get; set; }

    }
}
