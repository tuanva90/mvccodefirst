using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    public class ItemValidateContent
    {
        public Func<Task<bool>> CheckContentEdit;
        public Func<Task> DoUnLockAsync;
    }
}
