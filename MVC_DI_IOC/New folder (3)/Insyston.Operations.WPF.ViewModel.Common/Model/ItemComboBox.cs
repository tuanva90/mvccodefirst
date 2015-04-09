// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemComboBox.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the ItemComboBox type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Model
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The item combo box.
    /// </summary>
    public class ItemComboBox : UserControlViewModelBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the type item.
        /// </summary>
        public SystemType TypeItem { get; set; }

        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// The _is checked.
        /// </summary>
        private bool _isChecked;

        /// <summary>
        /// Gets or sets a value indicating whether is checked.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this._isChecked;
            }

            set
            {
                this._isChecked = value;
                this.OnPropertyChanged("IsChecked");
            }
        }

        /// <summary>
        /// The _name.
        /// </summary>
        private string _name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                this._name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
