using Audio;
using Scene;
using UI;
using UnityEngine;

namespace Character
{
    public class PlayerThrowController : MonoBehaviour
    {
        private bool _firstOrSecond;

        /// <summary>
        ///     To avoid that that "Update" runs the functions more than one time if it is the Enemies turn.
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

        private Vector3 _tempRotation;

        /// <summary>
        ///     Stores the Increasing Value for the Velocity.
        /// </summary>
        private float _velocityCounter;


        private ValueBar _velocitySlider;

        [SerializeField] private GameObject angleGauge;

        [SerializeField] [Tooltip("The Speed at which the Angle changes")]
        private float anglePerSecond = 20;

        [SerializeField] private GameObject anglePointer;

        [Space] [SerializeField] [Tooltip("The Speed at which the Projectile Rotates")]
        private float angularVelocity = 360;

        [SerializeField] [Tooltip("How fast the Velocity should increase")]
        private float countingSteps = 10;

        [SerializeField] [Tooltip("The Maximum Angle that can be reached when the positive Y Axis is Zero")]
        private float maxAngle = 90;

        [SerializeField] private float maxVelocity = 100;

        [SerializeField] [Tooltip("The Prefab for the Projectile")]
        private GameObject projectilePrefab;

        [SerializeField] [Tooltip("The Position where the Projectile should Spawn")]
        private Transform projectileSpawn;

        [SerializeField] [Tooltip("The Pivot of the throw rotation")]
        private Transform rotationPoint;

        [SerializeField] [Tooltip("The ValueBar to Visualize the Velocity;")]
        private GameObject valueSlider;


        // Start is called before the first frame update
        private void Start()
        {
            _tempRotation = rotationPoint.rotation.eulerAngles;
            angleGauge.SetActive(false);
            anglePointer.SetActive(false);
            valueSlider.SetActive(false);
            _velocitySlider = valueSlider.GetComponent<ValueBar>();
            _velocitySlider.Maximum = maxVelocity;
        }

        // Update is called once per frame
        private void Update()
        {
            //if (_onStrike && ArenaInterface.StrikeFinished)
            //{
            //    if (_onlyOneTime)
            //    {
            //        angleGauge.SetActive(true);
            //        anglePointer.SetActive(true);
            //        _onlyOneTime = false;
            //    }

            //    SetAngle();
            //    Shoot();
            //}

            if (!(_onStrike && ArenaInterface.strikeFinished)) return;
            if (Time.timeScale <= 0.0f) return;
            SetAngle();
            Shoot();
            if (!_onlyOneTime) return;
            angleGauge.SetActive(true);
            anglePointer.SetActive(true);
            _onlyOneTime = false;
        }


        public void SetOnStrike()
        {
            _temp = null;
            _onStrike = true;
            _onlyOneTime = true;
        }


        private void Shoot()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InstantiateProjectile();
                //angleGauge.SetActive(false);
                //anglePointer.SetActive(false);
                valueSlider.SetActive(true);
            }

            if (Input.GetKey(KeyCode.Space))
                if (_velocityCounter <= maxVelocity)
                {
                    _velocityCounter += countingSteps * Time.deltaTime;
                    _velocitySlider.Value = _velocityCounter;
                }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                ThrowProjectile();
                valueSlider.SetActive(false);
                angleGauge.SetActive(false);
                anglePointer.SetActive(false);
                _onStrike = false;
                ArenaInterface.ProjectileCreated();
                SetStriker();
                if (gameObject.Equals(ArenaInterface.leftPlayer))
                {
                    IngameStatsManager.Main.IncreaseLeftShoots();
                }
                else if (gameObject.Equals(ArenaInterface.rightPlayer))
                {
                    IngameStatsManager.Main.IncreaseRightShoots();
                }
            }

        }


        private void SetAngle()
        {
            if (gameObject.Equals(ArenaInterface.leftPlayer))
            {
                _tempRotation.z -= Input.GetAxis("Horizontal") * anglePerSecond * Time.deltaTime;
            }
            else if (gameObject.Equals(ArenaInterface.rightPlayer))
            {
                _tempRotation.z += Input.GetAxis("Horizontal") * anglePerSecond * Time.deltaTime;
            }
            _tempRotation.z = Mathf.Clamp(_tempRotation.z, -maxAngle, maxAngle);

            rotationPoint.rotation = Quaternion.Euler(_tempRotation);
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
            _tempRigid.velocity = _temp.transform.up * _velocityCounter;
            var tempAv = Vector3.forward * angularVelocity;
            _tempRigid.angularVelocity = tempAv;
            AudioContainer.Main.PlayProjectileLoopSound();
            _velocityCounter = 0;
        }


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
                angleGauge.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
        }

        /// <summary>
        /// Determines who's turn is next
        /// </summary>
        private void SetStriker()
        {
            if (_firstOrSecond)
                ArenaInterface.RightStrike();
            else
                ArenaInterface.LeftStrike();
        }
    }
}