// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormTabHyperlinkViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the TabHyperlinkViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModel.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Insyston.Operations.WPF.ViewModel.Common.Controls;
    using Insyston.Operations.WPF.ViewModel.Common.Interfaces;

    /// <summary>
    /// The tab hyperlink view model.
    /// </summary>
    public class FormTabHyperlinkViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// The _form tab hyperlinks.
        /// </summary>
        private List<CustomHyperlink> _listFormTabHyperlinks;

        /// <summary>
        /// Gets or sets the custom hyperlinks.
        /// </summary>
        public List<CustomHyperlink> ListFormTabHyperlink
        {
            get
            {
                return this._listFormTabHyperlinks;
            }
            set
            {
                this.SetField(ref this._listFormTabHyperlinks, value, () => this.ListFormTabHyperlink);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormTabHyperlinkViewModel"/> class.
        /// </summary>
        public FormTabHyperlinkViewModel()
        {
            this._listFormTabHyperlinks = new List<CustomHyperlink>();
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

    /// <summary>
    /// The hyper link action.
    /// </summary>
    public enum HyperLinkAction
    {
        SummaryState,
        PersonalDetailsState,
        CredentialsState,
        GroupsState,
        PermissionsState,
        Users,
        Groups,
        DetailsState,
        UsersState,
    }
}
