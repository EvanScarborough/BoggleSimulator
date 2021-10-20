using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Boggle.Helpers
{
    public class Input
    {
        public enum Button { UP, DOWN, LEFT, RIGHT, ACCEPT, BACK, SHIFT, PAUSE };

        private static Dictionary<Button, Keys> _keyMap = new Dictionary<Button, Keys>();
        private static Dictionary<Button, Buttons> _controllerMap = new Dictionary<Button, Buttons>();

        private static Dictionary<Button, int> _delayState = new Dictionary<Button, int>();
        private static MouseState _oldMouseState;
        private static MouseState _newMouseState;

        private static GameWindow _window;
        public static string TextInput { get; set; } = "";
        public static int TextMaxChars { get; set; } = 20;
        private static bool _typingEnabled = false;
        public static bool Typing() { return _typingEnabled; }

        public static bool Clicked => _newMouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Released;
        public static bool RightClicked => _newMouseState.RightButton == ButtonState.Pressed && _oldMouseState.RightButton == ButtonState.Released;
        public static bool Clicking => _newMouseState.LeftButton == ButtonState.Pressed;
        public static bool RightClicking => _newMouseState.RightButton == ButtonState.Pressed;
        public static Point MousePosition => _newMouseState.Position;
        public static bool MouseOver(Rectangle rect)
        {
            return MousePosition.X > rect.Left && MousePosition.Y > rect.Top && MousePosition.X <= rect.Left + rect.Width && MousePosition.Y <= rect.Top + rect.Height;
        }
        public static bool ClickedOn(Rectangle rect) => Clicked && MouseOver(rect);
        public static bool RightClickedOn(Rectangle rect) => RightClicked && MouseOver(rect);
        public static bool ClickingOn(Rectangle rect) => Clicking && MouseOver(rect);
        public static bool RightClickingOn(Rectangle rect) => RightClicking && MouseOver(rect);

        public static bool Initialize(GameWindow window)
        {
            // load key mappings from setup file
            _keyMap.Add(Button.UP, Keys.W);
            _keyMap.Add(Button.DOWN, Keys.S);
            _keyMap.Add(Button.LEFT, Keys.A);
            _keyMap.Add(Button.RIGHT, Keys.D);
            _keyMap.Add(Button.ACCEPT, Keys.Space);
            _keyMap.Add(Button.BACK, Keys.E);
            _keyMap.Add(Button.SHIFT, Keys.LeftShift);
            _keyMap.Add(Button.PAUSE, Keys.Escape);

            _controllerMap.Add(Button.UP, Buttons.LeftThumbstickUp);
            _controllerMap.Add(Button.DOWN, Buttons.LeftThumbstickDown);
            _controllerMap.Add(Button.LEFT, Buttons.LeftThumbstickLeft);
            _controllerMap.Add(Button.RIGHT, Buttons.LeftThumbstickRight);
            _controllerMap.Add(Button.ACCEPT, Buttons.A);
            _controllerMap.Add(Button.BACK, Buttons.B);
            _controllerMap.Add(Button.SHIFT, Buttons.B);
            _controllerMap.Add(Button.PAUSE, Buttons.Start);

            _oldMouseState = Mouse.GetState();
            _newMouseState = _oldMouseState;

            _window = window;

            return true;
        }

        public static bool IsDown(Button button)
        {
            return Keyboard.GetState().IsKeyDown(_keyMap[button]) || GamePad.GetState(PlayerIndex.One).IsButtonDown(_controllerMap[button]);
        }

        public static bool Pressed(Button button)
        {
            if (!_delayState.ContainsKey(button)) _delayState.Add(button, 0);
            if(_delayState[button] <= 0 && IsDown(button))
            {
                _delayState[button] = 20;
                return true;
            }
            return false;
        }

        public static void EnableTyping()
        {
            if (_typingEnabled) return;
            _window.TextInput += Window_TextInput;
            _typingEnabled = true;
        }
        public static void DisableTyping()
        {
            if (!_typingEnabled) return;
            _window.TextInput -= Window_TextInput;
            _typingEnabled = false;
        }
        private static void Window_TextInput(object sender, TextInputEventArgs e)
        {
            if (char.IsControl(e.Character))
            {
                if (e.Character == (char)Keys.Enter)
                {
                    DisableTyping();
                }
                else if (e.Character == (char)Keys.Back)
                {
                    if (TextInput.Length > 0) TextInput = TextInput.Substring(0, TextInput.Length - 1);
                }
            }
            else
            {
                if (TextInput.Length < TextMaxChars) TextInput += e.Character;
            }
        }

        public static void Update(Rectangle bounds)
        {
            _oldMouseState = _newMouseState;
            _newMouseState = Mouse.GetState();
            _newMouseState = new MouseState(
                (int)((double)_newMouseState.X / (double)bounds.Width * (double)Global.SCREEN_WIDTH),
                (int)((double)_newMouseState.Y / (double)bounds.Height * (double)Global.SCREEN_HEIGHT),
                _newMouseState.ScrollWheelValue, _newMouseState.LeftButton, _newMouseState.MiddleButton,
                _newMouseState.RightButton, _newMouseState.XButton1, _newMouseState.XButton2, _newMouseState.HorizontalScrollWheelValue);

            List<Button> reduce = new List<Button>();
            List<Button> tozero = new List<Button>();
            foreach (Button b in _delayState.Keys)
            {
                if (!IsDown(b))
                    tozero.Add(b);
                else reduce.Add(b);
            }
            foreach(Button b in reduce)
            {
                _delayState[b] = _delayState[b] - 1;
            }
            foreach(Button b in tozero)
            {
                _delayState[b] = 0;
            }
        }
    }
}
