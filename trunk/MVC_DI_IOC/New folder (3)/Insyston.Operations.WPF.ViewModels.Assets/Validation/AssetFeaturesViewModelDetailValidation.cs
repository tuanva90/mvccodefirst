// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetFeaturesViewModelDetailValidation.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetFeaturesViewModelDetailValidation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Validation
{
    using FluentValidation;

    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetFeatures;

    /// <summary>
    /// The asset features view model detail validation.
    /// </summary>
    public class AssetFeaturesViewModelDetailValidation : AbstractValidator<AssetFeaturesDetailViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetFeaturesViewModelDetailValidation"/> class.
        /// </summary>
        public AssetFeaturesViewModelDetailValidation()
        {
            this.RuleFor(viewModel => viewModel.FeatureName)
                .NotEmpty()
                .WithMessage("Feature Name is mandatory.");
            this.RuleFor(viewModel => viewModel.RequiredLengthString)
                .Must(this.IsNumericData)
                .When(viewModel => viewModel.IsChecked)
                .WithMessage("Required Length must be numeric data.");
            this.RuleFor(viewModel => viewModel.RequiredLengthString)
                .Must(this.IsGreaterThan0)
                .When(viewModel => viewModel.IsChecked)
                .WithMessage("Required Length must be greater than 0.");
            this.RuleFor(viewModel => viewModel.RequiredLengthString)
                .Must(this.IsOutOfRange)
                .When(viewModel => viewModel.IsChecked)
                .WithMessage("Required Length is invalid.");
            this.RuleFor(viewModel => viewModel.RequiredLengthString)
                .Must(this.IsWholeNumber)
                .When(viewModel => viewModel.IsChecked)
                .WithMessage("Required length must be a whole number");
        }

        /// <summary>
        /// The is whole number.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsWholeNumber(AssetFeaturesDetailViewModel source, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            decimal d;
            if (!decimal.TryParse(value, out d) || value.Contains(","))
            {
                return true;
            }

            if (d > int.MaxValue)
            {
                return true;
            }

            int n;
            return int.TryParse(value, out n);
        }

        /// <summary>
        /// The is numeric data.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsNumericData(AssetFeaturesDetailViewModel source, string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Contains(","))
            {
                return false;
            }

            decimal n;
            return decimal.TryParse(value, out n);
        }

        /// <summary>
        /// The is greater than 0.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsGreaterThan0(AssetFeaturesDetailViewModel source, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                int n;
                if (int.TryParse(value, out n))
                {
                    if (n < 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The is out of range.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsOutOfRange(AssetFeaturesDetailViewModel source, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                decimal n;
                if (decimal.TryParse(value, out n))
                {
                    if (n > int.MaxValue)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
