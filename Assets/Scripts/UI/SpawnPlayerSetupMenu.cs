using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace UI
{
    public class SpawnPlayerSetupMenu : MonoBehaviour
    {
        public GameObject playerSetupMenuPrefab;
        public PlayerInput input;

        private void Awake()
        {
            var rootMenu = GameObject.Find("PlayerSelectPanel");
            if (rootMenu != null)
            {
                var menu = Instantiate(playerSetupMenuPrefab, rootMenu.transform);
                input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
                menu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(input.playerIndex);
            }
        }
    }
}