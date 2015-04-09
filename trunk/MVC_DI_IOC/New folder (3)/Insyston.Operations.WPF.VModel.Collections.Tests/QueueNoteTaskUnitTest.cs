// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueNoteTaskUnitTest.cs" company="TMA">
//   UnitTest
// </copyright>
// <summary>
//   The queue note task unit test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.VModel.Collections.Tests
{
    using System.Windows;

    using Insyston.Operations.Business.Collections;
    using Insyston.Operations.WPF.ViewModels.Collections;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// The queue note task unit test.
    /// </summary>
    [TestClass]
    public class QueueNoteTaskUnitTest
    {
        /// <summary>
        /// The test function get category note.
        /// </summary>
        [TestMethod]
        public void TestFunctionGetCategoryNote()
        {
            Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                .IgnoreArguments().DoNothing();
            var viewModel = Mock.Create<QueueNoteTaskViewModel>(Behavior.CallOriginal);
            Mock.Arrange(() => QueueAssignmentFunctions.GetCategoryTaskActivity())
                .Returns(FakeData.FakeSelectList());
            Mock.SetupStatic(typeof(CollectionsQueueSettingsFunctions), StaticConstructor.Mocked);
            Mock.Arrange(() => CollectionsQueueSettingsFunctions.ReadCollectionSystemDefaultAsync())
                .Returns(FakeData.FakesCollectionDefault());
            var inst = new PrivateAccessor(viewModel);
            inst.CallMethod("GetCategoryNote");
            Assert.IsNotNull(viewModel.ListCategory);
        }

        /// <summary>
        /// The test function get category task.
        /// </summary>
        [TestMethod]
        public void TestFunctionGetCategoryTask()
        {
            Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                .IgnoreArguments().DoNothing();
            var viewModel = Mock.Create<QueueNoteTaskViewModel>(Behavior.CallOriginal);
            Mock.Arrange(() => QueueAssignmentFunctions.GetCategoryTaskActivity())
                .Returns(FakeData.FakeSelectList());
            Mock.SetupStatic(typeof(CollectionsQueueSettingsFunctions), StaticConstructor.Mocked);
            Mock.Arrange(() => CollectionsQueueSettingsFunctions.ReadCollectionSystemDefaultAsync())
                .Returns(FakeData.FakesCollectionDefault());
            var inst = new PrivateAccessor(viewModel);
            inst.CallMethod("GetCategoryTask");
            Assert.IsNotNull(viewModel.ListCategory);
        }
    }
}
