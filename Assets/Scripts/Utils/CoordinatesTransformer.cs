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

    public static Vector2 LatLonToWebMercator(float lat, float lon)
    {
        float x = (lon + 180.0f) / 360.0f;

        float sinLatitude = Mathf.Sin(lat * Mathf.Deg2Rad);
        float y = 0.5f - Mathf.Log((1.0f + sinLatitude) / (1.0f - sinLatitude)) / (4.0f * Mathf.PI);

        return new Vector2(x, y);
    }

    public static Vector2 NormalizeMercatorCoordinates(Vector2 mercatorPos, Vector2 mercatorTopLeft, Vector2 mercatorBottomRight)
    {
        Vector2 normalized = new Vector2(
            (mercatorPos.x - mercatorTopLeft.x) / (mercatorBottomRight.x - mercatorTopLeft.x),
            (mercatorPos.y - mercatorTopLeft.y) / (mercatorBottomRight.y - mercatorTopLeft.y)
        );

        return normalized;
    }
}
