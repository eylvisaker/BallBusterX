using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace BallBusterX.Scenes
{
    public interface IGameStateFactory
    {
        GameState CreateGameState();
    }

    public class GameStateFactory : IGameStateFactory
    {
        private IComponentContext componentContext;

        public GameStateFactory(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        public GameState CreateGameState()
        {
            return componentContext.Resolve<GameState>();
        }
    }
}
