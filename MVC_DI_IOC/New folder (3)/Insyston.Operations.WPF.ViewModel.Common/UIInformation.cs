using System;
using System.Linq;
using System.Windows.Threading;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

namespace Insyston.Operations.WPF.ViewModels.Common
{
    public class UIInformation
    {
        private string _Icon;

        public UIInformation(OldViewModelBase vwModel)
        {
            this.ViewModel = vwModel;
            this.ParentDispatcher = Dispatcher.CurrentDispatcher;
        }

        public OldViewModelBase ViewModel { get; private set; }

        public Dispatcher ParentDispatcher { get; private set; }

        public string Icon
        {
            get
            {
                if (string.IsNullOrEmpty(this._Icon))
                {
                    return "Images/Information.ico";
                }
                else
                {
                    return this._Icon;
                }
            }
            set
            {
                if (value.ToLower().Contains("images/"))
                {
                    this._Icon = value;
                }
                else
                {
                    this._Icon = string.Format("Images/{0}", value);
                }
            }
        }
    }
}
