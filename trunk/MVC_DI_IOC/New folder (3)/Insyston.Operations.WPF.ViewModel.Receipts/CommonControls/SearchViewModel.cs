using Insyston.Operations.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Events;
using Insyston.Operations.View.OpenItems.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using Insyston.Operations.Logging;

namespace Insyston.Operations.View.OpenItems.Receipts.CommonControls.ViewModel
{
    public class SearchViewModel<T> : ViewModelBase //, IDataErrorInfo where T : class
    {
        private readonly DelegateSearch<T> delegateSearch;
        private readonly DelegateSearchFilter<T> delegateSearchFilter;
        private object[] filters;

        private List<string> columns;
        private Dictionary<string, List<string>> filterOperators;
        private List<T> results;
        T selectedRow;
        private string column, filterOperator, filterValue;
        private DateTime filterFromDate, filterToDate;
        private string title;
        private int count = 0;

        public delegate void ClearGridFilterHandler();
        public event ClearGridFilterHandler ClearGridFilter;

        public DelegateCommand Search { get; private set; }
        public DelegateCommand Ok { get; private set; }

        public SearchViewModel()
        {
        }

        public SearchViewModel(DelegateSearch<T> delSearch, string title)
        {
            delegateSearch = delSearch;
            IconFileName = "/OpenItems/Search.ico";
            Title = title;
            FilterFromDate = DateTime.Today;
            FilterToDate = DateTime.Today;

            Search = new DelegateCommand(OnSearch);
            Ok = new DelegateCommand(OnOk);
        }

        public SearchViewModel(DelegateSearchFilter<T> delSearch, string title, params object[] filter)
        {
            delegateSearchFilter = delSearch;
            IconFileName = "/OpenItems/Search.ico";
            Title = title;
            FilterFromDate = DateTime.Today;
            FilterToDate = DateTime.Today;
            filters = filter;

            Search = new DelegateCommand(OnSearch);
            Ok = new DelegateCommand(OnOk);
        }

        public void SetFilterOperators(Dictionary<string, List<string>> columnOperators)
        {
            List<string> columnFilterOperators;

            if (filterOperators.Keys.Where(key => key == columnOperators.FirstOrDefault().Key).Count() > 0)
            {
                return;
            }

            if (typeof(T).GetProperty(columnOperators.FirstOrDefault().Key) != null && typeof(T).GetProperty(columnOperators.FirstOrDefault().Key).PropertyType == typeof(DateTime))
            {
                columnOperators.FirstOrDefault().Value.Add("Between");
            }

            columnFilterOperators = columnOperators.FirstOrDefault().Value.OrderBy(item => item).ToList();
            columnFilterOperators.Insert(0, "No Filter");

            filterOperators.Add(columnOperators.FirstOrDefault().Key, columnFilterOperators);

            if (string.IsNullOrEmpty(column))
            {
                Column = columns.FirstOrDefault();
            }
        }

        public List<T> Results
        {
            get
            {
                return results;
            }
            set
            {
                if (results != value)
                {
                    results = value;
                    RaisePropertyChanged("Results");
                }
            }
        }

        public T SelectedRow
        {
            get
            {
                return selectedRow;
            }
            set
            {
                selectedRow = value;
                RaisePropertyChanged("SelectedRow");
                RaisePropertyChanged("IsRowSelected");
            }
        }

        public List<string> Columns
        {
            get
            {
                return columns;
            }
            set
            {
                if (columns != value)
                {
                    columns = value;
                    RaisePropertyChanged("Columns");
                }
            }
        }

        public List<string> FilterOperators
        {
            get
            {
                if (string.IsNullOrEmpty(column) || filterOperators == null || filterOperators.Keys.Where(key => key == column.Replace(" ", string.Empty)).Count() == 0)
                {
                    return null;
                }

                return filterOperators[column.Replace(" ", string.Empty)].ToList();
            }
            set
            {
            }
        }

        public string Column
        {
            get
            {
                return column;
            }
            set
            {
                if (column != value)
                {
                    column = value;
                    RaisePropertyChanged("Column");
                    RaisePropertyChanged("FilterOperators");
                    RaisePropertyChanged("IsDateColumn");
                    FilterOperator = "No Filter";
                    RaisePropertyChanged("FromDateErrorMessage");
                }
            }
        }

        public string FilterOperator
        {
            get
            {
                return filterOperator;
            }
            set
            {
                if (filterOperator != value)
                {
                    filterOperator = value;
                    RaisePropertyChanged("FilterOperator");
                    RaisePropertyChanged("IsFilterOperator");
                    RaisePropertyChanged("IsBetweenFilterOperator");
                    RaisePropertyChanged("FromDateErrorMessage");
                }
            }
        }

        public string FilterValue
        {
            get
            {
                return filterValue;
            }
            set
            {
                if (filterValue != value)
                {
                    filterValue = value;
                    RaisePropertyChanged("FilterValue");
                }
            }
        }

