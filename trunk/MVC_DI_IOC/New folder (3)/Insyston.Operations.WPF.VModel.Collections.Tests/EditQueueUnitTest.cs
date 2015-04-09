// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditQueueUnitTest.cs" company="TMA Solutions">
//   2014
// </copyright>
// <summary>
//   The edit queue unit test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.VModel.Collections.Tests
{
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Collections.Model;
    using Insyston.Operations.WPF.ViewModels.Collections;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// The edit queue unit test.
    /// </summary>
    [TestClass]
    public class EditQueueUnitTest
    {
        /// <summary>
        /// The test on step async at start.
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncAtStart()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                    var editQueueMock = Mock.Create<EditQueueViewModel>(Behavior.CallOriginal, new CollectionsManagementViewModel());

                    Mock.NonPublic.Arrange<Task<bool>>(editQueueMock, "LockAsync").Returns(Task.Run(() => true));

                    await editQueueMock.OnStepAsync(EditQueueViewModel.EnumSteps.Start);

                    Assert.IsTrue(editQueueMock.IsCheckedOut);
                    Assert.IsTrue(editQueueMock.ActionCommands.Count == 2);
                });
        }

        /// <summary>
        /// The test on step async at cancel.
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncAtCancel()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    var managementMock = Mock.Create<CollectionsManagementViewModel>(Behavior.CallOriginal);
                    var editQueueMock = Mock.Create<EditQueueViewModel>(
                        Behavior.CallOriginal, managementMock);

                    Mock.NonPublic.Arrange<Task>(editQueueMock, "UnLockAsync").Returns(Task.Run(() => { }));
                    Mock.Arrange(() => editQueueMock.SelectedQueue).Returns(new QueueDetailsModel());
                    Mock.Arrange(
                        () => managementMock.OnStepAsync(CollectionsManagementViewModel.EnumSteps.SelectQueue))
                        .Returns(Task.Run(() => { }));

                    var mainView = new PrivateAccessor(editQueueMock);
                    mainView.SetProperty("IsCheckedOut", true);
                    await editQueueMock.OnStepAsync(EditQueueViewModel.EnumSteps.Cancel);

                    bool isCheckedOutOnMainViewModel = mainView.GetProperty("IsCheckedOut") is bool && (bool)mainView.GetProperty("IsCheckedOut");

                    Assert.IsFalse(isCheckedOutOnMainViewModel);
                });
        }
    }
}
