using Mgmt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        /// Has a touch began on the screen.
        /// </summary>
        private bool TouchStarted;

        /// <summary>
        /// The origin of the touch.
        /// </summary>
        private Vector2 TouchOrigin = -Vector2.one;

        /// <summary>
        /// The current position of the player's finger.
        /// </summary>
        private Vector2 CurrentTouchPosition = -Vector2.one;

        /// <summary>
        /// The player's scren width.
        /// </summary>
        private float ScreenWidth;

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

#if UNITY_ANDROID
        /// <summary>
        /// Should the weapon be rotating left?
        /// </summary>
        private bool RotateLeft;

        /// <summary>
        /// Should the weapon be rotating right?
        /// </summary>
        private bool RotateRight;

        /// <summary>
        /// The angle at which the weapon's rotation changes.
        /// </summary>
        private float AngleChange = 1f;
#endif

        void Start()
        {
            ExitPoint = transform.GetChild(0);
#if UNITY_ANDROID
            ScreenWidth = Screen.width;

            EventTrigger AngleRight = GameObject.Find(GameConstants.UI_NAME_ANGLE_RIGHT_BUTTON).GetComponent<EventTrigger>();
            EventTrigger AngleLeft = GameObject.Find(GameConstants.UI_NAME_ANGLE_LEFT_BUTTON).GetComponent<EventTrigger>();

            EventTrigger.Entry startRotateLeftEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            startRotateLeftEntry.callback.AddListener(StartRotateLeft);
            EventTrigger.Entry startRotateRightEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            startRotateRightEntry.callback.AddListener(StartRotateRight);

            EventTrigger.Entry stopRotateLeftEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            stopRotateLeftEntry.callback.AddListener(StopRotateLeft);
            EventTrigger.Entry stopRotateRightEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            stopRotateRightEntry.callback.AddListener(StopRotateRight);


            AngleLeft.triggers.Add(startRotateLeftEntry);
            AngleLeft.triggers.Add(stopRotateLeftEntry);
            AngleRight.triggers.Add(startRotateRightEntry);
            AngleRight.triggers.Add(stopRotateRightEntry);


#endif
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

            if (!GameMgr.SwipeControl)
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

                    float ratio = Mathf.Abs(XDelta) / ScreenWidth;

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
            } else
            {
                CurrentAngle += RotateLeft ? AngleChange : (RotateRight ? -1 * AngleChange : 0);
                if (CurrentAngle < 0) CurrentAngle = 0;
                if (CurrentAngle > 180) CurrentAngle = 180;
                transform.rotation = Quaternion.AngleAxis(CurrentAngle, Vector3.forward);
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
        /// Performs the action if needed.
        /// </summary>
        private void DoAction()
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

#if UNITY_ANDROID

        /// <summary>
        /// Starts  rotating the weapon to the left.
        /// </summary>
        private void StartRotateRight(BaseEventData data)
        {
            RotateRight = true;
            if (RotateLeft && RotateRight)
                RotateLeft = RotateRight = false;
        }

        /// <summary>
        /// Starts rotating the weapon to the right.
        /// </summary>
        private void StartRotateLeft(BaseEventData data)
        {
            RotateLeft = true;
            if (RotateLeft && RotateRight)
                RotateLeft = RotateRight = false;
        }
        
        /// <summary>
        /// Stops the weapon from rotating right.
        /// </summary>
        private void StopRotateRight(BaseEventData data)
        {
            RotateRight = false;
        }

        /// <summary>
        /// Stops the wepaon from rotating left.
        /// </summary>
        private void StopRotateLeft(BaseEventData data)
        {
            RotateLeft = false;
        }
#endif
    }

}