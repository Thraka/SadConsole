#region Copyright

// ****************************************************************************
// <copyright file="WinFormsDataBindingModule.cs">
// Copyright (c) 2012-2017 Vyacheslav Volkov
// </copyright>
// ****************************************************************************
// <author>Vyacheslav Volkov</author>
// <email>vvs0205@outlook.com</email>
// <project>MugenMvvmToolkit</project>
// <web>https://github.com/MugenMvvmToolkit/MugenMvvmToolkit</web>
// <license>
// See license.txt in this solution or http://opensource.org/licenses/MS-PL
// </license>
// ****************************************************************************

#endregion

using MugenMvvmToolkit.Binding;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.WinForms.Binding.Infrastructure;

namespace MugenMvvmToolkit.WinForms.Binding.Modules
{
    public class CustomDataBindingMembers : IModule
    {
        #region Properties

        public int Priority => ApplicationSettings.ModulePriorityInitialization + 1;

        #endregion

        #region Implementation of interfaces

        public bool Load(IModuleContext context)
        {
            //BindingBuilderExtensions.RegisterDefaultBindingMember<SadConsole.Editor.FormsControls.ColorPresenter>(() => (c) => c.Color);

            return true;
        }

        public void Unload(IModuleContext context)
        {
        }

        #endregion
    }
}
