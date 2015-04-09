// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditMembershipViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The edit membership view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Security
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;

    using Insyston.Operations.Business.Security.Model;
    using Insyston.Operations.Logging;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The edit membership view model.
    /// </summary>
    public class EditMembershipViewModel : SubViewModelUseCaseBase<MembershipViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditMembershipViewModel"/> class.
        /// </summary>
        /// <param name="main">
        /// The main.
        /// </param>
        public EditMembershipViewModel(MembershipViewModel main)
            : base(main)
        {
        }

        /// <summary>
        /// The enum steps.
        /// </summary>
        public enum EnumSteps
        {
            Start,
            Save,
            Cancel,
        }

        /// <summary>
        /// Gets the available collector list.
        /// </summary>
        public ObservableCollection<Membership> AvailableUsersList
        {
            get
            {
                return this.MainViewModel.AvailableUsersList;
            }
        }

        /// <summary>
        /// The on step async.
        /// </summary>
        /// <param name="stepName">
        /// The step name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override async Task OnStepAsync(object stepName)
        {
            bool canProcess = true;
            var currentStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (currentStep)
            {
                case EnumSteps.Start:
                    if (await this.LockAsync() == false)
                    {
                        return;
                    }
                    this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.MainViewModel.ActiveViewModel = this;
                    this.MainViewModel.IsCheckedOut = this.IsCheckedOut = true;

                    // raise the properties belong to currently collection queue changed.
                    this.SetActionCommandsAsync();
                    break;
                case EnumSteps.Cancel:
                    canProcess = await this.MainViewModel.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        await this.UnLockAsync();
                        this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                        this.MainViewModel.IsCheckedOut = this.IsCheckedOut = false;
                        this.MainViewModel.IsChanged = false;
                        await this.MainViewModel.OnStepAsync(MembershipViewModel.EnumSteps.Reset);
                    }
                    break;
                case EnumSteps.Save:
                    try
                    {
                        this.Validate();
                        if (this.HasErrors == false)
                        {
                            this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                            this.MainViewModel.ValidateNotError();
                            await this.MainViewModel.SaveAllGroups();
                            await this.UnLockAsync();
                            this.MainViewModel.IsCheckedOut = this.IsCheckedOut = false;
                            this.MainViewModel.IsChanged = false;
                            await this.MainViewModel.OnStepAsync(MembershipViewModel.EnumSteps.Refresh);
                            this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.Membership, EnumSteps.Save);
                        }
                    }
                    catch (Exception exc)
                    {
                        ExceptionLogger.WriteLog(exc);
                        this.ShowErrorMessage("Error encountered while Saving Group", "Group");
                    }
                    break;
            }
            if (canProcess)
            {
                this.OnStepChanged(currentStep.ToString());
            }
        }

        /// <summary>
        /// The set action commands async.
        /// </summary>
        protected override async void SetActionCommandsAsync()
        {
            this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() },
                    };
        }

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task UnLockAsync()
        {
            await base.UnLockAsync("LXMUserEntityRelation", "-1", this.InstanceGUID);
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task<bool> LockAsync()
        {
            return await base.LockAsync("LXMUserEntityRelation", "-1", this.InstanceGUID);
        }
    }
}
