using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Business.Security;
using Insyston.Operations.Business.Security.Model;
using Insyston.Operations.Logging;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Common.Commands;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Insyston.Operations.WPF.ViewModels.Security.Validation;


namespace Insyston.Operations.WPF.ViewModels.Security
{
    using System.Windows.Media;

    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common;

    public class EditUserViewModel : SubViewModelUseCaseBase<UsersViewModel>
    {
        private UserDetails _SelectedUser;
        private ObservableCollection<LXMGroup> _NotAMemberOfGroups;
        private ObservableCollection<object> _SelectedNotAMemberOfGroups;
        private ObservableCollection<object> _SelectedAMemberOfGroups;

        public EditUserViewModel(UsersViewModel main)
            : base(main)
        {
            this.SelectGroups = new DelegateCommand<ObservableCollection<object>>(this.OnSelectGroups);
            this.DeSelectGroups = new DelegateCommand<ObservableCollection<object>>(this.OnDeselectGroups);
            this.Validator = new EditUserViewModelValidation();
            this.PropertyChanged += this.EditUserViewModel_PropertyChanged;
        }
        public enum EnumSteps
        {
            Start,
            AddGroup,
            RemoveGroup,
            ResetPermissions,
            Cancel,
            Save,
            Error,
            ItemLocked

        }

        public DelegateCommand<ObservableCollection<object>> SelectGroups { get; private set; }

        public DelegateCommand<ObservableCollection<object>> DeSelectGroups { get; private set; }

        //public UserDetails SelectedUser
        //{
        //    get
        //    {
        //        return this._SelectedUser;
        //    }
        //    set
        //    {
        //        if (this.SetField(ref _SelectedUser, value, () => SelectedUser))
        //        {
        //            if (this._SelectedUser != null && this._SelectedUser.UserCredentials != null)
        //            {
        //                this._SelectedUser.UserCredentials.IsEnabledChanged = false;
        //            }
        //        }
        //    }
        //}


        public UserDetails SelectedUser
        {
            get
            {
                return this.MainViewModel.SelectedUser;
            }
            set
            {

                if (SelectedUser != value && SelectedUser != null && SelectedUser.UserCredentials != null)
                {
                    this.MainViewModel.SelectedUser = value;
                    this.SelectedUser.UserCredentials.IsEnabledChanged = false;
                }

            }
        }
        public ObservableCollection<LXMGroup> NotAMemberOfGroups
        {
            get
            {
                return this._NotAMemberOfGroups;
            }
            set
            {
                this.SetField(ref _NotAMemberOfGroups, value, () => NotAMemberOfGroups);
            }
        }

        public ObservableCollection<object> SelectedNotAMemberOfGroups
        {
            get
            {
                return this._SelectedNotAMemberOfGroups;
            }
            set
            {
                if (this.SetField(ref _SelectedNotAMemberOfGroups, value, () => SelectedNotAMemberOfGroups))
                {
                    if (value != null)
                    {
                        this.SelectedNotAMemberOfGroups.CollectionChanged += (o, e) => { this.MainViewModel.IsChanged = true; };
                    }
                }
            }
        }

        public ObservableCollection<object> SelectedAMemberOfGroups
        {
            get
            {
                return this._SelectedAMemberOfGroups;
            }
            set
            {
                if (this.SetField(ref _SelectedAMemberOfGroups, value, () => SelectedAMemberOfGroups))
                {
                    if (value != null)
                    {
                        this.SelectedAMemberOfGroups.CollectionChanged += (o, e) => { this.MainViewModel.IsChanged = true; };
                    }
                }
            }
        }

        public PermissionOption SelectedComponentPermissionOption
        {
            get
            {
                if (this.SelectedUser != null && this.SelectedUser.SelectedComponent != null)
                {
                    return this.SelectedUser.SelectedComponent.PermissionOption;
                }
                else
                {
                    return PermissionOption.All;
                }
            }
            set
            {
                if (this.SelectedUser != null && this.SelectedUser.SelectedComponent != null && this.SelectedUser.SelectedComponent.PermissionOption != value)
                {
                    if (value == PermissionOption.All)
                    {
                        //this.ConfirmationWindow.Raise(new Insyston.Operations.WPF.ViewModels.Common.ConfirmationWindowViewModel(null) { Content = "All subordinate permissions will be reset. Are you sure?", Title = "Apply Permissions Confirmation" },
                        //    (callBack) =>
                        //    {
                        //        if (callBack.Confirmed)
                        //        {
                        //            this.SelectedUser.SelectedComponent.PermissionOption = value;
                        //        }
                        //    });
                        ConfirmationWindowView confirm = new ConfirmationWindowView();
                        ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                        confirmViewModel.Content = "All subordinate permissions will be reset. Are you sure?";
                        confirmViewModel.Title = "Apply Permissions Confirmation";
                        confirmViewModel.DoActionOkClick += OkClickSelectedComponentPermissionOption;
                        confirm.DataContext = confirmViewModel;

                        confirm.ShowDialog();
                    }
                    else
                    {
                        this.SelectedUser.SelectedComponent.PermissionOption = value;
                    }
                    this.OnPropertyChanged(() => SelectedComponentPermissionOption);
                }
            }
        }

