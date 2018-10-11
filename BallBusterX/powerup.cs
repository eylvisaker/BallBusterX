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

using AgateLib.Display.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BallBusterX
{
    public class CPowerUp
    {
        public double delay;
        public float w, h;
        public float r, g, b, a;
        public float extray;
        public Vector2 position, velocity;

        public Sprite icon;

        public CPowerUp(float myx, float myy)
        {
            this.delay = 100;
            this.w = this.h = 1.0f;
            this.r = this.g = this.b = 1.0f;
            this.a = 1.0f;

            this.position = new Vector2(myx, myy);
            this.velocity = new Vector2(0, 100);

            this.extray = this.position.Y;

            this.isred = this.isblue = false;
        }

        public float x => position.X;
        public float y => position.Y;

        public float vx { get => velocity.X; set => velocity.X = value; }
        public float vy { get => velocity.Y; set => velocity.Y = value; }

        public virtual bool update(GameTime time)
        {
            float time_ms = (float)time.ElapsedGameTime.TotalMilliseconds;

            if (this.effect != PowerupTypes.PU_NONE)
            {
                // we'll set the effect to z when the player attains the power up
                this.position.Y += this.velocity.Y * time_ms;
                this.position.X += this.velocity.X * time_ms;
                this.extray = this.position.Y;

                //if ((unsigned)this.delay + start < (int)Timing.TotalMilliseconds)
                //{
                this.velocity.Y += 300.0f * time_ms;

                //	this.start= (int)Timing.TotalMilliseconds;
                //}
                if (this.velocity.Y > 600) return false;
                return true;
            }

            this.delay -= time.ElapsedGameTime.TotalMilliseconds;

            if (this.delay > 0)
            {
                this.w += 0.06f;
                this.position.X -= 0.03f * icon.SpriteWidth;
                this.h -= 0.03f;
                //		this.r-= 0.03f;
                this.a -= 0.03f;
                this.position.Y -= velocity.Y * time_ms;
                this.position.X += velocity.X * time_ms;
                this.extray -= (velocity.Y * 1.5f) * time_ms;


                if (this.a <= 0.0f) return false;
            }

            return true;
        }
        public Color Color
        {
            get
            {
                return new Color(
                    (int)(r * 255.0f),
                    (int)(g * 255.0f),
                    (int)(b * 255.0f),
                    (int)(a * 255.0f)
                    );
            }
        }

        public void setEffect(PowerupTypes neweffect)
        {
            oldeffect = effect;
            effect = neweffect;

            switch (effect)
            {
                case PowerupTypes.RANDOM:

                    isred = isblue = true;
                    break;

                case PowerupTypes.FASTBALL:
                case PowerupTypes.SMALLPADDLE:
                case PowerupTypes.CATCHRED:
                case PowerupTypes.BLASTER:

                    isred = true;
                    break;

                case PowerupTypes.MULTIBALL:
                case PowerupTypes.PU3BALL:
                case PowerupTypes.LARGEPADDLE:
                case PowerupTypes.SLOWBALL:
                case PowerupTypes.STICKY:
                case PowerupTypes.PTS100:
                case PowerupTypes.PTS250:
                case PowerupTypes.PTS500:
                case PowerupTypes.CATCHBLUE:
                case PowerupTypes.POW:

                    isblue = true;
                    break;

                case PowerupTypes.DOOR:
                    this.velocity.Y = 0;

                    break;

            }

        }
        public PowerupTypes getEffect()
        {
            return effect;
        }

        public bool isRed() { return isred; }
        public bool isBlue() { return isblue; }

        public PowerupTypes oldeffect;
        private PowerupTypes effect;
        private bool isred, isblue;



    }

    internal class CPowerUpList
    {
        public CPowerUpList()
        {
            total = 0;
        }

        public void clear()
        {
            data.Clear();

            total = 0;
        }

        public void addEffect(PowerupTypes effect, Sprite icon, int weight)
        {
            PUData pudata = new PUData(effect, icon, weight);

            total += weight;

            data.Add(pudata);

        }

        public void removeEffect(PowerupTypes effect)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].effect == effect)
                {
                    total -= data[i].weight;

                    //delete data[i];
                    data.RemoveAt(i);
                }

            }
        }

        public void AssignPowerup(out CPowerUp powerup, float x, float y, Random random)
        {
            int roll = random.Next(data.Sum(d => d.weight));
            PUData sel = null;

            roll %= total;

            for (int i = 0; i < data.Count; i++)
            {
                roll -= data[i].weight;

                if (roll < 0)
                {
                    sel = data[i];
                    break;
                }

            }


            powerup = new CPowerUp(x, y);
            powerup.setEffect(sel.effect);
            powerup.icon = sel.icon;

        }

        private class PUData
        {
            public PUData(PowerupTypes myeffect, Sprite myicon, int myweight)
            {
                effect = myeffect;
                icon = myicon;
                weight = myweight;
            }

            public PowerupTypes effect;
            public Sprite icon;
            public int weight;

        };

        private List<PUData> data = new List<PUData>();
        private int total;
    }

    public enum PowerupTypes
    {
        ONEUP = 'a',
        BLASTER = 'b',
        FASTBALL = 'c',
        FIREBALL = 'd',
        MULTIBALL = 'e',
        PU3BALL = '3',
        LARGEPADDLE = 'f',
        REGULARPADDLE = 'g',
        SMALLPADDLE = 'h',
        NORMALSPEED = 'i',
        SLOWBALL = 'j',
        STICKY = 'k',
        RESET = 'l',
        PTS100 = 'm',
        PTS250 = 'n',
        PTS500 = 'o',
        PTS1000 = 'p',
        RANDOM = 'r',
        CATCHBLUE = 's',
        CATCHRED = 't',
        SUPERSTICKY = 'u',

        POW = 'P',
        SMASH = 'W',
        RBSWAP = 'S',
        DOOR = 'D',

        PU_NONE = 'z'
    };
}