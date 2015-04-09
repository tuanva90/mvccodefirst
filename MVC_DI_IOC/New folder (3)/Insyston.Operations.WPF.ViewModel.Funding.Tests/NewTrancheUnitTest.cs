// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewTrancheUnitTest.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The new tranche unit test.
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
    /// The new tranche unit test.
    /// </summary>
    [TestClass]
    public class NewTrancheUnitTest
    {
        /// <summary>
        /// The test on step async start_ no changed.
        /// Expect newTrancheViewModel.IsChanged == true
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncStart_NoChanged()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();

                        // Create mock
                        var summaryViewModel = Mock.Create<FundingSummaryViewModel>(
                            new Constructor(),
                            Behavior.CallOriginal);
                        var detailsViewModel = Mock.Create<FundingDetailsViewModel>(x =>
                        {
                            x.CallConstructor(() => new FundingDetailsViewModel(summaryViewModel));
                            x.SetBehavior(Behavior.CallOriginal);
                        });
                        var newTrancheViewModel = Mock.Create<NewTrancheViewModel>(x =>
                        {
                            x.CallConstructor(() => new NewTrancheViewModel(summaryViewModel));
                            x.SetBehavior(Behavior.CallOriginal);
                            
                        });

                        // Create mock static class
                        Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                        Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);

                        // Fake data
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

                        newTrancheViewModel.IsChanged = false;

                        // Run method
                        await newTrancheViewModel.OnStepAsync(FundingDetailsViewModel.EnumStep.Start);

                        // Expect newTrancheViewModel.IsChanged == true
                        Assert.AreEqual(newTrancheViewModel.IsChanged, true);
                    });
        }

        /// <summary>
        /// The test on step async save_ added tranche id.
        /// Expect summaryViewModel.AddedTrancheId = newTrancheViewModel.SelectedTrancheProfile.TrancheId
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncSave_AddedTrancheId()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    // Create mock
                    var summaryViewModel = Mock.Create<FundingSummaryViewModel>(
                        new Constructor(),
                        Behavior.CallOriginal);
                    var existingTranche = Mock.Create<ExistingTrancheViewModel>(
                        x =>
                        {
                            x.CallConstructor(() => new ExistingTrancheViewModel(summaryViewModel));
                            x.SetBehavior(Behavior.CallOriginal);
                        });
                    var detailsViewModel = Mock.Create<FundingDetailsViewModel>(
                        x =>
                        {
                            x.CallConstructor(() => new FundingDetailsViewModel(summaryViewModel));
                            x.SetBehavior(Behavior.CallOriginal);
                        });
                    var newTrancheViewModel = Mock.Create<NewTrancheViewModel>(x =>
                    {
                        x.CallConstructor(() => new NewTrancheViewModel(summaryViewModel));
                        x.SetBehavior(Behavior.CallOriginal);

                    });
                    var contracts = FakeDataFunding.FakeDataFundingDetail.FakeTrancheContracts();

                    // Create mock static class
                    Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(FundingFunctions), StaticConstructor.Mocked);

                    // Fake data
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

                    Mock.Arrange(() => newTrancheViewModel.IncludedInTrancheContracts)
                        .Returns(new ObservableModelCollection<TrancheContractSummary>(contracts));
                    Mock.Arrange(() => detailsViewModel.OriginalTrancheStatus).Returns(TrancheStatus.Confirmed);

                    Mock.Arrange(() => newTrancheViewModel.SelectedTrancheProfile).Returns(FakeDataFunding.FakeDataFundingDetail.FakeTrancheProfile());

                    Mock.NonPublic.Arrange<Task>(newTrancheViewModel, "SaveAsync").Returns(Task.Run(() => { }));

                    // Fake data for private property
                    var priva = new PrivateAccessor(summaryViewModel);
                    priva.SetProperty("ExistingTrancheViewModel", existingTranche);
                    summaryViewModel.AddedTrancheId = new int();

                    // Fake data
                    summaryViewModel.AddedTrancheId = 1;
                    Mock.Arrange(() => summaryViewModel.FundingDetails)
                        .Returns(new FundingDetailsViewModel(summaryViewModel));
                    Mock.Arrange(() => existingTranche.SetTrancheIdAsync(Arg.IsAny<int>())).Returns(Task.Run(() => { }));
                    Mock.Arrange(() => FundingFunctions.GetAllTranchesAsync())
                        .Returns(FakeDataFunding.FakeDataFundingSummary.FakeFundingSummaryList());

                    // Run method
                    await newTrancheViewModel.OnStepAsync(FundingDetailsViewModel.EnumStep.Save);

                    // Expect summaryViewModel.AddedTrancheId = newTrancheViewModel.SelectedTrancheProfile.TrancheId
                    Assert.AreEqual(summaryViewModel.AddedTrancheId, newTrancheViewModel.SelectedTrancheProfile.TrancheId);
                });
        }

    }
}
