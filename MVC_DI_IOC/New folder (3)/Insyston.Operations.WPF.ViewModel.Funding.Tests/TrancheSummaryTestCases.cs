using System;
using System.Collections.Generic;
using System.Linq;
using Insyston.Operations.Business.Common;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Business.Funding;
using Insyston.Operations.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System.Threading.Tasks;
using System.Windows;
using Insyston.Operations.Business.Funding.Model;

namespace Insyston.Operations.WPF.ViewModel.Funding.Tests
{
    [TestClass]
    public class TrancheSummaryTestCases
    {
        [TestMethod]
        public async Task SelectATrancheFromList()
        { 
            var fundings = Helper.FakeFundingSummary();
            Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();
            FundingSummaryViewModel summaryViewModel = Mock.Create<FundingSummaryViewModel>(Behavior.CallOriginal);
            ExistingTrancheViewModel existingViewModel = Mock.Create<ExistingTrancheViewModel>(Behavior.CallOriginal, summaryViewModel);

            Mock.NonPublic.Arrange<ExistingTrancheViewModel>(summaryViewModel, "ExistingTrancheViewModel").Returns(existingViewModel);
            Mock.Arrange(() => existingViewModel.AllFundingStatuses).ReturnsCollection(Helper.FakeFundingStatuses());
            Mock.Arrange(() => existingViewModel.AllFinanceTypes).ReturnsCollection(Helper.FakeFinanceTypes());
            Mock.Arrange(() => existingViewModel.AllSuppliers).ReturnsCollection(Helper.FakeSuppliers());           
            Mock.Arrange(() => existingViewModel.AllInternalCompanies).ReturnsCollection(Helper.FakeInternalCompanies());
            Mock.Arrange(() => existingViewModel.AllInstalmentType).ReturnsCollection(Helper.FakeInstallmentTypes());
            Mock.Arrange(() => existingViewModel.AllFrequencies).ReturnsCollection(Helper.FakeFrequencies());
            Mock.Arrange(() => existingViewModel.AllFunders).ReturnsCollection(Helper.FakeEntityRelations());
            Mock.Arrange(() => FundingFunctions.GetFundingTrancheAsync(1)).Returns(new Task<FundingTranche>(()=>Helper.FakeTrancheProfile()));
            Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync()).Returns(new Task<IEnumerable<DirectDebitProfile>>(()=>new List<DirectDebitProfile>()));
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.TrancheStatus)).Returns(new Task<List<SystemConstant>>(()=> Helper.FakeTrancheStatuses()));
            Mock.Arrange(() => FundingFunctions.GetDefaultTrancheFiltersByFilterTypeAndTrancheIdAsync(FilterType.FundingStatus, existingViewModel.SelectedTrancheProfile.TrancheId)).Returns(new Task<List<int>>(() => new List<int> { 485 }));

            //Mock.SetupStatic<FundingFunctions>(Behavior.Loose);
            Mock.Arrange(() => FundingFunctions.GetAllTranchesAsync()).IgnoreArguments().Returns(new Task<List<FundingSummary>>(() => fundings));
            Mock.Arrange(() => FundingFunctions.GetAllTrancheContractsAsync(DateTime.Today, 1, TrancheStatus.Pending)).IgnoreArguments().Returns(new Task<List<TrancheContractSummary>>(() => Helper.FakeTrancheContracts()));
            Mock.Arrange(() => summaryViewModel.SelectedTranche).Returns(fundings.First());

            await summaryViewModel.OnStepAsync(FundingSummaryViewModel.EnumStep.SelectTranche);

            Assert.IsTrue(summaryViewModel.FundingDetails.SelectedTrancheProfile.TrancheId == 1);
        }
    }
}
