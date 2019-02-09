using Mgmt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class ExplosionController : MonoBehaviour
    {
        /// <summary>
        /// The damage the explosion creates.
        /// </summary>
        public float Damage;

        /// <summary>
        /// The time for the explosion to occur.
        /// </summary>
        public float TimeForExplosion;

        /// <summary>
        /// The max size the explosiion will reach.
        /// </summary>
        private float MaxSize = 5;

        /// <summary>
        /// The time that passed since the explosion started.
        /// </summary>
        private float TimePastSinceStartOfExplosion = 0;

        /// <summary>
        /// All hit game objects in order to avoid duplications.
        /// </summary>
        private IList<string> HitGameObjects = new List<string>();

        // Update is called once per frame
        void Update()
        {
            DoExpand();
        }

        /// <summary>
        /// Expands the exploision size.
        /// </summary>
        private void DoExpand()
        {
            if (TimePastSinceStartOfExplosion > TimeForExplosion) Destroy(gameObject);

            float NewSize = Mathf.Lerp(1, MaxSize, TimePastSinceStartOfExplosion / TimeForExplosion);
            transform.localScale = new Vector3(NewSize, NewSize, 1);
            TimePastSinceStartOfExplosion += Time.deltaTime;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.name.Contains(GameConstants.NAME_PLAYER) && !HitGameObjects.Contains(collision.gameObject.name))
            {
                SpriteRenderer Renderer = GetComponent<SpriteRenderer>();

                Bounds CurrentBounds = Renderer.bounds;
                Vector3 Center = CurrentBounds.center;
                Vector3 HalfSize = CurrentBounds.extents;

                Vector3 HitPosition = new Vector3(0, int.MinValue, 0);

                if (collision.gameObject.transform.position.x > transform.position.x)
                {
                    HitPosition.x = Center.x + HalfSize.x;
                }
                else
                {
                    HitPosition.x = Center.x - HalfSize.x;
                }
                HitGameObjects.Add(collision.gameObject.name);
                collision.gameObject.BroadcastMessage("DamagedExternal", new object[] { Damage, HitPosition });
            }
        }
    }
}