using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.WPF.ViewModel.Receipts.CashCheque;
using business = Insyston.Operations.Business.Common;
using Insyston.Operations.WPF.View.Receipts;
using Insyston.Operations.Business.Common.Enums;

namespace Insyston.Operations.WPF.ViewModel.Receipts.ExcelImport
{
    public class CashReceiptExcel : OldViewModelBase, INotifyDataErrorInfo, IEditableObject
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private ObservableCollection<OpenItemSearch> openItems;

        private string receiptDate;
        private string contractID;
        private double amountReceived;
        private string referenceNo;
        private string paymentMethod;
        private string bankName;
        private string accountName;
        private string chequeNumber;
        private string bsbNumber;        

        public string ReceiptDate 
        {
            get
            {
                return receiptDate;
            }
            set
            {
                if (receiptDate != value)
                {
                    receiptDate = value;
                    RaisePropertyChanged("ReceiptDate");
                }
            }
        }

        public string ContractID 
        {
            get
            {
                return contractID;
            }
            set
            {
                if (contractID != value)
                {
                    contractID = value;
                    RaisePropertyChanged("ContractID");
                }
            }
        }

        public double AmountReceived 
        {
            get
            {
                return amountReceived;
            }
            set
            {
                if (amountReceived != value)
                {
                    amountReceived = value;
                    RaisePropertyChanged("AmountReceived");
                }
            }
        }

        public string ReferenceNo 
        {
            get
            {
                return referenceNo;
            }
            set
            {
                if (referenceNo != value)
                {
                    referenceNo = value;
                    RaisePropertyChanged("ReferenceNo");
                }
            }
        }

        public string PaymentMethod 
        {
            get
            {
                return paymentMethod;
            }
            set
            {
                if (paymentMethod != value)
                {
                    paymentMethod = value;
                    RaisePropertyChanged("PaymentMethod");
                }
            }
        }

        public string BankName 
        {
            get
            {
                return bankName;
            }
            set
            {
                if (bankName != value)
                {
                    bankName = value;
                    RaisePropertyChanged("BankName");
                }
            }
        }

        public string AccountName 
        {
            get
            {
                return accountName;
            }
            set
            {
                if (accountName != value)
                {
                    accountName = value;
                    RaisePropertyChanged("AccountName");
                }
            }
        }

        public string ChequeNumber 
        {
            get
            {
                return chequeNumber;
            }
            set
            {
                if (chequeNumber != value)
                {
                    chequeNumber = value;
                    RaisePropertyChanged("ChequeNumber");
                }
            }
        }

        public string BSBNumber 
        {
            get
            {
                return bsbNumber;
            }
            set
            {
                if(bsbNumber != value)
                {
                    bsbNumber = value;
                    RaisePropertyChanged("BSBNumber");
                }
            }
        }

        public ObservableCollection<OpenItemSearch> OpenItems
        {
            get
            {
                return openItems;
            }
            set
            {
                if (openItems != value)
                {
                    openItems = value;
                    RaiseErrorChanged("OpenItems");
                }
            }
        }

        public void BeginEdit()
        {
            this.ClearErrors();
        }

        public void CancelEdit()
        {
        }

