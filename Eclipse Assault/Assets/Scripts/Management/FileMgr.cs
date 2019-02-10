using Data;
using System.Collections;
using System.Collections.Generic;
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
            return Instance == null ? new FileMgr() : Instance;
        }

        private FileMgr()
        {
            Instance = this;
        }

        /// <summary>
        /// Loads the weapon data from a file.
        /// </summary>
        /// <param name="path">the file of the given data</param>
        /// <returns></returns>
        public WeaponData LoadWeaponData(string path)
        {

            TextAsset json = Resources.Load(path, typeof(TextAsset))as TextAsset;

            WeaponData data = JsonUtility.FromJson<WeaponData>(json.text);
            return data;
        }

        /// <summary>
        /// Saves a given state into persistent memory.
        /// </summary>
        /// <param name="State">The state variable</param>
        public void SaveState(State State)
        {

            BinaryFormatter BinFormatter = new BinaryFormatter();
            FileStream Stream = File.Create(Application.persistentDataPath + "/state");

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
    }
}