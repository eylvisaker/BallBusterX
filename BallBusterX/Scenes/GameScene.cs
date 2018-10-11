using AgateLib.Input;
using AgateLib.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace BallBusterX.Scenes
{
    public class GameScene : Scene
    {
        private GameState gameState;
        private IMouseEvents mouse;
        private Point mousePos;
        private Song song;
        private readonly CImage img;
        private readonly CSound snd;
        private readonly BBXConfig config;
        private readonly GraphicsDevice graphicsDevice;
        private readonly SpriteBatch spriteBatch;

        public GameScene(GameState gameState, GraphicsDevice graphicsDevice, IMouseEvents mouse, CImage img, CSound snd, BBXConfig config)
        {
            this.gameState = gameState;
            this.graphicsDevice = graphicsDevice;
            this.mouse = mouse;
            this.img = img;
            this.snd = snd;
            this.config = config;
            mouse.MouseMove += Mouse_MouseMove;

            this.spriteBatch = new SpriteBatch(graphicsDevice);

            InitializeLevel();
        }

        private void InitializeLevel()
        {
            gameState.initLevel(true);

            song = PlayRandomSong();
        }

        private Song PlayRandomSong()
        {
            if (snd.music.Count == 0)
                return null;

            int randsong = gameState.random.Next(snd.music.Count);
            Song selectedSong = snd.music[randsong];

            if (config.PlayMusic)
            {
                MediaPlayer.Play(selectedSong);
            }

            return selectedSong;
        }

        private void Mouse_MouseMove(object sender, MouseEventArgs e)
        {
            mousePos = e.MousePosition;

            if (!gameState.paused)
            {
                gameState.MouseMove(mousePos);
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            base.OnUpdate(time);

            mouse.Update(time);

            gameState.UpdateLevel(time);

            IsFinished = gameState.gameover;

            if (gameState.transitionout)
            {
                if (song != null)
                    MediaPlayer.Stop();
            }
        }

        protected override void DrawScene(GameTime time)
        {
            spriteBatch.Begin();

            gameState.DrawLevel(spriteBatch);

            if (gameState.paused)
            {
                img.arrow.Draw(spriteBatch, mousePos.ToVector2());
            }

            spriteBatch.End();
        }
    }
}
