using System;
using System.Collections.Generic;
using System.Text;
using AgateLib;
using AgateLib.Scenes;
using AgateLib.UserInterface;
using AgateLib.UserInterface.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BallBusterX.Scenes
{
    public class SplashScene : Scene
    {
        private readonly GraphicsDevice graphicsDevice;
        private readonly IContentProvider content;
        private readonly CImage img;
        private readonly CSound snd;
        private readonly SpriteBatch spriteBatch;
        private readonly IContentLayout textLayout;
        private readonly double initialDelay;
        private double delay = 500;
        private bool loaded;

        public SplashScene(GraphicsDevice graphicsDevice, IContentProvider content, CImage img, CSound snd)
        {
            this.graphicsDevice = graphicsDevice;
            this.content = content;
            this.img = img;
            this.snd = snd;
            this.spriteBatch = new SpriteBatch(graphicsDevice);

            img.preload(content);

            if (System.Diagnostics.Debugger.IsAttached)
                delay = 5000;

            StringBuilder text = new StringBuilder();

            text.AppendLine("Ball: Buster eXtreme");
            text.AppendLine("Copyright 2004-18 Patrick Avella, Erik Ylvisaker");
            text.AppendLine("Game Programming: Patrick Avella (patrickavella.com)");
            text.AppendLine("eXtreme Version Programming: Erik Ylvisaker (vermiliontower.com)");
            text.AppendLine("Game Art: Patrick Avella (patrickavella.com)");
            text.AppendLine("Background Music: Partners in Rhyme (musicloops.com)");
            text.AppendLine("Sound Effects: A1 Free Sound Effects (a1freesoundeffects.com)");

            ContentLayoutEngine layout = new ContentLayoutEngine(img.Fonts);
            textLayout = layout.LayoutContent(text.ToString(), font: new FontStyleProperties { Color = Color.Black });

            initialDelay = delay;
        }

        protected override void OnUpdate(GameTime time)
        {
            base.OnUpdate(time);

            if (delay < initialDelay && !loaded)
            {
                img.load(content);
                snd.Load(content);

                loaded = true;
            }

            delay -= time.ElapsedGameTime.TotalMilliseconds;

            if (delay < 0)
            {
                IsFinished = true;
            }
        }

        protected override void DrawScene(GameTime time)
        {
            graphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            spriteBatch.Draw(img.palogo, new Vector2(165, 100), Color.White);
            spriteBatch.Draw(img.vtlogo, new Vector2(365, 100), Color.White);

            textLayout.Draw(new Vector2(175, 250), spriteBatch);
                
            spriteBatch.End();
            base.DrawScene(time);
        }
    }
}
