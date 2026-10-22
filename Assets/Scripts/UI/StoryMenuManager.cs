using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject entryPanel;
    [SerializeField] private Button firstSelected;
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private GameObject controllerLayoutPanel;

    void Start()
    {
        firstSelected.Select();
    }
    
    public void SelectEntryPanel(Button fromButton)
    {
        storyPanel.SetActive(false);
        craftingPanel.SetActive(false);
        controllerLayoutPanel.SetActive(false);
        entryPanel.SetActive(true); 
        fromButton.Select();
    }
    
    public void SelectStoryPanel()
    {
        entryPanel.SetActive(false);
        storyPanel.SetActive(true);
        storyPanel.transform.Find("BackButton").GetComponent<Button>().Select();
    }

    public void SelectCraftingPanel()
    {
        entryPanel.SetActive(false);
        craftingPanel.SetActive(true);
        craftingPanel.transform.Find("BackButton").GetComponent<Button>().Select();
    }

    public void SelectControllerPanel()
    {
        entryPanel.SetActive(false);
        controllerLayoutPanel.SetActive(true);
        controllerLayoutPanel.transform.Find("BackButton").GetComponent<Button>().Select();
    }
    
    public void LoadNextScene()
    {
        SceneManager.LoadScene("PlayerSetup");
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene("MenueScene");
    }
}
