using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MapboxController : MonoBehaviour
{

    class GeoMarker
    {
        public string id { get; set; }
        public float mls { get; set; }
        public Vector2 pos = new();
        public GameObject marker { get; set; }
    }

    [SerializeField] RawImage mapImage;
    [SerializeField] string authToken;
    [SerializeField] string userName;
    [SerializeField] float transparency;
    [SerializeField] Gradient operativeColor;

    [SerializeField] GameObject userIcon;
    [SerializeField] GameObject operativeIcon;

    LocationService locationService;

    [SerializeField] GameObject missionControllerGO;
    MissionController missionController;

    const string mapboxURL = "https://api.mapbox.com/styles/v1/";
    int width;
    int height;

    Dictionary<int, GeoMarker> lastMarkers = new();
    Dictionary<int, GeoMarker> markers = new();
    float lastLon = -1f, 
        lastLat = -1f;

    Tuple<Vector2, Vector2> bounds = null;

    Vector2 mapSize;

    float currentHeading;
    float rotationSpeed = 0.8f;


    private int zoom = 17;

    private void Awake()
    {
        locationService = LocationService.Instance;
        currentHeading = locationService.GetCompassHeading();
        width = Screen.width > 1080 ? 1080 : Screen.width;
        height = Screen.height > 1280 ? 1280 : Screen.height;

        RectTransform rectTransform = mapImage.rectTransform;
        rectTransform.sizeDelta = new Vector2(width, height);
        mapSize = new Vector2(width, height);

        missionController = missionControllerGO.GetComponent<MissionController>();
    }

    void Start()
    {
        mapImage.enabled = false;
        userIcon.GetComponent<Image>().color = operativeColor.Evaluate(0);
    }


    // Update is called once per frame
    void Update()
    {

        userIcon.SetActive(mapImage.enabled);

        if (missionController == null || missionController.operatives == null)
        {
            return;
        }

        foreach(var operative in missionController.operatives) {
            var key = int.Parse(operative.Key);
            var lastData = operative.Value.lastData;
            if(lastData != null) {
                if (markers.ContainsKey(key))
                {
                    markers[key].mls = lastData.mls;
                    markers[key].pos = new Vector2(lastData.location.lat, lastData.location.lon);
                }else
                {
                    markers[key] = new GeoMarker() { pos = new Vector2(lastData.location.lat, lastData.location.lon), mls = lastData.mls, id = lastData.id };
                    GameObject marker = Instantiate(operativeIcon, mapImage.rectTransform);
                    marker.GetComponentInChildren<TextMeshProUGUI>().text = "" + int.Parse(lastData.id);
                    markers[key].marker = marker;
                }
                UpdateMarker(markers[key].marker, markers[key].pos.x, markers[key].pos.y, markers[key].mls);
            }
        }
        if(missionController.operatives.Count > 0)
        {
            bounds = CalculateBoundsWithMargin(locationService.Latitude, locationService.Longitude);
        }
        

        StartCoroutine(GetMap());

        if(bounds == null)
        {
            userIcon.GetComponent<RectTransform>().position= new Vector2(mapSize.x / 2, mapSize.y / 2);
        }else
        {
            Vector2 pos = CalculatePos(locationService.Latitude, locationService.Longitude);
            userIcon.GetComponent<RectTransform>().position = CalculatePos(locationService.Latitude, locationService.Longitude);
        }

        float targetHeading = locationService.GetCompassHeading();
        currentHeading = Mathf.Lerp(currentHeading, targetHeading, rotationSpeed * Time.deltaTime);
        userIcon.transform.rotation = Quaternion.Euler(0,0, (float)Math.Round((double)targetHeading,2));

    }

    IEnumerator GetMap()
    {
        float lat = locationService.Latitude;
        float lon = locationService.Longitude;

        if (PositionsChanged())
        {
            string url = "";
            if(bounds != null)
            {
                url = string.Format(
                "{0}{1}/{2}/static/[{3},{4},{5},{6}]/{7}x{8}?access_token={9}",
                mapboxURL,
                "mapbox",
                "streets-v12",
                bounds.Item1.y.ToString(System.Globalization.CultureInfo.InvariantCulture),
                bounds.Item1.x.ToString(System.Globalization.CultureInfo.InvariantCulture),
                bounds.Item2.y.ToString(System.Globalization.CultureInfo.InvariantCulture),
                bounds.Item2.x.ToString(System.Globalization.CultureInfo.InvariantCulture),
                width, height,
                authToken);
            } else
            {
                url = string.Format(
                "{0}{1}/{2}/static/{3},{4},{5},{6},{7}/{8}x{9}?access_token={10}",
                mapboxURL,
                "mapbox",
                "streets-v12",
                lon.ToString(System.Globalization.CultureInfo.InvariantCulture),
                lat.ToString(System.Globalization.CultureInfo.InvariantCulture),
                zoom, 0, 30,
                width, height,
                authToken);
            }

            lastLat = lat;
            lastLon = lon;

            Debug.Log("url | " + url);

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            
            if(request.result == UnityWebRequest.Result.Success)
            {
                mapImage.enabled = true;
                mapImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Color color = mapImage.color;
                color.a = transparency;
                mapImage.color = color;
            } else
            {
                ErrorDialog.Instance.ShowMessage(request.error);
            }

        } else
        {
            yield break;
        }
    }

    bool PositionsChanged()
    {
        bool changed = false;

        foreach(var marker in markers)
        {
            if (!lastMarkers.ContainsKey(marker.Key) || !lastMarkers[marker.Key].pos.Equals(marker.Value.pos))
            {
                lastMarkers[marker.Key] = marker.Value;
                changed = true;
            }
        }

        return changed || !Mathf.Approximately(lastLat, locationService.Latitude) || !Mathf.Approximately(lastLon, locationService.Longitude);
    }

    private Vector2 CalculatePos(float lat, float lon)
    {
        return CoordinatesTransformer.ProjectToMapBounds(lat, lon, width, height, bounds.Item1.x, bounds.Item1.y, bounds.Item2.x, bounds.Item2.y);
    }

    void UpdateMarker(GameObject marker, float lat, float lon, float mls)
    {
        marker.SetActive(mapImage.enabled);
        if(bounds == null)
        {
            return;
        }
        Vector2 pos = CalculatePos(lat, lon);
        marker.GetComponent<RectTransform>().position = pos;
        marker.GetComponent<UnityEngine.UI.Image>().color = operativeColor.Evaluate(mls / 100);
    }

    public Tuple<Vector2, Vector2> CalculateBoundsWithMargin(float lat, float lon)
    {
        float minLat = float.MaxValue;
        float minLon = float.MaxValue;
        float maxLat = float.MinValue;
        float maxLon = float.MinValue;
       

        foreach (var operative in markers.Values)
        {
            minLat = Mathf.Min(minLat, operative.pos.x);
            minLon = Mathf.Min(minLon, operative.pos.y);
            maxLat = Mathf.Max(maxLat, operative.pos.x);
            maxLon = Mathf.Max(maxLon, operative.pos.y);
        }

        minLat = Mathf.Min(minLat, lat);
        minLon = Mathf.Min(minLon, lon);
        maxLat = Mathf.Max(maxLat, lat);
        maxLon = Mathf.Max(maxLon, lon);

        minLat -= 0.00009f;
        maxLat += 0.00009f;
        minLon -= 0.00009f;
        maxLon += 0.00009f;

        return Tuple.Create(new Vector2(minLat,minLon), new Vector2(maxLat, maxLon));
    }
}
