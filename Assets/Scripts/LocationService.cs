using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class LocationService : MonoBehaviour
{

    private static LocationService _instance;

    public static LocationService Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<LocationService>();
            }
            return _instance;
        }
    }

    [SerializeField] public GameObject locationTextField;

    [SerializeField] public float Latitude;
    [SerializeField] public float Longitude;
    [SerializeField] public float Altitude;

    private bool PermissionBeingRequested = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckAndRequestPermissions());
    }

    void Update()
    {
        TextMeshProUGUI textMeshProText = locationTextField.GetComponentInChildren<TextMeshProUGUI>();

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Latitude = 50.082441f;
            Longitude = 14.495798f;
            Altitude = 220;
            return;
        }
        

        if(Input.location.status == LocationServiceStatus.Initializing)
        {
            textMeshProText.text = "Initializing location...";
        } else if (Input.location.status == LocationServiceStatus.Running)
        {
            Latitude = Input.location.lastData.latitude;
            Longitude = Input.location.lastData.longitude;
            Altitude = Input.location.lastData.altitude;
            textMeshProText.text = GetLocationText(Latitude, Longitude, Altitude);
        } else if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation)) {
            // Permission not granted
            textMeshProText.text = "Location permission not granted";
            if(!PermissionBeingRequested)
            {
                StartCoroutine(CheckAndRequestPermissions());
            }
            
        }
    }

    private string GetLocationText(float lat, float lon, float alt)
    {
        return string.Format("{0} {1} {2}", lat, lon, alt);
    }

    private IEnumerator CheckAndRequestPermissions()
    {

        PermissionBeingRequested = true;

#if UNITY_EDITOR
        // do not check permissions
#elif UNITY_ANDROID

        if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        { 
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation); 
        }
#endif
        Input.location.Start(2,2);
        Input.compass.enabled = true;

        PermissionBeingRequested = false;

        yield break;
    }

    private void OnApplicationQuit()
    {
        Input.location.Stop();
    }

    public float GetCompassHeading()
    {
        return Input.compass.trueHeading;
    }
}
