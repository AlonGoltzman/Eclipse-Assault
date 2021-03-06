﻿using Mgmt;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class HealthBarController : MonoBehaviour
    {
        /// <summary>
        /// The character's HP.
        /// </summary>
        public float HP;

        /// <summary>
        /// The initial HP of the enemy, used to define the health bar position and color.
        /// </summary>
        private float InitialHP;

        /// <summary>
        /// The Health Bar game object, used to define the current HP.
        /// </summary>
        private GameObject HealthBar;

        /// <summary>
        /// The health barr sprite itself, used to change the name.
        /// </summary>
        private GameObject HealthBarSprite;

        // Start is called before the first frame update
        void Start()
        {
            InitialHP = HP;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.Contains(GameConstants.NAME_HEALTH_BAR_CONTAINER))
                {
                    Transform bar = transform.GetChild(i);
                    HealthBar = bar.transform.gameObject;
                    HealthBarSprite = HealthBar.transform.GetChild(0).gameObject;
                }
            }

            if (HealthBar == null) Debug.LogErrorFormat("For {0} there is no HealthBar object.", name);
            if (HealthBarSprite == null) Debug.LogErrorFormat("For {0} there is no HealthBar object.", name);
        }

        public void Hit(GameObject other)
        {
            float Damage = other.GetComponent<ProjectileController>().Damage;
            Destroy(other.gameObject);
            Hit(Damage);
        }

        public void Hit(float Damage)
        {
            HP -= Damage;
            float LeftHPPercent = ((InitialHP - HP) / InitialHP);
            HealthBar.transform.localScale = new Vector3(1 - LeftHPPercent, HealthBar.transform.localScale.y, 1);
            HealthBarSprite.GetComponent<SpriteRenderer>().color = GameConstants.HEALTH_BAR_GRADIENT.Evaluate(LeftHPPercent);

            if (HP <= 0)
            {
                LevelArena CurrentLevel = (LevelArena)GameConstants.CURRENT_LEVEL;
                if (transform.parent.gameObject.name.Contains(GameConstants.NAME_ENEMY))
                {
                    GameStatistics.EnemiesDestroyed++;
                    CurrentLevel.DestroyedEnemy(transform.parent.gameObject.GetComponent<EnemyController>().PointsForDestruction);
                }
                if (transform.parent.gameObject.name.Equals(GameConstants.NAME_PLAYER))
                {
                    CurrentLevel.SaveState();
                    SceneManager.LoadScene(GameConstants.LEVEL_NAME_MENU, LoadSceneMode.Single);
                }
                Destroy(transform.parent.gameObject);
            }
        }
    }
}