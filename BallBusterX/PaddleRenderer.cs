using AgateLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BallBusterX
{
    public interface IPaddleRenderer
    {
        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }

    public class PaddleRenderer : IPaddleRenderer
    {
        private readonly Paddle paddle;
        private readonly CImage img;
        private readonly GraphicsDevice graphics;
        private readonly IContentProvider content;

        private RenderTarget2D renderTarget;
        private SpriteBatch mySpriteBatch;

        private const int overhang = 4;
        private int imageWidth;

        private float frame;
        private const float animationFrameTime = 50;
        private const int paddleHeight = 20;

        public PaddleRenderer(Paddle paddle, CImage img, GraphicsDevice graphics)
        {
            this.paddle = paddle;
            this.img = img;
            this.graphics = graphics;

            renderTarget = new RenderTarget2D(graphics, 200, 40);
            mySpriteBatch = new SpriteBatch(graphics);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var srcRect = new Rectangle(0, paddleHeight * (int)(frame / animationFrameTime), renderTarget.Width, paddleHeight);
            var origin = new Vector2(srcRect.Width / 2, srcRect.Height / 2);

            spriteBatch.Draw(renderTarget,
                             paddle.Position,
                             srcRect,
                             Color.White,
                             paddle.RotationAngle,
                             origin,
                             paddle.Opacity, // using opacity for scale during death animation.
                             SpriteEffects.None,
                             0);

            //Texture2D pad = img.paddle;

            //int roundedWidth = (int)(paddle.Width + 0.5f);
            //int leftWidth = roundedWidth / 2;
            //int rightWidth = roundedWidth - leftWidth;

            //pad.Scale = new Vector2(paddleOpacity, paddleOpacity);
            //pad.Alpha = paddleOpacity;
            //pad.RotationAngleDegrees = paddleRotationAngle;

            //pad.SetRotationCenter(OriginAlignment.Center);
            //pad.Draw(spriteBatch,
            //         new Vector2(paddle.x, paddle.y + paddle.Height / 2));

            //pad.SetRotationCenter(OriginAlignment.Center);

            //pad.Draw(spriteBatch,
            //         new Vector2(paddle.x + leftWidth, paddle.y + paddle.Height / 2));

            // Uncomment this to see the hitbox for the paddle.
            //if (!dying)
            //{
            //    FillRect(spriteBatch,
            //        paddle.HitBox, new Color(44, 44, 00, 99));
            //}
        }

        public void Update(GameTime gameTime)
        {
            frame += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            frame %= (animationFrameTime * 2);

            CheckPaddleImage();
        }

        private void CheckPaddleImage()
        {
            int drawWidth = (int)(paddle.Width + 0.5f) + overhang * 2;

            if (drawWidth != imageWidth)
            {
                var globalRenderTargets = graphics.GetRenderTargets();

                graphics.SetRenderTarget(renderTarget);

                graphics.Clear(Color.Transparent);

                int leftWidth = drawWidth / 2;
                int rightWidth = drawWidth - leftWidth;
                int srcHeight = img.newpaddle.Height;

                Point renderTargetCenter = new Point(renderTarget.Width / 2, renderTarget.Height / 2);

                mySpriteBatch.Begin(blendState: BlendState.NonPremultiplied);

                mySpriteBatch.Draw(img.newpaddle,
                                   new Rectangle(renderTargetCenter.X - leftWidth, 0, leftWidth, srcHeight),
                                   new Rectangle(0, 0, leftWidth, srcHeight),
                                   Color.White);

                mySpriteBatch.Draw(img.newpaddle,
                                   new Rectangle(renderTargetCenter.X, 0, rightWidth, srcHeight),
                                   new Rectangle(img.newpaddle.Width - rightWidth, 0, rightWidth, srcHeight),
                                   Color.White);

                mySpriteBatch.End();

                graphics.SetRenderTargets(globalRenderTargets);

                imageWidth = drawWidth;
            }
        }
    }
}
