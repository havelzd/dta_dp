using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GeospatialController : MonoBehaviour
{

    public ARCoreExtensions arCoreExtensions;
    public AREarthManager arEarthManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Do nothing if we haven't enabled ARCoreExtensions yet.
        if (!arCoreExtensions.enabled)
            return;

        // Do nothing if session isn't ready yet.
        if (ARSession.state != ARSessionState.SessionTracking || Input.location.status != LocationServiceStatus.Running)
        {
            //Debug.Log("Session not ready yet");
            return;
        }

        // Do nothing if not configured properly.
        if (arCoreExtensions.ARCoreExtensionsConfig.GeospatialMode == GeospatialMode.Disabled)
        {
            Debug.LogError("GeospatialMode is disabled. Please enable it in the ARCore Extensions Config object.");
            return;
        }

        if (arEarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled) != FeatureSupported.Supported)
        {
            // On fresh install the value is always FeatureSupported.Unknown..
            return;
        }

    }

    public void AfterPermissionsGranted()
    {
        // Script is disabled by default since we want to wait for permissions to be granted first, enable it now.
        arCoreExtensions.enabled = true;
        Debug.Log("Enabling ARCORE Extensions");
    }
}
