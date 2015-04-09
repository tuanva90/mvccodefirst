// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignmentUnitTest.cs" company="TMA Solutions">
//   2014 
// </copyright>
// <summary>
//   The assignment unit test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.VModel.Collections.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Collections;
    using Insyston.Operations.Business.Collections.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Collections;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// The assignment unit test.
    /// </summary>
    [TestClass]
    public class AssignmentUnitTest
    {
        /// <summary>
        /// The test Load Data On Screen.
        /// </summary>
        [TestMethod]
        public void TestLoadDataOnScreen()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for the test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                    // Create mock
                    var assignmentMock = Mock.Create<CollectionsAssignmentViewModel>(Behavior.CallOriginal);

                    // Arrange
                    Mock.Arrange(() => QueueAssignmentFunctions.GetAssignmentDetails(Arg.AnyInt, Arg.AnyInt))
                        .Returns(FakeData.FakeAssignmentDetails);
                    Mock.Arrange(() => assignmentMock.SelectedQueue).Returns(FakeData.FakeSelectedQueue);
                    Mock.Arrange(() => QueueAssignmentFunctions.GetListAssignees()).Returns(FakeData.FakeListAssignees);
                    Mock.Arrange(() => QueueAssignmentFunctions.GetAssignmentContacts(Arg.AnyInt, Arg.AnyInt))
                        .Returns(FakeData.FakeAssignmentContacts);
                    Mock.Arrange(
                        () =>
                        QueueAssignmentFunctions.GetCollectionHistory(Arg.AnyInt, Arg.IsNull<int>(), Arg.AnyInt))
                        .Returns(FakeData.FakeCollectionHistory);
                    Mock.Arrange(() => QueueAssignmentFunctions.GetListNoteTask(Arg.IsNull<int>(), Arg.AnyInt))
                        .Returns(FakeData.FakeListNoteTask);
                    Mock.NonPublic.Arrange<Task>(
                        assignmentMock,
                        "LoadFinancialAndContractSummary",
                        ArgExpr.IsNull<Action>(),
                        ArgExpr.IsAny<bool?>()).Returns(Task.Run(() => { }));
                    Mock.Arrange(() => assignmentMock.CurrentEntityId).Returns(1);

                    assignmentMock.IsNoCheckValidQueue = true;
                    await assignmentMock.LoadDataOnScreen(null);

                    // Check program load data correct
                    Assert.IsFalse(assignmentMock.IsNoCheckValidQueue);
                    Assert.IsTrue(assignmentMock.ListContacts.Count == 2);
                    Assert.IsTrue(assignmentMock.ListContractHistory.Count == 2);
                    Assert.IsTrue(assignmentMock.ListNoteTask.Count == 2);
                    Assert.IsTrue(assignmentMock.ListAssignees.Count == 3);
                });
        }

        /// <summary>
        /// The test on step async start.
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncAtStart()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for the test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();
                    var assignmentMock = Mock.Create<CollectionsAssignmentViewModel>(Behavior.CallOriginal);

                    Mock.NonPublic.Arrange<Task>(assignmentMock, "PopulateAllCollectionsQueuesAssignmentAsync")
                        .Returns(Task.Run(() => { }));

                    await assignmentMock.OnStepAsync(CollectionsAssignmentViewModel.EnumSteps.Start);

                    Assert.IsTrue(assignmentMock.DefaultTerm == 0);
                    Assert.IsTrue(assignmentMock.ActionCommands != null);
                    Assert.IsTrue(
                        assignmentMock.ActionCommands.Count == 2
                        && assignmentMock.ActionCommands.First().Parameter
                        == CollectionsAssignmentViewModel.EnumSteps.Refresh.ToString()
                        && assignmentMock.ActionCommands[1].Parameter
                        == CollectionsAssignmentViewModel.EnumSteps.AssignmentFilter.ToString());
                });
        }

        /// <summary>
        /// The test on step async select queue.
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncAtSelectQueue()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();
                    var assignmentMock = Mock.Create<CollectionsAssignmentViewModel>(Behavior.CallOriginal);

                    Mock.Arrange(() => assignmentMock.LoadDataOnScreen(Arg.IsAny<Action>()))
                        .IgnoreArguments()
                        .Returns(Task.Run(() => { }));
                    Mock.Arrange(() => assignmentMock.SelectedQueue).Returns(FakeData.FakeSelectedQueue);
                    Mock.Arrange(() => assignmentMock.ActionCommands).Returns(FakeData.FakeActionCommand);
                    Mock.Arrange(() => assignmentMock.OnNotifyFinancialSummary()).DoNothing();
                    Mock.NonPublic.Arrange<QueueAssignmentDetailsModel>(assignmentMock, "QueueDetail")
                        .Returns(FakeData.FakeQueueDetail());

                    await assignmentMock.OnStepAsync(CollectionsAssignmentViewModel.EnumSteps.SelectQueue);

                    Assert.IsTrue(assignmentMock.CurrentClientNodeID == 1);
                    Assert.IsTrue(assignmentMock.IsQueueSelected);
                });
        }

        /// <summary>
        /// The test on step sync at previous.
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncAtPrevious()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();
                    var assignmentMock = Mock.Create<CollectionsAssignmentViewModel>(Behavior.CallOriginal);

                    var fakeFilteredItems = FakeData.FakeFilteredItems();
                    var fakeActionCommand = FakeData.FakeActionCommand();

                    Mock.Arrange(() => assignmentMock.FilteredItems).Returns(fakeFilteredItems);
                    Mock.Arrange(() => assignmentMock.SelectedQueue).Returns(fakeFilteredItems[1]);
                    Mock.NonPublic.Arrange<Task>(assignmentMock, "PreviousAssignDetail", Arg.AnyInt)
                        .Returns(Task.Run(() => { }));
                    Mock.Arrange(
                        () => assignmentMock.OnStepAsync(CollectionsAssignmentViewModel.EnumSteps.SelectQueue))
                        .Returns(Task.Run(() => { }));

                    Mock.ArrangeSet(
                        () => assignmentMock.ActionCommands.Add(fakeActionCommand[0]));
                    Mock.ArrangeSet(
                        () =>
                        assignmentMock.ActionCommands.Add(fakeActionCommand[1]));
                    Mock.ArrangeSet(
                        () =>
                        assignmentMock.ActionCommands.Add(fakeActionCommand[2]));

                    await assignmentMock.OnStepAsync(CollectionsAssignmentViewModel.EnumSteps.Previous);

                    // ActionCommand cant contain the Previous Command, cause we standing at the first item.
                    Assert.IsFalse(assignmentMock.ActionCommands.Contains(fakeActionCommand[1]));
                });
        }

        /// <summary>
        /// The test on step sync at next.
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncAtNext()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();
                    var assignmentMock = Mock.Create<CollectionsAssignmentViewModel>(Behavior.CallOriginal);

                    var fakeFilteredItems = FakeData.FakeFilteredItems();
                    var fakeActionCommand = FakeData.FakeActionCommand();

                    Mock.Arrange(() => assignmentMock.FilteredItems).Returns(fakeFilteredItems);
                    Mock.Arrange(() => assignmentMock.SelectedQueue).Returns(fakeFilteredItems[1]);
                    Mock.NonPublic.Arrange<Task>(assignmentMock, "NextAssignDetail", Arg.AnyInt).IgnoreArguments()
                        .Returns(Task.Run(() => { }));
                    Mock.Arrange(
                        () => assignmentMock.OnStepAsync(CollectionsAssignmentViewModel.EnumSteps.SelectQueue))
                        .Returns(Task.Run(() => { }));

                    Mock.ArrangeSet(() => assignmentMock.ActionCommands.Add(fakeActionCommand[0]));
                    Mock.ArrangeSet(() => assignmentMock.ActionCommands.Add(fakeActionCommand[1]));
                    Mock.ArrangeSet(() => assignmentMock.ActionCommands.Add(fakeActionCommand[2]));

                    await assignmentMock.OnStepAsync(CollectionsAssignmentViewModel.EnumSteps.Next);

                    // ActionCommand can't contain the Next Command, cause we standing at the last item.
                    Assert.IsFalse(assignmentMock.ActionCommands.Contains(fakeActionCommand[2]));
                });
        }

        /// <summary>
        /// The test load history data.
        /// </summary>
        [TestMethod]
        public void TestLoadHistoryData()
        {
            Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

            var assignmentMock = Mock.Create<CollectionsAssignmentViewModel>(Behavior.CallOriginal);

            Mock.Arrange(
                () =>
                QueueAssignmentFunctions.GetCollectionHistory(
                    Arg.IsAny<int>(),
                    Arg.IsNull<int>(),
                    Arg.IsAny<int>())).IgnoreArguments().Returns(FakeData.FakeCollectionHistory);
            Mock.Arrange(() => assignmentMock.SelectedQueue).Returns(FakeData.FakeSelectedQueue);
            Mock.ArrangeSet(
                () => assignmentMock.ListContractHistory = new ObservableCollection<CollectionHistoryModel>());
            Mock.Arrange(() => assignmentMock.CurrentEntityId).Returns(1);

            var loadHistoryData = new PrivateAccessor(assignmentMock);
            loadHistoryData.CallMethod("LoadHistoryData");

            Assert.AreEqual(2, assignmentMock.ListContractHistory.Count);
        }

        /// <summary>
        /// The test load note task data.
        /// </summary>
        [TestMethod]
        public void TestLoadNoteTaskData()
        {
            Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

            var assignmentMock = Mock.Create<CollectionsAssignmentViewModel>(Behavior.CallOriginal);

            Mock.Arrange(() => QueueAssignmentFunctions.GetListNoteTask(Arg.IsNull<int>(), Arg.IsAny<int>()))
                .Returns(FakeData.FakeListNoteTask);
            Mock.Arrange(() => assignmentMock.CurrentEntityId).Returns(1);

            var loadHistoryData = new PrivateAccessor(assignmentMock);
            loadHistoryData.CallMethod("LoadNoteTaskData");

            Assert.AreEqual(2, assignmentMock.ListNoteTask.Count);
        }

        /// <summary>
        /// The test load financial summary.
        /// </summary>
        [TestMethod]
        public void TestLoadFinancialAndContractSummary()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    var assignmentMock = Mock.Create<CollectionsAssignmentViewModel>(Behavior.CallOriginal);

                    // Mock.Arrange(() => assignmentMock.SelectedQueue).Returns(FakeData.FakeSelectedQueue);
                    Mock.Arrange<Task>(
                        () =>
                        QueueAssignmentFunctions.GetTotalInvestmentBalance(
                            Arg.IsAny<CollectionAssignmentModel>(),
                            Arg.IsAny<QueueAssignmentDetailsModel>(),
                            true)).IgnoreArguments().Returns(Task.Run(() => 1m));
                    Mock.Arrange<Task>(
                        () =>
                        QueueAssignmentFunctions.GetTotalArrears(
                            Arg.IsNull<CollectionSetting>(),
                            Arg.IsAny<CollectionAssignmentModel>(),
                            Arg.IsAny<QueueAssignmentDetailsModel>(),
                            Arg.IsAny<bool>())).IgnoreArguments().Returns(Task.Run(() => 2m));
                    Mock.Arrange<Task>(
                        () =>
                        QueueAssignmentFunctions.GetReceivableBalance(
                            Arg.IsAny<CollectionAssignmentModel>(),
                            Arg.IsAny<QueueAssignmentDetailsModel>(),
                            Arg.IsAny<bool>())).IgnoreArguments().Returns(Task.Run(() => 3m));
                    Mock.Arrange<Task>(
                        () =>
                        QueueAssignmentFunctions.GetCountLiveContracts(
                            Arg.IsAny<CollectionAssignmentModel>(),
                            Arg.IsAny<QueueAssignmentDetailsModel>(),
                            Arg.IsAny<bool>())).IgnoreArguments().Returns(Task.Run(() => 4));
                    Mock.Arrange<Task>(
                        () =>
                        QueueAssignmentFunctions.GetCountClosedContracts(
                            Arg.IsAny<CollectionAssignmentModel>(),
                            Arg.IsAny<QueueAssignmentDetailsModel>(),
                            Arg.IsAny<bool>())).IgnoreArguments().Returns(Task.Run(() => 5));
                    Mock.Arrange<Task>(
                        () =>
                        QueueAssignmentFunctions.GetCountArrearContracts(
                            Arg.IsAny<CollectionAssignmentModel>(),
                            Arg.IsAny<QueueAssignmentDetailsModel>(),
                            Arg.IsAny<bool>())).IgnoreArguments().Returns(Task.Run(() => 6));
                    Mock.Arrange<Task>(
                        () =>
                        QueueAssignmentFunctions.GetContractSummary(
                            Arg.IsAny<CollectionAssignmentModel>(),
                            Arg.IsAny<QueueAssignmentDetailsModel>(),
                            true)).IgnoreArguments().Returns(FakeData.FakeGetContractSummary);

                    Mock.Arrange<Task>(() => QueueAssignmentFunctions.GetCollectionSetting())
                        .Returns(Task.Run(() => (CollectionSetting)null));

                    var loadFinancialSummary = new PrivateAccessor(assignmentMock);
                    await Task.WhenAll(loadFinancialSummary.CallMethod("LoadFinancialAndContractSummary", null, false) as Task);

                    Assert.AreEqual(1, assignmentMock.InvestmentBalance);
                    Assert.AreEqual(2, assignmentMock.TotalArrears);
                    Assert.AreEqual(3, assignmentMock.ReceivableBalance);
                    Assert.AreEqual(4, assignmentMock.LiveContracts);
                    Assert.AreEqual(5, assignmentMock.ClosedContracts);
                    Assert.AreEqual(6, assignmentMock.ArrearsContracts);
                    Assert.AreEqual(2, assignmentMock.ListContractSummary.Count);
                });
        }
    }
}
