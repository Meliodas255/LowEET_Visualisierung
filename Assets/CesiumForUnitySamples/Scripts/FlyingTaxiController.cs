using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using CesiumForUnity;
using Unity.Mathematics;

public class FlyingTaxiController : MonoBehaviour
{
    public string jsonFilePath;
    public Transform globeTransform; 
    public GameObject flyingTaxiPrefab; 
    public float flightSpeed = 100f; 

    private CoordinateData[] coordinateData;
    private int currentWaypointIndex = 0; 
    private GameObject flyingTaxi;
    private CesiumGlobeAnchor globeAnchor; 
        public Quaternion modelRotationOffset = Quaternion.Euler(90, 180, 45); // Offset um 90° in Y-Achse

    void Start()
    {
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);
        coordinateData = JsonConvert.DeserializeObject<CoordinateData[]>(jsonText);

        flyingTaxi = Instantiate(flyingTaxiPrefab);
        flyingTaxi.transform.SetParent(globeTransform, false);

        globeAnchor = flyingTaxi.AddComponent<CesiumGlobeAnchor>();

        SetFlyingTaxiPosition(coordinateData[0].latitude, coordinateData[0].longitude, coordinateData[0].altitude);
    }

    void Update()
    {
        if (currentWaypointIndex < coordinateData.Length - 20)
        {
            SetFlyingTaxiPosition(coordinateData[currentWaypointIndex].latitude,
                                  coordinateData[currentWaypointIndex].longitude,
                                  coordinateData[currentWaypointIndex].altitude);
            

           /* RotateTowardsNextWaypoint(
                coordinateData[currentWaypointIndex].latitude,
                coordinateData[currentWaypointIndex].longitude,
                coordinateData[currentWaypointIndex].altitude,
                coordinateData[currentWaypointIndex + 1].latitude,
                coordinateData[currentWaypointIndex + 1].longitude,
                coordinateData[currentWaypointIndex + 1].altitude
            );*/

            if (Vector3.Distance(flyingTaxi.transform.position, globeAnchor.transform.position) < 1f)
            //if (Vector3.Distance(flyingTaxi.transform.position, coordinateData[currentWaypointIndex+20].) < 1f)

            {
                currentWaypointIndex++;
            }
        }
        else
        {
            currentWaypointIndex = 0;
        }
    }

    /*void RotateTowardsNextWaypoint(float currentLatitude, float currentLongitude, float currentAltitude,
                               float nextLatitude, float nextLongitude, float nextAltitude)
    {
    double3 currentPosition = CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(new double3(currentLongitude, currentLatitude, currentAltitude));
    double3 nextPosition = CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(new double3(nextLongitude, nextLatitude, nextAltitude));
    double3 directionEcef = (nextPosition - currentPosition).normalized;
    double3 upEcef = CesiumWgs84Ellipsoid.GeodeticSurfaceNormal(currentPosition);

    globeAnchor.rotationGlobeFixed = quaternion.LookRotation(new float3(directionEcef), new float3(upEcef));
    }*/
    void RotateTowardsNextWaypoint(float currentLatitude, float currentLongitude, float currentAltitude,
                               float nextLatitude, float nextLongitude, float nextAltitude)
{
    // Konvertiere die aktuellen und nächsten Wegpunkte zu ECEF-Koordinaten
    Debug.Log("Vor Umrechnung in ecef:" + "lat "+currentLatitude + " long "+ currentLongitude+ "alt "+ currentAltitude  );

    double3 currentPosition = CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(new double3(currentLongitude, currentLatitude, currentAltitude));
    Debug.Log("ECEF Koordinaten:" + currentPosition );

    double3 nextPosition = CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(new double3(nextLongitude, nextLatitude, nextAltitude));
    Debug.Log("Werte der Koordinaten in ECEF aktuell:" +currentPosition + "  Nächste Position   "+ nextPosition  );
    // Berechne die Richtung und normalisiere sie
    double3 directionEcef = nextPosition - currentPosition;
    Debug.Log("Werte der Abzüge:" + directionEcef  );

    directionEcef = math.normalize(directionEcef);
    Debug.Log("Werte der Abzüge normalisiert:" + directionEcef );

    // Berechne die Aufwärtsrichtung
    double3 upEcef = CesiumWgs84Ellipsoid.GeodeticSurfaceNormal(currentPosition);
    Debug.Log("Werte up:" + upEcef );
    // Setze die Rotation basierend auf der berechneten Richtung
    //globeAnchor.rotationGlobeFixed = quaternion.LookRotation(new float3((float)directionEcef.x, (float)directionEcef.y, (float)directionEcef.z), 
                                                              //new float3((float)upEcef.x, (float)upEcef.y, (float)upEcef.z));
    //globeAnchor.rotationGlobeFixed = quaternion.LookRotation(new float3(directionEcef), new float3(upEcef));
        Quaternion rotation = Quaternion.LookRotation(
        new Vector3((float)directionEcef.x, (float)directionEcef.y, (float)directionEcef.z),
        new Vector3((float)upEcef.x, (float)upEcef.y, (float)upEcef.z)
    );

    rotation = rotation * modelRotationOffset;

    flyingTaxi.transform.rotation = rotation;
    Debug.Log($"Aktuelle Rotation (Quaternion): {globeAnchor.rotationGlobeFixed}");

// Konvertiere die Rotation zu Euler-Winkeln
Vector3 eulerAngles = ((Quaternion)globeAnchor.rotationGlobeFixed).eulerAngles;
Debug.Log($"Rotation in Euler-Winkeln: Pitch = {eulerAngles.x}, Yaw = {eulerAngles.y}, Roll = {eulerAngles.z}");
}

    void SetFlyingTaxiPosition(float latitude, float longitude, float altitude)
    {
        globeAnchor.latitude = latitude;
        globeAnchor.longitude = longitude;
        globeAnchor.height = altitude;
        globeAnchor.rotationEastUpNorth = Quaternion.Euler(-90, 45, 0);

    }

    [System.Serializable]
    public class CoordinateData
    {
        public float longitude;
        public float latitude;
        public float altitude;
    }
}