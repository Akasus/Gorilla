using Audio;
using Terrain;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace Scene
{
    public class ProjectileCollisionManager : MonoBehaviour
    {
        /// <summary>
        ///     The Radius of the Explosion when a Player is punched
        /// </summary>
        [SerializeField] private int explosionRatioPlayer = 500;


        [SerializeField] private GameObject explosion;
        [SerializeField] private GameObject bigExplosion;
            /// <summary>
        ///     The Radius of the Explosion when the Terrain is punched
        /// </summary>
        [SerializeField] public int explosionRatioTerrain = 100;
        private static bool _inAction;

        

        private void OnCollisionEnter(Collision other)
        {
            if(_inAction) return;
            _inAction = true;
            AudioContainer.Main.StopProjectileLoopSound();
            var pos = transform.position;

            switch (other.gameObject.tag)
            {
                case "Terrain":
                    Instantiate(explosion, transform.position, transform.rotation);
                    AudioContainer.Main.PlaySoundWithIndex(1);
                    Terrain2DMain.Main.Dig(pos, explosionRatioTerrain);
                    break;

                case "LeftPlayer":
                    Debug.Log("LeftPlayer");
                    Instantiate(bigExplosion, transform.position, transform.rotation);
                    AudioContainer.Main.PlaySoundWithIndex(1);
                    Terrain2DMain.Main.Dig(pos, explosionRatioPlayer);
                    Destroy(other.gameObject);
                    GameSettings.IncreaseRightWinCounter();
                    GameSettings.GetBoardController().ShowScoreBoard();
                    break;

                case "RightPlayer":
                    Debug.Log("RightPlayer");
                    Instantiate(bigExplosion, transform.position, transform.rotation);
                    AudioContainer.Main.PlaySoundWithIndex(1);
                    Terrain2DMain.Main.Dig(pos, explosionRatioPlayer);
                    Destroy(other.gameObject);
                    GameSettings.IncreaseLeftWinCounter();
                    GameSettings.GetBoardController().ShowScoreBoard();
                    break;
            }
            ArenaInterface.ProjectileDestroyed();
            _inAction = false;
            Destroy(gameObject);

        }
    }
}