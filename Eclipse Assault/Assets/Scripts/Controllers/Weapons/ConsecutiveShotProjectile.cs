using UnityEngine;

namespace Controllers
{
    public abstract class ConsecutiveShotProjectile : MonoBehaviour
    {
        public abstract void ExpandProjectile(Vector2 dst);

        public void ResetSize()
        {
            transform.localScale = Vector3.one;
        }
    }

}