using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_DI_IOC.Core;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;


namespace MVC_DI_IOC.Data.Mapping
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