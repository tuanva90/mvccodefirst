using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data;
using System.Data.OleDb;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Telerik.Windows.Controls;
using Insyston.Operations.Base;
using System.ComponentModel.Composition;
using Insyston.Operations.WPF.View.Common;
using Insyston.Operations.WPF.ViewModel.Receipts.CashCheque;

namespace Insyston.Operations.WPF.View.Receipts.CashCheque
{
    public partial class CashReceiptExcelImport : OperationsView
    {
        public CashReceiptExcelImport()
        {
            InitializeComponent();
        }

        public CashReceiptExcelImport(int receiptBatchID) 
        {
            InitializeComponent();
            CashReceiptExcelImportViewModel = new CashReceiptExcelImportViewModel(receiptBatchID);
        }

        public CashReceiptExcelImportViewModel CashReceiptExcelImportViewModel 
        {
            get
            {
                return this.DataContext as CashReceiptExcelImportViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }       
    }
}
