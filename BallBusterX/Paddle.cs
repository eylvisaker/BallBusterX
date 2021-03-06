﻿using AgateLib.Mathematics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BallBusterX
{
    public class Paddle
    {
        private const int minPaddleSizeIndex = -5;
        private const int maxPaddleSizeIndex = 10;

        public const int StandardPaddleWidth = 100;
        public const int GamePaddleY = 570;
        private const float maxPaddleImbueV = 1000.0f;
        private const float minPaddleImbueV = 200.0f;

        const float widthGrowthRate = 40;

        private float targetWidth = StandardPaddleWidth;

        private int paddleSizeIndex;  

        public float Width = StandardPaddleWidth;
        public float Height = 20;

        public float x = 350;
        public float y = GamePaddleY;
        public float RotationAngle;
        public float Opacity = 1;
        public float Velocity;   // velocity that the paddle is moving.
        public float ballImbueVelocity;        // velocity the paddle gives to the balls

        public Paddle()
        {

        }

        public float ImbueV
        {
            get { return ballImbueVelocity; }
            set
            {
                ballImbueVelocity = value;

                if (ballImbueVelocity < minPaddleImbueV)
                    ballImbueVelocity = minPaddleImbueV;
            }
        }

        public int PaddleSizeIndex
        {
            get => paddleSizeIndex;
            set
            {
                paddleSizeIndex = value;

                if (paddleSizeIndex > 10) paddleSizeIndex = 10;
                if (paddleSizeIndex < -5) paddleSizeIndex = -5;

                targetWidth = StandardPaddleWidth + paddleSizeIndex * 10;
            }
        }

        public float basePaddleImbueV; // ordinary velocity the paddle gives
        public float basePaddleImbueVStart, basePaddleImbueVEnd;

        public float LeftBoundary = 60;
        public float RightBoundary = 740;

        public void Update(GameTime time, IReadOnlyList<Ball> balls)
        {
            GrowIfSizeChanged(time, balls);
            CheckEdge();
        }

        public Rectangle HitBox => new Rectangle((int)(x - Width / 2), (int)(y - Height / 2), (int)Width, (int)Height);

        public Vector2 Position => new Vector2(x, y);

        public bool CheckEdge()
        {
            float minx = LeftBoundary + Width / 2;
            float maxx = RightBoundary - Width / 2;

            if (x < minx)
            {
                x = minx;
                return true;
            }
            if (x > maxx)
            {
                x = maxx;
                return true;
            }
            return false;
        }

        private void GrowIfSizeChanged(GameTime time, IReadOnlyList<Ball> balls)
        {
            if (targetWidth != Width)
            {
                var oldWidth = Width;
                Width += MathF.Tanh((targetWidth - Width) / 10) * widthGrowthRate * (float)time.ElapsedGameTime.TotalSeconds;

                if (balls.Count > 0)
                {
                    // now, adjust the positions of the balls, so none of them
                    // are way off the edge of the paddle.
                    int i;
                    for (i = 0; i < balls.Count; i++)
                    {
                        if (balls[i].IsSticking)
                        {
                            // the formula is to keep the balls at the same ratio of distance
                            // from the edge of the paddle to the size of the paddle
                            float oldPos = balls[i].StickyDifference / oldWidth;

                            balls[i].StickyDifference = oldPos * Width;
                        }

                    }
                }
            }
        }

        public void ResetPosition()
        {
            y = 560;
            RotationAngle = 0.0f;
            Opacity = 1.0f;
        }
    }
}
