using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System.Data.OleDb;
using System.Data;
using Insyston.Operations.Model;
using Insyston.Operations.Business.OpenItems;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Insyston.Operations.Logging;
using Insyston.Operations.WPF.ViewModel.Receipts.ExcelImport;
using Insyston.Operations.WPF.ViewModel.Common;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.Security;
using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.WPF.View.Receipts;
using Insyston.Operations.Business.Common.Enums;
using System.Threading;
using System.Windows.Forms;

namespace Insyston.Operations.WPF.ViewModel.Receipts.CashCheque
{
    [Export(typeof(CashReceiptExcelImportViewModel))]
    public class CashReceiptExcelImportViewModel : CashReceiptExcel
    {
        private List<CashReceiptExcel> receipts;
        private readonly int receiptBatchID;
        private string path;

        public InteractionRequest<ConfirmationWindowViewModel> UIConfirmation { get; private set; }

        public delegate void ClearGridFilterEventHandler();
        public event ClearGridFilterEventHandler ClearGridFilter;

        public DelegateCommand Browse { get; private set; }
        public DelegateCommand Upload { get; private set; }
        public DelegateCommand Save { get; private set; }
        public DelegateCommand OpenExcelTemplate { get; private set; }
        public DelegateCommand Delete { get; private set; }
        public static List<PaymentType> CashChequePaymentTypes { get; private set; }

