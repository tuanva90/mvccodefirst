using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System.Data.OleDb;
using System.Data;
using Insyston.Operations.Model;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System.ComponentModel;
using Insyston.Operations.Logging;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.WPF.ViewModel.Common;
using Insyston.Operations.WPF.ViewModel.Receipts.ExcelImport;
using Insyston.Operations.Security;
using System.Threading;
using System.Windows.Forms;

namespace Insyston.Operations.WPF.ViewModel.Receipts.DishonourReversal
{
    [Export(typeof(DishonourReversalExcelImportViewModel))]
    public class DishonourReversalExcelImportViewModel : OldViewModelBase
    {
        private List<DishonourReversalReceiptExcel> receipts;
        private readonly List<DropdownList> reasonCodes;
        private readonly string excelTemplateFileName;

        private readonly ReceiptBatchType batchType;
        private readonly int receiptBatchID;
        private string title, path;

        public InteractionRequest<ConfirmationWindowViewModel> UIConfirmation { get; private set; }

        public delegate void ClearGridFilterEventHandler();
        public event ClearGridFilterEventHandler ClearGridFilter;

        public DelegateCommand Browse { get; private set; }
        public DelegateCommand Upload { get; private set; }
        public DelegateCommand Save { get; private set; }
        public DelegateCommand OpenExcelTemplate { get; private set; }
        public DelegateCommand Delete { get; private set; }
        public static List<PaymentType> CashChequePaymentTypes { get; private set; }

        public DishonourReversalExcelImportViewModel(ReceiptBatchType receiptBatchType, int receiptBatchId)
        {
            batchType = receiptBatchType;
            receiptBatchID = receiptBatchId;

            reasonCodes = DishonourReversalFunctions.GetReasonCodes(batchType);

            Browse = new DelegateCommand(OnBrowse);
            Upload = new DelegateCommand(OnUpload);
            Save = new DelegateCommand(OnSave);
            Delete = new DelegateCommand(OnDelete);
            OpenExcelTemplate = new DelegateCommand(OnOpenExcelTemplate);

            Title = batchType.ToString() + " Receipts Excel Import";
            IconFileName = "Excel.jpg";
            UIConfirmation = new InteractionRequest<ConfirmationWindowViewModel>();

            if (batchType == ReceiptBatchType.Dishonour)
            {
                excelTemplateFileName = "DishonourReceiptImportTemplate.xlsx";
            }
            else
            {
                excelTemplateFileName = "ReversalReceiptImportTemplate.xlsx";
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                if (path != value)
                {
                    if (string.IsNullOrEmpty(value) == false && value.EndsWith(".xls") == false && value.EndsWith(".xlsx") == false)
                    {
                        ShowMessage("Receipt Import accepts only Excel files\n\nPlease select an Excel file to Import", "Excel Import");
                        OnBrowse();
                        return;
                    }
                    
                    path = value;
                    RaisePropertyChanged("Path");
                }
            }
        }