        public void EndEdit()
        {            
            DateTime date;
            int no=0;
            double dblAmtApplied;
            bool isChequePayment = false;

            if (string.IsNullOrEmpty(ReceiptDate))
            {
                this.SetError("ReceiptDate", "Receipt Date is required");
            }
            else if (DateTime.TryParse(ReceiptDate, out date) == false)
            {
                this.SetError("ReceiptDate", "Receipt Date is Invalid");
            }            

            if (string.IsNullOrEmpty(ContractID))
            {
                this.SetError("ContractID", "Contract No. is required");
            }
            else if (Int32.TryParse(ContractID, out no) == false)
            {
                this.SetError("ContractID", "Contract No. is Invalid");
            }
            else if (no <= 0)
            {
                this.SetError("ContractID", "Contract No. should be Positive Number");
            }
            else if (business.ContractFunctions.ContractExists(no) == false)
            {
                this.SetError("ContractID", "Contract No. is Not Valid");
            }
            else
            {
                OpenItems = OpenItemFunctions.GetOpenItemsSearch(ReceiptApplyTo.Contract, no, false, false, (ReceiptPrimaryAllocation)Convert.ToInt32(Shared.CashChequeReceiptDefaults.ApplyToOption.GetValueOrDefault()));

                if (openItems.Count == 0)
                {
                    this.SetError("ContractID", "No Open Items Pending for this Contract");
                }
            }

            if (amountReceived == 0)
            {
                this.SetError("AmountReceived", "Amount Received is Required");
            }
            else if (OpenItems != null && openItems.Count > 0)
            {                
                OpenItemFunctions.ApplyPayments(ReceiptBatchType.CashCheque, OpenItems, Convert.ToDecimal(amountReceived), ReceiptApplyTo.Contract, no, Shared.CashChequeReceiptDefaults);

                if (amountReceived < 0)
                {
                    openItems.First().AmountApplied = Convert.ToDecimal(amountReceived);                 
                }

                dblAmtApplied = Convert.ToDouble(openItems.Sum(item => item.AmountApplied));

                if (Math.Abs(amountReceived - dblAmtApplied) != 0)
                {
                    this.SetError("AmountReceived", "Amount exceeds Open Item Balances Pending");
                }
            }

            if (string.IsNullOrEmpty(PaymentMethod))
            {
                this.SetError("PaymentMethod", "Receipt Type is required");
            }
            else if (CashReceiptExcelImportViewModel.CashChequePaymentTypes.Where(payType => payType.PaymentTypeDesc.ToLower() == PaymentMethod.ToLower()).Count() == 0)
            {
                this.SetError("PaymentMethod", "Receipt Type is Invalid");
            }
            else
            {
                isChequePayment = CashReceiptExcelImportViewModel.CashChequePaymentTypes.Where(payType => payType.PaymentTypeDesc.ToLower() == PaymentMethod.ToLower() && payType.SCPaymentTypeId == (int)SCPaymentTypes.Cheque).Count() > 0;

                if (isChequePayment)
                {
                    if (string.IsNullOrEmpty(BankName))
                    {
                        this.SetError("BankName", "Bank Name is required");
                    }

                    if (string.IsNullOrEmpty(AccountName))
                    {
                        this.SetError("AccountName", "Account Name is required");
                    }

                    if (string.IsNullOrEmpty(ChequeNumber))
                    {
                        this.SetError("ChequeNumber", "Cheque Number is required");
                    }
                    else if (Int32.TryParse(ChequeNumber, out no) == false || no <= 0)
                    {
                        this.SetError("ChequeNumber", "Cheque Number is Invalid");
                    }

                    if (string.IsNullOrEmpty(BSBNumber))
                    {
                        this.SetError("BSBNumber", "BSB Number is required");
                    }
                    else if (Int32.TryParse(BSBNumber, out no) == false)
                    {
                        this.SetError("BSBNumber", "BSB Number is Invalid");
                    }
                    else if (bsbNumber.Trim().Length != 6)
                    {
                        this.SetError("BSBNumber", "BSB Number should be 6 digits long");
                    }
                }
            }

            if(string.IsNullOrEmpty(paymentMethod) == false && isChequePayment == false)
            {
                RemoveError("BankName");
                RemoveError("AccountName");
                RemoveError("ChequeNumber");
                RemoveError("BSBNumber");
            }
        }

        public void SetError(string propertyName, string error)
        {
            if (base.ErrorMessages != null && base.ErrorMessages.Where(err => err == propertyName).Count() > 0)
            {
                base.ErrorMessages.Remove(propertyName);
            }

            AddErrorMessage(propertyName, error);
            RaiseErrorChanged(propertyName);
        }

        public void RemoveError(string propertyName)
        {
            if (base.ErrorMessages != null && base.ErrorMessages.Where(err => err == propertyName).Count() > 0)
            {
                base.ErrorMessages.Remove(propertyName);
            }

            RaiseErrorChanged(propertyName);
        }

        public void ClearErrors()
        {
            if (ErrorMessages != null)
            {
                ClearErrorMessages();
            }

            RaiseErrorChanged(string.Empty);
        }

        public void RaiseErrorChanged(string propertyName)
        {
            try
            {
                if (this.ErrorsChanged != null)
                {
                    this.ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
                }
            }
            catch
            {
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return GetErrorMessage(propertyName);            
        }

        public bool HasErrors
        {
            get { return base.HasError; }
        }
    }
}
