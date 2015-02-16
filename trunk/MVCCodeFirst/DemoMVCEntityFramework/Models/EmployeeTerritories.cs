using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DemoMVCEntityFramework.Models
{
    public class EmployeeTerritories
    {
        public EmployeeTerritories() { }
        [Key]
        public int EmployeeID { get; set; }
        //Foreign for Territory
        [MaxLength(50)]
        public string TerritoryID { get; set; }
        public Territory Territory { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}