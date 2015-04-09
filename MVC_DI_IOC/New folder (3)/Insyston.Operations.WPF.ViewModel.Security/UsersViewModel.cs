using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Insyston.Operations.Business.Common;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Business.Security;
using Insyston.Operations.Business.Security.Model;
using Insyston.Operations.Model;
using Insyston.Operations.Security;
using Insyston.Operations.WPF.ViewModels.Common.Commands;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Insyston.Operations.WPF.ViewModels.Security
{
    using System.Windows.Media;

    using Insyston.Operations.WPF.ViewModels.Common;

    public delegate void StoryBoardChanged(string storyBoard);

    public class UsersViewModel : ViewModelUseCaseBase
    {
       public delegate void OnLoadUserContent();
        public OnLoadUserContent _OnLoadUserContent;
        internal bool isCopy;
        private EnumSteps _CurrentEnumStep;
        private ObservableCollection<LXMGroup> _AllGroups;
        private ObservableCollection<UserDetails> _AllUsers;
        private ObservableCollection<SystemParam> _States;
        private const string LoadingText = "Please Wait Loading ...";

        /// <summary>
        /// The _ selected user.
        /// </summary>
        private UserDetails _SelectedUser;
        private bool _IsUserSelected;
        private bool _IsSetSystemOptionPermission;
        private List<int> SystemOptionPermissionID = new List<int>() ;//{ 21, 23, 27 };
        public UsersViewModel()
        {

            this.InstanceGUID = Guid.NewGuid();
         
            this.Edit = new EditUserViewModel(this);
            this.PropertyChanged += this.UsersViewModel_PropertyChanged;
        }

        public event StoryBoardChanged OnStoryBoardChanged;

        // Action select user
        /// <summary>
        /// The on select user.
        /// </summary>

        public enum EnumSteps
        {
            Start,
            SelectUser,
            PersonalDetailsState,
            CredentialsState,
            GroupsState,
            PermissionsState,
            SummaryState,
            Edit,
            Add,
            Copy,
            RefreshUser,
            Cancel
        }

        public EditUserViewModel Edit { get; private set; }

        public ObservableCollection<LXMGroup> AllGroups
        {
            get
            {
                return this._AllGroups;
            }
            set
            {
                this.SetField(ref _AllGroups, value, () => AllGroups);
            }
        }

        public ObservableCollection<UserDetails> AllUsers
        {
            get
            {
                return this._AllUsers;
            }
            set
            {
                this.SetField(ref _AllUsers, value, () => AllUsers);
            }
        }

        public UserDetails SelectedUser
        {
            get
            {
                return this._SelectedUser;
            }
            set
            {
                SetSelectedUserAsync(value);
                //don't put any logic here, logic should be defined in SetSelectedUser() method
            }
        }

        private async Task SetSelectedUserAsync(UserDetails value)
        {
            bool canProceed = true;

            if (this.ActiveViewModel != null && this.ActiveViewModel.IsCheckedOut && this.IsChanged)
            {
                //this.ActiveViewModel.ConfirmationWindow.Raise(new Insyston.Operations.WPF.ViewModels.Common.ConfirmationWindowViewModel(null) { Content = "Are you sure to proceed without saving?", Title = "Confirm Save - Group" },
                //    (callBack) =>
                //    {
                //        if (callBack.Confirmed == false)
                //        {
                //            canProceed = false;
                //        }
                //    });
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Users";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
                else
                {
                    this.IsChanged = false;
                }
            }

            if (canProceed)
            {
                //if (this.Edit.IsCheckedOut)
                //{
                //    await this.Edit.OnStepAsync(EnumSteps.Cancel);
                //}

                // Raise event to change style for hyperlink when select another record.
                this.ValidateNotError();
                this.IsChanged = false;
                this.ActiveViewModel.IsCheckedOut = false;
                // Just do UnLockAsync if not in mode Add.
                if (value != null && !value.IsNewUser)
                {
                    if (this.SelectedUser != null && this.SelectedUser.UserCredentials != null)
                    {
                        await UnLockAsync();
                    }
                }

                this.SetField(ref _SelectedUser, value, () => SelectedUser);

                if (this._SelectedUser != null)
                {
                    this._SelectedUser.ComponentPermissionChanged += this.Edit.SelectedUser_ComponentPermissionChanged;
                    this._SelectedUser.ComponentPermissionChanged += () => { this.OnPropertyChanged(() => SelectedComponentPermissionOption); };
                }
                if (value != null)
                {
                    await OnStepAsync(EnumSteps.SelectUser);
                }
            }
        }

        public ObservableCollection<SystemParam> States
        {
            get
            {
                return this._States;
            }
            set
            {
                this.SetField(ref _States, value, () => States);
            }
        }

        public async Task<LXMUserDefaultSetting> GetUserDefaultSettingsAsync()
        {
            return await UserFunctions.GetUserDefaultSortFilterSettingsAsync("Users");
        }

        public bool IsUserSelected
        {
            get
            {
                return this._IsUserSelected;
            }
            set
            {
                if (this.SetField(ref _IsUserSelected, value, () => IsUserSelected))
                {
                    this.SetActionCommandsAsync();
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
            }
        }

        public PermissionOption IsAddIsDeletePermissionOption
        {
            get
            {
                if (this.SelectedUser != null && this.SelectedUser.SelectedComponent != null)
                {
                    return this.SelectedUser.SelectedComponent.IsAddIsDeletePermissionOption;
                }
                else
                {
                    return PermissionOption.All;
                }
            }
            private set
            {
            }
        }

        public Action ReSizeGrid;

        public void GetResizeGrid()
        {
            if (ReSizeGrid != null)
            {
                ReSizeGrid();
            }
        }

        /// <summary>
        /// The unlock item.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task UnlockItem()
        {
            return UnLockAsync();
        }

        /// <summary>
        /// The check content is editing.
        /// Calling from DocumentTab
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<bool> CheckContentEditing()
        {
            if (this.ActiveViewModel.IsCheckedOut && this.IsChanged)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public EnumSteps CurrentStep { get; private set; }
        public override async Task OnStepAsync(object stepName)
        {
            UserDetails user;
            this._CurrentEnumStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (this._CurrentEnumStep)
            {
                case EnumSteps.Start:
                    this.SetBusyAction(LoadingText);
                    await Task.WhenAll(this.PopulateStatesAsync(), this.PopulateAllGroupsAsync());
                    await this.PopulateAllUsersForViewAsync();
                    this.ResetBusyAction();
                    //this._OnLoadUserContent();
                    break;
                case EnumSteps.SelectUser:
                    this.SetBusyAction(LoadingText);
                    this.Edit.SetBusyAction(LoadingText);
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                    if (this.SelectedUser.IsNewUser == false)
                    {
                        this.SelectedUser.LXMUserDetails = await UserFunctions.GetUserDetailsAsync(this.SelectedUser.UserEntityId);
                        await this.PopulateGroupsForUserAsync();
                        
                    }

                    await this.PopulateCredentialsForUserAsync();

                    if (this.isCopy == false)
                    {
                        await this.PopulatePermissionsForUserAsync();
                    }

                    this.ActiveViewModel = this;
                    this.SetActionCommandsAsync();
                    if (this.IsCheckedOut)
                    {
                        await this.OnStepAsync(EnumSteps.Edit.ToString());
                    }
                    this.SelectedUser.SystemOptionPermissionChanged -= SelectedUser_SystemOptionPermissionChanged;
                    this.SelectedUser.SystemOptionPermissionChanged += SelectedUser_SystemOptionPermissionChanged;
                    this.Edit.ResetBusyAction();
                    this.ResetBusyAction();
                    if (this.SelectedUser.IsNewUser == false)
                    {
                        this.RaiseActionsWhenChangeStep(EnumScreen.Users, EnumSteps.SelectUser, this.SelectedUser);
                    }
                    break;
                case EnumSteps.PersonalDetailsState:
                    this.CurrentStep = EnumSteps.PersonalDetailsState;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._CurrentEnumStep.ToString());
                    }
                    break;
                case EnumSteps.CredentialsState:
                    this.CurrentStep = EnumSteps.CredentialsState;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._CurrentEnumStep.ToString());
                    }
                    break;
                case EnumSteps.GroupsState:
                    this.CurrentStep = EnumSteps.GroupsState;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._CurrentEnumStep.ToString());
                    }
                    break;
                case EnumSteps.PermissionsState:
                    this.CurrentStep = EnumSteps.PermissionsState;
                    //case EnumSteps.SummaryState:
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._CurrentEnumStep.ToString());
                    }
                    break;
                case EnumSteps.Edit:
                    //if (await this.LockAsync() == false)
                    //{
                    //    return;
                    //}
                    this.RaiseActionsWhenChangeStep(EnumScreen.Users, EnumSteps.Edit);
                    this.Edit.SelectedUser = this.SelectedUser;
                    this.Edit.InstanceGUID = this.InstanceGUID;
                    // this.Edit.NotAMemberOfGroups = new ObservableCollection<LXMGroup>(this.AllGroups.Except(this.SelectedUser.LXMUserGroups));
                    await this.Edit.OnStepAsync(EditUserViewModel.EnumSteps.Start);
                    break;
                case EnumSteps.Add:
                    this.RaiseActionsWhenChangeStep(EnumScreen.Users, EnumSteps.Add);
                    UserDetails newUser = await UserFunctions.AddNewUserAsync();
                    await SetSelectedUserAsync(newUser);
                    await this.OnStepAsync(EnumSteps.Edit);
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.CurrentStep = EnumSteps.PersonalDetailsState;
                        this.OnStoryBoardChanged(EnumSteps.PersonalDetailsState.ToString());
                    }
                    break;
                case EnumSteps.Copy:
                    this.isCopy = true;
                    UserDetails copyUser = await UserFunctions.CopyUserAsync(this.SelectedUser);
                    copyUser.IsNewUser = true;
                    await SetSelectedUserAsync(copyUser);
                    this.RaiseActionsWhenChangeStep(EnumScreen.Users, EnumSteps.Copy);
                    await this.OnStepAsync(EnumSteps.Edit);
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.CurrentStep = EnumSteps.PersonalDetailsState;
                        this.OnStoryBoardChanged(EnumSteps.PersonalDetailsState.ToString());
                    }
                    break;
                case EnumSteps.RefreshUser:
                    user = this.AllUsers.FirstOrDefault(item => item.UserEntityId == this.SelectedUser.UserEntityId);

                    if (user == null)
                    {
                        user = this.SelectedUser;
                        this.AllUsers.Add(user);
                    }
                    else
                    {
                        this.AllUsers.Remove(user);
                        this.AllUsers.Add(user);
                    }
                    this.AllUsers = new ObservableCollection<UserDetails>(this.AllUsers.OrderBy(a => a.UserCredentials.LoginName));
                    this.SelectedUser = user;
                    break;
            }
            this.SetActionCommandsAsync();
            this.OnStepChanged(_CurrentEnumStep.ToString());
        }

        public void OnSortChanged(string columnName, SortingState sortState)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => { UserFunctions.SaveUserDefaultSortSettingsAsync("Users", columnName, sortState); }));
        }

        public void OnFilterChanged(string columnName, IEnumerable<object> distinctFilterValues, OperatorValueFilterDescriptorBase filter1, OperatorValueFilterDescriptorBase filter2, FilterCompositionLogicalOperator logicalOperator)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => { UserFunctions.SaveUserDefaultFilterSettingsAsync("Users", columnName, distinctFilterValues, filter1, filter2, logicalOperator); }));
        }

        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    this.UnLockAsync();
                    //this.IsCheckedOut = false;
                    //this.IsChanged = false;
                    if (this.AllUsers != null)
                    {
                        //this.AllUsers.Dispose();
                        this.AllUsers.Clear();
                        this.AllUsers = null;
                    }
                    if (this.AllGroups != null)
                    {
                        //this.AllGroups.Dispose();
                        this.AllGroups.Clear();
                        this.AllGroups = null;
                    }
                    if (this.SelectedUser != null)
                    {
                        this.SelectedUser.Dispose();
                        this.SelectedUser = null;
                    }
                    if (this.Edit != null)
                    {
                        this.Edit.Dispose();
                        this.Edit = null;
                    }
                    if (this.States != null)
                    {
                        //this.States.Dispose();
                        this.States.Clear();
                        this.States = null;
                    }

                    base.Dispose();
                }));
        }

        internal async Task PopulatePermissionsForUserAsync()
        {
            List<TreeComponent> componentsWithNoForms = new List<TreeComponent>();


            EffectiveOptionPermission.AllGroupsOptionPermissions = new List<OptionPermission>();
            foreach (var group in SelectedUser.LXMUserGroups)
            {
                EffectiveOptionPermission.AllGroupsOptionPermissions.AddRange(await PermissionFunctions.GetUserEntityOptionPermissionsAsync(group.UserEntityId));
            }

            EffectiveOptionPermission.AllUserOptionPermissions = await PermissionFunctions.GetUserEntityOptionPermissionsAsync(this.SelectedUser.UserEntityId);
 
            IEnumerable<Model.Component> flattennedComponents = await PermissionFunctions.InitialiseFlattennedComponentsAndPermissionsAsync(this.SelectedUser.UserEntityId, componentsWithNoForms, !this.SelectedUser.IsNewUser);

            this.SelectedUser.Components = new ObservableModelCollection<Model.Component>(PermissionFunctions.MakeComponentsInHierarchy(flattennedComponents, componentsWithNoForms));

            if (this.SelectedUser.SelectedComponent != null)
            {
                this.SelectedUser.SelectedComponent = (TreeComponent)flattennedComponents.Where(component => component.ID == this.SelectedUser.SelectedComponent.ID).FirstOrDefault();
            }

            SetSystemOptionPermissionChanged(this.SelectedUser.Components.FirstOrDefault() as TreeComponent);
            bool isAllTrue = CheckAllSystemOptionPermission(this.SelectedUser.Components.FirstOrDefault() as TreeComponent, true);
            bool isAllFalse = CheckAllSystemOptionPermission(this.SelectedUser.Components.FirstOrDefault() as TreeComponent, false);
            if (isAllTrue || isAllFalse)
            {
                this.SelectedUser.IsAllSystemOption = true;
                this.SelectedUser.IsCheckedSystemOptionPermission = isAllTrue;
            }
            
            if (!this.SelectedUser.IsAllSystemOption)
            {
                this.SelectedUser.IsSelectSystemOption = true;
            }
        }

        protected override async void SetActionCommandsAsync()
        {
            if (this.CanEdit)
            {
                if (this._CurrentEnumStep == EnumSteps.Start)
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = EnumSteps.Add.ToString(), Command = new Add() },
                    };
                }
                else if (this._CurrentEnumStep == EnumSteps.SelectUser)
                {
                    if (this.SelectedUser == null || this.SelectedUser.IsNewUser)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = EnumSteps.Add.ToString(), Command = new Add() },
                        };
                    }
                    else
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = EnumSteps.Add.ToString(), Command = new Add() },
                            new ActionCommand { Parameter = EnumSteps.Edit.ToString(), Command = new Edit() },
                            new ActionCommand { Parameter = EnumSteps.Copy.ToString(), Command = new Copy() },
                        };
                    }
                }
                if (this.Edit.IsCheckedOut)
                {
                    if (CurrentStep != EnumSteps.PermissionsState)
                    {
                        ActionCommand cmd = this.Edit.ActionCommands.FirstOrDefault(d => d.Parameter == EditUserViewModel.EnumSteps.ResetPermissions.ToString());
                        this.Edit.ActionCommands.Remove(cmd);
                    }
                    else
                    {
                        ActionCommand cmd = this.Edit.ActionCommands.FirstOrDefault(d => d.Parameter == EditUserViewModel.EnumSteps.ResetPermissions.ToString());
                        ActionCommand cmdError = this.Edit.ActionCommands.FirstOrDefault(d => d.Parameter == EditUserViewModel.EnumSteps.Error.ToString());
                        ActionCommand cmd1 = new ActionCommand { Parameter = EditUserViewModel.EnumSteps.ResetPermissions.ToString(), Command = new ResetPermissions() };
                        if (cmd == null)
                        {
                            int position = this.Edit.ActionCommands.IndexOf(cmdError);
                            if (position != -1)
                            {
                                this.Edit.ActionCommands.Insert(position, cmd1);
                            }
                            else
                            {
                                this.Edit.ActionCommands.Add(cmd1);
                            }
                        }
                    }
                }
            }
        }
        public void CloseCommandsEdit()
        {
            this.ActiveViewModel = this;
            if (this.ActionCommands != null)
            {
                this.ActionCommands.Clear();
            }
            if (this.CanEdit)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = EnumSteps.Add.ToString(), Command = new Add() },
                    };
            }
        }

        public async Task<bool> CheckIfUnSavedChanges()
        {
            bool canProceed = true;
            if (this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Users";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }
            return canProceed;
        }

        protected override async Task UnLockAsync()
        {
            if (this.SelectedUser != null && this.SelectedUser.UserCredentials != null)
            {
                await base.UnLockAsync("LXMUser", this.SelectedUser.UserCredentials.UserId.ToString(), InstanceGUID);
            }
        }

        protected override async Task<bool> LockAsync()
        {
            if (this.SelectedUser != null && this.SelectedUser.UserCredentials != null)
            {
                return await base.LockAsync("LXMUser", this.SelectedUser.UserCredentials.UserId.ToString(), InstanceGUID);
            }
            return true;
        }

        private async Task PopulateStatesAsync()
        {
            this.States = new ObservableCollection<SystemParam>(await SystemParamsFunctions.GetAllStatesAsync());
        }

        private async Task PopulateCredentialsForUserAsync()
        {
            this.SelectedUser.UserCredentials = new UserCredentials();
            this.SelectedUser.UserCredentials = await UserFunctions.GetUserCredentialsAsync(this.SelectedUser);

            if (this.SelectedUser.UserCredentials.Password == Cryptography.Encrypt(string.Format("{0}{1}", this.SelectedUser.UserCredentials.LoginName.ToLower(), Authentication.WinAuth)))
            {
                // raise loading data
                this.SelectedUser.UserCredentials.IsLoadData = true;
                this.SelectedUser.UserCredentials.IsWindowsAuthentication = true;
            }

            if (!this.SelectedUser.IsNewUser)
            {
                this.SelectedUser.UserCredentials.IsLoadPassword = true;
                this.SelectedUser.UserCredentials.NewPassword = this.SelectedUser.UserCredentials.Password;
                this.SelectedUser.UserCredentials.ReEnterPassword = this.SelectedUser.UserCredentials.Password;

                this.SelectedUser.UserCredentials.IsLoadPassword = false;
                this.SelectedUser.UserCredentials.IsChangePassword = false; 
            }
            
        }

        private async Task PopulateGroupsForUserAsync()
        {
            this.SelectedUser.LXMUserGroups = new ObservableModelCollection<LXMGroup>(await UserFunctions.GetUserGroupsAsync(this.SelectedUser.UserEntityId, this.AllGroups));
        }

        private async Task PopulateAllGroupsAsync()
        {
            this.AllGroups = new ObservableCollection<LXMGroup>(await GroupFunctions.GetGroupsAsync());
            EffectiveOptionPermission.AllGroupSources = new Dictionary<int,string>();
            foreach(LXMGroup group in AllGroups)
            {
                EffectiveOptionPermission.AllGroupSources[group.UserEntityId] = group.GroupName;
            }
        }

        private async Task<UserDetails> PopulateUsersForViewAsync(LXMUserDetail item, List<LXMUser> logins)
        {
            LXMUser user;
            UserDetails current = new UserDetails { UserEntityId = item.UserEntityId };

            user = logins.Where(login => login.UserEntityId == item.UserEntityId).FirstOrDefault();

            if (user != null)
            {
                current.UserCredentials = new UserCredentials
                {
                    AccountExpireFlag = user.AccountExpireFlag,
                    AccountExpiryDate = user.AccountExpiryDate,
                    ChgPassOnLogon = user.ChgPassOnLogon,
                    DateCreated = user.DateCreated,
                    EnabledDate = user.EnabledDate,
                    EntityId = user.EntityId,
                    GraceLoginsUsed = user.GraceLoginsUsed,
                    LastDateModified = user.LastDateModified,
                    LastUserId = user.LastUserId,
                    LoginName = user.LoginName,
                    LoginViolation = user.LoginViolation,
                    Password = user.Password,
                    PasswordExpiryDate = user.PasswordExpiryDate,
                    PasswordNeverExpires = user.PasswordNeverExpires,
                    UserCannotChangePass = user.UserCannotChangePass,
                    UserEntityId = user.UserEntityId,
                    UserId = user.UserId,
                    Visible = user.Visible
                };
            }
            else
            {
                current.UserCredentials = new UserCredentials();
            }

            current.LXMUserDetails = item;
            current.LXMUserGroups = new ObservableModelCollection<LXMGroup>(await UserFunctions.GetUserGroupsAsync(item.UserEntityId, this.AllGroups));
            current.UpdateLXMUserGroupsString();

            return current;
        }
        private async Task PopulateAllUsersForViewAsync()
        {
            List<Task<UserDetails>> tasks;
            List<LXMUserDetail> users;
            List<LXMUser> logins;
            Task<List<LXMUserDetail>> usersTask = UserFunctions.GetAllUserDetailsAsync();
            Task<List<LXMUser>> loginsTask = UserFunctions.GetAllUserCredentialsAsync();

            this.AllUsers = new ObservableCollection<UserDetails>();
            await Task.WhenAll(usersTask, loginsTask);
            users = usersTask.Result;
            logins = loginsTask.Result;

            tasks = new List<Task<UserDetails>>();
            for (int i = 0; i < users.Count; i++)
            {
                tasks.Add(this.PopulateUsersForViewAsync(users[i], logins));
            }
            await Task.WhenAll(tasks);

            for (int i = 0; i < tasks.Count; i++)
            {
                this.AllUsers.Add(tasks[i].Result);
            }
            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
            //{
            //    this.AllUsers.Add(current);
            //});
        }

        private void SetSelectedComponent(TreeComponent component, int selectedComponentID)
        {
            if (component == null)
            {
                return;
            }

            if (component.ID == selectedComponentID)
            {
                this.SelectedUser.SelectedComponent = component;
                return;
            }

            foreach (TreeComponent childComponent in component.Components)
            {
                this.SetSelectedComponent(childComponent, selectedComponentID);
            }
        }

        private void UsersViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ActiveViewModel != null)
            {
                if (this.ActiveViewModel.IsCheckedOut && (e.PropertyName.EndsWith("PermissionType") || e.PropertyName.IndexOf("IsCheckedSystemOptionPermission") != -1 || e.PropertyName.IndexOf("IsAdd") != -1 || e.PropertyName.IndexOf("IsDelete") != -1 || e.PropertyName.IndexOf(".PermissionOption") != -1 || e.PropertyName.IndexOf("UserCredentials.") != -1 || e.PropertyName.IndexOf("LXMUserDetails.") != -1))
                {
                    this.IsChanged = true;
                }
            }
        }

        #region Private Methods for System Option Permissiion
        private void SelectedUser_SystemOptionPermissionChanged()
        {
            this._IsSetSystemOptionPermission = true;
            SetSystemOptionPermissionValues(this.SelectedUser.Components.FirstOrDefault() as TreeComponent, this.SelectedUser.IsCheckedSystemOptionPermission);
            this._IsSetSystemOptionPermission = false;
        }
        private void SetSystemOptionPermissionChanged(TreeComponent Component)
        {
            if (Component != null)
            {
                if (SystemOptionPermissionID.Count == 0 || SystemOptionPermissionID.Contains(Component.ID))
                {
                    foreach (var op in Component.OptionPermissions)
                    {
                        op.OptionPermissionChanged += op_OptionPermissionChanged;
                    }
                }
                foreach (TreeComponent cp in Component.Components)
                {
                    SetSystemOptionPermissionChanged(cp);
                }
            }
        }
        private void op_OptionPermissionChanged(EffectiveOptionPermission optionPermission)
        {
            if (this._IsSetSystemOptionPermission)
            {
                return;
            }

            if (this.ActiveViewModel != null)
            {
                if (this.ActiveViewModel.IsCheckedOut)
                {
                    this.IsChanged = true;
                }
            }
            bool isAllTrue = CheckAllSystemOptionPermission(this.SelectedUser.Components.FirstOrDefault() as TreeComponent, true);
            bool isAllFalse = CheckAllSystemOptionPermission(this.SelectedUser.Components.FirstOrDefault() as TreeComponent, false);
            if (isAllTrue || isAllFalse)
            {
                this.SelectedUser.IsAllSystemOption = true;
                this.SelectedUser.IsCheckedSystemOptionPermission = isAllTrue;
            }
            else
            {
                this.SelectedUser.IsSelectSystemOption = true;
            }
        }
        private void SetSystemOptionPermissionValues(TreeComponent Component, bool Value)
        {
            if (Component != null)
            {
                if (SystemOptionPermissionID.Count == 0 || SystemOptionPermissionID.Contains(Component.ID))
                {
                    foreach (var op in Component.OptionPermissions)
                    {
                        if (op.Type == OptionType.Flag)
                        {
                            op.Flag = Value;
                        }
                    }
                }
                foreach (TreeComponent cp in Component.Components)
                {
                    SetSystemOptionPermissionValues(cp, Value);
                }
            }
        }
        private bool CheckAllSystemOptionPermission(TreeComponent Component, bool Value)
        {
            bool isAll = true;
            if (SystemOptionPermissionID.Count == 0 || SystemOptionPermissionID.Contains(Component.ID))
            {
                foreach (var op in Component.OptionPermissions)
                {
                    if (op.Type == OptionType.Flag)
                    {
                        if (!op.UserPermissionFlag.HasValue || op.UserPermissionFlag.Value != Value)
                        {
                            isAll = false;
                            break;
                        }
                    }
                }

            }

            if (isAll)
            {
                foreach (TreeComponent cp in Component.Components)
                {
                    isAll = CheckAllSystemOptionPermission(cp, Value);
                    if (!isAll)
                    {
                        break;
                    }
                }
            }

            return isAll;
        }
        #endregion
    }
}
