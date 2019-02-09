using Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mgmt
{
    public class GameMgr : MonoBehaviour
    {

        /// <summary>
        /// How many space units are moved per each iteration of the coroutine.
        /// </summary>
        public float UnitsPerMovement;

        /// <summary>
        /// The time between each movement action
        /// </summary>
        public float TimeBetweenMovements;

        /// <summary>
        /// Time between spawn of each enemy.
        /// </summary>
        public float TimeBetweenEnemySpawn;

        /// <summary>
        /// The enemy to spawn.
        /// </summary>
        public GameObject Enemy;

        /// <summary>
        /// The spawn points for enemies.
        /// </summary>
        public GameObject[] EnemyWalls;

        /// <summary>
        /// The game manager instance.
        /// </summary>
        private static GameMgr _instance;

        /// <summary>
        /// Time since the last enemy spawned.
        /// </summary>
        private float TimeSinceLastSpawn = 0;

        /// <summary>
        /// Is the current level the menu level.
        /// </summary>
        private bool Menu;

        /// <summary>
        /// Is the current levle the arena level.
        /// </summary>
        private bool Arena;

        /// <summary>
        /// Is swipe control enabled.
        /// </summary>
        [HideInInspector]
        public static bool TiltControl;

        /// <summary>
        /// Has the level changed in the prepare method.
        /// </summary>
        private bool LevelChanged;

        /// <summary>
        /// The player's state.
        /// Contains points and purchased information
        /// </summary>
        private State PlayerState;

        /// <summary>
        /// The text field which indicates how many points the player has.
        /// </summary>
        private Text PointsText;

        private void Awake()
        {

            if (_instance == null)
            {
                _instance = this;
                StateSaver.LoadState(out PlayerState);
            }
            else if (_instance != this)
            {
                // Destory this game object if a game manager exists already.
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            GameConstants.PREFAB_DAMAGE_PS = Resources.Load(GameConstants.PREFAB_PATH_DAMAGE_PARTICLE_SYSTEM, typeof(GameObject)) as GameObject;
        }

        private void Update()
        {
            if (Arena)
                UpdateArena();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Prepare();
        }


        /// <summary>
        /// The update function used for the Arena level.
        /// </summary>
        private void UpdateArena()
        {
            if (TimeSinceLastSpawn <= 0)
            {
                GameObject NewEnemy = Instantiate(Enemy);

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
                NewEnemyController.PointsForDestruction = (int)Random.Range(0, 50);

                TimeSinceLastSpawn = TimeBetweenEnemySpawn;
            }
            else
            {
                TimeSinceLastSpawn -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Moves all the objects tagged with Moveables a certain amount of units.
        /// </summary>
        /// <returns></returns>
        private IEnumerator MoveMoveables()
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
        /// Prepares the game manager according to the loaded level.
        /// </summary>
        private void Prepare()
        {
            bool LevelIsMenu = SceneManager.GetActiveScene().name.Equals(GameConstants.LEVEL_NAME_MENU);
            bool LevelIsArena = SceneManager.GetActiveScene().name.Equals(GameConstants.LEVEL_NAME_ARENA);

            LevelChanged = (LevelIsArena && Menu) || (LevelIsMenu && Arena);

            if (!LevelChanged && !Menu && !Arena)
                LevelChanged = true;

            if (!LevelChanged) return;

            Menu = LevelIsMenu;
            Arena = LevelIsArena;

            if (Menu)
            {
#if !UNITY_ANDROID
                Destroy(GameObject.Find(GameConstants.UI_MENU_NAME_TILT_CONTROL));
#endif
                Toggle toggle = GameObject.Find(GameConstants.UI_MENU_NAME_TILT_CONTROL).GetComponent<Toggle>();
                TiltControl = toggle.isOn;

                Button PlayButton = GameObject.Find(GameConstants.UI_MENU_NAME_PLAY_BUTTON).GetComponent<Button>();
                PlayButton.onClick.AddListener(Play);
            }

            if (Arena)
            {
                DefineHealthGradient();

                Bounds GroundBounds = GameObject.Find(GameConstants.NAME_GROUND).GetComponent<SpriteRenderer>().bounds;
                GameConstants.POSITION_Y_GROUND = (GroundBounds.center + GroundBounds.extents).y;

                ConfigureEnemySpawnPoints();

                ConfigurePointsView();

                StartCoroutine("MoveMoveables");
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
        public void ConfigurePointsView()
        {
            PointsText = GameObject.Find(GameConstants.UI_ARENA_NAME_POINTS).GetComponent<Text>();
            PointsText.text = string.Format(GameConstants.TEXT_POINTS, PlayerState.Points);
        }

        /// <summary>
        /// Loads the arena level.
        /// </summary>
        public void Play()
        {
            if (Menu)
            {
                SceneManager.LoadScene(GameConstants.LEVEL_NAME_ARENA, LoadSceneMode.Single);
                return;
            }
        }

        /// <summary>
        /// Toggles the tilt control if you are in the menu level.
        /// </summary>
        public void ToggleTiltControls()
        {
            if (!Menu) return;
            TiltControl = !TiltControl;
        }

        /// <summary>
        /// Notifys that an enemy has been destroyed.
        /// </summary>
        /// <param name="PointsAdded">How many points is the enemy worth.</param>
        public void DestroyedEnemy(int PointsAdded)
        {
            PlayerState.AddPoints(PointsAdded);
            PointsText.text = string.Format(GameConstants.TEXT_POINTS, PlayerState.Points);
        }

        /// <summary>
        /// Saves the current state, should be used after every time the player dies.
        /// </summary>
        public void SaveState()
        {
            StateSaver.SaveState(PlayerState);
        }
    }

    public static class GameConstants
    {
        public static readonly string BUTTON_SHOOT = "SHOOT";
        public static readonly string BUTTON_MOVE_RIGHT = "RIGHT";
        public static readonly string BUTTON_MOVE_LEFT = "LEFT";

        public static readonly string NAME_PLAYER = "Player";
        public static readonly string NAME_ENEMY = "Blimp";
        public static readonly string NAME_BULLET_PLAYER = "BulletPlayer";
        public static readonly string NAME_HEALTH_BAR = "HealthBar";
        public static readonly string NAME_HEALTH_BAR_CONTAINER = "Bar";
        public static readonly string NAME_HEALTH_BAR_CONTAINER_SPIRTE = "BarSprite";
        public static readonly string NAME_GROUND = "Ground";
        public static readonly string NAME_ENEMY_BOMB_DROP_POINT = "BombDropPoint";
        public static readonly string NAME_CAMERA_CONTAINER = "CameraContainer";
        public static readonly string NAME_ENEMY_SPAWN = "EnemyWall";
        public static readonly string NAME_ENEMY_SPAWN_CONTAINER = "EnemySpawnContainer";
        public static readonly string NAME_GAME_MANAGER = "GameMgr";

        public static readonly string LEVEL_NAME_MENU = "Menu";
        public static readonly string LEVEL_NAME_ARENA = "Arena";

        public static readonly string UI_MENU_NAME_MENU = "UI Menu";
        public static readonly string UI_MENU_NAME_TILT_CONTROL = "Tilt Control";
        public static readonly string UI_MENU_NAME_PLAY_BUTTON = "Play Button";

        public static readonly string UI_ARENA_NAME_POINTS = "Points Text";

        public static readonly string TAG_MOVEABLES = "Moveables";
        public static readonly string TAG_ENEMY_SPAWN = "EnemySpawn";

        public static readonly string TEXT_POINTS = "Points:{0}";

        public static readonly string PREFAB_PATH_DAMAGE_PARTICLE_SYSTEM = "Prefabs/DamageParticleSystem";

        public static readonly int SpeedMagnitudeReduction = 25;

        public static Gradient HEALTH_BAR_GRADIENT = new Gradient();

        public static readonly GradientColorKey GREEN = new GradientColorKey(Color.green, 0);
        public static readonly GradientColorKey YELLOW = new GradientColorKey(new Color(1, 1, 0, 1), 0.5f);
        public static readonly GradientColorKey RED = new GradientColorKey(Color.red, 1);

        public static readonly GradientAlphaKey ALPHA = new GradientAlphaKey(1, 1);

        public static readonly float PIXELS_PER_UNIT = 100f;

        public static float POSITION_Y_GROUND;

        public static float X_MIDDLE_OF_SCREEN = Screen.width / 2;
        public static float Y_MIDDLE_OF_SCREEN = Screen.height / 2;

        public static GameObject PREFAB_DAMAGE_PS;
    }

    public static class GameStatistics
    {
        /// <summary>
        /// The amount of bullets that were shot in the duration of the game.
        /// </summary>
        public static int BulletsShot = 0;

        /// <summary>
        /// The number of enemies created in this level.
        /// </summary>
        public static int EnemiesCreated = 0;

        /// <summary>
        /// The number of enemies destroyed.
        /// </summary>
        public static int EnemiesDestroyed = 0;
    }
}

