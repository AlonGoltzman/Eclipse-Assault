using Controllers;
using Mgmt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class EnemyController : MonoBehaviour
    {
        /// <summary>
        /// The blimp's movement speed.
        /// </summary>
        public float MovementSpeed;

        /// <summary>
        /// The time between each movement.
        /// </summary>
        public float TimeBetweenMovements;

        /// <summary>
        /// How long does the blimp have between dropping bombs.
        /// </summary>
        public float BombCoolDownTime;

        /// <summary>
        /// The bomb that this enemy drops.
        /// </summary>
        public GameObject Bomb;

        /// <summary>
        /// The damage that this enemy does.
        /// </summary>
        public float Damage;

        /// <summary>
        /// How long until the blimp can actually drop a bomb.
        /// </summary>
        private float RemainingCooldown = 0;

        /// <summary>
        /// The bomb's exit point.
        /// </summary>
        private Transform ExitPoint;

        /// <summary>
        /// The wall at which this blimp will despawn.
        /// </summary>
        private GameObject DespawnWall;

        // Update is called once per frame
        void Start()
        {
            for(int i =0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.name.Contains(GameConstants.NAME_ENEMY_BOMB_DROP_POINT))
                {
                    ExitPoint = transform.GetChild(i);
                    break;
                }
            }
            StartCoroutine("DoMovement");
        }

        void Update()
        {
            if (RemainingCooldown > 0) RemainingCooldown -= Time.deltaTime;
            DoDropBomb();

            if ((MovementSpeed < 0 && transform.position.x <= DespawnWall.transform.position.x) ||
                (MovementSpeed > 0 && transform.position.x >= DespawnWall.transform.position.x))
            {
                Destroy(gameObject);
            }
       
        }

        /// <summary>
        /// Move the blimp either left or right, according to the defined movement speed.
        /// </summary>
        private IEnumerator DoMovement()
        {
            while (true)
            {
                transform.position = new Vector3(transform.position.x + MovementSpeed, transform.position.y, transform.position.z);
                yield return new WaitForSeconds(TimeBetweenMovements);
            }
        }

        /// <summary>
        /// The Enemy will consider whether or not to drop a bomb now.
        /// </summary>
        private void DoDropBomb()
        {
            if (RemainingCooldown > 0) return;
            GameObject NewBomb = Instantiate(Bomb);
            NewBomb.transform.position = ExitPoint.position;
            NewBomb.GetComponent<BombController>().SetDamage(Damage);
            RemainingCooldown = BombCoolDownTime;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Contains(GameConstants.NAME_BULLET_PLAYER))
            {
                gameObject.GetComponentInChildren<HealthBarController>().Hit(other.gameObject);
            }
        }

        public void SetSpeed(float speed)
        {
            MovementSpeed = speed;
        }

        public void SetDespawnWall(GameObject gameObject)
        {
            DespawnWall = gameObject;
        }
    }
}