using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mgmt
{
    public static class GameConstants
    {
        //=================================================================
        //      Input Buttons
        //=================================================================
        public static readonly string BUTTON_SHOOT = "SHOOT";
        public static readonly string BUTTON_MOVE_RIGHT = "RIGHT";
        public static readonly string BUTTON_MOVE_LEFT = "LEFT";


        //=================================================================
        //      Object Names
        //=================================================================
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

        //=================================================================
        //      Level Names
        //=================================================================
        public static readonly string LEVEL_NAME_MENU = "Menu";
        public static readonly string LEVEL_NAME_ARENA = "Arena";
        public static readonly string LEVEL_NAME_STORE = "Store";

        //=================================================================
        //      Menu Item Names
        //=================================================================
        public static readonly string UI_MENU_NAME_MENU = "UI Menu";
        public static readonly string UI_MENU_NAME_TILT_CONTROL = "Tilt Control";
        public static readonly string UI_MENU_NAME_PLAY_BUTTON = "Play Button";
        public static readonly string UI_MENU_NAME_RESET_SAVE = "Reset Save";
        public static readonly string UI_MENU_NAME_STORE_BUTTON = "Store Button";

        //=================================================================
        //      Store Items
        //=================================================================
        public static readonly string UI_STORE_WEAPON_PATH = "Data/Weapons/";
        public static readonly string UI_STORE_NAME_ITEM_DISPLAY = "Item Display";
        public static readonly string UI_STORE_NAME_ITEM_TEXT_CONTAINER = "Description Container";
        public static readonly string UI_STORE_NAME_ITEM_NAME = "Item Name";
        public static readonly string UI_STORE_NAME_ITEM_DESCRIPTION = "Item Description";
        public static readonly string UI_STORE_NAME_ITEM_PRICE = "Item Price";
        public static readonly string UI_STORE_NAME_PURCHASE_BUTTON = "Purchase Item";


        //=================================================================
        //      Game Item Names
        //=================================================================
        public static readonly string UI_ARENA_NAME_POINTS = "Points Text";

        //=================================================================
        //      Tags
        //=================================================================
        public static readonly string TAG_MOVEABLES = "Moveables";
        public static readonly string TAG_ENEMY_SPAWN = "EnemySpawn";

        //=================================================================
        //      Regex
        //=================================================================
        public static readonly string TEXT_POINTS = "Points:{0}";
        public static readonly string TEXT_ITEM_DESCRIPTION = "<i><color=BLUE>Description:</color></i>{0}\n<i>Additional Details:</i>{1}";

        //=================================================================
        //      Prefabs and GameObjects
        //=================================================================
        public static readonly string PREFAB_PATH_DAMAGE_PARTICLE_SYSTEM = "Prefabs/DamageParticleSystem";
        public static readonly string PREFAB_PATH_STORE_ITEM_TEMPLATE = "Prefabs/UI Prefabs/Item Template";
        public static GameObject PREFAB_DAMAGE_PS;

        //=================================================================
        //      Variables
        //=================================================================
        public static readonly int SPEED_REDUCTION_MAGNITUTE = 25;
        public static readonly float PIXELS_PER_UNIT = 100f;
        public static float POSITION_Y_GROUND;
        public static float X_MIDDLE_OF_SCREEN = Screen.width / 2;
        public static float Y_MIDDLE_OF_SCREEN = Screen.height / 2;

        //=================================================================
        //      Gradients
        //=================================================================
        public static Gradient HEALTH_BAR_GRADIENT = new Gradient();
        public static readonly GradientColorKey GREEN = new GradientColorKey(Color.green, 0);
        public static readonly GradientColorKey YELLOW = new GradientColorKey(new Color(1, 1, 0, 1), 0.5f);
        public static readonly GradientColorKey RED = new GradientColorKey(Color.red, 1);
        public static readonly GradientAlphaKey ALPHA = new GradientAlphaKey(1, 1);

        //=================================================================
        //      Data folder related varaibles
        //=================================================================
        public static readonly string DATA_WEAPONS = "Weapons";

    }
}
