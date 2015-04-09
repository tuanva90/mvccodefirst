// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MembershipView.xaml.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for MembershipView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.Views.Security
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Security;
    using Insyston.Operations.Business.Security.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Security;
    using Insyston.Operations.WPF.Views.Security.Controls;

    /// <summary>
    /// Interaction logic for MembershipView.xaml
    /// </summary>
    public partial class MembershipView
    {
        /// <summary>
        /// The _group items.
        /// </summary>
        private readonly List<QueueItemCollectors> _groupItems = new List<QueueItemCollectors>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MembershipView"/> class.
        /// </summary>
        public MembershipView()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// The on loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="routedEventArgs">
        /// The routed event args.
        /// </param>
        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var model = this.DataContext as MembershipViewModel;
            if (model != null)
            {
                model.OnUsersInGroupChanged -= this.OnUsersInGroupChanged;
                model.OnUsersInGroupChanged += this.OnUsersInGroupChanged;

                model.OnCheckOutChanged -= this.OnCheckOutChanged;
                model.OnCheckOutChanged += this.OnCheckOutChanged;

                model.SaveMemberShip = this.AddGroupsList;
            }
            await((MembershipViewModel)this.DataContext).OnStepAsync(MembershipViewModel.EnumSteps.Start);
        }

        /// <summary>
        /// The on check out changed.
        /// </summary>
        /// <param name="isCheckOut">
        /// The is check out.
        /// </param>
        private void OnCheckOutChanged(bool isCheckOut)
        {
            foreach (var item in this._groupItems)
            {
                if (item.Name == "ShowAllEnabled")
                {
                    continue;
                }

                item.IsEnable = isCheckOut;
            }
        }

        /// <summary>
        /// The on users in group changed.
        /// </summary>
        /// <param name="groups">
        /// The groups.
        /// </param>
        /// <param name="users">
        /// The users.
        /// </param>
        /// <param name="isSaveData">
        /// The is save data.
        /// </param>
        private async void OnUsersInGroupChanged(List<GroupDetails> groups, List<LXMUserDetail> users, bool isSaveData = false)
        {
            if (groups == null && users == null)
            {
                this.OnGroupListChanged();
            }
            else
            {
                await this.AddGroupsList(groups, users, isSaveData);
            }
        }

        private void OnGroupListChanged()
        {
            var model = this.DataContext as MembershipViewModel;
            if (model != null)
            {
                for (int i = 1; i <= this._groupItems.Count; i++)
                {
                    model.GroupList_OnChanged(model.Groups.ToList()[i-1], model.AllUsers.ToList(), new List<Membership>(this._groupItems[i - 1].QueueListCollection));
                }
            }
        }

        /// <summary>
        /// The add groups list.
        /// </summary>
        /// <param name="groups">
        /// The groups.
        /// </param>
        /// <param name="users">
        /// The users.
        /// </param>
        /// <param name="isSaveData">
        /// The is save data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task AddGroupsList(List<GroupDetails> groups, List<LXMUserDetail> users, bool isSaveData = false)
        {
            MembershipScrollViewer.ScrollToVerticalOffset(0);
            MembershipScrollViewer.ScrollToHorizontalOffset(0);
            int yPos = 0;
            var xPos = 0;
            if (isSaveData)
            {
                for (int i = 1; i <= this._groupItems.Count; i++)
                {
                    await GroupFunctions.SaveGroups(groups[i - 1], users, new List<Membership>(this._groupItems[i - 1].QueueListCollection));
                }
                return;
            }

            this._groupItems.Clear();
            for (var i = 1; i <= groups.Count; i++)
            {
                var groupItem = new QueueItemCollectors
                {
                    Width = 250,
                    Height = 270,
                    HeaderId = groups[i - 1].UserEntityId,
                    HeaderQueue = groups[i - 1].LXMGroup.GroupName,
                    QueueListCollection = new ObservableCollection<Membership>(),
                    IsEnable = false
                };

                var usersInGroup = GroupFunctions.GetGroupUsers(groups[i - 1].UserEntityId, users);
                foreach (var user in usersInGroup.Select(item => new Membership
                {
                    UserId = item.UserEntityId,
                    UserName = item.Firstname + " " + item.Lastname
                }))
                {
                    groupItem.QueueListCollection.Add(user);
                }

                groupItem.HorizontalAlignment = HorizontalAlignment.Left;
                groupItem.VerticalAlignment = VerticalAlignment.Top;
                groupItem.Margin = new Thickness(xPos, yPos, 0, 0);

                QueueList.Children.Add(groupItem);

                if (i % 3 == 0)
                {
                    yPos += 280;
                }

                if (i % 3 == 0)
                {
                    xPos = 0;
                }
                else
                {
                    xPos += 270;
                }
                this._groupItems.Add(groupItem);
            }
        }
    }
}
