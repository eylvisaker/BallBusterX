using AgateLib;
using AgateLib.Display;
using AgateLib.Input;
using AgateLib.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BallBusterX.Scenes
{
    public class PausedScene : Scene
    {
        private readonly CImage img;
        private Font font;
        private Texture2D whiteTexture;
        private SpriteBatch spriteBatch;
        private KeyboardEvents keyboard;
        private float pauseTimer;

        public PausedScene(GraphicsDevice device, CImage img, IContentProvider content)
        {
            this.img = img;
            this.font = new Font(img.Fonts.Default);
            this.whiteTexture = content.Load<Texture2D>("imgs/white");

            font.Size = 30;
            font.Color = Color.White;
            font.TextAlignment = OriginAlignment.Center;

            this.spriteBatch = new SpriteBatch(device);

            keyboard = new KeyboardEvents();
            keyboard.KeyUp += Keyboard_KeyUp;

            UpdateBelow = false;
            DrawBelow = true;
        }

        private void Keyboard_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Keys.Pause || e.Key == Keys.P || e.Key == Keys.Escape)
                IsFinished = true;
        }

        protected override void OnUpdate(GameTime time)
        {
            base.OnUpdate(time);
            keyboard.Update(time);
        }

        protected override void DrawScene(GameTime time)
        {
            pauseTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
            pauseTimer %= 1000;

            spriteBatch.Begin();

            Rectangle myrect = new Rectangle(0, 0, 800, 600);
            Color mycolor = new Color(100, 0, 0, 100);

            FillRect(myrect, mycolor);

            if (pauseTimer % 1000 < 500)
            {
                font.DrawText(spriteBatch, new Vector2(400, 200), "PAUSED");
                font.DrawText(spriteBatch, new Vector2(400, 300), "Press 'P' to unpause.");
            }

            var mouseState = Mouse.GetState();

            img.arrow.Draw(spriteBatch, new Vector2(mouseState.X, mouseState.Y));

            spriteBatch.End();
        }

        private void FillRect(Rectangle rectangle, Color color)
        {
            Color premulColor = color * (color.A / 255.0f);
            premulColor.A = color.A;

            spriteBatch.Draw(whiteTexture, rectangle, premulColor);
        }
    }
}
