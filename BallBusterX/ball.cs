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
            this.X = this.Y = -10;
            this.VelocityX = 50.0f;
            this.VelocityY = -120.0f;
            this.Velocity = 130.0f;
            this.Angle = 0;
            this.AngularVelocity = 45;

            this.IsSticking = true;
            this.StickyDifference = 50;
            this.StickTimeLeft_ms = 4000;

            HasFireball = false;
            HasSmash = false;
        }

        public Ball(Ball ball)
        {
            this.X = ball.X;
            this.Y = ball.Y;
            this.VelocityX = ball.VelocityX;
            this.VelocityY = ball.VelocityY;
            this.Velocity = ball.Velocity;
            this.Angle = ball.Angle;
            this.AngularVelocity = ball.AngularVelocity;

            this.HasFireball = ball.HasFireball;
            this.power = ball.power;
            this.HasSmash = ball.HasSmash;

            this.IsSticking = ball.IsSticking;
            this.StickyDifference = ball.StickyDifference;
            this.StickTimeLeft_ms = ball.StickTimeLeft_ms;
        }

        public int Damage => baseDamage + power * powDamageIncrement;

        public float Width => 10;
        public float Height => 10;
        public float Radius => 5;

        public float X, Y;
        public float VelocityX, VelocityY, Velocity;
        public float SmashAngle;
        public float SmashAngleV = 1200;

        public Vector2 BallCenter => new Vector2(X + Width / 2, Y + Height / 2);

        public float Angle { get; set; }

        public float AngularVelocity { get; set; }

        public double timeToNextFade_ms;

        public bool HasFireball;
        public bool HasSmash;


        public bool IsSticking;
        public float StickyDifference;
        public double StickTimeLeft_ms;

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

        public int Spikes
        {
            get
            {
                if (power == 0) return 0;
                return power + 2;
            }
        }

        public Rectangle HitBox => new Rectangle((int)X, (int)Y, (int)Width, (int)Height);

        /// <summary>
        /// checks to make sure balls velocity is ballv
        /// </summary>
        /// <returns></returns>
        public bool CheckVelocity()
        {
            float velmag = (float)Math.Sqrt(VelocityX * VelocityX + VelocityY * VelocityY);
            float velscale = Velocity / velmag;

            if (velmag == Velocity)
                return false;

            VelocityX *= velscale;
            VelocityY *= velscale;

            return true;

        }

        public void Update(GameTime time)
        {
            float time_s = (float)time.ElapsedGameTime.TotalSeconds;
            float time_ms = time_s * 1000;

            StickTimeLeft_ms -= time_ms;
            timeToNextFade_ms -= time_ms;

            if (!IsSticking)
                Angle += AngularVelocity * time_s;

            if (Angle > 360) Angle -= 360.0f;
            if (Angle < -360) Angle += 360.0f;

            SmashAngle += SmashAngleV * time_s;

            if (SmashAngle > 360) SmashAngle -= 360;

            if (Math.Abs(VelocityY) < 40)
            {
                if (VelocityY == 0) VelocityY = 0.0001f;
                VelocityY += Math.Sign(VelocityY) * 25 * time_s;
            }
            if (Math.Abs(VelocityX) < 40)
            {
                if (VelocityX == 0) VelocityX = 0.0001f;
                VelocityX += Math.Sign(VelocityX) * 25 * time_s;
            }
        }

        public bool CollideWith(Ball otherBall)
        {
            // obviously, we don't want to collide with ourself
            if (otherBall == this)
                return false;

            float displacementSqr;                  // square of the distance between the two balls
            float displacementX, displacementY;     // two component displacement vector

            // get the displacement vector
            displacementX = otherBall.X - X;
            displacementY = otherBall.Y - Y;

            // just in case they happen to be at the exact same spot
            if (displacementX == 0 && displacementY == 0)
                displacementX = 1;

            displacementSqr = displacementX * displacementX + displacementY * displacementY;


            // don't collide with balls that are stuck if we are moving up
            if (IsSticking && otherBall.VelocityY < 0)
                return false;
            else if (VelocityY < 0 && otherBall.IsSticking)
                return false;

            // check to see if actual collision
            else if (displacementSqr < Width * Width && displacementSqr > 1)
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
                thisVParallel = displacementX * VelocityX + displacementY * VelocityY;
                otherVParallel = displacementX * otherBall.VelocityX + displacementY * otherBall.VelocityY;

                // same for the perpendicular velocity
                thisVPerp = perpX * VelocityX + perpY * VelocityY;
                otherVPerp = perpX * otherBall.VelocityX + perpY * otherBall.VelocityY;

                // now swap the parallel components.
                float t;

                t = thisVParallel;
                thisVParallel = otherVParallel;
                otherVParallel = t;

                // reconstruct the actual velocity vector
                VelocityX = thisVParallel * displacementX + thisVPerp * perpX;
                VelocityY = thisVParallel * displacementY + thisVPerp * perpY;

                otherBall.VelocityX = otherVParallel * displacementX + otherVPerp * perpX;
                otherBall.VelocityY = otherVParallel * displacementY + otherVPerp * perpY;

                // now make sure the balls aren't touching
                otherBall.X = X + displacementX * Width;
                otherBall.Y = Y + displacementY * Height;

                CheckVelocity();
                otherBall.CheckVelocity();

                return true;
            }
            else
            {
                return false;
            }
        }

    }
}