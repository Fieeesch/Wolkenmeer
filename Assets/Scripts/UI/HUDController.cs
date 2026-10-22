using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class HUDController : MonoBehaviour
    {
        private static HUDController _instance;
        
        public static HUDController Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogError("GameManager is null!");
                return _instance;
            }
        }
        
        [SerializeField] private Canvas gameEndCanvas;
        [SerializeField] private TextMeshProUGUI canvasText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Button firstSelected;
        [SerializeField] private RectTransform _healthBar;
        [SerializeField] private RectTransform _balloon;
        [SerializeField] private RectTransform _cloud;
        private float _oneStep;
        
        private void Awake()
        {
            if (_instance)
                Destroy(gameObject);
            else
                _instance = this;
        }

        private void Start()
        {
            _oneStep = Vector3.Distance(_cloud.position, _balloon.position) / GameManager.Instance.ShipHealth;
            GameManager.Instance.SetHUD(_instance);
            gameEndCanvas.enabled = false;
        }

        /// <summary>
        /// Reduces the Healthbar in the HUD
        /// </summary>
        /// <param name="damage">Damage that was dealt</param>
        public void ReduceHealthBar(float damage)
        {
            _healthBar.position = new Vector3(_healthBar.position.x, _healthBar.position.y - (_oneStep * damage), _healthBar.position.z);
        }

        /// <summary>
        /// Shows the Canvas at the end of the Level
        /// </summary>
        /// <param name="won">If the Game has been won or lost</param>
        public void ShowGameEndCanvas(bool won)
        {
            canvasText.text = won ? "YOu Won!" : "Game Over!";
            scoreText.text = won ? $"Score: {GameManager.Instance.ShipHealth}" : "";
            Time.timeScale = 0f;
            _healthBar.gameObject.SetActive(false);
            _cloud.gameObject.SetActive(false);
            gameEndCanvas.enabled = true;
            
            firstSelected.interactable = false;
            // nach 3s wird der Button freigegeben
            StartCoroutine(ActivateButtonDelayed(3f));
        }

        private IEnumerator ActivateButtonDelayed(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            firstSelected.interactable = true;
            firstSelected.Select();
        }
    }
}