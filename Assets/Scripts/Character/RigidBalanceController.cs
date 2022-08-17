using UnityEngine;

namespace Character
{
    public class RigidBalanceController : MonoBehaviour
    {
        [SerializeField] private Vector3 centerOfMass;
        [SerializeField]private Rigidbody objectRigidbody;

        //Silly little Script that changes the center of mass by the Value given in the Inspector!
        private void Start()
        {
            objectRigidbody.centerOfMass = centerOfMass;
        }
    }
}