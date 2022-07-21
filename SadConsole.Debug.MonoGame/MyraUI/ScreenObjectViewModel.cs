//using System;
//using System.Collections.Generic;
//using System.Text;
//using ReactiveUI;

//namespace SadConsole.MyraUI
//{
//    public class ScreenObjectViewModel: ReactiveObject
//    {
//        private IScreenObject _screenObject;
//        private bool _isVisible;
//        private bool _isEnabled;

//        public ScreenObjectViewModel(IScreenObject screenObject)
//        {
//            _screenObject = screenObject;
            
//        }

//        public bool IsVisible
//        {
//            get => _screenObject.IsVisible;
//            set { _screenObject.IsVisible = value; this.RaisePropertyChanged(); }
//        }
//    }
//}
