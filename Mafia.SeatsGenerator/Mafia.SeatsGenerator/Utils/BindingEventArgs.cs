using System;

namespace Mafia.SeatsGenerator.Utils
{
    public class BindingEventArgs : EventArgs
    {
        public BindingEventArgs(object bindingContext)
        {
            this.BindingContext = bindingContext;
        }

        public object BindingContext { get; }
    }
}