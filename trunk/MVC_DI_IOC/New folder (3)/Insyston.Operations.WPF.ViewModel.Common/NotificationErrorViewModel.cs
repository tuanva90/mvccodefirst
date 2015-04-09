namespace Insyston.Operations.WPF.ViewModels.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    public class NotificationErrorViewModel : ViewModelUseCaseBase
    {
        private List<CustomHyperlink> _ListCustomHyperlink;
        public List<CustomHyperlink> listCustomHyperlink
        {
            get
            {
                return this._ListCustomHyperlink;
            }
            set
            {
                this.SetField(ref this._ListCustomHyperlink, value, () => this.listCustomHyperlink);
            }
        }

        public NotificationErrorViewModel()
        {
            this._ListCustomHyperlink = new List<CustomHyperlink>();
            //List<CustomHyperlink> customHyperlinks = new List<CustomHyperlink>
            //                                             {
            //                                                 new CustomHyperlink { HyperlinkHeader = "Summary"} , //HyperlinkName = "Summary" },
            //                                                 new CustomHyperlink { HyperlinkHeader = "Permission" }, //HyperlinkName = "Permission" }
            //                                             };
            //this.CustomHyperlinks = customHyperlinks;
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
