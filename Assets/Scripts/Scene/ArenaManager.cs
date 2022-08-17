using Character;
using UnityEngine;

namespace Scene
{
    public class ArenaManager : MonoBehaviour
    {
        [SerializeField] private float npcStaticAngle;
        [SerializeField] private float npcStaticVelocity;
        [SerializeField] private float npcDynamicAngleRange;
        [SerializeField] private float npcDynamicVelocityRange;


        [Space]
        [SerializeField] private Transform leftSpawn;
        [SerializeField] private GameObject nonPlayer;
        [SerializeField] private GameObject player;
        [SerializeField] private Transform rightSpawn;

        // Start is called before the first frame update
        private void Start()
        {
            //Instances a player or non player to leftSpawn
            if (GameSettings.leftControlIsPlayer)
            {
                var leftCtrl = Instantiate(player, leftSpawn.position, leftSpawn.rotation);
                leftCtrl.GetComponent<PlayerThrowController>().Initialize(true);
                GameSettings.LeftPlayer = leftCtrl;
                leftCtrl.tag = "LeftPlayer";
            }
            else
            {
                var leftCtrl = Instantiate(nonPlayer, leftSpawn.position, leftSpawn.rotation);
                var leftCtrlController = leftCtrl.GetComponent<NpcThrowController>();
                leftCtrlController.Initialize(true);
                leftCtrlController.InverseDirection();
                leftCtrlController.dynAngleRange = npcDynamicAngleRange;
                leftCtrlController.dynVelocityRange = npcDynamicVelocityRange;
                leftCtrlController.staticVelocity = npcStaticVelocity;
                leftCtrlController.staticAngle = npcStaticAngle;
                leftCtrlController.InverseDirection();
                GameSettings.LeftPlayer = leftCtrl;
                leftCtrl.tag = "LeftPlayer";
            }

            //Instances a player or non player to rightSpawn

            if (GameSettings.rightControlIsPlayer)
            {
                var rightCtrl = Instantiate(player, rightSpawn.position, rightSpawn.rotation);
                rightCtrl.GetComponent<PlayerThrowController>().Initialize(false);
                GameSettings.RightPlayer = rightCtrl;
                rightCtrl.tag = "RightPlayer";
            }
            else
            {
                var rightCtrl = Instantiate(nonPlayer, rightSpawn.position, rightSpawn.rotation);
                var rightCtrlController = rightCtrl.GetComponent<NpcThrowController>();
                rightCtrlController.Initialize(false);
                rightCtrlController.dynAngleRange = npcDynamicAngleRange;
                rightCtrlController.dynVelocityRange = npcDynamicVelocityRange;
                rightCtrlController.staticVelocity = npcStaticVelocity;
                rightCtrlController.staticAngle = npcStaticAngle;
                GameSettings.RightPlayer = rightCtrl;
                rightCtrl.tag = "RightPlayer";
            }
        }
    }
}