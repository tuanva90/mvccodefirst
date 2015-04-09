using System;
using System.Linq;
using FluentValidation;

namespace Insyston.Operations.WPF.ViewModels.Funding
{
    public class FundingSummaryViewModelValidation : AbstractValidator<FundingSummaryViewModel>
    {
        public FundingSummaryViewModelValidation()
        {
        }
    }
}
