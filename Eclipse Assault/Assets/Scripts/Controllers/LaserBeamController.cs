using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class LaserBeamController : ProjectileController
    { 
        /// <summary>
        /// How many sprites of the laser beam is needed to have a width of 1 unit.
        /// </summary>
        private float ScaleToOneDistanceUnit;

        void Start()
        {
            SpriteRenderer Renderer = GetComponent<SpriteRenderer>();
            Bounds SpriteBounds = Renderer.bounds;
            Vector3 LeftCenter = SpriteBounds.center;
            //Times 2 due to the fact that extends are half of the bounds.
            Vector3 Size = SpriteBounds.extents *2;
            float ActualSize = Mathf.Abs(Size.x);

            ScaleToOneDistanceUnit = 1 / ActualSize;
        }


        /// <summary>
        /// Expands the laser to a specific point in space.
        /// </summary>
        /// <param name="dst"></param>
        public void ExpandBeam(Vector2 dst)
        {
            Vector3 MyPosition = transform.position;
            float Delta = Vector2.Distance(new Vector2(MyPosition.x, MyPosition.y), dst);

            transform.localScale = new Vector3(Delta * ScaleToOneDistanceUnit, 1, 1);
        }

        void Update()
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

    }
}