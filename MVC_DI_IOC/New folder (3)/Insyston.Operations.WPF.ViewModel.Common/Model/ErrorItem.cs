using System;
using System.Linq;
using Insyston.Operations.Model;

namespace Insyston.Operations.WPF.ViewModels.Common.Model
{
    public class ErrorItem : NotificationObject
    {
        private string _Message;
        private string _PropertyName;

        public string Message
        {
            get
            {
                return this._Message;
            }
            set
            {
                if (value != this.Message)
                {
                    this._Message = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }

        public string PropertyName
        {
            get
            {
                return this._PropertyName;
            }
            set
            {
                if (value != this._PropertyName)
                {
                    this._PropertyName = value;
                    this.RaisePropertyChanged("PropertyName");
                }
            }
        }
    }
}
