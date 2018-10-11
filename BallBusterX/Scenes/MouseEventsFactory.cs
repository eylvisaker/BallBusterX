using System;
using System.Collections.Generic;
using System.Text;
using AgateLib.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BallBusterX.Scenes
{
    public interface IMouseEventsFactory
    {
        MouseEvents CreateMouseEvents();
    }

    public class MouseEventsFactory : IMouseEventsFactory
    {
        private readonly GraphicsDevice device;
        private readonly GameWindow window;

        public MouseEventsFactory(GraphicsDevice device, GameWindow window)
        {
            this.device = device;
            this.window = window;
        }

        public MouseEvents CreateMouseEvents() => new MouseEvents(device, window);
    }
}
