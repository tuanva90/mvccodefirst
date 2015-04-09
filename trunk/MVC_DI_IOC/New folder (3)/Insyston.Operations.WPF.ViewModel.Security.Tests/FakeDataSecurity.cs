// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDataSecurity.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the FakeData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Security.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Documents;

    using Insyston.Operations.Business.Common.Model;
    using Insyston.Operations.Business.Security.Model;
    using Insyston.Operations.Model;

    /// <summary>
    /// The fake data.
    /// </summary>
    internal class FakeDataSecurity
    {
        /// <summary>
        /// The fake data security user.
        /// </summary>
        internal static class FakeDataSecurityUser
        {
            #region Fake Data LXMGroup

            /// <summary>
            /// The fake groups.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<ObservableCollection<LXMGroup>> FakeGroups()
            {
                return await Task.Run(
                    () =>
                    {
                        List<LXMGroup> groups;

                        groups = new List<LXMGroup>();
                        groups.Add(new LXMGroup { GroupId = 2, GroupName = "First", GroupDescription = "GroupFirst", UserEntityId = 10 });
                        groups.Add(new LXMGroup { GroupId = 3, GroupName = "Credit", GroupDescription = "Credit", UserEntityId = 38 });
                        groups.Add(new LXMGroup { GroupId = 4, GroupName = "Management", GroupDescription = "Management", UserEntityId = 49 });
                        groups.Add(new LXMGroup { GroupId = 5, GroupName = "Lease Administration", GroupDescription = "Lease Administration", UserEntityId = 51 });

                        return new ObservableCollection<LXMGroup>(groups);
                    });
            }

            /// <summary>
            /// The fake groups object.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<ObservableCollection<object>> FakeGroupsObject()
            {
                return await Task.Run(
                    () =>
                    {
                        List<LXMGroup> groups;

                        groups = new List<LXMGroup>();
                        groups.Add(new LXMGroup { GroupId = 2, GroupName = "First", GroupDescription = "GroupFirst", UserEntityId = 10 });
                        groups.Add(new LXMGroup { GroupId = 3, GroupName = "Credit", GroupDescription = "Credit", UserEntityId = 38 });
                        groups.Add(new LXMGroup { GroupId = 4, GroupName = "Management", GroupDescription = "Management", UserEntityId = 49 });
                        groups.Add(new LXMGroup { GroupId = 5, GroupName = "Lease Administration", GroupDescription = "Lease Administration", UserEntityId = 51 });

                        return new ObservableCollection<object>(groups);
                    });
            }

            /// <summary>
            /// The fake selected not a member of groups.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<ObservableCollection<object>> FakeSelectedNotAMemberOfGroups()
            {
                return await Task.Run(
                    () =>
                    {
                        List<LXMGroup> groups;

                        groups = new List<LXMGroup>();
                        groups.Add(new LXMGroup { GroupId = 7, GroupName = "First", GroupDescription = "GroupFirst", UserEntityId = 7 });
                        groups.Add(new LXMGroup { GroupId = 8, GroupName = "Credit", GroupDescription = "Credit", UserEntityId = 8 });

                        return new ObservableCollection<object>(groups);
                    });
            }

            /// <summary>
            /// The fake not a member of groups.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<ObservableCollection<LXMGroup>> FakeNotAMemberOfGroups()
            {
                return await Task.Run(
                    () =>
                    {
                        List<LXMGroup> groups;

                        groups = new List<LXMGroup>();
                        groups.Add(new LXMGroup { GroupId = 7, GroupName = "First", GroupDescription = "GroupFirst", UserEntityId = 7 });
                        groups.Add(new LXMGroup { GroupId = 8, GroupName = "Credit", GroupDescription = "Credit", UserEntityId = 8 });
                        groups.Add(new LXMGroup { GroupId = 9, GroupName = "Management", GroupDescription = "Management", UserEntityId = 9 });
                        groups.Add(new LXMGroup { GroupId = 10, GroupName = "Lease Administration", GroupDescription = "Lease Administration", UserEntityId = 10 });

                        return new ObservableCollection<LXMGroup>(groups);
                    });
            }

            /// <summary>
            /// The fake user groups.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableModelCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableModelCollection<LXMGroup> FakeLxmUserGroups()
            {
                List<LXMGroup> groups;

                groups = new List<LXMGroup>();
                groups.Add(new LXMGroup { GroupId = 1, GroupName = "First", GroupDescription = "GroupFirst", UserEntityId = 15 });

                return new ObservableModelCollection<LXMGroup>(groups);
            }

            /// <summary>
            /// The fake groups list.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<LXMGroup>> FakeGroupsList()
            {
                return await Task.Run(
                    () =>
                    {
                        List<LXMGroup> groups;

                        groups = new List<LXMGroup>();
                        groups.Add(
                            new LXMGroup
                                {
                                    GroupId = 2,
                                    GroupName = "Credit",
                                    GroupDescription = "Credit",
                                    UserEntityId = 38
                                });
                        groups.Add(
                            new LXMGroup
                                {
                                    GroupId = 3,
                                    GroupName = "Management",
                                    GroupDescription = "Management",
                                    UserEntityId = 49
                                });
                        groups.Add(
                            new LXMGroup
                                {
                                    GroupId = 4,
                                    GroupName = "Lease Administration",
                                    GroupDescription = "Lease Administration",
                                    UserEntityId = 51
                                });

                        return new List<LXMGroup>(groups);
                    });
            }

            /// <summary>
            /// The fake groups list numerable.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>IEnumerable</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static IEnumerable<LXMGroup> FakeGroupsListIeNumerable()
            {
                List<LXMGroup> groups;

                groups = new List<LXMGroup>();
                groups.Add(new LXMGroup { GroupId = 2, GroupName = "Credit", GroupDescription = "Credit", UserEntityId = 38 });
                groups.Add(new LXMGroup { GroupId = 3, GroupName = "Management", GroupDescription = "Management", UserEntityId = 49 });
                groups.Add(new LXMGroup { GroupId = 4, GroupName = "Lease Administration", GroupDescription = "Lease Administration", UserEntityId = 51 });

                return new List<LXMGroup>(groups);
            }

            #endregion

            #region Fake Data UserDetail

            /// <summary>
            /// The fake users.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableCollection<UserDetails> FakeUsers()
            {
                List<UserDetails> users;

                users = new List<UserDetails>();
                users.Add(new UserDetails { UserEntityId = 100, LXMUserGroupsString = "Insyston100" });
                users.Add(new UserDetails { UserEntityId = 150, LXMUserGroupsString = "Insyston150" });
                users.Add(new UserDetails { UserEntityId = 50, LXMUserGroupsString = "Insyston50" });

                return new ObservableCollection<UserDetails>(users);
            }

            /// <summary>
            /// The fake selected user.
            /// </summary>
            /// <returns>
            /// The <see cref="UserDetails"/>.
            /// </returns>
            internal static UserDetails FakeSelectedUser()
            {
                return new UserDetails { UserEntityId = 1, IsNewUser = false, LXMUserDetails = new LXMUserDetail { Firstname = "Test", Lastname = "User", AddressLine1 = "Test Addr.", PostalSuburbCity = "Sydney" } };
            }

            /// <summary>
            /// The fake selected user with user credential.
            /// </summary>
            /// <returns>
            /// The <see cref="UserDetails"/>.
            /// </returns>
            internal static UserDetails FakeSelectedUserWithUserCredentialInGroup()
            {
                return new UserDetails { UserEntityId = 1, IsNewUser = false, LXMUserDetails = new LXMUserDetail { Firstname = "Test", Lastname = "User", AddressLine1 = "Test Addr.", PostalSuburbCity = "Sydney" }, UserCredentials = new UserCredentials { UserEntityId = 2, UserId = 2, AccountExpireFlag = 0, EnabledDate = DateTime.Today.AddDays(-2), LoginName = "LXMUser", Password = "LXMUser@85621", Visible = false } };
            }

            /// <summary>
            /// The fake selected user with user credential not in group.
            /// </summary>
            /// <returns>
            /// The <see cref="UserDetails"/>.
            /// </returns>
            internal static UserDetails FakeSelectedUserWithUserCredentialNotInGroup()
            {
                return new UserDetails { UserEntityId = 1, IsNewUser = false, LXMUserDetails = new LXMUserDetail { UserEntityId = 10, Firstname = "Test", Lastname = "User", AddressLine1 = "Test Addr.", PostalSuburbCity = "Sydney" }, UserCredentials = new UserCredentials { UserEntityId = 2, UserId = 2, AccountExpireFlag = 0, EnabledDate = DateTime.Today.AddDays(-2), LoginName = "LXMUser", Password = "LXMUser@85621", Visible = false }, LXMUserGroups = new ObservableModelCollection<LXMGroup>(FakeLxmUserGroups()) };
            }

            /// <summary>
            /// The fake selected user task.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<UserDetails> FakeSelectedUserTask()
            {
                return await Task.Run(
                    () =>
                    {
                        return new UserDetails { UserEntityId = 1, IsNewUser = false, LXMUserDetails = new LXMUserDetail { Firstname = "Test", Lastname = "User", AddressLine1 = "Test Addr.", PostalSuburbCity = "Sydney" } };
                    });
            }

            /// <summary>
            /// The fake selected user with group.
            /// </summary>
            /// <returns>
            /// The <see cref="UserDetails"/>.
            /// </returns>
            internal static UserDetails FakeSelectedUserWithGroup()
            {
                return new UserDetails { UserEntityId = 1, LXMUserDetails = new LXMUserDetail { Firstname = "Test", Lastname = "User", AddressLine1 = "Test Addr.", PostalSuburbCity = "Sydney" }, LXMUserGroups = new ObservableModelCollection<LXMGroup>(FakeLxmUserGroups()) };
            }

            #endregion

            #region Fake Data OptionPermission

            /// <summary>
            /// The fake option permission list.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<OptionPermission>> FakeOptionPermissionList()
            {
                return await Task.Run(
                    () =>
                    {
                        List<OptionPermission> optionPermissions;

                        optionPermissions = new List<OptionPermission>();
                        optionPermissions.Add(new OptionPermission { UserEntityID = 100, OptionValue = "System" });
                        optionPermissions.Add(new OptionPermission { UserEntityID = 150, OptionValue = "SystemAdmin" });
                        optionPermissions.Add(new OptionPermission { UserEntityID = 50, OptionValue = "Option" });

                        return new List<OptionPermission>(optionPermissions);
                    });
            }

            /// <summary>
            /// The fake option permission list entity.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<OptionPermission>> FakeOptionPermissionListEntity()
            {
                return await Task.Run(
                    () =>
                    {
                        List<OptionPermission> optionPermissions;

                        optionPermissions = new List<OptionPermission>();
                        optionPermissions.Add(new OptionPermission { UserEntityID = 25, OptionValue = "System" });
                        optionPermissions.Add(new OptionPermission { UserEntityID = 36, OptionValue = "SystemAdmin" });

                        return new List<OptionPermission>(optionPermissions);
                    });
            }
            #endregion

            #region Fake Data SystemParam

            /// <summary>
            /// The fake states list.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<IEnumerable<SystemParam>> FakeStatesList()
            {
                return await Task.Run(
                    () =>
                    {
                        List<SystemParam> states;

                        states = new List<SystemParam>();
                        states.Add(
                            new SystemParam
                                {
                                    ParamId = 4,
                                    ParamType = 4,
                                    ParamDesc = "four",
                                    ParamElement = "numberFour",
                                    LastUserId = 1,
                                    LastDateModified = new DateTime(2014, 8, 15, 11, 00, 00),
                                    Enabled = true
                                });
                        states.Add(
                            new SystemParam
                                {
                                    ParamId = 5,
                                    ParamType = 5,
                                    ParamDesc = "five",
                                    ParamElement = "numberFive",
                                    LastUserId = 1,
                                    LastDateModified = new DateTime(2014, 8, 15, 11, 00, 00),
                                    Enabled = true
                                });
                        states.Add(
                            new SystemParam
                                {
                                    ParamId = 6,
                                    ParamType = 6,
                                    ParamDesc = "six",
                                    ParamElement = "numberSix",
                                    LastUserId = 1,
                                    LastDateModified = new DateTime(2014, 8, 15, 11, 00, 00),
                                    Enabled = true
                                });
                        states.Add(
                            new SystemParam
                                {
                                    ParamId = 7,
                                    ParamType = 7,
                                    ParamDesc = "seven",
                                    ParamElement = "numberSeven",
                                    LastUserId = 1,
                                    LastDateModified = new DateTime(2014, 8, 15, 11, 00, 00),
                                    Enabled = true
                                });

                        return new List<SystemParam>(states);
                    });
            }

            #endregion

            #region Fake Data LXMUserDetail

            /// <summary>
            /// The fake user detail.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<LXMUserDetail> FakeLxmUserDetail()
            {
                return await Task.Run(
                    () =>
                    {
                        return new LXMUserDetail
                                   {
                                       UserEntityId = 1,
                                       Firstname = "Thinh",
                                       Middlename = "Phuc",
                                       Lastname = "Pham",
                                       JobTitle = "Senior",
                                       PhoneNumber = "01228131980",
                                       MobileNumber = "01228131980",
                                       FaxNumber = "123",
                                       Email = "123@gmail.com",
                                       Fullname = "Thinh pham",
                                       AddressLine1 = null,
                                       AddressLine2 = null,
                                       AddressLine3 = null,
                                       SuburbCity = "HCM",
                                       StateId = 10,
                                       ZipCode = "9000",
                                       PostalAddress = null,
                                       PostalSuburbCity = null,
                                       PostalStateId = 7,
                                       PostalZipCode = "92000"
                                   };
                    });
            }

            /// <summary>
            /// The fake user detail list.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<LXMUserDetail>> FakeLxmUserDetailList()
            {
                return await Task.Run(
                    () =>
                    {
                        List<LXMUserDetail> users;

                        users = new List<LXMUserDetail>();
                        users.Add(
                            new LXMUserDetail
                                {
                                    UserEntityId = 100,
                                    Firstname = "Duy",
                                    Middlename = "Nhat",
                                    Lastname = "Nguyen"
                                });
                        users.Add(
                            new LXMUserDetail
                                {
                                    UserEntityId = 150,
                                    Firstname = "Cuong",
                                    Middlename = "Trung",
                                    Lastname = "Pham"
                                });
                        users.Add(
                            new LXMUserDetail
                                {
                                    UserEntityId = 50,
                                    Firstname = "Thinh",
                                    Middlename = "Phuc",
                                    Lastname = "Pham"
                                });

                        return new List<LXMUserDetail>(users);
                    });
            }

            #endregion

            #region Fake Data UserCredential

            /// <summary>
            /// The fake user credentials.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<UserCredentials> FakeUserCredentials()
            {
                return await Task.Run(
                    () =>
                    {
                        return new UserCredentials
                                   {
                                       UserEntityId = 2,
                                       UserId = 2,
                                       AccountExpireFlag = 0,
                                       EnabledDate = DateTime.Today.AddDays(-2),
                                       LoginName = "LXMUser",
                                       Password = "LXMUser@85621",
                                       Visible = true
                                   };
                    });
            }

            /// <summary>
            /// The fake user credentials list.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<LXMUser>> FakeUserCredentialsList()
            {
                return await Task.Run(
                    () =>
                    {
                        List<LXMUser> users;

                        users = new List<LXMUser>();
                        users.Add(
                            new LXMUser
                                {
                                    UserEntityId = 100,
                                    PasswordExpiryDate = new DateTime(2014, 08, 18, 2, 15, 0, 0)
                                });
                        users.Add(
                            new LXMUser
                                {
                                    UserEntityId = 150,
                                    PasswordExpiryDate = new DateTime(2014, 08, 18, 2, 15, 0, 0)
                                });
                        users.Add(
                            new LXMUser
                                {
                                    UserEntityId = 50,
                                    PasswordExpiryDate = new DateTime(2014, 08, 18, 2, 15, 0, 0)
                                });

                        return new List<LXMUser>(users);
                    });
            }

            #endregion

            #region Fake Data Component

            /// <summary>
            /// The fake flatten components list.
            /// </summary>
            /// <returns>
            /// The <see cref="List"/>.
            /// </returns>
            internal static List<Component> FakeFlattennedComponentsList()
            {
                List<Component> flattennedComponents;

                flattennedComponents = new List<Component>();

                flattennedComponents.Add(new Component { Name = "System", IsEnabled = true, ID = 1 });
                flattennedComponents.Add(new Component { Name = "System.System Management", IsEnabled = true, ID = 2 });
                flattennedComponents.Add(new Component { Name = "System.System Management.Parameters", IsEnabled = true, ID = 3 });

                flattennedComponents.Last().Forms = new ObservableModelCollection<Form>();
                flattennedComponents.Last().Forms.Add(new Form { Name = "Banks", IsEnabled = true, ID = 101 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Bookmarks", IsEnabled = true, ID = 102 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Document Branding", IsEnabled = true, ID = 103 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Document Templates", IsEnabled = true, ID = 104 });

                flattennedComponents.Add(new Component { Name = "System.System Management.System Settings", IsEnabled = true });

                flattennedComponents.Last().Forms = new ObservableModelCollection<Form>();
                flattennedComponents.Last().Forms.Add(new Form { Name = "Amort Schedule", IsEnabled = true, ID = 201 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Entity", IsEnabled = true, ID = 202 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Facility Limits", IsEnabled = true, ID = 203 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Finance One", IsEnabled = true, ID = 204 });

                return flattennedComponents;
            }

            /// <summary>
            /// The fake selected user components.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableModelCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableModelCollection<Component> FakeSelectedUserComponents()
            {
                List<Component> flattennedComponents;

                flattennedComponents = new List<Component>();

                flattennedComponents.Add(new Component { Name = "System", IsEnabled = true, ID = 1 });
                flattennedComponents.Add(new Component { Name = "System.System Management", IsEnabled = true, ID = 2 });
                flattennedComponents.Add(new Component { Name = "System.System Management.Parameters", IsEnabled = true, ID = 3 });

                flattennedComponents.Last().Forms = new ObservableModelCollection<Form>();
                flattennedComponents.Last().Forms.Add(new Form { Name = "Banks", IsEnabled = true, ID = 101 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Bookmarks", IsEnabled = true, ID = 102 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Document Branding", IsEnabled = true, ID = 103 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Document Templates", IsEnabled = true, ID = 104 });

                flattennedComponents.Add(new Component { Name = "System.System Management.System Settings", IsEnabled = true });

                flattennedComponents.Last().Forms = new ObservableModelCollection<Form>();
                flattennedComponents.Last().Forms.Add(new Form { Name = "Amort Schedule", IsEnabled = true, ID = 201 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Entity", IsEnabled = true, ID = 202 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Facility Limits", IsEnabled = true, ID = 203 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Finance One", IsEnabled = true, ID = 204 });

                return new ObservableModelCollection<Component>(flattennedComponents);
            }

            /// <summary>
            /// The fake flatten components.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>IEnumerable</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static async Task<IEnumerable<Component>> FakeFlattennedComponents()
            {
                return await Task.Run(
                    () =>
                    {
                        List<Component> flattennedComponents;

                        flattennedComponents = new List<Component>();

                        flattennedComponents.Add(new Component { Name = "System", IsEnabled = true, ID = 1 });
                        flattennedComponents.Add(
                            new Component { Name = "System.System Management", IsEnabled = true, ID = 2 });
                        flattennedComponents.Add(
                            new Component { Name = "System.System Management.Parameters", IsEnabled = true, ID = 3 });

                        flattennedComponents.Last().Forms = new ObservableModelCollection<Form>();
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Banks", IsEnabled = true, ID = 101 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Bookmarks", IsEnabled = true, ID = 102 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Document Branding", IsEnabled = true, ID = 103 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Document Templates", IsEnabled = true, ID = 104 });

                        flattennedComponents.Add(
                            new Component { Name = "System.System Management.System Settings", IsEnabled = true });

                        flattennedComponents.Last().Forms = new ObservableModelCollection<Form>();
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Amort Schedule", IsEnabled = true, ID = 201 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Entity", IsEnabled = true, ID = 202 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Facility Limits", IsEnabled = true, ID = 203 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Finance One", IsEnabled = true, ID = 204 });

                        return flattennedComponents;
                    });
            }

            /// <summary>
            /// The fake flatten components task.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<IEnumerable<Component>> FakeFlattennedComponentsTask()
            {
                return await Task.Run(
                    () =>
                    {
                        List<Component> flattennedComponents;

                        flattennedComponents = new List<Component>();

                        flattennedComponents.Add(new Component { Name = "System", IsEnabled = true, ID = 1 });
                        flattennedComponents.Add(
                            new Component { Name = "System.System Management", IsEnabled = true, ID = 2 });
                        flattennedComponents.Add(
                            new Component { Name = "System.System Management.Parameters", IsEnabled = true, ID = 3 });

                        flattennedComponents.Last().Forms = new ObservableModelCollection<Form>();
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Banks", IsEnabled = true, ID = 101 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Bookmarks", IsEnabled = true, ID = 102 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Document Branding", IsEnabled = true, ID = 103 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Document Templates", IsEnabled = true, ID = 104 });

                        flattennedComponents.Add(
                            new Component { Name = "System.System Management.System Settings", IsEnabled = true });

                        flattennedComponents.Last().Forms = new ObservableModelCollection<Form>();
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Amort Schedule", IsEnabled = true, ID = 201 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Entity", IsEnabled = true, ID = 202 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Facility Limits", IsEnabled = true, ID = 203 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Finance One", IsEnabled = true, ID = 204 });

                        return flattennedComponents;
                    });
            }
            #endregion
        }

        /// <summary>
        /// The fake data security group.
        /// </summary>
        internal static class FakeDataSecurityGroup
        {
            #region Fake Data Methods

            /// <summary>
            /// fake Managers.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>List</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static List<DropdownList> FakeManagers()
            {
                List<DropdownList> fakeManagers = new List<DropdownList>
                                                  {
                                                      new DropdownList { Description = "Manager01", ID = 1 },
                                                      new DropdownList { Description = "Manager02", ID = 2 }
                                                  };
                return fakeManagers;
            }

            /// <summary>
            /// The fake user details.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<LXMUserDetail>> FakeUserDetails()
            {
                return await Task.Run(
                    () =>
                    {
                        var fakeUserDetails = new List<LXMUserDetail>
                                                      {
                                                          new LXMUserDetail { UserEntityId = 7 },
                                                          new LXMUserDetail { UserEntityId = 8 },
                                                          new LXMUserDetail { UserEntityId = 9 }
                                                      };
                        return fakeUserDetails;
                    });
            }

            /// <summary>
            /// The fake non task user details.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>List</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static List<LXMUserDetail> FakeNonTaskUserDetails()
            {
                List<LXMUserDetail> fakeUserDetails = new List<LXMUserDetail>
                                                      {
                                                          new LXMUserDetail { UserEntityId = 7 },
                                                          new LXMUserDetail { UserEntityId = 8 },
                                                          new LXMUserDetail { UserEntityId = 9 }
                                                      };
                return fakeUserDetails;
            }

            /// <summary>
            /// The fake group details.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<GroupDetails>> FakeGroupDetails()
            {
                return await Task.Run(
                    () =>
                    {
                        List<GroupDetails> fakeGroupDetails = new List<GroupDetails>
                                                                      {
                                                                          new GroupDetails
                                                                              {
                                                                                  UserEntityId
                                                                                      =
                                                                                      7
                                                                              },
                                                                          new GroupDetails
                                                                              {
                                                                                  UserEntityId
                                                                                      =
                                                                                      8
                                                                              },
                                                                          new GroupDetails
                                                                              {
                                                                                  UserEntityId
                                                                                      =
                                                                                      9
                                                                              }
                                                                      };
                        return fakeGroupDetails;
                    });
            }

            /// <summary>
            /// The fake group detail.
            /// </summary>
            /// <returns>
            /// The <see cref="GroupDetails"/>.
            /// </returns>
            internal static GroupDetails FakeGroupDetail()
            {
                GroupDetails fakeGroupDetail = new GroupDetails
                {
                    UserEntityId = 7,
                    IsNewGroup = false,
                    IsAllSystemOption = false,
                    Users = FakeUsers(),
                    LXMGroup = new LXMGroup { ManagerUserEntityId = 4 }
                };
                return fakeGroupDetail;
            }

            /// <summary>
            /// The fake group detail 3.
            /// </summary>
            /// <returns>
            /// The <see cref="GroupDetails"/>.
            /// </returns>
            internal static GroupDetails FakeGroupDetail3()
            {
                GroupDetails fakeGroupDetail = new GroupDetails
                {
                    UserEntityId = 7,
                    IsNewGroup = false,
                    IsAllSystemOption = false,
                    Users = FakeUsers(),
                    SelectedComponent = new TreeComponent { HasAdd = false, HasDelete = false },
                    LXMGroup = new LXMGroup { ManagerUserEntityId = 4 }
                };
                return fakeGroupDetail;
            }

            /// <summary>
            /// The fake group detail for validate.
            /// </summary>
            /// <returns>
            /// The <see cref="GroupDetails"/>.
            /// </returns>
            internal static GroupDetails FakeGroupDetailForValidate()
            {
                GroupDetails fakeGroupDetail = new GroupDetails
                {
                    LXMGroup = new LXMGroup { ManagerUserEntityId = 4 }
                };
                return fakeGroupDetail;
            }

            /// <summary>
            /// The fake non group detail.
            /// </summary>
            /// <returns>
            /// The <see cref="GroupDetails"/>.
            /// </returns>
            internal static GroupDetails FakeNonLxmGroupDetail()
            {
                GroupDetails fakeGroupDetail = new GroupDetails
                {
                    UserEntityId = 7,
                    IsNewGroup = false,
                    IsAllSystemOption = false,
                    Users = FakeUsers()
                };
                return fakeGroupDetail;
            }

            /// <summary>
            /// The fake all users.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableCollection<LXMUserDetail> FakeAllUsers()
            {
                List<LXMUserDetail> users;

                users = new List<LXMUserDetail>();
                users.Add(new LXMUserDetail { Firstname = "Test", Lastname = "User", AddressLine1 = "Test Addr.", PostalSuburbCity = "Sydney", UserEntityId = 7 });
                users.Add(new LXMUserDetail { Firstname = "Test1", Lastname = "User1", AddressLine1 = "Test Addr.", PostalSuburbCity = "Sydney1", UserEntityId = 8 });
                users.Add(new LXMUserDetail { Firstname = "Test2", Lastname = "User2", AddressLine1 = "Test Addr.", PostalSuburbCity = "Sydney2", UserEntityId = 9 });
                return new ObservableCollection<LXMUserDetail>(users);
            }

            /// <summary>
            /// The fake object user details.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableCollection<object> FakeObjLxmUserDetails()
            {
                List<LXMUserDetail> users;

                users = new List<LXMUserDetail>();
                users.Add(new LXMUserDetail { Firstname = "Test", Lastname = "User", AddressLine1 = "Test Addr.", PostalSuburbCity = "Sydney", UserEntityId = 4 });
                users.Add(new LXMUserDetail { Firstname = "Test1", Lastname = "User1", AddressLine1 = "Test Addr.", PostalSuburbCity = "Sydney1", UserEntityId = 7 });
                users.Add(new LXMUserDetail { Firstname = "Test2", Lastname = "User2", AddressLine1 = "Test Addr.", PostalSuburbCity = "Sydney2", UserEntityId = 9 });
                return new ObservableCollection<object>(users);
            }

            /// <summary>
            /// The fake a group.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<LXMGroup> FakeAGroup()
            {
                return await Task.Run(
                    () =>
                    {
                        LXMGroup group = new LXMGroup
                        {
                            GroupId = 2,
                            UserEntityId = 7,
                            GroupName = "Group01",
                            LastUserId = 4
                        };
                        return group;
                    });
            }

            /// <summary>
            /// The fake new group.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<GroupDetails> FakeNewGroup()
            {
                return await Task.Run(
                    () =>
                    {
                        GroupDetails group = new GroupDetails
                        {
                            UserEntityId = -1,
                            IsNewGroup = true,
                            LXMGroup =
                                new LXMGroup
                                {
                                    AllowTaskAssignment = false,
                                    DateCreated = DateTime.Now,
                                    GroupName = string.Empty,
                                    ManagerUserEntityId = 0,
                                    LastDateModified = DateTime.Now,
                                    LastUserId = 7
                                }
                        };
                        return group;
                    });
            }

            /// <summary>
            /// The fake GetUserEntityOptionPermissionsAsync.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<OptionPermission>> FakePermissionOptions()
            {
                return await Task.Run(
                    () =>
                    {
                        List<OptionPermission> permissionOptions = new List<OptionPermission>
                                                                           {
                                                                               new OptionPermission
                                                                                   {
                                                                                       UserEntityID
                                                                                           =
                                                                                           7,
                                                                                       ComponentOptionID
                                                                                           =
                                                                                           7
                                                                                   },
                                                                               new OptionPermission
                                                                                   {
                                                                                       UserEntityID
                                                                                           =
                                                                                           8,
                                                                                       ComponentOptionID
                                                                                           =
                                                                                           8
                                                                                   }
                                                                           };
                        return new List<OptionPermission>(permissionOptions);
                    });
            }

            /// <summary>
            /// The fake flatten components.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<IEnumerable<Component>> FakeFlattennedComponents()
            {
                return await Task.Run(
                    () =>
                    {
                        List<Component> flattennedComponents;

                        flattennedComponents = new List<Component>();

                        flattennedComponents.Add(new Component { Name = "System", IsEnabled = true, ID = 1 });
                        flattennedComponents.Add(
                            new Component { Name = "System.System Management", IsEnabled = true, ID = 2 });
                        flattennedComponents.Add(
                            new Component { Name = "System.System Management.Parameters", IsEnabled = true, ID = 3 });

                        flattennedComponents.Last().Forms = new ObservableModelCollection<Form>();
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Banks", IsEnabled = true, ID = 101 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Bookmarks", IsEnabled = true, ID = 102 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Document Branding", IsEnabled = true, ID = 103 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Document Templates", IsEnabled = true, ID = 104 });

                        flattennedComponents.Add(
                            new Component { Name = "System.System Management.System Settings", IsEnabled = true });

                        flattennedComponents.Last().Forms = new ObservableModelCollection<Form>();
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Amort Schedule", IsEnabled = true, ID = 201 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Entity", IsEnabled = true, ID = 202 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Facility Limits", IsEnabled = true, ID = 203 });
                        flattennedComponents.Last()
                            .Forms.Add(new Form { Name = "Finance One", IsEnabled = true, ID = 204 });

                        return flattennedComponents;
                    });
            }

            /// <summary>
            /// The fake non task flatten components.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>IEnumerable</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static IEnumerable<Component> FakeNonTaskFlattennedComponents()
            {
                List<Component> flattennedComponents;

                flattennedComponents = new List<Component>();

                flattennedComponents.Add(new Component { Name = "System", IsEnabled = true, ID = 1 });
                flattennedComponents.Add(new Component { Name = "System.System Management", IsEnabled = true, ID = 2 });
                flattennedComponents.Add(new Component { Name = "System.System Management.Parameters", IsEnabled = true, ID = 3 });

                flattennedComponents.Last().Forms = new ObservableModelCollection<Form>();
                flattennedComponents.Last().Forms.Add(new Form { Name = "Banks", IsEnabled = true, ID = 101 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Bookmarks", IsEnabled = true, ID = 102 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Document Branding", IsEnabled = true, ID = 103 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Document Templates", IsEnabled = true, ID = 104 });

                flattennedComponents.Add(new Component { Name = "System.System Management.System Settings", IsEnabled = true });

                flattennedComponents.Last().Forms = new ObservableModelCollection<Form>();
                flattennedComponents.Last().Forms.Add(new Form { Name = "Amort Schedule", IsEnabled = true, ID = 201 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Entity", IsEnabled = true, ID = 202 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Facility Limits", IsEnabled = true, ID = 203 });
                flattennedComponents.Last().Forms.Add(new Form { Name = "Finance One", IsEnabled = true, ID = 204 });

                return flattennedComponents;
            }

            /// <summary>
            /// The fake users.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableModelCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableModelCollection<LXMUserDetail> FakeUsers()
            {
                List<LXMUserDetail> users;

                users = new List<LXMUserDetail>();
                users.Add(
                    new LXMUserDetail
                    {
                        Firstname = "Test",
                        Lastname = "User",
                        AddressLine1 = "Test Addr.",
                        PostalSuburbCity = "Sydney",
                        UserEntityId = 7
                    });
                users.Add(
                    new LXMUserDetail
                    {
                        Firstname = "Test1",
                        Lastname = "User1",
                        AddressLine1 = "Test Addr.",
                        PostalSuburbCity = "Sydney1",
                        UserEntityId = 8
                    });

                return new ObservableModelCollection<LXMUserDetail>(users);
            }

            /// <summary>
            /// The fake non task group details.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableCollection<GroupDetails> FakeNonTaskGroupDetails()
            {
                List<GroupDetails> groups;

                groups = new List<GroupDetails>();
                groups.Add(new GroupDetails { UserEntityId = 38 });
                groups.Add(new GroupDetails { UserEntityId = 49 });
                groups.Add(new GroupDetails { UserEntityId = 51 });

                return new ObservableCollection<GroupDetails>(groups);
            }

            /// <summary>
            /// The fake managers 1.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>List</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static List<DropdownList> FakeManagers1()
            {
                List<DropdownList> managers = new List<DropdownList>();
                managers.Add(new DropdownList { ID = 5, Description = "Managers1_01" });
                managers.Add(new DropdownList { ID = 4, Description = "Managers1_02" });
                return new List<DropdownList>(managers);
            }

            /// <summary>
            /// The fake group detail 2.
            /// </summary>
            /// <returns>
            /// The <see cref="GroupDetails"/>.
            /// </returns>
            internal static GroupDetails FakeGroupDetail2()
            {
                GroupDetails fakeGroupDetail = new GroupDetails
                {
                    UserEntityId = 7,
                    IsNewGroup = true,
                };
                return fakeGroupDetail;
            }

            #endregion

        }

        /// <summary>
        /// The fake data security system setting.
        /// </summary>
        internal static class FakeDataSecuritySystemSetting
        {
            /// <summary>
            /// The fake system setting.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<LXMUserSystemSetting> FakeSystemSetting()
            {
                return await Task.Run(
                    () =>
                    {
                        return new LXMUserSystemSetting
                                   {
                                       DaysInactivity = 10,
                                       GraceLogins = 5,
                                       NoOfInvalidLogins = 4,
                                       PasswordChangeDays = 10,
                                       PasswordHistoryStore = 5,
                                       PasswordStrength = "@X123"
                                   };
                    });
            }

            /// <summary>
            /// The fake a system setting.
            /// </summary>
            /// <returns>
            /// The <see cref="LXMUserSystemSetting"/>.
            /// </returns>
            internal static LXMUserSystemSetting FakeASystemSetting()
            {
                return new LXMUserSystemSetting { DaysInactivity = 10, GraceLogins = 5, NoOfInvalidLogins = 4, PasswordChangeDays = 10, PasswordHistoryStore = 5, PasswordStrength = "@X123" };
            }
        }

    }
}
