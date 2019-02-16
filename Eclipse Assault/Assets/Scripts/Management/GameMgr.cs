using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mgmt
{
    public class GameMgr : MonoBehaviour
    {

        ///// <summary>
        ///// How many space units are moved per each iteration of the coroutine.
        ///// </summary>
        //public float UnitsPerMovement;

        ///// <summary>
        ///// The time between each movement action
        ///// </summary>
        //public float TimeBetweenMovements;

        ///// <summary>
        ///// Time between spawn of each enemy.
        ///// </summary>
        //public float TimeBetweenEnemySpawn;

        ///// <summary>
        ///// The enemy to spawn.
        ///// </summary>
        //public GameObject Enemy;

        ///// <summary>
        ///// The spawn points for enemies.
        ///// </summary>
        //public GameObject[] EnemyWalls;

        /// <summary>
        /// The game manager instance.
        /// </summary>
        private static GameMgr Instance;

        ///// <summary>
        ///// Time since the last enemy spawned.
        ///// </summary>
        //private float TimeSinceLastSpawn = 0;

        ///// <summary>
        ///// Is the current level the menu level.
        ///// </summary>
        //private bool Menu;

        ///// <summary>
        ///// Is the current levle the arena level.
        ///// </summary>
        //private bool Arena;

        /// <summary>
        /// Is swipe control enabled.
        /// </summary>
        [HideInInspector]
        public bool TiltControl;

        ///// <summary>
        ///// Has the level changed in the prepare method.
        ///// </summary>
        //private bool LevelChanged;

        ///// <summary>
        ///// The player's state.
        ///// Contains points and purchased information
        ///// </summary>
        //private State PlayerState;

        ///// <summary>
        ///// The text field which indicates how many points the player has.
        ///// </summary>
        //private Text PointsText;

        /// <summary>
        /// The current Loaded Level.
        /// </summary>
        private Level LoadedLevel;

        [HideInInspector]
        public StateInRuntime PlayerState;

        private void Awake()
        {

            if (Instance == null)
            {
                Instance = this;
                GameConstants.PREFAB_GAME_MANAGER = Instance;
            }
            else if (Instance != this)
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

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (Level LevelScript in GetComponents<Level>())
            {
                Destroy(LevelScript);
            }

            if (scene.name.Equals(GameConstants.LEVEL_NAME_MENU))
            {
                LoadedLevel = gameObject.AddComponent<LevelMenu>();
            }
            else if (scene.name.Equals(GameConstants.LEVEL_NAME_STORE))
            {
                LoadedLevel = gameObject.AddComponent<LevelShop>();
            }
            else if (scene.name.Equals(GameConstants.LEVEL_NAME_ARENA))
            {
                LoadedLevel = gameObject.AddComponent<LevelArena>();
            }
            GameConstants.CURRENT_LEVEL = LoadedLevel;
            LoadedLevel.PlayerState = PlayerState;
        }
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

