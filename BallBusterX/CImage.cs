/*****************************************************************************
	Ball: Buster
	Copyright (C) 2004-9 Patrick Avella, Erik Ylvisaker

    This file is part of Ball: Buster.

    Ball: Buster is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    Ball: Buster is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Ball: Buster; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/
using AgateLib;
using AgateLib.Display;
using AgateLib.UserInterface;
using BallBusterX.ContentDataModel;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BallBusterX
{
    public class CImage
    {
        public Texture2D leftborder, rightborder, topborder;

        public Sprite block;
        public Sprite cblock, sblock;

        public Sprite woodblock, marbleblock1, marbleblock2;
        public Sprite rubyblock1, rubyblock2, rubyblock3;


        public Sprite crack;

        public Sprite paddle, flash, smallpaddle, largepaddle, fireball, ball, spike, smash;

        public Sprite pupaddleregular, pupaddlesmall, pupaddlelarge, pufastball, puslowball, puregularspeed,
            pumultiball, pu3ball, pu1up, publaster, pufireball, pusticky, pusupersticky, pureset, purandom,
            pu100, pu250, pu500, pu1000,
            pucatchblue, pucatchred,
            pupow, pusmash, purbswap, pudoor;

        public Sprite arrow, bblogo, xlogo;

        public Texture2D palogo, vtlogo;
        private FontProvider fonts;

        public IFontProvider Fonts => fonts;

        //public TextStyler fontStyler;

        public CImage()
        {
        }

        public void preload(IContentProvider content)
        {
            fonts = new FontProvider();
            fonts.Add("sans", FontLoader.Load(content, "AgateLib/AgateSans"));
            
            this.palogo = content.Load<Texture2D>("imgs/palogo");
            this.vtlogo = content.Load<Texture2D>("imgs/vtlogo");
        }

        public void load(IContentProvider content)
        {
            leftborder = content.Load<Texture2D>("imgs/leftborder");
            rightborder = content.Load<Texture2D>("imgs/rightborder");
            topborder = content.Load<Texture2D>("imgs/topborder");

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new HyphenatedNamingConvention())
                .WithTypeConvertersForBasicStructures()
                .Build();

            var imageData = deserializer.Deserialize<SpritesFile>(
                content.ReadAllText("sprites.yaml"));

            foreach(var field in GetType().GetTypeInfo().DeclaredFields)
            {
                string yamlKey = field.Name.ToLowerInvariant();

                if (field.FieldType == typeof(Texture2D))
                {
                    if (imageData.Images.TryGetValue(yamlKey, out var data))
                    {
                        field.SetValue(this, content.Load<Texture2D>($"imgs/{data.Image}"));
                    }
                    else
                    {
                        Log($"No image specified for {field.Name}");
                    }
                }
                else if (field.FieldType == typeof(Sprite))
                {
                    if (imageData.Sprites.TryGetValue(yamlKey, out var data))
                    {
                        var sprite = new Sprite();

                        sprite.AnimationType = data.Animation?.Type ?? sprite.AnimationType;
                        sprite.FrameTime = data.Animation?.FrameTime ?? sprite.FrameTime;

                        foreach (var frameData in data.Frames)
                        {
                            var frame = new SpriteFrame();

                            frame.Image = content.Load<Texture2D>($"imgs/{frameData.Image}");
                            frame.SourceRect = frameData.SourceRect;

                            sprite.AddFrame(frame);
                        }

                        field.SetValue(this, sprite);
                    }
                    else
                    {
                        Log($"No sprite specified for {field.Name}");
                    }
                }
            }
        }

        private void Log(string message)
        {
            Debug.WriteLine(message);
        }
    }
}