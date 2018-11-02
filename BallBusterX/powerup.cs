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
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BallBusterX
{
    public class PowerUp
    {
        private class PowerUpTrail
        {
            public Vector2 Position { get; set; }
            public float LifeLeft = 1;
            private Sprite icon;

            public PowerUpTrail(Sprite icon)
            {
                this.icon = new Sprite(icon);
            }

            public void Update(GameTime time)
            {
                LifeLeft -= (float)time.ElapsedGameTime.TotalSeconds * 2;
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                if (LifeLeft <= 0)
                    return;

                icon.Scale = new Vector2(LifeLeft, LifeLeft);
                icon.Alpha = LifeLeft * 0.5f;

                Vector2 drawPosition = Position;
                drawPosition.X += (1 - LifeLeft) * icon.SpriteWidth * 0.5f;
                drawPosition.Y += (1 - LifeLeft) * icon.SpriteWidth * 0.5f;

                icon.Draw(spriteBatch, drawPosition);
            }

            public void Initialize(Vector2 position)
            {
                LifeLeft = 1;
                Position = position;
            }
        }

        public double delay;
        public float widthScale = 1, heightScale = 1;
        public float alpha;
        public float extray;
        public Vector2 position, velocity;

        public Sprite icon;

        public PowerupTypes oldeffect;
        private PowerupTypes effect;
        private bool isred, isblue;

        private List<PowerUpTrail> trails = new List<PowerUpTrail>();
        private float timeToDropTrail;
        private const float TrailDropPeriod = 0.1f;

        public PowerUp(float myx, float myy)
        {
            this.delay = 100;
            this.alpha = 1.0f;

            this.position = new Vector2(myx, myy);
            this.velocity = new Vector2(0, 100);

            this.extray = this.position.Y;

            this.isred = this.isblue = false;
        }

        public float x => position.X;
        public float y => position.Y;

        public Rectangle HitBox => new Rectangle((int)x, (int)y, 40, 40);

        public float vx { get => velocity.X; set => velocity.X = value; }
        public float vy { get => velocity.Y; set => velocity.Y = value; }

        public bool IsRed => isred;
        public bool IsBlue => isblue;
        public Color Color => new Color(Color.White, alpha);


        public virtual bool Update(GameTime time)
        {
            float time_s = (float)time.ElapsedGameTime.TotalSeconds;

            timeToDropTrail -= time_s;

            foreach(var trail in trails)
            {
                trail.Update(time);
            }

            if (this.effect != PowerupTypes.PU_NONE)
            {
                // we'll set the effect to z when the player attains the power up
                this.position.Y += this.velocity.Y * time_s;
                this.position.X += this.velocity.X * time_s;
                this.extray = this.position.Y;

                this.velocity.Y += 300.0f * time_s;

                if (timeToDropTrail <= 0)
                {
                    timeToDropTrail += TrailDropPeriod;
                    PowerUpTrail trail;

                    if (trails.FirstOrDefault()?.LifeLeft <= 0)
                    {
                        trail = trails.FirstOrDefault();
                        trails.RemoveAt(0);
                    }
                    else
                    {
                        trail = new PowerUpTrail(icon);
                    }

                    trail.Initialize(this.position);
                    trails.Add(trail);
                }

                return true;
            }
            else
            {
                position.X -= 1.5f * time_s;
                this.widthScale += 3f * time_s;
                this.heightScale += 3f * time_s;

                this.alpha -= 1f * time_s;
                this.position.Y -= velocity.Y * time_s;
                this.position.X += velocity.X * time_s;
                this.extray -= (velocity.Y * 1.5f) * time_s;

                if (this.alpha <= 0.0f) return false;
            }

            return true;
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

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (PowerUpTrail trail in trails)
            {
                trail.Draw(spriteBatch);
            }

            icon.Color = Color;
            icon.Scale = new Vector2(widthScale, heightScale);
            icon.Draw(spriteBatch, position);
        }
    }

    internal class PowerUpList
    {
        public PowerUpList()
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

        public void AssignPowerup(out PowerUp powerup, float x, float y, Random random)
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

            powerup = new PowerUp(x, y);
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