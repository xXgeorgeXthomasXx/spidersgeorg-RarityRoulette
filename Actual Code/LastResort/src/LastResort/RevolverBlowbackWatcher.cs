using BepInEx.Logging;
using UnityEngine;

namespace LastResort
{
    internal class RevolverBlowbackWatcher : MonoBehaviour
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("Revolver");


        public float shotTime;

        Character characterGettingShot;

        Vector3 shotDirection;
        public void Start()
        {
            characterGettingShot = Character.localCharacter;
        }

        public void Update()
        {
            if (shotTime > 0f)
            {
                shotTime -= Time.deltaTime;
                UpdateShotPhysicsT();

            }
        }

        public void ShootSelfT(float howLongToFly, Character whoIsGettingShot, Vector3 whichDirectionShooting)
        {
            shotTime = howLongToFly;
            characterGettingShot = whoIsGettingShot;
            shotDirection = whichDirectionShooting;
        }

        public void UpdateShotPhysicsT()
        {
            Vector3 ForceDirection = (shotDirection * 25f) * -1f;
            characterGettingShot.Fall(0.5f);
            characterGettingShot.AddForce(ForceDirection, 1f, 1f);
        }


    }
}
