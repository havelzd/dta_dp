using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OperativeMarker : MonoBehaviour
{

    public OperativeData Data;
    public SettingsManager settings;
    LocationService locationService;

    [SerializeField] float ScaleLowerBound;
    const float ScaleUpperBound = 1;

    [SerializeField] private float MinDistMeters;
    [SerializeField] private float MaxDistMeters;
    private bool ShowDist;

    [SerializeField] private Gradient statusColor;
    [SerializeField] bool shouldScale = true;

    public TextMeshProUGUI id;
    public Image status;
    [SerializeField] TextMeshProUGUI distanceText;


    private void Awake()
    {
        locationService = LocationService.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadSettings();
        settings.RegisterSettingsChangedCallback(LoadSettings);
    }

    // Update is called once per frame
    void Update()
    {
        if(Data != null)
        {
            var operativeId = int.Parse(Data.id);
            id.text = operativeId.ToString();

            double distM = CoordinatesTransformer.CalculateDistanceM(locationService.Latitude, locationService.Longitude, Data.location.lat, Data.location.lon);

            float scale;

            if(distM >= MaxDistMeters)
            {
                scale = ScaleLowerBound;
            } else if (distM <= MinDistMeters)
            {
                scale = ScaleUpperBound;
            } else
            {
                scale = Mathf.Clamp((float)(ScaleUpperBound - distM / (MaxDistMeters - MinDistMeters)), ScaleLowerBound, ScaleUpperBound);
            }

            if (shouldScale)
            {
                status.transform.localScale = new Vector3(scale, scale, scale);
            }

            if(ShowDist && distanceText != null)
            {
                distanceText.gameObject.SetActive(true);
                if (distM < 1000)
                {
                    
                    distanceText.text = distM.ToString("F2") + " m";
                }
                else
                {
                    distanceText.text = (distM/1000).ToString("F2") + " Km";
                }
            } else if(!ShowDist && distanceText != null)
            {
                distanceText.gameObject.SetActive(false);
            }

            status.color = statusColor.Evaluate(Data.mls / 100);
        }
    }

    void LoadSettings()
    {
        Debug.Log("SettingsChanged");
        ShowDist = PlayerPrefs.GetInt("showDistance", 0) == 1;
        MinDistMeters = PlayerPrefs.GetFloat("minDist", 5f);
        MaxDistMeters = PlayerPrefs.GetFloat("maxDist", 1000f);
        Debug.Log("ShowDistance " + ShowDist);
        Debug.Log("Min " + MinDistMeters);
        Debug.Log("Max " + MaxDistMeters);
    }
}
