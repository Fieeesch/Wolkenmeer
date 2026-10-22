using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject controllerLayout;
    [SerializeField] private GameObject craftingTutorial;
    [SerializeField] private Canvas hudCanvas;
    
    private Animator[] _animators;

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    public void PauseGame()
    {
        hudCanvas.enabled = false;
        pauseMenu.SetActive(true);
        // Ingame-Zeit anhalten
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        hudCanvas.enabled = true;
        // Ingame-Zeit fortsetzen
        Time.timeScale = 1f;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene("MenueScene");
    }

    public void ShowControllerLayout()
    {
        craftingTutorial.SetActive(false);
        controllerLayout.SetActive(true);
    }

    public void ShowCraftingTutorial()
    {
        controllerLayout.SetActive(false);
        craftingTutorial.SetActive(true);
    }
}
