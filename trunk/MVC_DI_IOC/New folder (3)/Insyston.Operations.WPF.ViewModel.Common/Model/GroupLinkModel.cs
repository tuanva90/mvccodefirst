using Insyston.Operations.WPF.ViewModels.Common.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Common.Model
{
    using System.Windows;

    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    public class GroupLinkModel
    {
        public string Header { get; set; }
        public List<CustomHyperlink> Hyperlinks { get; set; }
    }
}
