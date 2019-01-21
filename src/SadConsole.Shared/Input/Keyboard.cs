using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        public ReadOnlyCollection<AsciiKey> KeysPressed => KeysPressedInternal.AsReadOnly();

        private List<AsciiKey> KeysPressedInternal { get; }

        /// <summary>
		/// A collection of keys currently held down.
		/// </summary>
        public ReadOnlyCollection<AsciiKey> KeysDown => KeysDownInternal.AsReadOnly();

		private List<AsciiKey> KeysDownInternal { get; }

        // List that parallels KeysDownInternal of unmapped virtual keys.  Always use AddKeyDown and RemoveKeyDownAt to
		// modify both these lists so they stay parallel.
		private List<Keys> UnmappedVirtualKeysDown { get; }

        /// <summary>
        /// A collection of keys that were just released this frame.
        /// </summary>
        public ReadOnlyCollection<AsciiKey> KeysReleased => KeysReleasedInternal.AsReadOnly();

        private List<AsciiKey> KeysReleasedInternal { get; }

		/// <summary>
		/// How often a key is included in the <see cref="KeysPressedInternal"/> collection after the <see cref="InitialRepeatDelay"/> time has passed.
		/// </summary>
		public float RepeatDelay = 0.04f;

        /// <summary>
        /// The initial delay after a key is first pressed before it is included a second time (while held down) in the <see cref="KeysPressedInternal"/> collection.
        /// </summary>
        public float InitialRepeatDelay = 0.8f;

        public Keyboard()
        {
            KeysPressedInternal = new List<AsciiKey>();
            KeysReleasedInternal = new List<AsciiKey>();
            KeysDownInternal = new List<AsciiKey>();
            UnmappedVirtualKeysDown = new List<Keys>();
        }

        /// <summary>
        /// Clears the <see cref="KeysPressed"/>, <see cref="KeysDown"/>, <see cref="KeysReleased"/> collections.
        /// </summary>
        public void Clear()
        {
            KeysPressedInternal.Clear();
            KeysDownInternal.Clear();
            UnmappedVirtualKeysDown.Clear();
            KeysReleasedInternal.Clear();
        }

        /// <summary>
        /// Returns true if the key is not in the <see cref="KeysDownInternal"/> collection.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True when the key is not being pressed.</returns>
        public bool IsKeyUp(Keys key)
        {
            return !KeysDownInternal.Contains(AsciiKey.Get(key));
        }

        /// <summary>
        /// Returns true if the key is in the <see cref="KeysDownInternal"/> collection.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True when the key is being pressed.</returns>
        public bool IsKeyDown(Keys key)
        {
            return KeysDownInternal.Contains(AsciiKey.Get(key));
        }

        /// <summary>
        /// Returns true when the key is in the <see cref="KeysReleasedInternal"/> collection.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True when the key was released this update frame.</returns>
        public bool IsKeyReleased(Keys key)
        {
            return KeysReleasedInternal.Contains(AsciiKey.Get(key));
        }

        /// <summary>
        /// Returns true when the key is in the <see cref="KeysPressedInternal"/> collection.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True when the key was considered first pressed.</returns>
        public bool IsKeyPressed(Keys key)
        {
            return KeysPressedInternal.Contains(AsciiKey.Get(key));
        }

        // Always use the next routines to add or remove keys from KeysDownInternal or
        // UnmappedVirtualKeysDown.  This ensures that they stay parallel.
        private void AddKeyDown(AsciiKey key, Keys unmappedVirtualKey)
        {
            KeysDownInternal.Add(key);
            UnmappedVirtualKeysDown.Add(unmappedVirtualKey);
        }

        private void RemoveKeyDownAt(int i)
        {
            KeysDownInternal.RemoveAt(i);
            UnmappedVirtualKeysDown.RemoveAt(i);
        }

        /// <summary>
        /// Reads the keyboard state using the <see cref="GameTime"/> from the update frame.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            KeysPressedInternal.Clear();
            KeysReleasedInternal.Clear();

            // Cycle all the keys down known if any are up currently, remove them from KeysDownInternal
            KeyboardState state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            bool shiftPressed = state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);
            var unmappedVirtualKeys = state.GetPressedKeys();

			for (int i = 0; i < KeysDownInternal.Count;)
            {
                if (state.IsKeyUp(UnmappedVirtualKeysDown[i]))
                {
                    KeysReleasedInternal.Add(KeysDownInternal[i]);
                    RemoveKeyDownAt(i);
                }
                else
                    i++;
            }

            // KeysDownInternal now contains all the keys that are currently down except potentially newly pressed keys
            // however the shift state may have changed so if there was an 'a' previously and the shift key was
            // just pressed, then the 'a' needs to get replaced with 'A'.

            // For all new keys down, if we don't know them, add them to pressed, add them to down.
            foreach (var unmappedVirtualKey in unmappedVirtualKeys)
            {
                bool firstPressed = false;

                AsciiKey key = new AsciiKey();
                AsciiKey keyOppositeShift = new AsciiKey();
                int activeKeyIndex = -1;

                // These keys will be mapped since Fill does the mapping automatically
                key.Fill(unmappedVirtualKey, shiftPressed);
                keyOppositeShift.Fill(unmappedVirtualKey, !shiftPressed);

                if (KeysDownInternal.Contains(key))
                {
                    activeKeyIndex = KeysDownInternal.FindIndex(k => k == key);
                    var thisKey = KeysDownInternal[activeKeyIndex];
                    thisKey.TimeHeld += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    KeysDownInternal[activeKeyIndex] = thisKey;
                }
                else if (KeysDownInternal.Contains(keyOppositeShift))
                {
                    activeKeyIndex = KeysDownInternal.FindIndex(k => k == keyOppositeShift);
                    var thisKey = KeysDownInternal[activeKeyIndex];
                    thisKey.Character = key.Character;
                    thisKey.TimeHeld += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    KeysDownInternal[activeKeyIndex] = thisKey;
				}
				else
                {
                    // The physical key (independent of shift) for this character was not being pressed before
                    // so add it in.
                    firstPressed = true;
                    activeKeyIndex = KeysDownInternal.Count;
                    AddKeyDown(key, unmappedVirtualKey);
                }

                var activeKey = KeysDownInternal[activeKeyIndex];

                if (firstPressed)
                {
                    KeysPressedInternal.Add(activeKey);
                }
                else if (activeKey.PostInitialDelay == false && activeKey.TimeHeld >= InitialRepeatDelay)
                {
                    activeKey.PostInitialDelay = true;
                    activeKey.TimeHeld = 0f;
                    KeysPressedInternal.Add(activeKey);
                }
                else if (activeKey.PostInitialDelay && activeKey.TimeHeld >= RepeatDelay)
                {
                    activeKey.TimeHeld = 0f;
                    KeysPressedInternal.Add(activeKey);
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
