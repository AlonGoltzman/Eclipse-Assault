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

        public void SetDamage(float Damage)
        {
            ProjectileDamage = Damage;
        }
    }
}