using FluentValidation;
using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Assets.Validation.AssetClassesCategoryControlValidate
{
    public class CategoryViewModelValidation : AbstractValidator<AssetClassesCategoryViewModel>
    {
        //public AssetClassesCategoryViewModelValidation()
        //{
        //    this.RuleFor(category => category.CategoryDetailViewModel).SetValidator(new AssetClassesCategoryDetailViewModelValidation());
        //}
    }
}
