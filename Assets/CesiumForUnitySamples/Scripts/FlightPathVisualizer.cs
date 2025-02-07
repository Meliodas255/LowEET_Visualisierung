using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using CesiumForUnity;

public class FlightPathVisualizer : MonoBehaviour
{
    public string jsonFilePath;
    public GameObject waypointPrefab;
    public GameObject VertiportPrefab;

    public Transform globeTransform;
    public Color lineColor = Color.red;
    public CesiumGeoreference georeference;

    // Start is called before the first frame update
    void Start()
    {
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);
        CoordinateData[] coordinateData = JsonConvert.DeserializeObject<CoordinateData[]>(jsonText);

        GameObject[] waypoints = new GameObject[coordinateData.Length];
        for (int i = 0; i < coordinateData.Length - 30; i += 30)
        {
            GameObject waypoint = Instantiate(waypointPrefab);

            Vector3d worldPosition = ConvertLatLonAltToECEF(coordinateData[i].latitude, coordinateData[i].longitude, coordinateData[i].altitude);
            waypoint.transform.position = new Vector3((float)worldPosition.x, (float)worldPosition.y, (float)worldPosition.z);
            waypoint.transform.SetParent(globeTransform, false);

            // Berechne die Rotation in East-Up-North
            Quaternion rotationEUN = CalculateRotationEUN(coordinateData[i], coordinateData[i + 1]);
            waypoint.transform.rotation = rotationEUN;

            // Setze die Rotation des CesiumGlobeAnchor
            AddCesiumGlobeAnchor(waypoint, coordinateData[i], rotationEUN);

            waypoint.name = "Waypoint" + i;
            waypoint.tag = "Waypoint";

            waypoints[i] = waypoint;
        }
        for (int i = 0; i < coordinateData.Length -5 ;i += 1)
        {
            if(i == 0||i==coordinateData.Length-5){
            GameObject Vertiport = Instantiate(VertiportPrefab);

            Vector3d worldPosition = ConvertLatLonAltToECEF(coordinateData[i].latitude, coordinateData[i].longitude, coordinateData[i].altitude);
            Vertiport.transform.position = new Vector3((float)worldPosition.x, (float)worldPosition.y, (float)worldPosition.z);
            Vertiport.transform.SetParent(globeTransform, false);

            
            // Setze die Rotation des CesiumGlobeAnchor
            AddCesiumGlobeAnchorPort(Vertiport, coordinateData[i]);
            }

        }

    }

    Vector3d ConvertLatLonAltToECEF(double latitude, double longitude, double altitude)
    {
        const double a = 6378137.0;           // Halbachse in Metern
        const double f = 1 / 298.257223563;    // Abplattung der Erde
        const double e2 = f * (2 - f);         // Quadrat der Exzentrizität

        double latRad = latitude * Math.PI / 180.0;
        double lonRad = longitude * Math.PI / 180.0;

        double N = a / Math.Sqrt(1 - e2 * Math.Sin(latRad) * Math.Sin(latRad));

        double x = (N + altitude) * Math.Cos(latRad) * Math.Cos(lonRad);
        double y = (N + altitude) * Math.Cos(latRad) * Math.Sin(lonRad);
        double z = (N * (1 - e2) + altitude) * Math.Sin(latRad);
        //Debug.Log("x"+x);
        //Debug.Log("y" + y);
        //Debug.Log("z" + z);

        return new Vector3d(x, y, z);
    }

    Quaternion CalculateRotationEUN(CoordinateData start, CoordinateData end)
    {
        Vector3d startECEF = ConvertLatLonAltToECEF(start.latitude, start.longitude, start.altitude);
        Vector3d endECEF = ConvertLatLonAltToECEF(end.latitude, end.longitude, end.altitude);

        Vector3d direction = new Vector3d(endECEF.x - startECEF.x, endECEF.y - startECEF.y, endECEF.z - startECEF.z).normalized;

        Vector3d up = new Vector3d(startECEF.x, startECEF.y, startECEF.z).normalized;
        Vector3d east = Vector3d.Cross(up, new Vector3d(0, 0, 1)).normalized;
        Vector3d north = Vector3d.Cross(up, east).normalized;

        Matrix4x4 matrix = new Matrix4x4();
        matrix.SetColumn(0, new Vector4((float)east.x, (float)east.y, (float)east.z, 0));
        matrix.SetColumn(1, new Vector4((float)north.x, (float)north.y, (float)north.z, 0));
        matrix.SetColumn(2, new Vector4((float)up.x, (float)up.y, (float)up.z, 0));
        matrix.SetColumn(3, new Vector4(0, 0, 0, 1));

        return Quaternion.LookRotation(matrix.MultiplyVector(new Vector3((float)direction.x, (float)direction.y, (float)direction.z)));
    }

    void AddCesiumGlobeAnchor(GameObject waypoint, CoordinateData coordinate, Quaternion rotation)
    {
        CesiumGlobeAnchor cesiumAnchor = waypoint.GetComponent<CesiumGlobeAnchor>();
        if (cesiumAnchor == null)
        {
            cesiumAnchor = waypoint.AddComponent<CesiumGlobeAnchor>();
        }
        cesiumAnchor.latitude = coordinate.latitude;
        cesiumAnchor.longitude = coordinate.longitude;
        cesiumAnchor.height = coordinate.altitude;
            
        //Debug.Log("ROTATION" + rotation);
        Quaternion rotationOffset = Quaternion.Euler(45, 45, 0); // Werte hier anpassen

        // Berechne die endgültige Rotation
        Quaternion finalRotation = rotation * rotationOffset;
        // Setze die Rotation des Cesium Globe Anchors
        cesiumAnchor.rotationEastUpNorth = finalRotation; 
    }

        void AddCesiumGlobeAnchorPort(GameObject vertiport, CoordinateData coordinate)
    {
        CesiumGlobeAnchor cesiumAnchor = vertiport.GetComponent<CesiumGlobeAnchor>();
        if (cesiumAnchor == null)
        {
            cesiumAnchor = vertiport.AddComponent<CesiumGlobeAnchor>();
        }
        cesiumAnchor.latitude = coordinate.latitude;
        cesiumAnchor.longitude = coordinate.longitude;
        cesiumAnchor.height = coordinate.altitude;
        cesiumAnchor.rotationEastUpNorth = Quaternion.Euler(-90, 45, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    [System.Serializable]
    public class CoordinateData
    {
        public double longitude;
        public double latitude;
        public double altitude;
    }
}

public struct Vector3d
{
    public double x, y, z;

    public Vector3d(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Vector3d Cross(Vector3d a, Vector3d b)
    {
        return new Vector3d(
            a.y * b.z - a.z * b.y,
            a.z * b.x - a.x * b.z,
            a.x * b.y - a.y * b.x
        );
    }

    public Vector3d normalized
    {
        get
        {
            double magnitude = Math.Sqrt(x * x + y * y + z * z);
            return new Vector3d(x / magnitude, y / magnitude, z / magnitude);
        }
    }
}