using System;
using System.Collections.Generic;
using System.Text;
using AgateLib;
using AgateLib.Scenes;
using BallBusterX.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BallBusterX
{
    public class BBX
    {
        private GraphicsDevice graphicsDevice;
        private ContentProvider content;
        private readonly CImage img;
        private SceneStack scenes = new SceneStack();

        public BBX(GraphicsDevice graphicsDevice, ContentProvider content)
        {
            this.graphicsDevice = graphicsDevice;
            this.content = content;

            img = new CImage();

            scenes.Add(new SplashScene(graphicsDevice, content, img));
        }

        public void Update(GameTime gameTime)
        {
            scenes.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            scenes.Draw(gameTime);
        }
    }
}
