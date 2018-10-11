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
        private readonly CSound snd;
        private SceneStack scenes = new SceneStack();

        public BBX(GraphicsDevice graphicsDevice, GameWindow window, ContentProvider content)
        {
            this.graphicsDevice = graphicsDevice;
            this.content = content;

            img = new CImage();
            snd = new CSound();

            scenes.Add(new SplashScene(graphicsDevice, new MouseEventsFactory(graphicsDevice, window), content, img, snd));
        }

        public event Action NoMoreScenes;

        public void Update(GameTime gameTime)
        {
            scenes.Update(gameTime);

            if (scenes.Count == 0)
            {
                NoMoreScenes?.Invoke();
            }
        }

        public void Draw(GameTime gameTime)
        {
            scenes.Draw(gameTime);
        }
    }
}
