using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CesiumForUnity;

public class AddAnchor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        foreach(GameObject waypoint in waypoints)
        {
            if (!waypoint.GetComponent<CesiumGlobeAnchor>()) {
                waypoint.AddComponent<CesiumGlobeAnchor>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
