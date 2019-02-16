using UnityEngine;

namespace Controllers
{
    public class BulletController : ProjectileController
    {

        /// <summary>
        /// The bullet's speed.
        /// The angle must be provided.
        /// </summary>
        public int Speed;

        /// <summary>
        /// How many frames can this bullet live.
        /// Ideally we would like 60FPS, which means anywhere from 2 to 5 seconds.
        /// </summary>
        [Range(5, 30)]
        public int MaxSeconds;

        /// <summary>
        /// The amount of frames this bullet has left to live.
        /// </summary>
        private float SecondsLeft;

        /// <summary>
        /// The angle at which this bullet was shot.
        /// </summary>
        private float Angle = int.MinValue;

        /// <summary>
        /// How many space units will the bullet move in the X axis per frame.
        /// </summary>
        private float MovementX;

        /// <summary>
        /// How many space units will the bullet move in the Y axis per frame.
        /// </summary>
        private float MovementY;


        void Start()
        {
            SecondsLeft = MaxSeconds;
        }

        public void SetAngle(float angle)
        {

            Angle = angle;

            var Movement = Quaternion.AngleAxis(Angle, Vector3.forward) * Vector3.right * Speed;

            Movement.z = 62.5f;
            GetComponent<Rigidbody>().AddForce(Movement);
            //MovementX = Movement.x / GameConstants.SpeedMagnitudeReduction;
            //MovementY = Movement.y / GameConstants.SpeedMagnitudeReduction;

        }

        // Update is called once per frame
        void Update()
        {
            if (SecondsLeft > 0)
            {
                DoMovement();
            }
            else
            {
                Destroy(gameObject);
            }

            SecondsLeft -= Time.deltaTime;
        }

        /// <summary>
        /// Performs the bullet's movement accoridng to the angle provided.
        /// </summary>
        private void DoMovement()
        {
            if (Angle == int.MinValue)
            {
                Debug.LogErrorFormat("No Angle provided for bullet {0}", gameObject.name);
                Destroy(gameObject);
                return;
            }

            //transform.position = new Vector3(transform.position.x + MovementX, transform.position.y + MovementY, transform.);
        }
    }
}