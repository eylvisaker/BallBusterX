using AgateLib;
using AgateLib.Display;
using AgateLib.Input;
using AgateLib.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;

namespace BallBusterX.Scenes
{
    public class TitleScene : Scene
    {
        private readonly GraphicsDevice graphicsDevice;
        private readonly IContentProvider content;
        private readonly CImage img;
        private readonly CSound snd;
        private readonly BBXConfig config;
        private readonly SpriteBatch spriteBatch;
        private readonly Random random;
        private readonly WorldCollection worlds;
        private readonly GameScene attractMode;
        private readonly MouseEvents mouse;
        private readonly Font font;
        private readonly Texture2D whiteTexture;
        private double clock;
        private float time_s;
        private GameState game;
        private double timeSinceMouseMove;
        private int mousex;
        private int mousey;
        private bool hideTitle;
        private int beginningWorld;
        private int beginningLevel;
        private bool beginningChanged;

        public TitleScene(GraphicsDevice graphicsDevice,
                          IContentProvider content,
                          IMouseEventsFactory mouseEventsFactory,
                          CImage img, 
                          CSound snd, 
                          BBXConfig config)
        {
            this.graphicsDevice = graphicsDevice;
            this.content = content;
            this.img = img;
            this.snd = snd;
            this.config = config;
            this.spriteBatch = new SpriteBatch(graphicsDevice);

            this.random = new Random();

            this.worlds = new WorldCollection();
            this.worlds.LoadWorlds(content);

            this.mouse = mouseEventsFactory.CreateMouseEvents();

            mouse.MouseMove += Mouse_MouseMove;
            mouse.MouseUp += Mouse_MouseUp;

            font = new Font(img.Fonts.Default);
            whiteTexture = content.Load<Texture2D>("imgs/white");
        }

        private void Mouse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (hideTitle)
            {
                hideTitle = false;
                timeSinceMouseMove = 0;

                return;
            }
            if (mousex > 100 - 20 && mousex < 500 && mousey > 100 && mousey < 130)
            {
                var gameScene = new GameScene(
                    new GameState(graphicsDevice, img, snd, content, worlds)
                    {
                        world = beginningWorld,
                        level = beginningLevel,
                    });

                SceneStack.Add(gameScene);
            }

            //if (mousex > 100 - 20 && mousex < 500 && mousey > 130 && mousey < 160)
            //{
            //    titlemode = "leveleditor";
            //}
            if (mousex > 100 - 20 && mousex < 500 && mousey > 160 && mousey < 190)
            {
                SceneStack.Remove(this);
            }
            if (mousex > 100 - 20 && mousex < 500 && mousey > 190 && mousey < 220)
            {
                return;
            }
            if (mousex > 100 - 20 && mousex < 500 && mousey > 220 && mousey < 250)
            {
                config.BackgroundScroll = !config.BackgroundScroll;
                game.bgspeed = config.BackgroundScroll ? 50.0f : 0.0f;
            }
            if (mousex > 100 - 20 && mousex < 500 && mousey > 250 && mousey < 280)
            {
                //vsync = !vsync;

                //Display.RenderState.WaitForVerticalBlank = vsync;
            }
            if (mousex > 100 - 20 && mousex < 500 && mousey > 280 && mousey < 320)
            {
                config.PlayMusic = !config.PlayMusic;
            }

        }

        private void Mouse_MouseMove(object sender, MouseEventArgs e)
        {
            timeSinceMouseMove = 0;
            hideTitle = false;

            mousex = e.MousePosition.X;
            mousey = e.MousePosition.Y;
        }

        protected override void OnSceneStart()
        {
            base.OnSceneStart();

            Song song = null;
            if (snd.music.Count > 0)
            {
                song = snd.music[0];

                if (config.PlayMusic)
                    MediaPlayer.Play(song);
            }

            game = new GameState(graphicsDevice, img, snd, content, worlds);

            game.lives = 2;
            game.thescore = 0;
            game.titlemode = " ";

            game.world = random.Next(0, worlds.Count - 1);
            game.level = random.Next(0, worlds[game.world].lvls.Count);

            game.initLevel(true);
            game.attractMode = true;
            game.attractvelocity = 0;

            hideTitle = false;

            beginningWorld = 0;
            beginningLevel = 0;
            beginningChanged = false;
        }

        protected override void OnUpdate(GameTime time)
        {
            base.OnUpdate(time);

            clock += time.ElapsedGameTime.TotalMilliseconds;
            timeSinceMouseMove += time.ElapsedGameTime.TotalMilliseconds;

            mouse.Update(time);

            game.UpdateLevel(time);
            UpdateTitle(time);


            if (timeSinceMouseMove > 15000)
                hideTitle = true;

            // check to see if attract mode died or won.
            if (game.blocks.Count <= game.uncountedBlocks
                || game.balls.Count == 0
                || game.leveltime > 100000
                || beginningChanged)
            {
                if (beginningChanged)
                {
                    game.world = beginningWorld;
                    game.level = beginningLevel;
                }
                else
                {
                    game.world = random.Next(0, worlds.Count - 1);
                    game.level = random.Next(0, worlds[game.world].lvls.Count);
                }


                game.initLevel(true);
                game.attractMode = true;
                game.attractvelocity = 0;

                if (beginningChanged)
                {
                    // update incase we went past the number of levels
                    beginningLevel = game.level;
                    beginningWorld = game.world;
                }

                beginningChanged = false;
            }

            //if (titlemode == "quit")
            //{
            //    //if (session.is_playing()) session.stop();
            //    return "quit";
            //}
            //if (titlemode == "leveleditor")
            //{
            //    level = beginningLevel;
            //    //if (session.is_playing()) session.stop();

            //    world = beginningWorld;
            //    level = beginningLevel;

            //    return "editor";
            //}
            //if (titlemode == "startgame")
            //{
            //    attractMode = false;
            //    lives = 2;
            //    thescore = 0;

            //    world = beginningWorld;
            //    level = beginningLevel;

            //    beginningWorld = 0;
            //    beginningLevel = 0;

            //    //if (session.is_playing()) session.stop();
            //    return "level";
            //}

        }

