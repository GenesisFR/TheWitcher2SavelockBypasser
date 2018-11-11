// Taken from https://www.pinvoke.net/default.aspx/advapi32.regnotifychangekeyvalue
using System;

namespace TheWitcher2SavelockBypasser
{
    public class RegistryChangeEventArgs : EventArgs
    {
        #region Fields
        private bool _stop;
        private Exception _exception;
        #endregion

        #region Constructor
        public RegistryChangeEventArgs(RegistryChangeMonitor monitor)
        {
            this.Monitor = monitor;
        }
        #endregion

        #region Properties
        public RegistryChangeMonitor Monitor { get; }

        public Exception Exception
        {
            get { return this._exception; }
            set { this._exception = value; }
        }

        public bool Stop
        {
            get { return this._stop; }
            set { this._stop = value; }
        }
        #endregion
    }
}