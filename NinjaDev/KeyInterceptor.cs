using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NinjaDev {

    public class KeyInterceptor : IDisposable {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private static LowLevelKeyboardProc _proc;
        private static IntPtr _hookID = IntPtr.Zero;

        public event EventHandler<KeyInterceptedEventArgs> KeyIntercepted;

        public KeyInterceptor() {
            Task.Run(() => {
                _proc = HookCallback;
                _hookID = SetHook(_proc);
                Application.Run();
            });
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc) {
            using (Process curProcess = Process.GetCurrentProcess()) {
                using (ProcessModule curModule = curProcess.MainModule) {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


        public int _lasCancelledtDown;

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            int vkCode = Marshal.ReadInt32(lParam);
            if (vkCode == 231) { //automation
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP) {
                if (_lasCancelledtDown == Marshal.ReadInt32(lParam)) {
                    return (IntPtr)1;
                }
            }
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) {
                vkCode = Marshal.ReadInt32(lParam);
                var args = new KeyInterceptedEventArgs(vkCode);
                Console.WriteLine("key-> " + vkCode);
                OnKeyIntercepted(args);
                if (args.KeyCancelled) {
                    _lasCancelledtDown = vkCode;
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public void Dispose() {
            UnhookWindowsHookEx(_hookID);
            Application.Exit();
        }

        protected virtual void OnKeyIntercepted(KeyInterceptedEventArgs e) {
            KeyIntercepted?.Invoke(this, e);
        }
    }
}
