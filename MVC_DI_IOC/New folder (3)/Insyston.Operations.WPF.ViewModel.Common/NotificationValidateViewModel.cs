using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Common
{
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    public class NotificationValidateViewModel : ViewModelUseCaseBase
    {
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                this.SetField(ref _title, value, () => Title);
            }
        }

        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                this.SetField(ref _content, value, () => Content);
            }
        }
        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
    }
}
