using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDI.CORE.Entities;
using UDI.EF.DAL;

namespace UDI.EF.Repositories
{
    class EFUserRepository: EFRepositoryBase<User>
    {
        public EFUserRepository(EFContext _inputDBContext)
            : base(_inputDBContext)
        {
        }
        
        
    }
}
