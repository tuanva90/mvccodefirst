using System;
using System.Linq;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Business.Security;
using Insyston.Operations.Business.Security.Model;
using Insyston.Operations.Logging;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Common.Commands;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Insyston.Operations.WPF.ViewModels.Security.Validation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Insyston.Operations.WPF.ViewModels.Security
{
    using System.Windows;
    using System.Windows.Media;

    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common;

    public class EditGroupViewModel : SubViewModelUseCaseBase<GroupsViewModel>
    {
        private GroupDetails _SelectedGroup;
        private ObservableCollection<object> _SelectedMembers;
        private ObservableCollection<object> _SelectedNotMembers;
        private ObservableCollection<LXMUserDetail> _NotMembers;

        public EditGroupViewModel(GroupsViewModel main) : base(main)
        {
            this.SelectMembers = new DelegateCommand<ObservableCollection<object>>(this.OnSelectMembers);
            this.SelectNonMembers = new DelegateCommand<ObservableCollection<object>>(this.OnSelectNonMembers);
            this.Validator = new EditGroupViewModelValidation();
            this.PropertyChanged += this.EditGroupViewModel_PropertyChanged;
        }
        
        public enum EnumSteps
        {
            Start,
            AddMember,
            RemoveMember,
            ResetPermissions,
            Cancel,
            Save,
            Error,
            ItemLocked
        }

        public DelegateCommand<ObservableCollection<object>> SelectMembers { get; private set; }

        public DelegateCommand<ObservableCollection<object>> SelectNonMembers { get; private set; }

        //public GroupDetails SelectedGroup
        //{
        //    get
        //    {
        //        return this._SelectedGroup;
        //    }
        //    set
        //    {
        //        this.SetField(ref _SelectedGroup, value, () => SelectedGroup);
        //    }
        //}
        public GroupDetails SelectedGroup
        {
            get
            {
                return this.MainViewModel.SelectedGroup;
            }
            set
            {
                this.MainViewModel.SetSelectedGroupValue(value);
            }
        }
        public ObservableCollection<object> SelectedMembers
        {
            get
            {
                return this._SelectedMembers;
            }
            set
            {
                this.SetField(ref _SelectedMembers, value, () => SelectedMembers);
                if (SelectedMembers != null)
                {
                    this.SelectedMembers.CollectionChanged += (o, e) => { this.MainViewModel.isChanged = true; };
                }
            }
        }

        public ObservableCollection<object> SelectedNotMembers
        {
            get
            {
                return this._SelectedNotMembers;
            }
            set
            {
                this.SetField(ref _SelectedNotMembers, value, () => SelectedNotMembers);
                if (SelectedNotMembers != null)
                {
                    this.SelectedNotMembers.CollectionChanged += (o, e) => { this.MainViewModel.isChanged = true; };
                }
            }
        }

        public ObservableCollection<LXMUserDetail> NotMembers
        {
            get
            {
                return this._NotMembers;
            }
            set
            {
                this.SetField(ref _NotMembers, value, () => NotMembers);                
            }
        }

        public PermissionOption SelectedComponentPermissionOption
        {
            get
            {
                if (this.SelectedGroup != null && this.SelectedGroup.SelectedComponent != null)
                {
                    return this.SelectedGroup.SelectedComponent.PermissionOption;
                }
                else
                {
                    return PermissionOption.All;
                }
            }
            set
            {
                if (this.SelectedGroup != null && this.SelectedGroup.SelectedComponent != null && this.SelectedGroup.SelectedComponent.PermissionOption != value)
                {
                    if (value == PermissionOption.All)
                    {
                        //this.ConfirmationWindow.Raise(new Insyston.Operations.WPF.ViewModels.Common.ConfirmationWindowViewModel(null) { Content = "All subordinate permissions will be reset. Are you sure?", Title = "Apply Permissions Confirmation" },
                        //    (callBack) =>
                        //    {
                        //        if (callBack.Confirmed)
                        //        {
                        //            this.SelectedGroup.SelectedComponent.PermissionOption = value;                        
                        //        }
                        //    });
                        ConfirmationWindowView confirm = new ConfirmationWindowView();
                        ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                        confirmViewModel.Content = "All subordinate permissions will be reset. Are you sure?";
                        confirmViewModel.Title = "Apply Permissions Confirmation";
                        confirm.DataContext = confirmViewModel;

                        confirm.ShowDialog();
                        if (confirm.DialogResult == true)
                        {
                            this.SelectedGroup.SelectedComponent.PermissionOption = value;
                        }
                    }
                    else
                    {
                        this.SelectedGroup.SelectedComponent.PermissionOption = value;
                    }

                    this.OnPropertyChanged(() => this.SelectedComponentPermissionOption);
                }
            }
        }

        public EnumSteps CurrentStep { get; protected set; }
        public override async Task OnStepAsync(object stepName)
        {
            LXMUserDetail user;
            bool canProcess = true;
            CurrentStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (CurrentStep)
            {
                case EnumSteps.Start:
                    if (await this.LockAsync() == false)
                    {
                        // Raise event to visible FormMenu if record selected is locked.
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.Groups, EnumSteps.ItemLocked);

                        // Change background if record selected is locked when editing.
                        this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");

                        // await this.MainViewModel.OnStepAsync(GroupsViewModel.EnumSteps.CurrentGroup);
                        return;
                    }
                    this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.MainViewModel.ActiveViewModel = this;
                    this.SetActionCommandsAsync();
                    this.SelectedGroup.SystemLevelAddDeletePermissionChanged += SelectedUser_SystemLevelAddDeletePermissionChanged;
                    break;
                case EnumSteps.AddMember:
                    if (this.SelectedNotMembers != null && this.SelectedNotMembers.Count > 0)
                    {
                        while (this.SelectedNotMembers.Count > 0)
                        {
                            user = (LXMUserDetail)this.SelectedNotMembers.First();
                            this.SelectedGroup.Users.Add(user);
                            this.NotMembers.Remove(user);
                        }
                    }
                    break;
                case EnumSteps.RemoveMember:
                    if (this.SelectedMembers != null && this.SelectedMembers.Count > 0)
                    {                        
                        if (this.Validate(() => CurrentStep))
                        {
                            while (this.SelectedMembers.Count > 0)
                            {
                                user = (LXMUserDetail)this.SelectedMembers.First();
                                this.SelectedGroup.Users.Remove(user);
                                this.NotMembers.Add(user);
                            }
                        }
                    }
                    break;
                case EnumSteps.ResetPermissions:
                    //this.ConfirmationWindow.Raise(new Insyston.Operations.WPF.ViewModels.Common.ConfirmationWindowViewModel(null) { Content = "This resets all your permissions to None, Do you confirm this?", Title = "Reset Permissions" },
                    //    (callBack) =>
                    //    {
                    //        if (callBack.Confirmed)
                    //        {
                    //            PermissionFunctions.ResetPermissions(this.SelectedGroup.Components);
                    //            this.SelectedGroup_ComponentPermissionChanged();
                    //        }
                    //    });   
                    ConfirmationWindowView confirm = new ConfirmationWindowView();
                    ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                    confirmViewModel.Content = "This resets all your permissions to None, Do you confirm this?";
                    confirmViewModel.Title = "Reset Permissions";
                    confirmViewModel.DoActionOkClick += OkClickResetPermission;
                    confirm.DataContext = confirmViewModel;

                    confirm.ShowDialog();
                    break;
                case EnumSteps.Cancel:
                    canProcess = await this.MainViewModel.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                        this.MainViewModel.ValidateNotError();

                        // Just do UnLockAsync if not in mode Add.
                        if (!this.SelectedGroup.IsNewGroup)
                        {
                            await this.UnLockAsync();
                        }
                        else
                        {
                            this.IsCheckedOut = false;
                            this.IsChanged = false;
                        }

                        // await this.UnLockAsync();
                        
                        if (this.SelectedGroup.IsNewGroup)
                        {
                            this.MainViewModel.OnCancelNewItem(EnumScreen.Groups);
                        }
                        if (this.SelectedGroup.IsNewGroup && this.MainViewModel.isCopy == false)
                        {
                            this.SelectedGroup = await GroupFunctions.AddNewGroupAsync();
                        }
                        if (this.MainViewModel.isChanged)
                        {
                            this.MainViewModel.isChanged = false;
                            await this.MainViewModel.OnStepAsync(GroupsViewModel.EnumSteps.SelectGroup);
                        }
                        else
                        {
                            await this.MainViewModel.OnStepAsync(GroupsViewModel.EnumSteps.CurrentGroup);
                        }
                        this.MainViewModel.isCopy = false;
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.Groups, EnumSteps.Cancel);
                    }
                    break;
                case EnumSteps.Save:
                    try
                    {
                        this.Validate();
                        if (this.HasErrors == false)
                        {
                            this.MainViewModel.ValidateNotError();
                            this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                            await GroupFunctions.SaveAsync(this.SelectedGroup);
                            if (!this.SelectedGroup.IsNewGroup)
                            {
                                await this.UnLockAsync();
                            }
                            else
                            {
                                this.IsCheckedOut = false;
                                this.IsChanged = false;
                            }
                            this.MainViewModel.IsCheckedOut = this.IsCheckedOut;
                            this.SelectedGroup.IsNewGroup = false;
                            this.MainViewModel.isCopy = false;
                            this.MainViewModel.isChanged = false;
                            await this.MainViewModel.OnStepAsync(GroupsViewModel.EnumSteps.SelectGroup);
                            await this.MainViewModel.OnStepAsync(GroupsViewModel.EnumSteps.RefreshGroup);
                            this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.Groups, EnumSteps.Save, this.SelectedGroup);
                        }
                        else
                        {
                            CurrentStep = EnumSteps.Error;
                            this.SetActionCommandsAsync();
                            this.MainViewModel.ListErrorHyperlink = this.ListErrorHyperlink;
                            this.MainViewModel.OnErrorHyperlinkSelected();
                        }
                    }
                    catch (Exception exc)
                    {
                        ExceptionLogger.WriteLog(exc);
                        this.ShowErrorMessage("Error encountered while Saving Group", "Group");
                    }
                    break;
                case EnumSteps.Error:
                    NotificationErrorView errorPopup = new NotificationErrorView();
                    NotificationErrorViewModel errorPopupViewModel = new NotificationErrorViewModel();
                    errorPopupViewModel.listCustomHyperlink = this.MainViewModel.ListErrorHyperlink;
                    errorPopup.DataContext = errorPopupViewModel;

                    errorPopup.Style = (Style)Application.Current.FindResource("RadWindowStyleNew");

                    errorPopup.ShowDialog();
                    break;
            }
            if (canProcess)
            {
                this.OnStepChanged(CurrentStep.ToString());
            }
        }

        private void OkClickResetPermission()
        {
            PermissionFunctions.ResetPermissions(this.SelectedGroup.Components);
            this.SelectedGroup_ComponentPermissionChanged();
        }

        void SelectedUser_SystemLevelAddDeletePermissionChanged()
        {
            //this.ConfirmationWindow.Raise(new Insyston.Operations.WPF.ViewModels.Common.ConfirmationWindowViewModel(null) { Content = "All subordinate permissions will be reset. Are you sure?", Title = "Apply Permissions Confirmation" },
            //           (callBack) =>
            //           {
            //               if (callBack.Confirmed)
            //               {
            //                   this.SelectedGroup.SelectedComponent.SetSystemLevelAddAndDeletePermissions();
            //               }
            //           });

            ConfirmationWindowView confirm = new ConfirmationWindowView();
            ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
            confirmViewModel.Content = "All subordinate permissions will be reset. Are you sure?";
            confirmViewModel.Title = "Apply Permissions Confirmation";
            confirm.DataContext = confirmViewModel;

            confirm.ShowDialog();
            if (confirm.DialogResult == true)
            {
                this.SelectedGroup.SelectedComponent.SetSystemLevelAddAndDeletePermissions();
            }
        }
        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    this.UnLockAsync();
                    if (this.SelectedMembers != null)
                    {
                        //this.SelectedMembers.Dispose();
                        this.SelectedMembers.Clear();
                        this.SelectedMembers = null;
                    }
                    if (this.SelectedNotMembers != null)
                    {
                        //this.SelectedNotMembers.Dispose();
                        this.SelectedNotMembers.Clear();
                        this.SelectedNotMembers = null;
                    }
                    if (this.NotMembers != null)
                    {
                        //this.NotMembers.Dispose();
                        this.NotMembers.Clear();
                        this.NotMembers = null;
                    }
                    if (this.SelectedGroup != null)
                    {
                        this.SelectedGroup.Dispose();
                        this.SelectedGroup = null;
                    }
                    base.Dispose();
                }));
        }

        internal void SelectedGroup_ComponentPermissionChanged()
        {
            this.OnPropertyChanged(() => SelectedComponentPermissionOption);
        }

        protected override async void SetActionCommandsAsync()
        {
            if (CurrentStep.Equals(EnumSteps.Error))
            {
                if (this.MainViewModel.CurrentStep.Equals(GroupsViewModel.EnumSteps.PermissionsState))
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {   
                        new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() },
                        new ActionCommand { Parameter = EnumSteps.ResetPermissions.ToString(), Command = new ResetPermissions() },
                        new ActionCommand { Parameter = EnumSteps.Error.ToString(), Command = new Error() }
                    };
                }
                else
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {   
                        new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() },
                        new ActionCommand { Parameter = EnumSteps.Error.ToString(), Command = new Error() }
                    };
                }

            }
            else
            {
                if (this.MainViewModel.CurrentStep.Equals(GroupsViewModel.EnumSteps.PermissionsState))
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {   
                        new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() },
                        new ActionCommand { Parameter = EnumSteps.ResetPermissions.ToString(), Command = new ResetPermissions() },
                    };
                }
                else
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {   
                        new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() },
                    };
                }
            }
        }

        protected override async Task UnLockAsync()
        {
            if (this.SelectedGroup != null && this.SelectedGroup.LXMGroup != null)
            {
                await base.UnLockAsync("LXMGroup", this.SelectedGroup.LXMGroup.GroupId.ToString(), this.InstanceGUID);
            }
        }

        protected override async Task<bool> LockAsync()
        {
            if (this.SelectedGroup != null && this.SelectedGroup.LXMGroup != null)
            {
                return await base.LockAsync("LXMGroup", this.SelectedGroup.LXMGroup.GroupId.ToString(), this.InstanceGUID);
            }
            return true;
        }

        private void OnSelectMembers(ObservableCollection<object> selectedItems)
        {
            this.SelectedMembers = selectedItems;
        }

        private void OnSelectNonMembers(ObservableCollection<object> selectedItems)
        {
            this.SelectedNotMembers = selectedItems;
        }

        private void EditGroupViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Validate(e.PropertyName);
        }
    }
}
