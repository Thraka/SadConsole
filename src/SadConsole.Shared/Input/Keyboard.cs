using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace SadConsole.Input
{
    /// <summary>
    /// Represents the state of the keyboard.
    /// </summary>
    public class Keyboard
    {
        /// <summary>
        /// A collection of keys registered as pressed which behaves like a command prompt when holding down keys. Uses the <see cref="RepeatDelay"/> and <see cref="InitialRepeatDelay"/> settings.
        /// </summary>
        public List<AsciiKey> KeysPressed { get; internal set; }

        /// <summary>
        /// A collection of keys currently held down.
        /// </summary>
        public List<AsciiKey> KeysDown { get; internal set; }

        /// <summary>
        /// A collection of keys that were just released this frame.
        /// </summary>
        public List<AsciiKey> KeysReleased { get; internal set; }

        /// <summary>
        /// How often a key is included in the <see cref="KeysPressed"/> collection after the <see cref="InitialRepeatDelay"/> time has passed.
        /// </summary>
        public float RepeatDelay = 0.04f;

        /// <summary>
        /// The initial delay after a key is first pressed before it is included a second time (while held down) in the <see cref="KeysPressed"/> collection.
        /// </summary>
        public float InitialRepeatDelay = 0.8f;

        public Keyboard()
        {
            KeysPressed = new List<AsciiKey>();
            KeysReleased = new List<AsciiKey>();
            KeysDown = new List<AsciiKey>();
        }

        /// <summary>
        /// Clears the <see cref="KeysPressed"/>, <see cref="KeysDown"/>, <see cref="KeysReleased"/> collections.
        /// </summary>
        public void Clear()
        {
            KeysPressed.Clear();
            KeysDown.Clear();
            KeysReleased.Clear();
        }

        /// <summary>
        /// Returns true if the key is not in the <see cref="KeysDown"/> collection.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True when the key is not being pressed.</returns>
        public bool IsKeyUp(Keys key)
        {
            return !KeysDown.Contains(AsciiKey.Get(key));
        }

        /// <summary>
        /// Returns true if the key is in the <see cref="KeysDown"/> collection.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True when the key is being pressed.</returns>
        public bool IsKeyDown(Keys key)
        {
            return KeysDown.Contains(AsciiKey.Get(key));
        }

        /// <summary>
        /// Returns true when the key is in the <see cref="KeysReleased"/> collection.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True when the key was released this update frame.</returns>
        public bool IsKeyReleased(Keys key)
        {
            return KeysReleased.Contains(AsciiKey.Get(key));
        }

        /// <summary>
        /// Returns true when the key is in the <see cref="KeysPressed"/> collection.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True when the key was considered first pressed.</returns>
        public bool IsKeyPressed(Keys key)
        {
            return KeysPressed.Contains(AsciiKey.Get(key));
        }


        /// <summary>
        /// Reads the keyboard state using the <see cref="GameTime"/> from the update frame.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            this.KeysPressed.Clear();
            this.KeysReleased.Clear();

            // Cycle all the keys down known if any are up currently, remove
            KeyboardState state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            bool shiftPressed = state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);
            var keys = state.GetPressedKeys();
            for (int i = 0; i < this.KeysDown.Count;)
            {
                if (state.IsKeyUp(this.KeysDown[i].Key))
                {
                    KeysReleased.Add(this.KeysDown[i]);
                    this.KeysDown.Remove(this.KeysDown[i]);
                }
                else
                    i++;
            }

            // KeysDown now contains all the keys that are currently down except potentially newly pressed keys
            // however the shift state may have changed so if there was an 'a' previously and the shift key was
            // just pressed, then the 'a' needs to get replaced with 'A'.

            // For all new keys down, if we don't know them, add them to pressed, add them to down.
            for (int i = 0; i < keys.Length; i++)
            {
                bool firstPressed = false;

                AsciiKey key = new AsciiKey();
                AsciiKey keyOppositeShift = new AsciiKey();
                AsciiKey activeKey;

                key.Fill(keys[i], shiftPressed);
                keyOppositeShift.Fill(keys[i], !shiftPressed);

				if (this.KeysDown.Contains(key))
                {
					activeKey = this.KeysDown.First(k => k == key);
                    // NOTE: activeKey is a STRUCT - thus the above call gives a copy of it rather than the one in KeysDown.
                    // This means we are increasing the TimeHeld on the copy rather than on the one in KeysDown.  See the
                    // comments below.
                    activeKey.TimeHeld += (float)gameTime.ElapsedGameTime.TotalSeconds;
				}
				else if (this.KeysDown.Contains(keyOppositeShift))
                {
                    activeKey = this.KeysDown.First(k => k == keyOppositeShift);
                    activeKey.Character = key.Character;
                    activeKey.TimeHeld += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
				else
                {
                    // The physical key (independent of shift) for this character was not being pressed before
                    // so add it in.
                    activeKey = key;
                    firstPressed = true;
				}

                // NOTE: The key removed here is NOT the same as the one added.  The comparer for AsciiKey depends only on
                // character values.  As noted in the above comments, the key in KeysDown has not had it's TimeHeld
                // incremented - the copy in activeKey has.  Therefore, taking the original one out and inserting the new one maintains a
                // valid TimeHeld.  So in spite of the following two statements looking like inverses, they are actually
                // vitally important.  That being said, it seems like it would be far preferable to just modify the one
                // in KeysDown rather than getting copies, modifying the copy, removing the original and inserting the 
                // copy.  That would entail keeping an index into KeysDown rather than activeKey I think. I think this 
                // definitely needs some rethinking but I'm pressing forward for the moment.
				this.KeysDown.Remove(activeKey);
				this.KeysDown.Add(activeKey);

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
			}
		}

        /// <summary>
        /// Send the keyboard to the active console.
        /// </summary>
        public void Process()
        {
            if (Global.FocusedConsoles.Console != null && Global.FocusedConsoles.Console.UseKeyboard)
                Global.FocusedConsoles.Console.ProcessKeyboard(this);
        }
    }
}
