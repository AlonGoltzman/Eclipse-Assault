using Mgmt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controllers
{
    public class SingularShotController : GunController
    {
        /// <summary>
        /// Can the character shoot.
        /// </summary>
        private bool CanShoot = true;


        /// <summary>
        /// Performs the action if needed.
        /// </summary>
        public override void DoAction()
        {
            if (CanShoot)

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
        }

        private IEnumerator WaitForAbilityToShoot()
        {
            yield return new WaitForSeconds(TimeBetweenShots);
            CanShoot = true;
            yield break;
        }
    }

}