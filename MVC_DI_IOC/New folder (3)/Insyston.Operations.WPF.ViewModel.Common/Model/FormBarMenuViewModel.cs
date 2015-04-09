// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormBarMenuViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The form bar menu view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModel.Common.Model
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Insyston.Operations.WPF.ViewModel.Common.Interfaces;

    /// <summary>
    /// The form bar menu view model.
    /// </summary>
    public class FormBarMenuViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// The on click hyperlink.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public delegate void OnClickHyperlink(object sender);

        /// <summary>
        /// The hyperlink clicked.
        /// </summary>
        public OnClickHyperlink HyperlinkClicked;

        /// <summary>
        /// The _form bar content.
        /// </summary>
        private string _formBarContent;

        /// <summary>
        /// The _form menu content.
        /// </summary>
        private string _formMenuContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormBarMenuViewModel"/> class.
        /// </summary>
        public FormBarMenuViewModel()
        {
            this._formBarContent = "User";
            this._formMenuContent = "User";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormBarMenuViewModel"/> class.
        /// </summary>
        /// <param name="barContent">
        /// The bar content.
        /// </param>
        /// <param name="menuContent">
        /// The menu content.
        /// </param>
        /// <param name="hyperlinkClicked">
        /// The hyperlink clicked.
        /// </param>
        public FormBarMenuViewModel(string barContent, string menuContent, OnClickHyperlink hyperlinkClicked)
        {
            this._formBarContent = barContent;
            this._formMenuContent = menuContent;
            this.HyperlinkClicked = hyperlinkClicked;
        }

        /// <summary>
        /// Gets or sets the form bar content.
        /// </summary>
        public string FormBarContent
        {
            get
            {
                return this._formBarContent;
            }
            set
            {
                if (value != null)
                {
                    this.SetField(ref this._formBarContent, value, () => FormBarContent);
                }
            }
        }

        /// <summary>
        /// Gets or sets the form menu content.
        /// </summary>
        public string FormMenuContent
        {
            get
            {
                return this._formMenuContent;
            }
            set
            {
                if (value != null)
                {
                    this.SetField(ref this._formMenuContent, value, () => FormMenuContent);
                }
            }
        }

        /// <summary>
        /// The _ on click command.
        /// </summary>
        private ICommand _onClickCommand;

        /// <summary>
        /// Gets or sets the on click command.
        /// </summary>
        public ICommand OnClickCommand
        {
            get
            {
                if (this._onClickCommand == null)
                {
                    this._onClickCommand = new RelayCommand(this.HyperlinkOnClick);
                }
                return this._onClickCommand;
            }
            set
            {
                this._onClickCommand = value;
            }
        }

        /// <summary>
        /// The hyperlink on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        private void HyperlinkOnClick(object sender)
        {
            this.HyperlinkClicked(this._formMenuContent);
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
