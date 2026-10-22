using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerConfigurationManager : MonoBehaviour
    {
        public PlayerInputManager inputManager;
        public GameManager gameManager;

        private List<PlayerConfiguration> _playerConfigs;

        [SerializeField] private int maxPlayers = 4;

        [SerializeField] private GameObject bonkoPrefab;
        [SerializeField] private GameObject gisbertaPrefab;
        [SerializeField] private GameObject fibiPrefab;
        [SerializeField] private GameObject olafPrefab;

        private string[] _chosenCharacters = new string[4];

        [SerializeField] private InputActionReference forceGameStartAction;

        public static PlayerConfigurationManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("SINGLETON - Trying to create another instance of a singleton!");
            }
            else
            {
                Instance = this;
                _playerConfigs = new List<PlayerConfiguration>();
                inputManager.onPlayerJoined += HandlePlayerJoin;
            }
        }

        public List<PlayerConfiguration> GetPlayerConfigs()
        {
            return _playerConfigs;
        }

        /// <summary>
        /// Sets the Skin of the Player Character
        /// </summary>
        /// <param name="index">Index of the Player</param>
        /// <param name="characterName">Name of the Character</param>
        /// <returns>True if worked, false if didn't work</returns>
        public bool SetPlayerSkin(int index, string characterName)
        {
            foreach (var character in _chosenCharacters)
            {
                if (characterName.Equals(character))
                {
                    return false;
                }
            }

            switch (characterName)
            {
                case "Bonko":
                    _playerConfigs[index].CharacterPrefab = bonkoPrefab;
                    _chosenCharacters[index] = characterName;
                    return true;
                case "Gisberta":
                    _playerConfigs[index].CharacterPrefab = gisbertaPrefab;
                    _chosenCharacters[index] = characterName;
                    return true;
                case "Fibi":
                    _playerConfigs[index].CharacterPrefab = fibiPrefab;
                    _chosenCharacters[index] = characterName;
                    return true;
                case "Olaf":
                    _playerConfigs[index].CharacterPrefab = olafPrefab;
                    _chosenCharacters[index] = characterName;
                    return true;
                default:
                    Debug.Log("not a valid character");
                    return false;
            }
        }

        /// <summary>
        /// Readies the Player and Starts the Level if all Players are ready
        /// </summary>
        /// <param name="index">Index of the Player</param>
        public void ReadyPlayer(int index)
        {
            _playerConfigs[index].IsReady = true;
            if (_playerConfigs.Count > 0 && _playerConfigs.Count <= maxPlayers && _playerConfigs.All(p => p.IsReady))
            {
                SceneManager.LoadScene("DifficultyScene");
            }
        }

        /// <summary>
        /// Unreadies the Player
        /// </summary>
        /// <param name="index">Index of the Player</param>
        public void UnreadyPlayer(int index)
        {
            _playerConfigs[index].IsReady = false;
            _chosenCharacters[index] = null;
        }

        public void HandlePlayerJoin(PlayerInput pi)
        {
            if (_playerConfigs.All(p => p.PlayerIndex != pi.playerIndex))
            {
                if (gameManager.transform.TryGetComponent<AudioSource>(out var source))
                {
                    AudioManager.Instance.PlayUI_CLICK(source);
                }

                // PlayerInput wird Teil des GameManagers
                pi.transform.SetParent(gameManager.transform);
                _playerConfigs.Add(new PlayerConfiguration(pi));
                gameManager.PlayerInputs.Add(pi.playerIndex, pi.transform.GetComponentInChildren<PlayerGameInput>());
            }
        }

        /// <summary>
        /// Forces the Level to start with all players that are ready (if existing)
        /// </summary>
        public void ForceGameStart()
        {
            var newPlayerConfigs = GetPlayerConfigs().Where(p => p.IsReady).ToList();

            if (newPlayerConfigs.Count > 0 && newPlayerConfigs.Count <= maxPlayers)
            {
                _playerConfigs = newPlayerConfigs;
                SceneManager.LoadScene("DifficultyScene");
            }
        }

        /// <summary>
        /// Returns to Menu Entry Screen
        /// </summary>
        public void CancelPlayerSelection()
        {
            _playerConfigs.Clear();
            Destroy(GameManager.Instance.gameObject);

            SceneManager.LoadScene("MenueScene");
        }
    }

    public class PlayerConfiguration
    {
        public PlayerConfiguration(PlayerInput pi)
        {
            PlayerIndex = pi.playerIndex;
            Input = pi;
        }

        public PlayerInput Input { get; set; }
        public int ControllerId { get; set; }
        public int PlayerIndex { get; set; }
        public bool IsReady { get; set; }
        public GameObject CharacterPrefab { get; set; }
    }
}