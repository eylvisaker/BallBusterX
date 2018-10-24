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

        public Paddle paddle = new Paddle();
        private IPaddleRenderer paddleRenderer;

        public const int ballLimit = 100;

        private bool doLighting = true;
        private float deathAnimation;

        private float lowBlockTimeLeft_ms;

        /// <summary>
        /// Total time this level has been running in milliseconds.
        /// </summary>
        public float levelTime_ms;

        // the variables here are REALLY sloppy, sorry......... :P
        public string gamemode;
        public bool attractMode;
        public int attractvelocity;

        public int levelChange;
        private readonly Texture2D whiteTexture;
        private readonly Effect lightingEffect;
        public int powerupLeft = 0;
        public int powerupTop = 0;

        public bool stickypaddle;
        public bool supersticky;

        public bool catchblue, catchred;
        public bool pow, smash;
        public bool fireball, blaster, stageComplete, transitionout, gameover, dying;

        public bool playmusic;

        public int ballStickCount;

        public float superstickytimeleft_s;
        public float fireballtimeleft_s;
        public float blastertimeleft_s;
        public float catchbluetimeleft_s;
        public float catchredtimeleft_s;
        public float powtimeleft_s;
        public float smashtimeleft_s;

        public Texture2D bgtile;
        public string bgFile;

        public bool paused;

        public bool vsync;
        public Keys hitkey;
        public string hitkeystring;

        public int transitionspeed, transdelay;
        public int transstart;
        public int Lives;
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

        //////////////////// TODO: Delete //////////////////////

        private const int standardPaddleWidth = 100;
        private const float maxPaddleImbueV = 1000.0f;
        private const float minPaddleImbueV = 200.0f;

        public int paddleSizeIndex => paddle.PaddleSizeIndex;
        public float paddleWidth => paddle.Width;
        public float paddleh => paddle.Height;
        public float paddlex => paddle.x;
        public float paddleRotationAngle => paddle.RotationAngle;
        public float paddleOpacity => paddle.Opacity;

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
        //////////////////// TODO: end delete //////////////////////
        ///
        
        public List<CBlock> blocks = new List<CBlock>();
        public List<CBlockPart> blockparts = new List<CBlockPart>();
        public List<CFlash> flashes = new List<CFlash>();
        public List<Ball> balls = new List<Ball>();
        public List<CFadeBall> fadeBalls = new List<CFadeBall>();
        public List<CPowerUp> powerups = new List<CPowerUp>();
        public List<CScoreByte> scoreBytes = new List<CScoreByte>();

        //List<CEnemy>			enemies;

        public WorldCollection worlds;
        private readonly BBXConfig config;
        public List<Highscore> highscores = new List<Highscore>();


        public GameState(GraphicsDevice graphics,
                         CImage img,
                         CSound snd,
                         IContentProvider content,
                         WorldCollection worlds,
                         BBXConfig config)
        {
            this.device = graphics;
            this.img = img;
            this.snd = snd;
            this.content = content;
            this.worlds = worlds;
            this.config = config;

            font = new Font(img.Fonts.Default);

            paddleRenderer = new PaddleRenderer(paddle, img, graphics);

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
            Lives = 3;
            blocksforitem = 7;
            blocksforpoints = 10;

            levelChange = 1;

            whiteTexture = content.Load<Texture2D>("imgs/white");
            lightingEffect = content.Load<Effect>("effects/lighting");
        }


        //-----------------------

        public int mousex, mousey;
        public bool mousedown;

        //=======================
        public string titlemode;
        private readonly GraphicsDevice device;
        private CImage img;
        private float time_s;
        private LightParameters lights = new LightParameters();
        private int paddleTargetWidth;
        private float ballCollisionSoundLimit;
        private readonly CSound snd;
        private readonly IContentProvider content;
        private readonly Font font;

        //----------------------


        public void initLevel(bool resetPowerups)
        {
            levelTime_ms = 0;
            stageComplete = false;
            transitionout = false;
            gameover = false;
            dying = false;

            lowBlockTimeLeft_ms = 20000;
            transcount = 0;
            paddle.ResetPosition();
            bgspeed = config.BackgroundScroll ? 50.0f : 0.0f;

            blockscrolly = 0;
            blockscrollspeedy = 0;

            paddleImbueV = basePaddleImbueV = basePaddleImbueVStart;

            paused = false;

            loadLevel(resetPowerups);

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
                Ball myball;

                for (int i = 0; i < balls.Count; i++)
                {
                    myball = balls[i];

                    if (!myball.sticking)
                    {
                        //delete balls[i];
                        balls.RemoveAt(i);

                        i--;
                    }
                    else
                    {
                        myball.stickTimeLeft_ms = 4000;
                    }

                }

                // reposition the balls so they make a nice circular wavefront when
                // they are released.  We want them to be centered, basically.
                float shiftStep = 15;
                if (balls.Count * shiftStep > paddle.Width - 10)
                    shiftStep = (paddle.Width - 10) / balls.Count;

                for (int i = 0; i < balls.Count; i++)
                {
                    myball = balls[i];

                    float shift = balls.Count / 2.0f - i;

                    myball.stickydifference = shift * shiftStep;

                    SetBallVelocityFromContact(myball);
                }

                if (balls.Count == 0)
                {
                    addBall();
                }
            }

            deleteAllPowerUps();
            deleteAllBlockParts();
            deleteAllScoreBytes();
            deleteAllFlashes();

            powtimeleft_s = 0;
            smashtimeleft_s = 0;
            fireballtimeleft_s = 0;
            catchredtimeleft_s = 0;
            catchbluetimeleft_s = 0;
            superstickytimeleft_s = 0;

            powerupcount = 0;
            ballslost = 0;

            //dropEnemy();

        }

        public void UpdateLevel(GameTime time)
        {
            img.paddle.Update(time);
            img.smallpaddle.Update(time);
            img.largepaddle.Update(time);

            paddleRenderer.Update(time);

            time_s = (float)time.ElapsedGameTime.TotalSeconds;

            if (!transitionout)
            {
                levelTime_ms += (float)time.ElapsedGameTime.TotalMilliseconds;
            }

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

            // If too many balls are around, we might get an exception on 
            // playing the colision sound. So this variable is around to make
            // sure we don't play too many collision sounds.
            ballCollisionSoundLimit -= (float)
                (2 * time.ElapsedGameTime.TotalSeconds / snd.ballscollide.Duration.TotalSeconds);
            ballCollisionSoundLimit = Math.Max(0, ballCollisionSoundLimit);

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

            Ball myball;

            for (j = 0; j < balls.Count; j++)
            {
                badBall = false;

                myball = balls[j];
                myball.update(time);


                if (myball.fireball)// && !myball.ballsticking)
                {
                    if (myball.timeToNextFade_ms < 0)
                    {
                        myball.timeToNextFade_ms = 10;

                        dropFadeBall(myball);
                    }
                }


                // doing collision checks for bricks first to determine where the ball hit it.
                if (myball.sticking == false)
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
                                PlayBallCollisionSound();
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
                                    PlayBallCollisionSound();
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
                if (myball.bally < 10 && !myball.sticking)
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
                    if (myball.bally < paddle.HitBox.Bottom
                        && !badBall
                        && !myball.sticking
                        && myball.ballvy > 0
                        && myball.bally > 300)
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

                    if (myball.ballvy > 0 || myball.bally > paddle.HitBox.Top)
                    {
                        // exponentially decaying attraction.  Not realistic, but hopefully it looks nice
                        float distx = paddle.x - myball.ballx;
                        float disty = paddle.y - myball.bally;

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

                // if the ball is sticking, make sure to update it's position and 
                // check to see if it should automagically be released.
                if (myball.sticking)
                {
                    myball.ballx = paddle.x + myball.stickydifference;
                    myball.bally = paddle.HitBox.Top - myball.ballh;
                    
                    if (myball.stickTimeLeft_ms <= 0 && !transitionout)
                    {
                        myball.sticking = false;
                        ballStickCount--;
                    }

                    if (attractMode && powerups.Count == 0)
                    {
                        if (random.NextDouble() < 0.1)
                        {
                            myball.sticking = false;
                            ballStickCount--;
                        }

                    }
                }


                // check the paddle
                bool hit_paddle = true;

                if (myball.ballx + myball.Radius < paddle.HitBox.Left) hit_paddle = false;
                else if (myball.ballx - myball.Radius > paddle.HitBox.Right) hit_paddle = false;
                else if (myball.bally + myball.Radius < paddle.HitBox.Top) hit_paddle = false;
                else if (myball.bally - myball.Radius > paddle.HitBox.Bottom) hit_paddle = false;

                if (hit_paddle)
                {
                    bool changeVY = SetBallVelocityFromContact(myball);

                    if (changeVY && !myball.sticking)
                    {
                        // check for powerups
                        if (fireball)
                        {
                            myball.fireball = true;
                        }

                        if (pow)
                        {
                            myball.Power++;
                        }

                        if (smash)
                        {
                            myball.smash = true;
                        }
                    }


                    // if you have the stick power up.....
                    if ((stickypaddle && myball.sticking == false) ||
                        (transitionout && ballStickCount == 0))
                    {
                        myball.sticking = true;
                        ballStickCount++;

                        myball.stickydifference = myball.ballx - paddlex;
                        myball.stickTimeLeft_ms = 4000;
                    }
                }



                if (badBall)
                {
                    DeleteBall(j);

                    if (!transitionout)
                    {
                        ballslost++;
                    }

                }

            }

            // update everything else
            UpdateBlocks(time);
            UpdateBlockParts(time);
            UpdateFlashes(time);
            updatePowerUps(time);
            UpdateCaughtPowerUps(time);
            UpdateScoreBytes(time);
            UpdateFadeBalls(time);

            paddle.Update(time, balls);
            //updateEnemies();


            // update bg scroll for fun... tied in with the transitioning 'system' ....
            if (transitionout)
            {
                paddle.y -= 100 * time_s;

                bgspeed += 100 * time_s;
                blockscrollspeedy += 40 * time_s;

                transcount = (int)(255 * (570 - paddle.y) / 560.0f);


                if (transcount >= 255)
                {

                    stageComplete = true;
                    paddle.y = Paddle.GamePaddleY;
                    snd.ching.Play();

                    transcount = 255;
                }
            }

            bgy += bgspeed * time_s;
            blockscrolly += blockscrollspeedy * time_s;

            CheckDeathCondition(time);

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
                diff = (int)(balls[lowestball].ballx - paddle.x);
                int vdiff = (int)(balls[lowestball].ballvx - attractvelocity);
                int sign = 0;

                // check to see if the ball is left or right of the paddle
                if (diff < -paddleWidth / 3) sign = -1;
                if (diff > paddleWidth / 3) sign = 1;

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

                if (vdiff * attractvelocity > 0 && Math.Abs((int)(diff)) < paddle.Width)
                {
                    if (vdiff > 0)
                        attractvelocity += Math.Max(vdiff, 10);
                    else
                        attractvelocity += Math.Min(vdiff, -10);
                }

            }
            else if (attractMode && lowestball > -1 && chasepowerup)
            {
                diff = (int)((powerups[lowestball].x + powerups[lowestball].w / 2) - paddle.x );
                int sign = 0;

                // check to see if the ball is left or right of the paddle
                if (diff < -paddle.Width / 3) sign = -1;
                if (diff >  paddle.Width / 3) sign = 1;

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
                paddle.x += attractvelocity * time_s;


                // pay attention to the borders
                if (paddle.CheckEdge())
                    attractvelocity = 0;


                // hey, if we've got the blaster, use it.
                if (blaster && random.NextDouble() < 0.05)
                {
                    addBall();
                }

                paddleVelocity = attractvelocity;
            }

            DropDoorIfPlayerSucks(time);

            CheckLevelCompleteCondition();
        }

        private void PlayBallCollisionSound()
        {
            if (ballCollisionSoundLimit >= 1)
                return;

            ballCollisionSoundLimit += 1;

            snd.ballscollide.Play();
        }

        private bool SetBallVelocityFromContact(Ball myball)
        {
            // the difference between the centers divided by paddle length * 2 gives number between -1 and 1... kinda
            float mydif = myball.ballx - paddlex;
            bool changeVY = false;

            // set the rotational velocity of the ball, so the surface of the ball
            // has the tangential velocity proportionate to that of the paddle.
            // negative sign because paddle is under the ball.
            myball.AngularVelocity = -0.1f * 2 * paddleVelocity / myball.ballw * 180 / (float)(Math.PI);

            changeVY = myball.ballvy > 0;

            mydif /= paddle.Width;
            mydif *= 2.0f;

            if (mydif > 0.97f) mydif = 0.97f;
            else if (mydif < -0.97f) mydif = -0.97f;

            // get the radian from asin();
            float angle = (float)Math.Asin(mydif);

            // apply new velocity....
            myball.ballvy = -paddleImbueV * (float)Math.Cos(angle);
            myball.ballvx = paddleImbueV * (float)Math.Sin(angle);
            myball.ballv = paddleImbueV;
            return changeVY;
        }

        private void UpdateCaughtPowerUps(GameTime time)
        {
            var time_s = (float)time.ElapsedGameTime.TotalSeconds;

            superstickytimeleft_s -= time_s;
            fireballtimeleft_s -= time_s;
            blastertimeleft_s -= time_s;
            catchbluetimeleft_s -= time_s;
            catchredtimeleft_s -= time_s;
            powtimeleft_s -= time_s;
            smashtimeleft_s -= time_s;

            supersticky = superstickytimeleft_s >= 0;
            fireball = fireballtimeleft_s >= 0;
            blaster = blastertimeleft_s >= 0;
            catchblue = catchbluetimeleft_s >= 0;
            pow = powtimeleft_s >= 0;
            smash = smashtimeleft_s >= 0;
        }

        private void CheckLevelCompleteCondition()
        {
            if (blocks.Count <= uncountedBlocks && !transitionout && !stageComplete)
            {
                transitionout = true;

                snd.speedup.Play();
            }
        }

        private void DropDoorIfPlayerSucks(GameTime time)
        {
            // see if we should drop a door powerup
            if (blocks.Count <= uncountedBlocks + 5 && !transitionout && !stageComplete)
            {
                lowBlockTimeLeft_ms -= (float)time.ElapsedGameTime.TotalMilliseconds;

                if (lowBlockTimeLeft_ms <= 0)
                {
                    dropPowerUp(400, 10, PowerupTypes.DOOR);
                    lowBlockTimeLeft_ms += 15000;
                }
            }
        }


        private void CheckDeathCondition(GameTime time)
        {
            deathAnimation -= (float)time.ElapsedGameTime.TotalMilliseconds;

            if (balls.Count == 0 && !transitionout)
            {
                if (paddle.Opacity == 1.0f)
                {
                    snd.die.Play();
                    dying = true;
                    paddle.y -= 100 * time_s;
                    deathAnimation = 0;
                }
                if (deathAnimation <= 0)
                {
                    paddle.Opacity -= 0.5f * time_s;
                    paddle.RotationAngle += 720.0f * time_s;
                    paddle.y -= 75 * time_s;
                    deathAnimation += 20;
                }
                if (paddle.Opacity < 0.0f)
                {
                    dying = false;
                    gameover = false;

                    freezeScore = true;
                    powerUp(PowerupTypes.RESET);
                    powerupcount--;
                    freezeScore = false;

                    paddleImbueV = basePaddleImbueV = basePaddleImbueVStart;

                    if (Lives == 0) { gameover = true; }
                    if (Lives > 0)
                    {
                        addBall();

                        if (!transitionout) Lives--;

                        paddle.ResetPosition();
                    }
                }
            }
        }

        // function for sort to sort balls by y.
        private int ballcheck(Ball a, Ball b)
        {
            return a.bally.CompareTo(b.bally);
        }


        public void DrawLevel(SpriteBatch spriteBatch)
        {
            //Display.Clear(Color.FromArgb(128, 0, 0, 128));

            // Draw the background tile, scrolled
            DrawBackground(spriteBatch, bgy);

            // Draw perimeter
            spriteBatch.Draw(img.topborder, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(img.leftborder, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(img.rightborder, new Vector2(735, 0), Color.White);

            spriteBatch.End();

            SetLightParameters();

            lightingEffect.Parameters["World"].SetValue(Matrix.Identity);
            lightingEffect.Parameters["ViewProjection"].SetValue(
                Matrix.CreateOrthographicOffCenter(
                    new Rectangle(0, 0, 800, 600), -1, 1));
            lightingEffect.CurrentTechnique = lightingEffect.Techniques["Render"];

            spriteBatch.Begin(effect: lightingEffect);

            // Draw blocks and Update their animations...
            DrawBlocks(spriteBatch);

            // we Draw the flash right on top of the block
            DrawFlashes(spriteBatch);

            // Draw all the other stuff except the balls here
            DrawBlockParts(spriteBatch);
            DrawFadeBalls(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin();

            paddleRenderer.Draw(spriteBatch);
            DrawPowerUps(spriteBatch);
            DrawScoreBytes(spriteBatch);
            DrawBalls(spriteBatch);

            DrawScore(spriteBatch);

            // Uncomment this to see the hitbox for the paddle.
            //if (!dying)
            //{
            //    FillRect(spriteBatch,
            //        paddle.HitBox, new Color(44, 44, 00, 99));
            //}

            // output debugging messages, if the debugger is attached
            // feel free to modify this to your heart's content.
            //if (debugger)
            //{
            //    font.DrawText(13, 505, "basePaddleImbueV: " + ((int)basePaddleImbueV));
            //    font.DrawText(13, 520, "paddleImbueV: " + ((int)paddleImbueV));
            //    font.DrawText(13, 550, "Balls Lost: " + (ballslost));
            //    font.DrawText(13, 565, "BallStickCount: " + (ballStickCount));
            //}

            DrawExtraLives(spriteBatch);
            DrawActivePowerUps(spriteBatch);

            // fade the screen to white if we are transitioning out
            DrawTransition(spriteBatch);
        }

        private void DrawTransition(SpriteBatch spriteBatch)
        {
            if (transitionout)
            {
                Rectangle myrect = new Rectangle(0, 0, 800, 600);
                Color mycolor = new Color(255, 255, 255, transcount);

                FillRect(spriteBatch, myrect, mycolor);

            }
        }

        private void DrawExtraLives(SpriteBatch spriteBatch)
        {

            // Draw extra lives
            img.ball.RotationAngle = 0;

            int ilives;
            for (ilives = 0; ilives < Lives; ilives++)
            {
                img.ball.Draw(spriteBatch, new Vector2(775 - (ilives * 13), 588));
            }
        }

        private void DrawScore(SpriteBatch spriteBatch)
        {

            // Draw whatever text
            string message = $"Score: {Score}";

            font.TextAlignment = OriginAlignment.BottomLeft;
            font.Size = 12;
            font.Color = Color.Black;
            font.DrawText(spriteBatch, new Vector2(11, 600), message);
            font.Color = Color.White;
            font.DrawText(spriteBatch, new Vector2(13, 598), message);
        }

        private void DrawBalls(SpriteBatch spriteBatch)
        {

            // Draw the balls
            Sprite ballimg;

            for (int j = 0; j < balls.Count; j++)
            {
                Ball myball = balls[j];

                if (myball.fireball)
                    ballimg = img.fireball;
                else
                    ballimg = img.ball;

                //ballimg.set_color(myball.color);

                int spikes = myball.Spikes;
                int ballx = (int)myball.ballx;
                int bally = (int)myball.bally;

                Vector2 ballCenter = myball.BallCenter;

                if (spikes != 0)
                {
                    int angle = 360 / spikes;

                    float x = ballCenter.X, y = ballCenter.Y;

                    //x -= 3;
                    //y -= 3;

                    img.spike.DisplayAlignment = OriginAlignment.TopLeft;
                    img.spike.SetRotationCenter(OriginAlignment.Center);

                    for (int k = 0; k < spikes; k++)
                    {
                        img.spike.RotationAngleDegrees = myball.Angle + angle * k;
                        img.spike.Draw(spriteBatch, new Vector2(x, y));
                    }
                }

                ballimg.DisplayAlignment = OriginAlignment.Center;
                ballimg.SetRotationCenter(OriginAlignment.Center);
                ballimg.Draw(spriteBatch, ballCenter);

                if (myball.smash)
                {
                    float offset = myball.ballw / 2 - img.smash.SpriteWidth / 2;

                    img.smash.DisplayAlignment = OriginAlignment.Center;
                    img.smash.SetRotationCenter(OriginAlignment.Center);
                    img.smash.RotationAngleDegrees = myball.SmashAngle;
                    img.smash.Draw(spriteBatch, ballCenter);
                }

                // Uncomment to see hitboxes for balls
                //FillRect(spriteBatch, myball.HitBox, new Color(Color.Red, 128));
            }
        }

        public void FillRect(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            Color premulColor = color * (color.A / 255.0f);
            premulColor.A = color.A;

            spriteBatch.Draw(whiteTexture, rectangle, premulColor);
        }

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

        private void SetLightParameters()
        {
            lights.Clear();
            lights.AmbientLightColor = worlds[world].light;

            Color fireballColor = Color.Yellow;
            Color ballColor = new Color(200, 200, 200);

            for (int i = 0; i < LightParameters.MaxLights && i < balls.Count; i++)
            {
                var ball = balls[i];

                lights.SetLightEnable(i, true);
                lights.SetLightPosition(i, new Vector3(ball.BallCenter, -1));

                if (ball.fireball)
                {
                    lights.SetLightColor(i, fireballColor);
                    lights.SetAttenuation(i, new Vector3(0.01f, 0.006f, 0.00018f));
                }
                else
                {
                    lights.SetLightColor(i, ballColor);
                    lights.SetAttenuation(i, new Vector3(0.01f, 0, 0.00025f));
                }
            }

            lights.ApplyTo(lightingEffect);
        }


        public void DeleteBall(int myball)
        {
            if (balls[myball].sticking)
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
        private void UpdateBlockParts(GameTime time)
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

        private void UpdateFlashes(GameTime time)
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

        private Ball addBall(Ball otherball)
        {
            if (balls.Count >= ballLimit)
                return null;

            Ball myBall = new Ball(otherball);

            balls.Add(myBall);

            return myBall;

        }

        private Ball addBall(float offset = 0)
        {
            if (balls.Count >= ballLimit)
                return null;

            Ball myball = new Ball();

            myball.stickydifference = offset;
            myball.bally = paddle.HitBox.Top - myball.ballw;
            myball.ballx = paddle.x + offset;

            SetBallVelocityFromContact(myball);

            myball.fireball = fireball;
            myball.smash = smash;

            if (pow)
                myball.Power++;

            ballStickCount++;


            balls.Add(myball);

            return myball;

        }

        private void deleteAllBalls()
        {

            while (balls.Count > 0)
                DeleteBall(0);

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
            ApplyPowerUp(powerup, 0);
        }

        private void ApplyPowerUp(CPowerUp powerup, int extraPoints)
        {
            int scoreGain = 0;
            PowerupTypes effect = powerup.getEffect();

            powerupcount++;

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
                    Lives++;
                    scoreGain = 50;

                    break;

                case PowerupTypes.STICKY:

                    scoreGain = 50;
                    stickypaddle = true;

                    break;

                case PowerupTypes.SUPERSTICKY:

                    scoreGain = 50;
                    supersticky = true;

                    superstickytimeleft_s = 30;
                    break;

                case PowerupTypes.MULTIBALL:

                    addBall(6);
                    addBall(-6);
                    addBall(18);
                    addBall(-18);

                    scoreGain = 50;

                    break;

                case PowerupTypes.PU3BALL:

                    SplitBalls();

                    scoreGain = 50;

                    break;

                case PowerupTypes.RBSWAP:

                    if (paddleImbueV != basePaddleImbueV)
                    {
                        paddleImbueV = basePaddleImbueV - (paddleImbueV - basePaddleImbueV);
                    }

                    paddle.PaddleSizeIndex = -paddle.PaddleSizeIndex;

                    BBUtility.SWAP(ref catchblue, ref catchred);
                    BBUtility.SWAP(ref catchbluetimeleft_s, ref catchredtimeleft_s);

                    scoreGain = 50;

                    break;

                case PowerupTypes.BLASTER:

                    blaster = true;
                    blastertimeleft_s = 10;

                    scoreGain = 50;

                    addBall();

                    break;

                case PowerupTypes.FIREBALL:

                    fireball = true;
                    fireballtimeleft_s = 10;

                    scoreGain = 50;
                    {
                        for (int i = 0; i < balls.Count; i++)
                        {
                            if (balls[i].sticking)
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

                    if (paddle.PaddleSizeIndex < 0)
                        paddle.PaddleSizeIndex++;

                    paddle.PaddleSizeIndex ++;

                    break;

                case PowerupTypes.SMALLPADDLE:

                    scoreGain = 500;

                    if (paddle.PaddleSizeIndex > 0)
                        paddle.PaddleSizeIndex--;

                    paddle.PaddleSizeIndex--;

                    break;

                case PowerupTypes.REGULARPADDLE:

                    scoreGain = 50;
                    paddle.PaddleSizeIndex = 0;

                    break;

                case PowerupTypes.CATCHBLUE:

                    scoreGain = 50;
                    catchblue = true;
                    catchbluetimeleft_s = 30;

                    break;

                case PowerupTypes.CATCHRED:

                    scoreGain = 50;
                    catchred = true;
                    catchredtimeleft_s = 30;

                    break;

                case PowerupTypes.POW:

                    scoreGain = 50;
                    pow = true;
                    powtimeleft_s = 10;
                    {
                        for (int i = 0; i < balls.Count; i++)
                        {
                            if (balls[i].sticking)
                                balls[i].Power++;
                        }
                    }

                    break;

                case PowerupTypes.SMASH:

                    scoreGain = 50;
                    smash = true;
                    smashtimeleft_s = 10;
                    {
                        for (int i = 0; i < balls.Count; i++)
                        {
                            if (balls[i].sticking)
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

                    scoreGain = TouchResetPowerup();

                    break;

                case PowerupTypes.RANDOM:

                    TouchMysteryPowerup(powerup);

                    // We can just return from here, since TouchMysteryPowerup
                    // awards the score.

                    break;
            }

            // don't let the point gain drop below zero
            if (scoreGain < 0)
                scoreGain = 0;

            scoreGain += extraPoints;

            // gain the actual points
            GainPoints(scoreGain, (int)(powerup.x + 20), (int)(powerup.y + 20));

        }

        private int TouchResetPowerup()
        {
            int scoreGain = 50;

            // adjust the amount of points gained for whether we are
            // losing "good" or "bad" powerups
            if (blaster) scoreGain += 25;
            if (fireball) scoreGain += 25;
            if (paddleWidth < 100) scoreGain -= 25;
            if (paddleWidth > 100) scoreGain += 25;
            if (paddleImbueV > basePaddleImbueV + 25) scoreGain -= 25;
            if (paddleImbueV < basePaddleImbueV - 25) scoreGain += 25;
            if (stickypaddle) scoreGain += 25;
            if (supersticky) scoreGain += 25;
            if (catchblue) scoreGain += 25;
            if (catchred) scoreGain -= 25;
            if (pow) scoreGain += 25;
            if (smash) scoreGain += 25;

            if (scoreGain < 1) scoreGain = 1;

            paddle.PaddleSizeIndex = 0;

            catchblue = catchred = stickypaddle = supersticky = blaster = fireball = false;
            pow = smash = false;
            paddleImbueV = basePaddleImbueV;
            {
                int i;
                for (i = 0; i < balls.Count; i++)
                {
                    Ball myball = balls[i];

                    if (myball.sticking)
                    {
                        myball.fireball = false;
                        myball.Power = 0;
                        myball.smash = false;
                    }
                }
            }

            return scoreGain;
        }

        private void TouchMysteryPowerup(CPowerUp powerup)
        {
            // this is worth 50 extra points, but we pass it to the actual powerup
            // so that the scorebyte that's displayed on the screen is the right
            // number.
            int scoreGain = 0;

            // declare some local variables, so enclose in braces to 
            // avoid stupid warnings.
            {
                CPowerUpList list = new CPowerUpList();
                CPowerUp actual;

                // build the list of available powerups.
                BuildPowerUpList(list);
                list.removeEffect(PowerupTypes.RANDOM);
                list.removeEffect(PowerupTypes.DOOR);
                list.removeEffect(PowerupTypes.RESET);

                powerupcount--;

                // choose one of them
                list.AssignPowerup(out actual, 0, 0, random);

                // execute it
                ApplyPowerUp(actual, 50);

                // change my icon:
                powerup.icon = actual.icon;
            }
        }

        private void SplitBalls()
        {
            int end = balls.Count;

            for (int i = 0; i < end; i++)
            {
                Ball myball = balls[i];
                Ball ball1, ball2;

                if (myball.sticking)
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

                ball1.sticking = false;
                ball2.sticking = false;

            }
        }

        private void BuildPowerUpList(CPowerUpList pulist)
        {
            // erase whatever's in the list now
            pulist.clear();

            int shortenWidthBoost = 10 * paddle.PaddleSizeIndex;

            // only list the powerups in the list that will actually have an effect
            if (paddleWidth < 200) pulist.addEffect(PowerupTypes.LARGEPADDLE, GetPUIcon(PowerupTypes.LARGEPADDLE), 100 - shortenWidthBoost);
            if (paddleWidth > 90) pulist.addEffect(PowerupTypes.SMALLPADDLE, GetPUIcon(PowerupTypes.SMALLPADDLE), 50 + shortenWidthBoost);
            if (!blaster) pulist.addEffect(PowerupTypes.BLASTER, GetPUIcon(PowerupTypes.BLASTER), 10);
            if (paddle.ImbueV <= 800) pulist.addEffect(PowerupTypes.FASTBALL, GetPUIcon(PowerupTypes.FASTBALL), 50);
            if (!fireball) pulist.addEffect(PowerupTypes.FIREBALL, GetPUIcon(PowerupTypes.FIREBALL), 10);
            if (paddle.PaddleSizeIndex != 0) pulist.addEffect(PowerupTypes.REGULARPADDLE, GetPUIcon(PowerupTypes.REGULARPADDLE), 20);
            if (basePaddleImbueV > paddleImbueV + 25 ||
                basePaddleImbueV < paddleImbueV - 25)
                pulist.addEffect(PowerupTypes.NORMALSPEED, GetPUIcon(PowerupTypes.NORMALSPEED), 30);
            if (paddleImbueV >= 200) pulist.addEffect(PowerupTypes.SLOWBALL, GetPUIcon(PowerupTypes.SLOWBALL), 30);

            if (!stickypaddle) pulist.addEffect(PowerupTypes.STICKY, GetPUIcon(PowerupTypes.STICKY), 20);
            if (stickypaddle) pulist.addEffect(PowerupTypes.SUPERSTICKY, GetPUIcon(PowerupTypes.SUPERSTICKY), 20);

            if (!catchblue) pulist.addEffect(PowerupTypes.CATCHBLUE, GetPUIcon(PowerupTypes.CATCHBLUE), 10);
            if (!catchred) pulist.addEffect(PowerupTypes.CATCHRED, GetPUIcon(PowerupTypes.CATCHRED), 20);

            if (balls.Count < 30)
            {
                int multiBallWeightBoost = (int)(150 * (1 - Math.Tanh(balls.Count / 2)));

                // Pick a weight so that 3 ball is real common when you only have one ball.
                pulist.addEffect(PowerupTypes.PU3BALL, GetPUIcon(PowerupTypes.PU3BALL), 30 + multiBallWeightBoost);
            }

            if (balls.Count < ballLimit - 4)
            {
                int multiBallWeightBoost = (int)(150 * (1 - Math.Tanh(balls.Count / 6)));

                pulist.addEffect(PowerupTypes.MULTIBALL, GetPUIcon(PowerupTypes.MULTIBALL), 20 + multiBallWeightBoost);
            }

            // these should always be available for dropping under any conditions.
            pulist.addEffect(PowerupTypes.RESET, GetPUIcon(PowerupTypes.RESET), 20);
            pulist.addEffect(PowerupTypes.ONEUP, GetPUIcon(PowerupTypes.ONEUP), 3);
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

        private void UpdateScoreBytes(GameTime time)
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
            int set;

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
                CPowerUpList pulist = new CPowerUpList();

                BuildPowerUpList(pulist);

                pulist.AssignPowerup(out ppowerup, myx, myy, random);
            }

            powerups.Add(ppowerup);
        }

        private void updatePowerUps(GameTime time)
        {
            if (powerups.Count > 0)
            {
                int i;

                for (i = 0; i < powerups.Count; i++)
                {
                    CPowerUp mypowerup = powerups[i];

                    bool hitpaddle = paddle.HitBox.Intersects(mypowerup.HitBox);

                    if (mypowerup.getEffect() == PowerupTypes.PU_NONE)
                        hitpaddle = false;

                    if (dying)
                        hitpaddle = false;

                    bool powerupAlive = mypowerup.update(time);

                    if (mypowerup.y > 600)
                    {
                        if (catchblue && mypowerup.IsBlue)
                            hitpaddle = true;
                        else if (catchred && mypowerup.IsRed)
                            hitpaddle = true;
                        else
                            powerupAlive = false;
                    }

                    if (hitpaddle)
                    {
                        powerUp(mypowerup);
                        mypowerup.setEffect(PowerupTypes.PU_NONE);
                        mypowerup.vx = paddleVelocity / 10;

                        snd.powerup.Play();
                    }

                    if (!powerupAlive)
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
            }
        }

        private void GainPoints(int pts, int x, int y)
        {
            if (freezeScore)
                return;

            GainPoints(pts);

            if (x > 0)
            {
                dropScoreByte(x, y, pts);
            }
        }

        public void GainPoints(int pts)
        {
            if (freezeScore)
                return;

            thescore += pts;
        }

        private void dropFadeBall(Ball myball)
        {
            if (fadeBalls.Count > ballLimit)
                return;

            CFadeBall fb = new CFadeBall(myball, random);

            fadeBalls.Add(fb);
        }

        private void UpdateFadeBalls(GameTime time)
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
                img.fireball.DisplayAlignment = OriginAlignment.Center;
                img.fireball.SetRotationCenter(OriginAlignment.Center);
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

        private void hitBlock(int myblock, Ball myball)
        {
            int blockx, blocky;

            blockx = (int)blocks[myblock].getx();
            blocky = (int)blocks[myblock].gety();


            // decrement the strength of the block
            blocks[myblock].decreaseStr(myball.Damage);

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

                    int damage = myball.Damage - (int)(distance) * 30;

                    if (damage > 0)
                    {
                        other.decreaseStr(damage);
                        checkBlock(i, myball, false);
                    }

                }
            }
        }

        public int Score => thescore;

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

        private void checkBlock(int myblock, Ball myball, bool playSound = true)
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

                    if (paddleWidth > 110)
                        scoreGain -= 2;
                    else if (paddleWidth < 90)
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
                    if (paddleWidth > 110) scoreGain -= 1;
                    if (paddleWidth < 90) scoreGain += 1;
                    if (blaster) scoreGain = 0;

                    if (scoreGain < 0) scoreGain = 0;
                }

                // if the block didn't get destroyed, light it up :) and play sound
                dropFlash(blocks[myblock].getx(), blocks[myblock].gety());

                if (playSound)
                    snd.bounce.Play();

            }

            // woot!
            GainPoints(scoreGain, blockx + 20, blocky + 10);

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

            paddleVelocity = (mousex - paddle.x) / time_s;

            paddle.x = mousex;
            paddle.CheckEdge();

            mousex = (int)(paddle.x + 0.5f);
            mousey = 400;

            Mouse.SetPosition(mousex, mousey);
        }

        public void MouseClick()
        {
            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i].sticking)
                {
                    balls[i].sticking = false;
                    //balls[i].bally -= blockscrolly;	// correct for paddle moving up
                    ballStickCount--;
                }
            }

            if (blaster && !dying)
            {
                addBall(random.Next(-5, 3));
            }
            return;
        }

        public void SelfDestruct()
        {
            while (balls.Count > 0)
                DeleteBall(0);
        }

        private void DrawActivePowerUps(SpriteBatch spriteBatch)
        {
            powerupLeft = 755;
            powerupTop = 535;

            if (paddleImbueV > basePaddleImbueV + 25)
                DrawActivePowerUp(spriteBatch, img.pufastball);
            else if (paddleImbueV < basePaddleImbueV - 25)
                DrawActivePowerUp(spriteBatch, img.puslowball);
            else
                DrawActivePowerUp(spriteBatch, img.puregularspeed);

            if (paddle.PaddleSizeIndex > 0)
                DrawActivePowerUp(spriteBatch, img.pupaddlelarge, $"{paddle.PaddleSizeIndex}", OriginAlignment.BottomRight);
            else if (paddle.PaddleSizeIndex < 0)
                DrawActivePowerUp(spriteBatch, img.pupaddlesmall, $"{-paddle.PaddleSizeIndex}", OriginAlignment.BottomRight);
            else
                DrawActivePowerUp(spriteBatch, img.pupaddleregular);

            if (supersticky)
            {
                DrawActivePowerUp(spriteBatch, img.pusupersticky, RemainingText(superstickytimeleft_s));
            }
            else if (stickypaddle)
            {
                DrawActivePowerUp(spriteBatch, img.pusticky);
            }

            if (blaster)
            {
                DrawActivePowerUp(spriteBatch, img.publaster, RemainingText(blastertimeleft_s));

                font.Size = 24;

                // Draw flashing CLICK! above paddle
                if (levelTime_ms % 200 < 100)
                    font.Color = Color.Red;
                else
                    font.Color = Color.White;

                font.TextAlignment = OriginAlignment.TopCenter;

                font.DrawText(spriteBatch,
                    new Vector2(400 + levelTime_ms % 3 - 2,
                                paddle.y - 100 + levelTime_ms % 3 - 2),
                    "Click!");

                font.TextAlignment = OriginAlignment.TopLeft;

                font.Color = Color.White;
            }

            if (fireball)
            {
                DrawActivePowerUp(spriteBatch, img.pufireball, RemainingText(fireballtimeleft_s));
            }

            if (catchblue)
            {
                DrawActivePowerUp(spriteBatch, img.pucatchblue, RemainingText(catchbluetimeleft_s));
            }

            if (catchred)
            {
                DrawActivePowerUp(spriteBatch, img.pucatchred, RemainingText(catchredtimeleft_s));

                if (catchredtimeleft_s < 0)
                {
                    // well, se used up the whole thing.  gain another 250 points.
                    catchred = false;

                    GainPoints(250, powerupLeft + 20, powerupTop + 20);
                    snd.powerup.Play();
                }

            }


            if (pow)
            {
                DrawActivePowerUp(spriteBatch, img.pupow, RemainingText(powtimeleft_s));
            }

            if (smash)
            {
                DrawActivePowerUp(spriteBatch, img.pusmash, RemainingText(smashtimeleft_s));
            }
        }

        private void DrawActivePowerUp(SpriteBatch spriteBatch, Sprite sprite)
        {
            DrawActivePowerUp(spriteBatch, sprite, "", OriginAlignment.TopCenter);
        }

        private void DrawActivePowerUp(SpriteBatch spriteBatch, Sprite image, string text, OriginAlignment textAlignment = OriginAlignment.TopCenter)
        {
            image.Alpha = (1.0f);
            image.Scale = new Vector2(1.0f, 1.0f);
            image.Draw(spriteBatch, new Vector2(powerupLeft, powerupTop));

            if (!string.IsNullOrWhiteSpace(text))
            {
                Size size = font.MeasureString(text);

                Rectangle targetRect = new Rectangle(powerupLeft + 3, powerupTop + 3, 34, 34);
                var dest = textAlignment.OnRect(targetRect);

                font.TextAlignment = textAlignment;
                font.Size = 10;

                font.Color = Color.Black;
                font.DrawText(spriteBatch, dest.X,     dest.Y - 1, text);
                font.DrawText(spriteBatch, dest.X,     dest.Y + 1, text);
                font.DrawText(spriteBatch, dest.X + 1, dest.Y, text);
                font.DrawText(spriteBatch, dest.X - 1, dest.Y, text);

                font.Color = Color.White;
                font.DrawText(spriteBatch, dest.X, dest.Y, text);
            }

            powerupTop -= 45;
        }

        private static string RemainingText(float timeRemaining_s)
        {
            TimeSpan ts = TimeSpan.FromSeconds(timeRemaining_s);

            string time = $"{ts.Minutes}:{ts.Seconds:00}";
            return time;
        }
    }
}
