using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ddFileTypes = Insyston.Operations.Business.Receipts.BankFileGenerator.FileTypes.DirectDebit;
using ccFileTypes = Insyston.Operations.Business.Receipts.BankFileGenerator.FileTypes.CreditCard;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Receipts.Model;
using System.Windows.Forms;
using Insyston.Operations.Security;
using receipts = Insyston.Operations.Business.Receipts;
using Insyston.Operations.WPF.View.Receipts;
using System.Threading;
using Insyston.Operations.Business.Receipts.BankFileGenerator.FileTypes;
using Insyston.Operations.Business.Receipts.BankFileGenerator;

namespace Insyston.Operations.WPF.ViewModel.Receipts.DDCC.ViewModel
{
    public class DDCCStatusChangeViewModel : OldViewModelBase, IDataErrorInfo
    {
        ReceiptBatchType batchType;
        private DateTime lodgeDate;
        private string fileLocation, archiveLocation;
        private int id;

        private List<DropdownList> internalCompanyBankList;
        private List<ReceiptBatchSummary> receiptBatches;
        private ObservableCollection<object> selectedReceiptBatches;

        private bool isInternalCompanySelected, isLocationChanged, isBusy, isChanged=false;
        private string title;

        public event ClearGridFilterHandler ClearGridFilter;

        public DelegateCommand<string> Browse { get; private set; }
        public DelegateCommand Refresh { get; private set; }
        public DelegateCommand Lodge { get; private set; }
        public DelegateCommand<ObservableCollection<object>> ReceiptBatchSelected { get; private set; }

        public DDCCStatusChangeViewModel(ReceiptBatchType batchtype)
        {
            batchType = batchtype;
            LodgeDate = DateTime.Today;
            IsInternalCompanySelected = true;            
            receipts.Utilities.GetDDCCFileLocations(batchtype, out fileLocation, out archiveLocation);
            isLocationChanged = false;

            Browse = new DelegateCommand<string>(OnBrowse);
            Refresh = new DelegateCommand(OnRefresh);
            Lodge = new DelegateCommand(OnLodge);
            ReceiptBatchSelected = new DelegateCommand<ObservableCollection<object>>(OnReceiptBatchSelected);
            OnRefresh();

            IconFileName = "Forward.jpg";
            Title = "Lodge" + Regex.Replace(batchType.ToString(), "[A-Z]", " $0") + " Batch";
            RaisePropertyChanged("RemitterColumnCaption");

            LockTableName = "ReceiptBatch";
        }

