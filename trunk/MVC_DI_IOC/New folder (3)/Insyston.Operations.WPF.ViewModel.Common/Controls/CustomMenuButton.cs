// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomMenuButton.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The custom menu button.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using System;
    using System.Runtime.Hosting;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The custom menu button.
    /// </summary>
    public class CustomMenuButton : ViewModelUseCaseBase
    {
        public CustomMenuButton()
        {
            _selectedStyle = (Style)Application.Current.FindResource("MenuStyle");
        }
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// The click button.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public delegate void ClickButton(object sender);

        /// <summary>
        /// The on click button.
        /// </summary>
        public ClickButton OnClickButton;

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

        /// <summary>
        /// The button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public void DoActionClick()
        {
            OnClickButton(this.Header);
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
