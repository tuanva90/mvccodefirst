// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingUnitTest.cs" company="TMA Solutions">
//   2014
// </copyright>
// <summary>
//   The setting unit test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.VModel.Collections.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Collections;
    using Insyston.Operations.WPF.ViewModels.Collections;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// The setting unit test.
    /// </summary>
    [TestClass]
    public class SettingUnitTest
    {
        /// <summary>
        /// The test OnStepAsync(start) function.
        /// </summary>
        [TestMethod]
        public void Test_OnStepAsyncAtStart_LoadDataCorrect()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                    var settingMock = Mock.Create<CollectionsSettingViewModel>(Behavior.CallOriginal);

                    Mock.Arrange(() => CollectionsQueueSettingsFunctions.ReadCollectionsSystemSettingsAsync())
                        .Returns(Task.FromResult(FakeData.FakeCollectionsSystemSettingsAsync()));
                    Mock.Arrange(() => CollectionsQueueSettingsFunctions.ReadCollectionSystemDefaultAsync())
                        .Returns(Task.FromResult(FakeData.FakeCollectionSystemDefaultAsync()));
                    Mock.Arrange(() => CollectionsQueueSettingsFunctions.ReadAllSystemParamAsync())
                        .Returns(FakeData.FakeAllSystemParameterAsync());

                    await settingMock.OnStepAsync(CollectionsSettingViewModel.EnumSteps.Start);

                    Assert.IsTrue(
                        settingMock.CollectionQueueSetting.ID == 1
                        && settingMock.CollectionQueueSetting.MinimumArrearsAmount == 10
                        && settingMock.CollectionQueueSetting.MinimumArrearsDays == 2
                        && settingMock.CollectionQueueSetting.MinimumArrearsPercent == 12);

                    Assert.IsTrue(
                        settingMock.CollectionSystemDefault.ID == 1
                        && settingMock.CollectionSystemDefault.FilterOptionID == 2
                        && settingMock.CollectionSystemDefault.NoteCategoryID == 1);

                    Assert.IsTrue(settingMock.Category.Count == 1);
                });
        }

        /// <summary>
        /// The test OnStepAsync(Edit).
        /// </summary>
        [TestMethod]
        public void Test_OnStepAsyncAtEdit_CheckedOutAndCorrectActionCommand()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                    var settingMock = Mock.Create<CollectionsSettingViewModel>(Behavior.CallOriginal);

                    settingMock.IsCheckedOut = false;
                    await settingMock.OnStepAsync(CollectionsSettingViewModel.EnumSteps.Edit);

                    Assert.IsTrue(settingMock.IsCheckedOut);
                    Assert.IsTrue(
                        settingMock.ActionCommands != null && settingMock.ActionCommands.Count == 2
                        && settingMock.ActionCommands.First()
                               .Parameter.Equals(CollectionsSettingViewModel.EnumSteps.Save.ToString())
                        && settingMock.ActionCommands[1].Parameter.Equals(
                            CollectionsSettingViewModel.EnumSteps.Cancel.ToString()));
                });
        }

        /// <summary>
        /// The test OnStepAsync(Save)
        /// </summary>
        [TestMethod]
        public void Test_OnStepAsyncAtSave_IsntCheckOutAndCorrectActionComand()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    var settingMock = Mock.Create<CollectionsSettingViewModel>(Behavior.CallOriginal);

                    Mock.Arrange(() => settingMock.HasErrors).Returns(false);
                    Mock.Arrange(() => settingMock.CollectionQueueSetting)
                        .Returns(FakeData.FakeCollectionsSystemSettingsAsync());
                    Mock.Arrange(() => settingMock.CollectionSystemDefault)
                        .Returns(FakeData.FakeCollectionSystemDefaultAsync());
                    Mock.Arrange(
                        () =>
                        CollectionsQueueSettingsFunctions.UpdateCollectionsSystemSettings(
                            settingMock.CollectionQueueSetting,
                            settingMock.CollectionSystemDefault)).Returns(Task.Run(() => { }));
                    settingMock.IsCheckedOut = true;
                    settingMock.CanEdit = true;

                    await settingMock.OnStepAsync(CollectionsSettingViewModel.EnumSteps.Save);

                    Assert.IsFalse(settingMock.IsCheckedOut);

                    Assert.IsTrue(
                        settingMock.ActionCommands != null && settingMock.ActionCommands.Count == 1
                        && settingMock.ActionCommands.First()
                               .Parameter.Equals(CollectionsSettingViewModel.EnumSteps.Edit.ToString()));
                });
        }

        /// <summary>
        /// The test OnStepAsync(Cancel)
        /// </summary>
        [TestMethod]
        public void TestOnStepAsyncCancel_IsntCheckOutAndCorrectActionComand()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null)).IgnoreArguments().DoNothing();

                    var settingMock = Mock.Create<CollectionsSettingViewModel>(Behavior.CallOriginal);

                    Mock.Arrange(() => CollectionsQueueSettingsFunctions.ReadCollectionsSystemSettingsAsync())
                        .Returns(Task.FromResult(FakeData.FakeCollectionsSystemSettingsAsync()));
                    Mock.Arrange(() => CollectionsQueueSettingsFunctions.ReadCollectionSystemDefaultAsync())
                        .Returns(Task.FromResult(FakeData.FakeCollectionSystemDefaultAsync()));
                    settingMock.IsCheckedOut = true;
                    settingMock.CanEdit = true;

                    await settingMock.OnStepAsync(CollectionsSettingViewModel.EnumSteps.Cancel);

                    Assert.IsFalse(settingMock.IsCheckedOut);
                    Assert.IsTrue(
                    settingMock.ActionCommands != null && settingMock.ActionCommands.Count == 1
                    && settingMock.ActionCommands.First()
                           .Parameter.Equals(CollectionsSettingViewModel.EnumSteps.Edit.ToString()));
                });
        }

        /// <summary>
        /// The test is percentage false.
        /// </summary>
        [TestMethod]
        public void TestIsPercentageFalse()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();
                    var viewModel = Mock.Create<CollectionsSettingViewModel>(Behavior.CallOriginal);
                    Mock.Arrange(() => viewModel.CollectionQueueSetting).Returns(FakeData.FakeCollectionsSettings());
                    Mock.Arrange(() => viewModel.IsPercentaged).Returns(false);
                    Mock.Arrange(() => viewModel.HasErrors).Returns(true);
                    await viewModel.OnStepAsync(CollectionsSettingViewModel.EnumSteps.Save);
                    var actual = viewModel.CollectionQueueSetting.MinimumArrearsPercent;
                    Assert.AreEqual(0, actual);
                });
        }

        /// <summary>
        /// The test is percentage true.
        /// </summary>
        [TestMethod]
        public void TestIsPercentageTrue()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();
                    var viewModel = Mock.Create<CollectionsSettingViewModel>(Behavior.CallOriginal);
                    Mock.Arrange(() => viewModel.CollectionQueueSetting).Returns(FakeData.FakeCollectionsSettings());
                    Mock.Arrange(() => viewModel.IsPercentaged).Returns(true);
                    Mock.Arrange(() => viewModel.HasErrors).Returns(true);
                    await viewModel.OnStepAsync(CollectionsSettingViewModel.EnumSteps.Save);
                    var actual = viewModel.CollectionQueueSetting.MinimumArrearsAmount;
                    Assert.AreEqual(0, actual);
                });
        }
    }
}
