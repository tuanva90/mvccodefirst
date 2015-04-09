// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomHyperlink.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The custom hyperlink.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The custom hyperlink.
    /// </summary>
    public class CustomHyperlink : ViewModelUseCaseBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHyperlink"/> class.
        /// </summary>
        public CustomHyperlink()
        {
            _selectedStyle = (Style)Application.Current.FindResource("HyperLinkButtonStyle");
            this.IsButtonHyperlinkVisible = Visibility.Visible;
            this.IsTextBlockVisible = Visibility.Collapsed;
        }

        /// <summary>
        /// The hyperlink clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public delegate void HyperlinkClicked(object sender);

        /// <summary>
        /// The item clicked.
        /// </summary>
        public Action<object> ItemClicked;

        /// <summary>
        /// Gets or sets the hyperlink header.
        /// </summary>
        public string HyperlinkHeader { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public HyperLinkAction Action { get; set; }

        /// <summary>
        /// Gets or sets the screen.
        /// </summary>
        public EnumScreen Screen { get; set; }

        /// <summary>
        /// The _selected style.
        /// </summary>
        private Style _selectedStyle;

        /// <summary>
        /// Gets or sets the selected style.
        /// </summary>
        public Style SelectedStyle
        {
            get
            {
                return this._selectedStyle;
            }
            set
            {
                if (value != null)
                {
                    this.SetField(ref this._selectedStyle, value, () => this.SelectedStyle);
                }
            }
        }

        private Visibility _isButtonHyperlinkVisible;

        public Visibility IsButtonHyperlinkVisible
        {
            get
            {
                return this._isButtonHyperlinkVisible;
            }
            set
            {
                this.SetField(ref _isButtonHyperlinkVisible, value, () => IsButtonHyperlinkVisible);
            }
        }

        private Visibility _isTextBlockVisible;

        public Visibility IsTextBlockVisible
        {
            get
            {
                return this._isTextBlockVisible;
            }
            set
            {
                this.SetField(ref _isTextBlockVisible, value, () => IsTextBlockVisible);
            }
        }

        ///// <summary>
        ///// The _on click hyperlink.
        ///// </summary>
        //private ICommand _onClickHyperlink;

        ///// <summary>
        ///// Gets or sets the on click hyperlink.
        ///// </summary>
        //public ICommand OnClickHyperlink
        //{
        //    get
        //    {
        //        if (this._onClickHyperlink == null)
        //        {
        //            this._onClickHyperlink = new RelayCommand(this.DoActionClick);
        //        }
        //        return this._onClickHyperlink;
        //    }
        //    set
        //    {
        //        this._onClickHyperlink = value;
        //    }
        //}

        /// <summary>
        /// The do action click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public void DoActionClick()
        {
            if (this.ItemClicked != null)
            {
                this.ItemClicked(this);
            }
        }

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
    }
}
