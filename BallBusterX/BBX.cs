using System;
using System.Collections.Generic;
using System.Text;
using AgateLib;
using AgateLib.Scenes;
using Autofac;
using Autofac.Core;
using BallBusterX.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BallBusterX
{
    public class BBX
    {
        private SceneStack scenes = new SceneStack();

        public BBX(BBXFactory bbxFactory)
        {
            SplashScene splashScene = bbxFactory.CreateSplashScene();

            splashScene.SceneEnd += (_, __) =>
            {
                TitleScene titleScene = bbxFactory.CreateTitleScene();
                scenes.Add(titleScene);

                titleScene.BeginGame += (gameState) =>
                {
                    var gameScene = bbxFactory.CreateGameScene(gameState);
                    scenes.Add(gameScene);
                };
            };

            scenes.Add(splashScene);
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

    public class BBXFactory
    {
        private IComponentContext context;

        public BBXFactory(IComponentContext context)
        {
            this.context = context;
        }

        public SplashScene CreateSplashScene()
        {
            return context.Resolve<SplashScene>();
        }

        public TitleScene CreateTitleScene()
        {
            return context.Resolve<TitleScene>();
        }

        public GameScene CreateGameScene(GameState gameState)
        {
            return context.Resolve<GameScene>(new[] { new NamedParameter("gameState", gameState) });
        }
    }
}
