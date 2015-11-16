using Dear.KeyboardControl;
using System;
using WindowsInput;
using WindowsInput.Native;

namespace NinjaDev {
    class Program {
        static void Main(string[] args) {
            new Program().Start();
        }

        private IKeyboardSimulator _keyboard = new InputSimulator().Keyboard;
        private SnippetManager _manager = new SnippetManager();
        private bool _active;
        private SnippetDriver _currentDriver;
        private KeyInterceptor _interceptor;

        private void Start() {
            Console.WriteLine("go");
            _manager.LoadAll();
            using (_interceptor = new KeyInterceptor()) {
                _interceptor.KeyIntercepted += Keyboard_KeyIntercepted;
                Console.WriteLine($"NinjaDev Started");
                Console.ReadLine();
            }
        }

        private void Keyboard_KeyIntercepted(object sender, KeyInterceptedEventArgs e) {
            Console.WriteLine($"-> {e.KeyName} Code: {e.KeyCode}");
            var k = e.KeyCode;
            if (k == 162 || k == 13) { //control and return
                return;
            }
            if (k == 231) { //automation
                return;
            }
            if (k >= 48 && k <= 57) {
                if (KeyboardPInvoke.GetKeyState(VirtualKey.Control) != -127) { //pressed
                    return;
                }
                e.CancelThisKey();
                int id = k - 48;
                if (id == 0) {
                    _active = false;
                    Console.WriteLine($"NinjaDev Disabled");
                    return;
                }
                var snippet = _manager.GetSnippet(id);
                if (snippet == "") {
                    Console.WriteLine("No snippet with ID " + id);
                    return;
                }
                _currentDriver = new SnippetDriver(snippet);
                Console.WriteLine($"Snippet {id} active");
                _active = true;
                return;
            }

            if (_active) {
                if (_currentDriver.Available) {
                    e.CancelThisKey();
                    var letter = _currentDriver.Next();
                    if (letter == 10) {
                        return;
                    }
                    if (letter == 13) { //enter
                        _keyboard.KeyPress(VirtualKeyCode.RETURN);
                        return;
                    }
                    _keyboard.TextEntry(letter);
                    return;
                }
                else {
                    Console.WriteLine("Snippet is over");
                }
            }
        }
    }
}