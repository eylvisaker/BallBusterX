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
using AgateLib.Input;
using AgateLib.Mathematics.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BallBusterX
{
    [Obsolete]
    internal class MainClass
    {
        public static Random random = new Random();
        private const float maxPaddleImbueV = 1000.0f;
        private const float minPaddleImbueV = 200.0f;
        private bool doLighting = true;

        public MainClass()
        {
            gamemode = "title";
            paddlex = 350;
            paddlew = 100;
            paddleh = 20;
            paddley = 560;
            paddlerot = 0.0f;
            paddlealpha = 1.0f;


            ballLimit = 120;
            blockPartLimit = 65;
            scoreByteLimit = 30;


            debugger = false;

            vsync = true;
            bgy = 0.0f;
            paddleImbueV = 500.0f;

            basePaddleImbueVStart = 325;
            basePaddleImbueVEnd = 425;

            ballStickCount = 0;

            verticalhit = false;
            stickypaddle = false;
            catchblue = catchred = false;
            catchbluestart = catchredstart = 0;
            fireball = blaster = stageover = transitionout = died = dying = false;
            deathstart = 0;

            transitionspeed = 0;
            transdelay = 100; transstart = 0;
            transcount = 0;

            playmusic = true;
            bgscroll = true;
            bgspeed = 50.0f;
            level = 1;
            thescore = 0;
            freezeScore = false;
            lives = 2;
            blocksforitem = 7;
            blocksforpoints = 10;
            blocksdestroyed = 0;



            bgtile = null;

            levelChange = 1;

            //-------------------

            mousex = mousey = 0;
            mousedown = false;
        }


        public class EditorState
        {
            public Dictionary<Point, char> Brushes = new Dictionary<Point, char>();
            public Dictionary<Point, string> Menu = new Dictionary<Point, string>();

            public char brush;
            public string editormode;
            public Rectangle toiletRect;

        }

        private EditorState editorState = new EditorState();
        //private IStopwatch mPauseTimer = Timing.CreateStopWatch();

        // the variables here are REALLY sloppy, sorry......... :P
        private string gamemode;
        private bool attractMode;
        private int attractvelocity;
        private bool hideTitle;
        private int mLastMouseMove;
        private int lastMouseMove
        {
            get { return mLastMouseMove; }
            set
            {
                System.Diagnostics.Debug.WriteLine("Last Mouse Move: " + Timing.TotalMilliseconds);
                mLastMouseMove = value;
            }
        }
        private int levelChange;

        //private float fps;
        private float time_s;

        private int beginningWorld;
        private int beginningLevel;
        private bool beginningChanged;
        private int powerupLeft = 0;
        private int powerupTop = 0;

        private bool stickypaddle;
        private bool supersticky;
        private bool catchblue, catchred;
        private bool pow, smash;
        private bool fireball, blaster, stageover, transitionout, died, dying;
        private bool bgscroll, playmusic;

        private int ballStickCount;

        private int catchbluestart, catchredstart;
        private int blasterstart;
        private int fireballstart;
        private int superstickystart;
        private int powstart;
        private int smashstart;

        private Texture2D bgtile;
        private string bgFile;

        private bool paused;

        private bool vsync;
        private Keys hitkey;
        private string hitkeystring;

        private int transitionspeed, transdelay;
        private int transstart, deathstart;
        private int lives;
        private int blocksforitem;    // these are for the system of dropping items....
        private int blocksforpoints;
        private int blocksdestroyed;
        private int transcount;

        private int uncountedBlocks;

        //ulong int score;
        private int thescore;
        private bool freezeScore;

        private int world;
        private int level;

        private int powerupcount;
        private int ballslost;

        private int ballLimit;
        private int blockPartLimit;
        private int scoreByteLimit;

        private bool verticalhit;
        private bool debugger;

        private float bgy, bgspeed;

        private float blockscrolly;
        private float blockscrollspeedy;

        private CImage img = new CImage();
        private CSound snd = new CSound();

        private float paddlex, paddley, paddlew, paddleh, paddlerot, paddlealpha;
        private float paddleVelocity;   // velocity that the paddle is moving.
        private float _paddleImbueV;        // velocity the paddle gives to the balls
        private float paddleImbueV
        {
            get { return _paddleImbueV; }
            set
            {
                _paddleImbueV = value;

                if (_paddleImbueV < minPaddleImbueV)
                    _paddleImbueV = minPaddleImbueV;
            }
        }

        private float basePaddleImbueV; // ordinary velocity the paddle gives
        private float basePaddleImbueVStart, basePaddleImbueVEnd;

        public struct lastPowerup
        {
            public Sprite pu;
            public int time;
        }

        private List<lastPowerup> lastPowerups = new List<lastPowerup>();
        private List<CBlock> blocks = new List<CBlock>();
        private List<CBlockPart> blockparts = new List<CBlockPart>();
        private List<CFlash> flashes = new List<CFlash>();
        private List<CBall> balls = new List<CBall>();
        private List<CFadeBall> fadeBalls = new List<CFadeBall>();
        private List<CPowerUp> powerups = new List<CPowerUp>();
        private List<CScoreByte> scoreBytes = new List<CScoreByte>();

        //List<CEnemy>			enemies;

        private List<CWorld> worlds = new List<CWorld>();
        private List<Highscore> highscores = new List<Highscore>();






        //-----------------------

        private int mousex, mousey;
        private bool mousedown;

        //=======================
        private string titlemode;
        private KeyboardEvents keyboard;
        private MouseEvents mouse;
        private GraphicsDevice device;
        private SpriteBatch spriteBatch;

        //----------------------




        public int Initialize(IContentProvider content, GraphicsDevice device)
        {
#if DEBUG
            if (Debugger.IsAttached)
                debugger = true;
#endif

            if (debugger)
            {
                playmusic = false;
            }

            this.device = device;
            this.spriteBatch = new SpriteBatch(device);

            // load the images, initiation frame rate counter, and register signals
            splash(content);

            img.load(content);
            snd.load(content);

            keyboard = new KeyboardEvents();
            keyboard.KeyDown += Keyboard_KeyDown;

            mouse = new MouseEvents();
            mouse.MouseMove += Mouse_MouseMoveEvent;
            mouse.MouseUp += Mouse_MouseUpEvent;
            mouse.MouseDown += Mouse_MouseDownEvent;

            loadHighscores();
            loadWorlds();


            //here we go!
            run();

            saveHighscores();

            freeWorlds();
            return 0;
        }

        public void Update(GameTime time)
        {

        }

        public void Draw(GameTime time)
        {

        }

        private void Keyboard_KeyDown(object sender, KeyEventArgs e)
        {
            hitkey = e.Key;
            hitkeystring = e.KeyString;

            //Keys modifiers = e.Modifiers & (Keys.LShiftKey | Keys.RShiftKey | Keys.ShiftKey);

            // check to see if we should change the starting level
            if (beginningLevel >= 0)
            {
                if (hitkey == Keys.Up)
                {
                    beginningLevel++;
                    beginningChanged = true;

                    // don't go past the last level
                    if (beginningWorld == worlds.Count - 1)
                    {
                        if (beginningLevel >= worlds[beginningWorld].lvls.Count)
                        {
                            beginningLevel--;
                            beginningChanged = false;
                        }

                    }

                    levelChange = 1;
                }
                else if (hitkey == Keys.Down)
                {
                    beginningLevel--;
                    beginningChanged = true;

                    levelChange = -1;

                    // don't go before the first level
                    if (beginningLevel < 0 && beginningWorld > 0)
                    {
                        beginningWorld--;

                        beginningLevel = worlds[beginningWorld].lvls.Count - 1;


                    }
                    else if (beginningLevel < 0 && beginningWorld == 0)
                    {
                        beginningLevel = 0;
                        beginningChanged = false;
                    }

                }

            }

            if ((hitkey == Keys.P || hitkey == Keys.Pause) && !transitionout && !attractMode)
            {
                paused = !paused;

                //if (paused)
                //{
                //    Timing.PauseAllTimers();
                //    mPauseTimer.Resume();
                //}
                //else
                //    Timing.ResumeAllTimers();

            }
        }

        private void Mouse_MouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            if (gamemode == "level")
            {
                for (int i = 0; i < balls.Count; i++)
                {
                    if (balls[i].ballsticking)
                    {
                        balls[i].ballsticking = false;
                        //balls[i].bally -= blockscrolly;	// correct for paddle moving up
                        ballStickCount--;
                    }

                }
                if (blaster && !dying)
                {
                    addBall((int)(paddlew * 0.5f + random.Next(-4, 2)));
                }
                return;
            }
            if (gamemode == "editor")
            {
                mousedown = true;
                handleEditorClicks();
            }

            hideTitle = false;
            lastMouseMove = (int)Timing.TotalMilliseconds;
        }

        private void Mouse_MouseUpEvent(object sender, MouseButtonEventArgs e)
        {
            if (gamemode == "level")
            {
                return;
            }

            if (gamemode == "editor")
            {
                mousedown = false;
                handleEditorClicks();

                foreach (KeyValuePair<Point, string> kvp in editorState.Menu)
                {
                    Rectangle rect = new Rectangle(kvp.Key, img.font.MeasureString(kvp.Value));

                    if (rect.Height > 16)
                        rect.Height = 16;

                    if (rect.Contains(mousex + 20, mousey))
                    {
                        EditorMenu(kvp.Value);
                        break;

                    }
                }

            }
            if (gamemode == "title")
            {
                if (hideTitle)
                {
                    hideTitle = false;
                    lastMouseMove = (int)Timing.TotalMilliseconds;

                    return;
                }
                //		mousedown= false; //handleEditorClicks();
                if (mousex > 100 - 20 && mousex < 500 && mousey > 100 && mousey < 130)
                {
                    titlemode = "startgame";
                }
                if (mousex > 100 - 20 && mousex < 500 && mousey > 130 && mousey < 160)
                {
                    titlemode = "leveleditor";
                }
                if (mousex > 100 - 20 && mousex < 500 && mousey > 160 && mousey < 190)
                {
                    titlemode = "quit";
                }
                if (mousex > 100 - 20 && mousex < 500 && mousey > 190 && mousey < 220)
                {
                    return;
                }
                if (mousex > 100 - 20 && mousex < 500 && mousey > 220 && mousey < 250)
                {
                    bgscroll = !bgscroll;
                    bgspeed = bgscroll ? 50.0f : 0.0f;
                }
                if (mousex > 100 - 20 && mousex < 500 && mousey > 250 && mousey < 280)
                {
                    vsync = !vsync;

                    Display.RenderState.WaitForVerticalBlank = vsync;
                }
                if (mousex > 100 - 20 && mousex < 500 && mousey > 280 && mousey < 320)
                {
                    playmusic = !playmusic;
                }

            }
        }

        private void EditorMenu(string p)
        {
            string x = p.ToLower();

            if (x.Contains("title")) editorState.editormode = "titlescreen";
            else if (x.Contains("new")) editorState.editormode = "new";
            else if (x.Contains("save")) saveEditor();
            else if (x.Contains("up"))
            {
                level++;
                editorState.editormode = "reload";
            }
            else if (x.Contains("down"))
            {
                level--;
                editorState.editormode = "reload";
            }
            else
                throw new Exception("Bug here.");

        }

        private void Mouse_MouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (mousex == e.MousePosition.X && mousey == e.MousePosition.Y)
                return;

            lastMouseMove = (int)Timing.TotalMilliseconds;
            hideTitle = false;

            mousex = e.MousePosition.X;
            mousey = e.MousePosition.Y;

            if (!attractMode && !paused)
            {
                paddleVelocity = (mousex - (paddlex + paddlew / 2)) / time_s;

                paddlex = mousex - paddlew / 2;
                paddleCheckEdge();

                mousex = (int)(paddlex + paddlew / 2);
                mousey = 400;
            }
        }

        private void run()
        {
            for (; ; )
            {

                if (gamemode == "level")
                    gamemode = runLevel();

                else if (gamemode == "title")
                    gamemode = runTitle();

                else if (gamemode == "editor")
                    gamemode = runEditor();


                else if (gamemode == "quit")
                    return;

            }

        }

        private void loadLevel(bool resetPowerups)
        {
            string file;
            CWorld w;

        // point to try to reload window.
        retry:
            if (level < 0 && world > 0)
            {
                world--;
                level = worlds[world].lvls.Count - 1;
            }
            else if (level < 0)
                level = 0;

            w = worlds[world];

            if (level >= w.lvls.Count)
            {
                world++;
                level = 0;

                if (world >= worlds.Count)
                    world = 0;

                w = worlds[world];

                resetPowerups = true;
            }

            if (Display.Caps.IsHardwareAccelerated && doLighting)
                AgateBuiltInShaders.Lighting2D.AmbientLight = w.light;

            file = "lvls/" + worlds[world].lvls[level] + ".lvl";

            StreamReader myfile;
            try
            {
                myfile = new StreamReader(Assets.OpenRead(file));
            }
            catch
            {
                level += levelChange;

                goto retry;
            }

            string mystr = "__________________";
            int j = 0;


            clearBlocks();

            for (j = 0; j < 24; j++)
            {
                mystr = myfile.ReadLine();

                for (int i = 0; i < 17 && i < mystr.Length; i++)
                {
                    if (mystr[i] != '_')
                    {
                        addBlock((60 + (40 * i)), 10 + (20 * j), mystr[i]);
                    }
                }

                if (myfile.EndOfStream)
                    break;

            }

            myfile.Dispose();


            if (blocks.Count == 0)
            {
                level += levelChange;

                goto retry;
            }

            if (bgFile != worlds[world].background)
            {
                bgFile = worlds[world].background;

                bgtile = content.Load<Texture2D>(worlds[world].background);
            }

        }

        private void DrawBackground(float yval)
        {
            int cols = (int)Math.Ceiling(Display.RenderTarget.Width / (float)bgtile.DisplayWidth);
            int rows = (int)Math.Ceiling(Display.RenderTarget.Height / (float)bgtile.DisplayHeight);

            while (yval > bgtile.DisplayHeight)
                yval -= bgtile.DisplayHeight;

            for (int j = -1; j < rows; j++)
            {
                for (int i = 0; i < cols; i++)
                {
                    bgtile.Draw((int)(i * bgtile.DisplayWidth), (int)(yval + j * bgtile.DisplayHeight));
                }
            }


            //bgtile.Draw(0, -bgtile.DisplayHeight + yval);
            //bgtile.Draw(bgtile.DisplayWidth, -bgtile.DisplayHeight + yval);
            //bgtile.Draw(0, 0 + yval);
            //bgtile.Draw(bgtile.DisplayWidth, 0 + yval);
            //bgtile.Draw(0, bgtile.DisplayHeight + yval);
            //bgtile.Draw(bgtile.DisplayWidth, bgtile.DisplayHeight + yval);

        }

        private void DrawLevel()
        {
            DB_EnterSubsection("Draw");

            Display.Clear(Color.FromArgb(128, 0, 0, 128));
            SetLightingForLevel();

            // Draw the background tile, scrolled
            DrawBackground(bgy);

            // Draw perimeter
            img.topborder.Draw(0, 0);
            img.leftborder.Draw(0, 0);
            img.rightborder.Draw(735, 0);

            ActivateLighting();

            // Draw blocks and Update their animations...
            DrawBlocks();

            // we Draw the flash right on top of the block
            DrawFlashes();


            // Draw all the other stuff except the balls here
            DrawBlockParts();
            DrawFadeBalls();

            if (doLighting)
            {
                //AgateBuiltInShaders.Basic2DShader.Activate();
            }

            // Draw paddle, other stuff, and lastly the balls.
            DB_EnterSubsection("Drawing Paddle");

            img.paddle.Update();
            img.smallpaddle.Update();
            img.largepaddle.Update();

            Sprite pad;

            if (paddlew == 50)
            {
                pad = img.smallpaddle;
            }
            else if (paddlew == 100)
            {
                pad = img.paddle;
            }
            else
            {
                pad = img.largepaddle;
            }

            pad.SetScale(paddlealpha, paddlealpha);
            pad.Alpha = paddlealpha;
            pad.RotationCenter = OriginAlignment.Center;
            pad.RotationAngleDegrees = paddlerot;
            pad.Draw((int)paddlex, (int)paddley);

            DB_ExitSubsection();

            DrawPowerUps();
            DrawScoreBytes();

            // Draw the balls
            DB_EnterSubsection("Drawing balls");

            Sprite ballimg;

            for (int j = 0; j < balls.Count; j++)
            {
                CBall myball = balls[j];

                if (myball.fireball)
                    ballimg = img.fireball;
                else
                    ballimg = img.ball;

                //ballimg.set_color(myball.color);

                int spikes = myball.spikes;
                int ballx = (int)myball.ballx;
                int bally = (int)myball.bally;


                if (spikes != 0)
                {
                    int angle = 360 / spikes;
                    int x, y;

                    x = ballx - 3;
                    y = bally - 3;

                    img.spike.DisplayAlignment = OriginAlignment.TopLeft;
                    img.spike.RotationCenter = OriginAlignment.Center;

                    for (int k = 0; k < spikes; k++)
                    {
                        img.spike.RotationAngleDegrees = myball.Ballangle + angle * k;
                        img.spike.Draw(x, y);
                    }
                }

                ballimg.Draw(ballx, bally);

                if (myball.smash)
                {
                    float offset = myball.ballw / 2 - img.smash.SpriteWidth / 2;

                    img.smash.RotationAngleDegrees = myball.SmashAngle;
                    img.smash.Draw(ballx + offset, bally + offset);
                }
            }

            DB_ExitSubsection();



            DB_EnterSubsection("Drawing info");

            // Draw whatever text

            string message = "Score: " + getScore().ToString();

            img.font.Size = 14;
            img.font.Color = Color.Black;
            img.font.DrawText(11, 585, message);
            img.font.Color = Color.White;
            img.font.DrawText(13, 583, message);

            // output debugging messages, if the debugger is attached
            //if (debugger)
            //{
            //    img.font.DrawText(13, 505, "basePaddleImbueV: " + ((int)basePaddleImbueV));
            //    img.font.DrawText(13, 520, "paddleImbueV: " + ((int)paddleImbueV));
            //    img.font.DrawText(13, 550, "Balls Lost: " + (ballslost));
            //    img.font.DrawText(13, 565, "BallStickCount: " + (ballStickCount));
            //}

            img.font.DrawText(60, 10, string.Format("FPS: {0:###.#}", Math.Round(Display.FramesPerSecond, 1)));

            DB_ExitSubsection();

            // Draw extra lives
            img.ball.RotationAngle = 0;

            int ilives;
            for (ilives = 0; ilives < lives; ilives++)
            {
                img.ball.Draw(775 - (ilives * 13), 588);
            }

            // Draw powerups that are in effect:
            DrawPowerupsInEffect();


            // fade the screen to white if we are transitioning out
            if (transitionout)
            {
                Rectangle myrect = new Rectangle(0, 0, 800, 600);
                Color mycolor = Color.FromArgb(transcount, 255, 255, 255);

                Display.FillRect(myrect, mycolor);

            }

            // display paused message
            if (paused)
            {
                Rectangle myrect = new Rectangle(0, 0, 800, 600);
                Color mycolor = Color.FromArgb(100, 0, 0, 0);

                Display.FillRect(myrect, mycolor);

                if (mPauseTimer.TotalMilliseconds % 1000 < 500)
                {
                    img.font.Size = 30;
                    img.font.Color = Color.White;
                    img.font.DisplayAlignment = OriginAlignment.Center;

                    img.font.DrawText(400, 200, "PAUSED");
                    img.font.DrawText(400, 300, "Press 'P' to unpause.");

                    img.font.DisplayAlignment = OriginAlignment.TopLeft;

                    img.font.Size = 14;

                }

                if (!attractMode)
                    img.arrow.Draw(mousex, mousey);
            }

            DB_ExitSubsection();
        }

        private void ActivateLighting()
        {
            if (Display.Caps.IsHardwareAccelerated == false)
                return;

            if (doLighting)
            {
                var shader = AgateBuiltInShaders.Lighting2D;

                shader.Activate();
            }
        }

        private void SetLightingForLevel()
        {
            if (Display.Caps.IsHardwareAccelerated == false)
                return;

            var shader = AgateBuiltInShaders.Lighting2D;

            while (shader.Lights.Count > balls.Count)
                shader.Lights.RemoveAt(shader.Lights.Count - 1);

            for (int i = 0; i < balls.Count; i++)
            {
                Light light;

                if (i < shader.Lights.Count)
                    light = shader.Lights[i];
                else
                {
                    light = new Light();
                    shader.Lights.Add(light);
                }

                if (balls[i].fireball)
                {
                    light.Position = new Vector3(balls[i].ballx, balls[i].bally, -1);
                    light.DiffuseColor = Color.FromArgb(255, 255, 0);
                    light.AmbientColor = Color.FromArgb(64, 32, 0);

                    light.AttenuationConstant = 0.01f;
                    light.AttenuationLinear = 0.005f;
                    light.AttenuationQuadratic = 0.000001f;

                }
                else
                {
                    light.Position = new Vector3(balls[i].ballx, balls[i].bally, -1);
                    light.DiffuseColor = Color.FromArgb(200, 200, 200);
                    light.AmbientColor = Color.Black;

                    light.AttenuationConstant = 0.01f;
                    light.AttenuationLinear = 0;
                    light.AttenuationQuadratic = 0.00001f;
                }
            }
        }

        private void EndFrame()
        {
            Display.EndFrame();

            // if we're paused, drop the framerate a little bit so we don't
            // monopolize the processor.
            if (paused)
            {
                System.Threading.Thread.Sleep(5);
            }

            Core.KeepAlive();

            if (Core.IsActive == false)
                System.Threading.Thread.Sleep(25);

            //while (Core.IsActive == false && Display.CurrentWindow.Closed == false)
            //{
            //    System.Threading.Thread.Sleep(25);
            //    Core.KeepAlive();
            //}

        }

        private void DrawPowerupsInEffect()
        {
            if (lastPowerups.Count > 0)
            {
                for (int i = 0; i < lastPowerups.Count; i++)
                {
                    lastPowerup lpu = lastPowerups[i];

                    if ((lpu.time - (int)Timing.TotalMilliseconds) / 1000.0f + 5 < 0)
                        lastPowerups.RemoveAt(i);
                }

                if (lastPowerups.Count > 0)
                {
                    img.font.DrawText(755, 10, "Last:");

                }

                powerupLeft = 755;
                powerupTop = 10 + img.font.FontHeight;

                for (int i = 0; i < lastPowerups.Count; i++)
                {
                    lastPowerup lpu = lastPowerups[i];

                    lpu.pu.SetScale(1.0f, 1.0f);
                    lpu.pu.Alpha = 1.0f;
                    lpu.pu.Draw(powerupLeft, powerupTop);

                    powerupTop += 45;
                }

            }

            powerupLeft = 755;
            powerupTop = 545;

            if (paddleImbueV > basePaddleImbueV + 25)
                DrawPowerupInEffect(img.pufastball);
            else if (paddleImbueV < basePaddleImbueV - 25)
                DrawPowerupInEffect(img.puslowball);
            else
                DrawPowerupInEffect(img.puregularspeed);

            if (paddlew > 110)
                DrawPowerupInEffect(img.pupaddlelarge);
            else if (paddlew < 90)
                DrawPowerupInEffect(img.pupaddlesmall);
            else
                DrawPowerupInEffect(img.pupaddleregular);

            if (supersticky)
            {
                int seconds = DrawPowerupInEffect(img.pusupersticky, superstickystart);

                if (seconds < 0)
                    supersticky = false;

            }
            else if (stickypaddle)
                DrawPowerupInEffect(img.pusticky);

            if (blaster)
            {
                int seconds = DrawPowerupInEffect(img.publaster, blasterstart);

                if (seconds < 0)
                    blaster = false;

                img.font.Size = 24;
                // Draw flashing CLICK! above paddle
                if ((int)Timing.TotalMilliseconds % 200 < 100)
                    img.font.Color = Color.Red;
                else
                    img.font.Color = Color.White;

                img.font.DisplayAlignment = OriginAlignment.TopCenter;


                img.font.DrawText(400 + (int)Timing.TotalMilliseconds % 3 - 2,
                                (int)(paddley) - 100 + (int)Timing.TotalMilliseconds % 3 - 2, "Click!");

                img.font.DisplayAlignment = OriginAlignment.TopLeft;

                img.font.Color = Color.White;

            }

            if (fireball)
            {
                int seconds = DrawPowerupInEffect(img.pufireball, fireballstart);

                if (seconds < 0)
                    fireball = false;
            }

            if (catchblue)
            {
                int seconds = DrawPowerupInEffect(img.pucatchblue, catchbluestart);

                if (seconds < 0)
                    catchblue = false;

            }
            if (catchred)
            {
                int seconds = DrawPowerupInEffect(img.pucatchred, catchredstart);


                if (seconds < 0)
                {
                    // well, se used up the whole thing.  gain another 250 points.
                    catchred = false;

                    gainPoints(250, powerupLeft + 20, powerupTop + 20);
                    snd.powerup.Play();
                }
            }

            if (pow)
            {
                int seconds = DrawPowerupInEffect(img.pupow, powstart);

                if (seconds < 0)
                    pow = false;
            }

            if (smash)
            {
                int seconds = DrawPowerupInEffect(img.pusmash, smashstart);

                if (seconds < 0)
                    smash = false;
            }
        }

        private int DrawPowerupInEffect(Sprite sprite)
        {
            return DrawPowerupInEffect(sprite, -1);
        }

        private int DrawPowerupInEffect(Sprite image, int start)
        {
            int milliseconds = start - (int)Timing.TotalMilliseconds;
            int seconds = milliseconds / 1000;


            string time;
            int minutes = seconds / 60;
            seconds %= 60;

            time = (minutes) + ":";
            if (seconds < 10)
                time += "0";

            time += (seconds);

            image.Alpha = (1.0f);
            image.SetScale(1.0f, 1.0f);
            image.Draw(powerupLeft, powerupTop);

            if (start > -1)
            {
                Size size = img.font.MeasureString(time);
                int fw = size.Width;
                int pos = powerupLeft + (40 - fw) / 2;

                img.font.Color = Color.Black;
                img.font.DrawText(spriteBatch, pos, powerupTop + 2, time);
                img.font.DrawText(spriteBatch, pos, powerupTop + 4, time);
                img.font.DrawText(spriteBatch, pos + 1, powerupTop + 3, time);
                img.font.DrawText(spriteBatch, pos - 1, powerupTop + 3, time);

                img.font.Color = Color.White;
                img.font.DrawText(spriteBatch, pos, powerupTop + 3, time);
            }

            powerupTop -= 45;

            return milliseconds;

        }

        private void initLevel(bool resetPowerups)
        {

            stageover = false;
            transitionout = false;
            died = false;
            dying = false;

            transcount = 0;
            paddley = 560;
            paddlerot = 0.0f;
            paddlealpha = 1.0f;
            bgspeed = bgscroll ? 50.0f : 0.0f;

            blockscrolly = 0;
            blockscrollspeedy = 0;

            paddleImbueV = basePaddleImbueV = basePaddleImbueVStart;

            paused = false;

            loadLevel(resetPowerups);

            lastPowerups.Clear();

            //deleteAllEnemies();

            if (resetPowerups)
            {
                deleteAllBalls();
                deleteAllFadeBalls();

                freezeScore = true;
                powerUp(PowerupTypes.RESET);
                freezeScore = false;

                ballStickCount = 0;

                addBall();

            }
            else
            {
                CBall myball;

                for (int i = 0; i < balls.Count; i++)
                {
                    myball = balls[i];

                    if (!myball.ballsticking)
                    {
                        //delete balls[i];
                        balls.RemoveAt(i);

                        i--;
                    }
                    else
                    {
                        myball.ballstickstart = (int)Timing.TotalMilliseconds;
                    }

                }

                // reposition the balls so they make a nice circular wavefront when
                // they are released.  We want them to be centered, basically.
                int num = balls.Count;
                for (int i = 0; i < num; i++)
                {
                    myball = balls[i];

                    myball.stickydifference = (i + 1) / (float)(num + 1) * paddlew;
                }

                if (balls.Count == 0)
                    addBall();
            }

            deleteAllPowerUps();
            deleteAllBlockParts();
            deleteAllScoreBytes();
            deleteAllFlashes();


            powerupcount = 0;
            ballslost = 0;

            //dropEnemy();

        }

        private Song playRandomSong()
        {
            if (snd.music.Count == 0)
                return null;

            int randsong = random.Next(snd.music.Count);
            Song session;

            session = snd.music[randsong];

            session.IsLooping = true;

            if (playmusic)
                session.Play();

            return session;

        }

        private string runLevel()
        {

            bool gameover = false;      // ain't over till the fat lady sings.

            levelChange = 1;

            // starting the game... initialize the level and make sure
            // to reset any powerups caught in attract mode.
            initLevel(true);

            Audio.StopAll();

            int frames = 0;

            while (!gameover)
            {
                initLevel(false);

                Song music = playRandomSong();

                int start = (int)Timing.TotalMilliseconds;
                int end = start;

                int lastframetime = (int)Timing.TotalMilliseconds;
                int frametime = (int)Timing.TotalMilliseconds;

                int lowblocktime = start;

                while (!stageover)
                {
                    lastframetime = frametime;
                    frametime = (int)Timing.TotalMilliseconds;

                    if (frametime - lastframetime > 50)
                    {
                        DB_Output();
                        System.Diagnostics.Debug.Print("Warning... frame time is: {0}\n", frametime - lastframetime);
                    }

                    if (frames % 100 == 0)
                        System.Diagnostics.Debug.Print("Frame time is: {0}\n", frametime - lastframetime);

                    DB_DefineNewFrameDescriptor();

                    if (paused && music != null && music.IsPlaying)
                        music.Stop();
                    else if (!paused && music != null && music.IsPlaying == false && playmusic && !transitionout)
                        music.Play();


                    time_s = (float)Display.DeltaTime / 1000.0f;

                    if (time_s > 0.050f) time_s = 0.050f;

                    if (!paused)
                    {

                        updateLevel();

                    }

                    Display.BeginFrame();

                    DrawLevel();

                    EndFrame();
                    frames++;

                    if (Core.IsAlive == false)
                        return "quit";

                    DB_EnterSubsection("End of level checks");

                    // see if we should drop a door powerup
                    if (blocks.Count <= uncountedBlocks + 5 && !transitionout && !stageover)
                    {
                        if (lowblocktime == start)
                            lowblocktime = (int)Timing.TotalMilliseconds;

                        if (Timing.TotalMilliseconds - lowblocktime > 20000)
                        {
                            dropPowerUp(400, 10, PowerupTypes.DOOR);
                            lowblocktime += 15000;
                        }
                    }

                    // see if the level is over...
                    if (blocks.Count <= uncountedBlocks && !transitionout && !stageover)
                    {
                        transitionout = true;

                        snd.speedup.Play();
                    }
                    if (transitionout)
                    {
                        if (music != null && music.IsPlaying) music.Stop();

                        if (end == start)
                            end = (int)Timing.TotalMilliseconds;
                    }

                    // Check for D-key self-destruct
                    if (Input.Unhandled.Keys[Keys.D])
                    {
                        while (balls.Count > 0)
                            deleteBall(0);
                    }

                    // see if the player is dead. If so, take life and give ball, else, game over.
                    if (balls.Count == 0 && !transitionout)
                    {
                        //System.Diagnostics.Debug.WriteLine("Timing.TotalMilliseconds: " + Timing.TotalMilliseconds.ToString());

                        if (paddlealpha == 1.0f)
                        {
                            snd.die.Play();
                            dying = true;
                            paddley -= 100 * time_s;


                        }
                        if (20 + deathstart < (int)Timing.TotalMilliseconds)
                        {
                            paddlealpha -= 1.0f * time_s;
                            paddlerot += 1440.0f * time_s;
                            paddley -= 100 * time_s;
                            deathstart = (int)Timing.TotalMilliseconds;
                        }
                        if (paddlealpha < 0.0f)
                        {
                            dying = false;
                            died = false;


                            freezeScore = true;
                            powerUp(PowerupTypes.RESET);
                            powerupcount--;
                            freezeScore = false;

                            paddleImbueV = basePaddleImbueV = basePaddleImbueVStart;

                            if (lives == 0) { stageover = true; died = true; }
                            if (lives > 0)
                            {
                                addBall();
                                if (!transitionout) lives--;
                                paddley = 560;
                                paddlerot = 0.0f;
                                paddlealpha = 1.0f;
                            }
                        }
                    }


                    // if escape is pressed, the boss is coming!

                    if (Input.Unhandled.Keys[Keys.Escape] || Input.Unhandled.Keys[Keys.End])
                    {
                        if (music != null && music.IsPlaying) music.Stop();
                        return "title";
                    }
                    DB_ExitSubsection();


                    DB_EndFrame();

                    // end stageover loop......
                }


                transitionout = false;
                bgspeed = bgscroll ? 50.0f : 0.0f;

                stageover = false;

                if (music != null && music.IsPlaying) music.Stop();
                if (!died)
                {
                    endLevel(end - start);

                }
                else
                {
                    gameover = true;
                }



                // end gameover loop......
            }

            endgame();

            return "title";

        }

        private void DB_EndFrame()
        {

        }

        private void DB_DefineNewFrameDescriptor()
        {

        }

        private void DB_Output()
        {

        }

        private void endgame()
        {
            bool addhighscore = false;
            int addposition = 0;


            // test to see if we have a new high score
            for (int i = 0; i < highscores.Count; i++)
            {
                if (getScore() > highscores[i].score)
                {
                    addhighscore = true;
                    addposition = i;

                    break;
                }
            }

            // also add the high score if the list has less than 10 positions.
            if (highscores.Count < 10 && addhighscore == false)
            {
                addhighscore = true;
                addposition = -1;
            }

            if (getScore() == 0)
                addhighscore = false;

            getkeypress();
            GetKeyChar();

            if (addhighscore)
            {
                // new high score!
                Highscore ns = new Highscore();

                ns.score = getScore();

                if (addposition > -1)
                    highscores.Insert(addposition, ns);
                else
                {
                    addposition = highscores.Count;
                    highscores.Add(ns);
                }

                bool hitenter = false;
                int start = (int)Timing.TotalMilliseconds;

                while (highscores.Count > 10)
                {
                    highscores.RemoveAt(10);
                }

                while (!hitenter)
                {
                    int time = (int)Timing.TotalMilliseconds - start;

                    Display.BeginFrame();
                    Display.Clear(Color.White);

                    img.font.Size = 24;
                    img.font.Color = Color.Black;
                    img.font.DrawText(200, 50, "New High Score!");

                    for (int j = 0; j < highscores.Count; j++)
                    {
                        int Drawy = 100 + 30 * j;

                        img.font.Color = Color.Black;

                        if (addposition == j)
                        {
                            img.font.Color = Color.FromHsv((time % 3600) / 10.0f, 0.7f, 0.7f);
                        }

                        img.font.DrawText(200, Drawy, (j + 1) + ".");
                        img.font.DrawText(230, Drawy, highscores[j].name);
                        img.font.DrawText(550, Drawy, highscores[j].score.ToString());
                    }

                    Keys key = getkeypress();
                    //string keyst = GetKeyChar();

                    if (key != Keys.None)
                    {
                        // Is Keys.Back the same as the backspace key?
                        if (key == Keys.Back && ns.name.Length > 0)
                        {
                            ns.name = ns.name.Substring(0, ns.name.Length - 1);
                        }
                        else if (key == Keys.Enter && ns.name.Length > 0)
                        {
                            hitenter = true;
                        }
                        else if (ns.name.Length < 12)
                        {
                            //ns.name += keyst;
                            if (key >= Keys.A && key <= Keys.Z ||
                                key >= Keys.D0 && key <= Keys.D9)
                            {

                                // if the shift key wasn't pressed, make it lowercase.
                                if (!(mods & 0x01))
                                    keys += ('a' - 'A');

                            }
                            else if (key == Keys.Space)
                            {
                                ns.name += ' ';
                            }
                        }

                    }


                    Display.EndFrame();
                    Core.KeepAlive();

                    if (Core.IsActive == false)
                        System.Threading.Thread.Sleep(25);

                }


                saveHighscores();

            }
        }

        private void endLevel(int levelTime)
        {
            int start = (int)Timing.TotalMilliseconds;
            int end = start + 10000;
            int median;
            float countRate = 1.5f;  //r means the numbers count slower
            int bonus = 100;        // basic level bonus for completing a level
            int extraBonus = 0;
            int timeBonus;
            int newscore;
            int time = levelTime / 1000;
            int minutes, seconds;
            int lineCount;
            int lineLimit;
            int tab = 400;
            int totalBonus;

            string timeString;

            seconds = time % 60;
            minutes = time / 60;

            timeString = (minutes) + ":";

            if (seconds < 10) timeString += "0";
            timeString += (seconds);



            timeBonus = ((180 - time) * 200) / 180;
            if (timeBonus < 0) timeBonus = 0;

            extraBonus = 0;

            if (ballslost == 0)
                extraBonus += 500;
            if (powerupcount == 0)
                extraBonus += 1500;

            totalBonus = bonus + timeBonus + extraBonus;
            newscore = getScore() + totalBonus;

            start = (int)Timing.TotalMilliseconds;
            end = start + 10000;

            while ((int)Timing.TotalMilliseconds < end && Core.IsAlive)
            {
                Display.BeginFrame();

                img.font.Size = 19;
                median = (int)Timing.TotalMilliseconds - start;

                Display.Clear(Color.White);


                lineCount = 0;
                lineLimit = 0;

                const int xpt = 175;

                img.font.Color = Color.Black;
                img.font.DrawText(xpt, 50, string.Format("Completed level {0}-{1}", world + 1, level + 1));

                if (median > 1000)
                {
                    img.font.DrawText(xpt, 100, "Score:");
                    img.font.DrawText(tab, 100, getScore().ToString());
                }

                if (median > 1500)
                {
                    img.font.DrawText(xpt, 150, "Level Bonus:");
                    img.font.DrawText(tab, 150, ((int)Math.Min(bonus, (median - 1500) / countRate)).ToString());
                }

                if (median > 2000)
                {
                    img.font.DrawText(xpt, 200, "Time: ");
                    img.font.DrawText(tab, 200, timeString);
                }

                if (median > 2500)
                {
                    img.font.DrawText(xpt, 230, "Time Bonus: ");
                    img.font.DrawText(tab, 230, ((int)Math.Min(timeBonus, (median - 2500) / countRate)).ToString());
                }

                if (median > 3000)
                {
                    lineLimit = (median - 3000) / 500 + 1;
                }




                if (ballslost == 0 && lineCount < lineLimit)
                {
                    img.font.DrawText(xpt, 280 + lineCount * 50, "No balls lost");
                    img.font.DrawText(tab, 280 + lineCount * 50, "500");

                    lineCount++;
                }

                /// show other bonuses.
                if (powerupcount == 0 && lineCount < lineLimit)
                {
                    img.font.DrawText(xpt, 280 + lineCount * 50, "No powerups");
                    img.font.DrawText(tab, 280 + lineCount * 50, "1500");

                    lineCount++;
                }

                /// display the final score
                if (lineCount < lineLimit)
                {
                    int medianstart = 3000 + (lineCount) * 500;

                    img.font.DrawText(xpt, 280 + lineCount * 50, "Final Score:");
                    img.font.DrawText(tab, 280 + lineCount * 50,
                        ((int)Math.Min(newscore, getScore() + (median - medianstart) / countRate)).ToString());

                    lineCount++;
                }


                Display.EndFrame();
                Core.KeepAlive();

                if (Core.IsActive == false)
                    System.Threading.Thread.Sleep(25);

            }


            gainPoints(totalBonus);

            // clear out time-based powerups
            blaster = false;
            catchred = false;
            catchblue = false;
            supersticky = false;

            pow = false;
            smash = false;
            fireball = false;


            level++;

            GC.Collect();
        }

        private bool paddleCheckEdge()
        {
            if (paddlex < 60) { paddlex = 60; return true; }
            if (paddlex > 740 - paddlew) { paddlex = 740 - paddlew; return true; }

            return false;
        }

        // function for STL sort to sort balls by y.
        private int ballcheck(CBall a, CBall b)
        {
            return a.bally.CompareTo(b.bally);
        }

        private void updateLevel()
        {

            DB_EnterSubsection("updateLevel()");

            // reset paddle velocity so that is mousemove event doesn't fire, then
            // the paddle is taken to not be moving.
            //if (!attractMode)
            //    paddleVelocity = 0;

            if (basePaddleImbueV < basePaddleImbueVEnd)
            {
                // want this to go up by about 1 per second.
                basePaddleImbueV += 1.0f * time_s;
            }

            // update the paddle imbue speed
            float diff = basePaddleImbueV - paddleImbueV;

            if (diff > 0)
                paddleImbueV += (float)Math.Min(1.5, diff / 2) * time_s;
            else if (diff < 0)
                paddleImbueV += (float)Math.Max(-1.5, diff / 2) * time_s;


            const int startSortingAt = 10;

            // sort for collisions
            if (balls.Count > startSortingAt)
            {
                balls.Sort(ballcheck);
            }

            int j;
            int lowestball = -1;
            bool badBall = false;
            bool chasepowerup = false;

            CBall myball;

            DB_EnterSubsection("main ball testing loop");

            for (j = 0; j < balls.Count; j++)
            {
                badBall = false;

                myball = balls[j];
                myball.update(time_s);


                if (myball.fireball)// && !myball.ballsticking)
                {
                    if (myball.lastfade + 10 < (int)Timing.TotalMilliseconds)
                    {
                        myball.lastfade = (int)Timing.TotalMilliseconds;

                        dropFadeBall(myball);
                    }
                }


                // doing collision checks for bricks first to determine where the ball hit it.
                if (myball.ballsticking == false)
                {

                    int i;

                    myball.bally += myball.ballvy * time_s;

                    myball.ballv = paddleImbueV;
                    myball.checkVelocity();

                    i = 0;
                    for (i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].collision(myball.ballx, myball.bally, myball.ballw, myball.ballh))
                        {
                            if (myball.fireball == false || (
                                blocks[i].mBlockType != CBlock.BlockType.Glass &&
                                blocks[i].mBlockType != CBlock.BlockType.Wood))
                            {
                                myball.ballvy = -myball.ballvy;

                                // position the ball outside the block
                                if (myball.ballvy < 0)
                                    myball.bally = blocks[i].gety(false) - myball.ballw - 1;
                                else
                                    myball.bally = blocks[i].gety(false) + blocks[i].Height + 1;

                            }
                            verticalhit = true;

                            if (!transitionout)
                                hitBlock(i, myball);

                            i = blocks.Count;
                        }
                    }

                    myball.ballx += myball.ballvx * time_s;

                    // now the x axis
                    for (i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].collision(myball.ballx, myball.bally, myball.ballw, myball.ballh))
                        {
                            if (!myball.fireball || (
                                blocks[i].mBlockType != CBlock.BlockType.Glass &&
                                blocks[i].mBlockType != CBlock.BlockType.Wood))
                            {
                                myball.ballvx = -myball.ballvx;

                                // position the ball outside the block
                                if (myball.ballvx < 0)
                                    myball.ballx = blocks[i].getx(false) - myball.ballw - 1;
                                else
                                    myball.ballx = blocks[i].getx(false) + blocks[i].Width + 1;

                            }

                            verticalhit = false;

                            if (!transitionout)
                                hitBlock(i, myball);

                            i = blocks.Count;
                        }
                    }

                }

                // check for ball collisions
                if (balls.Count <= startSortingAt)
                {
                    for (int k = 0; k < balls.Count; k++)
                    {
                        if (k != j)
                        {
                            if (myball.collideWith(balls[k]) && !transitionout)
                            {
                                snd.bounce.Play();
                            }
                        }
                    }
                }
                else
                {
                    // lots of balls, so restrict the search to balls that are within a ballwidth
                    // of the current ball
                    for (int k = 0; k < balls.Count; k++)
                    {
                        // find balls that are close
                        if (balls[k].bally < myball.bally - myball.ballw)
                            continue;

                        if (k != j)
                        {
                            if (myball.collideWith(balls[k]))
                            {
                                if (!transitionout)
                                    snd.bounce.Play();
                            }
                        }

                        if (balls[k].bally > myball.bally + myball.ballw)
                            break;
                    }
                }
                /*
			// check for collisions with enemies
			for (int i = 0; i < enemies.Count; i++)
			{
				if (enemies[i].collision(myball))
				{
					float angle = random.Next(0, 360) * (float)(Math.PI / 180);

					myball.ballvx = (float)Math.Cos(angle) * myball.ballv;
					myball.ballvy = (float)Math.Sin(angle) * myball.ballv;
				}
			}
				*/

                // check the perimeter
                if (myball.ballx < 60)
                {
                    myball.ballvx = -myball.ballvx;
                    myball.ballx = 60;
                }
                if (myball.ballx + myball.ballw > 740)
                {
                    myball.ballvx = -myball.ballvx;
                    myball.ballx = 740 - myball.ballw;
                }
                if (myball.bally < 10 && !myball.ballsticking)
                {
                    if (myball.ballvy < 0) myball.ballvy = -myball.ballvy;

                    myball.bally = 10;
                }
                if (myball.bally > 600)
                {
                    badBall = true;

                    if (!transitionout)
                        snd.ballfall.Play();
                }

                // check for attract mode
                // don't care about bad balls, and we don't care about balls that are sticking
                // so the paddle should worry about balls that are falling, but if there are none, than just
                // balls that are low
                if (attractMode)
                {
                    if (myball.bally < paddley + paddleh && !badBall && !myball.ballsticking &&
                        (myball.ballvy > 0 && myball.bally > 300))
                    {
                        if (lowestball == -1)
                            lowestball = j;
                        else if ((600 - myball.bally) / myball.ballvy <
                            (600 - balls[lowestball].bally) / balls[lowestball].ballvy
                            && myball.ballvy > 0)
                            lowestball = j;
                    }
                }

                // check for ball acceleration, if super magnet is in use
                if (supersticky)
                {

                    if (myball.ballvy > 0 || myball.bally > paddley)
                    {
                        // exponentially decaying attraction.  Not realistic, but hopefully it looks nice
                        float distx = (paddlex + paddlew / 2 - (myball.ballx + myball.ballw / 2));
                        float disty = (paddley - myball.bally);

                        float dist = (float)Math.Sqrt(distx * distx + disty * disty);

                        distx /= dist;
                        disty /= dist;

                        // multiply by a normalized ball velocity, so the magnet has the
                        // same effect on slow balls as fast balls.
                        dist = 0.8f * dist * (float)Math.Exp(-dist / 100) * myball.ballv / 400;


                        myball.ballvx += distx * dist;
                        myball.ballvy += disty * dist;

                        myball.checkVelocity();

                    }
                }


                // check the paddle
                bool hit_paddle = true;

                if (myball.ballx + myball.ballw < paddlex) hit_paddle = false;
                else if (myball.ballx > paddlex + paddlew) hit_paddle = false;
                else if (myball.bally + myball.ballh < paddley) hit_paddle = false;
                else if (myball.bally > paddley + paddleh) hit_paddle = false;

                if (hit_paddle)
                {
                    // the difference between the centers divided by paddle length * 2 gives number between -1 and 1... kinda
                    float mydif = ((myball.ballx + (myball.ballw / 2)) - (paddlex + (paddlew / 2)));
                    bool changeVY = false;

                    // set the rotational velocity of the ball, so the surface of the ball
                    // has the tangential velocity proportionate to that of the paddle.
                    // negative sign because paddle is under the ball.
                    myball.Ballvrot = -0.1f * 2 * paddleVelocity / myball.ballw * 180 / (float)(Math.PI);

                    if (myball.ballvy > 0)
                        changeVY = true;

                    if (mydif != 0.0f) mydif /= paddlew;
                    mydif *= 2.0f;

                    if (mydif > 0.97f) mydif = 0.97f;
                    else if (mydif < -0.97f) mydif = -0.97f;

                    // get the radian from asin();
                    mydif = (float)Math.Asin(mydif);

                    // apply new velocity....
                    myball.ballvy = -paddleImbueV * (float)Math.Cos(mydif);
                    myball.ballvx = paddleImbueV * (float)Math.Sin(mydif);
                    myball.ballv = paddleImbueV;





                    if (changeVY && !myball.ballsticking)
                    {
                        // check for powerups
                        if (fireball)
                        {
                            myball.fireball = true;
                        }

                        if (pow)
                        {
                            myball.setDamage(myball.damage + CBall.powIncrement);
                        }

                        if (smash)
                        {
                            myball.smash = true;
                        }
                    }


                    // if you have the stick power up.....
                    if ((stickypaddle && myball.ballsticking == false) ||
                        (transitionout && ballStickCount == 0))
                    {
                        myball.ballsticking = true;
                        ballStickCount++;

                        myball.stickydifference = myball.ballx - paddlex;
                        myball.ballstickstart = (int)Timing.TotalMilliseconds;
                    }


                }

                // if the ball is sticking, make sure to update it's position and 
                // check to see if it should automagically be released.
                if (myball.ballsticking)
                {

                    myball.ballx = paddlex + myball.stickydifference;
                    myball.bally = paddley - myball.ballh;

                    if (myball.ballstickstart + 4000 < (int)Timing.TotalMilliseconds && !transitionout)
                    {
                        myball.ballsticking = false;
                        ballStickCount--;

                    }

                    if (attractMode && powerups.Count == 0)
                    {
                        if (random.NextDouble() < 0.1)
                        {
                            myball.ballsticking = false;
                            ballStickCount--;
                        }

                    }
                }



                if (badBall)
                {
                    deleteBall(j);

                    if (!transitionout)
                    {
                        ballslost++;
                    }

                }

            }

            DB_ExitSubsection();

            // update everything else
            updateBlockParts();
            updateFlashes(time);
            updatePowerUps();
            updateScoreBytes();
            updateFadeBalls();
            //updateEnemies();


            // update bg scroll for fun... tied in with the transitioning 'system' ....
            if (transitionout)
            {
                paddley -= 100 * time_s;

                bgspeed += 100 * time_s;
                blockscrollspeedy += 40 * time_s;

                transcount = (int)(255 * (560 - paddley) / 560.0f);


                if (transcount >= 255)
                {

                    stageover = true;
                    paddley = 560;
                    snd.ching.Play();

                    transcount = 255;
                }
            }

            bgy += bgspeed * time_s;
            blockscrolly += blockscrollspeedy * time_s;


            /// update for attract mode
            // first check to see if we want to chase a powerup instead
            if (attractMode && lowestball == -1)
            {
                chasepowerup = true;

                for (int i = 0; i < powerups.Count; i++)
                {
                    if (lowestball == -1)
                        lowestball = i;
                    else if (powerups[i].y > powerups[lowestball].y)
                        lowestball = i;

                }

            }

            if (attractMode && lowestball > -1 && !chasepowerup)
            {
                // the idea is to create a computer player that plays like a human.  
                // the goal:  keep the paddle under the ball.
                // force the computer to accelerate the paddle instead of just using
                // something lame like paddlex = myball.ballx - 10
                diff = (int)(balls[lowestball].ballx - (paddlex + paddlew / 2));
                int vdiff = (int)(balls[lowestball].ballvx - attractvelocity);
                int sign = 0;

                // check to see if the ball is left or right of the paddle
                if (diff < -paddlew / 3) sign = -1;
                if (diff > paddlew / 3) sign = 1;

                // if the ball is outside the paddle region, we want to accelerate towards it.
                // the further the ball is the faster we want to accelerate
                attractvelocity += (int)(sign * 4 * (Math.Abs(diff) + 10));

                // otherwise, if the ball is above the paddle, but there's a large discrepancy
                // between the velocities of the ball and the paddle, we should accelerate 
                // the paddle in the direction the ball is going.
                // but make sure that we don't slow down if they are going towards each other
                /*if (balls[lowestball].ballvx * attractvelocity < 0 && 
					Math.Abs((int)(diff + vdiff * .01) < Math.Abs((int)(diff))) && sign == 0) // && Math.Abs(vdiff) > 15*/
                if (balls[lowestball].ballvx * attractvelocity > 0)
                {
                    if (vdiff > 0)
                        attractvelocity += Math.Max(vdiff, 10);
                    else
                        attractvelocity += Math.Min(vdiff, -10);
                }

                if (vdiff * attractvelocity > 0 && Math.Abs((int)(diff)) < paddlew)
                {
                    if (vdiff > 0)
                        attractvelocity += Math.Max(vdiff, 10);
                    else
                        attractvelocity += Math.Min(vdiff, -10);
                }

            }
            else if (attractMode && lowestball > -1 && chasepowerup)
            {
                diff = (int)((powerups[lowestball].x + powerups[lowestball].w / 2) - (paddlex + paddlew / 2));
                int sign = 0;

                // check to see if the ball is left or right of the paddle
                if (diff < -paddlew / 3) sign = -1;
                if (diff > paddlew / 3) sign = 1;

                // if the powerup is outside the paddle region, we want to accelerate towards it.
                // the further the powerup is the faster we want to accelerate
                if (Math.Abs(attractvelocity) < Math.Abs(diff) * 3 || attractvelocity * diff < 0)
                    attractvelocity += (int)(sign * (Math.Abs(diff) + 10));

                if (sign == 0)
                    attractvelocity = (int)(attractvelocity * 0.9f);


            }
            else if (attractMode && lowestball == -1)
            {
                attractvelocity = (int)(attractvelocity * 0.93f);
            }

            if (attractMode)
            {
                if (attractvelocity > 1300)
                    attractvelocity = 1300;
                else if (attractvelocity < -1300)
                    attractvelocity = -1300;

                // update the paddle position
                paddlex += attractvelocity * time_s;


                // pay attention to the borders
                if (paddleCheckEdge())
                    attractvelocity = 0;


                // hey, if we've got the blaster, use it.
                if (blaster && random.NextDouble() < 0.05)
                {
                    addBall();
                }

                paddleVelocity = attractvelocity;
            }


            DB_ExitSubsection();
        }

        private void DB_ExitSubsection()
        {

        }

        private void DB_EnterSubsection(string p)
        {

        }

        private CBlock.BlockType getBlockData(char type, out Sprite spr, out Color clr, out int str)
        {
            CBlock.BlockType retval = CBlock.BlockType.Glass;

            clr = Color.White;
            str = 100;
            spr = img.block;

            if (type == 'r')                                // red glass
            {
                clr = Color.FromArgb(255, 255, 0, 0);
            }
            else if (type == 'o')                           // orange glass
            {
                clr = Color.FromArgb(255, 255, 155, 0);
            }
            else if (type == 'y')                           // yellow glass
            {
                clr = Color.FromArgb(255, 255, 255, 0);
            }
            else if (type == 'g')                           // green glass
            {
                clr = Color.FromArgb(255, 0, 255, 0);
            }
            else if (type == 't')                           // turquoise glass
            {
                clr = Color.FromArgb(255, 0, 255, 204);
            }
            else if (type == 'b')                           // blue glass
            {
                clr = Color.FromArgb(255, 0, 0, 255);
            }
            else if (type == 'v')                           // violet glass
            {
                clr = Color.FromArgb(255, 128, 0, 200);
            }
            else if (type == 'w')                           // white glass
            {
                clr = Color.FromArgb(255, 255, 255, 255);

            }
            else if (type == 'p')                           // pine block -- wood
            {
                str = 250;
                spr = img.woodblock;


                retval = CBlock.BlockType.Wood;
            }
            else if (type == '1')                           // turqoise ruby
            {
                str = 1000;
                spr = img.rubyblock1;

                retval = CBlock.BlockType.Ruby;
            }
            else if (type == '2')                           // red ruby
            {
                str = 1500;
                spr = img.rubyblock2;

                retval = CBlock.BlockType.Ruby;
            }
            else if (type == '3')                           // blue ruby
            {
                str = 1500;
                spr = img.rubyblock3;

                retval = CBlock.BlockType.Ruby;
            }
            else if (type == 'c')                           // cobblestone
            {
                str = 200;
                spr = img.cblock;

                retval = CBlock.BlockType.Stone;

            }
            else if (type == 'd')                           // blue marble
            {
                str = 250;
                spr = img.marbleblock1;

                retval = CBlock.BlockType.Stone;

            }
            else if (type == 'e')                           // white marble
            {
                str = 300;
                spr = img.marbleblock2;

                retval = CBlock.BlockType.Stone;
            }
            else if (type == 's')                           // steel (invincible)
            {

                str = 10000;
                spr = img.sblock;


                retval = CBlock.BlockType.Invincible;


            }
            else
            {
                System.Diagnostics.Debug.Print("Bad block type: '{0}'", type);

                return CBlock.BlockType.Invalid;
            }

            return retval;

        }

        private void addBlock(int myx, int myy, char color, GameTime time)
        {
            CBlock pBlock = new CBlock(time);
            pBlock.color = color;
            pBlock.setCoords(myx, myy);


            pBlock.offsety = blockscrolly;


            int str;

            pBlock.mBlockType = getBlockData(color, out pBlock.block, out pBlock.clr, out str);
            pBlock.setStr(str);

            if (pBlock.mBlockType == CBlock.BlockType.Invalid)
            {
                // delete pBlock;
                return;
            }

            pBlock.animShift = myx + myy;

            if ((myx / 40) % 2 == 1)
                pBlock.flipcrack = true;


            blocks.Add(pBlock);

            if (pBlock.mBlockType == CBlock.BlockType.Invincible ||
                pBlock.mBlockType == CBlock.BlockType.Ruby)
                uncountedBlocks++;


        }

        private void clearBlocks()
        {
            blocks.Clear();

            uncountedBlocks = 0;
        }

        private void DrawBlocks()
        {
            DB_EnterSubsection("DrawBlocks");

            if (blocks.Count > 0)
            {
                int i;
                int sz = blocks.Count;
                float crack;
                float w = (float)img.crack.DisplayWidth;
                float cwidth;

                for (i = 0; i < sz; i++)
                {
                    CBlock myblock = blocks[i];

                    myblock.setFrame(time);

                    myblock.block.Color = myblock.clr;
                    myblock.block.Draw((int)myblock.getx(), (int)myblock.gety());

                    crack = myblock.crackPercentage();

                    if (myblock.shaking)
                    {
                        if ((int)Timing.TotalMilliseconds - myblock.shakeStart > 300)
                        {
                            myblock.shaking = false;
                        }

                    }

                    if (crack > 0.001)
                    {
                        cwidth = w * crack / 2;
                        float vscale = 1.0f * (myblock.flipcrack ? -1 : 1);
                        //int vshift = (vscale < 0) ? 20 : 0;


                        img.crack.Alpha = (crack);
                        img.crack.SetScale(crack / 2, vscale);

                        img.crack.Draw((int)(myblock.getx()/* + cwidth*/), (int)myblock.gety());

                        img.crack.SetScale(crack / 2, vscale);
                        img.crack.Draw((int)myblock.getx() + (int)(40 * (1 - crack / 2)), (int)myblock.gety());


                    }
                }

            }

            DB_ExitSubsection();
        }

        private void DrawBlock(int x, int y, char type)
        {
            Sprite spr;
            Color clr;
            int str;

            if (getBlockData(type, out spr, out clr, out str) != CBlock.BlockType.Invalid)
            {
                spr.Color = clr;
                spr.Draw(x, y);
            }
        }

        private void hitBlock(int myblock, CBall myball)
        {
            int blockx, blocky;

            blockx = (int)blocks[myblock].getx();
            blocky = (int)blocks[myblock].gety();


            // decrement the strength of the block
            blocks[myblock].decreaseStr(myball.damage);

            // if we have the fireball and the block is glass or wood, bust it up!
            if (myball.fireball && (
                blocks[myblock].mBlockType == CBlock.BlockType.Glass ||
                blocks[myblock].mBlockType == CBlock.BlockType.Wood))
            {
                blocks[myblock].decreaseStr(blocks[myblock].getStr());
            }

            checkBlock(myblock, myball);

            // if we have the smash powerup, we need to do splash damage
            if (myball.smash)
            {
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (i == myblock)
                        continue;

                    CBlock other = blocks[i];

                    float distance =
                        (float)Math.Abs((other.getx() - blockx) / 40.0) +
                        (float)Math.Abs((other.gety() - blocky) / 20.0);

                    if (distance > 1.1)
                        continue;

                    distance = distance / 19;

                    int damage = myball.damage - (int)(distance) * 30;

                    if (damage > 0)
                    {
                        other.decreaseStr(damage);
                        checkBlock(i, myball, false);
                    }

                }
            }
        }

        private void checkBlock(int myblock, CBall myball)
        {
            checkBlock(myblock, myball, true);
        }

        private void checkBlock(int myblock, CBall myball, bool playSound, GameTime time)
        {
            int scoreGain = 0;
            int blockx, blocky;

            blockx = (int)blocks[myblock].getx();
            blocky = (int)blocks[myblock].gety();

            if (blocks[myblock].getStr() <= 0)
            {
                dropBlockParts(blocks[myblock].getx(), blocks[myblock].gety(), blocks[myblock].block,
                    blocks[myblock].clr, myball.ballvx, myball.ballvy);

                if (playSound)
                    snd.shatter.Play();

                blocksdestroyed++;

                if (blaster)
                    scoreGain = 1;
                else
                {
                    scoreGain = 11 - balls.Count;

                    if (myball.fireball)
                        scoreGain -= 3;

                    if (paddlew > 110)
                        scoreGain -= 2;
                    else if (paddlew < 90)
                        scoreGain += 2;

                    //if (paddleImbueV > 510) scoreGain += 2;
                    //if (paddleImbueV < 500) scoreGain -= 2;

                }

                if (scoreGain < 1) scoreGain = 1;

                // if we destroy an invicible block, make sure the count is decremented so we don't exit
                // when there are other blocks left alive.
                if (blocks[myblock].mBlockType == CBlock.BlockType.Invincible)
                {
                    uncountedBlocks--;
                    scoreGain = 0;
                }
                if (blocks[myblock].mBlockType == CBlock.BlockType.Ruby)
                {
                    uncountedBlocks--;
                    scoreGain = 100;
                }

                if (blocksdestroyed % blocksforitem == 0)
                {
                    dropPowerUp((int)blocks[myblock].getx(), (int)blocks[myblock].gety(), false);
                }
                else if (blocksdestroyed % blocksforpoints == 0)
                {
                    dropPowerUp((int)blocks[myblock].getx(), (int)blocks[myblock].gety(), true);
                }

                blocks.RemoveAt(myblock);
            }
            else
            {

                // shake the block, if we have the smash powerup
                if (myball.smash)
                {
                    blocks[myblock].shake(time);
                }

                // check to see if we get points for hitting a ruby block
                if (blocks[myblock].mBlockType == CBlock.BlockType.Ruby)
                {
                    scoreGain = 20;
                }
                // check to see that the block is one we deserve points for
                else if (blocks[myblock].mBlockType != CBlock.BlockType.Invincible)
                {
                    scoreGain = 2;

                    // adjust the amount of points gained based on the various
                    // powerups in effect.
                    //if (paddleImbueV > 510) scoreGain += 1;
                    //if (paddleImbueV < 490) scoreGain -= 1;
                    if (paddlew > 110) scoreGain -= 1;
                    if (paddlew < 90) scoreGain += 1;
                    if (blaster) scoreGain = 0;

                    if (scoreGain < 0) scoreGain = 0;
                }

                // if the block didn't get destroyed, light it up :) and play sound
                dropFlash(blocks[myblock].getx(), blocks[myblock].gety(), time);

                if (playSound)
                    snd.bounce.Play();

            }

            // woot!
            gainPoints(scoreGain, blockx + 20, blocky + 10);

        }

        private void dropBlockParts(float myx, float myy, Sprite myblock, Color clr, float ballvx, float ballvy)
        {

            if (blockparts.Count > blockPartLimit)
                return;

            if ((ballvx < 0) && verticalhit)
            {
                CBlockPart pblock0 = new CBlockPart((ballvx + ballvx * 0.5f), -ballvy, myx, myy, myblock, clr);
                CBlockPart pblock1 = new CBlockPart(ballvx, -ballvy, myx + 20, myy, myblock, clr);
                CBlockPart pblock2 = new CBlockPart((ballvx + ballvx * 0.5f), -ballvy + (-ballvy * 0.2f), myx, myy + 10, myblock, clr);
                CBlockPart pblock3 = new CBlockPart(ballvx, -ballvy + (-ballvy * 0.2f), myx + 20, myy + 10, myblock, clr);
                blockparts.Add(pblock0);
                blockparts.Add(pblock1);
                blockparts.Add(pblock2);
                blockparts.Add(pblock3);
                return;
            }

            if ((ballvx >= 0) && verticalhit)
            {
                CBlockPart pblock0 = new CBlockPart((ballvx + ballvx * 0.5f), -ballvy, myx + 20, myy, myblock, clr);
                CBlockPart pblock1 = new CBlockPart(ballvx, -ballvy, myx, myy, myblock, clr);
                CBlockPart pblock2 = new CBlockPart((ballvx + -ballvx * 0.5f), -ballvy + (-ballvy * 0.2f), myx + 20, myy + 10, myblock, clr);
                CBlockPart pblock3 = new CBlockPart(ballvx, -ballvy + (-ballvy * 0.2f), myx, myy + 10, myblock, clr);
                blockparts.Add(pblock0);
                blockparts.Add(pblock1);
                blockparts.Add(pblock2);
                blockparts.Add(pblock3);
                return;
            }

            if ((ballvx < 0) && !verticalhit)
            {
                CBlockPart pblock0 = new CBlockPart((-ballvx + -ballvx * 0.5f), ballvy, myx, myy, myblock, clr);
                CBlockPart pblock1 = new CBlockPart(-ballvx, ballvy, myx + 20, myy, myblock, clr);
                CBlockPart pblock2 = new CBlockPart((-ballvx + -ballvx * 0.5f), ballvy + (ballvy * 0.2f), myx, myy + 10, myblock, clr);
                CBlockPart pblock3 = new CBlockPart(-ballvx, ballvy + (ballvy * 0.2f), myx + 20, myy + 10, myblock, clr);
                blockparts.Add(pblock0);
                blockparts.Add(pblock1);
                blockparts.Add(pblock2);
                blockparts.Add(pblock3);
                return;
            }

            if ((ballvx >= 0) && !verticalhit)
            {
                CBlockPart pblock0 = new CBlockPart((-ballvx + -ballvx * 0.5f), ballvy, myx + 20, myy, myblock, clr);
                CBlockPart pblock1 = new CBlockPart(-ballvx, ballvy, myx, myy, myblock, clr);
                CBlockPart pblock2 = new CBlockPart((-ballvx + -ballvx * 0.5f), ballvy + (ballvy * 0.2f), myx + 20, myy + 10, myblock, clr);
                CBlockPart pblock3 = new CBlockPart(-ballvx, ballvy + (ballvy * 0.2f), myx, myy + 10, myblock, clr);
                blockparts.Add(pblock0);
                blockparts.Add(pblock1);
                blockparts.Add(pblock2);
                blockparts.Add(pblock3);
                return;
            }


        }

        private void updateBlockParts()
        {
            DB_EnterSubsection("updateBlockParts");

            if (blockparts.Count > 0)
            {
                int i;
                for (i = 0; i < blockparts.Count; i++)
                {
                    CBlockPart b = blockparts[i];
                    if (!b.update(time_s))
                    {
                        // delete it if the alpha is gone....
                        deleteBlockPart(i);
                        i--;
                    }
                    else
                        blockparts[i] = b;

                }

            }

            DB_ExitSubsection();
        }

        private void deleteBlockPart(int myblockpart)
        {
            // delete blockparts[myblockpart];
            blockparts.RemoveAt(myblockpart);
        }

        private void deleteAllBlockParts()
        {
            while (blockparts.Count > 0)
                deleteBlockPart(0);
        }

        private void DrawBlockParts()
        {
            DB_EnterSubsection("DrawBlockParts");

            if (blockparts.Count > 0)
            {
                int i;
                for (i = 0; i < blockparts.Count; i++)
                {
                    CBlockPart part = blockparts[i];

                    part.block.SetScale(0.5f, 0.5f);
                    part.block.RotationAngleDegrees = part.rotation;

                    part.block.Color = part.mClr;
                    part.block.Alpha = (part.alpha);

                    part.block.Draw((int)part.x, (int)part.y);

                    part.block.RotationAngleDegrees = 0;
                    part.block.SetScale(1.0f, 1.0f);

                }

            }

            DB_ExitSubsection();

        }

        private void dropFlash(float myx, float myy, GameTime time)
        {
            CFlash pFlash = new CFlash((int)myx, (int)myy, time);
            flashes.Add(pFlash);
        }

        private void updateFlashes(GameTime time)
        {
            DB_EnterSubsection("updateFlashes");

            if (flashes.Count > 0)
            {
                int i;
                for (i = 0; i < flashes.Count; i++)
                {
                    if (!flashes[i].update(time))
                    {
                        // if there's no alpha, delete it
                        deleteFlash(i);
                        i--;

                    }

                }

            }

            DB_ExitSubsection();
        }

        private void deleteFlash(int myflash)
        {
            // delete flashes[myflash];
            flashes.RemoveAt(myflash);
        }

        private void deleteAllFlashes()
        {
            while (flashes.Count > 0)
                deleteFlash(0);
        }

        private void DrawFlashes()
        {
            DB_EnterSubsection("DrawFlashes");

            if (flashes.Count > 0)
            {
                int i;
                for (i = 0; i < flashes.Count; i++)
                {
                    img.flash.Alpha = (flashes[i].alpha);
                    img.flash.Draw((int)flashes[i].x, (int)flashes[i].y);

                }

            }

            DB_ExitSubsection();
        }

        private CBall addBall()
        {
            return addBall((int)(paddlew / 2));
        }

        private CBall addBall(CBall otherball)
        {
            if (balls.Count >= ballLimit)
                return null;

            CBall myBall = new CBall(otherball);

            balls.Add(myBall);

            return myBall;

        }

        private CBall addBall(int offset)
        {
            if (balls.Count >= ballLimit)
                return null;

            CBall myball = new CBall();

            myball.stickydifference = offset;
            myball.bally = paddley - myball.ballw;
            myball.ballx = paddlex + offset;

            myball.fireball = fireball;
            myball.smash = smash;

            if (pow)
                myball.setDamage(myball.damage + CBall.powIncrement);

            ballStickCount++;


            balls.Add(myball);

            return myball;

        }

        private void deleteBall(int myball)
        {
            if (balls[myball].ballsticking)
                ballStickCount--;

            balls.RemoveAt(myball);

        }

        private void deleteAllBalls()
        {

            while (balls.Count > 0)
                deleteBall(0);

        }

        private void recordPowerup(CPowerUp powerup)
        {
            if (powerup.icon == null)
                return;

            if (lastPowerups.Count >= 3)
            {
                //lastPowerups.erase(lastPowerups.begin(), lastPowerups.begin() + 1);
                lastPowerups.RemoveAt(0);
            }

            lastPowerup pu;

            pu.pu = powerup.icon;
            pu.time = (int)Timing.TotalMilliseconds;

            lastPowerups.Add(pu);

        }

        private void powerUp(PowerupTypes effect, GameTime time)
        {
            CPowerUp dummy = new CPowerUp(0, 0, time);

            dummy.setEffect(effect);
            dummy.icon = null;

            powerUp(dummy);
        }

        private void powerUp(CPowerUp powerup)
        {
            powerUp(powerup, 0);
        }

        private void powerUp(CPowerUp powerup, int extraPoints)
        {
            int scoreGain = extraPoints;
            PowerupTypes effect = powerup.getEffect();

            powerupcount++;

            if (effect != PowerupTypes.RANDOM)
                recordPowerup(powerup);

            switch (effect)
            {
                case PowerupTypes.PTS100:
                    scoreGain = 100;
                    powerupcount--;

                    break;

                case PowerupTypes.PTS250:
                    scoreGain = 250;
                    powerupcount--;

                    break;

                case PowerupTypes.PTS500:
                    scoreGain = 500;
                    powerupcount--;

                    break;

                case PowerupTypes.PTS1000:
                    scoreGain = 1000;
                    powerupcount--;

                    break;

                case PowerupTypes.ONEUP:
                    lives++;
                    scoreGain = 50;

                    break;

                case PowerupTypes.STICKY:

                    scoreGain = 50;
                    stickypaddle = true;

                    break;

                case PowerupTypes.SUPERSTICKY:

                    scoreGain = 50;
                    supersticky = true;

                    superstickystart = (int)Timing.TotalMilliseconds + 30000;

                    break;

                case PowerupTypes.MULTIBALL:

                    addBall((int)(paddlew * 0.5f) + 0);
                    addBall((int)(paddlew * 0.5f) + 10);
                    addBall((int)(paddlew * 0.5f) - 10);
                    addBall((int)(paddlew * 0.5f) - 20);
                    scoreGain = 50;

                    break;

                case PowerupTypes.PU3BALL:
                {
                    int end = balls.Count;


                    for (int i = 0; i < end; i++)
                    {
                        CBall myball = balls[i];
                        CBall ball1, ball2;

                        if (myball.ballsticking)
                            continue;

                        ball1 = addBall(myball);

                        if (ball1 == null)
                            break;

                        ball2 = addBall(myball);

                        if (ball2 == null)
                            break;




                        // come up with a unit vector that is perpendicular to the ball's velocity
                        float angle = (float)Math.Atan2(myball.ballvy, myball.ballvx);
                        float perpx, perpy;

                        perpx = (float)Math.Cos(angle + (float)(Math.PI / 2));
                        perpy = (float)Math.Sin(angle + (float)(Math.PI / 2));

                        // put the balls side by side.
                        ball1.ballx = myball.ballx + myball.ballw * 1.01f * perpx;
                        ball1.bally = myball.bally + myball.ballw * 1.01f * perpy;

                        ball2.ballx = myball.ballx - myball.ballw * 1.01f * perpx;
                        ball2.bally = myball.bally - myball.ballw * 1.01f * perpy;

                        // set the velocities
                        float vel = (float)Math.Pow(myball.ballvx, 2) + (float)Math.Pow(myball.ballvy, 2);
                        float boostAngle = (float)(Math.PI / 12);
                        vel = (float)Math.Sqrt(vel);

                        ball1.ballvx = vel * (float)Math.Cos(angle + boostAngle);
                        ball1.ballvy = vel * (float)Math.Sin(angle + boostAngle);

                        ball2.ballvx = vel * (float)Math.Cos(angle - boostAngle);
                        ball2.ballvy = vel * (float)Math.Sin(angle - boostAngle);

                        ball1.ballsticking = false;
                        ball2.ballsticking = false;

                    }
                }
                scoreGain = 50;

                break;

                case PowerupTypes.RBSWAP:

                    if (paddleImbueV != basePaddleImbueV)
                    {
                        paddleImbueV = basePaddleImbueV - (paddleImbueV - basePaddleImbueV);
                    }

                    if (paddlew < 90)
                        setPaddleSize(150);
                    else if (paddlew > 110)
                        setPaddleSize(50);

                    BBUtility.SWAP(ref catchblue, ref catchred);
                    BBUtility.SWAP(ref catchbluestart, ref catchredstart);

                    scoreGain = 50;

                    break;

                case PowerupTypes.BLASTER:

                    blaster = true;
                    blasterstart = (int)Timing.TotalMilliseconds + 10000;

                    scoreGain = 50;

                    addBall();

                    break;

                case PowerupTypes.FIREBALL:

                    fireball = true;
                    fireballstart = (int)Timing.TotalMilliseconds + 10000;

                    scoreGain = 50;
                    {
                        for (int i = 0; i < balls.Count; i++)
                        {
                            if (balls[i].ballsticking)
                                balls[i].fireball = true;
                        }
                    }
                    break;

                case PowerupTypes.FASTBALL:

                    scoreGain = 50;

                    if (balls.Count > 0)
                    {
                        paddleImbueV += 100.0f;

                        if (paddleImbueV > maxPaddleImbueV)
                            paddleImbueV = maxPaddleImbueV;

                        /*int i;
						for (i= 0; i < balls.Count; i++) {
							float angle= atan2(balls[i].ballvy, balls[i].ballvx);
							balls[i].ballvx= maxv * cos(angle);
							balls[i].ballvy= maxv * sin(angle);
							}*/
                    }


                    break;

                case PowerupTypes.SLOWBALL:

                    scoreGain = 50;
                    if (balls.Count > 0)
                    {
                        paddleImbueV -= 100.0f;

                        /*int i;
						for (i= 0; i < balls.Count; i++) {
							float angle= atan2(balls[i].ballvy, balls[i].ballvx);
							balls[i].ballvx= maxv * cos(angle);
							balls[i].ballvy= maxv * sin(angle);
							}*/
                    }


                    break;

                case PowerupTypes.NORMALSPEED:

                    scoreGain = 50;
                    if (balls.Count > 0)
                    {
                        paddleImbueV = basePaddleImbueV;
                        /*int i;
						for (i= 0; i < balls.Count; i++) {
							float angle= atan2(balls[i].ballvy, balls[i].ballvx);
							balls[i].ballvx= maxv * cos(angle);
							balls[i].ballvy= maxv * sin(angle);
							}*/
                    }

                    break;

                case PowerupTypes.LARGEPADDLE:

                    scoreGain = 50;
                    setPaddleSize(150);

                    break;

                case PowerupTypes.SMALLPADDLE:

                    scoreGain = 50;
                    setPaddleSize(50);

                    break;

                case PowerupTypes.REGULARPADDLE:

                    scoreGain = 50;
                    setPaddleSize(100);

                    break;

                case PowerupTypes.CATCHBLUE:

                    scoreGain = 50;
                    catchblue = true;
                    catchbluestart = (int)Timing.TotalMilliseconds + 30000;

                    break;

                case PowerupTypes.CATCHRED:

                    scoreGain = 50;
                    catchred = true;
                    catchredstart = (int)Timing.TotalMilliseconds + 30000;

                    break;

                case PowerupTypes.POW:

                    scoreGain = 50;
                    pow = true;
                    powstart = (int)Timing.TotalMilliseconds + 10000;
                    {
                        for (int i = 0; i < balls.Count; i++)
                        {
                            if (balls[i].ballsticking)
                                balls[i].setDamage(balls[i].damage + CBall.powIncrement);
                        }
                    }

                    break;

                case PowerupTypes.SMASH:

                    scoreGain = 50;
                    smash = true;
                    smashstart = (int)Timing.TotalMilliseconds + 10000;
                    {
                        for (int i = 0; i < balls.Count; i++)
                        {
                            if (balls[i].ballsticking)
                                balls[i].smash = true;
                        }
                    }

                    break;

                case PowerupTypes.DOOR:

                    scoreGain = 50;

                    if (transitionout == false)
                    {
                        transitionout = true;
                        snd.speedup.Play();
                    }

                    break;

                case PowerupTypes.RESET:

                    scoreGain = 50;

                    // adjust the amount of points gained for whether we are
                    // losing "good" or "bad" powerups
                    if (blaster) scoreGain += 25;
                    if (fireball) scoreGain += 25;
                    if (paddlew < 100) scoreGain -= 25;
                    if (paddlew > 100) scoreGain += 25;
                    if (paddleImbueV > basePaddleImbueV + 25) scoreGain -= 25;
                    if (paddleImbueV < basePaddleImbueV - 25) scoreGain += 25;
                    if (stickypaddle) scoreGain += 25;
                    if (supersticky) scoreGain += 25;
                    if (catchblue) scoreGain += 25;
                    if (catchred) scoreGain -= 25;
                    if (pow) scoreGain += 25;
                    if (smash) scoreGain += 25;

                    setPaddleSize(100);
                    catchblue = catchred = stickypaddle = supersticky = blaster = fireball = false;
                    pow = smash = false;
                    paddleImbueV = basePaddleImbueV;
                    {
                        int i;
                        for (i = 0; i < balls.Count; i++)
                        {
                            CBall myball = balls[i];

                            if (myball.ballsticking)
                            {
                                myball.fireball = false;
                                myball.setDamage(100);
                                myball.smash = false;
                            }
                        }
                    }

                    break;

                case PowerupTypes.RANDOM:

                    // this is worth 50 extra points, but we pass it to the actual powerup
                    // so that the scorebyte that's displayed on the screen is the right
                    // number.
                    scoreGain = 0;

                    // declare some local variables, so enclose in braces to 
                    // avoid stupid warnings.
                    {
                        CPowerUpList list = new CPowerUpList();
                        CPowerUp actual;

                        // build the list of available powerups.
                        BuildPowerUpList(list);
                        list.removeEffect(PowerupTypes.RANDOM);
                        list.removeEffect(PowerupTypes.DOOR);

                        powerupcount--;

                        // choose one of them
                        list.AssignPowerup(out actual, 0, 0, time);

                        // execute it
                        powerUp(actual, 50);

                        // change my icon:
                        powerup.icon = actual.icon;

                        // delete it.
                        // delete actual;
                    }

                    break;
            }

            // don't let the point gain drop below zero
            if (scoreGain < 0) scoreGain = 0;

            // gain the actual points
            gainPoints(scoreGain, (int)(powerup.x + 20), (int)(powerup.y + 20));

        }

        private void setPaddleSize(float fNewSize)
        {
            float oldSize = paddlew;
            paddlew = fNewSize;

            if (balls.Count > 0)
            {
                // now, adjust the positions of the balls, so none of them
                // are way off the edge of the paddle.
                int i;
                for (i = 0; i < balls.Count; i++)
                {
                    if (balls[i].ballsticking)
                    {
                        // the formula is to keep the balls at the same ratio of distance
                        // from the edge of the paddle to the size of the paddle
                        float oldPos = balls[i].stickydifference / oldSize;

                        balls[i].stickydifference = oldPos * paddlew;
                    }

                }
            }

            // make sure the paddle does not go outside the edge.
            paddleCheckEdge();
        }

        private void BuildPowerUpList(CPowerUpList pulist)
        {
            // erase whatever's in the list now
            pulist.clear();

            // only list the powerups in the list that will actually have an effect
            if (paddlew < 110) pulist.addEffect(PowerupTypes.LARGEPADDLE, GetPUIcon(PowerupTypes.LARGEPADDLE), 50);
            if (paddlew > 90) pulist.addEffect(PowerupTypes.SMALLPADDLE, GetPUIcon(PowerupTypes.SMALLPADDLE), 30);
            if (!blaster) pulist.addEffect(PowerupTypes.BLASTER, GetPUIcon(PowerupTypes.BLASTER), 10);
            if (paddleImbueV <= 800) pulist.addEffect(PowerupTypes.FASTBALL, GetPUIcon(PowerupTypes.FASTBALL), 50);
            if (!fireball) pulist.addEffect(PowerupTypes.FIREBALL, GetPUIcon(PowerupTypes.FIREBALL), 10);
            if (90 > paddlew || paddlew > 110) pulist.addEffect(PowerupTypes.REGULARPADDLE, GetPUIcon(PowerupTypes.REGULARPADDLE), 20);
            if (basePaddleImbueV > paddleImbueV + 25 ||
                basePaddleImbueV < paddleImbueV - 25)
                pulist.addEffect(PowerupTypes.NORMALSPEED, GetPUIcon(PowerupTypes.NORMALSPEED), 30);
            if (paddleImbueV >= 200) pulist.addEffect(PowerupTypes.SLOWBALL, GetPUIcon(PowerupTypes.SLOWBALL), 30);

            if (!stickypaddle) pulist.addEffect(PowerupTypes.STICKY, GetPUIcon(PowerupTypes.STICKY), 10);
            if (stickypaddle) pulist.addEffect(PowerupTypes.SUPERSTICKY, GetPUIcon(PowerupTypes.SUPERSTICKY), 20);

            if (!catchblue) pulist.addEffect(PowerupTypes.CATCHBLUE, GetPUIcon(PowerupTypes.CATCHBLUE), 10);
            if (!catchred) pulist.addEffect(PowerupTypes.CATCHRED, GetPUIcon(PowerupTypes.CATCHRED), 20);

            if (balls.Count < 30) pulist.addEffect(PowerupTypes.PU3BALL, GetPUIcon(PowerupTypes.PU3BALL), 30);
            pulist.addEffect(PowerupTypes.MULTIBALL, GetPUIcon(PowerupTypes.MULTIBALL), 20);

            // these should always be available for dropping under any conditions.
            pulist.addEffect(PowerupTypes.RESET, GetPUIcon(PowerupTypes.RESET), 30);
            pulist.addEffect(PowerupTypes.ONEUP, GetPUIcon(PowerupTypes.ONEUP), 2);
            pulist.addEffect(PowerupTypes.RANDOM, GetPUIcon(PowerupTypes.RANDOM), 30);

            pulist.addEffect(PowerupTypes.POW, GetPUIcon(PowerupTypes.POW), 20);
            pulist.addEffect(PowerupTypes.SMASH, GetPUIcon(PowerupTypes.SMASH), 10);
            pulist.addEffect(PowerupTypes.RBSWAP, GetPUIcon(PowerupTypes.RBSWAP), 30);

            pulist.addEffect(PowerupTypes.DOOR, GetPUIcon(PowerupTypes.DOOR), 1);

        }

        private Sprite GetPUIcon(PowerupTypes effect)
        {

            switch (effect)
            {
                case PowerupTypes.ONEUP: return img.pu1up;
                case PowerupTypes.POW: return img.pupow;
                case PowerupTypes.SMASH: return img.pusmash;
                case PowerupTypes.RBSWAP: return img.purbswap;
                case PowerupTypes.DOOR: return img.pudoor;
                case PowerupTypes.BLASTER: return img.publaster;
                case PowerupTypes.FASTBALL: return img.pufastball;
                case PowerupTypes.FIREBALL: return img.pufireball;
                case PowerupTypes.MULTIBALL: return img.pumultiball;
                case PowerupTypes.PU3BALL: return img.pu3ball;
                case PowerupTypes.LARGEPADDLE: return img.pupaddlelarge;
                case PowerupTypes.REGULARPADDLE: return img.pupaddleregular;
                case PowerupTypes.SMALLPADDLE: return img.pupaddlesmall;
                case PowerupTypes.NORMALSPEED: return img.puregularspeed;
                case PowerupTypes.SLOWBALL: return img.puslowball;
                case PowerupTypes.STICKY: return img.pusticky;
                case PowerupTypes.RESET: return img.pureset;
                case PowerupTypes.PTS100: return img.pu100;
                case PowerupTypes.PTS250: return img.pu250;
                case PowerupTypes.PTS500: return img.pu500;
                case PowerupTypes.PTS1000: return img.pu1000;
                case PowerupTypes.RANDOM: return img.purandom;
                case PowerupTypes.CATCHBLUE: return img.pucatchblue;
                case PowerupTypes.CATCHRED: return img.pucatchred;
                case PowerupTypes.SUPERSTICKY: return img.pusupersticky;

                default: return null;
            }
        }

        private void dropScoreByte(int myx, int myy, int amount)
        {
            if (scoreByteLimit < scoreBytes.Count)
                return;

            string str;
            Color clr;
            double scale = 1.0;

            if (amount > 0)
            {
                str = "+" + (amount);
                clr = Color.FromArgb(255, 50, 255, 50);
            }
            else if (amount < 0)
            {
                str = "-" + (-amount);
                clr = Color.Red;
            }
            else // amount == 0
                return;

            if (amount >= 20)
                scale *= 1.2;
            if (amount >= 50)
                scale *= 1.1;
            if (amount >= 100)
                scale *= 1.2;
            if (amount >= 250)
                scale *= 1.2;
            if (amount >= 500)
                scale *= 1.2;
            if (amount >= 1000)
                scale *= 1.2;

            img.font.Size = (int)(14 * scale);

            Size size = img.font.MeasureString(str);
            int width = size.Width;
            int height = size.Height;

            myx -= width / 2;
            myy -= height / 2;

            if (myx + width > 800)
                myx = 800 - width;

            //*surf = new Surface(
            CScoreByte sb = new CScoreByte(myx, myy, str, null /*surf*/, clr, scale);

            scoreBytes.Add(sb);

        }

        private void updateScoreBytes()
        {
            DB_EnterSubsection("updateScoreBytes");

            for (int i = 0; i < scoreBytes.Count; i++)
            {
                scoreBytes[i].update(time_s);

                if (scoreBytes[i].getAlpha() < 0.001f)
                    deleteScoreByte(i);

            }

            DB_ExitSubsection();

        }

        private void deleteScoreByte(int myscore)
        {
            // delete scoreBytes[myscore];
            //scoreBytes.erase(scoreBytes.begin() + myscore);
            scoreBytes.RemoveAt(myscore);
        }

        private void deleteAllScoreBytes()
        {
            while (scoreBytes.Count > 0)
                deleteScoreByte(0);
        }

        private void DrawScoreBytes()
        {

            for (int i = 0; i < scoreBytes.Count; i++)
            {
                CScoreByte mybyte = scoreBytes[i];
                int x = mybyte.getx;
                int y = mybyte.gety;
                string amount = mybyte.getAmount();

                img.font.Size = 24;
                img.font.Color = Color.Black;
                img.font.Alpha = (mybyte.getAlpha());
                img.font.Size = (int)(24 * mybyte.Scale / 2.0);

                img.font.DrawText(x + 1, y, amount);
                img.font.DrawText(x - 1, y, amount);
                img.font.DrawText(x, y + 1, amount);
                img.font.DrawText(x, y - 1, amount);

                img.font.Color = mybyte.getColor();
                img.font.Alpha = (mybyte.getAlpha());

                img.font.DrawText(x, y, amount);

            }

            img.font.Size = 14;

        }

        private void dropPowerUp(int myx, int myy, PowerupTypes type, GameTime time)
        {
            CPowerUp pup = new CPowerUp(myx, myy, time);

            pup.setEffect(type);
            pup.icon = GetPUIcon(type);

            powerups.Add(pup);
        }

        private void dropPowerUp(int myx, int myy, bool pointsonly, GameTime time)
        {
            CPowerUp ppowerup;
            CPowerUpList pulist = new CPowerUpList();
            int set;

            BuildPowerUpList(pulist);

            if (pointsonly)
            {

                ppowerup = new CPowerUp(myx, myy, time);

                set = (ppowerup.start / 10) % 50;

                if (set >= 0 && set < 35) { ppowerup.setEffect(PowerupTypes.PTS100); ppowerup.icon = img.pu100; }
                if (set >= 35 && set < 45) { ppowerup.setEffect(PowerupTypes.PTS250); ppowerup.icon = img.pu250; }
                if (set >= 45 && set < 49) { ppowerup.setEffect(PowerupTypes.PTS500); ppowerup.icon = img.pu500; }
                if (set >= 49 && set < 50) { ppowerup.setEffect(PowerupTypes.PTS1000); ppowerup.icon = img.pu1000; }
            }
            else
            {
                pulist.AssignPowerup(out ppowerup, myx, myy, time);
            }


            powerups.Add(ppowerup);

        }

        private void updatePowerUps()
        {
            if (powerups.Count > 0)
            {
                int i;
                bool badPowerup;



                for (i = 0; i < powerups.Count; i++)
                {
                    bool hitpaddle = true;
                    badPowerup = false;

                    CPowerUp mypowerup = powerups[i];

                    if (mypowerup.x > paddlex + paddlew) hitpaddle = false;
                    if (mypowerup.y > paddley + paddleh) hitpaddle = false;
                    if (mypowerup.x + 40 < paddlex) hitpaddle = false;
                    if (mypowerup.y + 40 < paddley) hitpaddle = false;

                    if (catchblue && mypowerup.isBlue())
                        hitpaddle = true;
                    if (catchred && mypowerup.isRed())
                        hitpaddle = true;

                    if (mypowerup.getEffect() == PowerupTypes.PU_NONE) hitpaddle = false;

                    if (dying) hitpaddle = false;

                    if (hitpaddle)
                    {
                        powerUp(mypowerup);
                        mypowerup.setEffect(PowerupTypes.PU_NONE);
                        mypowerup.vx = paddleVelocity / 10;

                        snd.powerup.Play();

                    }
                    if (!mypowerup.update(time_s))
                    {
                        // delete it if the alpha is gone....
                        badPowerup = true;

                    }


                    if (badPowerup)
                    {
                        deletePowerUp(i);
                        i--;
                    }
                }
            }
        }

        private void deletePowerUp(int mypowerup)
        {
            // delete powerups[mypowerup];
            //powerups.erase(powerups.begin() + mypowerup, powerups.begin() + mypowerup + 1);
            powerups.RemoveAt(mypowerup);
        }

        private void deleteAllPowerUps()
        {

            if (powerups.Count > 0)
            {
                int i;
                for (i = 0; i < powerups.Count; i++)
                {
                    // delete powerups[i];
                }
                powerups.Clear();
            }
        }

        private void DrawPowerUps()
        {
            DB_EnterSubsection("DrawPowerUps");

            for (int i = 0; i < powerups.Count; i++)
            {
                CPowerUp mypowerup = powerups[i];

                mypowerup.icon.Color = mypowerup.Color;
                mypowerup.icon.SetScale(mypowerup.w, mypowerup.h);
                mypowerup.icon.Draw((int)mypowerup.x, (int)mypowerup.y);

                if (mypowerup.oldeffect == PowerupTypes.RANDOM)
                {
                    img.purandom.Color = mypowerup.Color;
                    img.purandom.SetScale(mypowerup.h, mypowerup.h);
                    img.purandom.Draw((int)mypowerup.x, (int)mypowerup.extray);
                }
            }


            DB_ExitSubsection();
        }

        //-------------- TITLE SCREEN ===============================================================================

        private string runTitle()
        {

            int levelstart;

            Music session = null;
            if (snd.music.Count > 0)
            {
                session = snd.music[0];
                session.IsLooping = true;

                if (playmusic)
                    session.Play();
            }

            lives = 2;
            thescore = 0;
            titlemode = " ";

            world = random.Next(0, worlds.Count - 1);
            level = random.Next(0, worlds[world].lvls.Count);

            initLevel(true);
            attractMode = true;
            attractvelocity = 0;

            hideTitle = false;

            lastMouseMove = (int)Timing.TotalMilliseconds;
            levelstart = lastMouseMove;

            beginningWorld = 0;
            beginningLevel = 0;
            beginningChanged = false;

            int lastframetime = (int)Timing.TotalMilliseconds;
            int frametime = (int)Timing.TotalMilliseconds;

            do
            {

                DB_DefineNewFrameDescriptor();

                time_s = (float)Display.DeltaTime / 1000.0f;

                // cap the time interval
                if (time_s > 0.1) time_s = 0.1f;

                //fps = 1/(frametime - lastframetime);
                //System.Diagnostics.Debug.Print("time_s: {0}", time_s);

                if (session != null && session.IsPlaying && !playmusic) session.Stop();
                if (session != null && !session.IsPlaying && playmusic) session.Play();

                updateLevel();
                updateTitle();

                Display.BeginFrame();

                DrawLevel();
                DrawTitle();

                EndFrame();

                lastframetime = frametime;
                frametime = (int)Timing.TotalMilliseconds;

                if (frametime - lastMouseMove > 15000)
                    hideTitle = true;

                // check to see if attract mode died or won.
                if (blocks.Count <= uncountedBlocks || balls.Count == 0
                    || frametime - levelstart > 100000 || beginningChanged)
                {
                    if (beginningChanged)
                    {
                        world = beginningWorld;
                        level = beginningLevel;
                    }
                    else
                    {
                        world = random.Next(0, worlds.Count - 1);
                        level = random.Next(0, worlds[world].lvls.Count);
                    }


                    initLevel(true);
                    attractMode = true;
                    attractvelocity = 0;

                    if (beginningChanged)
                    {
                        // update incase we went past the number of levels
                        beginningLevel = level;
                        beginningWorld = world;
                    }

                    beginningChanged = false;

                    levelstart = frametime;
                }

                if (titlemode == "quit")
                {
                    //if (session.is_playing()) session.stop();
                    return "quit";
                }
                if (titlemode == "leveleditor")
                {
                    level = beginningLevel;
                    //if (session.is_playing()) session.stop();

                    world = beginningWorld;
                    level = beginningLevel;

                    return "editor";
                }
                if (titlemode == "startgame")
                {
                    attractMode = false;
                    lives = 2;
                    thescore = 0;

                    world = beginningWorld;
                    level = beginningLevel;

                    beginningWorld = 0;
                    beginningLevel = 0;

                    //if (session.is_playing()) session.stop();
                    return "level";
                }

                DB_EndFrame();

            } while (Input.Unhandled.Keys[Keys.Escape] == false && Core.IsAlive);

            //if (session.is_playing()) session.stop();
            return "quit";
        }

        private void updateTitle()
        {
            if (time_s > 0)
            {

                // update bg scroll for fun... 
                bgy += bgspeed * time_s;
                if (bgy >= bgtile.DisplayHeight) bgy -= bgtile.DisplayHeight;

            }
        }

        private void DrawTitle()
        {
            // Draw perimeter
            if (!hideTitle)
            {
                img.topborder.Draw(0, 0);
                img.leftborder.Draw(0, 0);
                img.rightborder.Draw(735, 0);
                img.topborder.Draw(0, 590);

                Display.FillRect(
                    new Rectangle(0, 0, 800, 600),
                    Color.FromArgb(100, 0, 0, 0)
                    );
            }

            // hidetitle becomes true after a period of mouse inactivity.
            if (!hideTitle)
            {
                Display.FillRect(
                    new Rectangle(95, 95, 800, 322),
                    Color.FromArgb(0, 0, 0, 100)
                    );

                img.font.Size = 24;

                img.font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 100 && mousey < 130)
                {
                    img.font.Color = Color.Yellow;
                }
                img.font.DrawText(100, 100, "[START THE GAME]");
                img.font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 130 && mousey < 160)
                {
                    img.font.Color = Color.Yellow;
                }
                img.font.DrawText(100, 130, "[LEVEL EDITOR]");
                img.font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 160 && mousey < 190)
                {
                    img.font.Color = Color.Yellow;
                }
                img.font.DrawText(100, 160, "[QUIT]");
                img.font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 190 && mousey < 220)
                {
                    img.font.Color = Color.Yellow;
                }
                img.font.DrawText(100, 190, "[Full Screen / Windowed]");
                img.font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 220 && mousey < 250)
                {
                    img.font.Color = Color.Yellow;
                }
                if (bgscroll) img.font.DrawText(100, 220, "[Background Scroll On]");
                if (!bgscroll) img.font.DrawText(100, 220, "[Background Scroll Off]");
                img.font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 250 && mousey < 280)
                {
                    img.font.Color = Color.Yellow;
                }
                if (vsync) img.font.DrawText(100, 250, "[VSync On]");
                if (!vsync) img.font.DrawText(100, 250, "[VSync Off]");
                img.font.Color = Color.White;
                if (mousex > 100 - 20 && mousex < 500 && mousey > 280 && mousey < 310)
                {
                    img.font.Color = Color.Yellow;
                }
                if (playmusic) img.font.DrawText(100, 280, "[Play Music]");
                if (!playmusic) img.font.DrawText(100, 280, "[Do not play Music]");
                img.font.Color = Color.White;


                // Draw high scores
                img.font.DrawText(500, 100, "High scores");


                for (int i = 0; i < highscores.Count; i++)
                {
                    int Drawy = 150 + 15 * i;

                    img.font.DrawText(500, Drawy, (i + 1).ToString() + ". ");
                    img.font.DrawText(530, Drawy, highscores[i].name);
                    img.font.DrawText(680, Drawy, (highscores[i].score).ToString());

                }

                img.bblogo.Draw(40, 350);

                img.xlogo.Update();
                img.xlogo.Draw(640, 350);

                img.font.Size = 14;
                img.font.Color = Color.White;

                img.font.DrawText(100, 540, "Ball: Buster, by Patrick Avella (C) 2004");
                img.font.DrawText(100, 555, "Ball: Buster eXtreme modifications made by Erik Ylvisaker (C) 2004-8");
                img.font.DrawText(100, 570, "Because Breaking Stuff is Fun");

            }
            else
            {
                img.font.Size = 24;
                if ((int)Timing.TotalMilliseconds % 100 < 50)
                    img.font.Color = Color.FromArgb(255, 120, 120);
                else
                    img.font.Color = Color.White;

                img.font.DrawText(200, 30, "Move mouse for title screen.");

                img.font.Color = Color.White;
            }

            img.font.Size = 14;
            img.font.DrawText(100, 500, "Starting on level " + (beginningWorld + 1) + " - " + (beginningLevel + 1));


            if (!hideTitle)
            {
                // Draw cursor
                img.arrow.Draw(mousex, mousey);
            }


        }




        //------------ EDITOR

        private void loadEditor()
        {
            clearBlocks();
            loadLevel(true);
        }

        private void DrawEditor()
        {
            Display.BeginFrame();
            Display.Clear();

            // Draw the background tile
            DrawBackground((int)bgy);


            // Draw perimeter
            img.topborder.Draw(0, 0);
            img.leftborder.Draw(0, 0);
            img.rightborder.Draw(735, 0);

            img.block.Update();

            DrawBlocks();

            // we Draw the flash right on top of the block
            DrawFlashes();

            Display.FillRect(new Rectangle(0, 490, 800, 200), new Color(Color.Black, 175));

            // Draw whatever text
            string message = "EDITING LEVEL: " + (world + 1).ToString() + "-" + (level + 1);

            img.font.Size = 14;
            img.font.Color = Color.White;
            img.font.DrawText(spriteBatch, 13, 583, message);

            // draw menu
            foreach (KeyValuePair<Point, string> kvp in editorState.Menu)
            {
                Rectangle rect = new Rectangle(kvp.Key, img.font.MeasureString(kvp.Value));

                if (rect.Height > 16)
                    rect.Height = 16;

                if (rect.Contains(mousex + 20, mousey))
                    img.font.Color = Color.Yellow;
                else
                    img.font.Color = Color.White;

                img.font.DrawText(spriteBatch, kvp.Key, kvp.Value);
            }

            Point pt = EditorMousePoint();

            if (pt.X >= 40 && pt.X < 80 + 40 * 16 && pt.Y >= 10 && pt.Y < 10 + 20 * 24)
            {
                Display.FillRect(
                    new Rectangle(pt.X, pt.Y, 40, 20),
                    Color.FromArgb(100, 100, 255, 100)
                    );
            }

            img.font.DrawText(200, 540, "Choose your Brush:");


            foreach (KeyValuePair<Point, char> pair in editorState.Brushes)
            {
                Rectangle rect = new Rectangle(pair.Key, new Size(40, 20));

                DrawBlock(rect.X, rect.Y, pair.Value);

                if (editorState.brush != pair.Value)
                {
                    Display.FillRect(rect, Color.FromArgb(175, 0, 0, 0));
                }
            }

            img.pureset.Draw(530, 560);
            editorState.toiletRect = new Rectangle(530, 560, img.pureset.DisplayWidth, img.pureset.DisplayHeight);

            if (editorState.brush != 'x')
                Display.FillRect(editorState.toiletRect, Color.FromArgb(175, 0, 0, 0));
            // Draw cursor
            img.arrow.Draw(mousex, mousey);

            Display.EndFrame();
            Core.KeepAlive();

        }

        private string runEditor()
        {
            bgspeed = bgscroll ? 50.0f : 0.0f;

            clearBlocks();

            loadEditor();

            EditorSetMenu();
            SetBrushes();

            img.arrow.Color = Color.White;

            editorState.brush = 'r';

            while (Input.Unhandled.Keys[Keys.Escape] == false && Core.IsAlive)
            {
                UpdateEditor();
                DrawEditor();

                if (editorState.editormode == "reload") { editorState.editormode = " "; return "editor"; }
                if (editorState.editormode == "titlescreen") { editorState.editormode = " "; return "title"; }
                if (editorState.editormode == "new") { editorState.editormode = " "; EditorNewLevel(); }
            }

            return "title";
        }

        private void EditorNewLevel()
        {
            clearBlocks();

            level = worlds[world].lvls.Count;
        }

        private void EditorSetMenu()
        {
            editorState.Menu.Clear();

            int height = img.font.FontHeight;

            editorState.Menu.Add(new Point(13, 500), "[TITLE SCREEN]");
            editorState.Menu.Add(new Point(13, 516), "[NEW LEVEL]");
            editorState.Menu.Add(new Point(13, 532), "[SAVE LEVEL]");
            editorState.Menu.Add(new Point(13, 548), "[UP LEVEL]");
            editorState.Menu.Add(new Point(13, 564), "[DOWN LEVEL]");
        }

        private void SetBrushes()
        {
            editorState.Brushes.Clear();

            editorState.Brushes.Add(new Point(200, 560), 'r');
            editorState.Brushes.Add(new Point(240, 560), 'o');
            editorState.Brushes.Add(new Point(280, 560), 'y');
            editorState.Brushes.Add(new Point(320, 560), 'g');
            editorState.Brushes.Add(new Point(360, 560), 't');
            editorState.Brushes.Add(new Point(400, 560), 'b');
            editorState.Brushes.Add(new Point(440, 560), 'v');
            editorState.Brushes.Add(new Point(480, 560), 'w');

            editorState.Brushes.Add(new Point(200, 580), 'p');
            editorState.Brushes.Add(new Point(240, 580), '1');
            editorState.Brushes.Add(new Point(280, 580), '2');
            editorState.Brushes.Add(new Point(320, 580), '3');
            editorState.Brushes.Add(new Point(360, 580), 'c');
            editorState.Brushes.Add(new Point(400, 580), 'd');
            editorState.Brushes.Add(new Point(440, 580), 'e');
            editorState.Brushes.Add(new Point(480, 580), 's');
        }

        private void UpdateEditor(GameTime time)
        {

            handleEditorClicks();

            // update the block fragments and the brick flashes
            updateFlashes(time);

            // update bg scroll for fun... 
            bgy += bgspeed * time_s;

            if (bgy >= bgtile.DisplayHeight)
                bgy -= bgtile.DisplayHeight;
        }

        private CBlock findBlockAt(Point point)
        {
            return findBlockAt(point.X, point.Y);
        }

        private CBlock findBlockAt(int myx, int myy)
        {

            if (blocks.Count > 0)
            {
                int i;
                for (i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].getx() == myx && blocks[i].gety() == myy)
                        return blocks[i];
                }
            }
            return null;
        }

        private void deleteBlockAt(int myx, int myy)
        {

            if (blocks.Count > 0)
            {
                int i;
                for (i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].getx() == myx && blocks[i].gety() == myy)
                    {
                        // delete blocks[i];
                        blocks.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        private void handleEditorClicks(GameTime time)
        {
            if (mousedown == false)
                return;


            foreach (KeyValuePair<Point, char> pair in editorState.Brushes)
            {
                Rectangle rect = new Rectangle(pair.Key, new Size(40, 20));

                if (rect.Contains(mousex + 20, mousey))
                {
                    editorState.brush = pair.Value;
                    break;
                }
            }

            if (editorState.toiletRect.Contains(mousex + 20, mousey))
                editorState.brush = 'x';

            Point pt = EditorMousePoint();

            if (pt.X >= 40 && pt.X < 80 + 40 * 16 && pt.Y >= 10 && pt.Y < 10 + 20 * 24)
            {
                CBlock block = findBlockAt(pt);

                if (block == null && editorState.brush != 'x')
                {
                    dropFlash(pt.X, pt.Y, time);
                    addBlock(pt.X, pt.Y, editorState.brush);
                }
                else if (block != null && editorState.brush == 'x')
                {
                    dropFlash(pt.X, pt.Y, time);
                    deleteBlockAt(pt.X, pt.Y);
                }
            }
        }

        private Point EditorMousePoint()
        {
            return new Point(mousex - (mousex % 40) + 20, mousey - ((mousey - 10) % 20));
        }

        private void saveEditor()
        {
            string name;

            if (level >= worlds[world].lvls.Count)
            {
                name = string.Format("{0}-{1}", world + 1, level + 1);

                while (worlds[world].lvls.Contains(name))
                {
                    name += "a";
                }

                worlds[world].lvls.Add(name);

                SaveWorlds();
            }
            else
                name = worlds[world].lvls[level];

            StreamWriter writer = new StreamWriter("lvls/" + name + ".lvl");

            for (int j = 0; j < 24; j++)
            {
                for (int i = 0; i < 17; i++)
                {
                    CBlock block = findBlockAt(60 + 40 * i, 10 + 20 * j);

                    if (block != null) writer.Write(block.color);
                    if (block == null) writer.Write("_");
                }

                writer.Write("\n");
            }

            writer.Close();
            snd.ching.Play();

        }

        private void splash(IContentProvider content)
        {
            img.preload(content);

            img.font.Color = Color.Black;
            int start = (int)Timing.TotalMilliseconds;
            int wait = 50;

            if (debugger)
                wait = 500;

            while (start + wait > (int)Timing.TotalMilliseconds && Core.IsAlive)
            {
                Display.BeginFrame();

                Display.Clear(Color.White);

                img.palogo.Draw(335, 100);

                img.font.Color = Color.Black;

                img.font.DrawText(175, 250, "Ball: Buster eXtreme.NET");
                img.font.DrawText(175, 265, "Copyright 2004-9 Patrick Avella, Erik Ylvisaker");
                img.font.DrawText(175, 280, "Game Programming: Patrick Avella (patrickavella.com)");
                img.font.DrawText(175, 295, "eXtreme & .NET Version Programming: Erik Ylvisaker");
                img.font.DrawText(175, 310, "Game Art: Patrick Avella (patrickavella.com)");
                img.font.DrawText(175, 325, "AgateLib Programming: Erik Ylvisaker (www.agatelib.org)");
                img.font.DrawText(175, 340, "Background Music: Partners in Rhyme (musicloops.com)");
                img.font.DrawText(175, 355, "Sound Effects: A1 Free Sound Effects (a1freesoundeffects.com)");


                Display.EndFrame();
                Core.KeepAlive();

            }

            img.font.Size = 14;
            img.font.Color = Color.White;
        }

        private void loadHighscores()
        {
            StreamReader hs;
            string buffer;

            try
            {
                hs = new StreamReader("highscores");
            }
            catch
            {
                // highscores file isn't there.. just return
                // and it will be saved.
                highscores.Add(new Highscore("Kanato", 500000));
                highscores.Add(new Highscore("Skel1", 400000));
                highscores.Add(new Highscore("Yelena", 300000));
                highscores.Add(new Highscore("Dave", 200000));
                highscores.Add(new Highscore("John", 100000));
                highscores.Add(new Highscore("Robert", 50000));
                highscores.Add(new Highscore("Victor", 25000));
                highscores.Add(new Highscore("Brant", 10000));
                highscores.Add(new Highscore("Alexis", 5000));

                return;
            }


            while (hs.EndOfStream == false)
            {
                Highscore myscore;

                buffer = hs.ReadLine();

                try
                {
                    if (buffer[0] != 0)
                    {
                        myscore = new Highscore(buffer);

                        highscores.Add(myscore);
                    }
                }
                catch
                {
                    // failed to create high scores.  That's ok, when the file is saved, it
                    // will be removed.
                }


            }

            hs.Dispose();
        }

        private void saveHighscores()
        {
            using (StreamWriter hs = new StreamWriter("highscores"))
            {

                for (int i = 0; i < highscores.Count; i++)
                {
                    hs.WriteLine(highscores[i].ToString());
                }
            }
        }

        private string GetKeyChar()
        {
            string temp = hitkeystring;

            hitkeystring = null;

            return temp;
        }

        private Keys getkeypress()
        {
            Keys temp = hitkey;

            hitkey = 0;

            return temp;
        }

        private int getScore()
        {
            return thescore;
        }

        private void gainPoints(int pts)
        {
            gainPoints(pts, -1, -1);
        }

        private void gainPoints(int pts, int x, int y)
        {
            if (freezeScore)
                return;

            thescore += pts;


            if (x > 0)
            {
                dropScoreByte(x, y, pts);
            }


        }

        private void dropFadeBall(CBall myball)
        {
            if (fadeBalls.Count > ballLimit)
                return;

            CFadeBall fb = new CFadeBall(myball);

            fadeBalls.Add(fb);
        }

        private void updateFadeBalls()
        {
            DB_EnterSubsection("updateFadeBalls");

            for (int i = 0; i < fadeBalls.Count; i++)
            {
                CFadeBall fb = fadeBalls[i];

                if (fb.update(time_s) == false)
                {
                    deleteFadeBall(i);
                    i--;
                }
            }

            DB_ExitSubsection();

        }

        private void DrawFadeBalls()
        {
            DB_EnterSubsection("DrawFadeBalls");

            for (int i = 0; i < fadeBalls.Count; i++)
            {
                CFadeBall fb = fadeBalls[i];

                img.fireball.Alpha = fb.alpha;
                img.fireball.RotationAngleDegrees = fb.angle;
                img.fireball.SetScale(fb.scale, fb.scale);

                img.fireball.Draw((int)fb.x, (int)fb.y);
            }

            img.fireball.Alpha = 1.0f;
            img.fireball.SetScale(1.0f, 1.0f);

            DB_ExitSubsection();

        }

        private void deleteFadeBall(int id)
        {
            fadeBalls.RemoveAt(id);
        }

        private void deleteAllFadeBalls()
        {
            fadeBalls.Clear();
        }

        private void loadWorlds()
        {
            CSettingsFile file = new CSettingsFile("lvls/worlds.cfg");

            for (int i = 0; i < file.SectionCount; i++)
            {
                string w = file.getSection(i);
                CWorld world;

                world = new CWorld();

                world.name = file.ReadString(w + ".name", w);
                world.background = file.ReadString(w + ".background", "bg1.png");

                string light = file.ReadString(w + ".ambient", "ffffff");
                world.light = Color.FromArgb(light);

                string[] values = file.ReadStringArray(w + ".levels");

                world.lvls.AddRange(values);

                if (world.lvls.Count > 0)
                {
                    worlds.Add(world);
                }
            }
        }

        private void SaveWorlds()
        {
            CSettingsFile file = new CSettingsFile("lvls/worlds.cfg");

            foreach (CWorld w in worlds)
            {
                file.WriteStringArray(w.name + ".levels", w.lvls.ToArray());
            }

            file.Save();
        }

        private void freeWorlds()
        {
            worlds.Clear();

            bgtile = null;
        }

        /*
	void deleteAllEnemies()
	{
		while (enemies.Count > 0)
			deleteEnemy(0);
	}
	void deleteEnemy(int id)
	{
		// delete enemies[id];

		enemies.erase(enemies.begin() + id, enemies.begin() + id + 1);
	}
	void updateEnemies()
	{
		for (int i = 0; i < enemies.Count; i++)
		{
			if (!enemies[i].update((float)(fps)))
			{
				deleteEnemy(i);

				i--;
			}
		}
	}


	void DrawEnemies()
	{
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].Draw();
		}
	}
	void dropEnemy()
	{
		CEnemy en = new CEnemy();

		enemies.Add(en);
	}
		 * */
    }
}