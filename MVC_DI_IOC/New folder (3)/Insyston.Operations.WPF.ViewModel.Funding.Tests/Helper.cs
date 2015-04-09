using System;
using System.Collections.Generic;
using System.Linq;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Business.Funding.Model;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModel.Common.Model;

namespace Insyston.Operations.WPF.ViewModel.Funding.Tests
{
    internal class Helper
    {
        internal static List<FundingSummary> FakeFundingSummary()
        {
            List<FundingSummary> fundings = new List<FundingSummary>();

            fundings.Add(new FundingSummary
            {
                FunderName = "Funder A",
                NodeId = 655,
                TrancheDate = new DateTime(2013, 12, 24),
                TrancheId = 1,
                TrancheStatus = Business.Common.Enums.TrancheStatus.Pending
            });
            fundings.Add(new FundingSummary
            {
                FunderName = "Funder A",
                NodeId = 656,
                TrancheDate = new DateTime(2013, 12, 24),
                TrancheId = 2,
                TrancheStatus = Business.Common.Enums.TrancheStatus.Confirmed
            });

            return fundings;
        }

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
                IsValid = true,
                NumberOfPayments = 6,
                StartDate = new DateTime(2014, 12, 24),
                Term = 6,
                SupplierId = -1
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
                IsValid = true,
                NumberOfPayments = 60,
                StartDate = new DateTime(2012, 8, 27),
                Term = 60,
                SupplierId = 234
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
                IsValid = true,
                NumberOfPayments = 60,
                StartDate = new DateTime(2012, 2, 23),
                Term = 60,
                SupplierId = 62
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
                IsValid = true,
                NumberOfPayments = 4,
                StartDate = new DateTime(2014, 1, 8),
                Term = 12,
                SupplierId = -1
            });

            return trancheContracts;
        }

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
                TrancheDate = new DateTime(2013, 1, 1),
                CalculateHoldingCost = true,
                DDProfile = -1
            };
        }

        internal static List<SystemConstant> FakeSystemConstants()
        {
            List<SystemConstant> result = new List<SystemConstant>();

            result.Add(new SystemConstant { SystemConstantId = 1, UserDescription = "System Constant 1" });
            result.Add(new SystemConstant { SystemConstantId = 2, UserDescription = "System Constant 2" });
            result.Add(new SystemConstant { SystemConstantId = 3, UserDescription = "System Constant 3" });
            result.Add(new SystemConstant { SystemConstantId = 4, UserDescription = "System Constant 4" });
            result.Add(new SystemConstant { SystemConstantId = 5, UserDescription = "System Constant 5" });
            result.Add(new SystemConstant { SystemConstantId = 6, UserDescription = "System Constant 6" });

            return result;
        }

        internal static ObservableModelCollection<SelectListViewModel> FakeFundingStatuses()
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

        internal static List<SystemConstant> FakeTrancheStatuses()
        {
            List<SystemConstant> trancheStatuses;

            trancheStatuses = new List<SystemConstant>();
            trancheStatuses.Add(new SystemConstant { UserDescription = TrancheStatus.Pending.ToString(), SystemConstantId = (int)TrancheStatus.Pending });
            trancheStatuses.Add(new SystemConstant { UserDescription = TrancheStatus.Confirmed.ToString(), SystemConstantId = (int)TrancheStatus.Confirmed });
            trancheStatuses.Add(new SystemConstant { UserDescription = TrancheStatus.Funded.ToString(), SystemConstantId = (int)TrancheStatus.Funded });

            return trancheStatuses;
        }

        internal static List<FinanceType> FakeRawFinanceTypes()
        {
            List<FinanceType> financeTypes;

            financeTypes = new List<FinanceType>();
            financeTypes.Add(new FinanceType { Description = "Operating Lease", FinanceTypeId = 1 });
            financeTypes.Add(new FinanceType { Description = "Finance Lease", FinanceTypeId = 2 });
            financeTypes.Add(new FinanceType { Description = "Hire Purchase", FinanceTypeId = 3 });
            financeTypes.Add(new FinanceType { Description = "Hire Buy-Rebate Option", FinanceTypeId = 6 });
            financeTypes.Add(new FinanceType { Description = "Hire with Option to Buy", FinanceTypeId = 7 });
            financeTypes.Add(new FinanceType { Description = "Chattel Mortgage", FinanceTypeId = 8 });
            financeTypes.Add(new FinanceType { Description = "Rental Lease", FinanceTypeId = 9 });

            return financeTypes;
        }

        internal static ObservableModelCollection<SelectListViewModel> FakeFinanceTypes()
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

        internal static ObservableModelCollection<SelectListViewModel> FakeInstallmentTypes()
        {
            List<SelectListViewModel> installmentTypes;

            installmentTypes = new List<SelectListViewModel>();
            installmentTypes.Add(new SelectListViewModel { Text = InstalmentType.Advance.ToString(), Id = (int)InstalmentType.Advance });
            installmentTypes.Add(new SelectListViewModel { Text = InstalmentType.Arrears.ToString(), Id = (int)InstalmentType.Arrears });

            return new ObservableModelCollection<SelectListViewModel>(installmentTypes);
        }

        internal static ObservableModelCollection<SelectListViewModel> FakeFrequencies()
        {
            List<SelectListViewModel> frequencies;

            frequencies = new List<SelectListViewModel>();
            frequencies.Add(new SelectListViewModel { Text = PaymentFrequency.Annual.ToString(), Id = (int)PaymentFrequency.Annual });
            frequencies.Add(new SelectListViewModel { Text = PaymentFrequency.Monthly.ToString(), Id = (int)PaymentFrequency.Monthly });
            frequencies.Add(new SelectListViewModel { Text = PaymentFrequency.Quarterly.ToString(), Id = (int)PaymentFrequency.Quarterly });
            frequencies.Add(new SelectListViewModel { Text = PaymentFrequency.SemiAnnual.ToString(), Id = (int)PaymentFrequency.SemiAnnual });
            frequencies.Add(new SelectListViewModel { Text = PaymentFrequency.Weekly.ToString(), Id = (int)PaymentFrequency.Weekly });

            return new ObservableModelCollection<SelectListViewModel>(frequencies);
        }

        internal static ObservableModelCollection<SelectListViewModel> FakeSuppliers()
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

        internal static List<vwEntityRelation> FakeRawSuppliers()
        {
            List<vwEntityRelation> suppliers;
            suppliers = new List<vwEntityRelation>();
            suppliers.Add(new vwEntityRelation { NodeName = "Hitachi Construction Machinery (Revesby)", NodeId = 59 });
            suppliers.Add(new vwEntityRelation { NodeName = "HItachi Construction Machinery (Archerfield)", NodeId = 62 });
            suppliers.Add(new vwEntityRelation { NodeName = "Hitachi Construction Machinery (Cavan-SA)", NodeId = 234 });
            suppliers.Add(new vwEntityRelation { NodeName = "St Barbara Limited", NodeId = 603 });
            suppliers.Add(new vwEntityRelation { NodeName = "Weldit Fabrications Pty Ltd", NodeId = 633 });
            return suppliers;
        }

        internal static ObservableModelCollection<SelectListViewModel> FakeInternalCompanies()
        {
            List<SelectListViewModel> internalCompanies;

            internalCompanies = new List<SelectListViewModel>();
            internalCompanies.Add(new SelectListViewModel { Text = "Marubeni Equipment Finance (Oceania) Pty Ltd", Id = 58 });
            internalCompanies.Add(new SelectListViewModel { Text = "AABB funders", Id = 659 });
            internalCompanies.Add(new SelectListViewModel { Text = "AA internal and Client", Id = 668 });
            internalCompanies.Add(new SelectListViewModel { Text = "MEF  Agency", Id = 598 });

            return new ObservableModelCollection<SelectListViewModel>(internalCompanies);
        }

        internal static List<vwEntityRelation> FakeRawInternalCompanies()
        {
            List<vwEntityRelation> internalCompanies;

            internalCompanies = new List<vwEntityRelation>();
            internalCompanies.Add(new vwEntityRelation { NodeName = "Marubeni Equipment Finance (Oceania) Pty Ltd", NodeId = 58 });
            internalCompanies.Add(new vwEntityRelation { NodeName = "AABB funders", NodeId = 659 });
            internalCompanies.Add(new vwEntityRelation { NodeName = "AA internal and Client", NodeId = 668 });
            internalCompanies.Add(new vwEntityRelation { NodeName = "MEF  Agency", NodeId = 598 });

            return internalCompanies;
        }

        internal static ObservableModelCollection<SelectListViewModel> FakeEntityRelations()
        {
            List<SelectListViewModel> result = new List<SelectListViewModel>();

            result.Add(new SelectListViewModel{Id = 1, Text = "Funder" });
            return new ObservableModelCollection<SelectListViewModel>(result);
        }

        internal static IList<DirectDebitProfile> FakeDirectDebitProfiles()
        {
            List<DirectDebitProfile> result;

            result = new List<DirectDebitProfile>
            {
                new DirectDebitProfile { ID = 1, DDRecipientId = 1, DDRemitterId = 1, DDUserId = 1, ProfileName = "DDB Profile A", SystemDefault = true }
            };
            return result;
        }

        internal static TestDbAsyncEnumerable<ContractFundingDetail> FakeContractFundingDetailsForCalcualationOnTranche1()
        {
            List<ContractFundingDetail> result;

            result = new List<ContractFundingDetail>
            {
                new ContractFundingDetail
                {
                    CalculateTo = 491,
                    CompoundingFrequency = 23,
                    ContractFundingDetailsId = 607,
                    ContractId = 20511,
                    EquityContribution = 0,
                    EquityProviderRef = "",
                    EquitySellOffDate = new DateTime(2013,06,24),
                    ExpectedInertiaTerm = 1,
                    ExpectedResidualP = 0.0,
                    FinanceTypeId = 13,
                    FirmTermEndDate = new DateTime(2014,01,24),
                    ForecastResidualP = 0.0,
                    FunderNodeId = 655,
                    FundingAmount = 0,
                    FundingDate = new DateTime(2013,05,28),
                    FundingPayoutAmount = 0,
                    FundingPayoutDate = new DateTime(1900,01,01),
                    FundingPrefix = "FQQ",
                    FundingRate = 10.0,
                    FundingRef = "FQQ-607",
                    FundingStatusId = 486,
                    FundingTrancheId = 1,
                    FundingTypeId = 4,
                    FVEquityD = 0,
                    FVEquityP = 0.0,
                    HoldingCost = 0,
                    InstalmentType = 65,
                    IsCompoundingFrequencyUpdatedByUser = false,
                    IsEquityContributionUpdatedByUser = false,
                    IsHoldingCostPaySeparately = true,
                    IsHoldingCostUpdatedByUser = false,
                    IsLagDays = true,
                    IsPaymentIncludesDuties = true,
                    IsRecalcRequired = true,
                    LagPaymentDay = 0,
                    LastModifiedDate = new DateTime(2014,01,24),
                    LossReserveAmount = 0,
                    LossReserveRate = 8.0,
                    NetCashPosition = 0,
                    NumberOfPayments = 12,
                    PaymentEntered = 1,
                    PaymentFrequency = 23,
                    PaymentsRetained = 5,
                    PaymentsSold = 7,
                    PVofResidual = 0,
                    QuoteFundingDetailsId = -1,
                    RentalsRetained = 0,
                    ResidualAmount = 0,
                    RetainedEntered = 1,
                    SolveFor = 488,
                    StartDate = new DateTime(2013,06,24),
                    TermInMonths = 12
                }
            };
            return new TestDbAsyncEnumerable<ContractFundingDetail>(result);
        }

        internal static TestDbAsyncEnumerable<ContractFlag> FakeContractFlagsForCalcualationOnTranche1()
        {
            List<ContractFlag> result;

            result = new List<ContractFlag>
            {
                new ContractFlag
                {
                    ContractId = 20511,
                    ReCalcFlag = 0,
                    CashPosted=0,
                    ContractStatus=0,
                    ContractStatusDate = DateTime.Today,
                    ErrorCode = "",
                    GLEOMSystemUpdate = false,
                    GLIEUpdate = 0,
                    GLOCUpdate = 0,
                    GLReceiptsUpdate = false,
                    GLStaticDataUpdate = false,
                    GLTimeBasedUpdate = false,
                    GLTransactionupdate = false,
                    HoldStructures = false,
                    InterimOCCalcByUser = false,
                    OverdueStatus = 0,
                    RecalcFlagDate= DateTime.Today,
                }
            };
            return new TestDbAsyncEnumerable<ContractFlag>(result);
        }

        internal static TestDbAsyncEnumerable<Contract> FakeContractsForCalcualationOnTranche1()
        {
            List<Contract> result;

            result = new List<Contract>
            {
                new Contract
                {
                    ContractId = 20511,
                    ResidualD = 0,
                    AmountFinanced = 20400,
                    AssetCost = 18181.82M,
                    TotalOCNonAmort = 0,
                    TotalAssetOCNonAmort = 0,
                    TotalOCAmort = 0,
                    BrokerageD = 400,
                }
            };
            return new TestDbAsyncEnumerable<Contract>(result);
        }

        internal static TestDbAsyncEnumerable<ContractAmortSched> FakeContractAmortSchedulesForCalcualationOnTranche1()
        {
            List<ContractAmortSched> result;

            result = new List<ContractAmortSched>
            {
                new ContractAmortSched
                {
                    ContractId = 20511,
                    NetPayment = 1775.49M,
                    GSTAmount = 0,
                    StampDutyAmount = 0,
                    PaymentNo = 6
                },
                new ContractAmortSched
                {
                    ContractId = 20511,
                    NetPayment = 1775.49M,
                    GSTAmount = 0,
                    StampDutyAmount = 0,
                    PaymentNo = 7
                },
                new ContractAmortSched
                {
                    ContractId = 20511,
                    NetPayment = 1775.49M,
                    GSTAmount = 0,
                    StampDutyAmount = 0,
                    PaymentNo = 8
                },
                new ContractAmortSched
                {
                    ContractId = 20511,
                    NetPayment = 1775.49M,
                    GSTAmount = 0,
                    StampDutyAmount = 0,
                    PaymentNo = 9
                },
                new ContractAmortSched
                {
                    ContractId = 20511,
                    NetPayment = 1775.49M,
                    GSTAmount = 0,
                    StampDutyAmount = 0,
                    PaymentNo = 10
                },
                new ContractAmortSched
                {
                    ContractId = 20511,
                    NetPayment = 1775.49M,
                    GSTAmount = 0,
                    StampDutyAmount = 0,
                    PaymentNo = 11
                },
                new ContractAmortSched
                {
                    ContractId = 20511,
                    NetPayment = 1775.49M,
                    GSTAmount = 0,
                    StampDutyAmount = 0,
                    PaymentNo = 12
                },
                new ContractAmortSched
                {
                    ContractId = 20511,
                    NetPayment = 0,
                    GSTAmount = 0,
                    StampDutyAmount = 0,
                    PaymentNo = 13
                },
            };
            return new TestDbAsyncEnumerable<ContractAmortSched>(result);
        }
    }
}