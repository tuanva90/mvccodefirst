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
    public class CategoryMapping : EntityTypeConfiguration<Category>
    {
        public CategoryMapping()
        {
            HasKey(cate => cate.CategoryID);
            Property(cate => cate.CategoryID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(cate => cate.CategoryName);
            Property(cate => cate.Description);
            Property(cate => cate.Picture);
            ToTable("Category");
        }
    }
}
