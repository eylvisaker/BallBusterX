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

namespace BallBusterX
{
    public class CFlash
    {
        const float life = 125;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myx"></param>
        /// <param name="myy"></param>
        /// <param name="start">Was (int)Timing.TotalMilliseconds;</param>
        public CFlash(int myx, int myy)
        {
            this.alpha = 0.8f;
            this.x = myx;
            this.y = myy;
            this.lifeLeft = life;
        }

        public float x, y, alpha;
        public float lifeLeft;

        public bool update(GameTime time)
        {
            lifeLeft -= (float)time.ElapsedGameTime.TotalMilliseconds;

            this.alpha = lifeLeft / life;

            if (this.alpha < 0) return false;

            return true;

        }
    }
}