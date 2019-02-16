using ControllerSO;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Mgmt
{
    public class FileMgr
    {

        private static FileMgr Instance;

        public static FileMgr GetInstance()
        {
            return Instance ?? new FileMgr();
        }

        private FileMgr()
        {
            Instance = this;
        }

        /// <summary>
        /// Saves a given state into persistent memory.
        /// </summary>
        /// <param name="State">The state variable</param>
        public void SaveState(State State)
        {
            BinaryFormatter BinFormatter = new BinaryFormatter();
            FileStream Stream = null;
            if (File.Exists(Application.persistentDataPath + "/state"))
            {
                Stream = File.OpenWrite(Application.persistentDataPath + "/state");
            }
            else
            {
                Stream = File.Create(Application.persistentDataPath + "/state");
            }

            BinFormatter.Serialize(Stream, State);
            Stream.Close();

        }

        /// <summary>
        /// Loads a save \ state into a given variable.
        /// </summary>
        /// <param name="state">The state variable</param>
        public void LoadState(out State state)
        {
            if (File.Exists(Application.persistentDataPath + "/state"))
            {
                try
                {
                    BinaryFormatter BinFormatter = new BinaryFormatter();
                    FileStream StateFile = File.Open(Application.persistentDataPath + "/state", FileMode.Open);
                    State LoadedState = (State)BinFormatter.Deserialize(StateFile);
                    StateFile.Close();

                    state = LoadedState;
                }
                catch (SerializationException)
                {
                    DeleteState();
                    state = State.NewState();
                }
                catch (IOException)
                {
                    DeleteState();
                    state = State.NewState();
                }
            }
            else
            {
                state = State.NewState();
            }
        }

        /// <summary>
        /// Deletes the current state in persistent memory.
        /// </summary>
        public void DeleteState()
        {
            //TODO: Provide proper fix.
            Debug.LogError("Failed to deserialize file, deleting it.");
            File.Delete(Application.persistentDataPath + "/state");
        }

        /// <summary>
        /// Loads a weapon stat according to it's GUID.
        /// If the GUID is not found then the default weapon is returned.
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public WeaponStats LoadWeaponStats(string GUID)
        {
            WeaponStats[] WeaponStats = Resources.LoadAll<WeaponStats>(GameConstants.STATS_WEAPONS_PATH);
            foreach (WeaponStats Stats in WeaponStats)
            {
                if (Stats.WeaponGUID.Equals(GUID))
                {
                    return Stats;
                }
            }

            foreach (WeaponStats Stats in WeaponStats)
            {
                if (Stats.DefaultWeapon)
                    return Stats;
            }
            return null;
        }
    }
}