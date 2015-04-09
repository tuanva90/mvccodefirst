using System;
using System.Collections.Generic;
using System.Linq;
using Insyston.Operations.Business.Common.Linq;
using Insyston.Operations.Business.Funding.Model;
using Telerik.Windows.Data;

namespace Insyston.Operations.WPF.Views.Funding.CustomAggregates
{
    public class SumInvestmentBalances : AggregateFunction<TrancheContractSummary, decimal>
    {
        public SumInvestmentBalances()
        {
            this.AggregationExpression = items => this.SumInvestmentBalance(items);
        }
        
        public string SourceField { get; set; }

        private decimal SumInvestmentBalance(IEnumerable<TrancheContractSummary> source)
        {
            if (source != null && source.Count() > 0)
            {
                return ((IEnumerable<decimal>)source.Where(item => item.IsSelected).AsQueryable().Select(this.SourceField)).Sum();
            }
            return 0;
        }
    }
}
