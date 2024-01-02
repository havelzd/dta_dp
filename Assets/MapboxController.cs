using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MapboxController : MonoBehaviour
{

    class GeoMarker
    {
        public float mls { get; set; }
        public Vector2 pos = new();
    }

    [SerializeField] RawImage mapImage;
    [SerializeField] string authToken;
    [SerializeField] string userName;
    [SerializeField] float transparency;
    [SerializeField] Gradient operativeColor;

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

    private void Awake()
    {
        locationService = LocationService.Instance;
        Debug.Log(locationService);
        width = Screen.width > 1080 ? 1080 : Screen.width;
        height = Screen.height > 1280 ? 1280 : Screen.height;

        RectTransform rectTransform = mapImage.rectTransform;
        rectTransform.sizeDelta = new Vector2(width, height);

        missionController = missionControllerGO.GetComponent<MissionController>();
    }

    void Start()
    {
        mapImage.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        foreach(var operative in missionController.operatives) {
            var key = int.Parse(operative.Key);
            var data = operative.Value.data;
            var lastData = data.Count > 0 ? data[data.Count - 1] : null;
            if(lastData != null) {
                if (markers.ContainsKey(key))
                {
                    markers[key].mls = lastData.mls;
                    markers[key].pos = new Vector2(lastData.location.lon, lastData.location.lat);
                }else
                {
                    markers[key] = new GeoMarker() { pos = new Vector2(lastData.location.lon, lastData.location.lat), mls = lastData.mls };
                }
                
            }
        }

        StartCoroutine(GetMap());
    }

    IEnumerator GetMap()
    {
        float lat = locationService.Latitude;
        float lon = locationService.Longitude;

        if (PositionsChanged())
        {

            var lonStr = lon.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string geojson = BuildGeoJson();
            Debug.Log("GeoJSON " + geojson);
            string url = string.Format(
                "{0}{1}/{2}/static/geojson({3})/{4},{5},{6},{7},{8}/{9}x{10}?access_token={11}",
                mapboxURL,
                "mapbox",
                "streets-v12",
                WWW.EscapeURL(geojson),
                lonStr,
                latStr,
                17, 0, 30,
                width, height, 
                authToken);

            Debug.Log(url);
            lastLat = lat;
            lastLon = lon;


            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            
            if(request.result == UnityWebRequest.Result.Success)
            {
                mapImage.enabled = true;
                Debug.Log("Response received");
                mapImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Color color = mapImage.color;
                color.a = transparency;
                mapImage.color = color;
            } else
            {
                Debug.Log("Error " + request.error + " " +  request.ToString());
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

    string BuildGeoJson()
    {
        var sb = new StringBuilder();
        List<string> features = new()
        {
            GetFeatureJSON("", ColorUtility.ToHtmlStringRGBA(operativeColor.Evaluate(1)), locationService.Longitude, locationService.Latitude)
        };

        foreach(var operative in lastMarkers)
        {
            features.Add(GetFeatureJSON("", ColorUtility.ToHtmlStringRGBA(operativeColor.Evaluate(operative.Value.mls / 100)), operative.Value.pos.x, operative.Value.pos.y));
        }

        sb.Append(@"{
            ""type"": ""FeatureCollection"",
            ""features"": [");

        sb.Append(string.Join(",", features));

        sb.Append(@"]}");

        return sb.ToString().Replace(" ", "").Replace("\n", "");
    }

    string GetFeatureJSON(string name, string color, float lon, float lat)
    {
        return $@"{{
                    ""type"": ""Feature"",
                    ""geometry"": {{
                        ""type"": ""Point"",
                        ""coordinates"": [{lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}]
                    }},
                    ""properties"": {{
                        ""marker-color"": ""#{color}""
                    }}
                }}
        ";
    }
}
