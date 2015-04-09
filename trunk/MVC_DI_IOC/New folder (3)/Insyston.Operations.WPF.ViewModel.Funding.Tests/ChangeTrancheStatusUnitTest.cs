using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Insyston.Operations.Business.Common;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Business.Funding;
using Insyston.Operations.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModel.Funding.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ChangeTrancheStatusUnitTest
    {
        public ChangeTrancheStatusUnitTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void ToConfirmedTest()
        {
            FundingDetailsViewModel viewModel = Mock.Create<FundingDetailsViewModel>();
            Entities entities = Mock.Create<Entities>();

            Mock.Arrange(() => entities.vwEntityRelations).IgnoreInstance().ReturnsCollection(Helper.FakeEntityRelations());
            Mock.Arrange(() => viewModel.OnStepAsync("Start"));
        }

        [TestMethod]
        public async Task TestTrancheContractsBackground()
        {
            bool isAllValid;

            ExistingTrancheViewModel viewModel;

            viewModel = Mock.Create<ExistingTrancheViewModel>(Behavior.CallOriginal, new FundingSummaryViewModel());
            viewModel.ActiveViewModel = viewModel;

            Mock.Arrange(() => FundingFunctions.GetAllTrancheContractsAsync(DateTime.Today, 1, TrancheStatus.Pending)).IgnoreArguments().ReturnsCollection(Helper.FakeTrancheContracts());

            Mock.Arrange(() => viewModel.AllFundingStatuses).ReturnsCollection(Helper.FakeFundingStatuses());
            Mock.Arrange(() => viewModel.AllFinanceTypes).ReturnsCollection(Helper.FakeFinanceTypes());
            Mock.Arrange(() => viewModel.AllSuppliers).ReturnsCollection(Helper.FakeSuppliers());
            Mock.Arrange(() => viewModel.AllInternalCompanies).ReturnsCollection(Helper.FakeInternalCompanies());
            Mock.Arrange(() => viewModel.AllInstalmentType).ReturnsCollection(Helper.FakeInstallmentTypes());
            Mock.Arrange(() => viewModel.AllFrequencies).ReturnsCollection(Helper.FakeFrequencies());
            Mock.Arrange(() => viewModel.AllFunders).ReturnsCollection(Helper.FakeEntityRelations());
            Mock.Arrange(() => viewModel.SelectedTrancheProfile).Returns(Helper.FakeTrancheProfile());
            Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync()).ReturnsCollection<DirectDebitProfile>(new List<DirectDebitProfile>());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.TrancheStatus)).ReturnsCollection<SystemConstant>(Helper.FakeTrancheStatuses());
            Mock.Arrange(() => FundingFunctions.GetDefaultTrancheFiltersByFilterTypeAndTrancheIdAsync(FilterType.FundingStatus, viewModel.SelectedTrancheProfile.TrancheId)).Returns((() => Task<List<int>>.Factory.StartNew(() => new List<int> { 485 })));
            await viewModel.OnStepAsync("TrancheSelected");

            isAllValid = viewModel.IncludedInTrancheContracts.All(item => item.IsValid == true);
            await viewModel.SetTrancheIdAsync(viewModel.SelectedTrancheProfile.TrancheId);
            viewModel.IncludedInTrancheContracts.First().FundingStatus = FundingStatus.Confirmed;
            await viewModel.OnStepAsync("Save");

            Assert.IsTrue(isAllValid && viewModel.IncludedInTrancheContracts.First().IsValid == false);
        }
    }
}