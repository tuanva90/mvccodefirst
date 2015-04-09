using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Security;
using Insyston.Operations.Business.Security.Model;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Common.Commands;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Insyston.Operations.WPF.ViewModels.Security
{
    using System.Windows;
    using System.Windows.Media;

    using Insyston.Operations.WPF.ViewModels.Common;

    public class GroupsViewModel : ViewModelUseCaseBase
    {
        internal bool isChanged, isCopy;

        private EnumSteps _CurrentEnumStep;
        private ObservableCollection<GroupDetails> _Groups;
        private GroupDetails _SelectedGroup;
        private ObservableCollection<LXMUserDetail> _AllUsers;
        private List<DropdownList> _Managers;
        private bool _IsGroupSelected;
        private bool _IsSetSystemOptionPermission;
        private List<int> SystemOptionPermissionID = new List<int>() ;//{ 21, 23, 27 };
        private const string LoadingText = "Please Wait Loading ...";
        public GroupsViewModel()
        {
            this.InstanceGUID = Guid.NewGuid();
            this.Edit = new EditGroupViewModel(this);
            this.IsGroupSelected = false;
            this.PropertyChanged += this.ViewGroupsViewModel_PropertyChanged;
        }

        public event StoryBoardChanged OnStoryBoardChanged;

        public enum EnumSteps
        {
            Start,
            SelectGroup,
            DetailsState,
            UsersState,
            PermissionsState,
            Edit,
            Add,
            Copy,
            RefreshGroup,
            CurrentGroup,
            Cancel
        }

        public EditGroupViewModel Edit { get; private set; }

        public ObservableCollection<GroupDetails> Groups
        {
            get
            {
                return this._Groups;
            }
            set
            {
                this.SetField(ref _Groups, value, () => Groups);
            }
        }

        public GroupDetails SelectedGroup
        {
            get
            {
                return this._SelectedGroup;
            }
            set
            {
                SetSelectedGroupAsync(value);
                //don't put any logic here, logic should be defined in SetSelectedGroup() method
            }
        }
        private async Task SetSelectedGroupAsync(GroupDetails value)
        {
            bool canProceed = true;

            if (this.ActiveViewModel != null && this.ActiveViewModel.IsCheckedOut && this.isChanged)
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
                confirmViewModel.Title = "Confirm Save - Group";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
                else
                {
                    this.isChanged = false;
                }
            }
            if (canProceed)
            {
                if (this.Edit.IsCheckedOut)
                {
                    await this.Edit.OnStepAsync(EnumSteps.Cancel);
                }

                // Raise event to change style for hyperlink when select another record.
                this.ValidateNotError();
                this.isChanged = false;

                // Just do UnLockAsync if not in mode Add.
                if (value != null && !value.IsNewGroup)
                {
                    await UnLockAsync();
                }

                this.SetField(ref _SelectedGroup, value, () => SelectedGroup);

                if (this._SelectedGroup != null)
                {
                    this._SelectedGroup.ComponentPermissionChanged += this.Edit.SelectedGroup_ComponentPermissionChanged;
                    this._SelectedGroup.ComponentPermissionChanged += () => { this.OnPropertyChanged(() => SelectedComponentPermissionOption); };
                }

                if (value != null)
                {
                    await this.OnStepAsync(EnumSteps.SelectGroup);
                }
            }
        }
        public ObservableCollection<LXMUserDetail> AllUsers
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

        public List<DropdownList> Managers
        {
            get
            {
                return this._Managers;
            }
            set
            {
                this.SetField(ref _Managers, value, () => Managers);
            }
        }

        public bool IsGroupSelected
        {
            get
            {
                return this._IsGroupSelected;
            }
            set
            {
                if (this.SetField(ref _IsGroupSelected, value, () => IsGroupSelected))
                {
                    this.SetActionCommandsAsync();
                }
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
            if (this.ActiveViewModel.IsCheckedOut && this.isChanged)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public EnumSteps CurrentStep { get; private set; }
        public override async Task OnStepAsync(object stepName)
        {
            GroupDetails group;
            this._CurrentEnumStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (this._CurrentEnumStep)
            {
                case EnumSteps.Start:
                    this.SetBusyAction(LoadingText);
                    await Task.WhenAll(this.PopulateAllUsersAsync(), this.PopulateAllGroupsAsync());
                    this.Managers = null;
                    this.ResetBusyAction();
                    break;
                case EnumSteps.SelectGroup:
                    this.SetBusyAction(LoadingText);
                    this.Edit.SetBusyAction(LoadingText);
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                    if (this.SelectedGroup.IsNewGroup == false)
                    {
                        this.SelectedGroup.LXMGroup = await GroupFunctions.GetGroupAsync(this.SelectedGroup.UserEntityId);
                    }

                    if (this.AllUsers != null && this.Managers == null)
                    {
                        this.Managers = this.AllUsers.Select(user => new DropdownList { Description = string.Format("{0}{1}{2}", user.Lastname, string.IsNullOrEmpty(user.Lastname) == false ? ", " : "", user.Firstname), ID = user.UserEntityId }).ToList();
                    }

                    if (this.isCopy == false)
                    {
                        await this.PopulateUsersAndPermissionsForGroupAsync();
                    }
                    this.ActiveViewModel = this;
                    this.SetActionCommandsAsync();

                    if (this.IsCheckedOut)
                    {
                        await this.OnStepAsync(EnumSteps.Edit.ToString());
                    }
                    this.SelectedGroup.SystemOptionPermissionChanged -= SelectedGroup_SystemOptionPermissionChanged;
                    this.SelectedGroup.SystemOptionPermissionChanged += SelectedGroup_SystemOptionPermissionChanged;
                    this.Edit.ResetBusyAction();
                    this.ResetBusyAction();
                    if (this.SelectedGroup.IsNewGroup == false)
                    {
                        this.RaiseActionsWhenChangeStep(EnumScreen.Groups, EnumSteps.SelectGroup, this.SelectedGroup);
                    }
                    break;
                case EnumSteps.CurrentGroup:
                    this.ActiveViewModel = this;
                    this.SetActionCommandsAsync();
                    break;
                case EnumSteps.DetailsState:
                    this.CurrentStep = EnumSteps.DetailsState;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._CurrentEnumStep.ToString());
                    }
                    break;
                case EnumSteps.UsersState:
                    this.CurrentStep = EnumSteps.UsersState;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._CurrentEnumStep.ToString());
                    }
                    break;
                case EnumSteps.PermissionsState:
                    this.CurrentStep = EnumSteps.PermissionsState;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._CurrentEnumStep.ToString());
                    }
                    break;
                case EnumSteps.Edit:
                    this.RaiseActionsWhenChangeStep(EnumScreen.Groups, EnumSteps.Edit);
                    this.Edit.SelectedGroup = this.SelectedGroup;
                    this.Edit.InstanceGUID = this.InstanceGUID;
                    this.Edit.NotMembers = new ObservableCollection<LXMUserDetail>(this.AllUsers.Except(this.SelectedGroup.Users));
                    await this.Edit.OnStepAsync(EditUserViewModel.EnumSteps.Start);
                    break;
                case EnumSteps.Add:
                    this.RaiseActionsWhenChangeStep(EnumScreen.Groups, EnumSteps.Add);
                    GroupDetails newGroup = await GroupFunctions.AddNewGroupAsync();
                    await SetSelectedGroupAsync(newGroup);
                    await this.OnStepAsync(EnumSteps.Edit);
                    this.CurrentStep = EnumSteps.DetailsState;
                    this.OnStoryBoardChanged(EnumSteps.DetailsState.ToString());
                    break;
                case EnumSteps.Copy:
                    this.isCopy = true;
                    GroupDetails copyGroup = await GroupFunctions.CopyGroupAsync(this.SelectedGroup);
                    copyGroup.IsNewGroup = true;
                    await SetSelectedGroupAsync(copyGroup);
                    this.RaiseActionsWhenChangeStep(EnumScreen.Groups, EnumSteps.Copy);
                    await this.OnStepAsync(EnumSteps.Edit);
                    this.CurrentStep = EnumSteps.DetailsState;
                    this.OnStoryBoardChanged(EnumSteps.DetailsState.ToString());
                    break;
                case EnumSteps.RefreshGroup:
                    group = this.Groups.Where(item => item.UserEntityId == this.SelectedGroup.UserEntityId).FirstOrDefault();

                    if (group == null)
                    {
                        group = this.SelectedGroup;
                        this.Groups.Add(group);
                    }
                    else
                    {
                        this.Groups.Remove(group);
                        this.Groups.Add(group);
                    }
                    this.Groups = new ObservableCollection<GroupDetails>(this.Groups.OrderBy(a => a.LXMGroup.GroupName));
                    this.SelectedGroup = group;
                    group.ManagerName = this.Managers.Where(manager => manager.ID == this.SelectedGroup.LXMGroup.ManagerUserEntityId).FirstOrDefault().Description;                 
                    break;
            }
            this.SetActionCommandsAsync();
            this.OnStepChanged(_CurrentEnumStep.ToString());
        }

        public async Task<LXMUserDefaultSetting> UserDefaultSettingsAsync()
        {
            return await UserFunctions.GetUserDefaultSortFilterSettingsAsync("Groups");
        }

        public void OnSortChanged(string columnName, SortingState sortState)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => { UserFunctions.SaveUserDefaultSortSettingsAsync("Groups", columnName, sortState); }));
        }

        public void OnFilterChanged(string columnName, IEnumerable<object> distinctFilterValues, OperatorValueFilterDescriptorBase filter1, OperatorValueFilterDescriptorBase filter2, FilterCompositionLogicalOperator logicalOperator)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => { UserFunctions.SaveUserDefaultFilterSettingsAsync("Groups", columnName, distinctFilterValues, filter1, filter2, logicalOperator); }));
        }

        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                new Action(
                    () =>
                        {
                            this.isChanged = false;
                            this.UnLockAsync();
                            if (this.Groups != null)
                            {
                                //this.Groups.Dispose();
                                this.Groups.Clear();
                                this.Groups = null;
                            }
                            if (this.AllUsers != null)
                            {
                                //this.AllUsers.Dispose();
                                this.AllUsers.Clear();
                                this.AllUsers = null;
                            }
                            if (this.Managers != null)
                            {
                                this.Managers.Clear();
                                this.Managers = null;
                            }
                            if (this.SelectedGroup != null)
                            {
                                this.SelectedGroup.Dispose();
                                this.SelectedGroup = null;
                            }
                            if (this.Edit != null)
                            {
                                this.Edit.Dispose();
                                this.Edit = null;
                            }

                            if (this.OnStoryBoardChanged != null)
                            {
                                //Parallel.ForEach(this.OnStoryBoardChanged.GetInvocationList(), d => this.OnStoryBoardChanged -= d as StoryBoardChanged);
                                this.OnStoryBoardChanged.GetInvocationList()
                                    .ForEach(d => this.OnStoryBoardChanged -= d as StoryBoardChanged);
                            }

                            base.Dispose();
                        }));
        }

        internal async Task PopulateUsersAndPermissionsForGroupAsync()
        {
            List<TreeComponent> componentWithNoForms = new List<TreeComponent>();

            EffectiveOptionPermission.AllUserOptionPermissions = await PermissionFunctions.GetUserEntityOptionPermissionsAsync(this.SelectedGroup.UserEntityId);
            EffectiveOptionPermission.AllGroupsOptionPermissions = new List<OptionPermission>();

            IEnumerable<Model.Component> flattennedComponents = await PermissionFunctions.InitialiseFlattennedComponentsAndPermissionsAsync(this.SelectedGroup.UserEntityId, componentWithNoForms, !this.SelectedGroup.IsNewGroup,                 
                true);
            this.SelectedGroup.Components = new ObservableModelCollection<Model.Component>(PermissionFunctions.MakeComponentsInHierarchy(flattennedComponents, componentWithNoForms, true));
            this.SelectedGroup.Users = new ObservableModelCollection<LXMUserDetail>(GroupFunctions.GetGroupUsers(this.SelectedGroup.UserEntityId, this.AllUsers));

            if (this.SelectedGroup.SelectedComponent != null)
            {
                this.SelectedGroup.SelectedComponent = (TreeComponent)flattennedComponents.Where(component => component.ID == this.SelectedGroup.SelectedComponent.ID).FirstOrDefault();
            }

            SetSystemOptionPermissionChanged(this.SelectedGroup.Components.FirstOrDefault() as TreeComponent);

            bool isAllTrue = CheckAllSystemOptionPermission(this.SelectedGroup.Components.FirstOrDefault() as TreeComponent, true);
            bool isAllFalse = CheckAllSystemOptionPermission(this.SelectedGroup.Components.FirstOrDefault() as TreeComponent, false);
            if (isAllTrue || isAllFalse)
            {
                this.SelectedGroup.IsAllSystemOption = true;
                this.SelectedGroup.IsCheckedSystemOptionPermission = isAllTrue;
            }
            if (!this.SelectedGroup.IsAllSystemOption)
            {
                this.SelectedGroup.IsSelectSystemOption = true;
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
                else if (this._CurrentEnumStep == EnumSteps.SelectGroup)
                {
                    if (this.SelectedGroup == null || this.SelectedGroup.IsNewGroup)
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

                        ActionCommand cmd = this.Edit.ActionCommands.FirstOrDefault(d => d.Parameter == EditGroupViewModel.EnumSteps.ResetPermissions.ToString());
                        this.Edit.ActionCommands.Remove(cmd);
                    }
                    else
                    {
                        ActionCommand cmd = this.Edit.ActionCommands.FirstOrDefault(d => d.Parameter == EditGroupViewModel.EnumSteps.ResetPermissions.ToString());
                        ActionCommand cmdError = this.Edit.ActionCommands.FirstOrDefault(d => d.Parameter == EditGroupViewModel.EnumSteps.Error.ToString());
                        ActionCommand cmd1 = new ActionCommand { Parameter = EditGroupViewModel.EnumSteps.ResetPermissions.ToString(), Command = new ResetPermissions() };
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
            if (this.isChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Groups";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }
            return canProceed;
        }
        /// <summary>
        /// The function used to set value for SelectedGroup when we don't want to check any business logics.
        /// </summary>
        /// <param name="value"></param>
        public void SetSelectedGroupValue(GroupDetails value)
        {
            this.SetField(ref _SelectedGroup, value, () => SelectedGroup);
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

        private async Task PopulateAllUsersAsync()
        {
            this.AllUsers = new ObservableCollection<LXMUserDetail>(await UserFunctions.GetAllUserDetailsAsync());
        }

        private async Task PopulateAllGroupsAsync()
        {
            this.Groups = new ObservableCollection<GroupDetails>(await GroupFunctions.GetGroupsSummaryAsync());
        }

        private async void ViewGroupsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ActiveViewModel != null)
            {
                if ((this.ActiveViewModel.IsCheckedOut)
                    && (e.PropertyName.EndsWith("PermissionType") || e.PropertyName.IndexOf("IsCheckedSystemOptionPermission") != -1 || e.PropertyName.IndexOf("IsAdd") != -1 || e.PropertyName.IndexOf("IsDelete") != -1 || e.PropertyName.IndexOf(".PermissionOption") != -1 || e.PropertyName.IndexOf("LXMGroup.") != -1))
                {
                    this.isChanged = true;
                }
            }
        }

        #region Private Methods for System Option Permissiion
        private void SelectedGroup_SystemOptionPermissionChanged()
        {
            this._IsSetSystemOptionPermission = true;
            SetSystemOptionPermissionValues(this.SelectedGroup.Components.FirstOrDefault() as TreeComponent, this.SelectedGroup.IsCheckedSystemOptionPermission);
            this._IsSetSystemOptionPermission = false;
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
                    this.isChanged = true;
                }
            }
            bool isAllTrue = CheckAllSystemOptionPermission(this.SelectedGroup.Components.FirstOrDefault() as TreeComponent, true);
            bool isAllFalse = CheckAllSystemOptionPermission(this.SelectedGroup.Components.FirstOrDefault() as TreeComponent, false);
            if (isAllTrue || isAllFalse)
            {
                this.SelectedGroup.IsAllSystemOption = true;
                this.SelectedGroup.IsCheckedSystemOptionPermission = isAllTrue;
            }
            else
            {
                this.SelectedGroup.IsSelectSystemOption = true;
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
                        if (!op.Flag.HasValue || op.Flag.Value != Value)
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