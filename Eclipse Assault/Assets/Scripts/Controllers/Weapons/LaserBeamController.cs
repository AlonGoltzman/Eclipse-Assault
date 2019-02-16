using System.Collections;
using UnityEngine;

namespace Controllers
{
    public class LaserBeamController : ConsecutiveShotProjectile
    { 
        /// <summary>
        /// How many sprites of the laser beam is needed to have a width of 1 unit.
        /// Ref: Scaling of laser - Resolution changes - No fix
        /// </summary>
        private float ScaleToOneDistanceUnit;

        private float TimeSinceLastHit = 1/256f;

        void Start()
        {
            //SpriteRenderer Renderer = GetComponent<SpriteRenderer>();
            //Bounds SpriteBounds = Renderer.bounds;
            //Vector3 LeftCenter = SpriteBounds.center;
            ////Times 2 due to the fact that extends are half of the bounds.
            //Vector3 Size = SpriteBounds.extents *2;
            //float ActualSize = Mathf.Abs(Size.x);

            //ScaleToOneDistanceUnit = 1 / ActualSize;
            // Scaling of laser - W\A
            ScaleToOneDistanceUnit = 6.25f;

            StartCoroutine("ShrinkLaser");
        }

        void Update()
        {
            TimeSinceLastHit -= Time.deltaTime;
        }

        /// <summary>
        /// Expands the laser to a specific point in space.
        /// </summary>
        /// <param name="dst"></param>
        public override void ExpandProjectile(Vector2 dst)
        {
            TimeSinceLastHit = 1 / 256f;
            Vector3 MyPosition = transform.position;
            float Delta = Vector2.Distance(new Vector2(MyPosition.x, MyPosition.y), dst);
            transform.localScale = new Vector3(Delta * ScaleToOneDistanceUnit, 1, 1);
        }

        /// <summary>
        /// A Coroutine that is used to shrink the laser back.
        /// After 0.5 seconds of no hits laser shrinks back down.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShrinkLaser()
        {
            while (true)
            {
                if(TimeSinceLastHit <= 0)
                {
                    transform.localScale = Vector3.one;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}