using ControllerSO;
using Mgmt;
using System.Collections;
using UnityEngine;

namespace Controllers
{
    public class WeaponController : MonoBehaviour
    {
        /// <summary>
        /// The Gun's current angle.
        /// </summary>
        protected float CurrentAngle = 0;

        /// <summary>
        /// The exit point of the gun.
        /// </summary>
        protected Transform ExitPoint;

        /// <summary>
        /// The weapon's statistics and definitions.
        /// </summary>
        public WeaponStats Stats;

        /// <summary>
        /// Defines if the weapon can shoot or not.
        /// </summary>
        private bool CanShoot = true;

        /// <summary>
        /// The projectile script used for consecutive shots such as the laser.
        /// </summary>
        private ConsecutiveShotProjectile ConsecutiveShotProjectileScript;

        /// <summary>
        /// Used for consecutive shots ignoring the first hit due to the following bug:
        /// First Time Hit - Raycast Issue - Unknown RC.
        /// </summary>
        private bool FirstHitPassed = false;

#if UNITY_ANDROID
        /// <summary>
        /// The origin of the touch.
        /// </summary>
        private Vector2 TouchOrigin = -Vector2.one;

        /// <summary>
        /// The current position of the player's finger.
        /// </summary>
        private Vector2 CurrentTouchPosition = -Vector2.one;

        /// <summary>
        /// The touch on the screen used to angle the weapon.
        /// Used for non-tlit controls.
        /// </summary>
        private Vector2 AngleTouch = -Vector2.one;

        /// <summary>
        /// The angle at which the weapon's rotation changes.
        /// </summary>
        private float AngleChange = 0.5f;
#endif

        void Start()
        {
            ExitPoint = transform.GetChild(0);
        }

        // Update is called once per frame
        void Update()
        {
            FollowCursor();

            DoAction();
        }

 
        /// <summary>
        /// Follows the mouse \ touch
        /// </summary>
        private void FollowCursor()
        {


#if UNITY_ANDROID
            if (GameConstants.PREFAB_GAME_MANAGER.TiltControl)
            {
                if (Input.touchCount > 0)
                {
                    Touch PlayerTouch = Input.touches[0];
                    if (PlayerTouch.phase == TouchPhase.Began && TouchOrigin.x < 0)
                    {
                        TouchOrigin = PlayerTouch.position;
                    }
                    else if (PlayerTouch.phase == TouchPhase.Ended && TouchOrigin.x <= 0)
                    {
                        TouchOrigin = -Vector2.one;
                        CurrentTouchPosition = -Vector2.one;
                    }
                    else if (PlayerTouch.phase == TouchPhase.Moved)
                    {
                        CurrentTouchPosition = PlayerTouch.position;
                    }
                }
                else
                {
                    TouchOrigin = -Vector2.one;
                    CurrentTouchPosition = -Vector2.one;
                }

                if (CurrentTouchPosition.x > 0 && TouchOrigin.x > 0)
                {
                    float XDelta = CurrentTouchPosition.x - TouchOrigin.x;

                    float ratio = Mathf.Abs(XDelta) / (GameConstants.X_MIDDLE_OF_SCREEN * 2);

                    float AngleChange = ratio * 180;

                    AngleChange *= XDelta < 0 ? 1 : -1;

                    if (CurrentAngle + AngleChange < 0)
                        CurrentAngle = 0;
                    else if (CurrentAngle + AngleChange > 180)
                        CurrentAngle = 180;
                    else
                        CurrentAngle += AngleChange;

                    TouchOrigin = CurrentTouchPosition;
                    //Debug.LogFormat("Delta: {0}, Ratio: {1}, Angle Change: {2}, Current Angle: {3}.", XDelta, ratio, AngleChange, CurrentAngle);
                    transform.rotation = Quaternion.AngleAxis(CurrentAngle, Vector3.forward);
                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.touches[i];

                        if (touch.position.y >= GameConstants.Y_MIDDLE_OF_SCREEN)
                        {
                            if (AngleTouch.x < 0)
                            {
                                AngleTouch = touch.position;
                                break;
                            }
                            else
                            {
                                AngleTouch = -Vector2.one;
                            }
                        }
                    }

                    if (AngleTouch.x > 0)
                    {
                        float ChangeAngle = AngleChange * (AngleTouch.x < GameConstants.X_MIDDLE_OF_SCREEN ? 1 : -1);
                        if (CurrentAngle + ChangeAngle < 0)
                            CurrentAngle = 0;
                        else if (CurrentAngle + ChangeAngle > 180)
                            CurrentAngle = 180;
                        else
                            CurrentAngle += ChangeAngle;
                        transform.rotation = Quaternion.AngleAxis(CurrentAngle, Vector3.forward);
                    }
                }
                AngleTouch = -Vector2.one;
            }
#else
            var GunPositionInWorld = Camera.main.WorldToScreenPoint(transform.position);
            var PositionDelta = Input.mousePosition - GunPositionInWorld;
            CurrentAngle = Mathf.Atan2(PositionDelta.y, PositionDelta.x) * Mathf.Rad2Deg;
            if (CurrentAngle > 180 || CurrentAngle < -90)
                CurrentAngle = 180;
            if (CurrentAngle < 0 && CurrentAngle >= -90)
                CurrentAngle = 0;
            transform.rotation = Quaternion.AngleAxis(CurrentAngle, Vector3.forward);
#endif
        }

