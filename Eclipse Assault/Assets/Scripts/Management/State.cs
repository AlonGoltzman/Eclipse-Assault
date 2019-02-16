using ControllerSO;
using System;
using System.Collections.Generic;

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
        /// The current used weapon.
        /// </summary>
        public string SelectedWeapon;

        //private ArmorStats SelectArmor;

        /// <summary>
        /// All the purchased items' GUID.
        /// </summary>
        private List<string> PurchasedItemsGUID;

        /// <summary>
        /// Creates a new default state that is used to define that a new "save" is created.
        /// </summary>
        /// <returns></returns>
        public static State NewState()
        {
            return new State(0, 0);
        }

        public State(int currentPoints, int totalPoints)
        {
            CurrentPoints = currentPoints;
            TotalPoints = totalPoints;
            SelectedWeapon = GameConstants.STATS_WEAPON_DEFAULT_GUID;
            PurchasedItemsGUID = new List<string>();
        }

        /// <summary>
        /// Add points to the state.
        /// </summary>
        /// <param name="AddedPoints">Points to add</param>
        public void AddPoints(int AddedPoints)
        {
            CurrentPoints += AddedPoints;
        }

        /// <summary>
        /// Adds an item as purchased.
        /// </summary>
        /// <param name="GUID"></param>
        public void AddPurchasedItem(int Price, string GUID)
        {
            PurchasedItemsGUID.Add(GUID);
            CurrentPoints -= Price;
        }

        public int Points { get { return CurrentPoints; } }
        public int HistoricPoints { get { return TotalPoints; } }
        public string SelectedWeaponStats { get { return SelectedWeapon; } set { SelectedWeapon = value; } }
        
        public bool HasItem(string GUID)
        {
            return PurchasedItemsGUID.Contains(GUID);
        }
    }

    /// <summary>
    /// An instance of the state to be used in Runtime.
    /// </summary>
    public class StateInRuntime
    {
        private State OriginatingState;

        private WeaponStats SelectedWeaponStats;

        private static StateInRuntime Instance;

        private static StateInRuntime ConvertState(State originalState)
        {
            StateInRuntime State = new StateInRuntime();
            State.SetState(originalState);
            State.SetWeaponStats(FileMgr.GetInstance().LoadWeaponStats(originalState.SelectedWeaponStats));
            return State;
        }

        public static StateInRuntime GetInstance()
        {
            return Instance ?? LoadState();
        }

        public static StateInRuntime LoadState()
        {
            FileMgr.GetInstance().LoadState(out State LoadedState);
            Instance = ConvertState(LoadedState);
            return Instance;
        }

        private StateInRuntime()
        {
        }

        /// <summary>
        /// Sets the originating state, can only be set once.
        /// </summary>
        /// <param name="OriginalState"></param>
        private void SetState(State OriginalState)
        {
            if (OriginatingState != null) return;
            OriginatingState = OriginalState;
        }

        /// <summary>
        /// Sets the state's used weapon stats.
        /// </summary>
        /// <param name="Stats"></param>
        private void SetWeaponStats(WeaponStats Stats)
        {
            SelectedWeaponStats = Stats;
        }

        /// <summary>
        /// Resets the State, both in Run time and both in persistant storage.
        /// </summary>
        public StateInRuntime Reset()
        {
            FileMgr.GetInstance().DeleteState();
            OriginatingState = null;
            State NewState = State.NewState();
            FileMgr.GetInstance().SaveState(NewState);
            Instance = ConvertState(NewState);
            return Instance;
        }

        /// <summary>
        /// Saves the current state.
        /// </summary>
        public void Save()
        {
            OriginatingState.SelectedWeaponStats = SelectedWeapon.WeaponGUID;
            FileMgr.GetInstance().SaveState(OriginatingState);
        }

        public WeaponStats SelectedWeapon { get { return SelectedWeaponStats; } }
        public int Points { get { return OriginatingState.Points; } }

        /// <summary>
        /// Adds points to the current balance.
        /// </summary>
        /// <param name="pointsAdded"></param>
        public void AddPoints(int pointsAdded)
        {
            OriginatingState.AddPoints(pointsAdded);
        }

        /// <summary>
        /// Removes points from the current balance and adds the GUID to the purchased items.
        /// </summary>
        /// <param name="GUID"></param>
        public void Bought(int Price, string GUID)
        {
            OriginatingState.AddPurchasedItem(Price, GUID);
        }

        /// <summary>
        /// Marks a specific GUID as the selected weapon.
        /// </summary>
        /// <param name="GUID"></param>
        public void SelectWeapon(string GUID)
        {
            OriginatingState.SelectedWeaponStats = GUID;
            SelectedWeaponStats = FileMgr.GetInstance().LoadWeaponStats(GUID);
        }

        /// <summary>
        /// Checks if the player has an item or not.
        /// </summary>
        /// <param name="gUID"></param>
        /// <returns></returns>
        public bool HasItem(string GUID)
        {
            return OriginatingState.HasItem(GUID);
        }
    }
}
