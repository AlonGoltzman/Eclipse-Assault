using Mgmt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controllers
{
    public class GunController : MonoBehaviour
    {

        /// <summary>
        /// The bullet that this gun shoots.
        /// </summary>
        public GameObject Bullet;

        /// <summary>
        /// The damage inflicted by the gun.
        /// </summary>
        public int Damage;

        /// <summary>
        /// How many seconds between shots.
        /// </summary>
        public float TimeBetweenShots;

        /// <summary>
        /// Should the gun shoot in this stage.
        /// </summary>
        private bool Shoot = false;

        /// <summary>
        /// Can the character shoot.
        /// </summary>
        private bool CanShoot = true;

        /// <summary>
        /// The Gun's current angle.
        /// </summary>
        private float CurrentAngle = 0;

        /// <summary>
        /// The exit point of the gun.
        /// </summary>
        private Transform ExitPoint;

        void Start()
        {
            ExitPoint = transform.GetChild(0);
        }

        // Update is called once per frame
        void Update()
        {
            FollowCursor();

            DoInput();

            DoAction();
        }

        /// <summary>
        /// Follows the mouse \ touch
        /// </summary>
        private void FollowCursor()
        {
            //TODO: Implement touch detection
            //Convert object position to screen position, according to that calculate the needed angle.
            //gun_calc_explanation from sprites folder explains.
            var GunPositionInWorld = Camera.main.WorldToScreenPoint(transform.position);
            var PositionDelta = Input.mousePosition - GunPositionInWorld;
            CurrentAngle = Mathf.Atan2(PositionDelta.y, PositionDelta.x) * Mathf.Rad2Deg;
            if (CurrentAngle > 180 || CurrentAngle < 0) return;
            transform.rotation = Quaternion.AngleAxis(CurrentAngle, Vector3.forward);
        }

        /// <summary>
        /// Reads input from the user.
        /// </summary>
        private void DoInput()
        {
            Shoot = Input.GetButtonUp(GameConstants.BUTTON_SHOOT);
        }

        /// <summary>
        /// Performs the action if needed.
        /// </summary>
        private void DoAction()
        {
            if (Shoot && CanShoot)
            {
                var bullet = Instantiate(Bullet);

                bullet.name = GameConstants.NAME_BULLET_PLAYER + GameStatistics.BulletsShot;
                GameStatistics.BulletsShot++;

                bullet.transform.position = ExitPoint.position;
                bullet.transform.rotation = transform.rotation;

                bullet.GetComponent<BulletController>().SetAngle(CurrentAngle);
                bullet.GetComponent<BulletController>().SetDamage(Damage);

                CanShoot = false;
                StartCoroutine("WaitForAbilityToShoot");
            }

            Shoot = false;
        }

        private IEnumerator WaitForAbilityToShoot()
        {
            yield return new WaitForSeconds(TimeBetweenShots);
            CanShoot = true;
            yield break;
        }
    }

}