using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.ViewModels;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.ViewModels
{
    class MainViewModel: CloseableViewModel
    {
        private IEditor document;

        public List<IEditor> Documents = new List<IEditor>();

        public System.Windows.Input.ICommand NewDocumentCommand { get; private set; }

        public IEditor Document => document;

        public MainViewModel()
        {
            NewDocumentCommand = MugenMvvmToolkit.Models.RelayCommandBase.FromAsyncHandler(ShowNewDocument);
        }

        private async Task ShowNewDocument()
        {
            using (var viewModel = GetViewModel<NewDocumentViewModel>())
            {
                await viewModel.ShowAsync(new MugenMvvmToolkit.Models.DataContext());

                //viewModel.
            }
        }
    }
}
