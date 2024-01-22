using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinatesTransformer
{

    const double EARTH_RADIUS_EQ_M = 6378137;
    const double EARTH_RADIUS_PO_M = 6356752;
    const double EARTH_RADIUS_MEAN_M = 6371000.0;
    const double DEGREE_TO_RADIANS = Math.PI / 180.0;
    const double ORIGIN_SHIFT = 2 * Mathf.PI * 6378137 / 2.0;

    public static Vector3 ConvertGeoToCartesian(
            double userLat, double userLon, double userAlt,
            double targetLat, double targetLon, double targetAlt)
    {
        double latRad = DegreesToRadians(userLat);

        // Calculate an approximate radius of earth at given latitude
        //double localEarthRadius = GetEarthRadius(userLat);

        // Get meters per degree of lat / lon
        double latToMeters = DegreesToRadians(EARTH_RADIUS_MEAN_M);
        double lonToMeters = DegreesToRadians(EARTH_RADIUS_MEAN_M * Math.Cos(latRad));

       
        double x = (userLon - targetLon) * lonToMeters;
        double y = userAlt - targetAlt;
        double z = (userLat - targetLat) * latToMeters;
        return new Vector3((float)x, (float)y, (float)z);
    }

    private static double GetEarthRadius(double lat)
    {
        double latRad = DegreesToRadians(lat);
        double rst = EARTH_RADIUS_EQ_M * Math.Sin(latRad);
        double rct = EARTH_RADIUS_PO_M * Math.Cos(latRad);
        return EARTH_RADIUS_EQ_M * EARTH_RADIUS_PO_M / Math.Sqrt(Math.Pow(rst,2) + Math.Pow(rct, 2));
    }

    public static double CalculateDistanceM(double lat1, double lon1, double lat2, double lon2)
    {
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        lat1 = DegreesToRadians(lat1);
        lat2 = DegreesToRadians(lat2);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EARTH_RADIUS_MEAN_M * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * DEGREE_TO_RADIANS;
    }

    // Convert lat/lon to Mercator px, py
    public static Vector2 LatLonToMeters(float latitude, float longitude)
    {
        var p = new Vector2();
        p.x = (float)(longitude * ORIGIN_SHIFT / 180.0);
        p.y = (float)(Mathf.Log(Mathf.Tan((float)((90 + latitude) * Mathf.PI / 360.0))) / (Mathf.PI / 180.0));
        p.y = (float)(p.y * ORIGIN_SHIFT / 180.0);
        return p;
    }

    // Convert Mercator meters to pixel coordinates within given bounds and map size
    public static Vector2 MetersToPixels(Vector2 m, Vector2 minPoint, Vector2 maxPoint, float mapWidth, float mapHeight)
    {
        var mapWidthGeo = maxPoint.x - minPoint.x;
        var mapHeightGeo = maxPoint.y - minPoint.y;

        var x = (((m.x - minPoint.x) / mapWidthGeo) * mapWidth) + mapWidth /2 ;
        var y = ((1 - (m.y - minPoint.y) / mapHeightGeo) * mapHeight) - mapHeight /2; // Inverting Y for Unity's coordinate system

        return new Vector2(x, y);
    }

    // Convert lat/lon to pixel coordinates within given bounds and map size
    //public static Vector2 LatLonToPixel(float latitude, float longitude, Vector2 minLatLon, Vector2 maxLatLon, float mapWidth, float mapHeight)
    //{
    //    var meters = LatLonToMeters(latitude, longitude);
    //    var minPoint = LatLonToMeters(minLatLon.x, minLatLon.y);
    //    var maxPoint = LatLonToMeters(maxLatLon.x, maxLatLon.y);
    //    return MetersToPixels(meters, minPoint, maxPoint, mapWidth, mapHeight);
    //}

    static Tuple<double,double> LatLonToWebMercator(float lat, float lon)
    {
        double x = (lon + 180) / 360;
        double sinLatitude = Mathf.Sin(lat * Mathf.Deg2Rad);
        double y = (0.5 - Mathf.Log((float)((1 + sinLatitude) / (1 - sinLatitude))) / (4 * Mathf.PI));
        return Tuple.Create(x, y);
    }

    public static Vector2 ProjectToMapBounds(float lat, float lon, float mapWidth, float mapHeight, float minLat, float minLon, float maxLat, float maxLon)
    {
        var mercatorCoords = LatLonToWebMercator(lat, lon);
        var minMercator = LatLonToWebMercator(minLat, minLon);
        var maxMercator = LatLonToWebMercator(maxLat, maxLon);

        double normalizedX = (mercatorCoords.Item1 - minMercator.Item1) / (maxMercator.Item1 - minMercator.Item1);
        double normalizedY = (mercatorCoords.Item2 - minMercator.Item2) / (maxMercator.Item2 - minMercator.Item2);

        return new Vector2((float)(normalizedX * mapWidth), (float)(normalizedY * mapHeight));
    }
}
