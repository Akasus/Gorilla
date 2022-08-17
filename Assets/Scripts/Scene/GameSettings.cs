using Menu;
using UnityEngine;

namespace Scene
{
    public static class GameSettings
    {
        public static string LeftPlayerName = "Player1";
        public static string RightPlayerName = "Player2";

        private static BoardController _boardController;


        public static GameObject LeftPlayer;
        public static GameObject RightPlayer;

        public static bool leftControlIsPlayer { get; private set; } = true;

        public static bool rightControlIsPlayer { get; private set; }

        public static int maxRounds { get; private set; } = 3;

        public static int currentRound { get; private set; } = 1;

        public static int leftPlayerWin { get; private set; }

        public static int rightPlayerWin { get; private set; }

        public static bool leftPlayerIsNext { get; private set; }
        public static bool rightPlayerIsNext { get; private set; }

        public static void SetLeftControl(bool isPlayer)
        {
            leftControlIsPlayer = isPlayer;
        }

        public static void SetRightControl(bool isPlayer)
        {
            rightControlIsPlayer = isPlayer;
        }


        public static void SetMaxRounds(int count)
        {
            maxRounds = count;
        }

        public static void IncreaseRounds()
        {
            if (currentRound <= maxRounds) currentRound++;
        }

        public static void Reset()
        {
            currentRound = 1;
            leftPlayerWin = 0;
            rightPlayerWin = 0;
        }

        public static bool IsLastRound()
        {
            return currentRound == maxRounds;
        }


        public static void IncreaseLeftWinCounter()
        {
            leftPlayerWin++;
            rightPlayerIsNext = true;
        }

        public static void IncreaseRightWinCounter()
        {
            rightPlayerWin++;
            leftPlayerIsNext = true;
        }

        public static void ResetNextPlayer()
        {
            leftPlayerIsNext = false;
            rightPlayerIsNext = false;
        }


        public static void SetBoardController(BoardController controller)
        {
            _boardController = controller;
        }

        public static BoardController GetBoardController()
        {
            return _boardController;
        }
    }
}