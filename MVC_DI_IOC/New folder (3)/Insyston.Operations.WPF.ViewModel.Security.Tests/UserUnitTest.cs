// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserUnitTest.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Summary description for UnitTest1
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Security.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Common;
    using Insyston.Operations.Business.Security;
    using Insyston.Operations.Business.Security.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.ObjectMap;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// The user unit test.
    /// </summary>
    [TestClass]
    public class UserUnitTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserUnitTest"/> class.
        /// </summary>
        public UserUnitTest()
        {
            Map.InitialiseMap();
        }

        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        #endregion

        #region Unit Tests

        /// <summary>
        /// The test data entities mocked.
        /// Expect Component.Count == 4
        /// </summary>
        [TestMethod]
        public void TestDataEntities_MockComponent()
        {
            Entities model;

            model = Mock.Create<Entities>();

            Mock.Arrange(() => model.Components).IgnoreInstance().ReturnsCollection(FakeDataSecurity.FakeDataSecurityUser.FakeFlattennedComponentsList());

            Assert.AreEqual(4, model.Components.Count());
        }

        /// <summary>
        /// The test static method mocked.
        /// Expect UserEntityId have value
        /// </summary>
        [TestMethod]
        public void TestStaticMethod_MockGetUserDetail()
        {
            Mock.SetupStatic(typeof(UserFunctions), StaticConstructor.Mocked);

            Mock.Arrange(() => UserFunctions.GetUserDetailsAsync(1)).Returns(FakeDataSecurity.FakeDataSecurityUser.FakeLxmUserDetail());

            var actual = UserFunctions.GetUserDetailsAsync(1);

            Assert.AreEqual(1, actual.Result.UserEntityId);
        }

        /// <summary>
        /// The test set selected user async.
        /// Expect isChanged from true to false, and data load to selectedUser
        /// </summary>
        [TestMethod]
        public void TestSetSelectedUserAsync_SetDataSelected()
        {
            GeneralThreadAffineContext.Run(
                // ReSharper disable once CSharpWarnings::CS1998
                async () =>
                    {
                        // Mock enviroment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                        ViewUsersViewModel userViewModel;

                        // Create mock
                        userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);

                        // Fake data
                        userViewModel.IsChanged = true;
                        userViewModel.SelectedUser = FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUser();

                        // Test
                        bool actual = userViewModel.IsChanged;
                        var actual2 = userViewModel.SelectedUser.LXMUserDetails;

                        Assert.AreEqual("Test", actual2.Firstname);
                        Assert.AreEqual(false, actual);
                    });
        }

        /// <summary>
        /// The test on step async start load all user.
        /// Expect : ActionCommand have "Add" and AllUser is loaded
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncStart_LoadAllUser()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock enviroment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();

                        ViewUsersViewModel userViewModel;

                        // Create mock
                        userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);

                        // Create mock static class
                        Mock.SetupStatic(typeof(SystemParamsFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(GroupFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(UserFunctions), StaticConstructor.Mocked);

                        // Fake data
                        Mock.Arrange(() => SystemParamsFunctions.GetAllStatesAsync()).Returns(FakeDataSecurity.FakeDataSecurityUser.FakeStatesList());
                        Mock.Arrange(() => GroupFunctions.GetGroupsAsync()).Returns(FakeDataSecurity.FakeDataSecurityUser.FakeGroupsList());
                        Mock.Arrange(() => UserFunctions.GetUserGroupsAsync(77, userViewModel.AllGroups))
                            .IgnoreArguments()
                            .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeGroupsList());
                        Mock.Arrange(() => UserFunctions.GetAllUserDetailsAsync()).Returns(FakeDataSecurity.FakeDataSecurityUser.FakeLxmUserDetailList());
                        Mock.Arrange(() => UserFunctions.GetAllUserCredentialsAsync())
                            .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeUserCredentialsList());

                        Mock.Arrange(() => userViewModel.CanEdit).Returns(true);

                        // Run method
                        await userViewModel.OnStepAsync("Start");

                        // Test
                        // Test load user correctly
                        int actual = userViewModel.AllUsers.Count;

                        // Test action command
                        var actual2 = userViewModel.ActionCommands;

                        Assert.AreEqual(3, actual);
                        Assert.AreEqual("Add", actual2.First().Parameter);
                    });
        }

        /// <summary>
        /// The test on step async select user not checked out when select user.
        /// Expect ActionCommand is add 3 action, selectedUser have data and check permission ok
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncSelectUser_SetDataWithNotCheckedOut()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock enviroment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();

                        ViewUsersViewModel userViewModel;
                        List<TreeComponent> treeComponent;

                        // Create mock
                        userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);
                        treeComponent = Mock.Create<List<TreeComponent>>();

                        // Create mock static class
                        Mock.SetupStatic(typeof(UserFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(PermissionFunctions), StaticConstructor.Mocked);

                        // Fake data
                        Mock.Arrange(() => userViewModel.SelectedUser).Returns(FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUser());
                        Mock.Arrange(() => userViewModel.IsCheckedOut).Returns(true);
                        
                        Mock.Arrange(() => UserFunctions.GetUserDetailsAsync(userViewModel.SelectedUser.UserEntityId))
                            .IgnoreArguments()
                            .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeLxmUserDetail());
                        Mock.Arrange(
                            () =>
                            UserFunctions.GetUserGroupsAsync(
                                userViewModel.SelectedUser.UserEntityId,
                                userViewModel.AllGroups)).IgnoreArguments().Returns(FakeDataSecurity.FakeDataSecurityUser.FakeGroupsList());
                        Mock.Arrange(() => UserFunctions.GetUserCredentialsAsync(userViewModel.SelectedUser))
                            .IgnoreArguments()
                            .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeUserCredentials());
                        Mock.Arrange(
                            () =>
                            PermissionFunctions.GetUserEntityOptionPermissionsAsync(
                                userViewModel.SelectedUser.UserEntityId))
                            .IgnoreArguments()
                            .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeOptionPermissionList());
                        Mock.Arrange(
                            () =>
                            PermissionFunctions.InitialiseFlattennedComponentsAndPermissionsAsync(
                                userViewModel.SelectedUser.UserEntityId,
                                treeComponent,
                                true,
                                false)).IgnoreArguments().Returns(FakeDataSecurity.FakeDataSecurityUser.FakeFlattennedComponentsTask());
                        Mock.Arrange(
                            () =>
                            PermissionFunctions.MakeComponentsInHierarchy(
                                new List<Component>(),
                                new List<TreeComponent>(),
                                false)).IgnoreArguments().Returns(await FakeDataSecurity.FakeDataSecurityUser.FakeFlattennedComponents());
                        
                        // Mock method non public
                        Mock.NonPublic.Arrange<bool>(
                            userViewModel,
                            "CheckAllSystemOptionPermission",
                            ArgExpr.IsNull<Component>(),
                            true).IgnoreArguments().Returns(true);

                        Mock.Arrange(() => userViewModel.CanEdit).Returns(true);
                        Mock.Arrange(() => userViewModel.IsCheckedOut).Returns(false);

                        // Run method
                        await userViewModel.OnStepAsync("SelectUser");

                        // Test
                        // Test get detail of selected user
                        var actual = userViewModel.SelectedUser.LXMUserDetails;

                        // Test action command
                        var actual2 = userViewModel.ActionCommands;

                        // Test check correctly system option permisson of selected user
                        bool actual3 = userViewModel.SelectedUser.IsCheckedSystemOptionPermission;

                        Assert.AreEqual("Thinh", actual.Firstname);
                        Assert.AreEqual(3, actual2.Count);
                        Assert.IsTrue(actual3);
                    });
        }

        /// <summary>
        /// The test on step async select user is checked out when select user.
        /// Expect ActionCommand is add 3 action, selectedUser have data, check permission ok and user are not in group
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncSelectUser_SetDataWithIsCheckedOut()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock enviroment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();

                        ViewUsersViewModel userViewModel;
                        List<TreeComponent> treeComponent;

                        // Create mock
                        userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);
                        treeComponent = Mock.Create<List<TreeComponent>>();

                        // Create mock static class
                        Mock.SetupStatic(typeof(UserFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(PermissionFunctions), StaticConstructor.Mocked);

                        // Fake data
                        Mock.Arrange(() => UserFunctions.GetUserDetailsAsync(userViewModel.SelectedUser.UserEntityId))
                            .IgnoreArguments()
                            .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeLxmUserDetail());
                        Mock.Arrange(
                            () =>
                            UserFunctions.GetUserGroupsAsync(
                                userViewModel.SelectedUser.UserEntityId,
                                userViewModel.AllGroups)).IgnoreArguments().Returns(FakeDataSecurity.FakeDataSecurityUser.FakeGroupsList());
                        Mock.Arrange(() => UserFunctions.GetUserCredentialsAsync(userViewModel.SelectedUser))
                            .IgnoreArguments()
                            .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeUserCredentials());
                        Mock.Arrange(
                            () =>
                            PermissionFunctions.GetUserEntityOptionPermissionsAsync(
                                userViewModel.SelectedUser.UserEntityId))
                            .IgnoreArguments()
                            .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeOptionPermissionList());
                        Mock.Arrange(
                            () =>
                            PermissionFunctions.InitialiseFlattennedComponentsAndPermissionsAsync(
                                userViewModel.SelectedUser.UserEntityId,
                                treeComponent,
                                true,
                                false)).IgnoreArguments().Returns(FakeDataSecurity.FakeDataSecurityUser.FakeFlattennedComponentsTask());
                        Mock.Arrange(
                            () =>
                            PermissionFunctions.MakeComponentsInHierarchy(
                                new List<Component>(),
                                new List<TreeComponent>(),
                                false)).IgnoreArguments().Returns(await FakeDataSecurity.FakeDataSecurityUser.FakeFlattennedComponents());
                        
                        // Mock method non public
                        Mock.NonPublic.Arrange<bool>(
                            userViewModel,
                            "CheckAllSystemOptionPermission",
                            ArgExpr.IsNull<Component>(),
                            true).IgnoreArguments().Returns(true);

                        Mock.Arrange(() => userViewModel.Edit.OnStepAsync(EditUserViewModel.EnumSteps.Start))
                            .IgnoreArguments()
                            .Returns(Task.Run(() => { }));

                        Mock.Arrange(() => userViewModel.SelectedUser).Returns(FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUser());
                        Mock.Arrange(() => userViewModel.CanEdit).Returns(true);
                        Mock.Arrange(() => userViewModel.IsCheckedOut).Returns(true);

                        // Run method
                        await userViewModel.OnStepAsync("SelectUser");

                        // Test
                        // Test get detail of selected user
                        var actual = userViewModel.SelectedUser.LXMUserDetails;

                        // Test action command
                        var actual2 = userViewModel.ActionCommands;

                        // Test check correctly system option permisson of selected user
                        bool actual3 = userViewModel.SelectedUser.IsCheckedSystemOptionPermission;

                        // Test if member of group or not
                        var actual4 = userViewModel.Edit.NotAMemberOfGroups;

                        Assert.AreEqual("Thinh", actual.Firstname);
                        Assert.AreEqual(3, actual2.Count);
                        Assert.IsTrue(actual3);
                        Assert.AreEqual(0, actual4.Count);
                    });
        }

        /// <summary>
        /// The test on step async edit data selected.
        /// Expect user in group
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncEdit_EditDataSelected()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock enviroment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();

                        ViewUsersViewModel userViewModel;

                        // Create mock
                        userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);

                        // Fake data
                        Mock.Arrange(() => userViewModel.Edit.OnStepAsync(Arg.IsAny<object>()))
                            .IgnoreArguments()
                            .Returns(Task.Run(() => { }));
                        Mock.Arrange(() => userViewModel.SelectedUser).Returns(FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUserWithGroup());
                        Mock.Arrange(() => userViewModel.AllGroups).Returns(await FakeDataSecurity.FakeDataSecurityUser.FakeGroups());
                        Mock.Arrange(() => userViewModel.CanEdit).Returns(true);

                        // Run method
                        await userViewModel.OnStepAsync("Edit");

                        // Test
                        // Test user in group
                        int actual = userViewModel.Edit.NotAMemberOfGroups.Count;

                        Assert.AreEqual(4, actual);
                    });
        }

        /// <summary>
        /// The test edit user view model_ on step start action command.
        /// Expect ActionCommand.Count is 3 member
        /// </summary>
        [TestMethod]
        public void TestEditUserViewModel_OnStepStartActionCommand()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    EditUserViewModel editUser;
                    ViewUsersViewModel userViewModel;

                    // Create mock
                    userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);

                    editUser = Mock.Create<EditUserViewModel>(
                        x =>
                        {
                            x.CallConstructor(() => new EditUserViewModel(userViewModel));
                            x.SetBehavior(Behavior.CallOriginal);
                        });

                    editUser.SelectedUser = FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUserWithGroup();
                    editUser.SelectedUser.Components = FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUserComponents();

                    await editUser.OnStepAsync("Start");

                    // Test action command
                    var actual = editUser.ActionCommands;

                    Assert.AreEqual(3, actual.Count);
                });
        }

        /// <summary>
        /// The test on step async edit user validate remove group with user in group.
        /// Expect validate has error
        /// </summary>
        [TestMethod]
        public void TestEditUserViewModel_ValidateRemoveGroupWithUserInGroup()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    EditUserViewModel editUser;
                    ViewUsersViewModel userViewModel;

                    // Create mock
                    userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);

                    editUser = Mock.Create<EditUserViewModel>(
                        x =>
                            {
                                x.CallConstructor(() => new EditUserViewModel(userViewModel));
                                x.SetBehavior(Behavior.CallOriginal);
                            });

                    // Fake data
                    Mock.Arrange(() => editUser.SelectedAMemberOfGroups)
                        .Returns(await FakeDataSecurity.FakeDataSecurityUser.FakeGroupsObject());
                    Mock.Arrange(() => editUser.SelectedUser)
                        .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUserWithUserCredentialInGroup);

                    // Run method
                    await editUser.OnStepAsync("RemoveGroup");

                    // Test
                    var actual = editUser.HasErrors;

                    Assert.AreEqual(true, actual);
                });
        }

        /// <summary>
        /// The test edit user view model_ on step reset permission.
        /// Expect component after reload have 4 member
        /// </summary>
        [TestMethod]
        public void TestEditUserViewModel_OnStepResetPermission()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    EditUserViewModel editUser;
                    ViewUsersViewModel userViewModel;

                    // Create mock
                    userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);
                    Mock.SetupStatic(typeof(EffectiveOptionPermission), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(PermissionFunctions), StaticConstructor.Mocked);

                    editUser = Mock.Create<EditUserViewModel>(
                        x =>
                        {
                            x.CallConstructor(() => new EditUserViewModel(userViewModel));
                            x.SetBehavior(Behavior.CallOriginal);
                        });

                    editUser.SelectedUser = FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUserWithGroup();
                    editUser.SelectedUser.Components = FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUserComponents();

                    // Run method
                    await editUser.OnStepAsync("ResetPermissions");

                    // Test number of component
                    var actual = editUser.SelectedUser.Components;

                    Assert.AreEqual(4, actual.Count);
                });
        }

        /// <summary>
        /// The test edit user view model_ on step cancel.
        /// Expect isCheckedOut change from true to false
        /// </summary>
        [TestMethod]
        public void TestEditUserViewModel_OnStepCancel()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    EditUserViewModel editUser;
                    ViewUsersViewModel userViewModel;

                    // Create mock
                    userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);

                    editUser = Mock.Create<EditUserViewModel>(
                        x =>
                        {
                            x.CallConstructor(() => new EditUserViewModel(userViewModel));
                            x.SetBehavior(Behavior.CallOriginal);
                        });

                    // Fake data
                    Mock.Arrange(() => editUser.SelectedAMemberOfGroups)
                        .Returns(await FakeDataSecurity.FakeDataSecurityUser.FakeGroupsObject());
                    Mock.Arrange(() => editUser.SelectedUser)
                        .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUser());
                    Mock.Arrange(() => userViewModel.OnStepAsync(ViewUsersViewModel.EnumSteps.SelectUser))
                        .IgnoreArguments()
                        .Returns(() => Task.Run(() => { }));

                    userViewModel.IsCheckedOut = true;
                    editUser.IsCheckedOut = false;

                    // Run method
                    await editUser.OnStepAsync("Cancel");

                    // Test
                    var actual = userViewModel.IsCheckedOut;

                    Assert.IsFalse(actual);
                });
        }

        /// <summary>
        /// The test edit user view model_ on step save.
        /// Expect isCheckedOut change from true to false
        /// and SelectedUser.IsNewUser change from true to false
        /// </summary>
        [TestMethod]
        public void TestEditUserViewModel_OnStepSaveSelectedUser()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    EditUserViewModel editUser;
                    ViewUsersViewModel userViewModel;

                    // Create mock
                    userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);
                    Mock.SetupStatic(typeof(UserFunctions), StaticConstructor.Mocked);

                    editUser = Mock.Create<EditUserViewModel>(
                        x =>
                        {
                            x.CallConstructor(() => new EditUserViewModel(userViewModel));
                            x.SetBehavior(Behavior.CallOriginal);
                        });

                    // Fake data
                    Mock.Arrange(() => UserFunctions.SaveAsync(null))
                        .IgnoreArguments()
                        .Returns(() => Task.Run(() => { }));
                    Mock.Arrange(() => userViewModel.OnStepAsync(ViewUsersViewModel.EnumSteps.SelectUser))
                        .IgnoreArguments()
                        .Returns(() => Task.Run(() => { }));
                    Mock.Arrange(() => userViewModel.OnStepAsync(ViewUsersViewModel.EnumSteps.RefreshUser))
                        .IgnoreArguments()
                        .Returns(() => Task.Run(() => { }));

                    editUser.SelectedUser = FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUser();
                    userViewModel.IsCheckedOut = true;
                    editUser.IsCheckedOut = false;
                    editUser.SelectedUser.IsNewUser = true;

                    // Run method
                    await editUser.OnStepAsync("Save");

                    // Test
                    var actual = userViewModel.IsCheckedOut;
                    var actual2 = editUser.SelectedUser.IsNewUser;

                    Assert.IsFalse(actual && actual2);
                });
        }

        /// <summary>
        /// The test on step async add new data.
        /// Expect data is added and set to selectedUser
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncAdd_AddNewDataSelected()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock enviroment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();

                        ViewUsersViewModel userViewModel;

                        // Create mock
                        userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);
                        Mock.SetupStatic(typeof(UserFunctions), StaticConstructor.Mocked);

                        // Fake data
                        Mock.Arrange(() => userViewModel.OnStepAsync(ViewUsersViewModel.EnumSteps.SelectUser))
                            .Returns(Task.Run(() => { }));
                        Mock.Arrange(() => userViewModel.Edit.OnStepAsync(Arg.IsAny<object>()))
                            .IgnoreArguments()
                            .Returns(Task.Run(() => { }));
                        Mock.Arrange(
                            () => userViewModel.AllGroups.Except(Arg.IsAny<ObservableModelCollection<LXMGroup>>()))
                            .IgnoreArguments()
                            .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeGroupsListIeNumerable());
                        Mock.Arrange(() => UserFunctions.AddNewUserAsync()).Returns(FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUserTask());

                        // Run method
                        await userViewModel.OnStepAsync("Add");

                        // Test
                        // Test new user is change to selectedUser
                        var actual = userViewModel.SelectedUser;
                        var actual2 = userViewModel.Edit.SelectedUser;

                        Assert.AreEqual(1, actual.UserEntityId);
                        Assert.AreEqual(1, actual2.UserEntityId);
                    });
        }

        /// <summary>
        /// The test on step async copy selected data.
        /// Expect the user copy is change property IsNewUser from false to true
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncCopy_CopyDataSelected()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock enviroment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();

                        ViewUsersViewModel userViewModel;

                        // Create mock
                        userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);
                        Mock.SetupStatic(typeof(UserFunctions), StaticConstructor.Mocked);

                        // Fake data
                        userViewModel.SelectedUser = FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUser();

                        Mock.Arrange(() => userViewModel.OnStepAsync(ViewUsersViewModel.EnumSteps.SelectUser))
                            .Returns(Task.Run(() => { }));
                        Mock.Arrange(() => userViewModel.Edit.OnStepAsync(Arg.IsAny<object>()))
                            .IgnoreArguments()
                            .Returns(Task.Run(() => { }));
                        Mock.Arrange(
                            () => userViewModel.AllGroups.Except(Arg.IsAny<ObservableModelCollection<LXMGroup>>()))
                            .IgnoreArguments()
                            .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeGroupsListIeNumerable());
                        Mock.Arrange(() => UserFunctions.CopyUserAsync(Arg.IsAny<UserDetails>()))
                            .IgnoreArguments()
                            .Returns(FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUserTask());

                        // Run method
                        await userViewModel.OnStepAsync("Copy");

                        // Test
                        // Test isNewUser was change form false to true
                        var actual = userViewModel.SelectedUser;

                        Assert.IsTrue(actual.IsNewUser);
                    });
        }

        /// <summary>
        /// The test on step async refresh all user data.
        /// Expect if have a new user is added when refresh will add the user to AllUser
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncRefreshUserData_ReloadAllUserData()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock enviroment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();

                        ViewUsersViewModel userViewModel;

                        // Create mock
                        userViewModel = Mock.Create<ViewUsersViewModel>(Behavior.CallOriginal);

                        // Fake data
                        Mock.Arrange(() => userViewModel.AllUsers).Returns(FakeDataSecurity.FakeDataSecurityUser.FakeUsers());
                        Mock.Arrange(() => userViewModel.SelectedUser).Returns(FakeDataSecurity.FakeDataSecurityUser.FakeSelectedUser());

                        // Run method
                        await userViewModel.OnStepAsync("RefreshUser");

                        // Test
                        // Test allUser increase number of member
                        int actual = userViewModel.AllUsers.Count;

                        Assert.AreEqual(4, actual);
                    });
        }

        #endregion
    }
}
