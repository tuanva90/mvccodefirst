using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Collections
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Insyston.Operations.Business.Collections.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Collections.Validation;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    public class EditCollectorsViewModel : SubViewModelUseCaseBase<CollectorsViewModel>
    {
        public EditCollectorsViewModel(CollectorsViewModel main)
            : base(main)
        {
            this.Validator = new CollectionsManagementViewModelValidation();
        }

        public enum EnumSteps
        {
            Start,
            AddMember,
            ResetPermissions,
            Cancel,
            Save,
        }
        public ObservableCollection<Collectors> AvailableCollectorList
        {
            get
            {
                return this.MainViewModel.AvailableCollectorList;
            }
        }

        public override async Task OnStepAsync(object stepName)
        {
            bool canProcess = true;
            var CurrentStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (CurrentStep)
            {
                case EnumSteps.Start:
                    if (await this.LockAsync() == false)
                    {
                        return;
                    }
                    this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.MainViewModel.ActiveViewModel = this;
                    this.ValidationSummary.Clear();
                    this.MainViewModel.IsCheckedOut = this.IsCheckedOut = true;
                    this.SetActionCommandsAsync();
                    break;
                case EnumSteps.Cancel:
                    canProcess = await this.MainViewModel.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        await this.UnLockAsync();
                        this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.Collectors, EnumSteps.Cancel);
                        this.MainViewModel.IsCheckedOut = this.IsCheckedOut = false;
                        this.MainViewModel.IsChanged = false;
                        await this.MainViewModel.OnStepAsync(CollectorsViewModel.EnumSteps.ResetCollectors);
                    }
                    break;
                case EnumSteps.Save:
                    this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                    await this.MainViewModel.SaveAllCollectors();
                    await this.UnLockAsync();
                    this.MainViewModel.IsCheckedOut = this.IsCheckedOut = false;
                    this.MainViewModel.IsChanged = false;
                    await this.MainViewModel.OnStepAsync(CollectorsViewModel.EnumSteps.RefreshQueue);
                    this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.Collectors, EnumSteps.Save);
                    break;
            }
            if (canProcess)
            {
                this.OnStepChanged(CurrentStep.ToString());
            }
        }

        protected override async Task UnLockAsync()
        {
            await base.UnLockAsync("xrefCollectionQueueCollector", "-1", this.InstanceGUID);
        }

        protected override async Task<bool> LockAsync()
        {
            return await base.LockAsync("xrefCollectionQueueCollector", "-1", this.InstanceGUID);
        }

        protected override async void SetActionCommandsAsync()
        {
            this.ActionCommands = new ObservableCollection<ActionCommand>
            {
                new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() },
            };
        }
    }
}
