using AgateLib.Display;
using AgateLib.Input;
using AgateLib.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace BallBusterX.Scenes
{
    public class NewHighscoreScene : Scene
    {
        private readonly GraphicsDevice graphics;
        private readonly CImage img;
        private readonly HighscoreCollection highscores;
        private readonly SpriteBatch spriteBatch;
        private readonly KeyboardEvents keyboard;
        private readonly Font font;

        private Highscore ns = new Highscore();
        private bool addhighscore;
        private int addposition;

        public NewHighscoreScene(GraphicsDevice graphics,
                                 CImage img,
                                 HighscoreCollection highscores)
        {
            this.graphics = graphics;
            this.img = img;
            this.highscores = highscores;

            this.spriteBatch = new SpriteBatch(graphics);
            this.keyboard = new KeyboardEvents();

            keyboard.KeyPress += Keyboard_KeyPress;

            DrawBelow = false;
            UpdateBelow = false;

            font = new Font(img.Fonts.Default);
        }

        public int NewScore
        {
            get => ns.score;
            set
            {
                ns.score = value;

                addposition = highscores.Count;

                for (int i = 0; i < highscores.Count; i++)
                {
                    if (value > highscores[i].score)
                    {
                        addhighscore = true;
                        addposition = i;

                        break;
                    }
                }
            }
        }

        protected override void OnSceneStart()
        {
            base.OnSceneStart();

            highscores.Insert(addposition, ns);
        }

        private void Keyboard_KeyPress(object sender, KeyPressEventArgs e)
        {
            Keys key = e.Key;

            // Is Keys.Back the same as the backspace key?
            if (key == Keys.Back && ns.name.Length > 0)
            {
                ns.name = ns.name.Substring(0, ns.name.Length - 1);
            }
            else if (key == Keys.Enter && ns.name.Length > 0)
            {
                IsFinished = true;
            }
            else if (ns.name.Length < 12)
            {
                ns.name += e.KeyString;
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            base.OnUpdate(time);

            keyboard.Update(time);
        }

        protected override void DrawScene(GameTime time)
        {
            graphics.Clear(Color.White);

            spriteBatch.Begin();

            font.Size = 24;
            font.Color = Color.Black;
            font.DrawText(spriteBatch, 200, 50, "New High Score!");

            for (int j = 0; j < highscores.Count; j++)
            {
                int Drawy = 100 + 40 * j;

                font.Color = Color.Black;

                if (addposition == j)
                {
                    font.Color = ColorX.FromHsv((time.TotalGameTime.TotalMilliseconds % 3600) / 10.0f, 0.7f, 0.7f);
                }

                font.DrawText(spriteBatch, 200, Drawy, (j + 1) + ".");
                font.DrawText(spriteBatch, 230, Drawy, highscores[j].name);
                font.DrawText(spriteBatch, 550, Drawy, highscores[j].score.ToString());
            }

            spriteBatch.End();

            base.DrawScene(time);
        }
    }
}
