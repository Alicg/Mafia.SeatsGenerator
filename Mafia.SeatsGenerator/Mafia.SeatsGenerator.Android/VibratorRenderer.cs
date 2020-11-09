using System;
using Android.App;
using Android.Content;
using Android.OS;
using Mafia.SeatsGenerator.Utils;

namespace Mafia.SeatsGenerator.Android
{
    public class VibratorRenderer : IVibrator
    {
        public bool CanVibrate
        {
            get
            {
                if ((int)Build.VERSION.SdkInt >= 11)
                {
                    using (var v = (Vibrator) Application.Context.GetSystemService(Context.VibratorService))
                    {
                        return v.HasVibrator;
                    }
                }
                return true;
            }
        }

        public void Vibrate(int milliseconds)
        {
            using (var v = (Vibrator)Application.Context.GetSystemService(Context.VibratorService))
            {
                if ((int)Build.VERSION.SdkInt >= 11)
                {
#if __ANDROID_11__
                    if (!v.HasVibrator)
                    {
                        Console.WriteLine("Android device does not have vibrator.");
                        return;
                    }
#endif
                }

                if (milliseconds < 0)
                    milliseconds = 0;

                try
                {
                    v.Vibrate((int)milliseconds);
                }
                catch
                {
                    Console.WriteLine("Unable to vibrate Android device, ensure VIBRATE permission is set.");
                }
            }
        }
    }
}