        public DateTime FilterFromDate
        {
            get
            {
                return filterFromDate;
            }
            set
            {
                if (filterFromDate != value)
                {
                    filterFromDate = value;
                    RaisePropertyChanged("FilterFromDate");
                }
            }
        }

        public DateTime FilterToDate
        {
            get
            {
                return filterToDate;
            }
            set
            {
                if (filterToDate != value)
                {
                    filterToDate = value;
                    RaisePropertyChanged("FilterToDate");
                }
            }
        }

        public T ReturnValue
        {
            get
            {
                return selectedRow;
            }
            set
            {
                RaisePropertyChanged("ReturnValue");
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

        public string FromDateErrorMessage
        {
            get
            {
                if (IsDateColumn && IsBetweenFilterOperator)
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
                if (string.IsNullOrEmpty(column))
                {
                    return false;
                }

                return (typeof(T).GetProperty(column.Replace(" ", string.Empty)).PropertyType == typeof(DateTime));
            }
            set
            {
            }
        }

        public bool IsFilterOperator
        {
            get
            {
                if (string.IsNullOrEmpty(FilterOperator))
                {
                    return false;
                }

                return (FilterOperator.ToLower() != "no filter");
            }
        }

        public bool IsBetweenFilterOperator
        {
            get
            {
                if (string.IsNullOrEmpty(FilterOperator))
                {
                    return false;
                }

                if (FilterOperator.ToLower() != "between")
                {
                    FilterToDate = DateTime.Today;
                }

                return (FilterOperator.ToLower() == "between");
            }
        }    

        public bool IsRowSelected
        {
            get
            {
                return (SelectedRow != null);
            }
        }

        public string RecordCountText
        {
            get
            {
                if (count > Business.OpenItems.Common.SearchRecordCount)
                {
                    return "The Number of records returned exceeded the limit, Result shows only the first " + Business.OpenItems.Common.SearchRecordCount.ToString() + " records. Please refine your search.";
                }

                return string.Empty;
            }
        }

        public Visibility IsCountTextAvailble
        {
            get
            {
                return count > Insyston.Operations.Business.OpenItems.Common.SearchRecordCount ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void Initialize()
        {
            Columns = Utilities.GetProperties<T>();
            filterOperators = new Dictionary<string, List<string>>();

            if (delegateSearch != null)
            {
                Results = delegateSearch.Invoke(string.Empty, string.Empty, string.Empty, out count);
            }
            else
            {
                Results = delegateSearchFilter.Invoke(string.Empty, string.Empty, string.Empty, out count, null, filters);
            }
        }

        private void OnSearch()
        {
            string strColumn;

            try
            {
                if (Validate())
                {
                    strColumn = column.Replace(" ", string.Empty);

                    IsBusy = true;

                    if (IsDateColumn)
                    {
                        if (IsBetweenFilterOperator)
                        {
                            if (delegateSearch != null)
                            {
                                Results = delegateSearch.Invoke(strColumn, filterOperator, filterFromDate, out count, filterToDate);
                            }
                            else
                            {
                                Results = delegateSearchFilter.Invoke(strColumn, filterOperator, filterFromDate, out count, filterToDate, filters);
                            }
                        }
                        else
                        {
                            if (delegateSearch != null)
                            {
                                Results = delegateSearch.Invoke(strColumn, filterOperator, filterFromDate, out count);
                            }
                            else
                            {
                                Results = delegateSearchFilter.Invoke(strColumn, filterOperator, filterFromDate, out count, null, filters);
                            }
                        }
                    }
                    else
                    {
                        if (delegateSearch != null)
                        {
                            Results = delegateSearch.Invoke(strColumn, filterOperator, filterValue, out count);
                        }
                        else
                        {
                            Results = delegateSearchFilter.Invoke(strColumn, filterOperator, filterValue, out count, null, filters);
                        }
                    }

                    RaisePropertyChanged("RecordCountText");
                    RaisePropertyChanged("IsCountTextAvailble");
                    ClearGridFilter();
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error encountered during Search", "Search - Error");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnOk()
        {
            ReturnValue = selectedRow;
            Close();
        }

        public string this[string columnName]
        {
            get
            {
                if (IsDateColumn)
                {
                    if (string.IsNullOrEmpty(filterFromDate.ToString()) == true)
                    {
                        if (IsBetweenFilterOperator)
                        {
                            return "From Date is Required";
                        }
                        else
                        {
                            return "Date is Required";
                        }
                    }

                    if (string.IsNullOrEmpty(filterToDate.ToString()) == true)
                    {
                        return "To Date is Required";
                    }

                    if (FilterToDate < filterFromDate)
                    {
                        return "To Date should be greater than From Date";
                    }
                }

                return string.Empty;
            }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }       
    }
}
