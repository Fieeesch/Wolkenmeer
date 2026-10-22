using System;
using System.Collections.Generic;
using Player;
using Spawning;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

public enum GameStates
{
    Menu = 0,
    Playing = 1,
    Lost = 2,
    Won = 3,
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public GameStates GameState { get; private set; }

    public Dictionary<int, PlayerGameInput> PlayerInputs = new();

    public WaveConfig DifficultyConfig;
    
    public float ShipHealth { get; private set; } = 1000f;

    private HUDController _hud;


    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("GameManager is null!");

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance)
            Destroy(gameObject);
        else
        {
            _instance = this;

            GameState = GameStates.Menu;
        }

        DontDestroyOnLoad(this);
    }

    public void SetHUD(HUDController hud)
    {
        _hud = hud;
    }

    public void StartLevel()
    {
        GameState = GameStates.Playing;
    }

    /// <summary>
    /// Make Ship take Damage
    /// </summary>
    /// <param name="damage">Damage to be dealt</param>
    public void ReduceLife(float damage)
    {
        if (GameState != GameStates.Playing) return;
        
        if (_hud == null)
        {
            _hud = HUDController.Instance;
        }
        ShipHealth -= damage;
        _hud.ReduceHealthBar(damage);
        if (ShipHealth <= 0)
        {
            GameState = GameStates.Lost;
            _hud.ShowGameEndCanvas(false);
        }
    }

    /// <summary>
    /// Set Gamestate to Won
    /// </summary>
    public void WinGame()
    {
        if (GameState == GameStates.Playing)
        {
            GameState = GameStates.Won;
            _hud.ShowGameEndCanvas(true);
        }
    }

    public void BindPlayerToInput(IInputReceiver player, int playerIndex)
    {
        if(player == null) Debug.LogWarning("player is null!");
        PlayerInputs[playerIndex].Player = player;
    }
}   