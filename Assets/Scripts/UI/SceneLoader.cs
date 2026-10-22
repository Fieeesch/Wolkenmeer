using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start() 
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("StoryScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
