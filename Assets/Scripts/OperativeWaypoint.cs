using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class OperativeWaypoint : MonoBehaviour
{

    public GameObject operative;
    public GameObject arCamera;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("OperativeWaypoint init");
    }

    // Update is called once per frame
    void Update()
    {
        //if(operative != null && operative.transform != null)
        //{

        //    RectTransform rectTransform = GetComponent<RectTransform>();
        //    if (rectTransform != null)
        //    {
        //        ARCameraManager arCameraManager = arCamera.GetComponent<ARCameraManager>();
        //        if (arCameraManager != null)
        //        {
        //            var arCamera = arCameraManager.GetComponent<Camera>();
        //            Vector2 pos = arCamera.WorldToScreenPoint(operative.transform.position);

        //            if(Vector3.Dot(operative.transform.position - arCamera.transform.position, arCamera.transform.forward) < 0) {

        //            }
        //                               {
        //                // operative is behind camera
        //                Debug.Log("Operative is behind camera");
        //                rectTransform.gameObject.SetActive(false);
        //            }
        //                               else
        //            {
        //                rectTransform.gameObject.SetActive(true);
        //            })
        //            rectTransform.anchoredPosition = arCameraManager.GetComponent<Camera>().WorldToScreenPoint(operative.transform.position);
        //            Debug.Log("Updating WaypointPos " + rectTransform.anchoredPosition);
        //        }
                
        //    }
            
        //}
        
    }
}
