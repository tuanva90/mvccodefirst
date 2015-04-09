// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToolbarCommandGridViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the ToolbarCommandGridViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The toolbar command grid view model.
    /// </summary>
    public class ToolbarCommandGridViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// The _custom toolbar commands.
        /// </summary>
        private List<CustomToolbarCommand> _customToolbarCommands;

        /// <summary>
        /// Gets or sets the custom toolbar commands.
        /// </summary>
        public List<CustomToolbarCommand> CustomToolbarCommands
        {
            get
            {
                return this._customToolbarCommands;
            }
            set
            {
                this.SetField(ref _customToolbarCommands, value, () => CustomToolbarCommands);
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
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task<bool> LockAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
