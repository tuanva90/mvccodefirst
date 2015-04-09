using Insyston.Operations.WPF.ViewModels.Assets.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Insyston.Operations.WPF.ViewModels.Assets.Helpers
{
    public class ItemDepreciationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemStopDepn { get; set; }
        public DataTemplate ItemDepnMethod { get; set; }
        public DataTemplate ItemSalvage { get; set; }
        public DataTemplate ItemEffectiveLife { get; set; }
        public DataTemplate ItemUseDefault { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var itemTemplate = item as ItemChildType;

            if (itemTemplate != null)
            {
                if (itemTemplate.TypeItem.Equals("ItemStopDepn"))
                    return ItemStopDepn;
                if (itemTemplate.TypeItem.Equals("ItemDepnMethod"))
                    return ItemDepnMethod;
                if (itemTemplate.TypeItem.Equals("ItemSalvage"))
                    return ItemSalvage;
                if (itemTemplate.TypeItem.Equals("ItemEffectiveLife"))
                    return ItemEffectiveLife;
                if (itemTemplate.TypeItem.Equals("ItemUseDefault"))
                    return ItemUseDefault;
            }
            return ItemStopDepn;
        }
    }
}
