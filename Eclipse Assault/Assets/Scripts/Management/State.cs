using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgmt
{
    [Serializable]
    public class State
    {

        /// <summary>
        /// The player's current amount of points.
        /// </summary>
        private int CurrentPoints;

        /// <summary>
        /// The player's toatl points since he started to play, includes the points wasted.
        /// </summary>
        private int TotalPoints;

        /// <summary>
        /// A placeholder.
        /// Later on the store will be introduced and the player will be able to purchase upgrades.
        /// These upgrades will be saved in the state.
        /// </summary>
        private object[] PurchasedUpgrades;

        /// <summary>
        /// Creates a new default state that is used to define that a new "save" is created.
        /// </summary>
        /// <returns></returns>
        public static State NewState()
        {
            return new State(0, 0, null);
        }

        public State(int currentPoints, int totalPoints, object[] purchasedUpgrades)
        {
            CurrentPoints = currentPoints;
            TotalPoints = totalPoints;
            PurchasedUpgrades = purchasedUpgrades;
        }

        /// <summary>
        /// Add points to the state.
        /// </summary>
        /// <param name="AddedPoints">Points to add</param>
        public void AddPoints(int AddedPoints)
        {
            CurrentPoints += AddedPoints;
        }

        public int Points { get { return CurrentPoints; } }
        public int HistoricPoints { get { return TotalPoints; } }
        
    }
}
