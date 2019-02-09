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

        /// <summary>
        /// Indicates if a new PS can be created or not.
        /// </summary>
        private bool CanCreatePS = true;

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

        /// <summary>
        /// Indicates that this entity has been hit as a certain position, causing damage.
        /// </summary>
        /// <param name="DamageDone">How much damage was done.</param>
        /// <param name="HitPosition">Where the hit occurred.</param>
        /// <param name="ReduceAmount">Should we reduce the amount of particles? Due to laser beam damage</param>
        private void Damaged(float DamageDone, Vector2 HitPosition)
        {
            //Broadcasts to the healthbar component that damage was done.
            BroadcastMessage("Hit", DamageDone);

            if (!CanCreatePS) { return; }

            SpriteRenderer Renderer = GetComponent<SpriteRenderer>();

            Bounds Bounds = Renderer.bounds;
            Vector3 Center = Bounds.center;
            Vector3 HalfSize = Bounds.extents;

            if (HitPosition.y != int.MinValue)
            {
                if (HitPosition.y < Center.y + HalfSize.y && HitPosition.y > Center.y - HalfSize.y)
                {
                    HitPosition.y = Center.y + (HalfSize.y * (Center.y < 0 ? 1 : -1));
                }
            }
            else
            {
                HitPosition.y = Random.Range(Center.y - (HalfSize.y * 0.2f), Center.y + (HalfSize.y * 0.9f));
                HitPosition.x += (HitPosition.x < Center.x ? -1 : 1) * HitPosition.x / 100;
            }

            bool HitOnRight = HitPosition.x >= Center.x + HalfSize.x;
            bool HitOnLeft = HitPosition.x <= Center.x - HalfSize.x;

            float Angle = 0f;

            if (Center.y - HalfSize.y - HitPosition.y >= 0)
            {
                Angle = HitOnLeft ? 135f : (HitOnRight ? -135f : 180f);
            }
            else
            {
                Angle = HitOnLeft ? 90f : (HitOnRight ? -90f : 0f);
            }

            GameObject DamagePSGameObject = Instantiate(GameConstants.PREFAB_DAMAGE_PS);
            DamagePSGameObject.name = "DamageParticleSystemFor" + name;
            DamagePSGameObject.transform.position = new Vector3(HitPosition.x, HitPosition.y, 0);
            DamagePSGameObject.transform.rotation = Quaternion.AngleAxis(Angle, Vector3.forward);

            StartCoroutine("WaitForNewPS");
            StartCoroutine("DestroyPS", DamagePSGameObject);
            CanCreatePS = false;
        }


        /// <summary>
        /// Invokes the damaged function, used for the broadcast method.
        /// This has a different name in order to provide a different function (using the same name causes an issue).
        /// </summary>
        /// <param name="args">the values sent</param>
        public void DamagedExternal(object[] args)
        {
            float DamageDone = (float)args[0];
            Vector2 HitPosition = (Vector3)args[1];
            Damaged(DamageDone, HitPosition);
        }

        /// <summary>
        /// Destroys a particle system after 1.5 seconds.
        /// </summary>
        /// <param name="PS">the particle system to destroy</param>
        /// <returns></returns>
        private IEnumerator DestroyPS(GameObject PS)
        {
            yield return new WaitForSeconds(1.5f);
            GameObject.Destroy(PS);
            yield break;
        }

        /// <summary>
        /// Waits until 1 seconds passes in order to allow the creation of a new particle system.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForNewPS()
        {
            yield return new WaitForSeconds(0.5f);
            CanCreatePS = true;
            yield break;
        }
    }
}