// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContractValidation.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the ContractValidation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Funding
{
    using System;

    using FluentValidation;

    using Insyston.Operations.Business.Funding.Model;

    /// <summary>
    /// The contract validation.
    /// </summary>
    public class ContractValidation : AbstractValidator<TrancheContractSummary>
    {
        /// <summary>
        /// The _ tranche date.
        /// </summary>
        private readonly DateTime _trancheDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractValidation"/> class.
        /// </summary>
        /// <param name="trancheDate">
        /// The tranche date.
        /// </param>
        /// <param name="isConfirm">
        /// The is confirm.
        /// </param>
        public ContractValidation(DateTime? trancheDate, bool isConfirm)
        {
            if (trancheDate != null)
            {
                this._trancheDate = (DateTime)trancheDate;
            }

            if (!isConfirm)
            {
                this.RuleFor(x => x.StartDate)
            .Must(this.MeetTrancheDate)
                .WithMessage(
                    "The new Funding Date requires selected records to be removed from the Tranche. Please check the Results page for errors.");
            }
            else
            {
                this.RuleFor(x => x.StartDate)
            .Must(this.MeetTrancheDate)
                .WithMessage(
                    "Cannot change the status to Confirmed as there are Invalid Contracts.");
            }
        }

        /// <summary>
        /// The meet tranche date.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="arg">
        /// The arg.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool MeetTrancheDate(TrancheContractSummary source, DateTime arg)
        {
            try
            {
                if (source.StartDate > _trancheDate)
                {
                    return false;
                }

                if (_trancheDate > source.LastPaymentDate)
                {
                    return false;
                }

                if (source.FundingStartDate > source.LastPaymentDate)
                {
                    return false;
                }               
            }
            catch (Exception)
            {
                return true;
            }
            return true;
        }
    }
}
