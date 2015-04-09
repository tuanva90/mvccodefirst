// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MembershipViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The membership view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Security
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Insyston.Operations.Business.Common.Model;
    using Insyston.Operations.Business.Security;
    using Insyston.Operations.Business.Security.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The membership view model.
    /// </summary>
    public class MembershipViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MembershipViewModel"/> class.
        /// </summary>
        public MembershipViewModel()
        {
            this.InstanceGUID = Guid.NewGuid();
            this.Edit = new EditMembershipViewModel(this);
            this.PropertyChanged += this.MembershipViewModel_PropertyChanged;
            _availableUsersList = new ObservableCollection<Membership>();
        }
        #region Properties

        /// <summary>
        /// The _available collector list.
        /// </summary>
        private ObservableCollection<Membership> _availableUsersList;

        /// <summary>
        /// The _groups.
        /// </summary>
        private ObservableCollection<GroupDetails> _groups;

        /// <summary>
        /// The _all users.
        /// </summary>
        private ObservableCollection<LXMUserDetail> _allUsers;

        /// <summary>
        /// The _managers.
        /// </summary>
        private List<DropdownList> _managers;

        /// <summary>
        /// The _current enum step.
        /// </summary>
        private EnumSteps _currentEnumStep;

        /// <summary>
        /// Gets or sets the available users list.
        /// </summary>
        public ObservableCollection<Membership> AvailableUsersList
        {
            get
            {
                return this._availableUsersList;
            }
            set
            {
                this.SetField(ref _availableUsersList, value, () => AvailableUsersList);
            }
        }

        /// <summary>
        /// Gets or sets the managers.
        /// </summary>
        public List<DropdownList> Managers
        {
            get
            {
                return this._managers;
            }
            set
            {
                this.SetField(ref _managers, value, () => Managers);
            }
        }

        /// <summary>
        /// Gets or sets the all users.
        /// </summary>
        public ObservableCollection<LXMUserDetail> AllUsers
        {
            get
            {
                return this._allUsers;
            }
            set
            {
                this.SetField(ref _allUsers, value, () => AllUsers);
            }
        }

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        public ObservableCollection<GroupDetails> Groups
        {
            get
            {
                return this._groups;
            }
            set
            {
                this.SetField(ref _groups, value, () => Groups);
            }
        }

        /// <summary>
        /// The check out changed.
        /// </summary>
        /// <param name="isCheckOut">
        /// The is check out.
        /// </param>
        public delegate void CheckOutChanged(bool isCheckOut);

        /// <summary>
        /// The on check out changed.
        /// </summary>
        public event CheckOutChanged OnCheckOutChanged;

        /// <summary>
        /// The save member ship.
        /// </summary>
        public Func<List<GroupDetails>, List<LXMUserDetail>, bool, Task> SaveMemberShip;

        /// <summary>
        /// The collectors queue changed.
        /// </summary>
        /// <param name="collectionQueues">
        /// The collection queues.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="isSaveData">
        /// The is save data.
        /// </param>
        public delegate void UsersInGroupChanged(List<GroupDetails> collectionQueues, List<LXMUserDetail> user, bool isSaveData = false);

        /// <summary>
        /// The on collectors changed.
        /// </summary>
        public event UsersInGroupChanged OnUsersInGroupChanged;

        /// <summary>
        /// The enum steps.
        /// </summary>
        public enum EnumSteps
        {
            Start,
            Edit,
            Refresh,
            Reset
        }

        /// <summary>
        /// Gets the edit.
        /// </summary>
        public EditMembershipViewModel Edit { get; private set; }
        #endregion

        #region Public Method

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
        /// The check content editing.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<bool> CheckContentEditing()
        {
            if (this.OnUsersInGroupChanged != null)
            {
                this.OnUsersInGroupChanged(null, null);
            }

            if (this.ActiveViewModel.IsCheckedOut && this.IsChanged)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
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
            this._currentEnumStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (this._currentEnumStep)
            {
                case EnumSteps.Start:
                    this.SetBusyAction("Please Wait Loading ...");
                    AvailableUsersList = new ObservableCollection<Membership>();
                    await Task.WhenAll(this.PopulateAllUsersAsync(), this.PopulateAllGroupsAsync());
                    await Instance();
                    if (this.OnUsersInGroupChanged != null)
                    {
                        this.OnUsersInGroupChanged(this.Groups.ToList(), this.AllUsers.ToList());
                    }
                    this.ResetBusyAction();
                    break;
                case EnumSteps.Edit:
                    //if (await this.LockAsync() == false)
                    //{
                    //    return;
                    //}
                    
                    if (this.OnCheckOutChanged != null)
                    {
                        this.OnCheckOutChanged(true);
                    }
                    this.Edit.InstanceGUID = this.InstanceGUID;
                    await this.Edit.OnStepAsync(EditMembershipViewModel.EnumSteps.Start);
                    break;
                case EnumSteps.Reset:
                    this.ActiveViewModel = this;
                    await Instance();
                    if (this.OnUsersInGroupChanged != null)
                    {
                        this.OnUsersInGroupChanged(this.Groups.ToList(), this.AllUsers.ToList());
                    }
                    break;
                case EnumSteps.Refresh:
                    this.ActiveViewModel = this;
                    await Task.WhenAll(this.PopulateAllUsersAsync(), this.PopulateAllGroupsAsync());
                    if (this.OnUsersInGroupChanged != null)
                    {
                        this.OnUsersInGroupChanged(this.Groups.ToList(), this.AllUsers.ToList());
                    }
                    if (this.OnCheckOutChanged != null)
                    {
                        this.OnCheckOutChanged(this.Edit.IsCheckedOut);
                    }
                    break;
            }
            SetActionCommandsAsync();
            this.OnStepChanged(_currentEnumStep.ToString());
        }

        /// <summary>
        /// The set action commands async.
        /// </summary>
        protected override async void SetActionCommandsAsync()
        {
            if (this.CanEdit)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = EnumSteps.Edit.ToString(), Command = new Edit() },
                                };
            }

        }

        /// <summary>
        /// The save all groups.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SaveAllGroups()
        {
            if (this.SaveMemberShip != null)
                            {
                                await this.SaveMemberShip(this.Groups.ToList(), this.AllUsers.ToList(), true);
                            }
        }

        /// <summary>
        /// The get availible collector.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<List<Membership>> GetAvailibleUsers()
        {
            var availibleUsers = new ObservableCollection<LXMUserDetail>(await UserFunctions.GetAllUserDetailsAsync());

            return availibleUsers.Select(item => new Membership
            {
                UserId = item.UserEntityId,
                UserName = item.Firstname + " " + item.Lastname
            }).ToList();
        }

        public async Task<bool> CheckIfUnSavedChanges()
        {
            if (this.OnUsersInGroupChanged != null)
            {
                this.OnUsersInGroupChanged(null, null);
            }

            bool canProceed = true;
            if (this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Membership";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }
            return canProceed;
        }
        #endregion

        #region Private Method

        /// <summary>
        /// The instance.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task Instance()
        {
            AvailableUsersList = new ObservableCollection<Membership>();
            foreach (Membership product in await GetAvailibleUsers())
            {
                AvailableUsersList.Add(product);
            }
        }

        /// <summary>
        /// The populate all groups async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task PopulateAllGroupsAsync()
        {
            this.Groups = new ObservableCollection<GroupDetails>(await GroupFunctions.GetGroupsSummaryAsync());
        }

        /// <summary>
        /// The populate all users async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task PopulateAllUsersAsync()
        {
            this.AllUsers = new ObservableCollection<LXMUserDetail>(await UserFunctions.GetAllUserDetailsAsync());
        }
        #endregion

        public void GroupList_OnChanged(GroupDetails group, List<LXMUserDetail> users, List<Membership> usersList)
        {
            using (Entities model = new Entities())
            {
                List<int> groupUsers = model.sp_GetGroupUsers(group.UserEntityId).Select(g => g.Value).ToList();
                var addedUsers =
                    usersList.Where(item => !groupUsers.Contains(item.UserId)).Select(x => x.UserId).ToList();
                var currentIds = usersList.Select(item => item.UserId).ToList();
                var usersInGroup = GroupFunctions.GetGroupUsers(group.UserEntityId, users);
                var removedUsers = usersInGroup.Where(i => !currentIds.Contains(i.UserEntityId)).Select(x => x.UserEntityId).ToList();

                if (this.IsCheckedOut && (addedUsers.Count != 0 || removedUsers.Count != 0))
                {
                    this.IsChanged = true;
                }
            }
        }

        private void MembershipViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
                //this.IsChanged = true;
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

        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                new Action(
                    () =>
                        {
                            this.UnLockAsync();

                            if (this._availableUsersList != null)
                            {
                                this._availableUsersList.Clear();
                                this._availableUsersList = null;
                            }
                            if (this._groups != null)
                            {
                                this._groups.Clear();
                                this._groups = null;
                            }
                            if (this._allUsers != null)
                            {
                                this._allUsers.Clear();
                                this._allUsers = null;
                            }
                            if (this._managers != null)
                            {
                                this._managers.Clear();
                                this._managers = null;
                            }
                            if (this.Edit != null)
                            {
                                this.Edit.Dispose();
                                this.Edit = null;
                            }

                            base.Dispose();
                        }));
        }
    }
}
