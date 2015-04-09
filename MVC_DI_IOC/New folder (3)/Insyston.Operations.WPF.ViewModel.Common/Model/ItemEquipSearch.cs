// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemEquipSearch.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the ItemEquipSearch type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Model
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Insyston.Operations.WPF.ViewModels.Common.Controls;

    /// <summary>
    /// The item equip search.
    /// </summary>
    public class ItemEquipSearch : INotifyPropertyChanged
    {
        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The _list source items.
        /// </summary>
        private DynamicCheckComboBoxViewModel _listSourceItems;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the item type.
        /// </summary>
        public SystemType ItemType { get; set; }

        /// <summary>
        /// Gets or sets the list source items.
        /// </summary>
        public DynamicCheckComboBoxViewModel ListSourceItems
        {
            get
            {
                return this._listSourceItems;
            }
            set
            {
                this._listSourceItems = value;
                this.OnPropertyChanged("ListSourceItems");
            }
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
