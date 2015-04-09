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
using Insyston.Operations.WPF.ViewModel.Common.Commands;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.WPF.ViewModel.Common.Model;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Insyston.Operations.WPF.ViewModel.Security
{
    public delegate void StoryBoardChanged(string storyBoard);

    public class ViewUsersViewModel : ViewModelUseCaseBase
    {
        public delegate void OnLoadUserContent();

        public OnLoadUserContent _OnLoadUserContent;
        internal bool isCopy;
        private EnumSteps _CurrentEnumStep;
        private ObservableCollection<LXMGroup> _AllGroups;
        private ObservableCollection<UserDetails> _AllUsers;
        private ObservableCollection<SystemParam> _States;

        /// <summary>
        /// The _ selected user.
        /// </summary>
        private UserDetails _SelectedUser;
        private bool _IsUserSelected;
        private bool _IsSetSystemOptionPermission;
        private List<int> SystemOptionPermissionID = new List<int>() ;//{ 21, 23, 27 };
        public ViewUsersViewModel()
        {
            this.Edit = new EditUserViewModel(this);
            this.PropertyChanged += this.ViewUsersViewModel_PropertyChanged;
        }

        public event StoryBoardChanged OnStoryBoardChanged;

        // Action select user
        public Action<object> OnSelectUser;

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
            RefreshUser
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
                this.ActiveViewModel.ConfirmationWindow.Raise(new Common.ConfirmationWindowViewModel(null) { Content = "Are you sure to proceed without saving?", Title = "Confirm Save - Group" },
                    (callBack) =>
                    {
                        if (callBack.Confirmed == false)
                        {
                            canProceed = false;
                        }
                    });
            }

            if (canProceed)
            {
                this.IsChanged = false;
                //if (this.SelectedUser != null && this.SelectedUser.UserCredentials != null)
                //{
                await UnLockAsync();
                //}

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
        public override async Task OnStepAsync(object stepName)
        {
            UserDetails user;

            EnumSteps previousStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());

            this._CurrentEnumStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (this._CurrentEnumStep)
            {
                case EnumSteps.Start:
                    await Task.WhenAll(this.PopulateStatesAsync(), this.PopulateAllGroupsAsync());
                    await this.PopulateAllUsersForViewAsync();
                    //this._OnLoadUserContent();
                    break;
                case EnumSteps.SelectUser:
                    GetSelectUser(EnumSteps.SelectUser);
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
                    break;
                case EnumSteps.PersonalDetailsState:
                case EnumSteps.CredentialsState:
                case EnumSteps.GroupsState:
                case EnumSteps.PermissionsState:
                case EnumSteps.SummaryState:
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._CurrentEnumStep.ToString());
                    }
                    break;
                case EnumSteps.Edit:
                    this.Edit.SelectedUser = this.SelectedUser;
                    this.Edit.NotAMemberOfGroups = new ObservableCollection<LXMGroup>(this.AllGroups.Except(this.SelectedUser.LXMUserGroups));
                    await this.Edit.OnStepAsync(EditUserViewModel.EnumSteps.Start);
                    break;
                case EnumSteps.Add:
                    UserDetails newUser = await UserFunctions.AddNewUserAsync();
                    await SetSelectedUserAsync(newUser);
                    await this.OnStepAsync(EnumSteps.Edit);
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(EnumSteps.PersonalDetailsState.ToString());
                    }
                    break;
                case EnumSteps.Copy:
                    this.isCopy = true;
                    UserDetails copyUser = await UserFunctions.CopyUserAsync(this.SelectedUser);
                    copyUser.IsNewUser = true;
                    await SetSelectedUserAsync(copyUser);
                    await this.OnStepAsync(EnumSteps.Edit);
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(EnumSteps.PersonalDetailsState.ToString());
                    }
                    break;
                case EnumSteps.RefreshUser:
                    user = this.AllUsers.Where(item => item.UserEntityId == this.SelectedUser.UserEntityId).FirstOrDefault();

                    if (user == null)
                    {
                        user = this.SelectedUser;
                        this.AllUsers.Add(user);
                    }
                    break;
            }
            this.SetActionCommandsAsync();
            this.OnStepChanged(_CurrentEnumStep.ToString());
        }

        /// <summary>
        /// Raise action select user
        /// </summary>
        /// <param name="param">
        /// The _param.
        /// </param>
        public void GetSelectUser(object param)
        {
            if (OnSelectUser != null)
            {
                OnSelectUser(param);
            }

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
                if (this.States != null)
                {
                    //this.States.Dispose();
                    this.States.Clear();
                    this.States = null;
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
        protected override async Task UnLockAsync()
        {
            if (this.SelectedUser != null && this.SelectedUser.UserCredentials != null)
            {
                await base.UnLockAsync("LXMUser", this.SelectedUser.UserCredentials.UserId.ToString());
            }
        }

        protected override async Task<bool> LockAsync()
        {
            if (this.SelectedUser != null && this.SelectedUser.UserCredentials != null)
            {
                return await base.LockAsync("LXMUser", this.SelectedUser.UserCredentials.UserId.ToString());
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

            if (this.SelectedUser.UserCredentials.Password == Cryptography.Encrypt(string.Format("{0}{1}", this.SelectedUser.UserCredentials.LoginName, Authentication.WinAuth)))
            {
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

        private void ViewUsersViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.EndsWith("PermissionType") || e.PropertyName.IndexOf(".PermissionOption") != -1 || e.PropertyName.IndexOf("UserCredentials.") != -1 || e.PropertyName.IndexOf("LXMUserDetails.") != -1)
            {
                this.IsChanged = true;
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
