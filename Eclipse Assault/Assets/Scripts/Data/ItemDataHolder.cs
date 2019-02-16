using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Mgmt.LevelShop;

namespace Data
{
    public class ItemDataHolder : MonoBehaviour
    {
        /// <summary>
        /// The GUID of the item that is indicated by this button.
        /// </summary>
        public string GUID;

        /// <summary>
        /// An int value derived from LevelShop's Item Type enum.
        /// </summary>
        public ItemType ItemType;

        /// <summary>
        /// Indicates if an item is selected or not.
        /// </summary>
        public bool Selected;
    }
}