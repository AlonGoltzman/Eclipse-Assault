using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mgmt
{
    public class LevelMenu : Level
    {
        /// <summary>
        /// Is swipe control enabled.
        /// </summary>
        public bool TiltControl;

        public void Start()
        {
            PlayerState = StateInRuntime.LoadState();
            GameConstants.PREFAB_GAME_MANAGER.PlayerState = PlayerState;
        }

        public void Awake()
        {
#if !UNITY_ANDROID
                Destroy(GameObject.Find(GameConstants.UI_MENU_NAME_TILT_CONTROL));
#endif
            Toggle toggle = GameObject.Find(GameConstants.UI_MENU_NAME_TILT_CONTROL).GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(ToggleTiltControl);
            TiltControl = toggle.isOn;

            Button PlayButton = GameObject.Find(GameConstants.UI_MENU_NAME_PLAY_BUTTON).GetComponent<Button>();
            PlayButton.onClick.AddListener(StartGame);

            Button StoreButton = GameObject.Find(GameConstants.UI_MENU_NAME_STORE_BUTTON).GetComponent<Button>();
            StoreButton.onClick.AddListener(OpenShop);

#if UNITY_EDITOR
            Button ResetSave = GameObject.Find(GameConstants.UI_MENU_NAME_RESET_SAVE).GetComponent<Button>();
            ResetSave.onClick.AddListener(() => { PlayerState = PlayerState.Reset(); GameConstants.PREFAB_GAME_MANAGER.PlayerState = PlayerState;});
#else 
                Destroy(GameObject.Find(GameConstants.UI_MENU_NAME_RESET_SAVE));
#endif
        }

        /// <summary>
        /// Starts the game by changing the scene to the Arena sceene.
        /// </summary>
        public void StartGame()
        {
            GameConstants.PREFAB_GAME_MANAGER.TiltControl = TiltControl;
            GameConstants.PREFAB_GAME_MANAGER.PlayerState = PlayerState;
            SceneManager.LoadScene(GameConstants.LEVEL_NAME_ARENA, LoadSceneMode.Single);
        }

        /// <summary>
        /// Changes the scene to the store level.
        /// </summary>
        public void OpenShop()
        {
            SceneManager.LoadScene(GameConstants.LEVEL_NAME_STORE, LoadSceneMode.Single);
        }

        /// <summary>
        /// Inverses the current setting of the tilt control.
        /// </summary>
        private void ToggleTiltControl(bool Value)
        {
            TiltControl = Value;
        }

    }
}
