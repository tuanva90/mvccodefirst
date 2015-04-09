using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Insyston.Operations.WPF.Views.Funding.FilterControls;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Insyston.Operations.WPF.ViewModels.Funding;
using Telerik.Windows.Data;
using Insyston.Operations.Business.Funding.Model;
using Telerik.Windows.Controls;
using System.Windows.Media.Animation;

namespace Insyston.Operations.WPF.Views.Funding
{
    /// <summary>
    /// Interaction logic for FundingDetails.xaml
    /// </summary>
    public partial class FundingDetails : UserControl
    {
        private FilterDescriptor _ExistingContractsFilter = new FilterDescriptor("IsSelected", FilterOperator.IsEqualTo, true);
        
        public FundingDetails()
        {
            this.InitializeComponent();
            this.DataContextChanged += this.FundingDetails_DataContextChanged;
        }

        private void OnStoryBoardChanged(string storyBoard)
        {
            ((Storyboard)this.Resources["SummaryState"]).Stop();
            ((Storyboard)this.Resources["ResultState"]).Stop();
            ((Storyboard)this.Resources[storyBoard]).Begin();
        }

        private void FundingDetails_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is FundingDetailsViewModel)
            {
                ((FundingDetailsViewModel)this.DataContext).onTrancheFilterReady -= this.FundingDetailsOnTrancheFilterReady;
                ((FundingDetailsViewModel)this.DataContext).onTrancheFilterReady += this.FundingDetailsOnTrancheFilterReady;
                ((FundingDetailsViewModel)this.DataContext).onAggregateFunctionCallRequired -= this.FundingDetailsOnAggregateFunctionCallRequired;
                ((FundingDetailsViewModel)this.DataContext).onAggregateFunctionCallRequired += this.FundingDetailsOnAggregateFunctionCallRequired;                
                ((FundingDetailsViewModel)this.DataContext).OnStoryBoardChanged -= this.OnStoryBoardChanged;
                ((FundingDetailsViewModel)this.DataContext).OnStoryBoardChanged += this.OnStoryBoardChanged;
                ((FundingDetailsViewModel)this.DataContext).SetLostFocusView += this.SetLostFocus;
                ((FundingDetailsViewModel)this.DataContext).CheckSelectAllCheckbox += this.CheckSelectAllCheckboxOnGrid;
                this.OnStoryBoardChanged("SummaryState");
            }
        }

        /// <summary>
        /// The check select all checkbox on grid.
        /// </summary>
        private void CheckSelectAllCheckboxOnGrid()
        {
            // Check Checkbox selected all on IncludedInTrancheContracts grid
            IncludedInTrancheContractsGrid.CalculateAggregates();
            if (((FundingDetailsViewModel)this.DataContext).IncludedInTrancheContracts.Count(x => x.IsSelected) == 0)
            {
                SelectAllIncludedInTrancheCheckBox.IsChecked = false;
            }
            else if (((FundingDetailsViewModel)this.DataContext).IncludedInTrancheContracts.Count(x => !x.IsSelected) == 0)
            {
                SelectAllIncludedInTrancheCheckBox.IsChecked = true;
            }
            else
            {
                SelectAllIncludedInTrancheCheckBox.IsChecked = false;
            }

            // Check Checkbox selected all on NotIncludedInTrancheContracts grid
            NotIncludedInTrancheContractsGrid.CalculateAggregates();
            if (((FundingDetailsViewModel)this.DataContext).NotIncludedInTrancheContracts.Count(x => x.IsSelected) == 0)
            {
                SelectAllNotIncludedInTrancheCheckBox.IsChecked = false;
            }
            else if (((FundingDetailsViewModel)this.DataContext).NotIncludedInTrancheContracts.Count(x => !x.IsSelected) == 0)
            {
                SelectAllNotIncludedInTrancheCheckBox.IsChecked = true;
            }
            else
            {
                SelectAllNotIncludedInTrancheCheckBox.IsChecked = false;
            }
        }

        //Method to clear focus from all control.
        //Use when change to tab Contract
        private void SetLostFocus()
        {
            this.CheckBoxCalculate.Focus();
        }
        private void FundingDetailsOnAggregateFunctionCallRequired(bool isSelected)
        {
            foreach (TrancheContractSummary item in this.IncludedInTrancheContractsGrid.Items)
            {
                item.IsExisting = isSelected;
            }
            foreach (TrancheContractSummary item in this.NotIncludedInTrancheContractsGrid.Items)
            {
                item.IsExisting = isSelected;
            }
            this.IncludedInTrancheContractsGrid.CalculateAggregates();
            this.NotIncludedInTrancheContractsGrid.CalculateAggregates();
        }
        private void FundingDetailsOnTrancheFilterReady()
        {
            PrepareGridFiltersFilterReady(IncludedInTrancheContractsGrid, true);
            PrepareGridFiltersFilterReady(NotIncludedInTrancheContractsGrid, false);
        }
        private void PrepareGridFiltersFilterReady(RadGridView gridView, bool isIncludedContractsInTrancheGrid)
        {
            FundingDetailsViewModel viewModel = this.DataContext as FundingDetailsViewModel;            
            //TrancheGrid.IsFilteringAllowed = true;
            CompositeFilterDescriptor _DynamicFilter = new CompositeFilterDescriptor();
            gridView.FilterDescriptors.Clear();
            _DynamicFilter.FilterDescriptors.Clear();
            gridView.FilterDescriptors.LogicalOperator = FilterCompositionLogicalOperator.Or;
            _DynamicFilter.LogicalOperator = FilterCompositionLogicalOperator.And;            
            
            this.SetupFilters(gridView, viewModel, isIncludedContractsInTrancheGrid);

            gridView.FilterDescriptors.SuspendNotifications();

            if (isIncludedContractsInTrancheGrid == false)
            {
                this.ApplyFieldFilterOnColumn(_DynamicFilter, "IsSelected", FilterOperator.IsEqualTo, false);
                this.ApplyDistinctFilterOnColumn("FinanceType", "FinanceTypeId", viewModel.AllFinanceTypes, _DynamicFilter);
                this.ApplyDistinctFilterOnColumn("Frequency", "FrequencyId", viewModel.AllFrequencies, _DynamicFilter);
                this.ApplyDistinctFilterOnColumn("FundingStatus", "FundingStatusId", viewModel.AllFundingStatuses, _DynamicFilter);
                this.ApplyDistinctFilterOnColumn("InstalmentType", "InstalmentType", viewModel.AllInstalmentType, _DynamicFilter);
                this.ApplyDistinctFilterOnColumn("InternalCompany", "InternalCompanyId", viewModel.AllInternalCompanies, _DynamicFilter);
                this.ApplyDistinctFilterOnColumn("FunderName", "FunderId", viewModel.AllFunders, _DynamicFilter);
                this.ApplyDistinctFilterOnColumn("Supplier", "SupplierId", viewModel.AllSuppliers, _DynamicFilter);
                this.ApplyFieldFilterOnColumn(_DynamicFilter, "Term", viewModel.DefaultTermOperator, viewModel.DefaultTerm);
                this.ApplyFieldFilterOnColumn(_DynamicFilter, "InvestmentBalance", viewModel.DefaultInvestmentBalanceOperator, viewModel.DefaultInvestmentBalance);
                this.ApplyFieldFilterOnColumn(_DynamicFilter, "GrossAmountOverDue", viewModel.DefaultGrossOverDueOperator, viewModel.DefaultGrossOverDue);
                this.ApplyFieldFilterOnColumn(_DynamicFilter, "StartDate", viewModel.DefaultFromDateOperator, viewModel.DefaultFromDateValue,
                    viewModel.DefaultToDateOperator, viewModel.DefaultToDateValue);
            }
            else
            {
                this.ApplyFieldFilterOnColumn(_DynamicFilter, "IsSelected", FilterOperator.IsEqualTo, false);
                this.ApplyDistinctFilterOnColumn("FinanceType", "FinanceTypeId", viewModel.AllFinanceTypesForIncludedInTrancheContracts, _DynamicFilter);
                this.ApplyDistinctFilterOnColumn("Frequency", "FrequencyId", viewModel.AllFrequenciesForIncludedInTrancheContracts, _DynamicFilter);
                this.ApplyDistinctFilterOnColumn("FundingStatus", "FundingStatusId", viewModel.AllFundingStatusesForIncludedInTrancheContracts, _DynamicFilter);
                this.ApplyDistinctFilterOnColumn("InstalmentType", "InstalmentType", viewModel.AllInstalmentTypeForIncludedInTrancheContracts, _DynamicFilter);
                this.ApplyDistinctFilterOnColumn("InternalCompany", "InternalCompanyId", viewModel.AllInternalCompaniesForIncludedInTrancheContracts, _DynamicFilter);
                this.ApplyDistinctFilterOnColumn("FunderName", "FunderId", viewModel.AllFundersForIncludedInTrancheContracts, _DynamicFilter);
                this.ApplyDistinctFilterOnColumn("Supplier", "SupplierId", viewModel.AllSuppliersForIncludedInTrancheContracts, _DynamicFilter);
                this.ApplyFieldFilterOnColumn(_DynamicFilter, "Term", viewModel.DefaultTermOperator, viewModel.DefaultTerm);
            }

            gridView.FilterDescriptors.Add(this._ExistingContractsFilter);
            gridView.FilterDescriptors.Add(_DynamicFilter);

            gridView.FilterDescriptors.ResumeNotifications();
        }

        private void SetupFilters(RadGridView gridView, FundingDetailsViewModel viewModel, bool isIncludedContractsInTrancheGrid = true)
        {
            gridView.Columns["Frequency"].FilteringControl = new SelectFilter()
            {
                ItemsSource = isIncludedContractsInTrancheGrid ? viewModel.AllFrequenciesForIncludedInTrancheContracts : viewModel.AllFrequencies,
                IsMultiSelect = true,
                Title = "Frequencies:",
            };
            gridView.Columns["FinanceType"].FilteringControl = new SelectFilter()
            {
                ItemsSource = isIncludedContractsInTrancheGrid ? viewModel.AllFinanceTypesForIncludedInTrancheContracts : viewModel.AllFinanceTypes,
                IsMultiSelect = true,
                Title = "Finance Types:",
            };
            gridView.Columns["FundingStatus"].FilteringControl = new SelectFilter()
            {
                ItemsSource = isIncludedContractsInTrancheGrid ? viewModel.AllFundingStatusesForIncludedInTrancheContracts : viewModel.AllFundingStatuses,
                IsMultiSelect = true,
                Title = "Funding Statuses:",
            };
            gridView.Columns["InstalmentType"].FilteringControl = new SelectFilter()
            {
                ItemsSource = isIncludedContractsInTrancheGrid ? viewModel.AllInstalmentTypeForIncludedInTrancheContracts : viewModel.AllInstalmentType,
                IsMultiSelect = false,
                Title = "Instalment Types:",
            };
            gridView.Columns["InternalCompany"].FilteringControl = new SelectFilter()
            {
                ItemsSource = isIncludedContractsInTrancheGrid ? viewModel.AllInternalCompaniesForIncludedInTrancheContracts : viewModel.AllInternalCompanies,
                IsMultiSelect = true,
                Title = "Internal Companies:",
            };

            gridView.Columns["Funder"].FilteringControl = new SelectFilter()
            {
                ItemsSource = isIncludedContractsInTrancheGrid ? viewModel.AllFundersForIncludedInTrancheContracts : viewModel.AllFunders,
                IsMultiSelect = true,
                Title = "Funders:",
            };

            gridView.Columns["Supplier"].FilteringControl = new SelectFilter()
            {
                ItemsSource = isIncludedContractsInTrancheGrid ? viewModel.AllSuppliersForIncludedInTrancheContracts : viewModel.AllSuppliers,
                IsMultiSelect = true,
                Title = "Suppliers:",
            };

            NumberFilter termFilter = new NumberFilter()
            {
                Title = "Term:",
                AllOperators = viewModel.AllTermOperators,
                SelectedOperator = isIncludedContractsInTrancheGrid ? null : viewModel.DefaultTermOperator,
            };

            if (isIncludedContractsInTrancheGrid == false)
            {
                Binding valueBinding = new Binding();
                valueBinding.Source = viewModel;
                valueBinding.Mode = BindingMode.TwoWay;
                valueBinding.Path = new PropertyPath("DefaultTerm");
                BindingOperations.SetBinding(termFilter, NumberFilter.ValueProperty, valueBinding);

                Binding operatorBinding = new Binding();
                operatorBinding.Source = viewModel;
                operatorBinding.Mode = BindingMode.TwoWay;
                operatorBinding.Path = new PropertyPath("DefaultTermOperator");
                BindingOperations.SetBinding(termFilter, NumberFilter.SelectedOperatorProperty, operatorBinding);
            }

            gridView.Columns["Term"].FilteringControl = termFilter;

            NumberFilter investmentBalanceFilter = new NumberFilter()
            {
                Title = "Investment Balance:",
                AllOperators = viewModel.AllTermOperators,
                SelectedOperator = isIncludedContractsInTrancheGrid ? null : viewModel.DefaultInvestmentBalanceOperator,
            };

            if (isIncludedContractsInTrancheGrid == false)
            {
                Binding investmentBalanceBinding = new Binding();
                investmentBalanceBinding.Source = viewModel;
                investmentBalanceBinding.Mode = BindingMode.TwoWay;
                investmentBalanceBinding.Path = new PropertyPath("DefaultInvestmentBalance");
                BindingOperations.SetBinding(investmentBalanceFilter, NumberFilter.ValueProperty, investmentBalanceBinding);

                Binding investmentBalanceOperatorBinding = new Binding();
                investmentBalanceOperatorBinding.Source = viewModel;
                investmentBalanceOperatorBinding.Mode = BindingMode.TwoWay;
                investmentBalanceOperatorBinding.Path = new PropertyPath("DefaultInvestmentBalanceOperator");
                BindingOperations.SetBinding(investmentBalanceFilter, NumberFilter.SelectedOperatorProperty, investmentBalanceOperatorBinding);
            }

            gridView.Columns["InvestmentBalance"].FilteringControl = investmentBalanceFilter;


            NumberFilter grossAmountOverDueFilter = new NumberFilter()
            {
                Title = "Gross Amount OverDue:",
                AllOperators = viewModel.AllTermOperators,
                SelectedOperator = isIncludedContractsInTrancheGrid ? null : viewModel.DefaultGrossOverDueOperator,
            };

            if (isIncludedContractsInTrancheGrid == false)
            {
                Binding grossAmountOverDueBinding = new Binding();
                grossAmountOverDueBinding.Source = viewModel;
                grossAmountOverDueBinding.Mode = BindingMode.TwoWay;
                grossAmountOverDueBinding.Path = new PropertyPath("DefaultGrossOverDue");
                BindingOperations.SetBinding(grossAmountOverDueFilter, NumberFilter.ValueProperty, grossAmountOverDueBinding);

                Binding grossOverdueOperatorBinding = new Binding();
                grossOverdueOperatorBinding.Source = viewModel;
                grossOverdueOperatorBinding.Mode = BindingMode.TwoWay;
                grossOverdueOperatorBinding.Path = new PropertyPath("DefaultGrossOverDueOperator");
                BindingOperations.SetBinding(grossAmountOverDueFilter, NumberFilter.SelectedOperatorProperty, grossOverdueOperatorBinding);
            }

            gridView.Columns["GrossAmountOverDue"].FilteringControl = grossAmountOverDueFilter;
           

            DateFilter dateFilter = new DateFilter()
            {
                AllFromDateOperators = viewModel.AllFromDateOperators,
                AllToDateOperators = viewModel.AllToDateOperators,
            };

            if (isIncludedContractsInTrancheGrid == false)
            {
                Binding fromDateOperatorBinding = new Binding();
                fromDateOperatorBinding.Source = viewModel;
                fromDateOperatorBinding.Mode = BindingMode.TwoWay;
                fromDateOperatorBinding.Path = new PropertyPath("DefaultFromDateOperator");
                BindingOperations.SetBinding(dateFilter, DateFilter.FromDateSelectedOperatorProperty, fromDateOperatorBinding);

                Binding toDateOperatorBinding = new Binding();
                toDateOperatorBinding.Source = viewModel;
                toDateOperatorBinding.Mode = BindingMode.TwoWay;
                toDateOperatorBinding.Path = new PropertyPath("DefaultToDateOperator");
                BindingOperations.SetBinding(dateFilter, DateFilter.ToDateSelectedOperatorProperty, toDateOperatorBinding);

                Binding fromDateValueBinding = new Binding();
                fromDateValueBinding.Source = viewModel;
                fromDateValueBinding.Mode = BindingMode.TwoWay;
                fromDateValueBinding.Path = new PropertyPath("DefaultFromDateValue");
                BindingOperations.SetBinding(dateFilter, DateFilter.FromDateValueProperty, fromDateValueBinding);

                Binding toDateValueBinding = new Binding();
                toDateValueBinding.Source = viewModel;
                toDateValueBinding.Mode = BindingMode.TwoWay;
                toDateValueBinding.Path = new PropertyPath("DefaultToDateValue");
                BindingOperations.SetBinding(dateFilter, DateFilter.ToDateValueProperty, toDateValueBinding);
            }
            gridView.Columns["StartDate"].FilteringControl = dateFilter;
        }

        private void ApplyFieldFilterOnColumn(CompositeFilterDescriptor dynamicFilter, string columnName,
            FilterOperator? field1Operator, object value1, FilterOperator? field2Operator = null, object value2 = null)
        {
            CompositeFilterDescriptor fieldFilter = new CompositeFilterDescriptor();
            bool isAvailable = false;
            fieldFilter.LogicalOperator = FilterCompositionLogicalOperator.And;
            if (field1Operator.HasValue)
            {
                fieldFilter.FilterDescriptors.Add(new FilterDescriptor(columnName,
                    field1Operator.Value, value1));
                isAvailable = true;
                if (field2Operator.HasValue)
                {
                    fieldFilter.FilterDescriptors.Add(new FilterDescriptor(columnName,
                        field2Operator.Value, value2));
                }
            }
            if (isAvailable)
            {
                dynamicFilter.FilterDescriptors.Add(fieldFilter);
            }
        }

        private void ApplyDistinctFilterOnColumn(string columnName, string dataMember, IEnumerable<SelectListViewModel> itemsSource,
            CompositeFilterDescriptor dynamicFilter)
        { 
            CompositeFilterDescriptor distinct = new CompositeFilterDescriptor();
            bool isAvailable = false;
            distinct.LogicalOperator = FilterCompositionLogicalOperator.Or;
            foreach (var item in itemsSource)
            {
                if (item.IsSelected)
                {
                    if (columnName == dataMember)
                    {
                        distinct.FilterDescriptors.Add(new FilterDescriptor(columnName,
                            FilterOperator.IsEqualTo, item.Text));
                    }
                    else
                    {
                        distinct.FilterDescriptors.Add(new FilterDescriptor(dataMember,
                            FilterOperator.IsEqualTo, item.Id));
                    }

                    isAvailable = true;
                }
            }
            if (isAvailable)
            {
                dynamicFilter.FilterDescriptors.Add(distinct);
            }
        }

        private void CheckBox_NotIncludedInTrancheConreactsGridChecked(object sender, RoutedEventArgs e)
        {
            NotIncludedInTrancheContractsGrid.CalculateAggregates();
            if (((FundingDetailsViewModel)this.DataContext).NotIncludedInTrancheContracts.Count(x=>x.IsSelected) == 0)
            {
                SelectAllNotIncludedInTrancheCheckBox.IsChecked = false;
            }
            else if (((FundingDetailsViewModel)this.DataContext).NotIncludedInTrancheContracts.Count(x => !x.IsSelected) == 0)
            {
                SelectAllNotIncludedInTrancheCheckBox.IsChecked = true;
            }
            else
            {
                SelectAllNotIncludedInTrancheCheckBox.IsChecked = false;
            }
        }
        private void CheckBox_IncludedInTrancheConreactsGridChecked(object sender, RoutedEventArgs e)
        {
            IncludedInTrancheContractsGrid.CalculateAggregates();
            if (((FundingDetailsViewModel)this.DataContext).IncludedInTrancheContracts.Count(x => x.IsSelected) == 0)
            {
                SelectAllIncludedInTrancheCheckBox.IsChecked = false;
            }
            else if (((FundingDetailsViewModel)this.DataContext).IncludedInTrancheContracts.Count(x => !x.IsSelected) == 0)
            {
                SelectAllIncludedInTrancheCheckBox.IsChecked = true;
            }
            else
            {
                SelectAllIncludedInTrancheCheckBox.IsChecked = false;
            }
        }

        private void CheckBox_IncludedInTranche_All_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in IncludedInTrancheContractsGrid.Items)
            {
                TrancheContractSummary contract = item as TrancheContractSummary;
                contract.IsSelected = ((CheckBox)sender).IsChecked.HasValue ? ((CheckBox)sender).IsChecked.Value : false;
            }
            this.IncludedInTrancheContractsGrid.CalculateAggregates();
        }

        private void CheckBox_NotIncludedInTranche_All_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in NotIncludedInTrancheContractsGrid.Items)
            {
                TrancheContractSummary contract = item as TrancheContractSummary;
                contract.IsSelected = ((CheckBox)sender).IsChecked.HasValue ? ((CheckBox)sender).IsChecked.Value : false;
            }
            this.NotIncludedInTrancheContractsGrid.CalculateAggregates();
        }
        
        //private async void ResultHyperlink_Click(object sender, RoutedEventArgs e)
        //{
        //    await ((FundingDetailsViewModel)this.DataContext).OnStepAsync(FundingDetailsViewModel.EnumStep.ResultState);
        //}
        //private async void SummaryHyperlink_Click(object sender, RoutedEventArgs e)
        //{
        //    await ((FundingDetailsViewModel)this.DataContext).OnStepAsync(FundingDetailsViewModel.EnumStep.SummaryState);
        //}
    }
}