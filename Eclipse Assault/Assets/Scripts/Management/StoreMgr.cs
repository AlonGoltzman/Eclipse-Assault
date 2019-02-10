using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Mgmt
{
    public class StoreMgr : MonoBehaviour
    {

        void Start()
        {
            GameObject ItemTemplate = Resources.Load(GameConstants.PREFAB_PATH_STORE_ITEM_TEMPLATE, typeof(GameObject)) as GameObject;
            GameObject ItemDisplay = GameObject.Find(GameConstants.UI_STORE_NAME_ITEM_DISPLAY);

            IDictionary<string, Type> JsonToType = new Dictionary<string, Type>();
            TextAsset[] JSONs = Resources.LoadAll<TextAsset>("Data/Store/");
            foreach (TextAsset json in JSONs)
            {
                string ResourcePath = AssetDatabase.GetAssetPath(json);
                if (ResourcePath.EndsWith("json"))
                {
                    if (ResourcePath.Contains(GameConstants.DATA_WEAPONS))
                    {
                        JsonToType.Add(json.text, typeof(WeaponData));
                    }
                }
            }

            foreach (KeyValuePair<string, Type> ItemDetails in JsonToType)
            {
                if (ItemDetails.Value.IsEquivalentTo(typeof(WeaponData)))
                {
                    WeaponData ItemData = JsonUtility.FromJson<WeaponData>(ItemDetails.Key);
                    GameObject CreatedItem = Instantiate(ItemTemplate, ItemDisplay.transform);

                    for (int i = 0; i < CreatedItem.transform.childCount; i++)
                    {
                        GameObject Child = CreatedItem.transform.GetChild(i).gameObject;
                        if (Child.name.Equals(GameConstants.UI_STORE_NAME_ITEM_TEXT_CONTAINER))
                        {
                            for (int j = 0; j < Child.transform.childCount; j++)
                            {
                                GameObject GrandChild = Child.transform.GetChild(j).gameObject;
                                if (GrandChild.name.Equals(GameConstants.UI_STORE_NAME_ITEM_NAME))
                                {
                                    GrandChild.GetComponent<Text>().text = ItemData.Name;
                                }
                                else if (GrandChild.name.Equals(GameConstants.UI_STORE_NAME_ITEM_DESCRIPTION))
                                {
                                    GrandChild.GetComponent<Text>().text = string.Format(GameConstants.TEXT_ITEM_DESCRIPTION, ItemData.Description, ItemData.AdditionalDetails());
                                }
                                else if (GrandChild.name.Equals(GameConstants.UI_STORE_NAME_ITEM_PRICE))
                                {
                                    GrandChild.GetComponent<Text>().text = string.Format("Price: {0}", ItemData.Price);
                                }
                            }
                        }
                        else if (Child.name.Equals(GameConstants.UI_STORE_NAME_PURCHASE_BUTTON))
                        {
                            GameMgr GameManager = GameObject.Find(GameConstants.NAME_GAME_MANAGER).GetComponent<GameMgr>();
                            if (GameManager.CanPurchase(ItemData.Price))
                            {
                                //TODO: Add purchase functionality.
                            }
                            else
                            {
                                Child.GetComponentInChildren<Text>().text = "Too Expensive";
                                Child.GetComponent<Button>().interactable = false;
                            }
                        }
                    }

                }
            }

        }


    }
}