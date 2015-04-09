// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementValidationUnitTest.cs" company="TMA Solution">
//   2014
// </copyright>
// <summary>
//   Defines the ManagementValidationUnitTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.VModel.Collections.Tests
{
    using System.Collections.Generic;
    using System.Windows;

    using FluentValidation.Results;

    using Insyston.Operations.WPF.ViewModels.Collections;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// The management validation unit test.
    /// </summary>
    [TestClass]
    public class ManagementValidationUnitTest
    {
        /// <summary>
        /// The test collection queue_ queue name and assignment_ not empty.
        /// </summary>
        [TestMethod]
        public void TestCollectionQueue_QueueNameAndAssignment_NotEmpty()
        {
            Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                .IgnoreArguments()
                .DoNothing();
            var viewModel = Mock.Create<EditQueueViewModel>(
                Behavior.CallOriginal,
                new CollectionsManagementViewModel());
            viewModel.SelectedQueue = FakeData.FakeQueueDetailModel();
            Mock.Arrange(() => viewModel.SelectedQueue.CollectionQueue).Returns(FakeData.FakeCollectionQueue());

            Mock.Arrange(() => viewModel.HasErrors).Returns(true);

            var inst = new PrivateAccessor(viewModel);
            inst.CallMethod("Validate");

            var vali = new PrivateAccessor(viewModel);
            var faile = vali.GetField("_Failures") as List<ValidationFailure>;

            Assert.IsNotNull(faile);
            Assert.AreEqual(2, faile.Count);
        }
    }
}