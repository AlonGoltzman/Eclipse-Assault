using UnityEngine;

namespace Mgmt
{
    /// <summary>
    /// A template of a level, used to delegate work from GameMgr to other classes, for a cleaner, more scaleable code.
    /// </summary>
    public abstract class Level : MonoBehaviour
    {
        /// <summary>
        /// The current player's state.
        /// </summary>
        public StateInRuntime PlayerState;



        /// <summary>
        /// Saves the current state, should be used after every time the player dies.
        /// </summary>
        public virtual void SaveState()
        {
            PlayerState.Save();
        }

    }
}
