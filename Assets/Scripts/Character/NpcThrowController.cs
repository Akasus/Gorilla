using Audio;
using Scene;
using UnityEngine;

namespace Character
{
    public class NpcThrowController : MonoBehaviour
    {
        /// <summary>
        ///     The chosen _angle as a Random Value between the Spread
        /// </summary>
        private float _angle;

        /// <summary>
        ///     Determines if it has the first Strike when the Level starts, or the Second.
        /// </summary>
        private bool _firstOrSecond;

        /// <summary>
        ///     To avoid that that "Update" runs the functions more than one time if it is the Players turn.
        /// </summary>
        private bool _onlyOneTime;

        /// <summary>
        ///     This determines that the Enemy is able to Throw!
        /// </summary>
        private bool _onStrike;

        /// <summary>
        ///     The Temp Storage for the Projectile
        /// </summary>
        private GameObject _temp;

        /// <summary>
        ///     The Temp Storage for the Projectiles Rigid-body
        /// </summary>
        private Rigidbody _tempRigid;

        /// <summary>
        ///     The chosen _velocity as a Random Value between the Spread
        /// </summary>
        private float _velocity;

        [SerializeField] [Tooltip("The Animation to visualize the given _angle")]
        private GameObject angleVisualizer;

        [SerializeField] [Tooltip("The Prefab for the Projectile")]
        private GameObject projectilePrefab;

        [SerializeField] [Tooltip("The Position where the Projectile should Spawn")]
        private Transform projectileSpawn;

        [SerializeField] [Tooltip("The Pivot of the throw rotation")]
        private Transform rotationPoint;
        [Space]
        [SerializeField]
        [Tooltip("The perfect _angle that is needed to hit the opposite Player")]
        public float staticAngle = 45;

        [SerializeField]
        [Tooltip("The perfect _velocity for the Projectile to hit the opposite Player")]
        public float staticVelocity = 10;
        [SerializeField]
        [Tooltip("The Spread of the _angle around the Perfect Value")]
        public float dynAngleRange = 45;

        [SerializeField]
        [Tooltip("The Spread of the _velocity around the Perfect Value")]
        public float dynVelocityRange = 5;
        [Space]
        [SerializeField]
        [Tooltip("The Speed at which the Projectile Rotates")]
        private float angularVelocity = 360;
        [SerializeField]
        [Tooltip("The Speed at which the _angle changes")]
        private float anglePerSecond = 20;


        // Start is called before the first frame update
        private void Start()
        {
            angleVisualizer.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!(_onStrike && ArenaInterface.strikeFinished)) return;

            SetAngle();
            Shoot();

            if (!_onlyOneTime) return;

            angleVisualizer.SetActive(true);
            _onlyOneTime = false;
        }

        /// <summary>
        ///     This Function got Executed by the ArenaManager and tells the Object wich rank it has.
        /// </summary>
        /// <param name="whichOne">Whether if it has the first or the second Strike </param>
        public void Initialize(bool whichOne)
        {
            if (whichOne)
            {
                _firstOrSecond = true;
                ArenaInterface.SetPlayerOne(this);
            }
            else
            {
                _firstOrSecond = false;
                ArenaInterface.SetPlayerTwo(this);
            }
        }


        /// <summary>
        ///     This Function got executed by the ArenaInterface to tell the Object that it is its turn, when the other is done.
        /// </summary>
        public void SetOnStrike()
        {
            _onStrike = true;
            _temp = null;
            _angle = Random.Range(staticAngle - dynAngleRange, staticAngle + dynAngleRange);
            _velocity = Random.Range(staticVelocity - dynVelocityRange, staticVelocity + dynVelocityRange);


            _onlyOneTime = true;
        }

        /// <summary>
        ///     A solution to inverse the _angle when the Enemy is on the Left side.
        ///     Got executed by the ArenaManager!
        /// </summary>
        public void InverseDirection()
        {
            staticAngle = 360 - staticAngle;
        }

        private void SetAngle()
        {
            if (!IsNear(rotationPoint.rotation.eulerAngles.z, _angle, 0.1f))
                rotationPoint.rotation = Quaternion.Euler(new Vector3(0, 0,
                    Mathf.LerpAngle(rotationPoint.rotation.eulerAngles.z, _angle, Time.deltaTime * anglePerSecond)));
        }

        private void Shoot()
        {
            if (!IsNear(rotationPoint.rotation.eulerAngles.z, _angle, 0.2f)) return;

            angleVisualizer.SetActive(false);
            InstantiateProjectile();
            ThrowProjectile();
            FinishStrike();
            if (gameObject.Equals(ArenaInterface.leftPlayer))
            {
                IngameStatsManager.Main.IncreaseLeftShoots();
            }
            else if (gameObject.Equals(ArenaInterface.rightPlayer))
            {
                IngameStatsManager.Main.IncreaseRightShoots();
            }
        }

        private void InstantiateProjectile()
        {
            if(_temp != null) return;
            _temp = Instantiate(projectilePrefab, projectileSpawn.position, projectileSpawn.rotation, projectileSpawn);
            _tempRigid = _temp.GetComponent<Rigidbody>();
            _tempRigid.useGravity = false;
        }

        private void ThrowProjectile()
        {
            _tempRigid.useGravity = true;
            _tempRigid.velocity = _temp.transform.up * _velocity;
            var tempAv = Vector3.forward * angularVelocity;
            _tempRigid.angularVelocity = tempAv;
            FinishStrike();
            //_temp.GetComponent<AudioSource>().Play();
            AudioContainer.Main.PlayProjectileLoopSound();
        }

        private void FinishStrike()
        {
            _onStrike = false;
            ArenaInterface.ProjectileCreated();
            SetStriker();
        }

        private void SetStriker()
        {
            if (_firstOrSecond)
                ArenaInterface.RightStrike();
            else
                ArenaInterface.LeftStrike();
        }

        //Static Area


        /// <summary>
        ///     Internal function to determine a nearby Value as true in dependence to the params.
        /// </summary>
        /// <param name="value">The Value that needs to be adjusted</param>
        /// <param name="zero">The Value that it should have in the perfect State</param>
        /// <param name="sensitivity">The spread around the perfect state</param>
        /// <returns></returns>
        private static bool IsNear(float value, float zero, float sensitivity)
        {
            return value >= zero - sensitivity && value <= zero + sensitivity;
        }
    }
}