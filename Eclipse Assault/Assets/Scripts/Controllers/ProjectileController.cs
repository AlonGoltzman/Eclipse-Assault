using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public abstract class ProjectileController : MonoBehaviour
    {

        /// <summary>
        /// The damage this bullet can inflict.
        /// </summary>
        private float ProjectileDamage;

        public float Damage
        {
            get { return ProjectileDamage; }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        public void SetDamage(float Damage)
        {
            ProjectileDamage = Damage;
        }
    }
}