using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.Business.Receipts;

namespace Insyston.Operations.WPF.ViewModel.Receipts.ExcelImport
{
    public class DishonourReversalReceiptExcel : OldViewModelBase, INotifyDataErrorInfo, IEditableObject
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private ReceiptBatchType batchType;
        private string receiptDate;
        private string receiptID;
        private string contractID;
        private decimal amountReceived;
        private string reference;
        private string reason;

        internal delegate bool IsReasonValid(string reason, ref int reasonCodeID);
        internal IsReasonValid ValidateReason;
        internal int ReasonCodeID;

        public ReceiptBatchType BatchType
        {
            get
            {
                return batchType;
            }
            set
            {
                if (batchType != value)
                {
                    batchType = value;
                    RaisePropertyChanged("BatchType");
                }
            }
        }

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

        public string ReceiptID
        {
            get
            {
                return receiptID;
            }
            set
            {
                if (receiptID != value)
                {
                    receiptID = value;
                    RaisePropertyChanged("ReceiptID");
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

        public decimal AmountReceived 
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

        public string Reference
        {
            get
            {
                return reference;
            }
            set
            {
                if (reference != value)
                {
                    reference = value;
                    RaisePropertyChanged("Reference");
                }
            }
        }

        public string Reason
        {
            get
            {
                return reason;
            }
            set
            {
                if (reason != value)
                {
                    reason = value;
                    RaisePropertyChanged("Reason");
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
            int receiptID=0, no=0;
            decimal amount = 0;

            if (batchType == ReceiptBatchType.Dishonour)
            {
                if (string.IsNullOrEmpty(ReceiptDate))
                {
                    this.SetError("ReceiptDate", "Receipt Date is required");
                }
                else if (DateTime.TryParse(ReceiptDate, out date) == false)
                {
                    this.SetError("ReceiptDate", "Receipt Date is Invalid");
                }
            }

            date = DateTime.Today;

            if (string.IsNullOrEmpty(ReceiptID))
            {
                this.SetError("ReceiptID", "Receipt No. is required");
            }
            else if (Int32.TryParse(ReceiptID, out no) == false)
            {
                this.SetError("ReceiptID", "Receipt No. is Invalid");
            }
            else if (no <= 0)
            {
                this.SetError("ReceiptID", "Receipt No. should be Positive Number");
            }
            else if (DishonourReversalFunctions.IsValidDishonourReversalReceipt((int)batchType, no) == false)
            {
                this.SetError("ReceiptID", "Receipt No. is Not Valid");
            }
            else
            {
                receiptID = no;
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
            else if (receiptID != 0 && ContractFunctions.ContractExistsinReceipt(no, receiptID, ref amount, ref date) == false)
            {
                this.SetError("ContractID", "Contract No. doesn't exist in this Receipt");
            }
            else
            {
                AmountReceived = amount;

                if (batchType == ReceiptBatchType.Reversals)
                {
                    receiptDate = date.ToString();
                }
            }

            if (ValidateReason != null)
            {
                if (string.IsNullOrEmpty(reason) == false && ValidateReason(reason, ref ReasonCodeID) == false)
                {
                    this.SetError("Reason", "Reason is Not Valid");
                }
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