        /// <summary>
        /// Performs the action (fires or calculates what needs to be calculated).
        /// </summary>
        public void DoAction()
        {
            float ROF = Stats.FireRate;
            if (ROF <= 0)
            { //Consecutive Shot - i.e. Laser
                if(ConsecutiveShotProjectileScript == null)
                {
                    ConsecutiveShotProjectileScript = GetComponentInChildren<ConsecutiveShotProjectile>();
                }

                Vector3 ExitPointPosition = ExitPoint.transform.position;
                Vector3 MousePosition = Input.mousePosition;
                MousePosition.z = 10;
                MousePosition = Camera.main.ScreenToWorldPoint(MousePosition);

                Vector3 Delta = MousePosition - ExitPointPosition;
                Delta = Delta.normalized;

                if (Physics.Raycast(ExitPointPosition, Delta, out RaycastHit Hits))
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
                        HitEntity.BroadcastMessage("DamagedExternal", new object[] { Stats.DamagePerShot, Hits.point });
                        ConsecutiveShotProjectileScript.SendMessage("ExpandProjectile", new Vector2(Hits.point.x, Hits.point.y));
                    }
                }
            }
            else
            { //Single shot - i.e. rail gun \ cannon
                if (CanShoot)
                {
                    var bullet = Instantiate(Stats.Projectile);

                    bullet.name = GameConstants.NAME_BULLET_PLAYER + GameStatistics.BulletsShot;
                    GameStatistics.BulletsShot++;

                    bullet.transform.position = ExitPoint.position;
                    bullet.transform.rotation = transform.rotation;

                    bullet.GetComponent<BulletController>().SetAngle(CurrentAngle);
                    bullet.GetComponent<BulletController>().SetDamage(Stats.DamagePerShot);

                    CanShoot = false;
                    StartCoroutine("WaitForAbilityToShoot");
                }
            }
        }

        /// <summary>
        /// Defines the weapon stats to be used.
        /// </summary>
        /// <param name="NewStats"></param>
        public void SetWeaponStats(WeaponStats NewStats)
        {
            Stats = NewStats;
            if (Stats.FireRate <= 0)
            {
                Instantiate(Stats.Projectile, ExitPoint.transform);
            }
        }

        /// <summary>
        /// Used when the weapon's rate of fire is greater than 0.
        /// Used to wait until being able to shoot again.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForAbilityToShoot()
        {
            yield return new WaitForSeconds(1.0f / Stats.FireRate);
            CanShoot = true;
            yield break;
        }
    }
}
