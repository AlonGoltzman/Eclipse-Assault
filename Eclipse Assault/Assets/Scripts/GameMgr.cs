using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        /// An on-screen button for android.
        /// </summary>
        public GameObject AndroidButton;

        /// <summary>
        /// An on-screen button for IOS.
        /// </summary>
        public GameObject IOSButton;

        /// <summary>
        /// The game manager instance.
        /// </summary>
        private static GameMgr _instance;

        private void Awake()
        {

            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                // Destory this game object if a game manager exists already.
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);

            GradientColorKey[] CKeys = new GradientColorKey[3];
            CKeys[0] = GameConstants.RED;
            CKeys[1] = GameConstants.YELLOW;
            CKeys[2] = GameConstants.GREEN; 

            GradientAlphaKey[] AKeys = new GradientAlphaKey[3];
            AKeys[0] = AKeys[1] = AKeys[2] = GameConstants.ALPHA;

            GameConstants.HEALTH_BAR_GRADIENT.SetKeys(CKeys, AKeys);

            Bounds GroundBounds = GameObject.Find(GameConstants.NAME_GROUND).GetComponent<SpriteRenderer>().bounds;
            GameConstants.POSITION_Y_GROUND = (GroundBounds.center + GroundBounds.extents).y;

#if UNITY_ANDROID
            GameObject CameraContainer = GameObject.Find(GameConstants.NAME_CAMERA_CONTAINER);
            Game Object NewAndroidButton = Instantiate(AndroidButton);
            NewAndroidButton.transform.parent = CameraContainer.transform;
#endif

            StartCoroutine("MoveMoveables");
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
    }

    public static class GameConstants
    {
        public static readonly string BUTTON_SHOOT = "SHOOT";
        public static readonly string BUTTON_MOVE_RIGHT = "RIGHT";
        public static readonly string BUTTON_MOVE_LEFT = "LEFT";

        public static readonly string NAME_PLAYER = "Player";
        public static readonly string NAME_BULLET_PLAYER = "BulletPlayer";
        public static readonly string NAME_HEALTH_BAR = "HealthBar";
        public static readonly string NAME_HEALTH_BAR_CONTAINER = "Bar";
        public static readonly string NAME_HEALTH_BAR_CONTAINER_SPIRTE = "BarSprite";
        public static readonly string NAME_GROUND = "Ground";
        public static readonly string NAME_ENEMY_BOMB_DROP_POINT = "BombDropPoint";
        public static readonly string NAME_CAMERA_CONTAINER = "CameraContainer";

        public static readonly string TAG_MOVEABLES = "Moveables";

        public static readonly int SpeedMagnitudeReduction = 25;

        public static Gradient HEALTH_BAR_GRADIENT = new Gradient();

        public static readonly GradientColorKey GREEN = new GradientColorKey(Color.green, 0);
        public static readonly GradientColorKey YELLOW = new GradientColorKey(new Color(1, 1, 0, 1), 0.5f);
        public static readonly GradientColorKey RED = new GradientColorKey(Color.red,1);

        public static readonly GradientAlphaKey ALPHA = new GradientAlphaKey(1, 1);

        public static readonly float PIXELS_PER_UNIT = 100f;

        public static float POSITION_Y_GROUND;
    }

    public static class GameStatistics
    {
        /// <summary>
        /// The amount of bullets that were shot in the duration of the game.
        /// </summary>
        public static int BulletsShot = 0;
    }
}