        public List<DishonourReversalReceiptExcel> Receipts
        {
            get
            {
                return receipts;
            }
            set
            {
                if (receipts != value)
                {
                    receipts = value;
                    RaisePropertyChanged("Receipts");
                    RaisePropertyChanged("IsReceiptsAvailable");
                }
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (title != value)
                {
                    title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        public string ReceiptDateCaption
        {
            get
            {
                return batchType.ToString() + " Date";
            }
        }

        public bool IsReceiptsAvailable
        {
            get
            {
                if (receipts != null && receipts.Count > 0)
                {
                    return true;
                }

                return false;
            }
            set
            {
                RaisePropertyChanged("IsReceiptsAvailable");
            }
        }

        public bool IsReversalBatch
        {
            get
            {
                return batchType == ReceiptBatchType.Reversals;
            }
        }

        public bool IsSaveEnabled
        {
            get
            {
                if (receipts == null || receipts.Count == 0)
                {
                    return false;
                }

                return receipts.Where(item => item.HasErrors).Count() == 0;
            }
            set
            {
                RaisePropertyChanged("IsSaveEnabled");
            }
        }        

        void Receipt_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            RaisePropertyChanged("IsSaveEnabled");
        }  

        private void OnBrowse()
        {
            OpenFileDialog dialog;
            LXMSystemReceiptDefault receiptDefault;

            dialog = new OpenFileDialog();
            dialog.DefaultExt = "*.xlsx";
            dialog.Filter = "Excel Files|*.xlsx;*.xls";

            if (string.IsNullOrEmpty(path))
            {
                receiptDefault = BatchTypeFunctions.GetReceiptBatchSystemDefaults((int)batchType);

                if(receiptDefault != null)
                {
                    if (string.IsNullOrEmpty(receiptDefault.ImportPath) == false)
                    {
                        dialog.InitialDirectory = receiptDefault.ImportPath;
                    }
                }                
            }

            dialog.ShowDialog();
            Path = dialog.FileName;   
        }

        private void OnUpload()
        {
            if (string.IsNullOrEmpty(Path))
            {
                OnBrowse();

                if (string.IsNullOrEmpty(Path))
                {
                    return;
                }
            }
            
            GetExcelData();
            IsBusy = false;
        }

        private void OnDelete()
        {
            RaisePropertyChanged("IsSaveEnabled");
        }

        private void OnSave()
        {
            ReceiptBatch receiptBatch;
            Receipt receipt;
            BankAccount bankAccount;

            DishonourReversalReceiptExcel dishonourReversalReceiptDetail;
            OpenItemReceiptAllocation openItemReceiptAllocation;

            try
            {
                receiptBatch = ReceiptBatchFunctions.Get(receiptBatchID);
                bankAccount = ReceiptBatchFunctions.GetBankAccount(batchType, ReceiptFunctions.Get(Convert.ToInt32(Receipts.FirstOrDefault().ReceiptID)).ReceiptBatchID);

                if (DishonourReversalFunctions.IsReceiptsBelongToSameBank(Receipts.Select(item => Convert.ToInt32(item.ReceiptID)).ToList()) == false)
                {
                    ShowMessage("All Receipts to be imported into this Batch must belong to single Bank Account", "Excel Import - Receipts Validation");
                    return;
                }
                else if (receiptBatch.BankAccountID.HasValue && receiptBatch.BankAccountID != bankAccount.ID)
                {
                    bankAccount = ReceiptBatchFunctions.GetBankAccount(batchType, receiptBatchID);
                    ShowMessage("Receipts of bank \"" + bankAccount.BankName + "\" can only be imported into this batch", "Excel Import - Receipts Validation");
                    return;
                }

                IsBusy = true;

                receiptBatch.BankAccountID = bankAccount.ID;
                receiptBatch.FilterDescription = "Bank: " + bankAccount.BankName;

                receiptBatch.Receipts = new List<Receipt>();

                foreach (DishonourReversalReceiptExcel excelItem in Receipts)
                {
                    receipt = new Receipt();

                    receipt.ReceiptBatchID = receiptBatchID;
                    receipt.ReceiptDate = Convert.ToDateTime(excelItem.ReceiptDate);
                    receipt.ApplyToTypeID = (int)ReceiptApplyTo.Contract;
                    receipt.ContractID = Convert.ToInt32(excelItem.ContractID);
                    receipt.Reference = excelItem.Reference;
                    receipt.LastUserID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
                    receipt.InternalReference = receipt.ContractID.ToString();
                    receipt.LastDateModified = DateTime.Now;

                    receipt.FIDAmountReceived = 0;
                    receipt.SDAmountReceived = 0;
                    receipt.GSTAmountReceived = 0;
                    receipt.NetAmountReceived = 0;
                    receipt.GrossAmountReceived = 0;

                    if (batchType == ReceiptBatchType.Dishonour)
                    {
                        receipt.DishonourReceiptDetail = new DishonourReceiptDetail();
                        receipt.DishonourReceiptDetail.DishonouredReceiptID = Convert.ToInt32(excelItem.ReceiptID);

                        if (excelItem.ReasonCodeID > 0)
                        {
                            receipt.DishonourReceiptDetail.ReasonCodeID = excelItem.ReasonCodeID;
                        }
                    }
                    else if (batchType == ReceiptBatchType.Reversals)
                    {
                        receipt.ReversalReceiptDetail = new ReversalReceiptDetail();
                        receipt.ReversalReceiptDetail.LinkedReceiptID = Convert.ToInt32(excelItem.ReceiptID);
                        receipt.ReversalReceiptDetail.ReversalTypeID = (int)ReversalTypes.Reversal;

                        if (excelItem.ReasonCodeID > 0)
                        {
                            receipt.ReversalReceiptDetail.ReasonCodeID = excelItem.ReasonCodeID;
                        }
                    }

                    foreach (OpenItemReceiptAllocation allocation in DishonourReversalFunctions.GetOpenItemReceiptAllocations(Convert.ToInt32(excelItem.ReceiptID)))
                    {
                        openItemReceiptAllocation = new OpenItemReceiptAllocation();

                        openItemReceiptAllocation.OpenItemID = allocation.OpenItemID;
                        openItemReceiptAllocation.NetAmountApplied = allocation.NetAmountApplied * -1;
                        openItemReceiptAllocation.GSTAmountApplied = allocation.GSTAmountApplied * -1;
                        openItemReceiptAllocation.FIDAmountApplied = allocation.FIDAmountApplied * -1;
                        openItemReceiptAllocation.SDAmountApplied = allocation.SDAmountApplied * -1;
                        openItemReceiptAllocation.GrossAmountApplied = allocation.GrossAmountApplied * -1;
                        receipt.OpenItemReceiptAllocations.Add(openItemReceiptAllocation);

                        receipt.FIDAmountReceived += openItemReceiptAllocation.FIDAmountApplied;
                        receipt.SDAmountReceived += openItemReceiptAllocation.SDAmountApplied;
                        receipt.GSTAmountReceived += openItemReceiptAllocation.GSTAmountApplied;
                        receipt.NetAmountReceived += openItemReceiptAllocation.NetAmountApplied;
                        receipt.GrossAmountReceived += openItemReceiptAllocation.GrossAmountApplied.GetValueOrDefault();
                    }

                    receiptBatch.GrossBatchTotal += receipt.GrossAmountReceived;
                    receiptBatch.NumberOfEntries++;

                    receiptBatch.Receipts.Add(receipt);
                }

                if (receiptBatch.Receipts.Where(item => item.ReceiptDate > DateTime.Today).Count() > 0)
                {
                    UIConfirmation.Raise(
                           new ConfirmationWindowViewModel(this) { Content = "One or more Receipt Date(s) are in the future. Select OK to continue or Cancel to modify", Icon = "Question.ico", Title = "Receipt Date Confirmation" },
                           (popupCallBack) =>
                           {
                               if (popupCallBack.Confirmed)
                               {                                   
                                    ReceiptBatchFunctions.Save(receiptBatch);
                                    Close();
                               }
                           });
                }
                else
                {
                    ReceiptBatchFunctions.Save(receiptBatch);
                    Close();
                }

                IsBusy = false;
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error encountered while Importing Excel Entries", "Excel Import - Error");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnOpenExcelTemplate()
        {
            Process process;            

            process = new Process();
            process.StartInfo = new ProcessStartInfo();
            process.StartInfo.Verb = "open";
            process.StartInfo.FileName = CreateExcelFile();
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }

        private void GetExcelData()
        {
            OleDbConnection connection;
            OleDbDataAdapter adapter;
            DataTable excelData;
            DishonourReversalReceiptExcel receipt;
            List<DishonourReversalReceiptExcel> receipts;
            DateTime receiptDate;
            Decimal amount;
            bool isRowEmpty;

            try
            {
                Receipts = null;
                IsReceiptsAvailable = false;
                IsSaveEnabled = false;

                if (File.Exists(path) == false)
                {
                    ShowMessage("Excel file doesn't exist at the specified location or the file is not accessible", "Excel Import - File Not Found");
                    OnBrowse();
                    return;
                }

                if (path.EndsWith(".xlsx"))
                {
                    connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties=""Excel 12.0 Xml;HDR=Yes;IMEX=1;"";Data Source=" + path);
                }
                else
                {
                    connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1;"";Data Source=" + path);
                }

                using (connection)
                {
                    connection.Open();
                    adapter = new OleDbDataAdapter("Select * From [Sheet1$]", connection);
                    excelData = new DataTable();
                    adapter.Fill(excelData);
                    connection.Close();
                }

                if (excelData.Rows.Count == 0)
                {
                    ShowMessage("Excel Sheet selected is Empty", "Excel Import");
                    return;
                }
                else if (CompareExcelWithTemplate(excelData) == false)
                {
                    ShowMessage("Excel Sheet selected doesn't match the expected template.\n\nPlease use the 'Get Import Template' link to get the correct Template", "Excel Import - Template Mismatch");
                    return;
                }

                IsBusy = true;
                receipts = new List<DishonourReversalReceiptExcel>();

                foreach (DataRow row in excelData.Rows)
                {
                    receipt = new DishonourReversalReceiptExcel();
                    receipt.ValidateReason = ValidateReason;
                    receipt.BatchType = batchType;

                    isRowEmpty = true;

                    if (batchType == ReceiptBatchType.Dishonour)
                    {
                        if (row["Dishonour Date"] != null && string.IsNullOrEmpty(row["Dishonour Date"].ToString()) == false)
                        {
                            if (DateTime.TryParse(row["Dishonour Date"].ToString(), out receiptDate))
                            {
                                receipt.ReceiptDate = receiptDate.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                receipt.ReceiptDate = row["Dishonour Date"].ToString();
                            }

                            isRowEmpty = false;
                        }
                    }

                    if (row["Receipt ID"] != null && string.IsNullOrEmpty(row["Receipt ID"].ToString()) == false)
                    {
                        receipt.ReceiptID = row["Receipt ID"].ToString();
                        isRowEmpty = false;
                    }

                    if (row["Contract ID"] != null && string.IsNullOrEmpty(row["Contract ID"].ToString()) == false)
                    {
                        receipt.ContractID = row["Contract ID"].ToString();
                        isRowEmpty = false;
                    }

                    receipt.AmountReceived = 0;

                    if (row["Amount"] != null && string.IsNullOrEmpty(row["Amount"].ToString()) == false && Decimal.TryParse(row["Amount"].ToString(), out amount))
                    {
                        receipt.AmountReceived = amount;
                        isRowEmpty = false;
                    }

                    if (row["Reference"] != null && string.IsNullOrEmpty(row["Reference"].ToString()) == false)
                    {
                        receipt.Reference = row["Reference"].ToString();
                        isRowEmpty = false;
                    }

                    if (row["Reason"] != null && string.IsNullOrEmpty(row["Reason"].ToString()) == false)
                    {
                        receipt.Reference = row["Reason"].ToString();
                        isRowEmpty = false;
                    }

                    if (isRowEmpty == false)
                    {
                        receipt.ErrorsChanged += Receipt_ErrorsChanged;
                        receipt.EndEdit();
                        receipts.Add(receipt);
                    }
                }

                Receipts = receipts;
                RaisePropertyChanged("IsReceiptsAvailable");
                RaisePropertyChanged("IsSaveEnabled");
                RaisePropertyChanged("ReceiptDateCaption");
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error occurred while reading Excel Sheet to be Imported\nThe excel sheet is eihter not readable or doesn't match the expected template\n", "Excel Import - Error");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool ValidateReason(string reason, ref int reasonCodeID)
        {
            DropdownList reasonCode;

            reasonCode= reasonCodes.Where(item => item.Description.ToLower() == reason).FirstOrDefault();

            if(reasonCode != null)
            {
                reasonCodeID = reasonCode.ID;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CompareExcelWithTemplate(DataTable excelData)
        {
            OleDbConnection connection;
            OleDbDataAdapter adapter;
            DataTable templateData;

            using (connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties=""Excel 12.0 Xml;HDR=Yes;IMEX=1;"";Data Source=" + CreateExcelFile()))
            {
                connection.Open();
                adapter = new OleDbDataAdapter("Select * From [Sheet1$]", connection);
                templateData = new DataTable();
                adapter.Fill(templateData);
                connection.Close();
            }

            if(templateData.Columns.Count != excelData.Columns.Count)
            {
                return false;
            }

            for(int index = 0; index < templateData.Columns.Count; index++)
            {
                if (excelData.Columns.Contains(templateData.Columns[index].ColumnName) == false)
                {
                    return false;
                }
            }

            return true;
        }

        private string CreateExcelFile()
        {
            FileStream file;
            string folderPath;
            
            if (System.IO.Path.GetTempPath().EndsWith(@"\"))
            {
                folderPath = System.IO.Path.GetTempPath();
            }
            else
            {
                folderPath = System.IO.Path.GetTempPath() + "\\";
            }

            if (File.Exists(folderPath + excelTemplateFileName))
            {
                try
                {
                    File.Delete(folderPath + excelTemplateFileName);
                }
                catch
                {
                }
            }

            if (File.Exists(folderPath + excelTemplateFileName) == false)
            {
                using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(this.GetType().Assembly.GetName().Name + ".Templates." + excelTemplateFileName))
                {
                    file = new FileStream(folderPath + excelTemplateFileName, FileMode.Create);
                    byte[] result = new byte[stream.Length];
                    stream.Read(result, 0, (int)stream.Length);
                    file.Write(result, 0, result.Length);
                    file.Close();
                }
            }

            return folderPath + excelTemplateFileName;
        }                   
    }
}
