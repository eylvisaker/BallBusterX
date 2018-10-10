using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BallBusterX.ContentDataModel
{
    public class SpritesFile
    {
        public Dictionary<string, ImageData> Images { get; set; }

        public Dictionary<string, SpriteData> Sprites { get; set; }
    }

    public class SpriteData
    {
        public List<SpriteFrameData> Frames { get; set; }
        public SpriteAnimationData Animation { get; set; }
    }

    public class SpriteAnimationData
    {
        public AnimationType? Type { get; set; }

        public int? FrameTime { get; set; }
    }

    public class SpriteFrameData
    {
        public string Image { get; set; }

        public Rectangle? SourceRect { get; set; }
    }

    public class ImageData
    {
        public string Image { get; set; }
    }
}
