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
    public class Ball
    {
        private const int powDamageIncrement = 40;
        private const int baseDamage = 100;

        private int power = 0;
        private const int maxPower = 4;

        /// <summary>
        /// 
        /// </summary>
        public Ball()
        {
            this.ballx = this.bally = -10;
            this.ballvx = 50.0f;
            this.ballvy = -120.0f;
            this.ballv = 130.0f;
            this.Angle = 0;
            this.AngularVelocity = 45;

            this.sticking = true;
            this.stickydifference = 50;
            this.stickTimeLeft_ms = 4000;

            fireball = false;
            smash = false;
        }

        public Ball(Ball ball)
        {
            this.ballx = ball.ballx;
            this.bally = ball.bally;
            this.ballvx = ball.ballvx;
            this.ballvy = ball.ballvy;
            this.ballv = ball.ballv;
            this.Angle = ball.Angle;
            this.AngularVelocity = ball.AngularVelocity;

            this.fireball = ball.fireball;
            this.power = ball.power;
            this.smash = ball.smash;

            this.sticking = ball.sticking;
            this.stickydifference = ball.stickydifference;
            this.stickTimeLeft_ms = ball.stickTimeLeft_ms;
        }

        public int Damage => baseDamage + power * powDamageIncrement;

        public float ballw => 10;
        public float ballh => 10;
        public float Radius => 5;

        public float ballx, bally;
        public float ballvx, ballvy, ballv; // velocity
        public float SmashAngle;
        public float SmashAngleV = 1200;

        public Vector2 BallCenter => new Vector2(ballx + ballw / 2, bally + ballh / 2);

        public float Angle { get; set; }

        public float AngularVelocity { get; set; }

        public double timeToNextFade_ms;

        public bool fireball;
        public bool smash;


        public bool sticking;
        public float stickydifference;
        public double stickTimeLeft_ms;

        /// <summary>
        /// Gets the number of damage powerups this ball has.
        /// 
        /// </summary>
        public int Power
        {
            get => power;
            set
            {
                power = value;

                if (power < 0) power = 0;
                if (power > maxPower) power = maxPower;
            }
        }

        public int spikes
        {
            get
            {
                if (power == 0) return 0;
                return power + 2;
            }
        }

        public Rectangle HitBox => new Rectangle((int)ballx, (int)bally, (int)ballw, (int)ballh);

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

        public void update(GameTime time)
        {
            float time_s = (float)time.ElapsedGameTime.TotalSeconds;
            float time_ms = time_s * 1000;

            stickTimeLeft_ms -= time_ms;
            timeToNextFade_ms -= time_ms;

            if (!sticking)
                Angle += AngularVelocity * time_s;

            if (Angle > 360) Angle -= 360.0f;
            if (Angle < -360) Angle += 360.0f;

            SmashAngle += SmashAngleV * time_s;

            if (SmashAngle > 360) SmashAngle -= 360;

            if (Math.Abs(ballvy) < 40)
            {
                if (ballvy == 0) ballvy = 0.0001f;
                ballvy += Math.Sign(ballvy) * 25 * time_s;
            }
            if (Math.Abs(ballvx) < 40)
            {
                if (ballvx == 0) ballvx = 0.0001f;
                ballvx += Math.Sign(ballvx) * 25 * time_s;
            }
        }

        public bool collideWith(Ball otherBall)
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
            if (sticking && otherBall.ballvy < 0)
                return false;
            else if (ballvy < 0 && otherBall.sticking)
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