// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormBarMenuViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The form bar menu view model.
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
    /// The form bar menu view model.
    /// </summary>
    public class FormBarMenuViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormBarMenuViewModel"/> class.
        /// </summary>
        public FormBarMenuViewModel()
        {
            _changedVisibility = new Visibility();
        }

        /// <summary>
        /// The _changed visibility.
        /// </summary>
        private Visibility _changedVisibility;

        /// <summary>
        /// Gets or sets the changed visibility.
        /// </summary>
        public Visibility ChangedVisibility
        {
            get
            {
                return this._changedVisibility;
            }
            set
            {
                this.SetField(ref _changedVisibility, value, () => ChangedVisibility);
            }
        }

        /// <summary>
        /// The _form menu content.
        /// </summary>
        private string _formMenuContent;

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
                this.SetField(ref this._formMenuContent, value, () => FormMenuContent);
            }
        }

        /// <summary>
        /// The _form bar content.
        /// </summary>
        private string _formBarContent;

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
                this.SetField(ref this._formBarContent, value, () => FormBarContent);
            }
        }

        /// <summary>
        /// The _on click hyperlink.
        /// </summary>
        private ICommand _onClickHyperlink;

        /// <summary>
        /// Gets or sets the on click hyperlink.
        /// </summary>
        public ICommand OnClickHyperlink
        {
            get
            {
                return _onClickHyperlink ?? (_onClickHyperlink = new RelayCommand(this.GetIdHyperlink));
            }
            set
            {
                _onClickHyperlink = value;
            }
        }

        /// <summary>
        /// The get id hyperlink.
        /// </summary>
        /// <param name="param">
        /// The _param.
        /// </param>
        private void GetIdHyperlink(object param)
        {
            if (ItemClicked != null)
            {
                ItemClicked(param);
            }
        }

        /// <summary>
        /// The item clicked.
        /// </summary>
        public Action<object> ItemClicked;

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
