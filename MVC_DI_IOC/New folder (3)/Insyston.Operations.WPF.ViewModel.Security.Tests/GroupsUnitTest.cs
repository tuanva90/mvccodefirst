// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupsUnitTest.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The groups unit test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Security.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Security;
    using Insyston.Operations.Business.Security.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.ObjectMap;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// The groups unit test.
    /// </summary>
    [TestClass]
    public class GroupsUnitTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsUnitTest"/> class.
        /// </summary>
        public GroupsUnitTest()
        {
            Map.InitialiseMap();
        }

        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Unit Tests

        /// <summary>
        /// When EnumerableSteps.Start
        /// Expect manager is null.
        /// </summary>
        [TestMethod]
        public void TestOnStepSyncStart_MangerIsNull()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock environment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                        // Create mock
                        var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);

                        // Managers before
                        viewModel.Managers = FakeDataSecurity.FakeDataSecurityGroup.FakeManagers();

                        Mock.SetupStatic(typeof(UserFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => UserFunctions.GetAllUserDetailsAsync()).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeUserDetails);
                        Mock.SetupStatic(typeof(GroupFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => GroupFunctions.GetGroupsSummaryAsync()).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeGroupDetails);

                        await viewModel.OnStepAsync("Start");

                        // Assert expected viewModel.Managers == null
                        Assert.IsNull(viewModel.Managers);
                    });
        }

        /// <summary>
        /// When EnumerableSteps.Start
        /// Expect Set Action Commands is Add.
        /// </summary>
        [TestMethod]
        public void TestOnStepSyncStart_SetActionCommandsAsync()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock environment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                        // Create mock
                        var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);

                        // Pass condition CanEdit == true
                        Mock.Arrange(() => viewModel.CanEdit).Returns(true);
                        Mock.SetupStatic(typeof(UserFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => UserFunctions.GetAllUserDetailsAsync()).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeUserDetails);
                        Mock.SetupStatic(typeof(GroupFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => GroupFunctions.GetGroupsSummaryAsync()).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeGroupDetails);

                        await viewModel.OnStepAsync("Start");

                        // Expected viewModel.ActionCommands.Count == 1 and viewModel.ActionCommands[0].Parameter == "Add"
                        Assert.AreEqual(1, viewModel.ActionCommands.Count);
                        Assert.AreEqual("Add", viewModel.ActionCommands[0].Parameter);
                    });
        }

        /// <summary>
        /// When EnumerableSteps.SelectGroup and Test PopulateUsersAndPermissionsForGroupAsync when isCopy == false
        /// AllUsers != null, Managers == null, SelectedGroup.IsNewGroup == true 
        /// Expect Model.Managers is not null and Components = 3, users = 4, SelectedComponent = null
        /// Expect IsAllSystemOption == true when isAllTrue || isAllFalse == true
        /// </summary>
        [TestMethod]
        public void TestOnStepSyncSelectGroup()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock environment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                        // Create mock
                        var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);

                        // Managers == null, fake AllUsers != null, SelectedGroup.IsNewGroup == true
                        Mock.Arrange(() => viewModel.SelectedGroup).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeGroupDetail());
                        viewModel.Managers = null;
                        Mock.Arrange(() => viewModel.AllUsers).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeAllUsers());

                        Mock.SetupStatic(typeof(GroupFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => GroupFunctions.GetGroupAsync(viewModel.SelectedGroup.UserEntityId))
                            .Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeAGroup());

                        // Fake inside func PopulateUsersAndPermissionsForGroupAsync
                        Mock.SetupStatic(typeof(PermissionFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(
                            () =>
                            PermissionFunctions.GetUserEntityOptionPermissionsAsync(
                                viewModel.SelectedGroup.UserEntityId)).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakePermissionOptions());
                        Mock.Arrange(
                            () =>
                            PermissionFunctions.InitialiseFlattennedComponentsAndPermissionsAsync(
                                viewModel.SelectedGroup.UserEntityId,
                                new List<TreeComponent>(),
                                !viewModel.SelectedGroup.IsNewGroup,
                                true)).IgnoreArguments().Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeFlattennedComponents());
                        Mock.Arrange(
                            () =>
                            PermissionFunctions.MakeComponentsInHierarchy(
                                new List<Component>(),
                                new List<TreeComponent>(),
                                true)).IgnoreArguments().Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeNonTaskFlattennedComponents());
                        Mock.Arrange(
                            () => GroupFunctions.GetGroupUsers(viewModel.SelectedGroup.UserEntityId, viewModel.AllUsers))
                            .Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeNonTaskUserDetails());
                        
                        Mock.NonPublic.Arrange(viewModel, "SetSystemOptionPermissionValues", new TreeComponent(), true).IgnoreArguments();

                        // Set isAllTrue and isAllFalse = true
                        Mock.NonPublic.Arrange<bool>(viewModel, "CheckAllSystemOptionPermission", new TreeComponent(), true).IgnoreArguments().Returns(true);   

                        await viewModel.OnStepAsync("SelectGroup");

                        // Expect ViewModel.Managers is not null
                        Assert.IsNotNull(viewModel.Managers);

                        Assert.AreEqual(2, viewModel.SelectedGroup.LXMGroup.GroupId);

                        // Components, users, SelectedComponent
                        Assert.AreEqual(4, viewModel.SelectedGroup.Components.Count);

                        Assert.AreEqual(3, viewModel.SelectedGroup.Users.Count);

                        Assert.AreEqual(null, viewModel.SelectedGroup.SelectedComponent);

                        // Expect IsAllSystemOption == true when isAllTrue || isAllFalse == true
                        Assert.AreEqual(true, viewModel.SelectedGroup.IsAllSystemOption);
                    });
        }

        /// <summary>
        /// When EnumerableSteps.SelectedGroup, with SelectedGroup != null or IsNewGroup == false , 
        /// Expect Set Action Commands is Add, Edit, Copy.
        /// </summary>
        [TestMethod]
        public void TestOnStepSyncSelectGroup_SetActionCommandsAsync()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock environment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                        // Create mock
                        var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);

                        // Managers == null, fake AllUsers != null, SelectedGroup.IsNewGroup == true
                        Mock.Arrange(() => viewModel.SelectedGroup).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeGroupDetail());
                        viewModel.Managers = null;
                        Mock.Arrange(() => viewModel.AllUsers).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeAllUsers());

                        Mock.SetupStatic(typeof(GroupFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => GroupFunctions.GetGroupAsync(viewModel.SelectedGroup.UserEntityId))
                            .Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeAGroup());

                        // Fake inside func PopulateUsersAndPermissionsForGroupAsync
                        Mock.SetupStatic(typeof(PermissionFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(
                            () =>
                            PermissionFunctions.GetUserEntityOptionPermissionsAsync(
                                viewModel.SelectedGroup.UserEntityId)).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakePermissionOptions());
                        Mock.Arrange(
                            () =>
                            PermissionFunctions.InitialiseFlattennedComponentsAndPermissionsAsync(
                                viewModel.SelectedGroup.UserEntityId,
                                new List<TreeComponent>(),
                                !viewModel.SelectedGroup.IsNewGroup,
                                true)).IgnoreArguments().Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeFlattennedComponents());
                        Mock.Arrange(
                            () =>
                            PermissionFunctions.MakeComponentsInHierarchy(
                                new List<Component>(),
                                new List<TreeComponent>(),
                                true)).IgnoreArguments().Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeNonTaskFlattennedComponents());
                        Mock.Arrange(
                            () => GroupFunctions.GetGroupUsers(viewModel.SelectedGroup.UserEntityId, viewModel.AllUsers))
                            .Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeNonTaskUserDetails());

                        Mock.NonPublic.Arrange(viewModel, "SetSystemOptionPermissionValues", new TreeComponent(), true)
                            .IgnoreArguments();

                        // Set isAllTrue and isAllFalse = true
                        Mock.NonPublic.Arrange<bool>(
                            viewModel,
                            "CheckAllSystemOptionPermission",
                            new TreeComponent(),
                            true).IgnoreArguments().Returns(true);

                        // Pass condition CanEdit == true
                        Mock.Arrange(() => viewModel.CanEdit).Returns(true);

                        await viewModel.OnStepAsync("SelectGroup");

                        // Expected viewModel.ActionCommands.Count == 3 and viewModel.ActionCommands Parameter == "Add, Edit, Copy"
                        Assert.AreEqual(3, viewModel.ActionCommands.Count);
                    });
        }

        /// <summary>
        /// The test edit set action commands async.
        /// Expect ActionCommands null
        /// </summary>
        [TestMethod]
        public void TestOnStepSyncEdit()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock environment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                    // Create mock
                    var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);
                    Mock.Create<ViewModelUseCaseBase>(new Constructor(), Behavior.CallOriginal);

                    // Pass condition CanEdit == true
                    Mock.Arrange(() => viewModel.CanEdit).Returns(true);
                    Mock.Arrange(() => viewModel.SelectedGroup).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeGroupDetail);
                    Mock.Arrange(() => viewModel.AllUsers).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeAllUsers);

                    await viewModel.OnStepAsync("Edit");

                    // Expected viewModel.ActionCommands == null
                    Assert.AreEqual(null, viewModel.ActionCommands);
                });
        }

        /// <summary>
        /// The test add set action commands async.
        /// UserEntityId = -1 and IsChanged is false
        /// </summary>
        [TestMethod]
        public void TestOnStepSyncAdd()
        {
            GeneralThreadAffineContext.Run(
                // ReSharper disable once CSharpWarnings::CS1998
                async () =>
                {
                    // Mock environment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                    var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);
                    Mock.SetupStatic(typeof(GroupFunctions), StaticConstructor.Mocked);
                    Mock.Arrange(() => GroupFunctions.AddNewGroupAsync()).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeNewGroup());
                    
                    Mock.Arrange(() => viewModel.OnStepAsync(ViewGroupsViewModel.EnumSteps.SelectGroup)).Returns(Task.Run(() => { }));
                    Mock.Arrange(() => viewModel.OnStepAsync(ViewGroupsViewModel.EnumSteps.Edit)).Returns(Task.Run(() => { }));

                    string expected = "complete";
                    viewModel.OnStoryBoardChanged += delegate { };
                    Mock.Raise(() => viewModel.OnStoryBoardChanged += null, expected);
                    viewModel.ActiveViewModel = viewModel;
                    
                    await viewModel.OnStepAsync("Add");

                    var actual = viewModel.SelectedGroup;

                    // UserEntityId = -1 and IsChanged is false
                    Assert.AreEqual(-1, actual.UserEntityId);
                    
                    Assert.AreEqual(false, viewModel.IsChanged);
                });
        }

        /// <summary>
        /// The test copy set action commands async.
        /// Expect copy success, IsNewGroup is true
        /// </summary>
        [TestMethod]
        public void TestOnStepSyncCopy()
        {
            GeneralThreadAffineContext.Run(
                // ReSharper disable once CSharpWarnings::CS1998
                async () =>
                {
                    // Mock environment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                    // Create mock
                    var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);

                    viewModel.SelectedGroup = FakeDataSecurity.FakeDataSecurityGroup.FakeNonLxmGroupDetail();
                    Mock.SetupStatic(typeof(GroupFunctions), StaticConstructor.Mocked);
                    Mock.Arrange(() => GroupFunctions.CopyGroupAsync(Arg.IsAny<GroupDetails>())).IgnoreArguments().Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeNewGroup());
                    Mock.Arrange(() => viewModel.OnStepAsync(ViewGroupsViewModel.EnumSteps.SelectGroup)).Returns(Task.Run(() => { }));
                    Mock.Arrange(() => viewModel.OnStepAsync(ViewGroupsViewModel.EnumSteps.Edit)).Returns(Task.Run(() => { }));
                    string expected = "complete";
                    viewModel.OnStoryBoardChanged += delegate { };
                    Mock.Raise(() => viewModel.OnStoryBoardChanged += null, expected);
                    
                    await viewModel.OnStepAsync("Copy");

                    // Expected copy success, IsNewGroup is true
                    Assert.AreEqual(true, viewModel.SelectedGroup.IsNewGroup);
                });
        }

        /// <summary>
        /// The test refresh group set action commands async.
        /// Expect add a group (groups.count = 4)
        /// </summary>
        [TestMethod]
        public void TestOnStepSyncRefreshGroup()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock environment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                    var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);

                    Mock.Arrange(() => viewModel.SelectedGroup).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeGroupDetail());
                    Mock.Arrange(() => viewModel.Groups).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeNonTaskGroupDetails());
                    Mock.Arrange(() => viewModel.Managers).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeManagers1());
                    
                    await viewModel.OnStepAsync("RefreshGroup");

                    // Expected: add a group (groups.count = 4)
                    Assert.AreEqual(4, viewModel.Groups.Count);
                });
        }

        /// <summary>
        /// The test edit group view model validation when SelectedGroup.LXMGroup don't have group name.
        /// Expect HasErrors == true or _Failures == true
        /// </summary>
        [TestMethod]
        public void TestGroupViewModelValidation_FailSelectedGroup()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock environment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                        // Create mock
                        var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);

                        var editGroupViewModel = Mock.Create<EditGroupViewModel>(x =>
                            {
                                x.CallConstructor(() => new EditGroupViewModel(viewModel));
                                x.SetBehavior(Behavior.CallOriginal);
                            });

                        Mock.Arrange(() => editGroupViewModel.SelectedGroup).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeGroupDetail());

                        // Mock.Arrange(() => editGroupViewModel.SelectedGroup).Returns(new GroupDetails()); //editGroupViewModel.HasErrors == false
                        await editGroupViewModel.OnStepAsync("Save");

                        // Expected: pass validator and _Failures == true
                        Assert.AreEqual(true, editGroupViewModel.HasErrors);
                    });
        }

        /// <summary>
        /// The test remove member view model validation when member is manager.
        /// Expect HasErrors == true or _Failures == true
        /// </summary>
        [TestMethod]
        public void TestGroupViewModelValidation_RemoveMember()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock environment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();

                        var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);

                        var editGroupViewModel = Mock.Create<EditGroupViewModel>(x =>
                        {
                            x.CallConstructor(() => new EditGroupViewModel(viewModel));
                            x.SetBehavior(Behavior.CallOriginal);
                        });

                        // fake SelectedMembers have a Manager
                        Mock.Arrange(() => editGroupViewModel.SelectedGroup).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeGroupDetailForValidate());
                        Mock.Arrange(() => editGroupViewModel.SelectedMembers).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeObjLxmUserDetails());

                        // Mock.Arrange(() => editGroupViewModel.SelectedGroup).Returns(new GroupDetails()); //editGroupViewModel.HasErrors == false
                        await editGroupViewModel.OnStepAsync("RemoveMember");

                        // Expected: User is Manager cannot remove and _Failures == true
                        Assert.AreEqual(true, editGroupViewModel.HasErrors);
                    });
        }

        /// <summary>
        /// The test set action commands async enumerable start in edit group view model.
        /// Expect viewModel.ActionCommands have 3 Actions (ResetPermissions, Save, Cancel)
        /// </summary>
        [TestMethod]
        public void TestSetActionCommandsAsync_StartEditGroupViewModel()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock environment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                    // Create mock
                    var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);
                    var editGroupViewModel = Mock.Create<EditGroupViewModel>(x =>
                    {
                        x.CallConstructor(() => new EditGroupViewModel(viewModel));
                        x.SetBehavior(Behavior.CallOriginal);
                    });
                    Mock.Arrange(() => editGroupViewModel.SelectedGroup).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeGroupDetail3);

                    await editGroupViewModel.OnStepAsync("Start");

                    // Expected viewModel.ActionCommands have 3 Actions (ResetPermissions, Save, Cancel)
                    Assert.AreEqual(3, editGroupViewModel.ActionCommands.Count);
                });
        }

        /// <summary>
        /// The test on step async enumerable save in edit group view model.
        /// Expect viewModel.IsCheckedOut == editGroupViewModel.IsCheckedOut
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncSave_EditGroupViewModel()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock environment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                    var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);

                    var editGroupViewModel = Mock.Create<EditGroupViewModel>(x =>
                    {
                        x.CallConstructor(() => new EditGroupViewModel(viewModel));
                        x.SetBehavior(Behavior.CallOriginal);
                    });

                    Mock.Arrange(() => editGroupViewModel.SelectedGroup).Returns(new GroupDetails());
                    Mock.SetupStatic(typeof(GroupFunctions), StaticConstructor.Mocked);
                    Mock.Arrange(() => GroupFunctions.SaveAsync(editGroupViewModel.SelectedGroup)).IgnoreArguments().Returns(Task.Run(() => { }));
                    viewModel.IsCheckedOut = true;
                    editGroupViewModel.IsCheckedOut = false;

                    await editGroupViewModel.OnStepAsync("Save");

                    // Expected: viewModel.IsCheckedOut == editGroupViewModel.IsCheckedOut
                    Assert.AreEqual(false, viewModel.IsCheckedOut);
                });
        }

        /// <summary>
        /// The test on step async enumerable cancel in edit group view model.
        /// Expect UserEntityId = -1 from UserEntityId = 7
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncCancel_EditGroupViewModel()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock environment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                    var viewModel = Mock.Create<ViewGroupsViewModel>(new Constructor(), Behavior.CallOriginal);

                    var editGroupViewModel = Mock.Create<EditGroupViewModel>(x =>
                    {
                        x.CallConstructor(() => new EditGroupViewModel(viewModel));
                        x.SetBehavior(Behavior.CallOriginal);
                    });

                    editGroupViewModel.SelectedGroup = FakeDataSecurity.FakeDataSecurityGroup.FakeGroupDetail2();

                    Mock.SetupStatic(typeof(GroupFunctions), StaticConstructor.Mocked);
                    Mock.Arrange(() => GroupFunctions.AddNewGroupAsync()).Returns(FakeDataSecurity.FakeDataSecurityGroup.FakeNewGroup());

                    Mock.Arrange(() => viewModel.OnStepAsync(ViewGroupsViewModel.EnumSteps.SelectGroup)).Returns(Task.Run(() => { }));
                    Mock.Arrange(() => viewModel.OnStepAsync(ViewGroupsViewModel.EnumSteps.CurrentGroup)).Returns(Task.Run(() => { }));

                    await editGroupViewModel.OnStepAsync("Cancel");

                    var actual = editGroupViewModel.SelectedGroup;

                    // UserEntityId = -1
                    Assert.AreEqual(-1, actual.UserEntityId);
                });
        }

        #endregion

    }
}
