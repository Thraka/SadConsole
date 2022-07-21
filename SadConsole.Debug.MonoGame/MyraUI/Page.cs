using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Myra.Assets;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
//using ReactiveUI;

namespace SadConsole.MyraUI
{
    /// <summary>
    /// Wraps a Myra UI Project xmml file.
    /// </summary>
    public class Page//<TViewModel> : ReactiveUI.IViewFor<TViewModel>
        //where TViewModel : class
    {
        private static IAssetResolver s_resourceResolver;
        private static AssetManager s_assetManager;

        /// <summary>
        /// A reusable asset manager which uses the <see cref="FileAssetResolver"/> (set to the current directory) first, and then falls back to the <see cref="ResourceAssetResolver"/> based on this assembly.
        /// </summary>
        public static AssetManager InternalAssetManager
        {
            get
            {
                if (s_assetManager == null)
                {
                    s_resourceResolver = new ChainedAssetResolver(new FileAssetResolver(Environment.CurrentDirectory), new ResourceAssetResolver(typeof(SadConsole.Debug.MonoGame.Debugger).Assembly, ""));
                    s_assetManager = new AssetManager(s_resourceResolver);
                }

                return s_assetManager;
            }
            set { s_assetManager = value; }
        }

        //private TViewModel _viewModel;

        /// <summary>
        /// The root widget representing this page.
        /// </summary>
        public Widget Root { get; }

        //public TViewModel ViewModel { get => _viewModel; set { _viewModel = value;  } }

        //object IViewFor.ViewModel { get => _viewModel; set => _viewModel = (TViewModel)value; }

        /// <summary>
        /// Loads a Myra UI Project xmml file.
        /// </summary>
        /// <param name="myraXmlFile">The file to load.</param>
        /// <param name="assetManager">The asset manager that will resolve and load the file.</param>
        public Page(string myraXmlFile, AssetManager assetManager)
        {
            string xmlContent = assetManager.Load<string>(myraXmlFile);
            XDocument xDoc = XDocument.Parse(xmlContent);
            XAttribute stylesheetPathAttr = xDoc.Root.Attribute("StylesheetPath");
            Stylesheet stylesheet = Stylesheet.Current;

            if (stylesheetPathAttr != null)
                stylesheet = assetManager.Load<Stylesheet>(stylesheetPathAttr.Value);

            var project = Project.LoadFromXml(xmlContent, assetManager, stylesheet);

            Root = project.Root;

            OnPageLoaded(stylesheet);
        }

        /// <summary>
        /// Method is run after the Myra UI is loaded and the <see cref="Root"/> property is set.
        /// </summary>
        protected virtual void OnPageLoaded(Stylesheet stylesheet) { }
    }
}
