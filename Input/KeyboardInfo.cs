#if !SHARPDX
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
#else
using SharpDX.DirectInput;
using SharpDX.Toolkit;
using Keys = SharpDX.DirectInput.Key;
#endif
using System.Collections.Generic;
using System.Linq;

namespace SadConsole.Input
{
    public class KeyboardInfo
    {
        public List<AsciiKey> KeysPressed { get; internal set; }
        public List<AsciiKey> KeysDown { get; internal set; }
        public List<AsciiKey> KeysReleased { get; internal set; }

        public float RepeatDelay = 0.04f;
        public float InitialRepeatDelay = 0.8f;

        public KeyboardInfo()
        {
            KeysPressed = new List<AsciiKey>();
            KeysReleased = new List<AsciiKey>();
            KeysDown = new List<AsciiKey>();
        }

        public void Clear()
        {
            KeysPressed.Clear();
            KeysDown.Clear();
            KeysReleased.Clear();
        }

        public void ProcessKeys(GameTime gameTime)
        {
            this.KeysPressed.Clear();
            this.KeysReleased.Clear();

#if !SILVERLIGHT && !SHARPDX
            KeyboardState state = Keyboard.GetState();
            bool shiftPressed = state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);
            var keys = state.GetPressedKeys();
#elif SHARPDX
            Keyboard k = new Keyboard(new DirectInput());
            KeyboardState state = k.GetCurrentState();
            k.Dispose();

            bool shiftPressed = state.IsPressed(Keys.LeftShift) || state.IsPressed(Keys.RightShift);
            var keys = state.PressedKeys;
#else
            KeyboardState state = Keyboard.GetState();
            bool shiftPressed = state.IsKeyDown(Keys.Shift);
            var keys = state.GetPressedKeys();
#endif



            // Cycle all the keys down known if any are up currently, remove
            for (int i = 0; i < this.KeysDown.Count; )
            {
#if SHARPDX
                if (!state.PressedKeys.Contains(this.KeysDown[i].XnaKey))
#else
                if (state.IsKeyUp(this.KeysDown[i].XnaKey))
#endif
                {
                    KeysReleased.Add(this.KeysDown[i]);
                    this.KeysDown.Remove(this.KeysDown[i]);
                }
                else
                    i++;
            }

            // For all new keys down, if we don't know them, add them to pressed, add them to down.
#if SHARPDX
            for (int i = 0; i < keys.Count; i++)
#else
            for (int i = 0; i < keys.Length; i++)
#endif
            {
                bool firstPressed = false;

                Input.AsciiKey key = new AsciiKey();
                Input.AsciiKey keyOppositeShift = new AsciiKey();
                Input.AsciiKey activeKey;

                key.Fill(keys[i], shiftPressed);
                keyOppositeShift.Fill(keys[i], !shiftPressed);

                if (this.KeysDown.Contains(key))
                {
                    activeKey = this.KeysDown.First(k => k == key);
                    activeKey.TimeHeld += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    this.KeysDown.Remove(key);
                }
                else if (this.KeysDown.Contains(keyOppositeShift))
                {
                    activeKey = this.KeysDown.First(k => k == keyOppositeShift);
                    activeKey.Character = key.Character;
                    activeKey.TimeHeld += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    this.KeysDown.Remove(keyOppositeShift);
                }
                else
                {
                    activeKey = key;
                    firstPressed = true;
                }

                if (firstPressed)
                {
                    this.KeysPressed.Add(activeKey);
                }
                else if (activeKey.PreviouslyPressed == false && activeKey.TimeHeld >= InitialRepeatDelay)
                {
                    activeKey.PreviouslyPressed = true;
                    activeKey.TimeHeld = 0f;
                    this.KeysPressed.Add(activeKey);
                }
                else if (activeKey.PreviouslyPressed == true && activeKey.TimeHeld >= RepeatDelay)
                {
                    activeKey.TimeHeld = 0f;
                    this.KeysPressed.Add(activeKey);
                }

                this.KeysDown.Add(activeKey);
            }
        }

        public bool IsKeyUp(Keys key)
        {
            return !KeysDown.Contains(AsciiKey.Get(key));
        }

        public bool IsKeyDown(Keys key)
        {
            return KeysDown.Contains(AsciiKey.Get(key));
        }

        public bool IsKeyReleased(Keys key)
        {
            return KeysReleased.Contains(AsciiKey.Get(key));
        }
    }
}
