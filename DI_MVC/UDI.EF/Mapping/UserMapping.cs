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
    public class UserMapping : EntityTypeConfiguration<User>
    {
        public UserMapping()
        {
            HasKey(u => u.CustomerID);
            Property(u => u.CustomerID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(u => u.UserName).IsRequired();
            Property(u => u.Password).IsRequired();
            Property(u => u.Roles).IsRequired();
            Property(u => u.Email).IsRequired();
            Property(u => u.Bool).IsRequired().HasColumnName("Remember");
            ToTable("User");
        }
    }
}
