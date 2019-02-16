using Controllers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Mgmt
{
    public class LevelArena : Level
    {

        /// <summary>
        /// How many space units are moved per each iteration of the coroutine.
        /// </summary>
        private float UnitsPerMovement;

        /// <summary>
        /// The time between each movement action
        /// </summary>
        private float TimeBetweenMovements;

        /// <summary>
        /// Time between spawn of each enemy.
        /// </summary>
        private float TimeBetweenEnemySpawn;

        /// <summary>
        /// The enemy to spawn.
        /// </summary>
        private GameObject Enemy;

        /// <summary>
        /// The spawn points for enemies.
        /// </summary>
        private GameObject[] EnemyWalls;

        /// <summary>
        /// Time since the last enemy spawned.
        /// </summary>
        private float TimeSinceLastSpawn = 0;

        /// <summary>
        /// The text field which indicates how many points the player has.
        /// </summary>
        private Text PointsText;

        private void Start()
        {
            Enemy = Resources.Load(GameConstants.PREFAB_PATH_ENEMY, typeof(GameObject)) as GameObject;

            PointsText = GameObject.Find(GameConstants.UI_ARENA_NAME_POINTS).GetComponent<Text>();

            TimeBetweenEnemySpawn = Mathf.Round(Random.Range(3, 7));
            TimeBetweenMovements = 1f / 64f;
            UnitsPerMovement = 1f / 128f;

            DefineHealthGradient();

            Bounds GroundBounds = GameObject.Find(GameConstants.NAME_GROUND).GetComponent<SpriteRenderer>().bounds;
            GameConstants.POSITION_Y_GROUND = (GroundBounds.center + GroundBounds.extents).y;

            ConfigureEnemySpawnPoints();

            ConfigurePointsView();

            WeaponController PlayerWeaponController = GameObject.Find(GameConstants.NAME_PLAYER).GetComponentInChildren<WeaponController>();
            PlayerWeaponController.SetWeaponStats(PlayerState.SelectedWeapon);

            StartCoroutine("MoveMoveables");
        }

        private void Awake()
        {
            
        }

        private void Update()
        {
            if (TimeSinceLastSpawn <= 0)
            {
                GameObject NewEnemy = GameObject.Instantiate(Enemy);

                bool Left = Random.Range(0, 2) == 0;

                GameObject SpawnWall = EnemyWalls[Left ? 0 : 1];

                float Y = Random.Range(SpawnWall.transform.position.y, SpawnWall.transform.position.y + 1);

                float TimeBetweenBombDrop = Random.Range(1, 4);

                NewEnemy.name = GameConstants.NAME_ENEMY + GameStatistics.EnemiesCreated++;
                NewEnemy.transform.position = new Vector3(SpawnWall.transform.position.x + UnitsPerMovement * 3 * (Left ? 1 : -1), Y, 0);

                EnemyController NewEnemyController = NewEnemy.GetComponent<EnemyController>();

                NewEnemyController.SetDespawnWall(EnemyWalls[Left ? 1 : 0]);
                NewEnemyController.SetSpeed(UnitsPerMovement * 1.5f * (Left ? 1 : -1));
                NewEnemyController.BombCoolDownTime = TimeBetweenBombDrop;
                NewEnemyController.PointsForDestruction = 1000000;

                TimeSinceLastSpawn = TimeBetweenEnemySpawn;
            }
            else
            {
                TimeSinceLastSpawn -= Time.deltaTime;
            }
        }


        /// <summary>
        /// Defines all the values of the health bar gradient
        /// </summary>
        private void DefineHealthGradient()
        {
            GradientColorKey[] CKeys = new GradientColorKey[3];
            CKeys[0] = GameConstants.RED;
            CKeys[1] = GameConstants.YELLOW;
            CKeys[2] = GameConstants.GREEN;

            GradientAlphaKey[] AKeys = new GradientAlphaKey[3];
            AKeys[0] = AKeys[1] = AKeys[2] = GameConstants.ALPHA;

            GameConstants.HEALTH_BAR_GRADIENT.SetKeys(CKeys, AKeys);
        }

        /// <summary>
        /// Configures the "EnemyWalls" objects, which indicate spawn and destruction of an enemy.
        /// </summary>
        private void ConfigureEnemySpawnPoints()
        {
            EnemyWalls = GameObject.FindGameObjectsWithTag(GameConstants.TAG_ENEMY_SPAWN);
            if (!EnemyWalls[0].name.Equals(GameConstants.NAME_ENEMY_SPAWN + "1"))
            {
                GameObject tmp = EnemyWalls[0];
                EnemyWalls[0] = EnemyWalls[1];
                EnemyWalls[1] = tmp;
            }

            EnemyWalls[0].transform.position = new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(-0.5f, 0, 10)).x, EnemyWalls[0].transform.position.y, 0);
            EnemyWalls[1].transform.position = new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(1.5f, 0, 10)).x, EnemyWalls[0].transform.position.y, 0);
        }

        /// <summary>
        /// Configures the view indicating how many points the user currently has.
        /// </summary>
        protected void ConfigurePointsView()
        { 
            PointsText.text = string.Format(GameConstants.TEXT_POINTS, PlayerState.Points);
        }

        /// <summary>
        /// Moves all the objects tagged with Moveables a certain amount of units.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator MoveMoveables()
        {
            while (true)
            {
                var Moveables = GameObject.FindGameObjectsWithTag(GameConstants.TAG_MOVEABLES);
                foreach (GameObject obj in Moveables)
                {
                    obj.transform.position = new Vector3(obj.transform.position.x + UnitsPerMovement, obj.transform.position.y, obj.transform.position.z);
                }
                yield return new WaitForSeconds(TimeBetweenMovements);
            }
        }

        /// <summary>
        /// Notifys that an enemy has been destroyed.
        /// </summary>
        /// <param name="pointsAdded"></param>
        public void DestroyedEnemy(int PointsAdded)
        {
            PlayerState.AddPoints(PointsAdded);
            PointsText.text = string.Format(GameConstants.TEXT_POINTS, PlayerState.Points);
        }
    }
}
