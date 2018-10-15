using AgateLib.Scenes;
using Autofac;
using BallBusterX.Scenes;
using Microsoft.Xna.Framework;
using System;

namespace BallBusterX
{
    public class BBX
    {
        private SceneStack scenes = new SceneStack();
        private readonly BBXFactory bbxFactory;

        public BBX(BBXFactory bbxFactory)
        {
            SplashScene splashScene = bbxFactory.CreateSplashScene();

            splashScene.SceneEnd += (_, __) => StartTitle();

            scenes.Add(splashScene);
            this.bbxFactory = bbxFactory;
        }

        private void StartTitle()
        {
            TitleScene titleScene = bbxFactory.CreateTitleScene();
            scenes.Add(titleScene);

            titleScene.BeginGame += BeginGame;
        }

        private void BeginGame(GameState gameState)
        {
            var gameScene = bbxFactory.CreateGameScene(gameState);
            scenes.Add(gameScene);

            gameScene.Pause += Pause;

        }

        private void Pause()
        {
            var pauseScene = bbxFactory.CreatePauseScene();

            scenes.Add(pauseScene);
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

        public PausedScene CreatePauseScene()
        {
            return context.Resolve<PausedScene>();
        }
    }
}
