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
        /// The touch on the screen used to move the player.
        /// Used for non-tilt controls.
        /// </summary>
        private Vector2 MoveTouch = -Vector2.one;
#endif 

        void Start()
        {
            Vector3 LeftPoint = new Vector3(-0.64f, 0);
            Vector3 RightPoint = new Vector3(0.64f, 0);
            Vector3 LeftPointViewport = Camera.main.WorldToViewportPoint(LeftPoint);
            Vector3 RightPointViewport = Camera.main.WorldToViewportPoint(RightPoint);

            SizeOfCharacterInViewportCoords = RightPointViewport.x - LeftPointViewport.x;
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
            if (!GameMgr.TiltControl)
            {
                MoveRight = Input.acceleration.x > 0.1;
                MoveLeft = Input.acceleration.x < -0.1;
            }
            else if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.touches[i];

                    if (touch.position.y < GameConstants.Y_MIDDLE_OF_SCREEN)
                    {
                        if (MoveTouch.x < 0)
                        {
                            MoveTouch = touch.position;
                        }
                        else
                        {
                            MoveTouch = -Vector2.one;
                            break;
                        }
                    }
                }

                if (MoveTouch.x > 0)
                {
                    MoveRight = MoveTouch.x > GameConstants.X_MIDDLE_OF_SCREEN;
                    MoveLeft = MoveTouch.x <= GameConstants.X_MIDDLE_OF_SCREEN;
                    MoveTouch = -Vector2.one;
                }

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
    }
}