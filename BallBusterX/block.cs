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

using Microsoft.Xna.Framework;
using System;

namespace BallBusterX
{
    public class CBlock
    {
        private int str;
        private int originalStr;

        private double liveTime;

        public float x, y, w, h;

        public enum BlockType
        {
            Glass,
            Wood,
            Stone,
            Invincible,
            Ruby,

            Invalid = -1,
        };

        public Sprite block;

        /// <summary>
        /// 
        /// </summary>
		public CBlock(Random random)
        {
            x = y = 0.0f;
            w = 40.0f;
            h = 20.0f;

            this.str = 100;
            this.block = null;

            mBlockType = BlockType.Invalid;

            this.random = random;
            flipcrack = false;

            offsety = 0;

            shaking = false;

        }
        public bool collision(float myx, float myy, float myw, float myh)
        {

            if (myx + myw < x) return false;
            if (myx > x + w) return false;
            if (myy + myh < y + offsety) return false;
            if (myy > y + h + offsety) return false;

            return true;

        }

        // "Color" of block... the value read from the input file for this block.
        public char color;

        public int getStr() { return str; }
        public void setStr(int strength) { originalStr = str = strength; }

        public void decreaseStr(int amount) { str -= amount; }

        public void setCoords(float myx, float myy) { x = myx; y = myy; }

        public BlockType mBlockType;

        public Color clr;
        public bool flipcrack;

        public int animShift;
        private readonly Random random;
        public float offsety;


        public bool shaking;
        private int frame;
        public float shakeTimeLeft;

        public float crackPercentage()
        {
            // I want a function that is linear, and it returns 
            //		0 when str = originalStr
            //		1 when str = 50
            // varies linearly with str in between
            var retVal = (originalStr - 50 - str) / (float)originalStr;

            if (retVal < 0)
                retVal = 0;
            if (retVal > 1.0f)
                retVal = 1.0f;

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="">was (int)Timing.TotalMilliseconds</param>
		public void Update(GameTime time)
        {
            liveTime += time.ElapsedGameTime.TotalMilliseconds;

            int mytime = (int)(animShift + liveTime);
            const int frameTime = 40;

            if (shaking)
            {
                shakeTimeLeft -= (float)time.ElapsedGameTime.TotalMilliseconds;

                if (shakeTimeLeft <= 0)
                {
                    shaking = false;
                }
            }

            if (mytime > 5000)
            {
                liveTime = 0;
            }


            int newframe = (mytime / frameTime) % block.Frames.Count;


            frame = newframe;

            block.CurrentFrameIndex = frame;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shakeStart">was (int)Timing.TotalMilliseconds;</param>
		public void shake()
        {
            shaking = true;
            shakeTimeLeft = 300;
        }

        private const int shakeMagnitude = 2;


        public float getx()
        {
            return getx(true);
        }

        public float getx(bool allowShake)
        {
            if (shaking && allowShake)
                return x + random.Next(-shakeMagnitude, shakeMagnitude + 1);
            else
                return x;

        }
        public float gety()
        {
            return gety(true);
        }
        public float gety(bool allowShake)
        {
            if (shaking && allowShake)
                return y + offsety + random.Next(-shakeMagnitude, shakeMagnitude + 1);
            else
                return y + offsety;
        }

        public float Height => h;
        public float Width => w;
    }
}