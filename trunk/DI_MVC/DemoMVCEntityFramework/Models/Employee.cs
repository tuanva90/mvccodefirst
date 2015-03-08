using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DemoMVCEntityFramework.Models
{
    public class Employee
    {
        public Employee() { }
        [Key]
        public int EmployeeID { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(50)]
        public string TitleOfCourtesy { get; set; }
        [MaxLength(50)]
        public DateTime BirthDate { get; set; }
        public DateTime HireDate { get; set; }
        public string Address { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(50)]
        public string Region { get; set; }
        [MaxLength(50)]
        public string PostalCode { get; set; }
        [MaxLength(50)]
        public string Country { get; set; }
        [MaxLength(50)]
        public string HomePhone { get; set; }
        [MaxLength(50)]
        public string Extension { get; set; }
        public byte[] Photo { get; set; }
        [MaxLength(50)]
        public string Notes { get; set; }
        public int ReportsTo { get; set; }
        [MaxLength(255)]
        public string PhotoPath { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}