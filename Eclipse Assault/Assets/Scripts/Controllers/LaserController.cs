using Mgmt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{

    public class LaserController : GunController
    {

        ///// <summary>
        ///// Defines whether or not the laser beam should be enlarged or not.
        ///// </summary>
        //private bool Enlarge = true;


        /// <summary>
        /// The actual laser beam.
        /// </summary>
        private LaserBeamController LaserBeam = null;

        /// <summary>
        /// Due to a bug, we ignore the first hit.
        /// Bug Ref: First Time Hit - Raycast Issue - Unknown RC.
        /// </summary>
        private bool FirstHitPassed = false;

        public override void DoAction()
        {
            if (LaserBeam == null)
            {
                LaserBeam = GetComponentInChildren<LaserBeamController>();
            }

            Vector3 ExitPointPosition = ExitPoint.transform.position;
            Vector3 MousePosition = Input.mousePosition;
            MousePosition.z = 10;
            MousePosition = Camera.main.ScreenToWorldPoint(MousePosition);

            Vector3 Delta = MousePosition - ExitPointPosition;
            Delta = Delta.normalized;

            RaycastHit Hits;

            if (Physics.Raycast(ExitPointPosition, Delta, out Hits))
            {
                // Ref:First Time Hit - Raycast Issue - W\A
                if (!FirstHitPassed)
                {
                    FirstHitPassed = true;
                    return;
                }

                Vector3 CameraPointOfImpact = Camera.main.WorldToViewportPoint(Hits.point);
                if (CameraPointOfImpact.x > 1 || CameraPointOfImpact.x < 0)
                {
                    return;
                }

                GameObject HitEntity = Hits.collider.gameObject;
                if (HitEntity.name.Contains(GameConstants.NAME_ENEMY))
                {
                    HitEntity.BroadcastMessage("Hit", ((float)1 / (float)Damage));
                    LaserBeam.SendMessage("ExpandBeam", new Vector2(Hits.point.x, Hits.point.y));
                }
            }

        }

    }

}