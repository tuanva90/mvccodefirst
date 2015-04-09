// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomToolbarCommand.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The custom toolbar command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The custom toolbar command.
    /// </summary>
    public class CustomToolbarCommand : ViewModelUseCaseBase
    {
        /// <summary>
        /// The item clicked.
        /// </summary>
        public Action<object> ToolbarCommandClicked;

        /// <summary>
        /// Gets or sets the toolbar command name.
        /// </summary>
        public string ToolbarCommandName { get; set; }

        /// <summary>
        /// Gets or sets the source image.
        /// </summary>
        public string SourceImage { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public EnumToolbarAction Key { get; set; }

        /// <summary>
        /// The _toolbar command visibility change.
        /// </summary>
        private Visibility _toolbarCommandVisibilityChange;

        /// <summary>
        /// Gets or sets the toolbar command visibility change.
        /// </summary>
        public Visibility ToolbarCommandVisibilityChange 
        {
            get
            {
                return this._toolbarCommandVisibilityChange;
            }

            set
            {
                this.SetField(ref this._toolbarCommandVisibilityChange, value, () => this.ToolbarCommandVisibilityChange);
            }
        }

        /// <summary>
        /// The do action click.
        /// </summary>
        public void DoActionClick()
        {
            if (this.ToolbarCommandClicked != null)
            {
                this.ToolbarCommandClicked(this);
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
