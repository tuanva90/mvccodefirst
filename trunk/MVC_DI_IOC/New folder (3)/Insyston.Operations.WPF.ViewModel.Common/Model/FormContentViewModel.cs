// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormContentViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the FormContentViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModel.Common.Model
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    using Insyston.Operations.WPF.ViewModel.Common.Interfaces;

    /// <summary>
    /// The form content view model.
    /// </summary>
    public class FormContentViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// The _form bar menu view model.
        /// </summary>
        private FormBarMenuViewModel _formBarMenuViewModel;

        /// <summary>
        /// The _hyperlink tab view model.
        /// </summary>
        private FormTabHyperlinkViewModel _hyperlinkTabViewModel;

        /// <summary>
        /// The _main content.
        /// </summary>
        private ObservableCollection<object> _mainContent;

        /// <summary>
        /// The _main data context.
        /// </summary>
        private ViewModelUseCaseBase _mainDataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormContentViewModel"/> class.
        /// </summary>
        public FormContentViewModel()
        {
            this._formBarMenuViewModel = new FormBarMenuViewModel();
            this._mainContent = new ObservableCollection<object>();
            this._hyperlinkTabViewModel = new FormTabHyperlinkViewModel();
        }

        /// <summary>
        /// Gets or sets the main content.
        /// </summary>
        public ObservableCollection<object> MainContent
        {
            get
            {
                return this._mainContent;
            }
            set
            {
                this.SetField(ref this._mainContent, value, () => this.MainContent);
            }
        }

        /// <summary>
        /// Gets or sets the main data context.
        /// </summary>
        public ViewModelUseCaseBase MainDataContext
        {
            get
            {
                return this._mainDataContext;
            }
            set
            {
                this.SetField(ref this._mainDataContext, value, () => this.MainDataContext);
            }
        }

        /// <summary>
        /// Gets or sets the form bar menu view model.
        /// </summary>
        public FormBarMenuViewModel FormBarMenuViewModel
        {
            get
            {
                return this._formBarMenuViewModel;
            }
            set
            {
                this.SetField(ref this._formBarMenuViewModel, value, () => this.FormBarMenuViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the hyperlink tab view model.
        /// </summary>
        public FormTabHyperlinkViewModel HyperlinkTabViewModel
        {
            get
            {
                return this._hyperlinkTabViewModel;
            }
            set
            {
                this.SetField(ref this._hyperlinkTabViewModel, value, () => this.HyperlinkTabViewModel);
            }
        }

        /// <summary>
        /// The hyperlink click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public void HyperlinkClick(object sender)
        {
            _mainDataContext.OnStepAsync(sender);
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
