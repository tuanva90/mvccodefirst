using System;
using System.Linq;

namespace Insyston.Operations.WPF.ViewModels.Common.Interfaces
{
    public abstract class SubViewModelUseCaseBase<T> : ViewModelUseCaseBase where T: ViewModelUseCaseBase
    {
        public SubViewModelUseCaseBase(T main) : base(false)
        {
            this.MainViewModel = main;
        }

        protected virtual T MainViewModel { get; private set; }
    }
}