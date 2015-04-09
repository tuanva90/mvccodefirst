using System;
using System.Linq;
using Microsoft.Practices.Prism.Events;
using Insyston.Operations.Business.Common.Enums;

namespace Insyston.Operations.WPF.ViewModel.Events
{
    public class ProgressChanged : CompositePresentationEvent<ProgressStatus>
    {

    }
}
