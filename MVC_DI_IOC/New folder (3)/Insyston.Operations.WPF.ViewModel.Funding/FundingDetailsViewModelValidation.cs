// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FundingDetailsViewModelValidation.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The funding details view model validation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Funding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentValidation;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Business.Funding;
    using Insyston.Operations.Business.Funding.Model;

    /// <summary>
    /// The funding details view model validation.
    /// </summary>
    public class FundingDetailsViewModelValidation : AbstractValidator<FundingDetailsViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FundingDetailsViewModelValidation"/> class.
        /// </summary>
        /// <param name="trancheDate">
        /// The tranche date.
        /// </param>
        /// <param name="isConfirm">
        /// The is Confirm.
        /// </param>
        public FundingDetailsViewModelValidation(DateTime? trancheDate, bool isConfirm)
        {
            ContractValidation contractValidation = new ContractValidation(trancheDate, isConfirm);

            // this.RuleFor(x => x.SelectedTrancheProfile).SetValidator(new FundingTrancheValidation());
            this.RuleFor(x => x.SelectedTrancheProfile.AssumedRate)
                .Must(this.CheckAssumedRate)
                .WithMessage("'Assumed Rate' must be greater than '0'.")
                .When(x => x.StillError);
            this.RuleFor(x => x.SelectedTrancheProfile.LossReserve)
                .GreaterThanOrEqualTo(0)
                .WithMessage("'Loss Reserve' must be greater or equal to '0'.")
                .When(x => x.StillError);

            // this.RuleFor(x => x.SelectedTrancheProfile.AssumedRate)
            // .GreaterThan(0)
            // .WithMessage("'Assumed Rate' must be greater than '0'.")
            // .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.Save);
            // this.RuleFor(x => x.SelectedTrancheProfile.LossReserve)
            // .GreaterThanOrEqualTo(0)
            // .WithMessage("'Loss Reserve' must be greater or equal to '0'.")
            // .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.Save);
            this.RuleFor(x => x.CurrentStep)
                .Must((f, s) => f.TrancheDateText != null)
                .WithMessage("Funding Date should not be blank.")
                .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.Save);

            this.RuleFor(x => x.CurrentStep)
                .Must((f, s) => f.TrancheDateText != null)
                .WithMessage("Funding Date should not be blank.")
                .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.ResultState);
            this.RuleFor(x => x.CurrentStep)
                .Must((f, s) => f.IncludedInTrancheContracts.Count > 0)
                .When(
                    x =>
                    x.CurrentStep == FundingDetailsViewModel.EnumStep.StatusToConfirmed
                    || x.CurrentStep == FundingDetailsViewModel.EnumStep.StatusToFunded
                    || x.CurrentStep == FundingDetailsViewModel.EnumStep.Calculate)
                .WithMessage("Cannot proceed as there is no existing contract.");

            this.RuleFor(x => x.CurrentStep)
                .Must(this.NotBeLocked)
                .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.StatusToConfirmed)
                .WithMessage("Contracts are locked by operations and cannot be Confirmed.");

            this.RuleFor(x => x.CurrentStep)
                .Must(this.HaveOnlyOneConfirmedRecordPerContract)
                .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.StatusToConfirmed)
                .WithMessage("Only one Confirmed record per Contract is allowed.");

            this.RuleFor(x => x.CurrentStep)
                .Must(this.NotInPaymentGroup)
                .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.StatusToConfirmed)
                .WithMessage("Contract must be removed from Payment Group before changing the Funding Status.");

            this.RuleFor(x => x.CurrentStep)
                .Must(this.BeCalculated)
                .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.StatusToFunded)
                .WithMessage("Re-Calculation is required for the contracts before moving to the Funded status.");

            this.RuleFor(x => x.CurrentStep)
                .Must(this.NotInPaymentGroup)
                .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.StatusToFunded)
                .WithMessage("Contract must be removed from Payment Group before changing the Funding Status.");

            this.RuleFor(x => x.CurrentStep)
                .Must((f, s) => f.OriginalTrancheStatus == TrancheStatus.Confirmed)
                .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.Calculate)
                .WithMessage("Calculation can be done only for tranches that are Confirmed.");

            this.RuleFor(x => x.CurrentStep)
                .Must((f, s) => f.SelectedFunder != null)
                .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.ResultState)
                .WithMessage("To see the result a funder has to be selected.");

            this.RuleFor(x => x.CurrentStep)
                .Must((f, s) => f.SelectedFunder != null)
                .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.Save)
                .WithMessage("To save a tranche a funder has to be selected.");

            this.RuleFor(x => x.CurrentStep)
                .Must((f, s) => f.SelectedTrancheProfile.NodeId != -1)
                .When(x => x.CurrentStep == FundingDetailsViewModel.EnumStep.StatusToConfirmed)
                .WithMessage("Cannot proceed as Funder is <None>.");

            this.RuleFor(x => x.IncludedInTrancheContracts)
                .SetCollectionValidator(contractValidation)
                .When(
                    x =>
                    x.CurrentStep == FundingDetailsViewModel.EnumStep.StatusToConfirmed
                    || x.CurrentStep == FundingDetailsViewModel.EnumStep.SelectTrancheDate);
        }

        /// <summary>
        /// The check assumed rate.
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
        private bool CheckAssumedRate(FundingDetailsViewModel source, double? arg)
        {
            if (source.CurrentStep == FundingDetailsViewModel.EnumStep.SelectTrancheDate)
            {
                if (source.ValidationSummary.FirstOrDefault(x => x.PropertyName.Equals("SelectedTrancheProfile.AssumedRate")) != null)
                {
                    return false;
                }
                return true;
            }
            if (source.SelectedTrancheProfile.AssumedRate > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// The not in payment group.
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
        private bool NotInPaymentGroup(FundingDetailsViewModel source, FundingDetailsViewModel.EnumStep arg)
        {
            if (FundingFunctions.AreAnyContractInTrancheInPaymentGroup(source.SelectedTrancheProfile.TrancheId))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// The be calculated.
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
        private bool BeCalculated(FundingDetailsViewModel source, FundingDetailsViewModel.EnumStep arg)
        {
            if (FundingFunctions.IsRecalcRequiredForContracts(source.SelectedTrancheProfile.TrancheId))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// The have only one confirmed record per contract.
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
        private bool HaveOnlyOneConfirmedRecordPerContract(FundingDetailsViewModel source, FundingDetailsViewModel.EnumStep arg)
        {
            List<TrancheContractSummary> selectedContracts = source.IncludedInTrancheContracts.ToList();

            if (FundingFunctions.AreThereAnyOtherRecordsConfirmedPerSelectedContracts(selectedContracts))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// The not be locked.
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
        private bool NotBeLocked(FundingDetailsViewModel source, FundingDetailsViewModel.EnumStep arg)
        {
            if (FundingFunctions.AreContractInCurrentLocks(source.SelectedTrancheProfile.TrancheId))
            {
                return false;
            }
            return true;
        }
    }
}
