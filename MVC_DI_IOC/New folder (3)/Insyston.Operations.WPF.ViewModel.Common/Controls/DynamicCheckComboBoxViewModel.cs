// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicCheckComboBoxViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicCheckComboBoxViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Insyston.Operations.WPF.ViewModels.Common.Annotations;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The dynamic check combo box view model.
    /// </summary>
    public class DynamicCheckComboBoxViewModel :UserControlViewModelBase, INotifyPropertyChanged
    {
        /// <summary>
        /// The _is enable combo box.
        /// </summary>
        private bool _isEnableComboBox;

        /// <summary>
        /// The _is editable.
        /// </summary>
        private bool _isEditable;

        /// <summary>
        /// The selected item changed.
        /// </summary>
        public Action<ItemComboBox> SelectedItemChanged;

        /// <summary>
        /// Gets or sets the type item.
        /// </summary>
        public SystemType TypeItem { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicCheckComboBoxViewModel"/> class.
        /// </summary>
        public DynamicCheckComboBoxViewModel()
        {
            this.ComboBoxItemList = new ObservableCollection<ItemComboBox>();
            this.IsEditable = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is enable combo box.
        /// </summary>
        public bool IsEnableComboBox
        {
            get
            {
                return this._isEnableComboBox;
            }
            set
            {
                this._isEnableComboBox = value;
                this.OnPropertyChanged("IsEnableComboBox");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is editable.
        /// </summary>
        public bool IsEditable
        {
            get
            {
                return this._isEditable;
            }
            set
            {
                this._isEditable = value;
                this.OnPropertyChanged("IsEditable");
            }
        }

        /// <summary>
        /// The _selected item.
        /// </summary>
        private ItemComboBox _selectedItem;

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public ItemComboBox SelectedItem
        {
            get
            {
                return this._selectedItem;
            }
            set
            {
                if (this._selectedItem != value && this.SelectedItemChanged != null)
                {
                    this.SelectedItemChanged(value);
                }
                this._selectedItem = value;              
                this.OnPropertyChanged("SelectedItem");
            }
        }

        /// <summary>
        /// The _current name.
        /// </summary>
        private string _currentName;

        /// <summary>
        /// Gets or sets the data test.
        /// </summary>
        public string CurrentName
        {
            get
            {
                return this._currentName;
            }
            set
            {
                this._currentName = value;
                this.OnPropertyChanged("CurrentName");
            }
        }

        /// <summary>
        /// The _combo box item list.
        /// </summary>
        private ObservableCollection<ItemComboBox> _comboBoxItemList;

        /// <summary>
        /// Gets or sets the data test.
        /// </summary>
        public ObservableCollection<ItemComboBox> ComboBoxItemList
        {
            get
            {
                return this._comboBoxItemList;
            }
            set
            {
                this._comboBoxItemList = value;
                this.OnPropertyChanged("ComboBoxItemList");
            }
        }

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The set selected item.
        /// </summary>
        /// <param name="itemId">
        /// The item Id.
        /// </param>
        public void SetSelectedItem(int itemId)
        {
            this._selectedItem = this.ComboBoxItemList.FirstOrDefault(x => x.ItemId == itemId);
            if (this._selectedItem == null)
            {
                this._selectedItem = this.ComboBoxItemList.FirstOrDefault();
            }
            this.OnPropertyChanged("SelectedItem");
        }

        /// <summary>
        /// The set list source item.
        /// </summary>
        /// <param name="listItem">
        /// The list item.
        /// </param>
        public void SetListSourceItem(ObservableCollection<ItemComboBox> listItem)
        {
            this._comboBoxItemList = listItem;
            this.OnPropertyChanged("ComboBoxItemList");
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        [NotifyPropertyChangedInvocator]
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
