using Character;
using UnityEngine;

namespace Scene
{
    public static class ArenaInterface
    {
        private static PlayerThrowController _player1;
        private static NpcThrowController _nonPlayer1;

        private static PlayerThrowController _player2;
        private static NpcThrowController _nonPlayer2;

        private static bool _isProjectile;

        public static GameObject leftPlayer
        {
            get
            {
                if (_player1 != null) return _player1.gameObject;
                if (_nonPlayer1 != null) return _nonPlayer1.gameObject;
                return null;
            }
        }

        public static GameObject rightPlayer
        {
            get
            {
                if (_player2 != null) return _player2.gameObject;
                if (_nonPlayer2 != null) return _nonPlayer2.gameObject;
                return null;
            }
        }
        public static bool strikeFinished => !_isProjectile;
        public static void SetPlayerOne(PlayerThrowController controller)
        {
            _player1 = controller;
        }
        public static void SetPlayerOne(NpcThrowController controller)
        {
            _nonPlayer1 = controller;
        }
        public static void SetPlayerTwo(PlayerThrowController controller)
        {
            _player2 = controller;
        }
        public static void SetPlayerTwo(NpcThrowController controller)
        {
            _nonPlayer2 = controller;
        }
        public static void ProjectileDestroyed()
        {
            _isProjectile = false;
        }
        public static void ProjectileCreated()
        {
            _isProjectile = true;
        }
        public static void LeftStrike()
        {
            if (_player1 != null) _player1.SetOnStrike();
            if (_nonPlayer1 != null) _nonPlayer1.SetOnStrike();
        }

        public static void RightStrike()
        {
            if (_player2 != null) _player2.SetOnStrike();
            if (_nonPlayer2 != null) _nonPlayer2.SetOnStrike();
        }

        public static void InitFirstStrike()
        {
            if (GameSettings.leftPlayerIsNext)
            {
                LeftStrike();
            }
            else if (GameSettings.rightPlayerIsNext)
            {
                RightStrike();
            }
            else
            {
                var i = Random.Range(1, 50);
                if (i < 25) LeftStrike();
                else RightStrike();
            }

            GameSettings.ResetNextPlayer();
        }
    }
}