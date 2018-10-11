using AgateLib.Input;
using AgateLib.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BallBusterX.Scenes
{
    public class GameScene : Scene
    {
        private GameState gameState;
        private IMouseEvents mouse;
        private Point mousePos;
        private readonly CImage img;
        private readonly GraphicsDevice graphicsDevice;
        private readonly SpriteBatch spriteBatch;

        public GameScene(GameState gameState, GraphicsDevice graphicsDevice, IMouseEvents mouse, CImage img)
        {
            this.gameState = gameState;
            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = new SpriteBatch(graphicsDevice);

            this.mouse = mouse;
            this.img = img;

            mouse.MouseMove += Mouse_MouseMove;

            gameState.initLevel(true);
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
