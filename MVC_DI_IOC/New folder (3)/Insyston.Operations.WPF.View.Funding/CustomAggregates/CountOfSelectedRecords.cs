using System;
using System.Collections.Generic;
using System.Linq;
using Insyston.Operations.Business.Common.Linq;
using Insyston.Operations.Business.Funding.Model;
using Telerik.Windows.Data;

namespace Insyston.Operations.WPF.Views.Funding.CustomAggregates
{
    public class CountOfSelectedRecords : AggregateFunction<TrancheContractSummary, int>
    {
        public CountOfSelectedRecords()
        {
            this.AggregationExpression = items => this.CountOfSelected(items);
        }
        
        public string SourceField { get; set; }

        private int CountOfSelected(IEnumerable<TrancheContractSummary> source)
        {
            if (source != null && source.Count() > 0)
            {
                return source.Count(item => item.IsSelected);
            }
            return 0;
        }
    }
}
