/*****************************************************************************
	Ball: Buster
	Copyright (C) 2004-9 Patrick Avella, Erik Ylvisaker

    This file is part of Ball: Buster.

    Ball: Buster is free software; you can redistribute it and/or modify
    it under the terms of the GNU General internal License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    Ball: Buster is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General internal License for more details.

    You should have received a copy of the GNU General internal License
    along with Ball: Buster; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using AgateLib.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BallBusterX
{
    public class CScoreByte
    {
        private string amount;
        private float x, y;
        private float vx, vy;
        private float alpha;
        private double scale;
        private Color mClr;
        private Font font;

        public string getAmount() { return amount; }

        public float getAlpha() { return alpha; }
        public int getx
        {
            get { return (int)(x); }
        }
        public int gety
        {
            get
            { return (int)(y); }
        }

        public Font Font => font;
        public Color getColor() { return mClr; }

        public CScoreByte(int myx, int myy, string myamount, Font font, Color clr, double scale)
        {
            alpha = 1.0f;

            x = myx;
            y = myy;
            amount = myamount;

            vy = -40;
            vx = 0;

            this.font = font;

            mClr = clr;

            this.scale = scale;
        }

        public void update(GameTime time)
        {
            var time_s = (float)time.ElapsedGameTime.TotalSeconds;

            x += vx * time_s;
            y += vy * time_s;

            if (y < 10)
                y = 10;

            alpha -= 1.0f * time_s;
        }

        public double Scale
        {
            get { return scale; }
        }
    }
}