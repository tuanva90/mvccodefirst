using Insyston.Operations.Business.OpenItems;
using Insyston.Operations.Model;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Insyston.Operations.Logging;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Common;
using receipts = Insyston.Operations.Business.Receipts;
using Insyston.Operations.Security;
using System.Threading;

namespace Insyston.Operations.WPF.ViewModel.Receipts.CommonControls
{
    public class AddOtherChargeViewModel : OldViewModelBase, IDataErrorInfo
    {
        private List<DropdownList> otherCharges;
        private ContractOtherCharge contractOtherCharge;
        private bool returnValue;

        public DelegateCommand NextPayment { get; private set; }
        public DelegateCommand Save { get; private set; }
        public DelegateCommand Loaded { get; private set; }       

        public AddOtherChargeViewModel(int contractID)
        {
            try
            {
                IconFileName = "AddOtherCharge.jpg";
                OtherCharges = OtherChargeFunctions.GetOtherCharges();

                ContractOtherCharge = new ContractOtherCharge();
                OtherChargeID = 0;                
                ContractOtherCharge.ContractId = contractID;
                ContractOtherCharge.StartDate = DateTime.Today;
                NextPayment = new DelegateCommand(OnNextPayment);
                Save = new DelegateCommand(OnSave);
                Loaded = new DelegateCommand(OnLoaded);
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error occurred while initializing Other Charges", "Add Other Charge - Error");
            }
        }

        public List<DropdownList> OtherCharges
        {
            get
            {
                return otherCharges;
            }
            set
            {
                if (otherCharges != value)
                {
                    otherCharges = value;
                    RaisePropertyChanged("OtherCharges");
                }
            }
        }

        public ContractOtherCharge ContractOtherCharge
        {
            get
            {
                return contractOtherCharge;
            }
            set
            {
                if (contractOtherCharge != value)
                {
                    contractOtherCharge = value;
                    RaisePropertyChanged("ContractOtherCharge");
                }
            }
        }

        public int? OtherChargeID
        {
            get
            {
                return contractOtherCharge.OtherChargeId;
            }
            set
            {
                OtherCharge otherCharge;

                if (contractOtherCharge.OtherChargeId != value)
                {                    
                    contractOtherCharge.OtherChargeId = value;
                    RaisePropertyChanged("OtherChargeID");

                    if (contractOtherCharge.OtherChargeId.GetValueOrDefault() != 0)
                    {
                        otherCharge = OtherChargeFunctions.GetOtherCharge(contractOtherCharge.OtherChargeId.Value);

                        contractOtherCharge.Amount = otherCharge.DefaultValue.GetValueOrDefault();
                        contractOtherCharge.PofRentalCalcType = otherCharge.PofRentalCalcType;
                        contractOtherCharge.POfRentalType = otherCharge.POfRentalType;
                        contractOtherCharge.IsPaymentNoApplied = otherCharge.NextPaymentNo;
                        contractOtherCharge.Description = otherCharge.Description;
                        contractOtherCharge.Amortised = otherCharge.Amortize;
                        contractOtherCharge.ApplyFID = otherCharge.ApplyFID;
                        contractOtherCharge.ApplyGST = otherCharge.ApplyGST;
                        contractOtherCharge.ApplySD = otherCharge.ApplySD;
                        contractOtherCharge.ApplyToInterim = otherCharge.ApplyToInterim;
                        contractOtherCharge.IncomeExpenseId = otherCharge.IncomeExpenseId;
                        contractOtherCharge.IncludeInertiaExt = otherCharge.IncludeInertiaExt;
                        contractOtherCharge.OnInterim = otherCharge.OnInterim;
                        contractOtherCharge.ExpenseStartType = otherCharge.ExpenseStartType;
                        contractOtherCharge.ExpenseAmountType = otherCharge.ExpenseAmountType;
                        contractOtherCharge.CreateExpense = otherCharge.CreateExpense;

                        if (otherCharge.NextPaymentNo)
                        {                            
                            OnNextPayment();
                        }
                        else
                        {
                            PaymentNo = 1;
                        }
                    }
                }
            }
        }

        public int? PaymentNo
        {
            get
            {
                return contractOtherCharge.PaymentNo;
            }
            set
            {
                DateTime? dtDate;

                if (contractOtherCharge.PaymentNo != value)
                {                    
                    contractOtherCharge.PaymentNo = value;
                    RaisePropertyChanged("PaymentNo");

                    if (value != null)
                    {
                        dtDate = ContractAssetAccountingFunctions.GetStartDate(contractOtherCharge.ContractId, value.Value);

                        if (dtDate != null)
                        {
                            ContractOtherCharge.StartDate = dtDate.Value;
                        }
                        else
                        {
                            ContractOtherCharge.StartDate = DateTime.Today;
                        }
                    }
                }
            }
        }

        public bool ReturnValue
        {
            get
            {
                return returnValue;
            }
            set
            {
                returnValue = value;
                RaisePropertyChanged("ReturnValue");
            }
        }

        private void OnNextPayment()
        {
            PaymentNo = ContractAssetAccountingFunctions.GetNextPaymentNo(contractOtherCharge.ContractId);
        }

        private void OnSave()
        {
            int contractID;
            contractOtherCharge.IsChanged = true;

            try
            {
                if (Validate())
                {
                    contractOtherCharge.Repeats = 1;
                    contractOtherCharge.AtSettlement = 0;
                    contractOtherCharge.TillLastPmt = 0;
                    contractOtherCharge.IncludeInertiaExt = 0;
                    contractOtherCharge.NonAmortPofRental = 0;
                    contractOtherCharge.TaxMethod = receipts.Constants.TaxMethod;
                    contractOtherCharge.LastUserId = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
                    contractOtherCharge.CreatedByUser = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.LoginName;
                    contractOtherCharge.RecPmtType = receipts.Constants.RecPmtTypeOtherCharge;
                    contractOtherCharge.OnInterim = 0;
                    contractOtherCharge.InterimFID = 0;
                    contractOtherCharge.InterimGST = 0;
                    contractOtherCharge.InterimNetAmount = 0;
                    contractOtherCharge.InterimAmount = 0;
                    contractOtherCharge.InterimStampDuty = 0;
                    contractOtherCharge.CreatedByProcessID = receipts.Constants.OtherChargeCreatedBy;

                    if (contractOtherCharge.IsPaymentNoApplied == false)
                    {
                        contractOtherCharge.PaymentNo = null;
                    }

                    if (receipts.OtherChargeFunctions.SaveContractOtherCharges(contractOtherCharge))
                    {
                        ReturnValue = true;
                        Close();
                    }                  
                }

                contractOtherCharge.IsChanged = false;
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error occurred while Saving Other Charge", "Add Other Charge - Error");
            }
        }

        private void OnLoaded()
        {
            contractOtherCharge.Disclosed = true;
        }

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get 
            {
                if (contractOtherCharge.IsChanged == false)
                {
                    return string.Empty;
                }

                if (columnName.ToLower() == "otherchargeid" && OtherChargeID <= 0)
                {
                    return "Other Charge has to be Selected";
                }
                else if(columnName.ToLower() == "paymentno" && contractOtherCharge.IsPaymentNoApplied)
                {
                    if(!PaymentNo.HasValue || PaymentNo == 0)
                    {
                        return "Payment No. is Required";
                    }
                    else if (ContractAssetAccountingFunctions.CheckPaymentNoExists(contractOtherCharge.ContractId, PaymentNo.Value) == false)
                    {
                        return "Payment No. is Invalid";
                    }
                }

                return string.Empty;
            }
        }      
    }
}
