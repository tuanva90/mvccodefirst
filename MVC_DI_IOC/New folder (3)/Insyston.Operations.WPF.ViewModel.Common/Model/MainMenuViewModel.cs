// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainMenuViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The main menu view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The main menu view model.
    /// </summary>
    public class MainMenuViewModel : ViewModelUseCaseBase
    {

        /// <summary>
        /// The _list buttons.
        /// </summary>
        private List<CustomMenuButton> _listButtons;

        /// <summary>
        /// Gets or sets the list buttons.
        /// </summary>
        public List<CustomMenuButton> ListButtons
        {
            get
            {
                return this._listButtons;
            }
            set
            {
                this.SetField(ref this._listButtons, value, () => ListButtons);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuViewModel"/> class.
        /// </summary>
        public MainMenuViewModel()
        {
            this._listButtons = new List<CustomMenuButton>();

            this._listButtons.Add(new CustomMenuButton { Header = "Home", SelectedStyle = (Style)Application.Current.FindResource("MenuStyleClick") });
            this.AddListView("Asset Register");
            this.AddListView("Collection");
            this.AddListView("Funding");
            this.AddListView("Security");
        }

        /// <summary>
        /// The add list view.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        private void AddListView(string name)
        {
            CustomMenuButton but = new CustomMenuButton();
            but.Header = name;
            this.ListButtons.Add(but);
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
