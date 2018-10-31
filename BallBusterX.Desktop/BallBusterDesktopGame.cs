using System;
using AgateLib;
using AgateLib.Input;
using AgateLib.Storage;
using Autofac;
using BallBusterX.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BallBusterX.Desktop
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class BallBusterDesktopGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private BBX bbx;
        private HighscoreCollection highscores;

        public BallBusterDesktopGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            Content.RootDirectory = "Content";
        }

        public BBXConfig Config { get; set; } = new BBXConfig();

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var container = InitializeContainer();

            highscores = container.Resolve<HighscoreCollection>();
            highscores.LoadHighscores();

            Window.Title = "Ball: Buster X";

            bbx = container.Resolve<BBX>();
            bbx.NoMoreScenes += Exit;
        }

        private IContainer InitializeContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(Config).AsSelf();
            builder.RegisterInstance(new CImage()).AsSelf();
            builder.RegisterInstance(new CSound()).AsSelf();
            builder.RegisterInstance(graphics.GraphicsDevice).AsSelf();
            builder.RegisterInstance(Window).As<GameWindow>();
            builder.RegisterInstance(new ContentProvider(Content)).AsImplementedInterfaces();
            builder.RegisterInstance(new WorldCollection());
            builder.RegisterType<BBX>();
            builder.RegisterType<TitleScene>();
            builder.RegisterType<SplashScene>();
            builder.RegisterType<GameScene>();
            builder.RegisterType<PausedScene>();
            builder.RegisterType<StageCompleteScene>();
            builder.RegisterType<GameState>();
            builder.RegisterType<NewHighscoreScene>();
            builder.RegisterType<HighscoreCollection>().SingleInstance();
            builder.RegisterType<MouseEvents>().AsImplementedInterfaces();
            builder.RegisterType<UserStorage>();
            builder.Register(c => new BBXFactory(c.Resolve<IComponentContext>())).AsSelf().SingleInstance();
            builder.Register(c => new GameStateFactory(c.Resolve<IComponentContext>())).AsSelf().AsImplementedInterfaces().SingleInstance();

            return builder.Build();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            bbx.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            bbx.Draw(gameTime);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            highscores.SaveHighscores();
        }
    }
}
