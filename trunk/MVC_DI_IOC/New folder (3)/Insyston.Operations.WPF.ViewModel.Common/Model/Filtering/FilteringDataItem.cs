// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilteringDataItem.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the FilteringDataItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Model.Filtering
{
    using Insyston.Operations.Model;

    /// <summary>
    /// The filtering data item.
    /// </summary>
    public class FilteringDataItem : ObservableModel
    {
        /// <summary>
        /// The _is selected.
        /// </summary>
        private bool _isSelected;

        /// <summary>
        /// The _id.
        /// </summary>
        private int _id;

        /// <summary>
        /// The _text.
        /// </summary>
        private string _text;

        /// <summary>
        /// Gets or sets a value indicating whether is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this._isSelected;
            }
            set
            {
                this.SetField(ref this._isSelected, value, () => IsSelected);
            }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id
        {
            get
            {
                return this._id;
            }
            set
            {
                this.SetField(ref this._id, value, () => Id);
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                this.SetField(ref this._text, value, () => Text);
            }
        }
    }
}
