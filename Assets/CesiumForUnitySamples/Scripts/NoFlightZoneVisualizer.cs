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

        float distanceBetweenCorners;
        float wallWidth = 100;

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

        // no idea, doesn´t work without
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

        wallCoordinateData[4] = cornerCoordinateData[4];
        wallCoordinateData[4].longitude = corner0longitude + (cornerCoordinateData[4].longitude - corner0longitude)/2;
        wallCoordinateData[4].latitude = corner0latitude + (cornerCoordinateData[4].latitude - corner0latitude)/2;
        wallCoordinateData[4].altitude = corner0altitude-(CornerAltDistance/2);

        /*
        // create walls 
        for(int i = 0; i < wallCoordinateData.Length; i++) {
            GameObject Wall = Instantiate(NoFlightWallPrefab);
            Wall.transform.SetParent(globeTransform, false);


            

            Wall.transform.localScale = new Vector3(wallWidth, CornerAltDistance, 100f);
            Wall.name = "Wall " + i + " " + wallCoordinateData[i].longitude + ", " + wallCoordinateData[i].latitude + ", " + wallCoordinateData[i].altitude;
            AddCesiumGlobeAnchor(Wall, wallCoordinateData[i], 0);
            arWalls[i] = Wall;
        }
        */
        // manuelle Version für Präsentation
        GameObject Wall_0 = Instantiate(NoFlightWallPrefab);
        Wall_0.transform.SetParent(globeTransform, false);
        Wall_0.transform.localScale = new Vector3(1600f, CornerAltDistance, 100f);
        Wall_0.name = "Wall " + 0 + " " + wallCoordinateData[0].longitude + ", " + wallCoordinateData[0].latitude + ", " + wallCoordinateData[0].altitude;
        AddCesiumGlobeAnchor(Wall_0, wallCoordinateData[0], 0);
        CesiumGlobeAnchor cesiumanchor0 = Wall_0.GetComponent<CesiumGlobeAnchor>();
        cesiumanchor0.rotationEastUpNorth = Quaternion.Euler(0, -19, 0);

        GameObject Wall_1 = Instantiate(NoFlightWallPrefab);
        Wall_1.transform.SetParent(globeTransform, false);
        Wall_1.transform.localScale = new Vector3(1750f, CornerAltDistance, 100f);
        Wall_1.name = "Wall " + 1 + " " + wallCoordinateData[1].longitude + ", " + wallCoordinateData[1].latitude + ", " + wallCoordinateData[1].altitude;
        AddCesiumGlobeAnchor(Wall_1, wallCoordinateData[1], 0);
        CesiumGlobeAnchor cesiumanchor1 = Wall_1.GetComponent<CesiumGlobeAnchor>();
        cesiumanchor1.rotationEastUpNorth = Quaternion.Euler(0, 15, 0);

        GameObject Wall_2 = Instantiate(NoFlightWallPrefab);
        Wall_2.transform.SetParent(globeTransform, false);
        Wall_2.transform.localScale = new Vector3(2250f, CornerAltDistance, 100f);
        Wall_2.name = "Wall " + 2 + " " + wallCoordinateData[2].longitude + ", " + wallCoordinateData[2].latitude + ", " + wallCoordinateData[2].altitude;
        AddCesiumGlobeAnchor(Wall_2, wallCoordinateData[2], 0);
        CesiumGlobeAnchor cesiumanchor2 = Wall_2.GetComponent<CesiumGlobeAnchor>();
        cesiumanchor2.rotationEastUpNorth = Quaternion.Euler(0, 90, 0);

        GameObject Wall_3 = Instantiate(NoFlightWallPrefab);
        Wall_3.transform.SetParent(globeTransform, false);
        Wall_3.transform.localScale = new Vector3(4450f, CornerAltDistance, 100f);
        Wall_3.name = "Wall " + 3 + " " + wallCoordinateData[3].longitude + ", " + wallCoordinateData[3].latitude + ", " + wallCoordinateData[3].altitude;
        AddCesiumGlobeAnchor(Wall_3, wallCoordinateData[3], 0);
        CesiumGlobeAnchor cesiumanchor3 = Wall_3.GetComponent<CesiumGlobeAnchor>();
        cesiumanchor3.rotationEastUpNorth = Quaternion.Euler(0, 0, 0);

        GameObject Wall_4 = Instantiate(NoFlightWallPrefab);
        Wall_4.transform.SetParent(globeTransform, false);
        Wall_4.transform.localScale = new Vector3(2500f, CornerAltDistance, 100f);
        Wall_4.name = "Wall " + 4 + " " + wallCoordinateData[4].longitude + ", " + wallCoordinateData[4].latitude + ", " + wallCoordinateData[4].altitude;
        AddCesiumGlobeAnchor(Wall_4, wallCoordinateData[4], 0);
        CesiumGlobeAnchor cesiumanchor4 = Wall_4.GetComponent<CesiumGlobeAnchor>();
        cesiumanchor4.rotationEastUpNorth = Quaternion.Euler(0, -59, 0);


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
        //first  long , second alt, third lat
        cesiumanchor.rotationEastUpNorth = Quaternion.Euler(0, 0, 0);
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
