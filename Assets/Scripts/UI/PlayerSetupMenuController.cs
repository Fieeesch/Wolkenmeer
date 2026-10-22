using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class PlayerSetupMenuController : MonoBehaviour
    {
        private int _playerIndex;

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private GameObject readyPanel;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private TextMeshProUGUI readyText;
        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI readyButtonText;
        [SerializeField] private Button manualStartButton;
        [SerializeField] private Button firstButton;

        private float _ignoreInputTime = 0.5f;
        private bool _inputEnabled = true;

        public void SetPlayerIndex(int pi)
        {
            _playerIndex = pi;
            titleText.SetText((pi + 1).ToString());
            _ignoreInputTime = Time.time + _ignoreInputTime;
        }

        void Update()
        {
            if (Time.time > _ignoreInputTime)
            {
                _inputEnabled = true;
            }
        }

        public void SetSkin(string characterName)
        {
            if (!_inputEnabled) return;

            if (!PlayerConfigurationManager.Instance.SetPlayerSkin(_playerIndex, characterName)) return;

            readyText.SetText(characterName.ToUpper());
            readyButtonText.SetText(characterName.ToUpper() + " bereit machen");
            readyButton.Select();
            readyPanel.SetActive(true);
            menuPanel.SetActive(false);
        }

        public void ReadyPlayer()
        {
            if (!_inputEnabled) return;
            if (GameManager.Instance.TryGetComponent<AudioSource>(out var src))
            {
                AudioManager.Instance.PlayUI_CLICK(src);
            }

            PlayerConfigurationManager.Instance.ReadyPlayer(_playerIndex);
            readyButton.gameObject.SetActive(false);
            manualStartButton.Select();
        }

        // Ist buggy und wird deshalb nicht mehr benutzt
        /*public void UnreadyPlayer()
        {
            if (!_inputEnabled) return;
            if (GameManager.Instance.TryGetComponent<AudioSource>(out var src))
            {
                AudioManager.Instance.PlayUI_BACK(src);
            }

            PlayerConfigurationManager.Instance.UnreadyPlayer(_playerIndex);
            firstButton.Select();
            readyPanel.SetActive(false);
            menuPanel.SetActive(true);
        }*/

        public void ManualStart()
        {
            if (!_inputEnabled) return;
            if (GameManager.Instance.TryGetComponent<AudioSource>(out var src))
            {
                AudioManager.Instance.PlayUI_CLICK(src);
            }

            PlayerConfigurationManager.Instance.ForceGameStart();
        }

        public void BackToMenu()
        {
            if (!_inputEnabled) return;
            if (GameManager.Instance.TryGetComponent<AudioSource>(out var src))
            {
                AudioManager.Instance.PlayUI_BACK(src);
            }

            PlayerConfigurationManager.Instance.CancelPlayerSelection();
        }

        public void PlayMoveSound()
        {
            if (GameManager.Instance.TryGetComponent<AudioSource>(out var source))
            {
                AudioManager.Instance.PlayUI_MOVE(source);
            }
        }

        public void PlayBackSound()
        {
            if (GameManager.Instance.TryGetComponent<AudioSource>(out var source))
            {
                AudioManager.Instance.PlayUI_BACK(source);
            }
        }
    }
}