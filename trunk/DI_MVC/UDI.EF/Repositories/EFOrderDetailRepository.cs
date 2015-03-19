using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;
using UDI.EF.DAL;

namespace UDI.EF.Repositories
{
    class EFOrderDetailRepository : EFRepositoryBase<OrderDetail>
    {
        public EFOrderDetailRepository(EFContext inputDBContext)
            : base(inputDBContext)
        {
        }
    }
}