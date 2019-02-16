using Mgmt;
using UnityEngine;

namespace Controllers
{
    public abstract class GunController : MonoBehaviour
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
        /// The Gun's current angle.
        /// </summary>
        protected float CurrentAngle = 0;

        /// <summary>
        /// The exit point of the gun.
        /// </summary>
        protected Transform ExitPoint;

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
        public float AngleChange;
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
                            }
                            else
                            {
                                AngleTouch = -Vector2.one;
                                break;
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
        /// Performs the action if needed.
        /// </summary>
        public abstract void DoAction();

    }

}