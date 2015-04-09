// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExistingTrancheUnitTest.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Summary description for ExistingTrancheUnitTest
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
    using Insyston.Operations.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// The existing tranche unit test.
    /// </summary>
    [TestClass]
    public class ExistingTrancheUnitTest
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
        /// The test on step async tranche selected_ populate tranche profile.
        /// Expect IsChange change from true to false
        /// and ActionCommand
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncTrancheSelected_PopulateTrancheProfile()
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
                    Mock.Create<FundingDetailsViewModel>(
                        x =>
                            {
                                x.CallConstructor(() => new FundingDetailsViewModel(new FundingSummaryViewModel()));
                                x.SetBehavior(Behavior.CallOriginal);
                            });

                    existingTranche = Mock.Create<ExistingTrancheViewModel>(
                        x =>
                        {
                            x.CallConstructor(() => new ExistingTrancheViewModel(fundingSummary));
                            x.SetBehavior(Behavior.CallOriginal);
                        });

                    Mock.SetupStatic(typeof(FundingFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);

                    // Fake data
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
                    Mock.NonPublic.Arrange<Task>(existingTranche, "PopulateTrancheProfile").Returns(Task.Run(() => { }));
                    
                    existingTranche.IsChanged = true;
                    existingTranche.CanEdit = true;
                    existingTranche.SelectedTrancheProfile =
                        FakeDataFunding.FakeDataExistingTranche.FakeSelectedTrancheProfile();

                    // Run method
                    await existingTranche.OnStepAsync("TrancheSelected");

                    // Test
                    var actual = existingTranche.IsChanged;
                    var actual2 = existingTranche.ActionCommands;

                    Assert.IsFalse(actual);
                    Assert.AreEqual(null, actual2);
                });
        }

        /// <summary>
        /// The test on step async edit_ populate tranche profile.
        /// Expect IsChanged, BusyContent, IsBusy, CurrentStep is changed value
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncEdit_ExistingTrancheProfile()
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
                    FundingDetailsViewModel fundingDetails;

                    // Create mock
                    fundingSummary = Mock.Create<FundingSummaryViewModel>(Behavior.CallOriginal);
                    fundingDetails = Mock.Create<FundingDetailsViewModel>(
                        x =>
                            {
                                x.CallConstructor(() => new FundingDetailsViewModel(new FundingSummaryViewModel()));
                                x.SetBehavior(Behavior.CallOriginal);
                            });

                    existingTranche = Mock.Create<ExistingTrancheViewModel>(
                        x =>
                        {
                            x.CallConstructor(() => new ExistingTrancheViewModel(fundingSummary));
                            x.SetBehavior(Behavior.CallOriginal);
                        });

                    Mock.SetupStatic(typeof(FundingFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);

                    Mock.Arrange(() => fundingDetails.OnStepAsync("Edit"))
                        .IgnoreArguments()
                        .Returns(Task.Run(() => { }));

                    // Fake data
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
                    Mock.NonPublic.Arrange<Task<bool>>(existingTranche, "LockAsync").Returns(Task.Run(() =>
                        {
                            return true;
                        }));
                    Mock.NonPublic.Arrange<Task>(existingTranche, "PopulateTrancheProfile").Returns(Task.Run(() => { }));
                    Mock.NonPublic.Arrange<Task>(existingTranche, "BuildBaseQueryAsync").Returns(Task.Run(() => { }));
                    existingTranche.IsChanged = true;
                    existingTranche.BusyContent = "Processing";
                    existingTranche.IsBusy = true;

                    // existingTranche.CurrentStep = FundingDetailsViewModel.EnumStep.Edit;
                    var priva = new PrivateAccessor(existingTranche);
                    priva.SetProperty("CurrentStep", FundingDetailsViewModel.EnumStep.Start);

                    // Run method
                    // ReSharper disable once CSharpWarnings::CS4014
                    await existingTranche.OnStepAsync("Edit");

                    // Test
                    // Test IsChanged, BusyContent, IsBusy, CurrentStep is changed value
                    var actual = existingTranche.IsChanged;
                    var actual2 = existingTranche.BusyContent;
                    var actual3 = existingTranche.IsBusy;
                    var actual4 = existingTranche.CurrentStep;

                    Assert.IsFalse(actual && actual3);
                    Assert.AreEqual(string.Empty, actual2);
                    Assert.AreEqual(FundingDetailsViewModel.EnumStep.Edit, actual4);
                });
        }

        /// <summary>
        /// The test on step async cancel_ busy content changed.
        /// Expect BusyContent is changed
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncCancel_BusyContentChanged()
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
                    Mock.Create<FundingDetailsViewModel>(
                        x =>
                            {
                                x.CallConstructor(() => new FundingDetailsViewModel(new FundingSummaryViewModel()));
                                x.SetBehavior(Behavior.CallOriginal);
                            });

                    existingTranche = Mock.Create<ExistingTrancheViewModel>(
                        x =>
                        {
                            x.CallConstructor(() => new ExistingTrancheViewModel(fundingSummary));
                            x.SetBehavior(Behavior.CallOriginal);
                        });

                    Mock.SetupStatic(typeof(FundingFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                    Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);

                    // Fake data
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

                    existingTranche.OriginalTrancheStatus = TrancheStatus.Confirmed;
                    Mock.NonPublic.Arrange<bool>(existingTranche, "CheckIfUnSavedChanges").Returns(true);
                    Mock.NonPublic.Arrange<Task>(existingTranche, "UnLockAsync").Returns(Task.Run(() => { }));
                    Mock.NonPublic.Arrange<Task>(existingTranche, "PopulateTrancheProfile").Returns(Task.Run(() => { }));
                    Mock.NonPublic.Arrange<Task>(existingTranche, "BuildBaseQueryAsync").Returns(Task.Run(() => { }));

                    existingTranche.IsBusy = true;
                    existingTranche.BusyContent = "cancel";
                    existingTranche.SelectedTrancheProfile =
                        FakeDataFunding.FakeDataExistingTranche.FakeSelectedTrancheProfile();

                    // Run method
                    // ReSharper disable once CSharpWarnings::CS4014
                    await existingTranche.OnStepAsync("Cancel");

                    // Test isBusy, BusyContent is changed
                    var actual = existingTranche.IsBusy;
                    var actual2 = existingTranche.BusyContent;

                    Assert.IsFalse(actual);
                    Assert.AreEqual(string.Empty, actual2);
                });
        }

        /// <summary>
        /// The test on step async save_ validate save tranche profile.
        /// Expect isChange is changed 
        /// and HasError is false
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncSave_ValidateSaveTrancheProfile()
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
                   Mock.Create<FundingDetailsViewModel>(
                       x =>
                           {
                               x.CallConstructor(() => new FundingDetailsViewModel(new FundingSummaryViewModel()));
                               x.SetBehavior(Behavior.CallOriginal);
                           });

                   existingTranche = Mock.Create<ExistingTrancheViewModel>(
                       x =>
                       {
                           x.CallConstructor(() => new ExistingTrancheViewModel(fundingSummary));
                           x.SetBehavior(Behavior.CallOriginal);
                       });

                   Mock.SetupStatic(typeof(FundingFunctions), StaticConstructor.Mocked);
                   Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                   Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                   Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                   Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);

                   // Fake data
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

                   existingTranche.IncludedInTrancheContracts =
                       FakeDataFunding.FakeDataExistingTranche.FakeTrancheContractsSummary();

                   // existingTranche.SelectedTrancheProfile =
                   // FakeDataFunding.FakeDataExistingTranche.FakeSelectedTrancheProfile();
                   // existingTranche.OriginalTrancheStatus = TrancheStatus.Confirmed;
                   Mock.NonPublic.Arrange<Task>(existingTranche, "UnLockAsync").Returns(Task.Run(() => { }));
                   Mock.NonPublic.Arrange<Task>(existingTranche, "SaveAsync").Returns(Task.Run(() => { }));
                   Mock.Arrange(() => fundingSummary.OnStepAsync(FundingSummaryViewModel.EnumStep.TrancheSaved))
                       .Returns(Task.Run(() => { }));

                   existingTranche.IsChanged = true;

                   // Run method
                   await existingTranche.OnStepAsync("Save");

                   // Test isChange is changed
                   var actual = existingTranche.IsChanged;

                   // Test Validate no error
                   var actual2 = existingTranche.HasErrors;

                   Assert.IsFalse(actual && actual2);
               });
        }

        /// <summary>
        /// The test on step async status to confirmed_ confirm existing tranche.
        /// Expect CurrentStep, OriginalTrancheStatus is changed
        /// and HasError is false and TrancheStatusId is 147
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncStatusToConfirmed_ConfirmExistingTranche()
        {
            GeneralThreadAffineContext.Run(
               () =>
               {
                   // Mock enviroment for test
                   Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                       .IgnoreArguments()
                       .DoNothing();

                   FundingSummaryViewModel fundingSummary;
                   ExistingTrancheViewModel existingTranche;

                   // Create mock
                   fundingSummary = Mock.Create<FundingSummaryViewModel>(Behavior.CallOriginal);
                   existingTranche = Mock.Create<ExistingTrancheViewModel>(
                       x =>
                       {
                           x.CallConstructor(() => new ExistingTrancheViewModel(fundingSummary));
                           x.SetBehavior(Behavior.CallOriginal);
                       });
                   Mock.SetupStatic(typeof(FundingFunctions), StaticConstructor.Mocked);

                   // Fake current step
                   var priva = new PrivateAccessor(existingTranche);
                   priva.SetProperty("CurrentStep", FundingDetailsViewModel.EnumStep.Start);

                   // Fake TranCheStatus
                   existingTranche.OriginalTrancheStatus = TrancheStatus.Pending;

                   // Fake data
                   existingTranche.IncludedInTrancheContracts =
                       FakeDataFunding.FakeDataExistingTranche.FakeTrancheContractsSummary();                   
                   existingTranche.SelectedTrancheProfile =
                       FakeDataFunding.FakeDataExistingTranche.FakeSelectedTrancheProfile();
                   Mock.NonPublic.Arrange<Task>(existingTranche, "PopulateTrancheProfile").Returns(Task.Run(() => { }));
                   Mock.NonPublic.Arrange<Task>(existingTranche, "BuildBaseQueryAsync").Returns(Task.Run(() => { }));
                   Mock.Arrange(
                       () =>
                       FundingFunctions.ChangeStatusToConfirmedAsync(
                           existingTranche.SelectedTrancheProfile,
                           existingTranche.IncludedInTrancheContracts.ToList())).IgnoreArguments().Returns(Task.Run(() =>
                               {
                                   return true;
                               }));

                   // Run method
                   // ReSharper disable once CSharpWarnings::CS4014
                   existingTranche.OnStepAsync("StatusToConfirmed");

                   // Test CurrentStep, OriginalTrancheStatus is changed
                   var actual = existingTranche.CurrentStep;
                   var actual2 = existingTranche.OriginalTrancheStatus;

                   // Test HasError false and TrancheStatusId is 147
                   var actual3 = existingTranche.SelectedTrancheProfile.TrancheStatusId;
                   var actual4 = existingTranche.HasErrors;

                   Assert.AreEqual(FundingDetailsViewModel.EnumStep.StatusToConfirmed, actual);
                   Assert.AreEqual(TrancheStatus.Confirmed, actual2);
                   Assert.AreEqual(147, actual3);
                   Assert.IsFalse(actual4);
               });
        }

        /// <summary>
        /// The test on step async status to pending_ tranche status to pending.
        /// Expect BusyContent, CurrentStep, OriginalTrancheStatus is changed
        /// and TrancheStatusId is 146
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncStatusToPending_TrancheStatusSToPending()
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
                   Mock.Create<FundingDetailsViewModel>(
                       x =>
                           {
                               x.CallConstructor(() => new FundingDetailsViewModel(new FundingSummaryViewModel()));
                               x.SetBehavior(Behavior.CallOriginal);
                           });

                   existingTranche = Mock.Create<ExistingTrancheViewModel>(
                       x =>
                       {
                           x.CallConstructor(() => new ExistingTrancheViewModel(fundingSummary));
                           x.SetBehavior(Behavior.CallOriginal);
                       });

                   Mock.SetupStatic(typeof(FundingFunctions), StaticConstructor.Mocked);
                   Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                   Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                   Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                   Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);

                   // Fake current step
                   var priva = new PrivateAccessor(existingTranche);
                   priva.SetProperty("CurrentStep", FundingDetailsViewModel.EnumStep.StatusToConfirmed);

                   // Fake TranCheStatus
                   existingTranche.OriginalTrancheStatus = TrancheStatus.Confirmed;

                   // Fake data
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

                   existingTranche.IncludedInTrancheContracts =
                       FakeDataFunding.FakeDataExistingTranche.FakeTrancheContractsSummary();
                   existingTranche.SelectedTrancheProfile =
                       FakeDataFunding.FakeDataExistingTranche.FakeSelectedTrancheProfile();
                   Mock.NonPublic.Arrange<Task>(existingTranche, "PopulateTrancheProfile").Returns(Task.Run(() => { }));
                   Mock.NonPublic.Arrange<Task<bool>>(existingTranche, "LockAsync").Returns(Task.Run(() =>
                       {
                           return true;
                       }));
                   Mock.NonPublic.Arrange<Task>(existingTranche, "UnLockAsync").Returns(Task.Run(() => { }));
                   Mock.NonPublic.Arrange<Task>(existingTranche, "BuildBaseQueryAsync").Returns(Task.Run(() => { }));
                   Mock.Arrange(
                       () =>
                       FundingFunctions.ChangeStatusToPendingAsync(
                           existingTranche.SelectedTrancheProfile)).IgnoreArguments().Returns(Task.Run(() =>
                           {
                               return true;
                           }));

                   existingTranche.BusyContent = "Pending";
                   existingTranche.IsChanged = true;
                   existingTranche.IsBusy = true;

                   // Run method
                   await existingTranche.OnStepAsync("StatusToPending");

                   // Test IsBusy, BusyContent, CurrentStep, OriginalTrancheStatus is changed
                   var actual = existingTranche.IsBusy;
                   var actual2 = existingTranche.BusyContent;
                   var actual3 = existingTranche.CurrentStep;
                   var actual4 = existingTranche.OriginalTrancheStatus;

                   // Test TrancheStatusId is 146
                   var actual5 = existingTranche.SelectedTrancheProfile.TrancheStatusId;

                   Assert.IsFalse(actual);
                   Assert.AreEqual(string.Empty, actual2);
                   Assert.AreEqual(FundingDetailsViewModel.EnumStep.StatusToPending, actual3);
                   Assert.AreEqual(TrancheStatus.Pending, actual4);
                   Assert.AreEqual(146, actual5);
               });
        }

        /// <summary>
        /// The test on step async status to funded_ tranche status to fund.
        /// Expect IsBusy, BusyContent, CurrentStep, OriginalTrancheStatus is changed
        /// and TrancheStatusId is 148, HasError is false
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncStatusToFunded_ValidateTrancheStatusToFund()
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
                   Mock.Create<FundingDetailsViewModel>(
                       x =>
                           {
                               x.CallConstructor(() => new FundingDetailsViewModel(new FundingSummaryViewModel()));
                               x.SetBehavior(Behavior.CallOriginal);
                           });

                   existingTranche = Mock.Create<ExistingTrancheViewModel>(
                       x =>
                       {
                           x.CallConstructor(() => new ExistingTrancheViewModel(fundingSummary));
                           x.SetBehavior(Behavior.CallOriginal);
                       });

                   Mock.SetupStatic(typeof(FundingFunctions), StaticConstructor.Mocked);
                   Mock.SetupStatic(typeof(SystemConstantFunctions), StaticConstructor.Mocked);
                   Mock.SetupStatic(typeof(FinanceTypeFunctions), StaticConstructor.Mocked);
                   Mock.SetupStatic(typeof(OperationsEntityFunctions), StaticConstructor.Mocked);
                   Mock.SetupStatic(typeof(DirectDebitProfileFunctions), StaticConstructor.Mocked);

                   // Fake current step
                   var priva = new PrivateAccessor(existingTranche);
                   priva.SetProperty("CurrentStep", FundingDetailsViewModel.EnumStep.StatusToConfirmed);

                   // Fake TranCheStatus
                   existingTranche.OriginalTrancheStatus = TrancheStatus.Confirmed;

                   // Fake data
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

                   existingTranche.IncludedInTrancheContracts =
                       FakeDataFunding.FakeDataExistingTranche.FakeTrancheContractsSummary();
                   existingTranche.SelectedTrancheProfile =
                       FakeDataFunding.FakeDataExistingTranche.FakeSelectedTrancheProfile();
                   Mock.NonPublic.Arrange<Task>(existingTranche, "PopulateTrancheProfile").Returns(Task.Run(() => { }));
                   Mock.NonPublic.Arrange<Task<bool>>(existingTranche, "LockAsync").Returns(Task.Run(() =>
                   {
                       return true;
                   }));
                   Mock.NonPublic.Arrange<Task>(existingTranche, "UnLockAsync").Returns(Task.Run(() => { }));
                   Mock.NonPublic.Arrange<Task>(existingTranche, "BuildBaseQueryAsync").Returns(Task.Run(() => { }));
                   Mock.Arrange(
                       () =>
                       FundingFunctions.ChangeStatusToFundedAsync(existingTranche.SelectedTrancheProfile, existingTranche.IncludedInTrancheContracts.ToList())).IgnoreArguments().Returns(Task.Run(() =>
                           {
                               return true;
                           }));

                   existingTranche.BusyContent = "Pending";
                   existingTranche.IsBusy = true;

                   // Run method
                   await existingTranche.OnStepAsync("StatusToFunded");

                   // Test IsBusy, BusyContent, CurrentStep, OriginalTrancheStatus is changed
                   var actual = existingTranche.IsBusy;
                   var actual2 = existingTranche.BusyContent;
                   var actual3 = existingTranche.CurrentStep;
                   var actual4 = existingTranche.OriginalTrancheStatus;

                   // Test TrancheStatusId is 148 and HasError is false
                   var actual5 = existingTranche.HasErrors;
                   var actual6 = existingTranche.SelectedTrancheProfile.TrancheStatusId;

                   Assert.IsFalse(actual);
                   Assert.AreEqual(string.Empty, actual2);
                   Assert.AreEqual(FundingDetailsViewModel.EnumStep.StatusToFunded, actual3);
                   Assert.AreEqual(TrancheStatus.Funded, actual4);
                   Assert.IsFalse(actual5);
                   Assert.AreEqual(148, actual6);
               });
        }
    }
}
