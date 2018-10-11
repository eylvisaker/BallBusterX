using AgateLib;
using AgateLib.Display;
using AgateLib.Display.Sprites;
using AgateLib.Mathematics.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace BallBusterX
{
    public class GameState
    {
        public Random random = new Random();

        private const float maxPaddleImbueV = 1000.0f;
        private const float minPaddleImbueV = 200.0f;
        private bool doLighting = true;
        public const int ballLimit = 100;

        /// <summary>
        /// Total time this level has been running in milliseconds.
        /// </summary>
        public double leveltime;

        // the variables here are REALLY sloppy, sorry......... :P
        public string gamemode;
        public bool attractMode;
        public int attractvelocity;

        public int levelChange;

        public int powerupLeft = 0;
        public int powerupTop = 0;

        public bool stickypaddle;
        public bool supersticky;

        public bool catchblue, catchred;
        public bool pow, smash;
        public bool fireball, blaster, stageover, transitionout, died, dying;
        public bool playmusic;

        public int ballStickCount;

        [Obsolete("Use catchbluetimeleft, catchredtimeleft instead")]
        public int catchbluestart, catchredstart;
        [Obsolete("Use blastertimeleft instead")]
        public int blasterstart;
        [Obsolete("Use fireballstart instead")]
        public int fireballstart;
        [Obsolete("Use superstickytimeleft instead")]
        public int superstickystart;
        public double superstickytimeleft;
        public double fireballtimeleft;
        public double blastertimeleft;
        public double catchbluetimeleft, catchredtimeleft;
        public double powtimeleft;
        public double smashtimeleft;

        [Obsolete("Use powtimeleft instead")]
        public int powstart;
        [Obsolete("Use smashtimeleft instead")]
        public int smashstart;

        public Texture2D bgtile;
        public string bgFile;

        public bool paused;

        public bool vsync;
        public Keys hitkey;
        public string hitkeystring;

        public int transitionspeed, transdelay;
        public int transstart, deathstart;
        public int lives;
        public int blocksforitem;    // these are for the system of dropping items....

        public int blocksforpoints;
        public int blocksdestroyed;
        public int transcount;

        public int uncountedBlocks;

        //ulong int score;
        public int thescore;
        public bool freezeScore;

        public int world;
        public int level;

        public int powerupcount;
        public int ballslost;

        public int blockPartLimit;
        public int scoreByteLimit;

        public bool verticalhit;
        public bool debugger;

        public float bgy, bgspeed;

        public float blockscrolly;
        public float blockscrollspeedy;

        public double pauseTimer;

        public float paddlex, paddley, paddlew, paddleh, paddlerot, paddlealpha;
        public float paddleVelocity;   // velocity that the paddle is moving.
        public float _paddleImbueV;        // velocity the paddle gives to the balls
        public float paddleImbueV
        {
            get { return _paddleImbueV; }
            set
            {
                _paddleImbueV = value;

                if (_paddleImbueV < minPaddleImbueV)
                    _paddleImbueV = minPaddleImbueV;
            }
        }

        public float basePaddleImbueV; // ordinary velocity the paddle gives
        public float basePaddleImbueVStart, basePaddleImbueVEnd;

        public struct lastPowerup
        {
            public Sprite pu;
            [Obsolete("This is unused?")]
            public int time;
        }

        public List<lastPowerup> lastPowerups = new List<lastPowerup>();
        public List<CBlock> blocks = new List<CBlock>();
        public List<CBlockPart> blockparts = new List<CBlockPart>();
        public List<CFlash> flashes = new List<CFlash>();
        public List<CBall> balls = new List<CBall>();
        public List<CFadeBall> fadeBalls = new List<CFadeBall>();
        public List<CPowerUp> powerups = new List<CPowerUp>();
        public List<CScoreByte> scoreBytes = new List<CScoreByte>();

        //List<CEnemy>			enemies;

        public WorldCollection worlds;
        private readonly BBXConfig config;
        public List<Highscore> highscores = new List<Highscore>();

        public Vector2 PaddlePos => new Vector2(paddlex, paddley);

        public GameState(GraphicsDevice device, CImage img, CSound snd, IContentProvider content, WorldCollection worlds, BBXConfig config)
        {
            this.device = device;
            this.img = img;
            this.snd = snd;
            this.content = content;
            this.worlds = worlds;
            this.config = config;

            font = new Font(img.Fonts.Default);

            paddlex = 350;
            paddlew = 100;
            paddleh = 20;
            paddley = 560;
            paddlealpha = 1.0f;

            blockPartLimit = 65;
            scoreByteLimit = 30;

            vsync = true;
            paddleImbueV = 500.0f;

            basePaddleImbueVStart = 325;
            basePaddleImbueVEnd = 425;

            transdelay = 100; 

            playmusic = true;
            bgspeed = 50.0f;
            level = 1;
            lives = 2;
            blocksforitem = 7;
            blocksforpoints = 10;

            levelChange = 1;
        }


        //-----------------------

        public int mousex, mousey;
        public bool mousedown;

        //=======================
        public string titlemode;
        private readonly GraphicsDevice device;
        private CImage img;
        private float time_s;
        private readonly CSound snd;
        private readonly IContentProvider content;
        private readonly Font font;

        //----------------------


        public void initLevel(bool resetPowerups)
        {
            leveltime = 0;
            stageover = false;
            transitionout = false;
            died = false;
            dying = false;

            transcount = 0;
            paddley = 560;
            paddlerot = 0.0f;
            paddlealpha = 1.0f;
            bgspeed = config.BackgroundScroll ? 50.0f : 0.0f;

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
                        myball.ballsticktimeleft = 4000;
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

        public void UpdateLevel(GameTime time)
        {
            img.paddle.Update(time);
            img.smallpaddle.Update(time);
            img.largepaddle.Update(time);

            time_s = (float)time.ElapsedGameTime.TotalSeconds;

            leveltime += time.ElapsedGameTime.TotalMilliseconds;

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

            for (j = 0; j < balls.Count; j++)
            {
                badBall = false;

                myball = balls[j];
                myball.update(time);


                if (myball.fireball)// && !myball.ballsticking)
                {
                    if (myball.timetonextfade < 0)
                    {
                        myball.timetonextfade = 10;

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
                        myball.ballsticktimeleft = 4000;
                    }


                }

                // if the ball is sticking, make sure to update it's position and 
                // check to see if it should automagically be released.
                if (myball.ballsticking)
                {

                    myball.ballx = paddlex + myball.stickydifference;
                    myball.bally = paddley - myball.ballh;

                    if (myball.ballsticktimeleft > 0 && !transitionout)
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

            // update everything else
            UpdateBlocks(time);
            updateBlockParts(time);
            updateFlashes(time);
            updatePowerUps(time);
            updateScoreBytes(time);
            updateFadeBalls(time);
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
        }

        // function for sort to sort balls by y.
        private int ballcheck(CBall a, CBall b)
        {
            return a.bally.CompareTo(b.bally);
        }


        public void DrawLevel(SpriteBatch spriteBatch)
        {
            //Display.Clear(Color.FromArgb(128, 0, 0, 128));
            SetLightingForLevel();

            // Draw the background tile, scrolled
            DrawBackground(spriteBatch, bgy);

            // Draw perimeter
            spriteBatch.Draw(img.topborder, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(img.leftborder, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(img.rightborder, new Vector2(735, 0), Color.White);

            ActivateLighting();

            // Draw blocks and Update their animations...
            DrawBlocks(spriteBatch);

            // we Draw the flash right on top of the block
            DrawFlashes(spriteBatch);


            // Draw all the other stuff except the balls here
            DrawBlockParts(spriteBatch);
            DrawFadeBalls(spriteBatch);

            if (doLighting)
            {
                //AgateBuiltInShaders.Basic2DShader.Activate();
            }

            // Draw paddle, other stuff, and lastly the balls.
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

            pad.Scale = new Vector2(paddlealpha, paddlealpha);
            pad.Alpha = paddlealpha;
            pad.SetRotationCenter(OriginAlignment.Center);
            pad.RotationAngleDegrees = paddlerot;
            pad.Draw(spriteBatch, new Vector2(PaddlePos.X + paddlew / 2, PaddlePos.Y + paddleh / 2));

            DrawPowerUps(spriteBatch);
            DrawScoreBytes(spriteBatch);

            // Draw the balls
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
                    img.spike.SetRotationCenter(OriginAlignment.Center);

                    for (int k = 0; k < spikes; k++)
                    {
                        img.spike.RotationAngleDegrees = myball.Ballangle + angle * k;
                        img.spike.Draw(spriteBatch, new Vector2(x, y));
                    }
                }

                ballimg.Draw(spriteBatch, new Vector2(ballx, bally));

                if (myball.smash)
                {
                    float offset = myball.ballw / 2 - img.smash.SpriteWidth / 2;

                    img.smash.RotationAngleDegrees = myball.SmashAngle;
                    img.smash.Draw(spriteBatch, new Vector2(ballx + offset, bally + offset));
                }
            }

            // Draw whatever text
            string message = "Score: " + getScore().ToString();

            font.Size = 14;
            font.Color = Color.Black;
            font.DrawText(spriteBatch, new Vector2(11, 585), message);
            font.Color = Color.White;
            font.DrawText(spriteBatch, new Vector2(13, 583), message);

            // output debugging messages, if the debugger is attached
            //if (debugger)
            //{
            //    font.DrawText(13, 505, "basePaddleImbueV: " + ((int)basePaddleImbueV));
            //    font.DrawText(13, 520, "paddleImbueV: " + ((int)paddleImbueV));
            //    font.DrawText(13, 550, "Balls Lost: " + (ballslost));
            //    font.DrawText(13, 565, "BallStickCount: " + (ballStickCount));
            //}

            //font.DrawText(60, 10, string.Format("FPS: {0:###.#}", Math.Round(Display.FramesPerSecond, 1)));

            // Draw extra lives
            img.ball.RotationAngle = 0;

            int ilives;
            for (ilives = 0; ilives < lives; ilives++)
            {
                img.ball.Draw(spriteBatch, new Vector2(775 - (ilives * 13), 588));
            }

            // Draw powerups that are in effect:
            //DrawPowerupsInEffect();


            // fade the screen to white if we are transitioning out
            if (transitionout)
            {
                Rectangle myrect = new Rectangle(0, 0, 800, 600);
                Color mycolor = new Color(255, 255, 255, transcount);

                FillRect(myrect, mycolor);

            }

            // display paused message
            if (paused)
            {
                Rectangle myrect = new Rectangle(0, 0, 800, 600);
                Color mycolor = new Color(0, 0, 0, 100);

                FillRect(myrect, mycolor);

                if (pauseTimer % 1000 < 500)
                {
                    font.Size = 30;
                    font.Color = Color.White;
                    font.TextAlignment = OriginAlignment.Center;

                    font.DrawText(spriteBatch, 400, 200, "PAUSED");
                    font.DrawText(spriteBatch, 400, 300, "Press 'P' to unpause.");

                    font.TextAlignment = OriginAlignment.TopLeft;

                    font.Size = 14;
                }
            }
        }

        private void FillRect(Rectangle myrect, Color mycolor) => throw new NotImplementedException();

        private void DrawBackground(SpriteBatch spriteBatch, float yval)
        {
            var renderTargetWidth = device.PresentationParameters.BackBufferWidth;
            var renderTargetHeight = device.PresentationParameters.BackBufferHeight;

            int cols = (int)Math.Ceiling(renderTargetWidth / (float)bgtile.Width);
            int rows = (int)Math.Ceiling(renderTargetHeight / (float)bgtile.Height);

            while (yval > bgtile.Height)
                yval -= bgtile.Height;

            for (int j = -1; j < rows; j++)
            {
                for (int i = 0; i < cols; i++)
                {
                    spriteBatch.Draw(bgtile, new Vector2(i * bgtile.Width, yval + j * bgtile.Height), Color.White);
                }
            }


            //bgtile.Draw(0, -bgtile.DisplayHeight + yval);
            //bgtile.Draw(bgtile.DisplayWidth, -bgtile.DisplayHeight + yval);
            //bgtile.Draw(0, 0 + yval);
            //bgtile.Draw(bgtile.DisplayWidth, 0 + yval);
            //bgtile.Draw(0, bgtile.DisplayHeight + yval);
            //bgtile.Draw(bgtile.DisplayWidth, bgtile.DisplayHeight + yval);

        }

        private void SetLightingForLevel()
        {
            //if (Display.Caps.IsHardwareAccelerated == false)
            //    return;

            //var shader = AgateBuiltInShaders.Lighting2D;

            //while (shader.Lights.Count > balls.Count)
            //    shader.Lights.RemoveAt(shader.Lights.Count - 1);

            //for (int i = 0; i < balls.Count; i++)
            //{
            //    Light light;

            //    if (i < shader.Lights.Count)
            //        light = shader.Lights[i];
            //    else
            //    {
            //        light = new Light();
            //        shader.Lights.Add(light);
            //    }

            //    if (balls[i].fireball)
            //    {
            //        light.Position = new Vector3(balls[i].ballx, balls[i].bally, -1);
            //        light.DiffuseColor = Color.FromArgb(255, 255, 0);
            //        light.AmbientColor = Color.FromArgb(64, 32, 0);

            //        light.AttenuationConstant = 0.01f;
            //        light.AttenuationLinear = 0.005f;
            //        light.AttenuationQuadratic = 0.000001f;

            //    }
            //    else
            //    {
            //        light.Position = new Vector3(balls[i].ballx, balls[i].bally, -1);
            //        light.DiffuseColor = Color.FromArgb(200, 200, 200);
            //        light.AmbientColor = Color.Black;

            //        light.AttenuationConstant = 0.01f;
            //        light.AttenuationLinear = 0;
            //        light.AttenuationQuadratic = 0.00001f;
            //    }
            //}
        }


        private void deleteBall(int myball)
        {
            if (balls[myball].ballsticking)
                ballStickCount--;

            balls.RemoveAt(myball);

        }

        private void dropBlockParts(float myx, float myy, Sprite myblock, Color clr, float ballvx, float ballvy)
        {

            if (blockparts.Count > blockPartLimit)
                return;

            if ((ballvx < 0) && verticalhit)
            {
                CBlockPart pblock0 = new CBlockPart((ballvx + ballvx * 0.5f), -ballvy, myx, myy, myblock, clr, random);
                CBlockPart pblock1 = new CBlockPart(ballvx, -ballvy, myx + 20, myy, myblock, clr, random);
                CBlockPart pblock2 = new CBlockPart((ballvx + ballvx * 0.5f), -ballvy + (-ballvy * 0.2f), myx, myy + 10, myblock, clr, random);
                CBlockPart pblock3 = new CBlockPart(ballvx, -ballvy + (-ballvy * 0.2f), myx + 20, myy + 10, myblock, clr, random);
                blockparts.Add(pblock0);
                blockparts.Add(pblock1);
                blockparts.Add(pblock2);
                blockparts.Add(pblock3);
                return;
            }

            if ((ballvx >= 0) && verticalhit)
            {
                CBlockPart pblock0 = new CBlockPart((ballvx + ballvx * 0.5f), -ballvy, myx + 20, myy, myblock, clr, random);
                CBlockPart pblock1 = new CBlockPart(ballvx, -ballvy, myx, myy, myblock, clr, random);
                CBlockPart pblock2 = new CBlockPart((ballvx + -ballvx * 0.5f), -ballvy + (-ballvy * 0.2f), myx + 20, myy + 10, myblock, clr, random);
                CBlockPart pblock3 = new CBlockPart(ballvx, -ballvy + (-ballvy * 0.2f), myx, myy + 10, myblock, clr, random);
                blockparts.Add(pblock0);
                blockparts.Add(pblock1);
                blockparts.Add(pblock2);
                blockparts.Add(pblock3);
                return;
            }

            if ((ballvx < 0) && !verticalhit)
            {
                CBlockPart pblock0 = new CBlockPart((-ballvx + -ballvx * 0.5f), ballvy, myx, myy, myblock, clr, random);
                CBlockPart pblock1 = new CBlockPart(-ballvx, ballvy, myx + 20, myy, myblock, clr, random);
                CBlockPart pblock2 = new CBlockPart((-ballvx + -ballvx * 0.5f), ballvy + (ballvy * 0.2f), myx, myy + 10, myblock, clr, random);
                CBlockPart pblock3 = new CBlockPart(-ballvx, ballvy + (ballvy * 0.2f), myx + 20, myy + 10, myblock, clr, random);
                blockparts.Add(pblock0);
                blockparts.Add(pblock1);
                blockparts.Add(pblock2);
                blockparts.Add(pblock3);
                return;
            }

            if ((ballvx >= 0) && !verticalhit)
            {
                CBlockPart pblock0 = new CBlockPart((-ballvx + -ballvx * 0.5f), ballvy, myx + 20, myy, myblock, clr, random);
                CBlockPart pblock1 = new CBlockPart(-ballvx, ballvy, myx, myy, myblock, clr, random);
                CBlockPart pblock2 = new CBlockPart((-ballvx + -ballvx * 0.5f), ballvy + (ballvy * 0.2f), myx + 20, myy + 10, myblock, clr, random);
                CBlockPart pblock3 = new CBlockPart(-ballvx, ballvy + (ballvy * 0.2f), myx, myy + 10, myblock, clr, random);
                blockparts.Add(pblock0);
                blockparts.Add(pblock1);
                blockparts.Add(pblock2);
                blockparts.Add(pblock3);
                return;
            }


        }
        private void updateBlockParts(GameTime time)
        {
            if (blockparts.Count > 0)
            {
                int i;
                for (i = 0; i < blockparts.Count; i++)
                {
                    CBlockPart b = blockparts[i];
                    if (!b.update(time))
                    {
                        // delete it if the alpha is gone....
                        deleteBlockPart(i);
                        i--;
                    }
                    else
                        blockparts[i] = b;

                }

            }
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


        private void DrawBlockParts(SpriteBatch spriteBatch)
        {
            if (blockparts.Count > 0)
            {
                int i;
                for (i = 0; i < blockparts.Count; i++)
                {
                    CBlockPart part = blockparts[i];

                    part.block.Scale = new Vector2(0.5f, 0.5f);
                    part.block.RotationAngleDegrees = part.rotation;

                    part.block.Color = part.mClr;
                    part.block.Alpha = part.alpha;

                    part.block.Draw(spriteBatch, part.position);

                    part.block.RotationAngleDegrees = 0;
                    part.block.Scale = new Vector2(1.0f, 1.0f);
                }
            }
        }

        private void dropFlash(float myx, float myy)
        {
            CFlash pFlash = new CFlash((int)myx, (int)myy);
            flashes.Add(pFlash);
        }

        private void updateFlashes(GameTime time)
        {
            if (flashes.Count > 0)
            {
                for (int i = 0; i < flashes.Count; i++)
                {
                    if (!flashes[i].update(time))
                    {
                        // if there's no alpha, delete it
                        deleteFlash(i);
                        i--;
                    }
                }
            }
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

        private void DrawFlashes(SpriteBatch spriteBatch)
        {
            if (flashes.Count > 0)
            {
                for (int i = 0; i < flashes.Count; i++)
                {
                    img.flash.Alpha = (flashes[i].alpha);
                    img.flash.Draw(spriteBatch, flashes[i].position);
                }
            }
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

            lastPowerup pu = new lastPowerup();

            pu.pu = powerup.icon;
            // pu.time = Timeing.Now.whatever

            lastPowerups.Add(pu);

        }

        private void powerUp(PowerupTypes effect)
        {
            CPowerUp dummy = new CPowerUp(0, 0);

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

                    superstickytimeleft = 30000;
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
                    blastertimeleft = 10000;

                    scoreGain = 50;

                    addBall();

                    break;

                case PowerupTypes.FIREBALL:

                    fireball = true;
                    fireballtimeleft = 10000;

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
                    catchbluetimeleft = 30000;

                    break;

                case PowerupTypes.CATCHRED:

                    scoreGain = 50;
                    catchred = true;

                    catchredtimeleft = 30000;
                    break;

                case PowerupTypes.POW:

                    scoreGain = 50;
                    pow = true;
                    powtimeleft = 10000;
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
                    smashtimeleft = 10000;
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
                        list.AssignPowerup(out actual, 0, 0, random);

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
                clr = new Color(50, 255, 50);
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

            var font = new Font(img.Fonts.Default);
            font.Size = (int)(14 * scale);

            Size size = font.MeasureString(str);
            int width = size.Width;
            int height = size.Height;

            myx -= width / 2;
            myy -= height / 2;

            if (myx + width > 800)
                myx = 800 - width;

            CScoreByte sb = new CScoreByte(myx, myy, str, font, clr, scale);

            scoreBytes.Add(sb);

        }

        private void updateScoreBytes(GameTime time)
        {
            for (int i = 0; i < scoreBytes.Count; i++)
            {
                scoreBytes[i].update(time);

                if (scoreBytes[i].getAlpha() < 0.001f)
                    deleteScoreByte(i);

            }
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

        private void DrawScoreBytes(SpriteBatch spriteBatch)
        {

            for (int i = 0; i < scoreBytes.Count; i++)
            {
                CScoreByte mybyte = scoreBytes[i];
                int x = mybyte.getx;
                int y = mybyte.gety;
                string amount = mybyte.getAmount();

                font.Size = 24;
                font.Color = Color.Black;
                font.Alpha = (mybyte.getAlpha());
                font.Size = (int)(24 * mybyte.Scale / 2.0);

                font.DrawText(spriteBatch, x + 1, y, amount);
                font.DrawText(spriteBatch, x - 1, y, amount);
                font.DrawText(spriteBatch, x, y + 1, amount);
                font.DrawText(spriteBatch, x, y - 1, amount);

                font.Color = mybyte.getColor();
                font.Alpha = (mybyte.getAlpha());

                font.DrawText(spriteBatch, x, y, amount);

            }

            font.Size = 14;

        }

        private void dropPowerUp(int myx, int myy, PowerupTypes type)
        {
            CPowerUp pup = new CPowerUp(myx, myy);

            pup.setEffect(type);
            pup.icon = GetPUIcon(type);

            powerups.Add(pup);
        }

        private void dropPowerUp(int myx, int myy, bool pointsonly)
        {
            CPowerUp ppowerup;
            CPowerUpList pulist = new CPowerUpList();
            int set;

            BuildPowerUpList(pulist);

            if (pointsonly)
            {
                ppowerup = new CPowerUp(myx, myy);

                set = random.Next(50);

                if (set >= 0 && set < 35) { ppowerup.setEffect(PowerupTypes.PTS100); ppowerup.icon = img.pu100; }
                if (set >= 35 && set < 45) { ppowerup.setEffect(PowerupTypes.PTS250); ppowerup.icon = img.pu250; }
                if (set >= 45 && set < 49) { ppowerup.setEffect(PowerupTypes.PTS500); ppowerup.icon = img.pu500; }
                if (set >= 49 && set < 50) { ppowerup.setEffect(PowerupTypes.PTS1000); ppowerup.icon = img.pu1000; }
            }
            else
            {
                pulist.AssignPowerup(out ppowerup, myx, myy, random);
            }

            powerups.Add(ppowerup);
        }

        private void updatePowerUps(GameTime time)
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
                    if (!mypowerup.update(time))
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

        private void DrawPowerUps(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < powerups.Count; i++)
            {
                CPowerUp mypowerup = powerups[i];

                mypowerup.icon.Color = mypowerup.Color;
                mypowerup.icon.Scale = new Vector2(mypowerup.w, mypowerup.h);
                mypowerup.icon.Draw(spriteBatch, mypowerup.position);

                if (mypowerup.oldeffect == PowerupTypes.RANDOM)
                {
                    img.purandom.Color = mypowerup.Color;
                    img.purandom.Scale = new Vector2(mypowerup.h, mypowerup.h);
                    img.purandom.Draw(spriteBatch, mypowerup.position);
                }
            }
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

            CFadeBall fb = new CFadeBall(myball, random);

            fadeBalls.Add(fb);
        }

        private void updateFadeBalls(GameTime time)
        {
            for (int i = 0; i < fadeBalls.Count; i++)
            {
                CFadeBall fb = fadeBalls[i];

                if (fb.update(time) == false)
                {
                    deleteFadeBall(i);
                    i--;
                }
            }
        }

        private void DrawFadeBalls(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < fadeBalls.Count; i++)
            {
                CFadeBall fb = fadeBalls[i];

                img.fireball.Alpha = fb.alpha;
                img.fireball.RotationAngleDegrees = fb.angle;
                img.fireball.Scale = new Vector2(fb.scale, fb.scale);

                img.fireball.Draw(spriteBatch, fb.position);
            }

            img.fireball.Alpha = 1.0f;
            img.fireball.Scale = new Vector2(1.0f, 1.0f);
        }

        private void deleteFadeBall(int id)
        {
            fadeBalls.RemoveAt(id);
        }

        private void deleteAllFadeBalls()
        {
            fadeBalls.Clear();
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

        private void ActivateLighting()
        {
            //if (Display.Caps.IsHardwareAccelerated == false)
            //    return;

            //if (doLighting)
            //{
            //    var shader = AgateBuiltInShaders.Lighting2D;

            //    shader.Activate();
            //}
        }

        private int getScore()
        {
            return thescore;
        }

        private bool paddleCheckEdge()
        {
            if (paddlex < 60) { paddlex = 60; return true; }
            if (paddlex > 740 - paddlew) { paddlex = 740 - paddlew; return true; }

            return false;
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

            //if (Display.Caps.IsHardwareAccelerated && doLighting)
            //    AgateBuiltInShaders.Lighting2D.AmbientLight = w.light;

            file = "lvls/" + worlds[world].lvls[level] + ".lvl";

            StreamReader myfile;
            try
            {
                myfile = new StreamReader(content.Open(file));
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

                bgtile = content.Load<Texture2D>(
                    $"imgs/{worlds[world].background}");
            }

        }

        private void checkBlock(int myblock, CBall myball, bool playSound = true)
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
                    blocks[myblock].shake();
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
                dropFlash(blocks[myblock].getx(), blocks[myblock].gety());

                if (playSound)
                    snd.bounce.Play();

            }

            // woot!
            gainPoints(scoreGain, blockx + 20, blocky + 10);

        }

        private void UpdateBlocks(GameTime time)
        {
            foreach (var block in blocks)
            {
                block.Update(time);
            }
        }

        private void DrawBlocks(SpriteBatch spriteBatch)
        {
            if (blocks.Count > 0)
            {
                int i;
                int sz = blocks.Count;
                float crack;
                float w = img.crack.DisplayWidth;
                float cwidth;

                for (i = 0; i < sz; i++)
                {
                    CBlock myblock = blocks[i];

                    myblock.block.Color = myblock.clr;
                    myblock.block.Draw(spriteBatch, myblock.position);

                    crack = myblock.crackPercentage();

                    if (crack > 0.001)
                    {
                        cwidth = w * crack / 2;
                        float vscale = 1.0f * (myblock.flipcrack ? -1 : 1);
                        //int vshift = (vscale < 0) ? 20 : 0;


                        img.crack.Alpha = (crack);
                        img.crack.Scale = new Vector2(crack / 2, vscale);

                        img.crack.Draw(spriteBatch, myblock.position);

                        img.crack.Scale = new Vector2(crack / 2, vscale);
                        img.crack.Draw(spriteBatch, new Vector2((int)myblock.getx() + (int)(40 * (1 - crack / 2)), (int)myblock.gety()));
                    }
                }
            }
        }

        private void addBlock(int myx, int myy, char color)
        {
            CBlock pBlock = new CBlock(random);
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

        private CBlock.BlockType getBlockData(char type, out Sprite spr, out Color clr, out int str)
        {
            CBlock.BlockType retval = CBlock.BlockType.Glass;

            clr = Color.White;
            str = 100;
            spr = img.block;

            if (type == 'r')                                // red glass
            {
                clr = new Color(255, 0, 0);
            }
            else if (type == 'o')                           // orange glass
            {
                clr = new Color(255, 155, 0);
            }
            else if (type == 'y')                           // yellow glass
            {
                clr = new Color(255, 255, 0);
            }
            else if (type == 'g')                           // green glass
            {
                clr = new Color(0, 255, 0);
            }
            else if (type == 't')                           // turquoise glass
            {
                clr = new Color(0, 255, 204);
            }
            else if (type == 'b')                           // blue glass
            {
                clr = new Color(0, 0, 255);
            }
            else if (type == 'v')                           // violet glass
            {
                clr = new Color(128, 0, 200);
            }
            else if (type == 'w')                           // white glass
            {
                clr = new Color(255, 255, 255);

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
        private void clearBlocks()
        {
            blocks.Clear();

            uncountedBlocks = 0;
        }

        public void MouseMove(Point mousePos)
        {
            mousex = mousePos.X;

            paddleVelocity = (mousex - (paddlex + paddlew / 2)) / time_s;

            paddlex = mousex - paddlew / 2;
            paddleCheckEdge();

            mousex = (int)(paddlex + paddlew / 2);
            mousey = 400;
        }

    }
}
