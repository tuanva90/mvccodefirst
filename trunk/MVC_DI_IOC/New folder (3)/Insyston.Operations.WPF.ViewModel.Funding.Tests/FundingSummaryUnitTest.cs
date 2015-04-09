// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FundingSummaryUnitTest.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The funding summary unit test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Funding.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Funding;
    using Insyston.Operations.Business.Funding.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// The funding summary unit test.
    /// </summary>
    [TestClass]
    public class FundingSummaryUnitTest
    {
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

        /// <summary>
        /// The test on step async start_ load summary.
        /// Expect fundingSummary.TrancheSummary is loaded
        /// and ActionCommand
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncStart_LoadSummary()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    FundingSummaryViewModel fundingSummary;

                    // Create mock
                    fundingSummary = Mock.Create<FundingSummaryViewModel>(Behavior.CallOriginal);

                    // Create mock static class
                    Mock.SetupStatic(typeof(FundingFunctions), StaticConstructor.Mocked);

                    // Fake data
                    Mock.Arrange(() => FundingFunctions.GetAllTranchesAsync())
                        .Returns(FakeDataFunding.FakeDataFundingSummary.FakeFundingSummaryList());
                    Mock.Arrange(() => fundingSummary.CanEdit).Returns(true);

                    // Run method
                    await fundingSummary.OnStepAsync("Start");

                    // Test
                    // Test fundingSummary.TrancheSummary can load
                    var actual = fundingSummary.TrancheSummary;

                    // Test Action Command
                    var actual2 = fundingSummary.ActionCommands;

                    Assert.AreEqual(655, actual.First().NodeId);
                    Assert.AreEqual(1, actual2.Count);
                });
        }

        /// <summary>
        /// The test on step async select tranche_ set selected tranche.
        /// Expect action command is add
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncSelectTranche_SetSelectedTranche()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    FundingSummaryViewModel fundingSummary;
                    ExistingTrancheViewModel existingTranche;

                    // Create mock
                    fundingSummary = Mock.Create<FundingSummaryViewModel>(Behavior.CallOriginal);

                    // fundingDetail = Mock.Create<FundingDetailsViewModel>(
                    //    x =>
                    //        {
                    //            x.CallConstructor(() => new FundingDetailsViewModel(fundingSummary));
                    //            x.SetBehavior(Behavior.CallOriginal);
                    //        });
                    existingTranche = Mock.Create<ExistingTrancheViewModel>(
                        x =>
                            {
                                x.CallConstructor(() => new ExistingTrancheViewModel(fundingSummary));
                                x.SetBehavior(Behavior.CallOriginal);
                            });

                    fundingSummary.CanEdit = true;

                    // Create mock static class
                    Mock.SetupStatic(typeof(FundingFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(ExistingTrancheViewModel), StaticConstructor.Mocked);

                    // Fake data
                    // Mock.Arrange(() => fundingDetail.OnStepAsync(FundingDetailsViewModel.EnumStep.Start))
                    //    .IgnoreArguments()
                    //    .Returns(() => Task.Run(() => { }));
                    Mock.Arrange(() => FundingFunctions.GetFundingTrancheAsync(Arg.IsAny<int>())).IgnoreArguments()
                        .Returns(FakeDataFunding.FakeDataFundingSummary.FakeFundingTranche());
                    Mock.Arrange(() => fundingSummary.SelectedTranche)
                        .Returns(await FakeDataFunding.FakeDataFundingSummary.FakeSelectedTranche());

                    Mock.Arrange(() => existingTranche.SetTrancheIdAsync(Arg.IsAny<int>())).Returns(Task.Run(() => { }));

                    // Fake data for private property
                    var priva = new PrivateAccessor(fundingSummary);
                    priva.SetProperty("ExistingTrancheViewModel", existingTranche);

                    Mock.Arrange(
                        () =>
                        fundingSummary.FundingDetails.OnStepAsync(FundingDetailsViewModel.EnumStep.TrancheSelected))
                        .IgnoreArguments()
                        .Returns(() => Task.Run(() => { }));
                    
                    // Run method
                    await fundingSummary.OnStepAsync("SelectTranche");

                    // Test action command
                    var actual = fundingSummary.ActionCommands;

                    Assert.AreEqual(1, actual.Count);
                    Assert.AreEqual("CreateNew", actual.First().Parameter);
                });
        }

        /// <summary>
        /// The test on step async start_ load new tranche summary.
        /// Expect action command is null
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncStart_LoadNewTrancheSummary()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    FundingSummaryViewModel fundingSummary;
                    NewTrancheViewModel newTranche;

                    // Create mock
                    fundingSummary = Mock.Create<FundingSummaryViewModel>(Behavior.CallOriginal);

                    newTranche = Mock.Create<NewTrancheViewModel>(
                        x =>
                        {
                            x.CallConstructor(() => new NewTrancheViewModel(fundingSummary));
                            x.SetBehavior(Behavior.CallOriginal);
                        });

                    // Mock.Arrange(() => FundingFunctions.GetFundingTrancheAsync(Arg.IsAny<int>())).IgnoreArguments()
                    // .Returns(FakeDataFunding.FakeDataFundingSummary.FakeFundingTranche());
                    // Mock.Arrange(() => fundingSummary.SelectedTranche)
                    // .Returns(await FakeDataFunding.FakeDataFundingSummary.FakeSelectedTranche());

                    // Fake data for private property
                    var priva = new PrivateAccessor(fundingSummary);
                    priva.SetProperty("NewTrancheViewModel", newTranche);

                    Mock.Arrange(
                        () =>
                        newTranche.OnStepAsync(FundingDetailsViewModel.EnumStep.Start))
                        .IgnoreArguments()
                        .Returns(() => Task.Run(() => { }));

                    // Run method
                    await fundingSummary.OnStepAsync("CreateNew");

                    // Test action command
                    var actual = fundingSummary.ActionCommands;

                    Assert.IsNull(actual);
                });
        }

        /// <summary>
        /// The test on step async tranche saved_ save in selected tranche.
        /// Expect selected tranche have value 
        /// and actionCommand is null
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncTrancheSaved_SaveInSelectedTranche()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    FundingSummaryViewModel fundingSummary;

                    // Create mock
                    fundingSummary = Mock.Create<FundingSummaryViewModel>(Behavior.CallOriginal);

                    // Fake data
                    Mock.Arrange(() => fundingSummary.FundingDetails.SelectedFunder).Returns(await FakeDataFunding.FakeDataFundingSummary.FakeSelectFunder());
                    fundingSummary.SelectedTranche = new FundingSummary();

                    // Run method
                    await fundingSummary.OnStepAsync("TrancheSaved");

                    // Test
                    // Test selected tranche
                    var actual = fundingSummary.SelectedTranche;

                    // Test actionCommand
                    var actual2 = fundingSummary.ActionCommands;

                    Assert.AreEqual("TranceSave", actual.FunderName);
                    Assert.IsNull(actual2);
                });
        }

        /// <summary>
        /// The test on step async tranche added_ add tranche in existing summary.
        /// Expect fundingDetail.BusyContent is null and IsBusy is false
        /// and TrancheSummary.Count is 2, selectedTranche has value
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncTrancheAdded_AddTrancheInExistingSummary()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    FundingSummaryViewModel fundingSummary;
                    ExistingTrancheViewModel existingTranche;

                    // Create mock
                    fundingSummary = Mock.Create<FundingSummaryViewModel>(Behavior.CallOriginal);
                    Mock.SetupStatic(typeof(FundingFunctions), StaticConstructor.Mocked);

                    existingTranche = Mock.Create<ExistingTrancheViewModel>(
                        x =>
                        {
                            x.CallConstructor(() => new ExistingTrancheViewModel(fundingSummary));
                            x.SetBehavior(Behavior.CallOriginal);
                        });

                    // Fake data for private property
                    var priva = new PrivateAccessor(fundingSummary);
                    priva.SetProperty("ExistingTrancheViewModel", existingTranche);

                    // Fake data
                    fundingSummary.AddedTrancheId = 1;
                    Mock.Arrange(() => fundingSummary.FundingDetails)
                        .Returns(new FundingDetailsViewModel(fundingSummary));
                    Mock.Arrange(() => existingTranche.SetTrancheIdAsync(Arg.IsAny<int>())).Returns(Task.Run(() => { }));
                    Mock.Arrange(() => FundingFunctions.GetAllTranchesAsync())
                        .Returns(FakeDataFunding.FakeDataFundingSummary.FakeFundingSummaryList());

                    // Run method
                    await fundingSummary.OnStepAsync("TrancheAdded");

                    // Test
                    // Test fundingDetail.BusyContent is null and IsBusy is false
                    var actual = fundingSummary.FundingDetails;

                    // Test value TrancheSummary
                    var actual2 = fundingSummary.TrancheSummary;

                    // Test SelectedTranche has value
                    var actual3 = fundingSummary.SelectedTranche;

                    Assert.AreEqual(string.Empty, actual.BusyContent);
                    Assert.IsFalse(actual.IsBusy);
                    Assert.AreEqual(2, actual2.Count);
                    Assert.AreEqual(1, actual3.TrancheId);
                });
        }
    }
}