        private void OkClickSelectedComponentPermissionOption()
        {
            this.SelectedUser.SelectedComponent.PermissionOption = PermissionOption.All;
        }

        public EnumSteps CurrentStep { get; protected set; }

        public override async Task OnStepAsync(object stepName)
        {
            bool canProcess = true;
            LXMGroup group;
            CurrentStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (CurrentStep)
            {
                case EnumSteps.Start:
                    if (await this.LockAsync() == false)
                    {
                        // Raise event to visible FormMenu if record selected is locked.
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.Users, EnumSteps.ItemLocked);

                        // Change background if record selected is locked when editing.
                        this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");

                        // await this.MainViewModel.OnStepAsync(GroupsViewModel.EnumSteps.CurrentGroup);
                        return;
                    }
                    this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.MainViewModel.ActiveViewModel = this;
                    this.SetActionCommandsAsync();
                    this.SelectedUser.SystemLevelAddDeletePermissionChanged += SelectedUser_SystemLevelAddDeletePermissionChanged;
                    break;
                case EnumSteps.AddGroup:
                    if (this.SelectedNotAMemberOfGroups != null && this.SelectedNotAMemberOfGroups.Count > 0)
                    {
                        List<Task> jobs = new List<Task>();
                        while (this.SelectedNotAMemberOfGroups.Count > 0)
                        {
                            group = (LXMGroup)this.SelectedNotAMemberOfGroups.First();
                            this.SelectedUser.LXMUserGroups.Add(group);
                            EffectiveOptionPermission.AllGroupsOptionPermissions.AddRange(await PermissionFunctions.GetUserEntityOptionPermissionsAsync(group.UserEntityId));
                            this.NotAMemberOfGroups.Remove(group);
                            jobs.Add(UserFunctions.AddGroupAndItsPermissionsAsync(this.SelectedUser, group));
                        }
                        await Task.WhenAll(jobs);
                        Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                        {
                            this.SelectedUser.UpdateLXMUserGroupsString();
                        }));
                    }
                    break;
                case EnumSteps.RemoveGroup:
                    if (this.SelectedAMemberOfGroups != null && this.SelectedAMemberOfGroups.Count > 0)
                    {
                        if (this.Validate(() => CurrentStep))
                        {

                            while (this.SelectedAMemberOfGroups.Count > 0)
                            {
                                group = (LXMGroup)this.SelectedAMemberOfGroups.First();
                                this.NotAMemberOfGroups.Add(group);
                                this.SelectedUser.LXMUserGroups.Remove(group);

                                EffectiveOptionPermission.AllGroupsOptionPermissions.RemoveAll(
                                    p=>p.UserEntityID == group.UserEntityId);
                            
                                await this.RemoveGroupAndPermissionsAsync(group.GroupName);
                            }

                            Application.Current.Dispatcher.InvokeAsync(new Action(() =>
                            {
                                this.SelectedUser.UpdateLXMUserGroupsString();
                            }));
                        }
                    }
                    break;
                case EnumSteps.ResetPermissions:
                    //this.ConfirmationWindow.Raise(new Insyston.Operations.WPF.ViewModels.Common.ConfirmationWindowViewModel(null) { Content = "This will reset all the permissions to None. Are you sure you wish to continue?", Title = "Reset Permissions" },
                    //    (callBack) =>
                    //    {
                    //        if (callBack.Confirmed)
                    //        {
                    //            PermissionFunctions.ResetPermissions(this.SelectedUser.Components);
                    //            this.SelectedUser_ComponentPermissionChanged();
                    //        }
                    //    });
                    //break;
                    ConfirmationWindowView confirm = new ConfirmationWindowView();
                    ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                    confirmViewModel.Content = "This resets all your permissions to None. Are you sure you wish to continue?";
                    confirmViewModel.Title = "Reset Permissions";
                    confirmViewModel.DoActionOkClick += OkClickResetPermission;
                    confirm.DataContext = confirmViewModel;

