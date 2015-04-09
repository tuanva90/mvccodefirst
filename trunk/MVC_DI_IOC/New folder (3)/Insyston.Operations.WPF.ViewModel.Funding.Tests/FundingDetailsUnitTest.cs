// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FundingDetailsUnitTest.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The funding details unit test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Funding.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Common;
    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Business.Funding;
    using Insyston.Operations.Business.Funding.Model;
    using Insyston.Operations.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// The funding details unit test.
    /// </summary>
    [TestClass]
    public class FundingDetailsUnitTest
    {
        /// <summary>
        /// The test on step async_ start_ populate records.
        /// Expect PopulateRecords
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncStart_PopulateRecords()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                           .IgnoreArguments()
                           .DoNothing();
                        var summaryViewModel = Mock.Create<FundingSummaryViewModel>(new Constructor(), Behavior.CallOriginal);
                        var detailsViewModel = Mock.Create<FundingDetailsViewModel>(x =>
                        {
                            x.CallConstructor(() => new FundingDetailsViewModel(summaryViewModel));
                            x.SetBehavior(Behavior.CallOriginal);
                        });

                        Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);

                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.Funder)).Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawFunders());
                        Mock.Arrange(
                            () =>
                            DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync()).Returns(FakeDataFunding.FakeDataFundingDetail.FakeDirectDebitProfiles());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(SystemConstantType.TrancheStatus)).Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(SystemConstantType.FundingStatus)).Returns(Task.Run(
                                () =>
                                    {
                                        return new List<SystemConstant>();
                                    }));
                        Mock.Arrange(
                            () =>
                            FinanceTypeFunctions.GetAllFinanceTypesAsync()).Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawFinanceTypes());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(SystemConstantType.Frequency)).Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(SystemConstantType.InstallmentType)).Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(EntityCategory.InternalCompany)).Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawInternalCompanies());
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(EntityCategory.Supplier)).Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawSuppliers());

                        Mock.Arrange(() => detailsViewModel.SelectedTrancheProfile).Returns(FakeDataFunding.FakeDataFundingDetail.FakeTrancheProfile());
                        Mock.Arrange(() => detailsViewModel.IncludedInTrancheContracts).Returns(new ObservableModelCollection<TrancheContractSummary>());

                        await detailsViewModel.OnStepAsync("Start");

                        // Expect PopulateRecords
                        Assert.AreEqual(1, detailsViewModel.AllDirectDebitProfiles.Where(f => f.ID == 1).Count());
                        Assert.AreEqual(5, detailsViewModel.AllFunders.Count());
                        Assert.AreEqual(6, detailsViewModel.AllTrancheStatuses.Count());
                        Assert.AreEqual(1, detailsViewModel.AllFundingStatuses.Where(f => f.Id == -1).Count());
                        Assert.AreEqual(1, detailsViewModel.AllFinanceTypes.Where(f => f.Id == 1).Count());
                        Assert.AreEqual(1, detailsViewModel.AllFrequencies.Where(f => f.Id == 6).Count());
                        Assert.AreEqual(1, detailsViewModel.AllInstalmentType.Where(f => f.Id == 1).Count());
                        Assert.AreEqual(1, detailsViewModel.AllInternalCompanies.Where(f => f.Id == 659).Count());
                        Assert.AreEqual(1, detailsViewModel.AllSuppliers.Where(f => f.Id == 234).Count());

                        Mock.Assert(() => FinanceTypeFunctions.GetAllFinanceTypesAsync(), Occurs.Once());
                    });
        }

        /// <summary>
        /// The test on step async_ start_ populate records no record to populate.
        /// Expect PopulateRecords NoRecord To Populate
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncStart_PopulateRecordsNoRecordToPopulate()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();
                        var summaryViewModel = Mock.Create<FundingSummaryViewModel>(
                            new Constructor(),
                            Behavior.CallOriginal);
                        var detailsViewModel = Mock.Create<FundingDetailsViewModel>(
                            x =>
                                {
                                    x.CallConstructor(() => new FundingDetailsViewModel(summaryViewModel));
                                    x.SetBehavior(Behavior.CallOriginal);
                                });

                        Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);

                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.TrancheStatus)).Returns(Task.Run(
                                () =>
                                {
                                    return new List<SystemConstant>();
                                }));
                        Mock.Arrange(() => FinanceTypeFunctions.GetAllFinanceTypesAsync()).Returns(Task.Run(
                                () =>
                                {
                                    return new List<FinanceType>();
                                }));
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.Frequency)).Returns(Task.Run(
                                () =>
                                {
                                    return new List<SystemConstant>();
                                }));
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.InstallmentType)).Returns(Task.Run(
                                () =>
                                {
                                    return new List<SystemConstant>();
                                }));
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.FundingStatus)).Returns(Task.Run(
                                () =>
                                {
                                    return new List<SystemConstant>();
                                }));
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.InternalCompany)).Returns(Task.Run(
                                () =>
                                {
                                    return new List<vwEntityRelation>();
                                }));
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.Supplier)).Returns(Task.Run(
                                () =>
                                {
                                    return new List<vwEntityRelation>();
                                }));
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.Funder)).Returns(Task.Run(
                                () =>
                                {
                                    return new List<vwEntityRelation>();
                                }));
                        Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync()).Returns(Task.Run(
                                () =>
                                {
                                    return Enumerable.Empty<DirectDebitProfile>();
                                }));
                        Mock.Arrange(() => detailsViewModel.SelectedTrancheProfile).Returns(FakeDataFunding.FakeDataFundingDetail.FakeTrancheProfile());
                        Mock.Arrange(() => detailsViewModel.IncludedInTrancheContracts).Returns(new ObservableModelCollection<TrancheContractSummary>());

                        await detailsViewModel.OnStepAsync("Start");

                        // Expect PopulateRecords NoRecord To Populate
                        Assert.AreEqual(detailsViewModel.AllFundingStatuses.Count, 1);
                        Assert.AreEqual(detailsViewModel.AllInstalmentType.Count(), 0);
                        Assert.AreEqual(detailsViewModel.AllInternalCompanies.Count(), 0);
                        Assert.AreEqual(detailsViewModel.AllSuppliers.Count(), 1);
                        Assert.AreEqual(detailsViewModel.AllFunders.Count(), 1);
                        Assert.AreEqual(detailsViewModel.AllTrancheStatuses.Count(), 0);
                        Assert.AreEqual(detailsViewModel.AllFinanceTypes.Count(), 0);
                        Assert.AreEqual(detailsViewModel.AllDirectDebitProfiles.Count(), 0);
                        Mock.Assert(() => FinanceTypeFunctions.GetAllFinanceTypesAsync(), Occurs.Once());
                    });
        }

        /// <summary>
        /// The test on step async_ select all contracts and add to existing.
        /// fake 4 TrancheContracts with 2 isSelected = true, 2 isSelected = false
        /// Expect IncludedInTrancheContracts = 2 and NotIncludedInTrancheContracts when OnStep AddToExisting
        /// Expect onAggregateCalled is true when OnStep SelectAllContracts
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncSelectAll_ContractsAndAddToExisting()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();
                        var summaryViewModel = Mock.Create<FundingSummaryViewModel>(
                            new Constructor(),
                            Behavior.CallOriginal);
                        var detailsViewModel = Mock.Create<FundingDetailsViewModel>(
                            x =>
                                {
                                    x.CallConstructor(() => new FundingDetailsViewModel(summaryViewModel));
                                    x.SetBehavior(Behavior.CallOriginal);
                                });

                        bool onAggreagateCalled = false;

                        detailsViewModel.NotIncludedInTrancheContracts = new ObservableModelCollection<TrancheContractSummary>(FakeDataFunding.FakeDataFundingDetail.FakeTrancheContracts());

                        detailsViewModel.onAggregateFunctionCallRequired += b => { onAggreagateCalled = true; };
                        detailsViewModel.IncludedInTrancheContracts = new ObservableModelCollection<TrancheContractSummary>();
                        await detailsViewModel.OnStepAsync("SelectAllContracts");
                        await detailsViewModel.OnStepAsync("AddToExisting");

                        // fake 4 TrancheContracts with 2 isSelected = true, 2 isSelected = false
                        // Expect IncludedInTrancheContracts = 2 and NotIncludedInTrancheContracts when OnStep AddToExisting
                        // Expect onAggreagateCalled is true when OnStep SelectAllContracts
                        Assert.IsTrue(detailsViewModel.IncludedInTrancheContracts.Count == 2);
                        Assert.IsTrue(detailsViewModel.NotIncludedInTrancheContracts.Count == 2);

                        Assert.IsTrue(onAggreagateCalled);
                    });
        }

        /// <summary>
        /// The test on step async_ de select all contracts and remove from existing.
        /// fake 4 TrancheContracts with 2 isSelected = true, 2 isSelected = false
        /// Expect IncludedInTrancheContracts = 2 and NotIncludedInTrancheContracts when OnStep RemoveFromExisting
        /// Expect onAggregateCalled is true when OnStep DeSelectAllContracts
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncDeSelectAllContractsAndRemoveFromExisting()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();
                        var summaryViewModel = Mock.Create<FundingSummaryViewModel>(
                            new Constructor(),
                            Behavior.CallOriginal);
                        var detailsViewModel = Mock.Create<FundingDetailsViewModel>(
                            x =>
                                {
                                    x.CallConstructor(() => new FundingDetailsViewModel(summaryViewModel));
                                    x.SetBehavior(Behavior.CallOriginal);
                                });

                        bool onAggreagateCalled = false;

                        detailsViewModel.IncludedInTrancheContracts = new ObservableModelCollection<TrancheContractSummary>(FakeDataFunding.FakeDataFundingDetail.FakeTrancheContracts());
                        detailsViewModel.NotIncludedInTrancheContracts = new ObservableModelCollection<TrancheContractSummary>();

                        detailsViewModel.onAggregateFunctionCallRequired += b => { onAggreagateCalled = true; };

                        await detailsViewModel.OnStepAsync(FundingDetailsViewModel.EnumStep.DeSelectAllContracts);
                        await detailsViewModel.OnStepAsync(FundingDetailsViewModel.EnumStep.RemoveFromExisting);

                        // fake 4 TrancheContracts with 2 isSelected = true, 2 isSelected = false
                        // Expect IncludedInTrancheContracts = 2 and NotIncludedInTrancheContracts when OnStep RemoveFromExisting
                        // Expect onAggreagateCalled is true when OnStep DeSelectAllContracts
                        Assert.IsTrue(detailsViewModel.IncludedInTrancheContracts.Count == 2);
                        Assert.IsTrue(detailsViewModel.NotIncludedInTrancheContracts.Count == 2);

                        Assert.IsTrue(onAggreagateCalled);
                    });
        }

        /// <summary>
        /// The test on step async_ calculate_ only_ in_ confirmed.
        /// Expect ErrorMessage = "Calculation can be done only for tranches that are Confirmed"
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncCalculate_OnlyInConfirmed()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();
                        var summaryViewModel = Mock.Create<FundingSummaryViewModel>(
                            new Constructor(),
                            Behavior.CallOriginal);
                        var detailsViewModel = Mock.Create<FundingDetailsViewModel>(
                            x =>
                                {
                                    x.CallConstructor(() => new FundingDetailsViewModel(summaryViewModel));
                                    x.SetBehavior(Behavior.CallOriginal);
                                });

                        Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);

                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.TrancheStatus))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(() => FinanceTypeFunctions.GetAllFinanceTypesAsync())
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawFinanceTypes());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.Frequency))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.InstallmentType))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.FundingStatus))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.InternalCompany))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawInternalCompanies());
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.Supplier))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawSuppliers());
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.Funder))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawFunders());
                        Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync())
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeDirectDebitProfiles());

                        Mock.Arrange(() => detailsViewModel.IncludedInTrancheContracts)
                            .Returns(
                                new ObservableModelCollection<TrancheContractSummary>(FakeDataFunding.FakeDataFundingDetail.FakeTrancheContracts()));

                        Mock.Arrange(() => detailsViewModel.OriginalTrancheStatus).Returns(TrancheStatus.Pending);
                        Mock.Arrange(() => detailsViewModel.SelectedTrancheProfile).Returns(FakeDataFunding.FakeDataFundingDetail.FakeTrancheProfile());
                        Mock.Arrange(() => detailsViewModel.IncludedInTrancheContracts).Returns(new ObservableModelCollection<TrancheContractSummary>(FakeDataFunding.FakeDataFundingDetail.FakeTrancheContracts()));

                        Mock.NonPublic.Arrange<Task<bool>>(detailsViewModel, "LockAsync").Returns(Task.Run(
                            () => { return true; }));

                        Mock.NonPublic.Arrange<Task>(detailsViewModel, "UnLockAsync").Returns(Task.Run(() => { }));
                        Mock.NonPublic.Arrange<Task>(detailsViewModel, "BuildBaseQueryAsync").Returns(
                            Task.Run(() => { }));

                        await detailsViewModel.OnStepAsync("Start");
                        await detailsViewModel.OnStepAsync("Calculate");

                        // Expect ErrorMessage = "Calculation can be done only for tranches that are Confirmed"
                        Assert.AreEqual(
                            detailsViewModel.ValidationSummary[0].ErrorMessage,
                            "Calculation can be done only for tranches that are Confirmed");
                        Mock.Arrange(() => detailsViewModel.OriginalTrancheStatus).Returns(TrancheStatus.Funded);
                        await detailsViewModel.OnStepAsync("Calculate");
                        Assert.AreEqual(
                            detailsViewModel.ValidationSummary[0].ErrorMessage,
                            "Calculation can be done only for tranches that are Confirmed");
                    });
        }

        /// <summary>
        /// The on step_ calculate_ when_ no_ contracts.
        /// Expect 1 error ErrorMessage "Cannot proceed as there is no existing contract"
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncCalculate_WhenNoContracts()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();
                        var summaryViewModel = Mock.Create<FundingSummaryViewModel>(
                            new Constructor(),
                            Behavior.CallOriginal);
                        var detailsViewModel = Mock.Create<FundingDetailsViewModel>(
                            x =>
                                {
                                    x.CallConstructor(() => new FundingDetailsViewModel(summaryViewModel));
                                    x.SetBehavior(Behavior.CallOriginal);
                                });
                        var contracts = FakeDataFunding.FakeDataFundingDetail.FakeTrancheContracts();

                        contracts.Clear();

                        Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);

                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.TrancheStatus))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(() => FinanceTypeFunctions.GetAllFinanceTypesAsync())
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawFinanceTypes());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.Frequency))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.InstallmentType))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.FundingStatus))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.InternalCompany))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawInternalCompanies());
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.Supplier))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawSuppliers());
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.Funder))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawFunders());
                        Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync())
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeDirectDebitProfiles());

                        Mock.Arrange(() => detailsViewModel.IncludedInTrancheContracts)
                            .Returns(new ObservableModelCollection<TrancheContractSummary>(contracts));
                        Mock.Arrange(() => detailsViewModel.OriginalTrancheStatus).Returns(TrancheStatus.Confirmed);

                        Mock.Arrange(() => detailsViewModel.SelectedTrancheProfile).Returns(FakeDataFunding.FakeDataFundingDetail.FakeTrancheProfile());
                        
                        Mock.NonPublic.Arrange<Task<bool>>(detailsViewModel, "LockAsync").Returns(Task.Run(() => { return true; }));

                        Mock.NonPublic.Arrange<Task>(detailsViewModel, "UnLockAsync").Returns(Task.Run(() => { }));
                        Mock.NonPublic.Arrange<Task>(detailsViewModel, "BuildBaseQueryAsync").Returns(
                            Task.Run(() => { }));

                        await detailsViewModel.OnStepAsync("Start");
                        await detailsViewModel.OnStepAsync("Calculate");

                        // Expect 1 error ErrorMessage "Cannot proceed as there is no existing contract");
                        Assert.AreEqual(detailsViewModel.ValidationSummary.Count, 1);
                        Assert.AreEqual(
                            detailsViewModel.ValidationSummary[0].ErrorMessage,
                            "Cannot proceed as there is no existing contract");

                        Mock.NonPublic.Assert(detailsViewModel, "LockAsync", Occurs.Never());
                        Mock.NonPublic.Assert(detailsViewModel, "UnLockAsync", Occurs.Never());
                    });
        }

        /// <summary>
        /// The on step_ calculate_ successful_ lock.
        /// Expect ValidationSummary no error, because StartDate less than TrancheDate and FundingStartDate less than LastPaymentDate in ContractValidation 
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncCalculate_SuccessfulLock()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();
                        var summaryViewModel = Mock.Create<FundingSummaryViewModel>(
                            new Constructor(),
                            Behavior.CallOriginal);
                        var detailsViewModel = Mock.Create<FundingDetailsViewModel>(
                            x =>
                            {
                                x.CallConstructor(() => new FundingDetailsViewModel(summaryViewModel));
                                x.SetBehavior(Behavior.CallOriginal);
                            });
                        var contracts = FakeDataFunding.FakeDataFundingDetail.FakeTrancheContracts();

                        Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(FundingFunctions), StaticConstructor.Mocked);

                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.TrancheStatus))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(() => FinanceTypeFunctions.GetAllFinanceTypesAsync())
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawFinanceTypes());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.Frequency))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.InstallmentType))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(
                            () =>
                            SystemConstantFunctions.GetSystemConstantsByTypeAsync(
                                SystemConstantType.FundingStatus))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeSystemConstants());
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.InternalCompany))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawInternalCompanies());
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.Supplier))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawSuppliers());
                        Mock.Arrange(
                            () =>
                            OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(
                                EntityCategory.Funder))
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeRawFunders());
                        Mock.Arrange(() => DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync())
                            .Returns(FakeDataFunding.FakeDataFundingDetail.FakeDirectDebitProfiles());

                        Mock.Arrange(() => FundingFunctions.CalculateFundingTrancheAsync(detailsViewModel.SelectedTrancheProfile)).IgnoreArguments()
                            .Returns(Task.Run(() => { }));

                        Mock.Arrange(() => detailsViewModel.IncludedInTrancheContracts)
                            .Returns(new ObservableModelCollection<TrancheContractSummary>(contracts));
                        Mock.Arrange(() => detailsViewModel.OriginalTrancheStatus).Returns(TrancheStatus.Confirmed);

                        Mock.Arrange(() => detailsViewModel.SelectedTrancheProfile).Returns(FakeDataFunding.FakeDataFundingDetail.FakeTrancheProfile());

                        Mock.NonPublic.Arrange<Task<bool>>(detailsViewModel, "LockAsync").Returns(Task.Run(() => { return true; }));

                        Mock.NonPublic.Arrange<Task>(detailsViewModel, "UnLockAsync").Returns(Task.Run(() => { }));
                        Mock.NonPublic.Arrange<Task>(detailsViewModel, "BuildBaseQueryAsync").Returns(
                            Task.Run(() => { }));

                        await detailsViewModel.OnStepAsync("Start");
                        await detailsViewModel.OnStepAsync("Calculate");

                        // Expect ValidationSummary no error, because StartDate < TrancheDate and FundingStartDate < LastPaymentDate in ContractValidation 
                        Assert.AreEqual(detailsViewModel.ValidationSummary.Count, 0);
                        Mock.NonPublic.Assert(detailsViewModel, "LockAsync", Occurs.Once());
                        Mock.NonPublic.Assert(detailsViewModel, "UnLockAsync", Occurs.Once());
                    });
        }
    }
}
