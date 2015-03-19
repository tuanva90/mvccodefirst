using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;
using UDI.EF.DAL;

namespace UDI.EF.Repositories
{
    public class EFProductRepository : EFRepositoryBase<Product>
    {
        public EFProductRepository(EFContext inputDBContext) : base(inputDBContext)
        {
        }        
    }
}