                    confirm.ShowDialog();
                    break;
                case EnumSteps.Cancel:
                    canProcess = await this.MainViewModel.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        // Just do UnLockAsync if not in mode Add.
                        if (!this.SelectedUser.IsNewUser)
                        {
                            await this.UnLockAsync();
                        }
                        else
                        {
                            this.IsCheckedOut = false;
                            this.IsChanged = false;
                        }

                        // await this.UnLockAsync();
                        // Raise event to change style for hyperlink
                        this.MainViewModel.ValidateNotError();
                        this.MainViewModel.IsCheckedOut = this.IsCheckedOut;
                        this.MainViewModel.IsChanged = false;                        
                        if (this.SelectedUser.IsNewUser)
                        {
                            this.MainViewModel.OnCancelNewItem(EnumScreen.Users);
                        }
                        if (this.SelectedUser.IsNewUser && this.MainViewModel.isCopy == false)
                        {
                            this.SelectedUser = await UserFunctions.AddNewUserAsync();
                        }
                        this.SelectedUser.CurrentPostalStateID = null;
                        this.SelectedUser.CurrentStateID = null;
                        this.MainViewModel.isCopy = false;
                        await this.MainViewModel.OnStepAsync(UsersViewModel.EnumSteps.SelectUser);
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.Users, EnumSteps.Cancel);
                    }
                    break;
                case EnumSteps.Save:
                    try
                    {
                        this.Validate();
                        if (this.HasErrors == false)
                        {
                            // Raise event to change style for hyperlink
                            this.MainViewModel.ValidateNotError();
                            await UserFunctions.SaveAsync(this.SelectedUser);
                            if (!this.SelectedUser.IsNewUser)
                            {
                                await this.UnLockAsync();
                            }
                            else
                            {
                                this.IsCheckedOut = false;
                                this.IsChanged = false;
                            }
                            this.SelectedUser.IsNewUser = false;
                            // await this.UnLockAsync();
                            this.MainViewModel.IsCheckedOut = this.IsCheckedOut;
                            this.MainViewModel.IsChanged = false;
                            this.MainViewModel.isCopy = false;
                            await this.MainViewModel.OnStepAsync(UsersViewModel.EnumSteps.SelectUser);
                            await this.MainViewModel.OnStepAsync(UsersViewModel.EnumSteps.RefreshUser);
                            this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.Users, EnumSteps.Save, this.SelectedUser);
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
                        this.ShowErrorMessage("Error encountered while Saving User.", "User");
                    }
                    break;
                case EnumSteps.Error:
                    // Show NotificationWindow when click Error button.
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
            PermissionFunctions.ResetPermissions(this.SelectedUser.Components);
            this.SelectedUser_ComponentPermissionChanged();
        }

        void SelectedUser_SystemLevelAddDeletePermissionChanged()
        {
            //this.ConfirmationWindow.Raise(new Insyston.Operations.WPF.ViewModels.Common.ConfirmationWindowViewModel(null) { Content = "All subordinate permissions will be reset. Are you sure?", Title = "Apply Permissions Confirmation" },
            //           (callBack) =>
            //           {
            //               if (callBack.Confirmed)
            //               {
            //                   this.SelectedUser.SelectedComponent.SetSystemLevelAddAndDeletePermissions();
            //               }
            //           });
            ConfirmationWindowView confirm = new ConfirmationWindowView();
            ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
            confirmViewModel.Content = "All subordinate permissions will be reset. Are you sure?";
            confirmViewModel.Title = "Apply Permissions Confirmation";
            confirmViewModel.DoActionOkClick += OkClickSystemLevelAddDeletePermissionChanged;
            confirm.DataContext = confirmViewModel;

            confirm.ShowDialog();
        }

        private void OkClickSystemLevelAddDeletePermissionChanged()
        {
            this.SelectedUser.SelectedComponent.SetSystemLevelAddAndDeletePermissions();
        }

        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    this.UnLockAsync();
                    if (this.NotAMemberOfGroups != null)
                    {
                        //this.NotAMemberOfGroups.Dispose();
                        this.NotAMemberOfGroups.Clear();
                        this.NotAMemberOfGroups = null;
                    }
                    if (this.SelectedNotAMemberOfGroups != null)
                    {
                        // this.SelectedNotAMemberOfGroups.Dispose();
                        this.SelectedNotAMemberOfGroups.Clear();
                        this.SelectedNotAMemberOfGroups = null;
                    }
                    if (this.SelectedAMemberOfGroups != null)
                    {
                        //  this.SelectedAMemberOfGroups.Dispose();
                        this.SelectedAMemberOfGroups.Clear();
                        this.SelectedAMemberOfGroups = null;
                    }
                    if (this.SelectedUser != null)
                    {
                        this.SelectedUser.Dispose();
                        this.SelectedUser = null;
                    }
                    base.Dispose();
                }));
        }

        internal void SelectedUser_ComponentPermissionChanged()
        {
            this.OnPropertyChanged(() => SelectedComponentPermissionOption);
        }

        protected override async void SetActionCommandsAsync()
        {
            if (CurrentStep.Equals(EnumSteps.Error))
            {
                if (this.MainViewModel.CurrentStep.Equals(UsersViewModel.EnumSteps.PermissionsState))
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
                if (this.MainViewModel.CurrentStep.Equals(UsersViewModel.EnumSteps.PermissionsState))
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
            if (this.SelectedUser != null && this.SelectedUser.UserCredentials != null)
            {
                await base.UnLockAsync("LXMUser", this.SelectedUser.UserCredentials.UserId.ToString(), this.InstanceGUID);
            }
        }

        protected override async Task<bool> LockAsync()
        {
            if (this.SelectedUser != null && this.SelectedUser.UserCredentials != null)
            {
                return await base.LockAsync("LXMUser", this.SelectedUser.UserCredentials.UserId.ToString(), this.InstanceGUID);
                //return await base.LockAsync("LXMUser", this.SelectedUser.UserCredentials.UserId.ToString(),this.InstanceGUID);
            }
            return true;
        }

        private async Task RemoveGroupAndPermissionsAsync(string sourceName)
        {
                IEnumerable<Model.Component> flattennedUserComponents = await PermissionFunctions.GetFlattennedComponentsAsync(this.SelectedUser.Components);


                await Task.WhenAll(from Model.Component component in flattennedUserComponents
                                   from TreeForm form in component.Forms
                                   select RemoveGroupAndPermissionsAsync(sourceName, component, form));

                foreach (TreeComponent component in flattennedUserComponents)
                {
                    component.UpdatePermissionTypeAndOptionBasedOnForms();
                }
        }

        private async Task RemoveGroupAndPermissionsAsync(string sourceName, Model.Component component, TreeForm form)
        {
            if (form.Source == sourceName)
            {
                List<IGrouping<int?, FormPermission>> permission = await PermissionFunctions.GetGroupingPermissionsByUserEntityIDsAsync(
                    this.SelectedUser.LXMUserGroups,
                    SelectedUser.UserEntityId, form);

                if (permission == null || permission.Count == 0)
                {
                    form.PermissionType = PermissionType.None;
                    form.Source = "User";
                }
                else
                {
                    StringBuilder source = new StringBuilder();
                    form.PermissionType = permission.First().Key.HasValue ? (PermissionType)permission.First().Key.Value : PermissionType.None;
                    foreach (FormPermission current in permission.First())
                    {
                        if (current.UserEntityID != SelectedUser.UserEntityId)
                        {
                            source.Append(SelectedUser.LXMUserGroups.FirstOrDefault(g => g.UserEntityId == current.UserEntityID).GroupName + ",");
                        }
                    }

                    if (source.Length == 0)
                    {
                        source.Append("User");
                        form.GroupPermissionType = null;
                    }
                    else
                    {
                        source.Remove(source.Length - 1, 1);
                        form.GroupPermissionType = form.PermissionType;
                    }
                    form.Source = source.ToString();
                }
            }
            else if (form.Source.Contains(string.Format(",{0}", sourceName)))
            {
                form.Source = form.Source.Replace(string.Format(",{0}", sourceName), string.Empty);
            }
            else if (form.Source.Contains(string.Format("{0},", sourceName)))
            {
                form.Source = form.Source.Replace(string.Format("{0},", sourceName), string.Empty);
            }
        }
        private void OnSelectGroups(ObservableCollection<object> selectedItems)
        {
            this.SelectedAMemberOfGroups = selectedItems;
        }

        private void OnDeselectGroups(ObservableCollection<object> selectedItems)
        {
            this.SelectedNotAMemberOfGroups = selectedItems;
        }

        private void EditUserViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Validate(e.PropertyName);
        }
    }
}
