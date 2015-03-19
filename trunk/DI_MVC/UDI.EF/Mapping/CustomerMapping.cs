using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;

namespace UDI.EF.Mapping
{
    public class CustomerMapping : EntityTypeConfiguration<Customer>
    {
        public CustomerMapping()
        {
            HasKey(c => c.CustomerID).HasRequired(u => u.User).WithOptional(c => c.Customer);
            Property(c => c.CustomerID);
            Property(c => c.ContactName).IsRequired();
            Property(c => c.ContactTitle).IsRequired();
            Property(c => c.Address).IsRequired();
            Property(c => c.City).IsRequired();
            Property(c => c.Country);
            Property(c => c.Fax);
            Property(c => c.Phone);
            Property(c => c.Region);
            Property(c => c.PostalCode);
            ToTable("Customer");
        }
    }
}
