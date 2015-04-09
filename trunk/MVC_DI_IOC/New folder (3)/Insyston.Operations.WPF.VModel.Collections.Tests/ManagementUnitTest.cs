// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementUnitTest.cs" company="TMA Solution">
//   Unit Test
// </copyright>
// <summary>
//   Summary description for ManagementUnitTest
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.VModel.Collections.Tests
{
    using System.Threading.Tasks;

    using Insyston.Operations.Business.Collections;
    using Insyston.Operations.Business.Collections.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Collections;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    using Application = System.Windows.Application;

    /// <summary>
    /// The management unit test.
    /// </summary>
    [TestClass]
    public class ManagementUnitTest
    {
        /// <summary>
        /// The test function OnStepAsync Enumerable step select queue.
        /// </summary>
        [TestMethod]
        public void TestFunctionOnstepAsyncEnumStepSelectQueue()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments().DoNothing();
                        var viewModel = Mock.Create<CollectionsManagementViewModel>(Behavior.CallOriginal);
                        Mock.Arrange(() => viewModel.SelectedQueue.IsNewQueue).Returns(true);
                        Mock.SetupStatic(typeof(QueueManagmentFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => QueueManagmentFunctions.GetCollectionQueueAsync(-1))
                            .Returns(FakeData.FakeTaskCollectionQueue);
                        Mock.Arrange(() => viewModel.SelectedQueue.QueueDetailId).Returns(-1);
                        Mock.Arrange(() => viewModel.SelectedQueue.CollectionQueue.ClientFinancialsTypeID).Returns(-1);
                        Mock.Arrange(() => viewModel.LoadAllDetailOnView(Arg.IsAny<int>())).DoNothing();
                        Mock.Arrange(() => viewModel.ClientFinancials).Returns(FakeData.FakeDropdownLists);
                        Mock.SetupStatic(typeof(CollectionsQueueCollectorsFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => CollectionsQueueCollectorsFunctions.QueueList())
                            .Returns(FakeData.FakeListCollectionQueues);
                        Mock.NonPublic.Arrange(viewModel, "SetActionCommandsAsync").DoNothing();
                        Mock.Arrange(() => viewModel.IsCheckedOut).Returns(true);
                        await viewModel.OnStepAsync("SelectQueue");
                        Assert.IsNotNull(viewModel.ActiveViewModel);
                    });
        }
       
        /// <summary>
        /// The test get client financial async_enumerable steps start.
        /// </summary>
        [TestMethod]
        public void TestGetClientFinancialAsync_OnstepAsync_EnumStepsStart()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();
                        var viewModel = Mock.Create<CollectionsManagementViewModel>(Behavior.CallOriginal);
                        viewModel.ClientFinancials = FakeData.FakeDropdownLists();
                        Mock.SetupStatic(typeof(QueueManagmentFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => QueueManagmentFunctions.GetClientFinancialsAsync())
                            .Returns(FakeData.FakeSystemConstant());
                        var inst = new PrivateAccessor(viewModel);
                        await Task.WhenAll(inst.CallMethod("GetClientFinancialAsync") as Task);
                        Assert.AreEqual(3, viewModel.ClientFinancials.Count);
                    });
        }

        /// <summary>
        /// The test populate all collections async_enumerable step start.
        /// </summary>
        [TestMethod]
        public void TestPopulateAllCollectionsQueues_AsyncOnstepAsync_EnumStepStart()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();
                        Mock.SetupStatic(typeof(QueueManagmentFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => QueueManagmentFunctions.GetQueueDetailsAsync())
                            .Returns(FakeData.FakeListQueueDetailModel());
                        var viewModel = Mock.Create<CollectionsManagementViewModel>(Behavior.CallOriginal);
                        var inst = new PrivateAccessor(viewModel);
                        await Task.WhenAll(inst.CallMethod("PopulateAllCollectionsQueuesAsync") as Task);
                        Assert.AreEqual(2, viewModel.AllQueueManagementDetails.Count);
                    });
        }

        /// <summary>
        /// The test load all detail on view.
        /// </summary>
        [TestMethod]
        public void TestLoadAllDetailOnView()
        {
               Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                   .IgnoreArguments()
                   .DoNothing();
               var viewModel = Mock.Create<CollectionsManagementViewModel>(Behavior.CallOriginal);
               Mock.SetupStatic(typeof(QueueManagmentFunctions), StaticConstructor.Mocked);
               Mock.Arrange(() => QueueManagmentFunctions.GetQueueFilterOperators())
                   .Returns(FakeData.FakeOperator());
               Mock.Arrange(() => QueueManagmentFunctions.GetAllQueueAssigneesAsync(Arg.IsAny<int>()))
                   .Returns(FakeData.FakeSelectList());
               Mock.Arrange(() => QueueManagmentFunctions.GetAllInternalCompaniesAsync(Arg.IsAny<int>()))
                   .Returns(FakeData.FakeSelectList());
               Mock.Arrange(() => QueueManagmentFunctions.GetAllFinancierAsync(Arg.IsAny<int>()))
                   .Returns(FakeData.FakeSelectList());
               Mock.Arrange(() => QueueManagmentFunctions.GetAllWorkgroupsAsync(Arg.IsAny<int>()))
                   .Returns(FakeData.FakeSelectList());
               Mock.Arrange(() => QueueManagmentFunctions.GetAllStatesAsync(Arg.IsAny<int>())).Returns(FakeData.FakeSelectList());
               Mock.Arrange(() => QueueManagmentFunctions.GetCollectionFilterByQueueID(Arg.IsAny<int>()))
                 .Returns(FakeData.FakeCollections());
               Mock.Arrange(() => viewModel.IsChanged).Returns(true);
               Mock.Arrange(() => viewModel.SelectedQueue.IsNewQueue).Returns(true);
               Mock.NonPublic.Arrange(viewModel, "LoadQueueFilter", ArgExpr.IsNull<int>()).DoNothing();
               Mock.NonPublic.Arrange(viewModel, "LoadQueueFilterString", ArgExpr.IsNull<int>()).DoNothing();
               viewModel.LoadAllDetailOnView(Arg.IsAny<int>());
               Assert.AreEqual("Assignee", viewModel.ListAssignee.Title);
               Assert.AreEqual("Internal Company", viewModel.ListCompany.Title);
               Assert.AreEqual("Financier", viewModel.ListFinancier.Title);
               Assert.AreEqual("Workgroup", viewModel.ListWorkgroup.Title);
               Assert.AreEqual("State", viewModel.ListState.Title);
        }
        
        /// <summary>
        /// The test function instance.
        /// </summary>
        [TestMethod]
        public void TestFunctionInstance()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();
                        var viewModel = Mock.Create<CollectionsManagementViewModel>(Behavior.CallOriginal);
                        Mock.SetupStatic(typeof(CollectionsManagementViewModel), StaticConstructor.Mocked);
                        Mock.Arrange(() => CollectionsManagementViewModel.GetAvailibleCollector())
                            .IgnoreArguments()
                            .Returns(FakeData.FakeCollectors());
                        var inst = new PrivateAccessor(viewModel);
                        await Task.WhenAll(inst.CallMethod("Instance") as Task);
                        Assert.AreEqual(2, viewModel.AvailableCollectorList.Count);
                    });
        }

        /// <summary>
        /// The test function OnSteps async_enumerable steps edit.
        /// </summary>
        [TestMethod]
        public void TestFunctionOnstepAsyncEnumStepsEdit()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments().DoNothing();
                        var viewModel = Mock.Create<CollectionsManagementViewModel>(Behavior.CallOriginal);
                        Mock.Arrange(() => viewModel.SelectedQueue).Returns(FakeData.FakeQueueDetailModel());
                        await viewModel.OnStepAsync("Edit");
                        Assert.IsNotNull(viewModel.Edit.SelectedQueue);
                    });
        }

        /// <summary>
        /// The test on step async_enumerable step add.
        /// </summary>
        [TestMethod]
        public void TestOnStepAsync_EnumStepAdd()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();
                        var viewModel = Mock.Create<CollectionsManagementViewModel>(Behavior.CallOriginal);
                        Mock.SetupStatic(typeof(QueueManagmentFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => QueueManagmentFunctions.AddNewQueueAsync(Arg.AnyInt)).Returns(FakeData.FakeTaskQueueDetailMode());
                        Mock.Arrange(() => viewModel.ClientFinancials).Returns(FakeData.FakeDropdownLists());
                        Mock.Arrange(() => viewModel.OnStepAsync("Edit")).Returns(Task.Run(() => { }));
                        Mock.Arrange(() => viewModel.OnStepAsync(CollectionsManagementViewModel.EnumSteps.SelectQueue)).Returns(Task.Run(() => { }));
                        Mock.NonPublic.Arrange<Task>(viewModel, "UnLockAsync").Returns(Task.Run(() => { }));
                        await viewModel.OnStepAsync("Add");
                        Assert.IsNotNull(viewModel.SelectedQueue);
                    });
        }

        /// <summary>
        /// The test on step async_enumerable step process.
        /// </summary>
        [TestMethod]
        public void TestOnStepAsync_EnumStepProcess()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                           .IgnoreArguments()
                           .DoNothing();
                        var viewModel = Mock.Create<CollectionsManagementViewModel>(Behavior.CallOriginal);
                        Mock.SetupStatic(typeof(QueueManagmentFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => QueueManagmentFunctions.GetQueueDetailsAsync())
                            .Returns(FakeData.FakeListQueueDetailModel());
                        await viewModel.OnStepAsync("Process");
                        Assert.IsTrue(viewModel.IsBusy);
                    });
        }

        /// <summary>
        /// The test on step async_enumerable step copy.
        /// </summary>
        [TestMethod]
        public void TestOnStepAsync_EnumStepCopy()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                          .IgnoreArguments()
                          .DoNothing();
                        var viewModel = Mock.Create<CollectionsManagementViewModel>(Behavior.CallOriginal);
                        Mock.Create<CollectionQueue>(Behavior.CallOriginal);
                        Mock.Arrange(() => viewModel.SelectedQueue).Returns(FakeData.FakeQueueDetailModel());
                    
                        Mock.SetupStatic(typeof(QueueManagmentFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => QueueManagmentFunctions.CopyQueueAsync(Arg.IsAny<QueueDetailsModel>()))
                            .IgnoreArguments()
                            .Returns(FakeData.FakeTaskQueueDetailMode());
                        Mock.Arrange(() => viewModel.OnStepAsync(CollectionsManagementViewModel.EnumSteps.Edit))
                            .Returns(Task.Run(() => { }));
                        Mock.NonPublic.Arrange<Task>(viewModel, "UnLockAsync").Returns(Task.Run(() => { }));
                        Mock.Arrange(() => viewModel.OnStepAsync(CollectionsManagementViewModel.EnumSteps.SelectQueue))
                            .Returns(Task.Run(() => { }));
                        await viewModel.OnStepAsync(CollectionsManagementViewModel.EnumSteps.Copy);
                        Assert.IsNotNull(viewModel.SelectedQueue);
                    });
        }
    }
}
