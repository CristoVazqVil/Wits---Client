using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wits.Classes
{
    internal class GameSingleton
    {
        private static GameSingleton instance;
        public int GameId { get; private set; }
        public int PlayerNumber { get; private set; }

        private GameSingleton() { }

        public static GameSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameSingleton();
                }
                return instance;
            }
        }

        public void SetGame(int gameId, int playerNUmber)
        {
            GameId = gameId;
            PlayerNumber = playerNUmber;
        }

        public void ClearGame()
        {
            GameId = 0;
            PlayerNumber = 0;
        }
    }
}
