using AgateLib.Diagnostics;
using AgateLib.Diagnostics.Rendering;
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
        private readonly HighscoreCollection highscores;

        public BBX(BBXFactory bbxFactory, HighscoreCollection highscores)
        {
            SplashScene splashScene = bbxFactory.CreateSplashScene();

            splashScene.SceneEnd += (_, __) => StartTitle();

            scenes.Add(splashScene);
            this.bbxFactory = bbxFactory;
            this.highscores = highscores;
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

            scenes.Clear();
            scenes.Add(gameScene);

            gameScene.Pause += Pause;
            gameScene.StageComplete += () => StageComplete(gameState, gameScene);
            gameScene.SceneEnd += (_, __) =>
            {
                if (highscores.IsNewHighscore(gameState.Score))
                {
                    NewHighscoreScene newHighscoreScene = bbxFactory.CreateNewHighscoreScene();

                    newHighscoreScene.NewScore = gameState.Score;

                    scenes.Add(newHighscoreScene);

                    newHighscoreScene.SceneEnd += (sender, e) => Reset();
                }
                else
                {
                    Reset();
                }
            };
        }

        private void Reset()
        {
            scenes.Clear();

            StartTitle();
        }

        private void StageComplete(GameState gameState, GameScene gameScene)
        {
            var completeScene = bbxFactory.CreateLevelCompleteScene(gameState);

            scenes.Add(completeScene);

            completeScene.SceneEnd += (_, __) =>
            {
                gameScene.InitializeLevel(false);
            };
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

        public StageCompleteScene CreateLevelCompleteScene(GameState gameState)
        {
            return context.Resolve<StageCompleteScene>(new[] { new NamedParameter("gameState", gameState) });
        }

        public NewHighscoreScene CreateNewHighscoreScene()
        {
            return context.Resolve<NewHighscoreScene>();
        }
    }
}
