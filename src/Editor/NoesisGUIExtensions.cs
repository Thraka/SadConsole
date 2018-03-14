////////////////////////////////////////////////////////////////////////////////////////////////////
// Noesis Engine - http://www.noesisengine.com
// Copyright (c) 2009-2010 Noesis Technologies S.L. All Rights Reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NoesisGUIExtensions
{
    /// <summary>
    /// Adds stroke capabilities to text elements.
    ///
    /// Usage:
    ///
    ///     <Grid 
    ///       xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///       xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///       xmlns:noesis="clr-namespace:NoesisGUIExtensions">
    ///         <TextBlock noesis:Text.Stroke="Red" noesis:Text.StrokeThickness="1" Text="Hello"/>
    ///     </Grid>
    ///
    /// </summary>
    public class Text
    {
        public Text()
        {
        }

        /// <summary>
        /// Specifies the brush used to stroke the text
        /// </summary>
        #region Stroke attached property

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.RegisterAttached("Stroke", typeof(Brush), typeof(Text),
            new PropertyMetadata(null));

        public static void SetStroke(UIElement element, Brush value)
        {
            element.SetValue(StrokeProperty, value);
        }

        public static Brush GetStroke(UIElement element)
        {
            return (Brush)element.GetValue(StrokeProperty);
        }

        #endregion

        /// <summary>
        /// Specifies the thickness of the text stroke
        /// </summary>
        #region StrokeThickness attached property

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.RegisterAttached("StrokeThickness", typeof(double), typeof(Text),
            new PropertyMetadata(0.0));

        public static void SetStrokeThickness(UIElement element, double value)
        {
            element.SetValue(StrokeThicknessProperty, value);
        }

        public static double GetStrokeThickness(UIElement element)
        {
            return (double)element.GetValue(StrokeThicknessProperty);
        }

        #endregion
    }

    /// <summary>
    /// Provides a base class for projections, which describe how to transform an object
    /// in 3-D space using perspective transforms.
    /// </summary>
    public abstract class Projection : Animatable
    {
    }

    /// <summary>
    /// Represents a perspective transform (a 3-D-like effect) on an object.
    /// </summary>
    public class PlaneProjection : Projection
    {
        /// <summary>
        /// Gets or sets the x-coordinate of the center of rotation of the object you rotate
        /// </summary>
        #region CenterOfRotationX dependency property

        public double CenterOfRotationX
        {
            get { return (double)GetValue(CenterOfRotationXProperty); }
            set { SetValue(CenterOfRotationXProperty, value); }
        }

        public static readonly DependencyProperty CenterOfRotationXProperty = 
            DependencyProperty.Register("CenterOfRotationX", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Gets or sets the y-coordinate of the center of rotation of the object you rotate
        /// </summary>
        #region CenterOfRotationY dependency property

        public double CenterOfRotationY
        {
            get { return (double)GetValue(CenterOfRotationYProperty); }
            set { SetValue(CenterOfRotationYProperty, value); }
        }

        public static readonly DependencyProperty CenterOfRotationYProperty = 
            DependencyProperty.Register("CenterOfRotationY", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Gets or sets the z-coordinate of the center of rotation of the object you rotate
        /// </summary>
        #region CenterOfRotationZ dependency property

        public double CenterOfRotationZ
        {
            get { return (double)GetValue(CenterOfRotationZProperty); }
            set { SetValue(CenterOfRotationZProperty, value); }
        }

        public static readonly DependencyProperty CenterOfRotationZProperty = 
            DependencyProperty.Register("CenterOfRotationZ", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Gets or sets the distance the object is translated along the x-axis of the screen
        /// </summary>
        #region GlobalOffsetX dependency property

        public double GlobalOffsetX
        {
            get { return (double)GetValue(GlobalOffsetXProperty); }
            set { SetValue(GlobalOffsetXProperty, value); }
        }

        public static readonly DependencyProperty GlobalOffsetXProperty = 
            DependencyProperty.Register("GlobalOffsetX", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Gets or sets the distance the object is translated along the y-axis of the screen
        /// </summary>
        #region GlobalOffsetY dependency property

        public double GlobalOffsetY
        {
            get { return (double)GetValue(GlobalOffsetYProperty); }
            set { SetValue(GlobalOffsetYProperty, value); }
        }

        public static readonly DependencyProperty GlobalOffsetYProperty = 
            DependencyProperty.Register("GlobalOffsetY", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Gets or sets the distance the object is translated along the z-axis of the screen
        /// </summary>
        #region GlobalOffsetZ dependency property

        public double GlobalOffsetZ
        {
            get { return (double)GetValue(GlobalOffsetZProperty); }
            set { SetValue(GlobalOffsetZProperty, value); }
        }

        public static readonly DependencyProperty GlobalOffsetZProperty = 
            DependencyProperty.Register("GlobalOffsetZ", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Gets or sets the distance the object is translated along the x-axis of the plane of the object
        /// </summary>
        #region LocalOffsetX dependency property

        public double LocalOffsetX
        {
            get { return (double)GetValue(LocalOffsetXProperty); }
            set { SetValue(LocalOffsetXProperty, value); }
        }

        public static readonly DependencyProperty LocalOffsetXProperty = 
            DependencyProperty.Register("LocalOffsetX", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Gets or sets the distance the object is translated along the y-axis of the plane of the object
        /// </summary>
        #region LocalOffsetY dependency property

        public double LocalOffsetY
        {
            get { return (double)GetValue(LocalOffsetYProperty); }
            set { SetValue(LocalOffsetYProperty, value); }
        }

        public static readonly DependencyProperty LocalOffsetYProperty = 
            DependencyProperty.Register("LocalOffsetY", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Gets or sets the distance the object is translated along the z-axis of the plane of the object
        /// </summary>
        #region LocalOffsetZ dependency property

        public double LocalOffsetZ
        {
            get { return (double)GetValue(LocalOffsetZProperty); }
            set { SetValue(LocalOffsetZProperty, value); }
        }

        public static readonly DependencyProperty LocalOffsetZProperty = 
            DependencyProperty.Register("LocalOffsetZ", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Gets or sets the number of degrees to rotate the object around the x-axis of rotation
        /// </summary>
        #region RotationX dependency property

        public double RotationX
        {
            get { return (double)GetValue(RotationXProperty); }
            set { SetValue(RotationXProperty, value); }
        }

        public static readonly DependencyProperty RotationXProperty = 
            DependencyProperty.Register("RotationX", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Gets or sets the number of degrees to rotate the object around the y-axis of rotation
        /// </summary>
        #region RotationY dependency property

        public double RotationY
        {
            get { return (double)GetValue(RotationYProperty); }
            set { SetValue(RotationYProperty, value); }
        }

        public static readonly DependencyProperty RotationYProperty = 
            DependencyProperty.Register("RotationY", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Gets or sets the number of degrees to rotate the object around the z-axis of rotation
        /// </summary>
        #region RotationZ dependency property

        public double RotationZ
        {
            get { return (double)GetValue(RotationZProperty); }
            set { SetValue(RotationZProperty, value); }
        }

        public static readonly DependencyProperty RotationZProperty = 
            DependencyProperty.Register("RotationZ", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0.0));

        #endregion

        #region CreateInstanceCore implementation

        protected override Freezable CreateInstanceCore()
        {
            return new PlaneProjection();
        }

        #endregion
    }

    /// <summary>
    /// Extends UI elements with properties not supported by WPF but included in Noesis
    /// </summary>
    public static class Element
    {
        /// <summary>
        /// Adds projection capabilities to UI elements.
        /// It Specifies the Projection object used to project the UI element.
        ///
        /// Usage:
        ///
        ///     <Grid
        ///       xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        ///       xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        ///       xmlns:noesis="clr-namespace:NoesisGUIExtensions">
        ///         <Grid.Resources>
        ///             <Storyboard x:Key="AnimProjection" AutoReverse="True" RepeatBehavior="Forever">
        ///                 <DoubleAnimationUsingKeyFrames
        ///                   Storyboard.TargetProperty="(noesis:Element.Projection).(noesis:PlaneProjection.RotationY)"
        ///                   Storyboard.TargetName="Root">
        ///                     <EasingDoubleKeyFrame KeyTime="0:0:2" Value="-60">
        ///                         <EasingDoubleKeyFrame.EasingFunction>
        ///                             <SineEase EasingMode="EaseInOut"/>
        ///                         </EasingDoubleKeyFrame.EasingFunction>
        ///                     </EasingDoubleKeyFrame>
        ///                 </DoubleAnimationUsingKeyFrames>
        ///             </Storyboard>
        ///         </Grid.Resources>
        ///         <Grid.Triggers>
        ///             <EventTrigger RoutedEvent="FrameworkElement.Loaded">
        ///                 <BeginStoryboard Storyboard="{StaticResource AnimProjection}"/>
        ///             </EventTrigger>
        ///         </Grid.Triggers>
        ///         <Grid x:Name="Root">
        ///             <noesis:Element.Projection>
        ///                 <noesis:PlaneProjection RotationY="60"/>
        ///             </noesis:Element.Projection>
        ///             <Rectangle Width="500" Height="300" Fill="#80FF0000"/>
        ///             <TextBlock
        ///               Text="3D Projection, wow!"
        ///               FontSize="40"
        ///               HorizontalAlignment="Center"
        ///               VerticalAlignment="Center"/>
        ///         </Grid>
        ///     </Grid>
        ///
        /// </summary>
        #region Projection attached property

        public static readonly DependencyProperty ProjectionProperty =
            DependencyProperty.RegisterAttached("Projection", typeof(Projection), typeof(Element),
            new PropertyMetadata(null));

        public static void SetProjection(UIElement element, Projection value)
        {
            element.SetValue(ProjectionProperty, value);
        }

        public static Projection GetProjection(UIElement element)
        {
            return (Projection)element.GetValue(ProjectionProperty);
        }

        #endregion

        /// <summary>
        /// Provides Focus Engagement properties: IsFocusEngagementEnabled and IsFocusEngaged.
        /// They can be used to style Control's focus state accordingly.
        /// </summary>
        #region Focus Engagement properties

        public static readonly DependencyProperty IsFocusEngagementEnabledProperty =
            DependencyProperty.RegisterAttached("IsFocusEngagementEnabled", typeof(bool), typeof(Element),
                new PropertyMetadata(null));

        public static void SetIsFocusEngagementEnabled(Control control, bool value)
        {
            control.SetValue(IsFocusEngagementEnabledProperty, value);
        }

        public static bool GetIsFocusEngagementEnabled(Control control)
        {
            return (bool)control.GetValue(IsFocusEngagementEnabledProperty);
        }

        public static readonly DependencyProperty IsFocusEngagedProperty =
            DependencyProperty.RegisterAttached("IsFocusEngaged", typeof(bool), typeof(Element),
                new PropertyMetadata(null));

        public static void SetIsFocusEngaged(Control control, bool value)
        {
            control.SetValue(IsFocusEngagedProperty, value);
        }

        public static bool GetIsFocusEngaged(Control control)
        {
            return (bool)control.GetValue(IsFocusEngagedProperty);
        }

        #endregion
    }

    /// <summary>
    /// Allows executing a Command in response to an event raised by the target object
    /// </summary>
    class EventToCommand
    {
        /// <summary>
        /// Specified the Command that will be executed when event is raised
        /// </summary>
        #region Command attached property
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(EventToCommand));

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }
        #endregion

        /// <summary>
        /// Specifies a parameter that will be passed to the Command execution
        /// </summary>
        #region CommandParameter attached property
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(EventToCommand));

        public static object GetCommandParameter(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }
        #endregion

        /// <summary>
        /// Event that triggers the command execution
        /// </summary>
        #region Event attached property
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.RegisterAttached("Event", typeof(string), typeof(EventToCommand),
                new PropertyMetadata(OnEventChanged));

        public static string GetEvent(DependencyObject obj)
        {
            return (string)obj.GetValue(EventProperty);
        }

        public static void SetEvent(DependencyObject obj, string value)
        {
            obj.SetValue(EventProperty, value);
        }

        private static void OnEventChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            string eventName = (string)args.NewValue;
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            var eventInfo = obj.GetType().GetEvent(eventName);
            if (eventInfo != null)
            {
                if (eventInfo.EventHandlerType == typeof(EventHandler))
                {
                    RegisterHandler(obj, eventInfo, "OnEvent");
                }
                else if (eventInfo.EventHandlerType == typeof(RoutedEventHandler))
                {
                    RegisterHandler(obj, eventInfo, "OnRoutedEvent");
                }
                else if (eventInfo.EventHandlerType == typeof(DependencyPropertyChangedEventHandler))
                {
                    RegisterHandler(obj, eventInfo, "OnDependencyEvent");
                }
            }
        }
        #endregion

        #region Event handlers
        private static void RegisterHandler(DependencyObject target, EventInfo eventInfo, string methodName)
        {
            var methodInfo = typeof(EventToCommand).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            eventInfo.GetAddMethod().Invoke(target,
                new object[] { Delegate.CreateDelegate(eventInfo.EventHandlerType, methodInfo) });
        }

        private static void OnEvent(object sender, EventArgs e)
        {
            if (sender is Clock)
            {
                sender = ((Clock)sender).Timeline;
            }

            ExecuteCommand((DependencyObject)sender);
        }

        private static void OnRoutedEvent(object sender, RoutedEventArgs e)
        {
            e.Handled = ExecuteCommand((DependencyObject)sender);
        }

        private static void OnDependencyEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand((DependencyObject)sender);
        }

        private static bool ExecuteCommand(DependencyObject dob)
        {
            if (dob != null)
            {
                ICommand command = dob.GetValue(CommandProperty) as ICommand;
                object parameter = dob.GetValue(CommandParameterProperty);
                if (command != null && command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
