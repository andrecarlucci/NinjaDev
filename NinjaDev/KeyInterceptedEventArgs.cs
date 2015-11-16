using System;
using System.Windows.Forms;

namespace NinjaDev {
    public class KeyInterceptedEventArgs : EventArgs {
        public string KeyName { get; }
        public int KeyCode { get; }
        public bool KeyCancelled { get; private set; }

        public KeyInterceptedEventArgs(int evtKeyCode) {
            KeyName = ((Keys)evtKeyCode).ToString();
            KeyCode = evtKeyCode;
        }

        public void CancelThisKey() {
            KeyCancelled = true;
        }
    }
}