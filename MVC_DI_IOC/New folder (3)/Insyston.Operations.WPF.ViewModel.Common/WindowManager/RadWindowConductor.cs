// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadWindowConductor.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the RadWindowConductor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.WindowManager
{
    using System;

    using global::Caliburn.Micro;

    using Telerik.Windows.Controls;

    /// <summary>
    /// The rad window conductor.
    /// </summary>
    internal class RadWindowConductor
    {
        /// <summary>
        /// The _deactivating from view.
        /// </summary>
        private bool _deactivatingFromView;

        /// <summary>
        /// The _deactivate from view model.
        /// </summary>
        private bool _deactivateFromViewModel;

        /// <summary>
        /// The _actually closing.
        /// </summary>
        private bool _actuallyClosing;

        /// <summary>
        /// The _view.
        /// </summary>
        private readonly RadWindow _view;

        /// <summary>
        /// The _model.
        /// </summary>
        private readonly object _model;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadWindowConductor"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="view">
        /// The view.
        /// </param>
        public RadWindowConductor(object model, RadWindow view)
        {
            this._model = model;
            this._view = view;
            var activatable = model as IActivate;
            if (activatable != null)
            {
                activatable.Activate();
            }
            var deactivatable = model as IDeactivate;
            if (deactivatable != null)
            {
                view.Closed += this.Closed;
                deactivatable.Deactivated += this.Deactivated;
            }
            var guard = model as IGuardClose;
            if (guard != null)
            {
                view.PreviewClosed += this.PreviewClosed;
            }
        }

        /// <summary>
        /// The closed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Closed(object sender, EventArgs e)
        {
            this._view.Closed -= this.Closed;
            this._view.PreviewClosed -= this.PreviewClosed;
            if (this._deactivateFromViewModel)
            {
                return;
            }
            var deactivatable = (IDeactivate)this._model;
            this._deactivatingFromView = true;
            deactivatable.Deactivate(true);
            this._deactivatingFromView = false;
        }

        /// <summary>
        /// The deactivated.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The DeactivationEventArgs.
        /// </param>
        private void Deactivated(object sender, DeactivationEventArgs e)
        {
            if (!e.WasClosed)
            {
                return;
            }
            ((IDeactivate)this._model).Deactivated -= this.Deactivated;
            if (this._deactivatingFromView)
            {
                return;
            }
            this._deactivateFromViewModel = true;
            this._actuallyClosing = true;
            this._view.Close();
            this._actuallyClosing = false;
            this._deactivateFromViewModel = false;
        }

        /// <summary>
        /// The preview closed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The WindowPreviewClosedEventArgs.
        /// </param>
        private void PreviewClosed(object sender, WindowPreviewClosedEventArgs e)
        {
            if (e.Cancel == true)
            {
                return;
            }
            var guard = (IGuardClose)this._model;
            if (this._actuallyClosing)
            {
                this._actuallyClosing = false;
                return;
            }
            bool runningAsync = false, shouldEnd = false;
            guard.CanClose(canClose =>
            {
                Execute.OnUIThread(() =>
                {
                    if (runningAsync && canClose)
                    {
                        this._actuallyClosing = true;
                        this._view.Close();
                    }
                    else
                    {
                        e.Cancel = !canClose;
                    }
                    shouldEnd = true;
                });
            });
            if (shouldEnd)
            {
                return;
            }
            e.Cancel = true;
            runningAsync = true;
        }
    }
}
