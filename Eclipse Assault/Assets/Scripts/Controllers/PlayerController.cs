using Mgmt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {

        /// <summary>
        /// The player's movement speed.
        /// </summary>
        public float MovementSpeed;

        /// <summary>
        /// Input read to move right?
        /// </summary>
        private bool MoveRight = false;

        /// <summary>
        /// Input read to move left?
        /// </summary>
        private bool MoveLeft = false;

        /// <summary>
        /// In order to limit the player to the viewport we need to get exactly
        /// how many viewport units the character is.
        /// </summary>
        private float SizeOfCharacterInViewportCoords;

#if UNITY_ANDROID
        /// <summary>
        /// Is the player holding the move right button.
        /// </summary>
        private bool HoldingMoveRight;

        /// <summary>
        /// Is the player holding the mvoe left button.
        /// </summary>
        private bool HoldingMoveLeft;
#endif

        void Start()
        {
            Vector3 LeftPoint = new Vector3(-0.64f, 0);
            Vector3 RightPoint = new Vector3(0.64f, 0);
            Vector3 LeftPointViewport = Camera.main.WorldToViewportPoint(LeftPoint);
            Vector3 RightPointViewport = Camera.main.WorldToViewportPoint(RightPoint);

            SizeOfCharacterInViewportCoords = RightPointViewport.x - LeftPointViewport.x;

#if UNITY_ANDROID
            //Movement triggers
            EventTrigger MoveRight = GameObject.Find(GameConstants.UI_NAME_MOVE_RIGHT_BUTTON).GetComponent<EventTrigger>();
            EventTrigger MoveLeft = GameObject.Find(GameConstants.UI_NAME_MOVE_LEFT_BUTTON).GetComponent<EventTrigger>();

            EventTrigger.Entry startMoveLeftEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            startMoveLeftEntry.callback.AddListener(StartMoveLeft);
            EventTrigger.Entry startMoveRightEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            startMoveRightEntry.callback.AddListener(StartMoveRight);

            EventTrigger.Entry stopMoveLeftEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            stopMoveLeftEntry.callback.AddListener(StopMoveLeft);
            EventTrigger.Entry stopMoveRightEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            stopMoveRightEntry.callback.AddListener(StopMoveRight);


            MoveLeft.triggers.Add(startMoveLeftEntry);
            MoveLeft.triggers.Add(stopMoveLeftEntry);
            MoveRight.triggers.Add(startMoveRightEntry);
            MoveRight.triggers.Add(stopMoveRightEntry);
#endif
        }

        // Update is called once per frame
        void Update()
        {
            DoInput();

            ValidateMovementLimits();

            DoAction();

        }


        /// <summary>
        /// Performs a check to see if there is any input to do in this frame (or relative frame).
        /// The shoot should remain GetButtonUp due to the fact that it can only be on shoot per frame.
        /// Move Left and right and continous movements so we can recieve "as much as we want".
        /// </summary>
        private void DoInput()
        {

#if UNITY_ANDROID
            if (!GameMgr.SwipeControl)
            {
                MoveRight = Input.acceleration.x > 0.1;
                MoveLeft = Input.acceleration.x < -0.1;
            } else
            {
                MoveRight = HoldingMoveRight;
                MoveLeft = HoldingMoveLeft;
            }
#else
            MoveRight = Input.GetButton(GameConstants.BUTTON_MOVE_RIGHT);
            MoveLeft = Input.GetButton(GameConstants.BUTTON_MOVE_LEFT);

            //If move right and move left is enabled, disable the movement.
            if (MoveRight && MoveLeft)
                MoveRight = MoveLeft = false;
#endif
        }


        /// <summary>
        /// Performs actions requested in this frame (or relative frame).
        /// </summary>
        private void DoAction()
        {
            if (MoveRight)
                transform.position = new Vector3(transform.position.x + MovementSpeed, transform.position.y, transform.position.z);

            if (MoveLeft)
                transform.position = new Vector3(transform.position.x - MovementSpeed, transform.position.y, transform.position.z);

            //Reset all variables to false.
            MoveLeft = MoveRight = false;
        }

        private void ValidateMovementLimits()
        {
            Vector3 PlayerViewportPosition = Camera.main.WorldToViewportPoint(transform.position);
            if (PlayerViewportPosition.x <= (SizeOfCharacterInViewportCoords / 2))
                MoveLeft = false;

            if (PlayerViewportPosition.x >= 1 - (SizeOfCharacterInViewportCoords / 2))
                MoveRight = false;
        }

        /// <summary>
        /// Makes sure that the characterr does not pass the camera's bounds.
        /// </summary>
        private void ClampPositionToCameraBounds()
        {
            Vector3 Position;
            Vector3 TransformPosition = transform.position;
            TransformPosition.x = TransformPosition.x + 0.64f;
            Position = Camera.main.WorldToViewportPoint(TransformPosition);
            Position.x = Mathf.Clamp01(Position.x);
            if (Position.x >= 1)
            {
                TransformPosition.x = TransformPosition.x - 0.64f;
                Position = Camera.main.WorldToViewportPoint(TransformPosition);
                transform.position = Camera.main.ViewportToWorldPoint(Position);
            }
            else
            {

                TransformPosition = transform.position;
                TransformPosition.x = TransformPosition.x - 0.64f;
                Position = Camera.main.WorldToViewportPoint(TransformPosition);
                Position.x = Mathf.Clamp01(Position.x);
                if (Position.x <= 0)
                {
                    TransformPosition.x = TransformPosition.x + 0.64f;
                    Position = Camera.main.WorldToViewportPoint(TransformPosition);
                    transform.position = Camera.main.ViewportToWorldPoint(Position);

                }
                else
                {
                    TransformPosition = transform.position;
                    Position = Camera.main.WorldToViewportPoint(TransformPosition);
                    transform.position = Camera.main.ViewportToWorldPoint(Position);
                }

            }
        }

#if UNITY_ANDROID
        /// <summary>
        /// Indicate that the play should move right.
        /// </summary>
        public void StartMoveRight(BaseEventData data)
        {
#if !UNITY_EDITOR
            if(GameMgr.SwipeControl)
#endif
            HoldingMoveRight = true;
        }

        /// <summary>
        /// Indicate that the play should move right.
        /// </summary>
        public void StartMoveLeft(BaseEventData data)
        {
#if !UNITY_EDITOR
            if (GameMgr.SwipeControl)
#endif
            HoldingMoveLeft = true;
        }

        public void StopMoveLeft(BaseEventData data)
        {
#if !UNITY_EDITOR
            if(GameMgr.SwipeControl)
#endif
            HoldingMoveLeft = false;
        }

        public void StopMoveRight(BaseEventData data)
        {
#if !UNITY_EDITOR
            if(GameMgr.SwipeControl)
#endif 
            HoldingMoveRight = false;
        }
#endif
        }
}