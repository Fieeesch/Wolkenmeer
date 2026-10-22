using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public interface IInputReceiver
    {
        public void OnMove(InputAction.CallbackContext context);
        public void OnInteract(InputAction.CallbackContext context);
        public void OnAbort(InputAction.CallbackContext context);
        public void OnShoot(InputAction.CallbackContext context);
        public void OnEdit(InputAction.CallbackContext context);
        public void OnPause(InputAction.CallbackContext context)
        {
            var pauseMenu = GameObject.Find("PauseManager");
            if (pauseMenu == null)
            {
                Debug.Log("PauseManager Game Object is null");
                return;
            }

            var pauseController = pauseMenu.GetComponent<PauseController>();
            if (pauseController == null)
            {
                Debug.Log("PauseController script is null");
                return;
            }
        
            pauseController.PauseGame();
        }
    }
}