using System;
using System.Collections.Generic;
using System.Text;
using AgateLib;
using AgateLib.Display;
using BallBusterX.ContentDataModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BallBusterX
{
    public class Sprite
    {
        private int frameTime;
        private int frameTimeLeft;

        List<SpriteFrame> frames = new List<SpriteFrame>();

        public Sprite()
        {
            FrameTime = 50;
        }

        public IReadOnlyList<SpriteFrame> Frames => frames;

        public int CurrentFrameIndex { get; set; }
        public float Alpha { get;  set; }
        public OriginAlignment RotationCenter { get;  set; }
        public float RotationAngleDegrees { get;  set; }
        public OriginAlignment DisplayAlignment { get;  set; }
        public Color Color { get;  set; }
        public float SpriteWidth { get; set; }
        public float DisplayWidth { get;  set; }
        public int RotationAngle { get;  set; }
        public AnimationType AnimationType { get;  set; }
        public int FrameTime
        {
            get => frameTime;
            set
            {
                frameTime = value;
                frameTimeLeft = value;
            }
        }

        internal void Update(GameTime time) {

        }

        internal void Draw(SpriteBatch spriteBatch, int x, int y) => throw new NotImplementedException();
        internal void SetScale(float paddlealpha1, float paddlealpha2) => throw new NotImplementedException();
        internal void Draw(SpriteBatch spriteBatch, float v1, float v2) => throw new NotImplementedException();
    }

    public class SpriteFrame
    {
        public Texture2D Image { get; internal set; }
        public Rectangle? SourceRect { get; internal set; }
    }

    public enum AnimationType
    {
        Loop,
        PingPong,
    }
}
