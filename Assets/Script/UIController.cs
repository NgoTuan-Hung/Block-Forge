using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    [Serializable]
    public class UIController
    {
        public GameObject LoseUI;
        public Button LoseButton;

        [SerializeField]
        private TextMeshProUGUI _scoreText,
            _maxScoreText;

        public void Init(LevelController levelController)
        {
            levelController.OnGameLose += ShowLoseUI;
            LoseButton.onClick.AddListener(HideLoseUI);
        }

        public void ShowLoseUI()
        {
            LoseUI.SetActive(true);
        }

        public void HideLoseUI()
        {
            LoseUI.SetActive(false);
        }

        public void Refresh()
        {
            LoseButton.onClick.RemoveAllListeners();
        }

        public void SetScore(int oldScore, int newScore, float duration)
        {
            //
        }

        private IEnumerator SetScoreCoroutine(int oldScore, int newScore, float duration)
        {
            yield break;
        }
    }
}