        private void UpdateTitle(GameTime time)
        {
            var time_s = (float)time.ElapsedGameTime.TotalSeconds;

            if (time_s > 0)
            {
                // update bg scroll for fun... 
                game.bgy += game.bgspeed * time_s;
                if (game.bgy >= game.bgtile.Height)
                    game.bgy -= game.bgtile.Height;
            }
        }

        protected override void DrawScene(GameTime time)
        {
            time_s = (float)time.ElapsedGameTime.TotalSeconds;

            // cap the time interval
            if (time_s > 0.1) time_s = 0.1f;

            //fps = 1/(frametime - lastframetime);
            //System.Diagnostics.Debug.Print("time_s: {0}", time_s);

            //if (session != null && session.IsPlaying && !playmusic) session.Stop();
            //if (session != null && !session.IsPlaying && playmusic) session.Play();
            spriteBatch.Begin();

            game.DrawLevel(spriteBatch, img);
            DrawTitle(spriteBatch, time);

            spriteBatch.End();
        }


        private void DrawTitle(SpriteBatch spriteBatch, GameTime time)
        {
            // Draw perimeter
            if (!hideTitle)
            {
                spriteBatch.Draw(img.topborder, new Vector2(0, 0), Color.White);
                spriteBatch.Draw(img.leftborder, new Vector2(0, 0), Color.White);
                spriteBatch.Draw(img.rightborder, new Vector2(735, 0), Color.White);
                spriteBatch.Draw(img.topborder, new Vector2(0, 590), Color.White);

                FillRect(
                    new Rectangle(0, 0, 800, 600),
                    new Color(0, 0, 0, 100)
                    );
            }

            // hidetitle becomes true after a period of mouse inactivity.
            if (!hideTitle)
            {
                FillRect(
                    new Rectangle(0, 95, 800, 322),
                    new Color(0, 0, 0, 100)
                    );

                font.Size = 20;

                font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 100 && mousey < 130)
                {
                    font.Color = Color.Yellow;
                }
                font.DrawText(spriteBatch, 100, 100, "[START THE GAME]");
                font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 130 && mousey < 160)
                {
                    font.Color = Color.Yellow;
                }
                font.DrawText(spriteBatch, 100, 130, "[LEVEL EDITOR]");
                font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 160 && mousey < 190)
                {
                    font.Color = Color.Yellow;
                }
                font.DrawText(spriteBatch, 100, 160, "[QUIT]");
                font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 190 && mousey < 220)
                {
                    font.Color = Color.Yellow;
                }
                font.DrawText(spriteBatch, 100, 190, "[Full Screen / Windowed]");
                font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 220 && mousey < 250)
                {
                    font.Color = Color.Yellow;
                }
                if (config.BackgroundScroll) font.DrawText(spriteBatch, 100, 220, "[Background Scroll On]");
                if (!config.BackgroundScroll) font.DrawText(spriteBatch, 100, 220, "[Background Scroll Off]");
                font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 250 && mousey < 280)
                {
                    font.Color = Color.Yellow;
                }
                //if (vsync) font.DrawText(100, 250, "[VSync On]");
                //if (!vsync) font.DrawText(100, 250, "[VSync Off]");
                font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 280 && mousey < 310)
                {
                    font.Color = Color.Yellow;
                }
                if (config.PlayMusic) font.DrawText(spriteBatch, 100, 280, "[Play Music]");
                if (!config.PlayMusic) font.DrawText(spriteBatch, 100, 280, "[Do not play Music]");
                font.Color = Color.White;


                // Draw high scores
                font.DrawText(spriteBatch, 500, 100, "High scores");


                for (int i = 0; i < game.highscores.Count; i++)
                {
                    int Drawy = 150 + 15 * i;

                    font.DrawText(spriteBatch, 500, Drawy, (i + 1).ToString() + ". ");
                    font.DrawText(spriteBatch, 530, Drawy, game.highscores[i].name);
                    font.DrawText(spriteBatch, 680, Drawy, (game.highscores[i].score).ToString());

                }

                img.bblogo.Draw(spriteBatch, new Vector2(40, 350));
                img.xlogo.Update(time);
                img.xlogo.Draw(spriteBatch, new Vector2(640, 350));

                font.Size = 14;
                font.Color = Color.White;

                font.DrawText(spriteBatch, 100, 540, "Ball: Buster, by Patrick Avella (C) 2004");
                font.DrawText(spriteBatch, 100, 555, "Ball: Buster eXtreme modifications made by Erik Ylvisaker (C) 2004-8"); 
                font.DrawText(spriteBatch, 100, 570, "Because Breaking Stuff is Fun");

            }
            else
            {
                font.Size = 24;
                if ((int)clock % 100 < 50)
                    font.Color = new Color(255, 120, 120);
                else
                    font.Color = Color.White;

                font.DrawText(spriteBatch, 200, 30, "Move mouse for title screen.");

                font.Color = Color.White;
            }

            font.Size = 14;
            font.DrawText(spriteBatch, 100, 500, $"Starting on level {(beginningWorld + 1)} - {beginningLevel + 1}");


            if (!hideTitle)
            {
                // Draw cursor
                img.arrow.Draw(spriteBatch, new Vector2(mousex, mousey));
            }


        }

        private void FillRect(Rectangle rectangle, Color color)
        {
            spriteBatch.Draw(whiteTexture, rectangle, color);
        }
    }
}
