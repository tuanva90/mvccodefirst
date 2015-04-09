using Insyston.Operations.WPF.ViewModels.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Insyston.Operations.WPF.Views.Shell.Selectors
{
    public class CommandSelector : DataTemplateSelector
    {
        public DataTemplate ButtonTemplate
        {
            get;
            set;
        }

        public DataTemplate CustomTemplate
        {
            get;
            set;
        }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var command = item as ActionCommand;

            if (command != null)
            {
                if (command.CommandType == ActionCommadType.Custom)
                {
                    return CustomTemplate;
                }
                return ButtonTemplate;
            }

            return null;
        }
    }
}
