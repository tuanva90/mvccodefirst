// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemToolbarViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The system toolbar action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Model
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The system toolbar action.
    /// </summary>
    public enum SystemToolbarAction
    {
        LogOff,
        Configuration
    }

    /// <summary>
    /// The system toolbar view model.
    /// </summary>
    public class SystemToolbarViewModel : ViewModelUseCaseBase
    {
        public SystemToolbarViewModel()
        {
            this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = EnumSteps.PowerOff.ToString(), Command = new PowerOff() },
                            new ActionCommand { Parameter = EnumSteps.Configuration.ToString(), Command = new Settings() }
                        };
        }
        /// <summary>
        /// The system toolbar click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public delegate void SystemToolbarClick(object sender);

        private ObservableCollection<ActionCommand> _actionCommands;
        public ObservableCollection<ActionCommand> ActionCommands
        {
            get
            {
                return this._actionCommands;
            }
            protected set
            {
                this.SetField(ref _actionCommands, value, () => _actionCommands);
            }
        }

        /// <summary>
        /// The on click button.
        /// </summary>
        public SystemToolbarClick OnClickButton;

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public HyperLinkAction Action { get; set; }

        /// <summary>
        /// The _on click system toolbar.
        /// </summary>
        private ICommand _onClickSystemToolbar;

        /// <summary>
        /// Gets or sets the on click system toolbar.
        /// </summary>
        public ICommand OnClickSystemToolbar
        {
            get
            {
                if (this._onClickSystemToolbar == null)
                {
                    this._onClickSystemToolbar = new RelayCommand(this.DoActionClick);
                }
                return this._onClickSystemToolbar;
            }
            set
            {
                this._onClickSystemToolbar = value;
            }
        }

        /// <summary>
        /// The do action click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        private void DoActionClick(object sender)
        {
            this.OnClickButton(sender);
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
