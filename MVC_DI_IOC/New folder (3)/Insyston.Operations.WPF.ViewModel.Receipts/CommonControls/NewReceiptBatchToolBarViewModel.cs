using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Common;
using Insyston.Operations.WPF.View.Receipts;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModel.Receipts
{
    [Export(typeof(NewReceiptBatchToolBarViewModel))]
    public class NewReceiptBatchToolBarViewModel : OldViewModelBase
    {
        public InteractionRequest<PopupWindow> Popup { get; private set; }
        public DelegateCommand<string> OpenPopup { get; private set; }

        public NewReceiptBatchToolBarViewModel()
        {
            Popup = new InteractionRequest<PopupWindow>();
            OpenPopup = new DelegateCommand<string>(OnOpenPopup);
        }

        private void OnOpenPopup(string value)
        {
            PopupWindow popupWindow;

            int module;

            module = Convert.ToInt32(value);
            if (module >= 1 && module <= 6)
            {
                popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "CommonControls.NewReceiptBatch", true);
                popupWindow.Parameters.Add(module);
                Popup.Raise(popupWindow, (popupCallBack) => { if (popupWindow.ReturnValue != null) Shared.AddLoadedItem((NavigationItem)popupWindow.ReturnValue); });
            }
        }      
    }
}
