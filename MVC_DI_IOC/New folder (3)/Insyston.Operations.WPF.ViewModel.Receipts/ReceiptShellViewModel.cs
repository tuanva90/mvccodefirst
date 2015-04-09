using System;
using System.Linq;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Commands;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.WPF.View.Receipts;
using Insyston.Operations.Business.Common.Model;
using Microsoft.Practices.Prism.Regions;

namespace Insyston.Operations.WPF.ViewModel.Receipts
{
    [Export(typeof(ReceiptShellViewModel))]
    public class ReceiptShellViewModel : OldViewModelBase
    {     
    }
}
