using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Insyston.Operations.Business.Common;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using Insyston.Operations.Business.Funding.Model;

namespace Insyston.Operations.WPF.ViewModel.Funding.Tests
{
    [TestClass]
    public class FundingDetailsViewModelTestCases
    {
        [TestMethod]
        public async Task OnStep_Start_Populate_Records()
        {
            var viewModel = Mock.Create<FundingDetailsViewModel>(Behavior.CallOriginal, new FundingSummaryViewModel());

            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.TrancheStatus)).ReturnsCollection(Helper.FakeSystemConstants()).InSequence();
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.TrancheStatus)).ReturnsCollection(new List<SystemConstant>()).InSequence();

            Mock.Arrange(() => FinanceTypeFunctions.GetAllFinanceTypesAsync()).ReturnsCollection(Helper.FakeRawFinanceTypes());

            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.Frequency)).ReturnsCollection(Helper.FakeSystemConstants()).InSequence();
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.Frequency)).ReturnsCollection(new List<SystemConstant>()).InSequence();

            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.InstallmentType)).ReturnsCollection(Helper.FakeSystemConstants()).InSequence();
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.InstallmentType)).ReturnsCollection(new List<SystemConstant>()).InSequence();

            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.FundingStatus)).ReturnsCollection(Helper.FakeSystemConstants()).InSequence();
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.FundingStatus)).ReturnsCollection(new List<SystemConstant>()).InSequence();

            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.InternalCompany)).ReturnsCollection(Helper.FakeRawInternalCompanies()).InSequence();
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.InternalCompany)).ReturnsCollection(new List<vwEntityRelation>()).InSequence();

            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Supplier)).ReturnsCollection(Helper.FakeRawSuppliers()).InSequence();
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Supplier)).ReturnsCollection(new List<vwEntityRelation>()).InSequence();

            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Funder)).ReturnsCollection((Helper.FakeEntityRelations())).InSequence();
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Funder)).ReturnsCollection(new List<vwEntityRelation>()).InSequence();

            Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync()).ReturnsCollection<DirectDebitProfile>(Helper.FakeDirectDebitProfiles()).InSequence();
            Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync()).ReturnsCollection<DirectDebitProfile>(new List<DirectDebitProfile>()).InSequence();

            await viewModel.OnStepAsync("Start");
            await viewModel.OnStepAsync("Start");

            Assert.AreEqual(viewModel.AllFundingStatuses.Where(f => f.Id == -1).Count(), 1);
            Assert.AreEqual(viewModel.AllInstalmentType.Where(f => f.Id == 1).Count(), 1);
            Assert.AreEqual(viewModel.AllInternalCompanies.Where(f => f.Id == 659).Count(), 1);
            Assert.AreEqual(viewModel.AllSuppliers.Where(f => f.Id == -1).Count(), 1);
            Assert.AreEqual(viewModel.AllFunders.Where(f => f.Id == -1).Count(), 1);
            Assert.AreEqual(viewModel.AllTrancheStatuses.Count(), 0);
            Assert.AreEqual(viewModel.AllFinanceTypes.Where(f => f.Id == 1).Count(), 1);
            Assert.AreEqual(viewModel.AllDirectDebitProfiles.Where(f => f.ID == 1).Count(), 1);
            Mock.Assert(() => FinanceTypeFunctions.GetAllFinanceTypesAsync(), Occurs.Once());
            Mock.Assert(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.TrancheStatus), Occurs.Exactly(2));
        }

        [TestMethod]
        public async Task OnStep_Start_Populate_Records_NoRecord_ToPopulate()
        {
            var viewModel = Mock.Create<FundingDetailsViewModel>(Behavior.CallOriginal, new FundingSummaryViewModel());

            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.TrancheStatus)).DoNothing();
            Mock.Arrange(() => FinanceTypeFunctions.GetAllFinanceTypesAsync()).DoNothing();
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.Frequency)).DoNothing();
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.InstallmentType)).DoNothing();
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.FundingStatus)).DoNothing();
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.InternalCompany)).DoNothing();
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Supplier)).DoNothing();
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Funder)).DoNothing();
            Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync()).DoNothing();

            await viewModel.OnStepAsync("Start");

            Assert.AreEqual(viewModel.AllFundingStatuses.Count, 1);
            Assert.AreEqual(viewModel.AllInstalmentType.Count(), 0);
            Assert.AreEqual(viewModel.AllInternalCompanies.Count(), 0);
            Assert.AreEqual(viewModel.AllSuppliers.Count(), 1);
            Assert.AreEqual(viewModel.AllFunders.Count(), 1);
            Assert.AreEqual(viewModel.AllTrancheStatuses.Count(), 0);
            Assert.AreEqual(viewModel.AllFinanceTypes.Count(), 0);
            Assert.AreEqual(viewModel.AllDirectDebitProfiles.Count(), 0);
            Mock.Assert(() => FinanceTypeFunctions.GetAllFinanceTypesAsync(), Occurs.Once());
        }

        [TestMethod]
        public async void OnStep_SelectAllContracts()
        {
            var viewModel = Mock.Create<FundingDetailsViewModel>(Behavior.CallOriginal, new FundingSummaryViewModel());

            bool onAggreagateCalled = false;
            Mock.Arrange(() => viewModel.NotIncludedInTrancheContracts).Returns(new ObservableModelCollection<TrancheContractSummary>(Helper.FakeTrancheContracts()));

            viewModel.onAggregateFunctionCallRequired += (b) => { onAggreagateCalled = true; };

            await viewModel.OnStepAsync("SelectAllContracts");
            await viewModel.OnStepAsync("AddToExisting");


            Assert.IsTrue(viewModel.IncludedInTrancheContracts.Count == Helper.FakeTrancheContracts().Count);
            Assert.IsTrue(viewModel.NotIncludedInTrancheContracts.Count == 0);

            Assert.IsTrue(onAggreagateCalled);
        }

        [TestMethod]
        public void OnStep_DeSelectAllContracts()
        {
            var viewModel = Mock.Create<FundingDetailsViewModel>(Behavior.CallOriginal, new FundingSummaryViewModel());

            bool onAggreagateCalled = false;
            Mock.Arrange(() => viewModel.IncludedInTrancheContracts).Returns(new ObservableModelCollection<TrancheContractSummary>(Helper.FakeTrancheContracts()));

            viewModel.onAggregateFunctionCallRequired += (b) => { onAggreagateCalled = true; };

            viewModel.OnStepAsync(FundingDetailsViewModel.EnumStep.DeSelectAllContracts);
            viewModel.OnStepAsync(FundingDetailsViewModel.EnumStep.RemoveFromExisting);

            Assert.IsTrue(viewModel.IncludedInTrancheContracts.Count == 0);
            Assert.IsTrue(viewModel.NotIncludedInTrancheContracts.Count == Helper.FakeTrancheContracts().Count);

            Assert.IsTrue(onAggreagateCalled);
        }

        [TestMethod]
        public async Task OnStep_Calculate_Only_In_Confirmed()
        {
            var viewModel = Mock.Create<FundingDetailsViewModel>(Behavior.CallOriginal, new FundingSummaryViewModel());

            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.TrancheStatus)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => FinanceTypeFunctions.GetAllFinanceTypesAsync()).ReturnsCollection(Helper.FakeRawFinanceTypes());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.Frequency)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.InstallmentType)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.FundingStatus)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.InternalCompany)).ReturnsCollection(Helper.FakeRawInternalCompanies());
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Supplier)).ReturnsCollection(Helper.FakeRawSuppliers());
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Funder)).ReturnsCollection((Helper.FakeEntityRelations()));
            Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync()).ReturnsCollection<DirectDebitProfile>(Helper.FakeDirectDebitProfiles());

            Mock.Arrange(() => viewModel.IncludedInTrancheContracts).Returns(new ObservableModelCollection<TrancheContractSummary>(Helper.FakeTrancheContracts()));
            Mock.Arrange(() => viewModel.OriginalTrancheStatus).Returns(TrancheStatus.Pending);
            Mock.Arrange(() => viewModel.SelectedTrancheProfile).Returns(Helper.FakeTrancheProfile());
            Mock.NonPublic.Arrange<bool>(viewModel, "Lock", "FundingTranche", Helper.FakeTrancheProfile().TrancheId.ToString()).Returns(true);
            Mock.NonPublic.Arrange(viewModel, "UnLock").DoNothing();

            await viewModel.OnStepAsync("Start");
            await viewModel.OnStepAsync("Calculate");
            Assert.AreEqual(viewModel.ValidationSummary[0].ErrorMessage, "Calculation can be done only for tranches that are Confirmed");
            Mock.Arrange(() => viewModel.OriginalTrancheStatus).Returns(TrancheStatus.Funded);
            await viewModel.OnStepAsync("Calculate");
            Assert.AreEqual(viewModel.ValidationSummary[0].ErrorMessage, "Calculation can be done only for tranches that are Confirmed");
        }

        [TestMethod]
        public async Task OnStep_Calculate_When_No_Contracts()
        {
            var viewModel = Mock.Create<FundingDetailsViewModel>(Behavior.CallOriginal, new FundingSummaryViewModel());
            var contracts = Helper.FakeTrancheContracts();

            contracts.Clear();

            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.TrancheStatus)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => FinanceTypeFunctions.GetAllFinanceTypesAsync()).ReturnsCollection(Helper.FakeRawFinanceTypes());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.Frequency)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.InstallmentType)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.FundingStatus)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.InternalCompany)).ReturnsCollection(Helper.FakeRawInternalCompanies());
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Supplier)).ReturnsCollection(Helper.FakeRawSuppliers());
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Funder)).ReturnsCollection((Helper.FakeEntityRelations()));
            Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync()).ReturnsCollection<DirectDebitProfile>(Helper.FakeDirectDebitProfiles());

            Mock.Arrange(() => viewModel.IncludedInTrancheContracts).Returns(new ObservableModelCollection<TrancheContractSummary>(contracts));
            Mock.Arrange(() => viewModel.OriginalTrancheStatus).Returns(TrancheStatus.Confirmed);
            Mock.Arrange(() => viewModel.SelectedTrancheProfile).Returns(Helper.FakeTrancheProfile());
            Mock.NonPublic.Arrange<bool>(viewModel, "Lock", "FundingTranche", Helper.FakeTrancheProfile().TrancheId.ToString()).Returns(true);
            Mock.NonPublic.Arrange(viewModel, "UnLock").DoNothing();

            await viewModel.OnStepAsync("Start");
            await viewModel.OnStepAsync("Calculate");
            Assert.AreEqual(viewModel.ValidationSummary.Count, 1);
            Assert.AreEqual(viewModel.ValidationSummary[0].ErrorMessage, "Cannot proceed as there is no existing contract");

            Mock.NonPublic.Assert(viewModel, "Lock", Occurs.Never());
            Mock.NonPublic.Assert(viewModel, "UnLock", Occurs.Never());
        }

        [TestMethod]
        public async Task OnStep_Calculate_UnSuccessful_Lock()
        {
            var viewModel = Mock.Create<FundingDetailsViewModel>(Behavior.CallOriginal, new FundingSummaryViewModel());

            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.TrancheStatus)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => FinanceTypeFunctions.GetAllFinanceTypesAsync()).ReturnsCollection(Helper.FakeRawFinanceTypes());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.Frequency)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.InstallmentType)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.FundingStatus)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.InternalCompany)).ReturnsCollection(Helper.FakeRawInternalCompanies());
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Supplier)).ReturnsCollection(Helper.FakeRawSuppliers());
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Funder)).ReturnsCollection((Helper.FakeEntityRelations()));
            Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync()).ReturnsCollection<DirectDebitProfile>(Helper.FakeDirectDebitProfiles());

            Mock.Arrange(() => viewModel.IncludedInTrancheContracts).Returns(new ObservableModelCollection<TrancheContractSummary>(Helper.FakeTrancheContracts()));
            Mock.Arrange(() => viewModel.OriginalTrancheStatus).Returns(TrancheStatus.Confirmed);
            Mock.Arrange(() => viewModel.SelectedTrancheProfile).Returns(Helper.FakeTrancheProfile());
            Mock.NonPublic.Arrange<bool>(viewModel, "Lock", "FundingTranche", Helper.FakeTrancheProfile().TrancheId.ToString()).Returns(false);

            await viewModel.OnStepAsync("Start");
            await viewModel.OnStepAsync("Calculate");

            Assert.AreEqual(viewModel.ValidationSummary.Count, 0);
            Mock.NonPublic.Assert(viewModel, "Lock", Occurs.Once());
            Mock.NonPublic.Assert(viewModel, "UnLock", Occurs.Never());
        }

        [TestMethod]
        public async Task OnStep_Calculate_SuccessfulAsync()
        {
            var viewModel = Mock.Create<FundingDetailsViewModel>(Behavior.CallOriginal, new FundingSummaryViewModel());

            var contractFundingDetails = Helper.FakeContractFundingDetailsForCalcualationOnTranche1().AsQueryable();
            var contractFlags = Helper.FakeContractFlagsForCalcualationOnTranche1().AsQueryable();
            var contracts = Helper.FakeContractsForCalcualationOnTranche1().AsQueryable();
            var contractAmortSchedules = Helper.FakeContractAmortSchedulesForCalcualationOnTranche1().AsQueryable();


            var mockSetContractFundingDetail = Mock.Create<DbSet<ContractFundingDetail>>();
            Mock.Arrange(() => ((IDbAsyncEnumerable<ContractFundingDetail>)mockSetContractFundingDetail).GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<ContractFundingDetail>(contractFundingDetails.GetEnumerator()));
            Mock.Arrange(() => ((IQueryable<ContractFundingDetail>)mockSetContractFundingDetail).Provider).Returns(new TestDbAsyncQueryProvider<ContractFundingDetail>(contractFundingDetails.Provider));
            Mock.Arrange(() => ((IQueryable<ContractFundingDetail>)mockSetContractFundingDetail).Expression).Returns(contractFundingDetails.Expression);
            Mock.Arrange(() => ((IQueryable<ContractFundingDetail>)mockSetContractFundingDetail).ElementType).Returns(contractFundingDetails.ElementType);
            Mock.Arrange(() => ((IQueryable<ContractFundingDetail>)mockSetContractFundingDetail).GetEnumerator()).Returns(contractFundingDetails.GetEnumerator());

            var mockSetContractFlag = Mock.Create<DbSet<ContractFlag>>();
            Mock.Arrange(() => ((IDbAsyncEnumerable<ContractFlag>)(mockSetContractFlag)).GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<ContractFlag>(contractFlags.GetEnumerator()));
            Mock.Arrange(() => ((IQueryable<ContractFlag>)mockSetContractFlag).Provider).Returns(new TestDbAsyncQueryProvider<ContractFlag>(contractFlags.Provider));
            Mock.Arrange(() => ((IQueryable<ContractFlag>)mockSetContractFlag).Expression).Returns(contractFlags.Expression);
            Mock.Arrange(() => ((IQueryable<ContractFlag>)mockSetContractFlag).ElementType).Returns(contractFlags.ElementType);
            Mock.Arrange(() => ((IQueryable<ContractFlag>)mockSetContractFlag).GetEnumerator()).Returns(contractFlags.GetEnumerator());

            var mockSetContract = Mock.Create<DbSet<Contract>>();
            Mock.Arrange(() => ((IDbAsyncEnumerable<Contract>)(mockSetContract)).GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Contract>(contracts.GetEnumerator()));
            Mock.Arrange(() => ((IQueryable<Contract>)mockSetContract).Provider).Returns(new TestDbAsyncQueryProvider<Contract>(contracts.Provider));
            Mock.Arrange(() => ((IQueryable<Contract>)mockSetContract).Expression).Returns(contracts.Expression);
            Mock.Arrange(() => ((IQueryable<Contract>)mockSetContract).ElementType).Returns(contracts.ElementType);
            Mock.Arrange(() => ((IQueryable<Contract>)mockSetContract).GetEnumerator()).Returns(contracts.GetEnumerator());

            var mockSetContractAmortSched = Mock.Create<DbSet<ContractAmortSched>>();
            Mock.Arrange(() => ((IDbAsyncEnumerable<ContractAmortSched>)(mockSetContractAmortSched)).GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<ContractAmortSched>(contractAmortSchedules.GetEnumerator()));
            Mock.Arrange(() => ((IQueryable<ContractAmortSched>)mockSetContractAmortSched).Provider).Returns(new TestDbAsyncQueryProvider<ContractAmortSched>(contractAmortSchedules.Provider));
            Mock.Arrange(() => ((IQueryable<ContractAmortSched>)mockSetContractAmortSched).Expression).Returns(contractAmortSchedules.Expression);
            Mock.Arrange(() => ((IQueryable<ContractAmortSched>)mockSetContractAmortSched).ElementType).Returns(contractAmortSchedules.ElementType);
            Mock.Arrange(() => ((IQueryable<ContractAmortSched>)mockSetContractAmortSched).GetEnumerator()).Returns(contractAmortSchedules.GetEnumerator());

            var mockContext = Mock.Create<Entities>();
            Mock.Arrange(() => mockContext.ContractFundingDetails).Returns(mockSetContractFundingDetail);
            Mock.Arrange(() => mockContext.ContractFlags).Returns(mockSetContractFlag);
            Mock.Arrange(() => mockContext.Contracts).Returns(mockSetContract);
            Mock.Arrange(() => mockContext.ContractAmortScheds).Returns(mockSetContractAmortSched);

            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.TrancheStatus)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => FinanceTypeFunctions.GetAllFinanceTypesAsync()).ReturnsCollection(Helper.FakeRawFinanceTypes());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.Frequency)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.InstallmentType)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.FundingStatus)).ReturnsCollection(Helper.FakeSystemConstants());
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.InternalCompany)).ReturnsCollection(Helper.FakeRawInternalCompanies());
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Supplier)).ReturnsCollection(Helper.FakeRawSuppliers());
            Mock.Arrange(() => OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Funder)).ReturnsCollection((Helper.FakeEntityRelations()));
            Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync()).ReturnsCollection<DirectDebitProfile>(Helper.FakeDirectDebitProfiles());

            Mock.Arrange(() => viewModel.IncludedInTrancheContracts).Returns(new ObservableModelCollection<TrancheContractSummary>(Helper.FakeTrancheContracts()));
            Mock.Arrange(() => viewModel.OriginalTrancheStatus).Returns(TrancheStatus.Confirmed);
            Mock.Arrange(() => viewModel.SelectedTrancheProfile).Returns(Helper.FakeTrancheProfile());
            Mock.NonPublic.Arrange<bool>(viewModel, "Lock").Returns(true);
            Mock.NonPublic.Arrange(viewModel, "UnLock").DoNothing();

            await viewModel.OnStepAsync("Start");
            await viewModel.OnStepAsync("Calculate");

            Assert.AreEqual(viewModel.ValidationSummary.Count, 0);
            Mock.NonPublic.Assert<bool>(viewModel, "Lock", Occurs.Once());
            Mock.NonPublic.Assert(viewModel, "UnLock", Occurs.Once());
        }
    }
}
