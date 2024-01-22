using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    private static SettingsManager _instance;

    public static SettingsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SettingsManager>();
            }
            return _instance;
        }
    }

    [SerializeField] Toggle showDistance;
    [SerializeField] Toggle showSquad;
    [SerializeField] Slider minDist;
    [SerializeField] Slider maxDist;
    [SerializeField] Slider clampAt;
    [SerializeField] Slider displayAt;

    [SerializeField] TextMeshProUGUI currentMinDist;
    [SerializeField] TextMeshProUGUI currentMaxDist;
    [SerializeField] TextMeshProUGUI currentClamp;
    [SerializeField] TextMeshProUGUI currentDisplay;

    public delegate void SettingsChangedCallback();
    private SettingsChangedCallback onSettingsChanged;

    // Start is called before the first frame update
    void Start()
    {
        LoadSettings();
    }

    void LoadSettings()
    {
        showDistance.isOn = PlayerPrefs.GetInt("showDistance", 1) == 1;
        showSquad.isOn = PlayerPrefs.GetInt("showSquad", 0) == 1;
        minDist.value = PlayerPrefs.GetFloat("minDist", 5f);
        maxDist.value = PlayerPrefs.GetFloat("maxDist", 1000f);
        clampAt.value = PlayerPrefs.GetFloat("clampAt", 0f);
        displayAt.value = PlayerPrefs.GetFloat("displayAt", 0f);
        showSquad.isOn = PlayerPrefs.GetInt("showSquad", 0) == 1;

        currentMinDist.text = minDist.value.ToString("F2") + " m";
        currentMaxDist.text = minDist.value.ToString("F2") + " m";
        currentClamp.text = clampAt.value.ToString("F2") + " m";
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt("showDistance", showDistance.isOn ? 1 : 0);
        PlayerPrefs.SetInt("showSquad", showSquad.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("minDist", minDist.value);
        PlayerPrefs.SetFloat("maxDist", maxDist.value);
        PlayerPrefs.SetFloat("clampAt", clampAt.value);
        PlayerPrefs.SetFloat("displayAt", displayAt.value);

    }

    public void OnSettingChange()
    {
        SaveSettings();
        if(onSettingsChanged != null)
        {
            onSettingsChanged();
        }
        
    }

    public void RegisterSettingsChangedCallback(SettingsChangedCallback callback)
    {
        onSettingsChanged += callback;
    }

    public void UnregisterSettingsChangedCallback(SettingsChangedCallback callback)
    {
        onSettingsChanged -= callback;
    }

    public void MinDistChange(float value)
    {
        currentMinDist.text = value.ToString("F2") + " m";
        OnSettingChange();
    }

    public void MaxDistChange(float value)
    {
        currentMaxDist.text = value.ToString("F2") + " m";
        OnSettingChange();
    }

    public void ClampAtChange(float value)
    {
        currentClamp.text = value.ToString("F2") + " <=";
        OnSettingChange();
    }

    public void DisplayAtChange(float value)
    {
        currentClamp.text = value.ToString("F2") + " <=";
        OnSettingChange();
    }

    public void ShowDistanceChange(bool value)
    {
        OnSettingChange();
    }

    public void ShowSquadChange(bool value)
    {
        OnSettingChange();
    }

}
