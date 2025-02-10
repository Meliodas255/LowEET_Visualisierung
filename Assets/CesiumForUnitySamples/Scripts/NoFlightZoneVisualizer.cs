using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using CesiumForUnity;
using Unity.Mathematics;


public class NoFlightZoneVisualizer : MonoBehaviour
{
    public string jsonFilePath;
    public GameObject NoFlightCornerPrefab;
    public GameObject NoFlightWallPrefab;
    public Transform globeTransform;
    public float CornerAltDistance = 1000;
    private CesiumGlobeAnchor globeAnchor;
    float wallRotationAlt = 0;

    // Start is called before the first frame update
    void Start()
    {
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);

        CoordinateData[] cornerCoordinateData = JsonConvert.DeserializeObject<CoordinateData[]>(jsonText);
        CoordinateData[] wallCoordinateData = new CoordinateData[cornerCoordinateData.Length];

        GameObject[] arLowCorner = new GameObject[cornerCoordinateData.Length];
        GameObject[] arUpperCorner = new GameObject[cornerCoordinateData.Length];
        GameObject[] arWalls = new GameObject[cornerCoordinateData.Length];

        float corner0longitude;
        float corner0latitude;
        float corner0altitude;

        float wallWidth;

        //create lower corners
        for(int i = 0; i < cornerCoordinateData.Length; i++) {
            GameObject LowCorner = Instantiate(NoFlightCornerPrefab);
            LowCorner.transform.SetParent(globeTransform, false);
            LowCorner.name = "LowCorner " + cornerCoordinateData[i].longitude + ", " + cornerCoordinateData[i].latitude + ", " + cornerCoordinateData[i].altitude;
            AddCesiumGlobeAnchor(LowCorner, cornerCoordinateData[i], 0);
            arLowCorner[i] = LowCorner;
        };

        // create upper corners
        for(int i = 0; i < cornerCoordinateData.Length; i++) {
            GameObject UpperCorner = Instantiate(NoFlightCornerPrefab);
            UpperCorner.transform.SetParent(globeTransform, false);
            UpperCorner.name = "UpperCorner " + cornerCoordinateData[i].longitude + ", " + cornerCoordinateData[i].latitude + ", " + cornerCoordinateData[i].altitude;
            cornerCoordinateData[i].altitude += CornerAltDistance;
            AddCesiumGlobeAnchor(UpperCorner, cornerCoordinateData[i], 0);
            arUpperCorner[i] = UpperCorner;
        };

        // corner 0 Coordinates get overwritten somehow, saving them here for further use
        corner0longitude = cornerCoordinateData[0].longitude;
        corner0latitude = cornerCoordinateData[0].latitude;
        corner0altitude = cornerCoordinateData[0].altitude;

        // calculate wall coordinates
        for(int i = 0; i < wallCoordinateData.Length-1; i++) {
            wallCoordinateData[i] = cornerCoordinateData[i];
            wallCoordinateData[i].longitude = cornerCoordinateData[i].longitude + (cornerCoordinateData[i+1].longitude - cornerCoordinateData[i].longitude)/2;
            wallCoordinateData[i].latitude = cornerCoordinateData[i].latitude + (cornerCoordinateData[i+1].latitude - cornerCoordinateData[i].latitude)/2;
            wallCoordinateData[i].altitude = cornerCoordinateData[i].altitude-(CornerAltDistance/2);
        }

        // calculate wall coordinates of last wall in array
        wallCoordinateData[wallCoordinateData.Length-1] = cornerCoordinateData[cornerCoordinateData.Length-1];
        wallCoordinateData[wallCoordinateData.Length-1].longitude = corner0longitude + (cornerCoordinateData[cornerCoordinateData.Length-1].longitude - corner0longitude)/2;
        wallCoordinateData[wallCoordinateData.Length-1].latitude = corner0latitude + (cornerCoordinateData[cornerCoordinateData.Length-1].latitude - corner0latitude)/2;
        wallCoordinateData[wallCoordinateData.Length-1].altitude = corner0altitude-(CornerAltDistance/2);

        // create walls 
        for(int i = 0; i < wallCoordinateData.Length; i++) {
            GameObject Wall = Instantiate(NoFlightWallPrefab);
            Wall.transform.SetParent(globeTransform, false);

            // wallWidth = distance between corners
            if (i == wallCoordinateData.Length-1) {
                wallWidth = Vector3.Distance (arLowCorner[0].transform.position, arLowCorner[arLowCorner.Length-1].transform.position);  
            }
            else {
                wallWidth = Vector3.Distance (arLowCorner[i].transform.position, arLowCorner[i+1].transform.position);  
            }

            // scaling of walls
            Wall.transform.localScale = new Vector3(wallWidth, CornerAltDistance, 100f);

            /*
            // calculate rotation of wall (not used because calculated values don't make sense)
            if (i == wallCoordinateData.Length-1) {
                wallRotationAlt = math.atan2(cornerCoordinateData[0].latitude-cornerCoordinateData[cornerCoordinateData.Length-1].latitude, cornerCoordinateData[0].longitude-cornerCoordinateData[cornerCoordinateData.Length-1].longitude)*(360/(2*math.PI));
            }
            else {
                wallRotationAlt = math.atan2(cornerCoordinateData[i+1].latitude-cornerCoordinateData[i].latitude ,cornerCoordinateData[i+1].longitude-cornerCoordinateData[i].longitude)*(360/(2*math.PI));
            }
            Debug.Log(wallRotationAlt);
            */
            

            Wall.name = "Wall " + i + " " + wallCoordinateData[i].longitude + ", " + wallCoordinateData[i].latitude + ", " + wallCoordinateData[i].altitude;
            AddCesiumGlobeAnchor(Wall, wallCoordinateData[i], 0);
            arWalls[i] = Wall;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 ConvertCoordinates(float longi, float lat, float height)
    {
        return new Vector3(longi, lat, height);
    }

    void AddCesiumGlobeAnchor(GameObject GameObject, CoordinateData coordinate, float rot)
    {
        CesiumGlobeAnchor cesiumanchor = GameObject.GetComponent<CesiumGlobeAnchor>();
        if(cesiumanchor == null)
        {
            cesiumanchor = GameObject.AddComponent<CesiumGlobeAnchor>();
        }
        cesiumanchor.latitude = coordinate.latitude;
        cesiumanchor.longitude = coordinate.longitude;
        cesiumanchor.height = coordinate.altitude;

        // rotate game object on it's own axis
        cesiumanchor.rotationEastUpNorth = Quaternion.Euler(0, wallRotationAlt, 0);
    }
    
    void RotateTowardsNextWaypoint(float currentLatitude, float currentLongitude, float currentAltitude, float nextLatitude, float nextLongitude, float nextAltitude)
    {
        // Konvertiere die aktuellen und nächsten Wegpunkte zu ECEF-Koordinaten
        double3 currentPosition = CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(new double3(currentLongitude, currentLatitude, currentAltitude));
        double3 nextPosition = CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(new double3(nextLongitude, nextLatitude, nextAltitude));
        Debug.Log("Werte der Positonen:" +currentPosition + "     "+ nextPosition  );
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
        globeAnchor.rotationGlobeFixed = quaternion.LookRotation(new float3(directionEcef), new float3(upEcef));
        Debug.Log($"Aktuelle Rotation (Quaternion): {globeAnchor.rotationGlobeFixed}");
    }

    [System.Serializable]
    public class CoordinateData
    {
        public float longitude;
        public float latitude;
        public float altitude;
    }
}
