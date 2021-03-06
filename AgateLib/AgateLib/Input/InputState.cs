﻿//
//    Copyright (c) 2006-2018 Erik Ylvisaker
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace AgateLib.Input
{
    public interface IInputState
    {
        GameTime GameTime { get; }

        GamePadState GamePadStateOf(PlayerIndex playerIndex);

        KeyboardState KeyboardState { get; }

        MouseState MouseState { get; }
    }

    /// <summary>
    /// Implements IInputState by polling the hardware.
    /// This will only poll hardware for states that are actually asked for,
    /// and hardware will only be polled once per frame.
    /// </summary>
    public class InputState : IInputState
    {
        private Dictionary<PlayerIndex, GamePadState?> gamePads = new Dictionary<PlayerIndex, GamePadState?>();
        private KeyboardState? keyboardState;
        private MouseState? mouseState;

        public GameTime GameTime { get; set; }

        public KeyboardState KeyboardState
        {
            get
            {
                if (keyboardState == null)
                    keyboardState = Keyboard.GetState();

                return keyboardState.Value;
            }
        }

        public MouseState MouseState
        {
            get
            {
                if (mouseState == null)
                    mouseState = Mouse.GetState();

                return mouseState.Value;
            }
        }

        internal void Initialize()
        {
            gamePads[PlayerIndex.One] = null;
            gamePads[PlayerIndex.Two] = null;
            gamePads[PlayerIndex.Three] = null;
            gamePads[PlayerIndex.Four] = null;
            keyboardState = null;
            mouseState = null;
        }

        /// <summary>
        /// Call to indicate that cached controller and keyboard configurations should be
        /// discarded.
        /// </summary>
        /// <param name="time"></param>
        internal void NewFrame(GameTime time)
        {
            Initialize();

            GameTime = time;
        }

        /// <summary>
        /// Gets the state of a player's gamepad.
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public GamePadState GamePadStateOf(PlayerIndex playerIndex)
        {
            if (gamePads[playerIndex] == null)
            {
                gamePads[playerIndex] = GamePad.GetState(playerIndex);
            }

            return gamePads[playerIndex].Value;
        }

    }
}
