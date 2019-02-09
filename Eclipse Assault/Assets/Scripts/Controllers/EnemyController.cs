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
        /// How many points does the player get for the destruction of this enemy.
        /// </summary>
        public int PointsForDestruction;

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

        /// <summary>
        /// A flag to identify if a particle system to indicate damage can be created.
        /// </summary>
        private bool CanCreatePS = true;

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
                //For PC, due to the fact that it is more powerful, the movment speed needs to be reduced.
                //More power = faster FPS = more physics ticks = faster movement from enemy.
#if UNITY_STANDALONE
                transform.position = new Vector3(transform.position.x + MovementSpeed/2, transform.position.y, transform.position.z);
#else
                transform.position = new Vector3(transform.position.x + MovementSpeed, transform.position.y, transform.position.z);
#endif
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
            NewBomb.name = "Bomb" + name;
            NewBomb.transform.position = ExitPoint.position;
            NewBomb.GetComponent<BombController>().SetDamage(Damage);
            RemainingCooldown = BombCoolDownTime;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Contains(GameConstants.NAME_BULLET_PLAYER))
            {
                float Damage = other.gameObject.GetComponent<ProjectileController>().Damage;
                Vector2 HitPoint = other.transform.position;
                Damaged(Damage, HitPoint,false);
                
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

        /// <summary>
        /// Indicates that this entity has been hit as a certain position, causing damage.
        /// </summary>
        /// <param name="DamageDone">How much damage was done.</param>
        /// <param name="HitPosition">Where the hit occurred.</param>
        /// <param name="ReduceAmount">Should we reduce the amount of particles? Due to laser beam damage</param>
        private void Damaged(float DamageDone, Vector2 HitPosition, bool ReduceAmount)
        {
            //Broadcasts to the healthbar component that damage was done.
            BroadcastMessage("Hit", DamageDone);

            if (!CanCreatePS) { return; }

            SpriteRenderer Renderer = GetComponent<SpriteRenderer>();

            Bounds Bounds = Renderer.bounds;
            Vector3 Center = Bounds.center;
            Vector3 HalfSize = Bounds.extents;

            bool HitOnRight = HitPosition.x >= Center.x + HalfSize.x;
            bool HitOnLeft = HitPosition.x <= Center.x - HalfSize.x;

            float Angle = 0f;

            if (Center.y - HalfSize.y - HitPosition.y <= 0)
            {
                Angle = HitOnLeft ? 135f : (HitOnRight ? -135f : 180f);
            }
            else
            {
                Angle = HitOnLeft ? 90f : (HitOnRight ? -90f : 0f);
            }

            if (HitPosition.y < Center.y + HalfSize.y || HitPosition.y > Center.y - HalfSize.y)
            {
                HitPosition.y = Center.y + (HalfSize.y * (Center.y < 0 ? 1 : -1));
            }

            GameObject DamagePSGameObject = Instantiate(GameConstants.PREFAB_DAMAGE_PS);
            DamagePSGameObject.transform.SetParent(transform);
            DamagePSGameObject.transform.position = new Vector3(HitPosition.x, HitPosition.y, 0);
            DamagePSGameObject.transform.rotation = Quaternion.AngleAxis(Angle, Vector3.forward);
            if (ReduceAmount)
            {
                ParticleSystem.EmissionModule Emission= DamagePSGameObject.GetComponent<ParticleSystem>().emission;
                ParticleSystem.MinMaxCurve OverTime = Emission.rateOverTime;
                OverTime.constant = 100f;
            }
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
            Damaged(DamageDone, HitPosition, true);
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