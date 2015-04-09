using System.Collections;
using Telerik.Windows.Controls.DragDrop;

namespace Insyston.Operations.WPF.ViewModels.Collections.Controls
{
    public class DragDropParameter
    {
        public object DraggedItem { get; set; }

        public IEnumerable ItemsSource { get; set; }

        public DragStatus DragStatus { get; set; }
    }
}
