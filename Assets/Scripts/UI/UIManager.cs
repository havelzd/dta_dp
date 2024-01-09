using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIManager: MonoBehaviour
    {

        public GameObject warningPrefab;
        public GameObject warningPanel;

        public GameObject menu;
        public GameObject uiToggle;

        public GameObject missionList;
        public GameObject activeMissionScreen;
        public bool missionRunning = false;

        [SerializeField] GameObject settingsScreen;
        [SerializeField] GameObject screenButtons;
        [SerializeField] GameObject mapBox;

        private bool mapOpened = false;

        public void Start()
        {
            settingsScreen.SetActive(false);
            menu.SetActive(false);
            screenButtons.SetActive(true);
            mapBox.SetActive(false);
            SetMissionScreensActive();
        }

        public void SetMissionScreensActive()
        {
            activeMissionScreen.SetActive(missionRunning);
            missionList.SetActive(!missionRunning);
        }

        public void DisplayWarning(String warning, String message)
        {
            GameObject warningObj = Instantiate(warningPrefab, warningPanel.transform);
            TextMeshProUGUI textMeshProText = warningObj.GetComponentInChildren<TextMeshProUGUI>();
            textMeshProText.text = warning + " " + message;
        }

        public void OnMission(bool started)
        {
            missionRunning = started;
            SetMissionScreensActive();
        }

        public void OnSettings()
        {
            screenButtons.SetActive(false);
            settingsScreen.SetActive(true);
        }

        public void OnSettingsClose()
        {
            screenButtons.SetActive(true);
            settingsScreen.SetActive(false);
        }

        public void OnMenu()
        {
            screenButtons.SetActive(false);
            menu.SetActive(true);
        }

        public void OnMenuClose()
        {
            screenButtons.SetActive(true);
            menu.SetActive(false);
        }

        public void OnMap()
        {
            screenButtons.SetActive(true);
            mapOpened = !mapOpened;
            mapBox.SetActive(mapOpened);
        }

        public void OnMapClose()
        {
            screenButtons.SetActive(true);
            mapBox.SetActive(false);
        }

    }
}