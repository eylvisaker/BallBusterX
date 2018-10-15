using AgateLib.Display;
using AgateLib.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BallBusterX.Scenes
{
    public class StageCompleteScene : Scene
    {
        private readonly GraphicsDevice device;
        private readonly CImage img;
        private readonly GameState gameState;
        private readonly SpriteBatch spriteBatch;
        private readonly Font font;
        private readonly TimeSpan levelTime;
        private readonly int totalBonus;
        private readonly int newscore;
        private float sceneTime;
        private int bonus;
        private int extraBonus;
        private int timeBonus;
        private string timeString;

        public StageCompleteScene(GraphicsDevice device, CImage img, GameState gameState)
        {
            this.device = device;
            this.img = img;
            this.gameState = gameState;
            this.spriteBatch = new SpriteBatch(device);

            font = new Font(img.Fonts.Default, 18);

            // basic level bonus for completing a level
            bonus = 100;

            TimeSpan levelTime = TimeSpan.FromMilliseconds(gameState.levelTime);
            timeString = $"{levelTime.Minutes}:{levelTime.Seconds:00}";

            //timeString = (minutes) + ":";

            //if (seconds < 10) timeString += "0";
            //timeString += (seconds);

            // Give up to 200 points if the player has completed the level in less than three minutes.
            timeBonus = (int)(((180 - levelTime.TotalSeconds) / 180) * 200);
            if (timeBonus < 0) timeBonus = 0;

            extraBonus = 0;

            if (gameState.ballslost == 0)
                extraBonus += 500;
            if (gameState.powerupcount == 0)
                extraBonus += 1500;

            totalBonus = bonus + timeBonus + extraBonus;
            newscore = gameState.Score + totalBonus;
        }

        protected override void OnUpdate(GameTime time)
        {
            base.OnUpdate(time);

            sceneTime += (float)time.ElapsedGameTime.TotalMilliseconds;

            if (sceneTime > 10000)
                StartNextLevel();
        }

        private void StartNextLevel()
        {
            gameState.GainPoints(totalBonus);

            // clear out time-based powerups
            gameState.blaster = false;
            gameState.catchred = false;
            gameState.catchblue = false;
            gameState.supersticky = false;

            gameState.pow = false;
            gameState.smash = false;
            gameState.fireball = false;

            gameState.level++;

            IsFinished = true;
        }


        protected override void DrawScene(GameTime time)
        {
            const float countRate = 1.5f;  // higher means the numbers count slower
            device.Clear(Color.White);
            spriteBatch.Begin();

            const int tab = 400;

            int lineCount = 0;
            int lineLimit = 0;

            const int xpt = 175;

            font.Color = Color.Black;
            font.DrawText(spriteBatch, xpt, 50, string.Format("Completed level {0}-{1}", gameState.world + 1, gameState.level + 1));

            if (sceneTime > 1000)
            {
                font.DrawText(spriteBatch, xpt, 100, "Score:");
                font.DrawText(spriteBatch, tab, 100, gameState.Score.ToString());
            }

            if (sceneTime > 1500)
            {
                font.DrawText(spriteBatch, xpt, 150, "Level Bonus:");
                font.DrawText(spriteBatch, tab, 150, ((int)Math.Min(bonus, (sceneTime - 1500) / countRate)).ToString());
            }

            if (sceneTime > 2000)
            {
                font.DrawText(spriteBatch, xpt, 200, "Time: ");
                font.DrawText(spriteBatch, tab, 200, timeString);
            }

            if (sceneTime > 2500)
            {
                font.DrawText(spriteBatch, xpt, 230, "Time Bonus: ");
                font.DrawText(spriteBatch, tab, 230, ((int)Math.Min(timeBonus, (sceneTime - 2500) / countRate)).ToString());
            }

            if (sceneTime > 3000)
            {
                lineLimit = (int)((sceneTime - 3000) / 500 + 1);
            }


            if (gameState.ballslost == 0 && lineCount < lineLimit)
            {
                font.DrawText(spriteBatch, xpt, 280 + lineCount * 50, "No balls lost");
                font.DrawText(spriteBatch, tab, 280 + lineCount * 50, "500");

                lineCount++;
            }

            /// show other bonuses.
            if (gameState.powerupcount == 0 && lineCount < lineLimit)
            {
                font.DrawText(spriteBatch, xpt, 280 + lineCount * 50, "No powerups");
                font.DrawText(spriteBatch, tab, 280 + lineCount * 50, "1500");

                lineCount++;
            }

            /// display the final score
            if (lineCount < lineLimit)
            {
                int medianstart = 3000 + (lineCount) * 500;

                font.DrawText(spriteBatch, xpt, 280 + lineCount * 50, "Final Score:");
                font.DrawText(spriteBatch, tab, 280 + lineCount * 50,
                    ((int)Math.Min(newscore, gameState.Score + (sceneTime - medianstart) / countRate)).ToString());

                lineCount++;
            }

            spriteBatch.End();
        }
    }
}
