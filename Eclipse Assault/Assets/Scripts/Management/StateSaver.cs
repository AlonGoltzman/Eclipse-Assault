﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Mgmt
{
    public class StateSaver : MonoBehaviour
    {

        public static void SaveState(State State)
        {

            BinaryFormatter BinFormatter = new BinaryFormatter();
            FileStream Stream = File.Create(Application.persistentDataPath + "/state");

            BinFormatter.Serialize(Stream, State);
            Stream.Close();

        }

        public static void LoadState(out State state)
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
                    state = DeleteState();
                }
                catch (IOException)
                {
                    state = DeleteState();
                }
            }
            else
            {
                state = State.NewState();
            }
        }

        private static State DeleteState()
        {
            //TODO: Provide proper fix.
            Debug.LogError("Failed to deserialize file, deleting it.");
            File.Delete(Application.persistentDataPath + "/state");
            return State.NewState();
        }
    }
}