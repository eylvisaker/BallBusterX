using AgateLib.Scenes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BallBusterX.Scenes
{
    public class GameScene : Scene
    {
        private GameState gameState;

        public GameScene(GameState gameState)
        {
            this.gameState = gameState;
        }
    }
}
