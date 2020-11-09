﻿using System;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Utils
{
    public class MultiGestureView : ContentView
    {
        /// <summary>
        /// Enable vibration on long press.
        /// </summary>
        public bool VibrateOnLongPress { get; set; }

        /// <summary>
        /// Vibration duration in milliseconds on long press. The default value is 100ms.
        /// </summary>
        public int LongPressVibrationDuration { get; set; } = 100;

        /// <summary>
        /// Enable vibration on tap.
        /// </summary>
        public bool VibrateOnTap { get; set; }

        /// <summary>
        /// Vibration duration in milliseconds on tap. The default value is 100ms.
        /// </summary>
        public int TapVibrationDuration { get; set; } = 100;

        /// <summary>
        /// Long press event.
        /// If the Content or its children have gesture recognizers set, in order to prevent gesture conflicts, it is recommended to set their InputTransparent property to True.
        /// </summary>
        public event EventHandler<BindingEventArgs> LongPressed
        {
            add { LongPressedHandler += value; }
            remove { LongPressedHandler -= value; }
        }
        public EventHandler<BindingEventArgs> LongPressedHandler;
        
        /// <summary>
        /// Tap event.
        /// If the Content or its children have gesture recognizers set, in order to prevent gesture conflicts, it is recommended to set their InputTransparent property to True.
        /// </summary>
        public event EventHandler<BindingEventArgs> Tapped
        {
            add { TappedHandler += value; }
            remove { TappedHandler -= value; }
        }
        public EventHandler<BindingEventArgs> TappedHandler;
        
        /// <summary>
        /// Right click event. Only works on UWP.
        /// </summary>
        public event EventHandler RightClicked
        {
            add { RightClickedHandler += value; }
            remove { RightClickedHandler -= value; }
        }
        public EventHandler RightClickedHandler;
    }
}