        public DateTime LodgeDate
        {
            get
            {
                return lodgeDate;
            }
            set
            {
                if (lodgeDate != value)
                {
                    lodgeDate = new DateTime(value.Year, value.Month, value.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    RaisePropertyChanged("LodgeDate");
                }
            }
        }        

        public string FileLocation
        {
            get
            {
                return fileLocation;
            }
            set
            {
                if (fileLocation != value)
                {                    
                    fileLocation = value;                    
                    isLocationChanged = true;
                    RaisePropertyChanged("FileLocation");
                }
            }
        }

        public string ArchiveLocation
        {
            get
            {
                return archiveLocation;
            }
            set
            {
                if (archiveLocation != value)
                {                   
                    archiveLocation = value;
                    isLocationChanged = true;
                    RaisePropertyChanged("ArchiveLocation");
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

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;
                    RaisePropertyChanged("ID");
                }
            }
        }

        public List<DropdownList> InternalCompanyBankList
        {
            get
            {
                return internalCompanyBankList;
            }
            set
            {
                if (internalCompanyBankList != value)
                {
                    internalCompanyBankList = value;
                    RaisePropertyChanged("InternalCompanyBankList");
                }
            }
        }

        public List<ReceiptBatchSummary> ReceiptBatches
        {
            get
            {
                return receiptBatches;
            }
            set
            {
                if (receiptBatches != value)
                {
                    receiptBatches = value;
                    RaisePropertyChanged("ReceiptBatches");
                }
            }
        }

        public ObservableCollection<object> SelectedReceiptBatches
        {
            get
            {
                return selectedReceiptBatches;
            }
            set
            {
                if (selectedReceiptBatches != value)
                {
                    selectedReceiptBatches = value;
                    RaisePropertyChanged("SelectedReceiptBatches");
                }
            }
        }

        public string RemitterColumnCaption
        {
            get
            {
                if (batchType == ReceiptBatchType.DirectDebit)
                {
                    return "Remitter Name";
                }
                else
                {
                    return "CDTF Account";
                }
            }
        }

        public bool IsInternalCompanySelected
        {
            get
            {
                return isInternalCompanySelected;
            }
            set
            {
                if (isInternalCompanySelected != value)
                {
                    isInternalCompanySelected = value;
                    InternalCompanyBankList = DDCCBatchFunctions.GetLodgeInternalCompanyList(batchType);
                    ID = -1;

                    if (internalCompanyBankList.Count > 0)
                    {
                        ID = 0;
                    }

                    RaisePropertyChanged("IsInternalCompanySelected");
                }
            }
        }

        public bool IsBankSelected
        {
            get
            {
                return !isInternalCompanySelected;
            }
            set
            {
                if (isInternalCompanySelected != !value)
                {
                    isInternalCompanySelected = !value;
                    InternalCompanyBankList = DDCCBatchFunctions.GetLodgeBanksList(batchType);
                    ID = -1;

                    if (internalCompanyBankList.Count > 0)
                    {
                        ID = 0;
                    }

                    RaisePropertyChanged("IsBankSelected");
                }
            }
        }

        public bool IsLodgeEnabled
        {
            get
            {
                return selectedReceiptBatches != null && selectedReceiptBatches.Count > 0;
            }
        }

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    RaisePropertyChanged("IsBusy");
                }
            }
        }

        private void OnRefresh()
        {
            if(isLocationChanged)
            {
                if (ValidateFolderPath("File", fileLocation) == false && ValidateFolderPath("Archive", archiveLocation) == false)
                {
                    receipts.Utilities.SaveDDCCFileLocations(batchType, fileLocation, archiveLocation);
                    receipts.Utilities.SaveDDCCFileLocations(batchType, fileLocation, archiveLocation);
                }                
            }

            IsBusy = true;
            ReceiptBatches = DDCCBatchFunctions.GetReceiptBatchCreatedList(batchType, isInternalCompanySelected, id);

            if (ClearGridFilter != null)
            {
                ClearGridFilter();
            }

            RaisePropertyChanged("IsLodgeEnabled");
            IsBusy = false;
        }

        private void OnLodge()
        {
            StringBuilder batchIDs;
            DDFileTypes ddFileType;
            CCFileTypes ccFileType;
            int fileTypeID = -1;
            isChanged = true;

            if (Validate())
            {
                IsBusy = true;
                batchIDs = new StringBuilder();

                if (isLocationChanged)
                {
                    receipts.Utilities.SaveDDCCFileLocations(batchType, fileLocation, archiveLocation);
                }

                foreach (ReceiptBatchSummary batch in selectedReceiptBatches)
                {                    
                    if (batchType == ReceiptBatchType.DirectDebit)
                    {
                        if (Enum.IsDefined(typeof(DDFileTypes), batch.FileTypeID))
                        {
                            fileTypeID = batch.FileTypeID;
                        }
                    }
                    else
                    {
                        if (Enum.IsDefined(typeof(CCFileTypes), batch.FileTypeID))
                        {
                            fileTypeID = batch.FileTypeID;
                        }
                    }

                    if (fileTypeID != -1)
                    {
                        LockUniqueIdentifier = batch.BatchID.ToString();
                        Lock();

                        if (IsLocked == false)
                        {
                            IsBusy = false;
                            return;
                        }
                    }                    
                }

                foreach (ReceiptBatchSummary batch in selectedReceiptBatches)
                {
                    if(batchType == ReceiptBatchType.DirectDebit)
                    {
                        if (Enum.IsDefined(typeof(DDFileTypes), batch.FileTypeID))
                        {
                            fileTypeID = batch.FileTypeID;
                        }
                    }
                    else
                    {
                        if (Enum.IsDefined(typeof(CCFileTypes), batch.FileTypeID))
                        {
                            fileTypeID = batch.FileTypeID;
                        }
                    }

                    if (fileTypeID != -1)
                    {
                        ReceiptBatchFunctions.UpdateStatus(batch.BatchID, (int)ReceiptBatchStatus.Pending, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId);
                        CreateFile(batch.BatchID, batch.FileTypeID, fileTypeID);
                        Shared.MoveNavigationItem(new NavigationItem { BatchStatus = (int)ReceiptBatchStatus.Created, BatchMonth = batch.BatchMonth, ReceiptID = batch.BatchID }, (int)ReceiptBatchStatus.Pending, true);
                    }
                    else
                    {
                        if(batchIDs.Length > 0)
                        {
                            batchIDs.Append(",");
                        }

                        batchIDs.Append(batch.BatchID.ToString());
                    }
                }

                foreach (ReceiptBatchSummary batch in selectedReceiptBatches)
                {
                    if (batchType == ReceiptBatchType.DirectDebit)
                    {
                        if (Enum.IsDefined(typeof(DDFileTypes), batch.FileTypeID))
                        {
                            fileTypeID = batch.FileTypeID;
                        }
                    }
                    else
                    {
                        if (Enum.IsDefined(typeof(CCFileTypes), batch.FileTypeID))
                        {
                            fileTypeID = batch.FileTypeID;
                        }
                    }

                    if (fileTypeID != -1)
                    {
                        LockUniqueIdentifier = batch.BatchID.ToString();
                        UnLock();
                    }
                }

                if (batchIDs.Length > 0)
                {
                    ShowMessage("System encountered Unrecognized File Type(s) for one or more Batches.\n\nPlease fix the File Type for Batche(s) : " + batchIDs.ToString(), "Invalid File Type",
                                                    callBack => { IsBusy = false; });
                }
                else
                {
                    IsBusy = false;
                    Close();
                }
            }
        }

        private void OnBrowse(string location)
        {
            FolderBrowserDialog folderDialog;

            folderDialog = new FolderBrowserDialog();
            if (location.Trim().ToUpper() == "FILE")
            {
                folderDialog.SelectedPath = fileLocation;
                folderDialog.Description = "Select folder for File Location";
                folderDialog.ShowDialog();
                FileLocation = folderDialog.SelectedPath;
            }
            else
            {
                folderDialog.SelectedPath = archiveLocation;
                folderDialog.Description = "Select folder for Archive Location";
                folderDialog.ShowDialog();
                ArchiveLocation = folderDialog.SelectedPath;
            }             
        }
       
        private void OnReceiptBatchSelected(ObservableCollection<object> selectedItems)
        {
            selectedReceiptBatches = selectedItems;
            RaisePropertyChanged("IsLodgeEnabled");
        }

        private bool CreateFile(int batchID, int batchFileTypeID, int fileTypeID)
        {
            FileType fileType = null;
            char separator = default(char);

            try
            {
                if(batchType == ReceiptBatchType.DirectDebit)
                {
                    switch((DDFileTypes)Enum.Parse(typeof(DDFileTypes), fileTypeID.ToString()))
                    {
                        case DDFileTypes.Type_1:
                            fileType = new ddFileTypes.FileType1(batchID, DDFileTypes.Type_1, lodgeDate, fileLocation, archiveLocation);
                            break;
                        case DDFileTypes.Type1:
                            fileType = new ddFileTypes.FileType1(batchID, DDFileTypes.Type1, lodgeDate, fileLocation, archiveLocation);
                            break;
                        case DDFileTypes.Type2:    
                            fileType = new ddFileTypes.FileType1(batchID, DDFileTypes.Type2, lodgeDate, fileLocation, archiveLocation);
                            break;
                        case DDFileTypes.PeopleSoft:
                            fileType = new ddFileTypes.FileType4(batchID, lodgeDate, fileLocation, archiveLocation);
                            separator = ',';
                            break;
                        case DDFileTypes.HSBC_NZ:
                            fileType = new ddFileTypes.FileType5(batchID, lodgeDate, fileLocation, archiveLocation);
                            break;
                        case DDFileTypes.ASB_NZ:
                            fileType = new ddFileTypes.FileType6(batchID, lodgeDate, fileLocation, archiveLocation);
                            break;
                        case DDFileTypes.ANZ_NZ:
                            fileType = new ddFileTypes.FileType7(batchID, lodgeDate, fileLocation, archiveLocation);
                            separator = ',';
                            break;
                    }
                }
                else if(batchType == ReceiptBatchType.CreditCard)
                {
                    switch ((CCFileTypes)Enum.Parse(typeof(CCFileTypes), fileTypeID.ToString()))
                    {
                        case CCFileTypes.Type1:
                            fileType = new ccFileTypes.FileType1(batchID, lodgeDate, fileLocation, archiveLocation);
                            separator = ',';
                            break;
                        case CCFileTypes.BPoint:
                            fileType = new ccFileTypes.FileType2(batchID, lodgeDate, fileLocation, archiveLocation);
                            separator = ',';
                            break;
                    }
                }

                if (fileType != null)
                {
                    Serializer.Serialize(fileType, separator);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        } 

        private bool ValidateFolderPath(string location, string path)
        {
            DirectoryInfo directory;

            if (string.IsNullOrEmpty(path) == false)
            {
                directory = new DirectoryInfo(path);

                if (!directory.Exists)
                {
                    ShowMessage(location + " Location Path is Invalid.\nPlease select a valid folder for " + location, location + " Location");
                    OnBrowse(location);
                    return true;
                }
            }
            else
            {
                ShowMessage(location + " Location Path is Invalid.\nPlease select a valid folder for " + location, location + " Location");
                OnBrowse(location);
                return true;
            }

            return false;
        }
        
        public string this[string columnName]
        {
            get 
            {
                if (isChanged == false)
                {
                    return string.Empty;
                }

                if (columnName == "FileLocation")
                {
                    if (ValidateFolderPath("File", fileLocation))
                    {
                        return "File Location is Invalid";
                    }
                    else if (string.Compare(fileLocation, archiveLocation, true) == 0)
                    {
                        return "File & Archive Locations must be different";
                    }
                }
                else if(columnName == "ArchiveLocation")
                {
                    if (ValidateFolderPath("Archive", archiveLocation))
                    {
                        return "Archive Location is Invalid";
                    }
                    else if (string.Compare(fileLocation, archiveLocation, true) == 0)
                    {
                        return "File & Archive Locations must be different";
                    }
                }

                return string.Empty;
            }
        }

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

    }
}
