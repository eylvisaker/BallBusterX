/*****************************************************************************
	Ball: Buster
	Copyright (C) 2004-9 Patrick Avella, Erik Ylvisaker

    This file is part of Ball: Buster.

    Ball: Buster is free software; you can redistribute it and/or modify
    it under the terms of the GNU General public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    Ball: Buster is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General public License for more details.

    You should have received a copy of the GNU General public License
    along with Ball: Buster; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using Microsoft.Xna.Framework;
using System;

namespace BallBusterX
{
    public class CBall
    {
        private const int maxSpikes = 5;
        public const int powIncrement = 40;
        public const int maxDamage = 100 + powIncrement * (maxSpikes - 1);

        /// <summary>
        /// 
        /// </summary>
        public CBall()
        {
            this.ballx = this.bally = -10;
            this.ballh = this.ballw = 10;
            this.ballvx = 50.0f;
            this.ballvy = -120.0f;
            this.ballv = 130.0f;
            this.ballangle = 0;
            this.ballvrot = 45;

            this.ballsticking = true;
            this.stickydifference = 50;
            this.ballsticktimeleft = 4000;

            fireball = false;
            smash = false;

            damage = 100;

            setDamage(damage);
        }

        public CBall(CBall ball)
        {
            this.ballx = ball.ballx;
            this.bally = ball.bally;
            this.ballh = ball.ballh;
            this.ballw = ball.ballw;
            this.ballvx = ball.ballvx;
            this.ballvy = ball.ballvy;
            this.ballv = ball.ballv;
            this.ballangle = ball.ballangle;
            this.ballvrot = ball.ballvrot;

            this.fireball = ball.fireball;

            this.ballsticking = ball.ballsticking;
            this.stickydifference = ball.stickydifference;
            this.ballsticktimeleft = ball.ballsticktimeleft;

            smash = ball.smash;
            damage = ball.damage;

            setDamage(damage);
        }

        public float ballx, bally, ballw, ballh;
        public float ballvx, ballvy, ballv; // velocity
        public float SmashAngle;
        public float SmashAngleV = 1200;

        private float ballangle; // rotational angle

        public float Ballangle
        {
            get { return ballangle; }
            set
            {
                ballangle = value;
            }
        }

        private float ballvrot; // rotational velocity

        public float Ballvrot
        {
            get { return ballvrot; }
            set
            {
                ballvrot = value;
            }
        }

        public double timetonextfade;

        public int damage;  // damage done by the ball when it hits something

        public bool fireball;
        public bool smash;


        public bool ballsticking;
        public float stickydifference;
        public double ballsticktimeleft;

        public Color color;


        public int spikes
        {
            get
            {
                int i = (damage - 100) / powIncrement;

                // make it so just one spike won't show up.  it jumps to two above zero.
                if (i > 0)
                    i++;

                return i;
            }
        }

        /// <summary>
        /// checks to make sure balls velocity is ballv
        /// </summary>
        /// <returns></returns>
        public bool checkVelocity()
        {
            float velmag = (float)Math.Sqrt(ballvx * ballvx + ballvy * ballvy);
            float velscale = ballv / velmag;

            if (velmag == ballv)
                return false;

            ballvx *= velscale;
            ballvy *= velscale;

            return true;

        }
        public void setDamage(int val)
        {
            if (val > maxDamage)
                val = maxDamage;

            damage = val;

            int alpha = 255 - (damage - 100);

            color = new Color(255, 255, 255, alpha);
        }

        public void update(GameTime time)
        {
            float time_s = (float)time.ElapsedGameTime.TotalSeconds;

            ballsticktimeleft -= time_s;
            timetonextfade -= time_s;

            if (!ballsticking)
                Ballangle += ballvrot * time_s;

            if (ballangle > 360) Ballangle -= 360.0f;
            if (ballangle < -360) ballangle += 360.0f;

            SmashAngle += SmashAngleV * time_s;

            if (SmashAngle > 360) SmashAngle -= 360;
        }

        public bool collideWith(CBall otherBall)
        {
            // obviously, we don't want to collide with ourself
            if (otherBall == this)
                return false;

            float displacementSqr;                  // square of the distance between the two balls
            float displacementX, displacementY;     // two component displacement vector

            // get the displacement vector
            displacementX = otherBall.ballx - ballx;
            displacementY = otherBall.bally - bally;

            // just in case they happen to be at the exact same spot
            if (displacementX == 0 && displacementY == 0)
                displacementX = 1;

            displacementSqr = displacementX * displacementX + displacementY * displacementY;


            // don't collide with balls that are stuck if we are moving up
            if (ballsticking && otherBall.ballvy < 0)
                return false;
            else if (ballvy < 0 && otherBall.ballsticking)
                return false;

            // check to see if actual collision
            else if (displacementSqr < ballw * ballw && displacementSqr > 1)
            {
                // yep, we collided.  Now we want to calculate to collision.
                // the algorithm is: the balls exchange velocities in the direction of the displacement
                // and velocity perpendicular to that remains unchanged.

                // First we want to normalize the displacement vector.
                displacementSqr = (float)Math.Sqrt(displacementSqr);

                displacementX /= displacementSqr;
                displacementY /= displacementSqr;

                float perpX, perpY;     // vector that is perpendicular to the displacement

                perpX = displacementY;      // simple 2-D algorithm for a perpendicular vector:
                perpY = -displacementX;     // swap the two components and change the sign of one.

                // calculate the components of the velocities in these two directions.
                float thisVParallel, thisVPerp;
                float otherVParallel, otherVPerp;

                // take the scalar product of the normalized displacement vector with the velocity;
                // this gives me the projection of the velocity onto the displacement.
                thisVParallel = displacementX * ballvx + displacementY * ballvy;
                otherVParallel = displacementX * otherBall.ballvx + displacementY * otherBall.ballvy;

                // same for the perpendicular velocity
                thisVPerp = perpX * ballvx + perpY * ballvy;
                otherVPerp = perpX * otherBall.ballvx + perpY * otherBall.ballvy;

                // now swap the parallel components.
                float t;

                t = thisVParallel;
                thisVParallel = otherVParallel;
                otherVParallel = t;

                // reconstruct the actual velocity vector
                ballvx = thisVParallel * displacementX + thisVPerp * perpX;
                ballvy = thisVParallel * displacementY + thisVPerp * perpY;

                otherBall.ballvx = otherVParallel * displacementX + otherVPerp * perpX;
                otherBall.ballvy = otherVParallel * displacementY + otherVPerp * perpY;

                // now make sure the balls aren't touching
                otherBall.ballx = ballx + displacementX * ballw;
                otherBall.bally = bally + displacementY * ballh;

                checkVelocity();
                otherBall.checkVelocity();

                return true;
            }
            else
            {
                return false;
            }
        }

    }
}