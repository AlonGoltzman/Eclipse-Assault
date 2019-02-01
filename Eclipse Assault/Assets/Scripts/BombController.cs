using Mgmt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class BombController : ProjectileController
    {


        /// <summary>
        /// The bombs initial speed.
        /// </summary>
        public float MinSpeed;

        /// <summary>
        /// The bombs maximum speed.
        /// </summary>
        public float MaxSpeed;

        /// <summary>
        /// Time it takes for a bomb to explode.
        /// </summary>
        public float TimeForExplosion;

        /// <summary>
        /// The Explosion prefab, in order to simulate the shockwave.
        /// </summary>
        public GameObject Explosion;

        /// <summary>
        /// How long has it been since the bomb dropped.
        /// </summary>
        private float TimePastSinceBombDropped = 0;

        /// <summary>
        /// The bomb's current speed, is defined by lerp between min and max sped according to the time the bomb existed. 
        /// The bomb will exist only 3 seconds.
        /// </summary>
        private float Speed;

        /// <summary>
        /// The Y position in which the explosion will take place.
        /// </summary>
        private float YForExplosion;


        void Start()
        {
            Speed = MinSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.position.y <= GameConstants.POSITION_Y_GROUND)
            {
                Explode();
            }

            DoMovement();

        }

        /// <summary>
        /// Moves the bomb downwards.
        /// </summary>
        private void DoMovement()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - Speed, transform.position.z);

            Speed = Mathf.Lerp(MinSpeed, MaxSpeed, (TimePastSinceBombDropped / 3));

            TimePastSinceBombDropped += Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.name.Contains(GameConstants.NAME_PLAYER))
            {
                collision.gameObject.BroadcastMessage("Hit", gameObject);

            }
        }

        private void Explode()
        {
            GameObject ExplosionInstance = Instantiate(Explosion);
            ExplosionInstance.transform.position = new Vector3(transform.position.x, GameConstants.POSITION_Y_GROUND, 0);
            ExplosionInstance.GetComponent<ExplosionController>().Damage = Damage;
            ExplosionInstance.GetComponent<ExplosionController>().TimeForExplosion = TimeForExplosion;
            Destroy(gameObject);
        }
    }
}