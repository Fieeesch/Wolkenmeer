using System;
using Spawning;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultyMenuManager : MonoBehaviour
{
    public void SelectDifficulty(WaveConfig difficultyConfig)
    {
        GameManager.Instance.DifficultyConfig = difficultyConfig;
        SceneManager.LoadScene("MainScene");
    }

    public void LoadPreviousScene()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene("PlayerSetup");
    }
}
