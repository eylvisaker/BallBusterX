﻿using System;
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
        private GraphicsDevice graphicsDevice;
        private ContentProvider content;
        private readonly CImage img;
        private readonly SpriteBatch spriteBatch;
        private readonly IContentLayout textLayout;
        private readonly double initialDelay;
        private double delay = 500;

        public SplashScene(GraphicsDevice graphicsDevice, ContentProvider content, CImage img)
        {
            this.graphicsDevice = graphicsDevice;
            this.content = content;
            this.img = img;

            this.spriteBatch = new SpriteBatch(graphicsDevice);

            img.preload(content);

            if (System.Diagnostics.Debugger.IsAttached)
                delay = 5000;


            StringBuilder text = new StringBuilder();

            text.AppendLine("Ball: Buster eXtreme.NET");
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

            if (delay < initialDelay)
            {
                img.load(content);
            }

            delay -= time.ElapsedGameTime.TotalMilliseconds;

            if (delay < 0)
            {
                IsFinished = true;
                SceneStack.Add(new TitleScene(graphicsDevice, content, img, new CSound(), new BBXConfig()));
            }
        }

        protected override void DrawScene(GameTime time)
        {
            graphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            spriteBatch.Draw(img.palogo, new Vector2(235, 100), Color.White);
            spriteBatch.Draw(img.vtlogo, new Vector2(435, 100), Color.White);

            textLayout.Draw(new Vector2(175, 250), spriteBatch);
                
            spriteBatch.End();
            base.DrawScene(time);
        }
    }
}
