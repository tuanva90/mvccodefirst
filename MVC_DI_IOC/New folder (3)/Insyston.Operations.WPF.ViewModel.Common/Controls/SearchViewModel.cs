using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Insyston.Operations.Business.Common;
using Insyston.Operations.Logging;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using Insyston.Operations.WPF.ViewModels.Common.WindowManager;

    public class SearchViewModel<T> : OldViewModelBase 
    {
        private readonly DelegateSearch<T> _DelegateSearch;
        private readonly DelegateSearchFilter<T> _DelegateSearchFilter;
        private readonly object[] _Filters;

        private List<string> _Columns;
        private Dictionary<string, List<string>> _FilterOperators;
        private List<T> _Results;
        private T _SelectedRow;
        private string _Column, _FilterOperator, _FilterValue;
        private DateTime _FilterFromDate, _FilterToDate;
        private string _Title;
        private int _Count = 0;

        public SearchViewModel()
        {
        }

        public SearchViewModel(DelegateSearch<T> delSearch, string _Title)
        {
            this._DelegateSearch = delSearch;
            this.IconFileName = "Search.jpg";
            this.Title = _Title;
            this.FilterFromDate = DateTime.Today;
            this.FilterToDate = DateTime.Today;

            //this.Search = new DelegateCommand(this.OnSearch);
            //this.Ok = new DelegateCommand(this.OnOk);
        }

        public SearchViewModel(DelegateSearchFilter<T> delSearch, string _Title, params object[] filter)
        {
            this._DelegateSearchFilter = delSearch;
            this.IconFileName = "Search.jpg";
            this.Title = _Title;
            this.FilterFromDate = DateTime.Today;
            this.FilterToDate = DateTime.Today;
            this._Filters = filter;

            //this.Search = new DelegateCommand(this.OnSearch);
            //this.Ok = new DelegateCommand(this.OnOk);
        }

        public delegate void ClearGridFilterHandler();

        public event ClearGridFilterHandler ClearGridFilter;

        //public DelegateCommand Search { get; private set; }

        //public DelegateCommand Ok { get; private set; }

        public List<T> Results
        {
            get
            {
                return this._Results;
            }
            set
            {
                if (this._Results != value)
                {
                    this._Results = value;
                    this.RaisePropertyChanged("Results");
                }
            }
        }

        public T SelectedRow
        {
            get
            {
                return this._SelectedRow;
            }
            set
            {
                this._SelectedRow = value;
                this.RaisePropertyChanged("SelectedRow");
                this.RaisePropertyChanged("IsRowSelected");
            }
        }

        public List<string> Columns
        {
            get
            {
                return this._Columns;
            }
            set
            {
                if (this._Columns != value)
                {
                    this._Columns = value;
                    this.RaisePropertyChanged("Columns");
                }
            }
        }

        public List<string> FilterOperators
        {
            get
            {
                if (string.IsNullOrEmpty(this._Column) || this._FilterOperators == null || this._FilterOperators.Keys.Where(key => key == this._Column.Replace(" ", string.Empty)).Count() == 0)
                {
                    return null;
                }

                return this._FilterOperators[this._Column.Replace(" ", string.Empty)].ToList();
            }
            set
            {
            }
        }

        public string Column
        {
            get
            {
                return this._Column;
            }
            set
            {
                if (this._Column != value)
                {
                    this._Column = value;
                    this.RaisePropertyChanged("Column");
                    this.RaisePropertyChanged("FilterOperators");
                    this.RaisePropertyChanged("IsDateColumn");
                    this.FilterOperator = "No Filter";
                    this.RaisePropertyChanged("FromDateErrorMessage");
                }
            }
        }

        public string FilterOperator
        {
            get
            {
                return this._FilterOperator;
            }
            set
            {
                if (this._FilterOperator != value)
                {
                    this._FilterOperator = value;
                    this.RaisePropertyChanged("FilterOperator");
                    this.RaisePropertyChanged("IsFilterOperator");
                    this.RaisePropertyChanged("IsBetweenFilterOperator");
                    this.RaisePropertyChanged("FromDateErrorMessage");
                }
            }
        }

        public string FilterValue
        {
            get
            {
                return this._FilterValue;
            }
            set
            {
                if (this._FilterValue != value)
                {
                    this._FilterValue = value;
                    this.RaisePropertyChanged("FilterValue");
                }
            }
        }

        public DateTime FilterFromDate
        {
            get
            {
                return this._FilterFromDate;
            }
            set
            {
                if (this._FilterFromDate != value)
                {
                    this._FilterFromDate = value;
                    this.RaisePropertyChanged("FilterFromDate");
                }
            }
        }

        public DateTime FilterToDate
        {
            get
            {
                return this._FilterToDate;
            }
            set
            {
                if (this._FilterToDate != value)
                {
                    this._FilterToDate = value;
                    this.RaisePropertyChanged("FilterToDate");
                }
            }
        }

        public T ReturnValue
        {
            get
            {
                return this._SelectedRow;
            }
            set
            {
                this.RaisePropertyChanged("ReturnValue");
            }
        }

        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                if (this._Title != value)
                {
                    this._Title = value;
                    this.RaisePropertyChanged("Title");
                }
            }
        }

        public string FromDateErrorMessage
        {
            get
            {
                if (this.IsDateColumn && this.IsBetweenFilterOperator)
                {
                    return "From Date";
                }
                else
                {
                    return "Date";
                }
            }
            set
            {
            }
        }

        public bool IsDateColumn
        {
            get
            {
                if (string.IsNullOrEmpty(this._Column))
                {
                    return false;
                }

                return (typeof(T).GetProperty(this._Column.Replace(" ", string.Empty)).PropertyType == typeof(DateTime));
            }
            set
            {
            }
        }

        public bool IsFilterOperator
        {
            get
            {
                if (string.IsNullOrEmpty(this.FilterOperator))
                {
                    return false;
                }

                return (this.FilterOperator.ToLower() != "no filter");
            }
        }

        public bool IsBetweenFilterOperator
        {
            get
            {
                if (string.IsNullOrEmpty(this.FilterOperator))
                {
                    return false;
                }

                if (this.FilterOperator.ToLower() != "between")
                {
                    this.FilterToDate = DateTime.Today;
                }

                return (this.FilterOperator.ToLower() == "between");
            }
        }

        public bool IsRowSelected
        {
            get
            {
                return (this.SelectedRow != null);
            }
        }

        public string RecordCountText
        {
            get
            {
                if (this._Count > Constants.SearchRecordCount)
                {
                    return string.Format("The Number of records returned exceeded the limit, Result shows only the first {0} records. Please refine your search.", Constants.SearchRecordCount.ToString());
                }

                return string.Empty;
            }
        }

        public Visibility IsCountTextAvailble
        {
            get
            {
                return this._Count > Constants.SearchRecordCount ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (this.IsDateColumn)
                {
                    if (string.IsNullOrEmpty(this._FilterFromDate.ToString()) == true)
                    {
                        if (this.IsBetweenFilterOperator)
                        {
                            return "From Date is Required";
                        }
                        else
                        {
                            return "Date is Required";
                        }
                    }

                    if (string.IsNullOrEmpty(this._FilterToDate.ToString()) == true)
                    {
                        return "To Date is Required";
                    }

                    if (this.FilterToDate < this._FilterFromDate)
                    {
                        return "To Date should be greater than From Date";
                    }
                }

                return string.Empty;
            }
        }

        public void SetFilterOperators(Dictionary<string, List<string>> columnOperators)
        {
            List<string> columnFilterOperators;

            if (this._FilterOperators.Keys.Where(key => key == columnOperators.FirstOrDefault().Key).Count() > 0)
            {
                return;
            }

            if (typeof(T).GetProperty(columnOperators.FirstOrDefault().Key) != null && typeof(T).GetProperty(columnOperators.FirstOrDefault().Key).PropertyType == typeof(DateTime))
            {
                columnOperators.FirstOrDefault().Value.Add("Between");
            }

            columnFilterOperators = columnOperators.FirstOrDefault().Value.OrderBy(item => item).ToList();
            columnFilterOperators.Insert(0, "No Filter");

            this._FilterOperators.Add(columnOperators.FirstOrDefault().Key, columnFilterOperators);

            if (string.IsNullOrEmpty(this._Column))
            {
                this.Column = this._Columns.FirstOrDefault();
            }
        }

        public void Initialize()
        {
            this.Columns = Utilities.GetProperties<T>();
            this._FilterOperators = new Dictionary<string, List<string>>();

            if (this._DelegateSearch != null)
            {
                this.Results = this._DelegateSearch.Invoke(string.Empty, string.Empty, string.Empty, out this._Count);
            }
            else
            {
                this.Results = this._DelegateSearchFilter.Invoke(string.Empty, string.Empty, string.Empty, out this._Count, null, this._Filters);
            }
        }

        public void OnOk()
        {
            this.ReturnValue = this._SelectedRow;
            this.Close();
        }

        private void OnSearch()
        {
            string strColumn;

            try
            {
                if (this.Validate())
                {
                    strColumn = this._Column.Replace(" ", string.Empty);

                    this.IsBusy = true;

                    if (this.IsDateColumn)
                    {
                        if (this.IsBetweenFilterOperator)
                        {
                            if (this._DelegateSearch != null)
                            {
                                this.Results = this._DelegateSearch.Invoke(strColumn, this._FilterOperator, this._FilterFromDate, out this._Count, this._FilterToDate);
                            }
                            else
                            {
                                this.Results = this._DelegateSearchFilter.Invoke(strColumn, this._FilterOperator, this._FilterFromDate, out this._Count, this._FilterToDate, this._Filters);
                            }
                        }
                        else
                        {
                            if (this._DelegateSearch != null)
                            {
                                this.Results = this._DelegateSearch.Invoke(strColumn, this._FilterOperator, this._FilterFromDate, out this._Count);
                            }
                            else
                            {
                                this.Results = this._DelegateSearchFilter.Invoke(strColumn, this._FilterOperator, this._FilterFromDate, out this._Count, null, this._Filters);
                            }
                        }
                    }
                    else
                    {
                        if (this._DelegateSearch != null)
                        {
                            this.Results = this._DelegateSearch.Invoke(strColumn, this._FilterOperator, this._FilterValue, out this._Count);
                        }
                        else
                        {
                            this.Results = this._DelegateSearchFilter.Invoke(strColumn, this._FilterOperator, this._FilterValue, out this._Count, null, this._Filters);
                        }
                    }

                    this.RaisePropertyChanged("RecordCountText");
                    this.RaisePropertyChanged("IsCountTextAvailble");
                    this.ClearGridFilter();
                    this.IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                TelerikWindowManager.Alert("Search - Error","Error encountered during Search");
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}
