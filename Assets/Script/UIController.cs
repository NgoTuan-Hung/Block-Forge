using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    [Serializable]
    public class UIController
    {
        public GameObject LoseUI;
        public Button LoseButton;

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
    }
}