        public CashReceiptExcelImportViewModel(int receiptBatchId)
        {
            receiptBatchID = receiptBatchId;
            IconFileName = "Excel.jpg";
            UIConfirmation = new InteractionRequest<ConfirmationWindowViewModel>();
            Browse = new DelegateCommand(OnBrowse);
            Upload = new DelegateCommand(OnUpload);
            Save = new DelegateCommand(OnSave);
            Delete = new DelegateCommand(OnDelete);
            OpenExcelTemplate = new DelegateCommand(OnOpenExcelTemplate);
            CashChequePaymentTypes = PaymentTypeFunctions.GetCashChequePaymentTypes();
            Shared.CashChequeReceiptDefaults = BatchTypeFunctions.GetReceiptBatchSystemDefaults((int)ReceiptBatchType.CashCheque);
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

        public List<CashReceiptExcel> Receipts
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
                receiptDefault = BatchTypeFunctions.GetReceiptBatchSystemDefaults((int)ReceiptBatchType.CashCheque);

                if (receiptDefault != null)
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
            List<Receipt> receipts;
            ReceiptBatch receiptBatch;
            Receipt receipt;
            CashReceiptDetail cashReceiptDetail;
            PaymentType paymentType;
            OpenItemReceiptAllocation openItemReceiptAllocation;
            bool confirmResult = true;

            try
            {
                IsBusy = true;

                receipts = new List<Receipt>();

                foreach (CashReceiptExcel excelItem in Receipts)
                {
                    receipt = new Receipt();

                    paymentType = CashReceiptExcelImportViewModel.CashChequePaymentTypes.Where(payType => payType.PaymentTypeDesc.ToLower() == excelItem.PaymentMethod.ToLower()).FirstOrDefault();

                    receipt.ReceiptBatchID = receiptBatchID;
                    receipt.ReceiptDate = Convert.ToDateTime(excelItem.ReceiptDate);
                    receipt.ApplyToTypeID = (int)ReceiptApplyTo.Contract;
                    receipt.ContractID = Convert.ToInt32(excelItem.ContractID);
                    receipt.Reference = excelItem.ReferenceNo;
                    receipt.GrossAmountReceived = Math.Round(Convert.ToDecimal(excelItem.AmountReceived), 2);
                    receipt.LastUserID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
                    receipt.InternalReference = receipt.ContractID.ToString();

                    if (paymentType.SCPaymentTypeId == (int)SCPaymentTypes.Cheque)
                    {
                        receipt.ChequeReceiptDetail = new ChequeReceiptDetail();
                        receipt.ChequeReceiptDetail.PaymentTypeID = paymentType.PaymentTypeId;
                        receipt.ChequeReceiptDetail.ChequeNumber = excelItem.ChequeNumber;
                        receipt.ChequeReceiptDetail.AccountName = excelItem.AccountName;
                        receipt.ChequeReceiptDetail.BankName = excelItem.BankName;
                        receipt.ChequeReceiptDetail.BSBNumber = excelItem.BSBNumber;
                    }
                    else
                    {
                        cashReceiptDetail = new CashReceiptDetail();
                        cashReceiptDetail.PaymentTypeID = paymentType.PaymentTypeId;
                        receipt.CashReceiptDetails.Add(cashReceiptDetail);
                    }

                    receipt.OpenItemReceiptAllocations = new List<OpenItemReceiptAllocation>();

                    foreach (OpenItemSearch openItem in excelItem.OpenItems)
                    {
                        openItemReceiptAllocation = new OpenItemReceiptAllocation();
                        openItemReceiptAllocation.OpenItemID = openItem.OpenItemID;
                        openItemReceiptAllocation.GrossAmountDue = openItem.AmountDue;
                        openItemReceiptAllocation.GrossAmountApplied = Convert.ToDecimal(openItem.AmountApplied);

                        receipt.OpenItemReceiptAllocations.Add(openItemReceiptAllocation);
                    }

                    receipts.Add(receipt);
                }

                if (receipts.Where(item => item.ReceiptDate > DateTime.Today).Count() > 0)
                {
                    UIConfirmation.Raise(
                           new ConfirmationWindowViewModel(this) { Content = "One or more Receipt Date(s) are in the future. Select OK to continue or Cancel to modify", Icon = "Question.ico", Title = "Receipt Date Confirmation" },
                           (popupCallBack) =>
                           {
                               if (popupCallBack.Confirmed)
                               {
                                   ReceiptFunctions.Save(receipts);
                                   Close();
                               }
                           });
                }
                else
                {
                    ReceiptFunctions.Save(receipts);
                    Close();
                }

                IsBusy = false;
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error encountered while saving Excel entries.", "Excel Import - Error");
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
            CashReceiptExcel receipt;
            List<CashReceiptExcel> receipts;
            DateTime receiptDate;
            double amount;
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
                receipts = new List<CashReceiptExcel>();

                foreach (DataRow row in excelData.Rows)
                {
                    receipt = new CashReceiptExcel();
                    isRowEmpty = true;

                    if (row["Account Name"] != null && string.IsNullOrEmpty(row["Account Name"].ToString()) == false)
                    {
                        receipt.AccountName = row["Account Name"].ToString();
                        isRowEmpty = false;
                    }

                    receipt.AmountReceived = 0;

                    if (row["Amount Received"] != null && string.IsNullOrEmpty(row["Amount Received"].ToString()) == false && double.TryParse(row["Amount Received"].ToString(), out amount))
                    {
                        receipt.AmountReceived = amount;
                        isRowEmpty = false;
                    }

                    if (row["Bank Name"] != null && string.IsNullOrEmpty(row["Bank Name"].ToString()) == false)
                    {
                        receipt.BankName = row["Bank Name"].ToString();
                        isRowEmpty = false;
                    }

                    if (row["BSB Number"] != null && string.IsNullOrEmpty(row["BSB Number"].ToString()) == false)
                    {
                        receipt.BSBNumber = row["BSB Number"].ToString();
                        isRowEmpty = false;
                    }

                    if (row["Cheque Number"] != null && string.IsNullOrEmpty(row["Cheque Number"].ToString()) == false)
                    {
                        receipt.ChequeNumber = row["Cheque Number"].ToString();
                        isRowEmpty = false;
                    }

                    if (row["Contract ID"] != null && string.IsNullOrEmpty(row["Contract ID"].ToString()) == false)
                    {
                        receipt.ContractID = row["Contract ID"].ToString();
                        isRowEmpty = false;
                    }

                    if (row["Receipt Type"] != null && string.IsNullOrEmpty(row["Receipt Type"].ToString()) == false)
                    {
                        receipt.PaymentMethod = row["Receipt Type"].ToString();
                        isRowEmpty = false;
                    }

                    if (row["Date Received"] != null && string.IsNullOrEmpty(row["Date Received"].ToString()) == false)
                    {
                        if (DateTime.TryParse(row["Date Received"].ToString(), out receiptDate))
                        {
                            receipt.ReceiptDate = receiptDate.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            receipt.ReceiptDate = row["Date Received"].ToString();
                        }

                        isRowEmpty = false;
                    }

                    if (row["Reference"] != null && string.IsNullOrEmpty(row["Reference"].ToString()) == false)
                    {
                        receipt.ReferenceNo = row["Reference"].ToString();
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

            if (templateData.Columns.Count != excelData.Columns.Count)
            {
                return false;
            }

            for (int index = 0; index < templateData.Columns.Count; index++)
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

            if (File.Exists(folderPath + "CashReceiptImportTemplate.xlsx"))
            {
                try
                {
                    File.Delete(folderPath + "CashReceiptImportTemplate.xlsx");
                }
                catch
                {
                }
            }

            if (File.Exists(folderPath + "CashReceiptImportTemplate.xlsx") == false)
            {
                using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(this.GetType().Assembly.GetName().Name + ".Templates.CashReceiptImportTemplate.xlsx"))
                {
                    file = new FileStream(folderPath + "CashReceiptImportTemplate.xlsx", FileMode.Create);
                    byte[] result = new byte[stream.Length];
                    stream.Read(result, 0, (int)stream.Length);
                    file.Write(result, 0, result.Length);
                    file.Close();
                }
            }

            return folderPath + "CashReceiptImportTemplate.xlsx";
        }
    }
}
