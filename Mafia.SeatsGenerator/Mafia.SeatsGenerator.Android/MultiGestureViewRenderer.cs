using Android.Content;
using Android.OS;
using Mafia.SeatsGenerator.Android;
using Mafia.SeatsGenerator.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Application = Android.App.Application;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(MultiGestureView), typeof(MultiGestureViewRenderer))]
namespace Mafia.SeatsGenerator.Android
{
    public class MultiGestureViewRenderer : ViewRenderer<MultiGestureView, View>
    {
        private MultiGestureView _view;
        private VibratorRenderer _vibrator = new VibratorRenderer();

        public MultiGestureViewRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<MultiGestureView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new View(Application.Context));
            }

            if (e.NewElement != null)
            {
                _view = e.NewElement as MultiGestureView;
            }

            Control.SoundEffectsEnabled = true;

            Control.LongClickable = true;
            Control.LongClick += (s, ea) =>
            {
                if (_view != null)
                {
                    _view.LongPressedHandler?.Invoke(s, new BindingEventArgs(_view.BindingContext));
                    if (_view.VibrateOnLongPress)
                    {
                        _vibrator.Vibrate(_view.LongPressVibrationDuration);
                    }
                }
            };

            Control.Clickable = true;
            Control.Click += (s, ea) =>
            {
                if (_view != null)
                {
                    _view.TappedHandler?.Invoke(s, new BindingEventArgs(this._view.BindingContext));
                    if (_view.VibrateOnTap)
                    {
                        _vibrator.Vibrate(_view.TapVibrationDuration);
                    }
                }
            };
        }
    }
}