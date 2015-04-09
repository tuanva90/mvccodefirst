// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDataFunding.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The fake data funding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Funding.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Business.Funding.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The fake data funding.
    /// </summary>
    internal class FakeDataFunding
    {
        /// <summary>
        /// The fake data funding summary.
        /// </summary>
        internal class FakeDataFundingSummary
        {
            /// <summary>
            /// The fake funding summary.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<FundingSummary>> FakeFundingSummaryList()
            {
                return await Task.Run(
                    () =>
                    {
                        List<FundingSummary> fundings = new List<FundingSummary>();

                        fundings.Add(
                            new FundingSummary
                                {
                                    FunderName = "Funder A",
                                    NodeId = 655,
                                    TrancheDate = new DateTime(2013, 12, 24),
                                    TrancheId = 1,
                                    TrancheStatus = TrancheStatus.Pending
                                });
                        fundings.Add(
                            new FundingSummary
                                {
                                    FunderName = "Funder A",
                                    NodeId = 656,
                                    TrancheDate = new DateTime(2013, 12, 24),
                                    TrancheId = 2,
                                    TrancheStatus = TrancheStatus.Confirmed
                                });

                        return fundings;
                    });
            }

            /// <summary>
            /// The fake tranche profile.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<FundingTranche> FakeFundingTranche()
            {
                return await Task.Run(
                    () =>
                    {
                        return new FundingTranche
                                   {
                                       TrancheId = 1,
                                       AssumedRate = 10,
                                       LossReserve = 8,
                                       InstalmentType = -1,
                                       NoOfPaymentRetained = 0,
                                       CashFlowResidual = false,
                                       PaymentRetained = false,
                                       TrancheStatusId = 146,
                                       Term = 12,
                                       NodeId = 655,
                                       EntityId = 598,
                                       CalculationMethod = true,
                                       TermOperator = "=",
                                       TrancheDate = new DateTime(2013, 1, 1),
                                       CalculateHoldingCost = true,
                                       DDProfile = -1
                                   };
                    });
            }

            /// <summary>
            /// The fake selected summary.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<FundingSummary> FakeSelectedTranche()
            {
                return await Task.Run(
                    () =>
                    {
                        return new FundingSummary
                                   {
                                       FunderName = "Funder A",
                                       NodeId = 655,
                                       TrancheDate = new DateTime(2013, 12, 24),
                                       TrancheId = 1,
                                       TrancheStatus = TrancheStatus.Pending
                                   };
                    });
            }

            /// <summary>
            /// The fake select list.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<SelectListViewModel> FakeSelectFunder()
            {
                return await Task.Run(
                    () => { return new SelectListViewModel { IsSelected = true, Id = 10, Text = "TranceSave" }; });
            }
        }

        /// <summary>
        /// The fake data existing tranche.
        /// </summary>
        internal class FakeDataExistingTranche
        {
            /// <summary>
            /// The fake data populate.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<int>> FakeDataPopulate()
            {
                return await Task.Run(
                    () =>
                    {
                        List<int> datalist = new List<int>();
                        datalist.Add(1);
                        datalist.Add(2);
                        datalist.Add(3);
                        datalist.Add(4);
                        datalist.Add(5);
                        datalist.Add(6);
                        return new List<int>(datalist);
                    });
            }

            /// <summary>
            /// The fake selected tranche profile.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static FundingTranche FakeSelectedTrancheProfile()
            {
                return new FundingTranche
                {
                    TrancheId = 1,
                    AssumedRate = 10,
                    LossReserve = 8,
                    InstalmentType = -1,
                    NoOfPaymentRetained = 0,
                    CashFlowResidual = false,
                    PaymentRetained = false,
                    TrancheStatusId = 146,
                    Term = 12,
                    NodeId = 655,
                    EntityId = 598,
                    CalculationMethod = true,
                    TermOperator = "=",
                    TrancheDate = new DateTime(2013, 1, 1),
                    CalculateHoldingCost = true,
                    DDProfile = -1
                };
            }

            /// <summary>
            /// The fake funding statuses.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableModelCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableModelCollection<SelectListViewModel> FakeAllFundingStatuses()
            {
                List<SelectListViewModel> fundingStatuses;

                fundingStatuses = new List<SelectListViewModel>();
                fundingStatuses.Add(new SelectListViewModel { Text = FundingStatus.None.ToString(), Id = (int)FundingStatus.None });
                fundingStatuses.Add(new SelectListViewModel { Text = FundingStatus.Confirmed.ToString(), Id = (int)FundingStatus.Confirmed });
                fundingStatuses.Add(new SelectListViewModel { Text = FundingStatus.Funded.ToString(), Id = (int)FundingStatus.Funded });
                fundingStatuses.Add(new SelectListViewModel { Text = FundingStatus.Indicative.ToString(), Id = (int)FundingStatus.Indicative, IsSelected = true });
                fundingStatuses.Add(new SelectListViewModel { Text = FundingStatus.Terminated.ToString(), Id = (int)FundingStatus.Terminated });

                return new ObservableModelCollection<SelectListViewModel>(fundingStatuses);
            }

            /// <summary>
            /// The fake all finance types.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableModelCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableModelCollection<SelectListViewModel> FakeAllFinanceTypes()
            {
                List<SelectListViewModel> financeTypes;

                financeTypes = new List<SelectListViewModel>();
                financeTypes.Add(new SelectListViewModel { Text = "Operating Lease", Id = 1 });
                financeTypes.Add(new SelectListViewModel { Text = "Finance Lease", Id = 2 });
                financeTypes.Add(new SelectListViewModel { Text = "Hire Purchase", Id = 3 });
                financeTypes.Add(new SelectListViewModel { Text = "Hire Buy-Rebate Option", Id = 6 });
                financeTypes.Add(new SelectListViewModel { Text = "Hire with Option to Buy", Id = 7 });
                financeTypes.Add(new SelectListViewModel { Text = "Chattel Mortgage", Id = 8 });
                financeTypes.Add(new SelectListViewModel { Text = "Rental Lease", Id = 9 });

                return new ObservableModelCollection<SelectListViewModel>(financeTypes);
            }

            /// <summary>
            /// The fake all funder.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableModelCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableModelCollection<SelectListViewModel> FakeAllFunder()
            {
                List<SelectListViewModel> funder;

                funder = new List<SelectListViewModel>();
                funder.Add(new SelectListViewModel { Text = PaymentFrequency.Annual.ToString(), Id = (int)PaymentFrequency.Annual });
                funder.Add(new SelectListViewModel { Text = PaymentFrequency.Monthly.ToString(), Id = (int)PaymentFrequency.Monthly });
                funder.Add(new SelectListViewModel { Text = PaymentFrequency.Quarterly.ToString(), Id = (int)PaymentFrequency.Quarterly });
                funder.Add(new SelectListViewModel { Text = PaymentFrequency.SemiAnnual.ToString(), Id = (int)PaymentFrequency.SemiAnnual });
                funder.Add(new SelectListViewModel { Text = PaymentFrequency.Weekly.ToString(), Id = (int)PaymentFrequency.Weekly });

                return new ObservableModelCollection<SelectListViewModel>(funder);
            }

            /// <summary>
            /// The fake all suppliers.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableModelCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableModelCollection<SelectListViewModel> FakeAllSuppliers()
            {
                List<SelectListViewModel> suppliers;

                suppliers = new List<SelectListViewModel>();
                suppliers.Add(new SelectListViewModel { Text = "Hitachi Construction Machinery (Revesby)", Id = 59 });
                suppliers.Add(new SelectListViewModel { Text = "HItachi Construction Machinery (Archerfield)", Id = 62 });
                suppliers.Add(new SelectListViewModel { Text = "Hitachi Construction Machinery (Cavan-SA)", Id = 234 });
                suppliers.Add(new SelectListViewModel { Text = "St Barbara Limited", Id = 603 });
                suppliers.Add(new SelectListViewModel { Text = "Weldit Fabrications Pty Ltd", Id = 633 });

                return new ObservableModelCollection<SelectListViewModel>(suppliers);
            }

            /// <summary>
            /// The fake all internal companies.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableModelCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableModelCollection<SelectListViewModel> FakeAllInternalCompanies()
            {
                List<SelectListViewModel> internalCompanies;

                internalCompanies = new List<SelectListViewModel>();
                internalCompanies.Add(new SelectListViewModel { Text = "Marubeni Equipment Finance (Oceania) Pty Ltd", Id = 58 });
                internalCompanies.Add(new SelectListViewModel { Text = "AABB funders", Id = 659 });
                internalCompanies.Add(new SelectListViewModel { Text = "AA internal and Client", Id = 668 });
                internalCompanies.Add(new SelectListViewModel { Text = "MEF  Agency", Id = 598 });

                return new ObservableModelCollection<SelectListViewModel>(internalCompanies);
            }

            /// <summary>
            /// The fake tranche contracts.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<TrancheContractSummary>> FakeTrancheContracts()
            {
                return await Task.Run(
                    () =>
                    {
                        List<TrancheContractSummary> trancheContracts;

                        trancheContracts = new List<TrancheContractSummary>();

                        trancheContracts.Add(
                            new TrancheContractSummary
                                {
                                    ContractId = 1011,
                                    ClientName = "B & D Brouff Earthmoving Pty Ltd",
                                    ContractPrefix = "FQQ",
                                    ContractReference = "FQQ-1011",
                                    FinanceTypeId = 8,
                                    FirmTermDate = new DateTime(2014, 6, 24),
                                    FrequencyId = 23,
                                    FundingStartDate = new DateTime(2014, 2, 24),
                                    FundingStatus = FundingStatus.None,
                                    GrossAmountOverDue = 9000,
                                    InstalmentType = InstalmentType.Advance,
                                    InternalCompanyId = 58,
                                    InvestmentBalance = 20000,
                                    IsExisting = true,
                                    IsValid = true,
                                    NumberOfPayments = 6,
                                    StartDate = new DateTime(2014, 12, 24),
                                    Term = 6,
                                    SupplierId = -1
                                });

                        trancheContracts.Add(
                            new TrancheContractSummary
                                {
                                    ContractId = 2120,
                                    ClientName = "PW & LA Simpson",
                                    ContractPrefix = "MEF",
                                    ContractReference = "MEF-2120",
                                    FinanceTypeId = 8,
                                    FirmTermDate = new DateTime(2017, 8, 27),
                                    FrequencyId = 23,
                                    FundingStartDate = new DateTime(2014, 1, 27),
                                    FundingStatus = FundingStatus.Indicative,
                                    GrossAmountOverDue = 19000,
                                    InstalmentType = InstalmentType.Advance,
                                    InternalCompanyId = 58,
                                    InvestmentBalance = 80000,
                                    IsExisting = false,
                                    IsValid = true,
                                    NumberOfPayments = 60,
                                    StartDate = new DateTime(2012, 8, 27),
                                    Term = 60,
                                    SupplierId = 234
                                });

                        trancheContracts.Add(
                            new TrancheContractSummary
                                {
                                    ContractId = 3343,
                                    ClientName = "Excavators Australia Plant Hire Pty Ltd",
                                    ContractPrefix = "FQQ",
                                    ContractReference = "MEF-3343",
                                    FinanceTypeId = 3,
                                    FirmTermDate = new DateTime(2017, 2, 23),
                                    FrequencyId = 23,
                                    FundingStartDate = new DateTime(2014, 2, 23),
                                    FundingStatus = FundingStatus.Indicative,
                                    GrossAmountOverDue = 2000,
                                    InstalmentType = InstalmentType.Arrears,
                                    InternalCompanyId = 62,
                                    InvestmentBalance = 20000,
                                    IsExisting = false,
                                    IsValid = true,
                                    NumberOfPayments = 60,
                                    StartDate = new DateTime(2012, 2, 23),
                                    Term = 60,
                                    SupplierId = 62
                                });

                        trancheContracts.Add(
                            new TrancheContractSummary
                                {
                                    ContractId = 5445,
                                    ClientName = "Barham Corporation Pty Ltd",
                                    ContractPrefix = "FQQ",
                                    ContractReference = "FQQ-5445",
                                    FinanceTypeId = 8,
                                    FirmTermDate = new DateTime(2015, 1, 8),
                                    FrequencyId = 24,
                                    FundingStartDate = new DateTime(2014, 4, 8),
                                    FundingStatus = FundingStatus.Indicative,
                                    GrossAmountOverDue = 2500,
                                    InstalmentType = InstalmentType.Arrears,
                                    InternalCompanyId = 58,
                                    InvestmentBalance = 12000,
                                    IsExisting = false,
                                    IsValid = true,
                                    NumberOfPayments = 4,
                                    StartDate = new DateTime(2014, 1, 8),
                                    Term = 12,
                                    SupplierId = -1
                                });

                        return trancheContracts;
                    });
            }

            /// <summary>
            /// The fake tranche contracts summary.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>ObservableModelCollection</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static ObservableModelCollection<TrancheContractSummary> FakeTrancheContractsSummary()
            {
                List<TrancheContractSummary> trancheContracts;

                trancheContracts = new List<TrancheContractSummary>();

                trancheContracts.Add(
                    new TrancheContractSummary
                    {
                        ContractId = 1011,
                        ClientName = "B & D Brouff Earthmoving Pty Ltd",
                        ContractPrefix = "FQQ",
                        ContractReference = "FQQ-1011",
                        FinanceTypeId = 8,
                        FirmTermDate = new DateTime(2014, 6, 24),
                        FrequencyId = 23,
                        FundingStartDate = new DateTime(2014, 2, 24),
                        FundingStatus = FundingStatus.None,
                        GrossAmountOverDue = 9000,
                        InstalmentType = InstalmentType.Advance,
                        InternalCompanyId = 58,
                        InvestmentBalance = 20000,
                        IsExisting = true,
                        IsValid = true,
                        NumberOfPayments = 6,
                        StartDate = new DateTime(2014, 12, 24),
                        Term = 6,
                        SupplierId = -1
                    });

                trancheContracts.Add(
                    new TrancheContractSummary
                    {
                        ContractId = 2120,
                        ClientName = "PW & LA Simpson",
                        ContractPrefix = "MEF",
                        ContractReference = "MEF-2120",
                        FinanceTypeId = 8,
                        FirmTermDate = new DateTime(2017, 8, 27),
                        FrequencyId = 23,
                        FundingStartDate = new DateTime(2014, 1, 27),
                        FundingStatus = FundingStatus.Indicative,
                        GrossAmountOverDue = 19000,
                        InstalmentType = InstalmentType.Advance,
                        InternalCompanyId = 58,
                        InvestmentBalance = 80000,
                        IsExisting = false,
                        IsValid = true,
                        NumberOfPayments = 60,
                        StartDate = new DateTime(2012, 8, 27),
                        Term = 60,
                        SupplierId = 234
                    });

                trancheContracts.Add(
                    new TrancheContractSummary
                    {
                        ContractId = 3343,
                        ClientName = "Excavators Australia Plant Hire Pty Ltd",
                        ContractPrefix = "FQQ",
                        ContractReference = "MEF-3343",
                        FinanceTypeId = 3,
                        FirmTermDate = new DateTime(2017, 2, 23),
                        FrequencyId = 23,
                        FundingStartDate = new DateTime(2014, 2, 23),
                        FundingStatus = FundingStatus.Indicative,
                        GrossAmountOverDue = 2000,
                        InstalmentType = InstalmentType.Arrears,
                        InternalCompanyId = 62,
                        InvestmentBalance = 20000,
                        IsExisting = false,
                        IsValid = true,
                        NumberOfPayments = 60,
                        StartDate = new DateTime(2012, 2, 23),
                        Term = 60,
                        SupplierId = 62
                    });

                trancheContracts.Add(
                    new TrancheContractSummary
                    {
                        ContractId = 5445,
                        ClientName = "Barham Corporation Pty Ltd",
                        ContractPrefix = "FQQ",
                        ContractReference = "FQQ-5445",
                        FinanceTypeId = 8,
                        FirmTermDate = new DateTime(2015, 1, 8),
                        FrequencyId = 24,
                        FundingStartDate = new DateTime(2014, 4, 8),
                        FundingStatus = FundingStatus.Indicative,
                        GrossAmountOverDue = 2500,
                        InstalmentType = InstalmentType.Arrears,
                        InternalCompanyId = 58,
                        InvestmentBalance = 12000,
                        IsExisting = true,
                        IsValid = true,
                        NumberOfPayments = 4,
                        StartDate = new DateTime(2014, 1, 8),
                        Term = 12,
                        SupplierId = -1
                    });

                return new ObservableModelCollection<TrancheContractSummary>(trancheContracts);
            }
        }

        /// <summary>
        /// The fake data funding detail.
        /// </summary>
        internal class FakeDataFundingDetail
        {
            /// <summary>
            /// The fake direct debit profiles.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<IEnumerable<DirectDebitProfile>> FakeDirectDebitProfiles()
            {
                return await Task.Run(
                    () =>
                    {
                        List<DirectDebitProfile> result;

                        result = new List<DirectDebitProfile>
                                         {
                                             new DirectDebitProfile
                                                 {
                                                     ID = 1,
                                                     DDRecipientId = 1,
                                                     DDRemitterId = 1,
                                                     DDUserId = 1,
                                                     ProfileName =
                                                         "DDB Profile A",
                                                     SystemDefault = true
                                                 }
                                         };
                        return result;
                    });
            }

            /// <summary>
            /// The fake system constants.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<SystemConstant>> FakeSystemConstants()
            {
                return await Task.Run(
                    () =>
                    {
                        List<SystemConstant> result = new List<SystemConstant>();

                        result.Add(
                            new SystemConstant { SystemConstantId = 1, UserDescription = "System Constant 1" });
                        result.Add(
                            new SystemConstant { SystemConstantId = 2, UserDescription = "System Constant 2" });
                        result.Add(
                            new SystemConstant { SystemConstantId = 3, UserDescription = "System Constant 3" });
                        result.Add(
                            new SystemConstant { SystemConstantId = 4, UserDescription = "System Constant 4" });
                        result.Add(
                            new SystemConstant { SystemConstantId = 5, UserDescription = "System Constant 5" });
                        result.Add(
                            new SystemConstant { SystemConstantId = 6, UserDescription = "System Constant 6" });

                        return result;
                    });
            }

            /// <summary>
            /// The fake raw finance types.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<FinanceType>> FakeRawFinanceTypes()
            {
                return await Task.Run(
                    () =>
                    {
                        List<FinanceType> financeTypes;

                        financeTypes = new List<FinanceType>();
                        financeTypes.Add(new FinanceType { Description = "Operating Lease", FinanceTypeId = 1 });
                        financeTypes.Add(new FinanceType { Description = "Finance Lease", FinanceTypeId = 2 });
                        financeTypes.Add(new FinanceType { Description = "Hire Purchase", FinanceTypeId = 3 });
                        financeTypes.Add(
                            new FinanceType { Description = "Hire Buy-Rebate Option", FinanceTypeId = 6 });
                        financeTypes.Add(
                            new FinanceType { Description = "Hire with Option to Buy", FinanceTypeId = 7 });
                        financeTypes.Add(new FinanceType { Description = "Chattel Mortgage", FinanceTypeId = 8 });
                        financeTypes.Add(new FinanceType { Description = "Rental Lease", FinanceTypeId = 9 });

                        return financeTypes;
                    });
            }

            /// <summary>
            /// The fake raw internal companies.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<vwEntityRelation>> FakeRawInternalCompanies()
            {
                return await Task.Run(
                    () =>
                    {
                        List<vwEntityRelation> internalCompanies;

                        internalCompanies = new List<vwEntityRelation>();
                        internalCompanies.Add(
                            new vwEntityRelation
                                {
                                    NodeName = "Marubeni Equipment Finance (Oceania) Pty Ltd",
                                    NodeId = 58
                                });
                        internalCompanies.Add(new vwEntityRelation { NodeName = "AABB funders", NodeId = 659 });
                        internalCompanies.Add(
                            new vwEntityRelation { NodeName = "AA internal and Client", NodeId = 668 });
                        internalCompanies.Add(new vwEntityRelation { NodeName = "MEF  Agency", NodeId = 598 });

                        return internalCompanies;
                    });
            }

            /// <summary>
            /// The fake raw funders.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<vwEntityRelation>> FakeRawFunders()
            {
                return await Task.Run(
                    () =>
                    {
                        List<vwEntityRelation> funders;

                        funders = new List<vwEntityRelation>();
                        funders.Add(
                            new vwEntityRelation
                                {
                                    NodeName = "Marubeni Equipment Finance (Oceania) Pty Ltd",
                                    NodeId = 58
                                });
                        funders.Add(new vwEntityRelation { NodeName = "AABB funders", NodeId = 659 });
                        funders.Add(new vwEntityRelation { NodeName = "AA internal and Client", NodeId = 668 });
                        funders.Add(new vwEntityRelation { NodeName = "MEF  Agency", NodeId = 598 });

                        return funders;
                    });
            }

            /// <summary>
            /// The fake raw suppliers.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            internal static async Task<List<vwEntityRelation>> FakeRawSuppliers()
            {
                return await Task.Run(
                    () =>
                    {
                        List<vwEntityRelation> suppliers;
                        suppliers = new List<vwEntityRelation>();
                        suppliers.Add(
                            new vwEntityRelation
                                {
                                    NodeName = "Hitachi Construction Machinery (Revesby)",
                                    NodeId = 59
                                });
                        suppliers.Add(
                            new vwEntityRelation
                                {
                                    NodeName = "HItachi Construction Machinery (Archerfield)",
                                    NodeId = 62
                                });
                        suppliers.Add(
                            new vwEntityRelation
                                {
                                    NodeName = "Hitachi Construction Machinery (Cavan-SA)",
                                    NodeId = 234
                                });
                        suppliers.Add(new vwEntityRelation { NodeName = "St Barbara Limited", NodeId = 603 });
                        suppliers.Add(
                            new vwEntityRelation { NodeName = "Weldit Fabrications Pty Ltd", NodeId = 633 });
                        return suppliers;
                    });
            }

            /// <summary>
            /// The fake tranche profile.
            /// </summary>
            /// <returns>
            /// The <see cref="FundingTranche"/>.
            /// </returns>
            internal static FundingTranche FakeTrancheProfile()
            {
                return new FundingTranche
                {
                    TrancheId = 1,
                    AssumedRate = 10,
                    LossReserve = 8,
                    InstalmentType = -1,
                    NoOfPaymentRetained = 0,
                    CashFlowResidual = false,
                    PaymentRetained = false,
                    TrancheStatusId = 146,
                    Term = 12,
                    NodeId = 655,
                    EntityId = 598,
                    CalculationMethod = true,
                    TermOperator = "=",
                    TrancheDate = new DateTime(2020, 12, 31),
                    CalculateHoldingCost = true,
                    DDProfile = -1
                };
            }

            /// <summary>
            /// The fake tranche contracts.
            /// </summary>
            /// <returns>
            /// The <see>
            ///         <cref>List</cref>
            ///     </see>
            ///     .
            /// </returns>
            internal static List<TrancheContractSummary> FakeTrancheContracts()
            {
                List<TrancheContractSummary> trancheContracts;

                trancheContracts = new List<TrancheContractSummary>();

                trancheContracts.Add(new TrancheContractSummary
                {
                    ContractId = 1011,
                    ClientName = "B & D Brouff Earthmoving Pty Ltd",
                    ContractPrefix = "FQQ",
                    ContractReference = "FQQ-1011",
                    FinanceTypeId = 8,
                    FirmTermDate = new DateTime(2014, 6, 24),
                    FrequencyId = 23,
                    FundingStartDate = new DateTime(2014, 2, 24),
                    FundingStatus = FundingStatus.None,
                    GrossAmountOverDue = 9000,
                    InstalmentType = InstalmentType.Advance,
                    InternalCompanyId = 58,
                    InvestmentBalance = 20000,
                    IsExisting = true,
                    IsSelected = true,
                    IsValid = true,
                    NumberOfPayments = 6,
                    StartDate = new DateTime(2014, 2, 24),
                    Term = 6,
                    SupplierId = -1,
                    LastPaymentDate = new DateTime(2020, 12, 31)
                });

                trancheContracts.Add(new TrancheContractSummary
                {
                    ContractId = 2120,
                    ClientName = "PW & LA Simpson",
                    ContractPrefix = "MEF",
                    ContractReference = "MEF-2120",
                    FinanceTypeId = 8,
                    FirmTermDate = new DateTime(2017, 8, 27),
                    FrequencyId = 23,
                    FundingStartDate = new DateTime(2014, 1, 27),
                    FundingStatus = FundingStatus.Indicative,
                    GrossAmountOverDue = 19000,
                    InstalmentType = InstalmentType.Advance,
                    InternalCompanyId = 58,
                    InvestmentBalance = 80000,
                    IsExisting = false,
                    IsSelected = true,
                    IsValid = true,
                    NumberOfPayments = 60,
                    StartDate = new DateTime(2012, 8, 27),
                    Term = 60,
                    SupplierId = 234,
                    LastPaymentDate = new DateTime(2020, 12, 31)
                });

                trancheContracts.Add(new TrancheContractSummary
                {
                    ContractId = 3343,
                    ClientName = "Excavators Australia Plant Hire Pty Ltd",
                    ContractPrefix = "FQQ",
                    ContractReference = "MEF-3343",
                    FinanceTypeId = 3,
                    FirmTermDate = new DateTime(2017, 2, 23),
                    FrequencyId = 23,
                    FundingStartDate = new DateTime(2014, 2, 23),
                    FundingStatus = FundingStatus.Indicative,
                    GrossAmountOverDue = 2000,
                    InstalmentType = InstalmentType.Arrears,
                    InternalCompanyId = 62,
                    InvestmentBalance = 20000,
                    IsExisting = false,
                    IsSelected = false,
                    IsValid = true,
                    NumberOfPayments = 60,
                    StartDate = new DateTime(2012, 2, 23),
                    Term = 60,
                    SupplierId = 62,
                    LastPaymentDate = new DateTime(2020, 12, 31)
                });

                trancheContracts.Add(new TrancheContractSummary
                {
                    ContractId = 5445,
                    ClientName = "Barham Corporation Pty Ltd",
                    ContractPrefix = "FQQ",
                    ContractReference = "FQQ-5445",
                    FinanceTypeId = 8,
                    FirmTermDate = new DateTime(2015, 1, 8),
                    FrequencyId = 24,
                    FundingStartDate = new DateTime(2014, 4, 8),
                    FundingStatus = FundingStatus.Indicative,
                    GrossAmountOverDue = 2500,
                    InstalmentType = InstalmentType.Arrears,
                    InternalCompanyId = 58,
                    InvestmentBalance = 12000,
                    IsExisting = false,
                    IsSelected = false,
                    IsValid = true,
                    NumberOfPayments = 4,
                    StartDate = new DateTime(2014, 1, 8),
                    Term = 12,
                    SupplierId = -1,
                    LastPaymentDate = new DateTime(2020, 12, 31)
                });

                return trancheContracts;
            }
        }
    }
}
