using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using System;

public class ProjectionTest
{

    double[][] coordinatesAndDist = new double[][]
    {
        new double[] { 50.077810, 14.461098, 50.078424, 14.473058, 858.8 },
    };

    [Test]
    public void TestCoordinatesProjection()
    {
        foreach (var coordinates in coordinatesAndDist)
        {
            var dists = CompareDistance(coordinates[0], coordinates[1], coordinates[2], coordinates[3]);
            var geoDistM = dists.Item1;
            var cartDistM = dists.Item2;

            Assert.IsTrue(IsSimilar(geoDistM, cartDistM));
            Assert.IsTrue(IsSimilar(cartDistM,(float) coordinates[4]));

        }
    }
   
    private Tuple<float,float> CompareDistance(double lat1, double lon1, double lat2, double lon2)
    {
        Vector3 position = TransformCoordinates((float)lat1, (float)lon1, 0, (float)lat2, (float)lon2, 0);
        
        float geoDistM = (float) CoordinatesTransformer.CalculateDistanceM(lat1, lon1, lat2, lon2);
        float cartDistM = Vector3.Distance(new() { x = 0, y = 0, z = 0 }, position);
        
        return Tuple.Create(geoDistM, cartDistM);
    }

    private bool IsSimilar(float a, float b)
    {
        Debug.Log($"{a}, {b}");
        return Mathf.Abs(a- b) < Mathf.Max(1E-02f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.Epsilon * 8f);
    }

    public Vector3 TransformCoordinates(float lat1, float lon1, float alt1, float lat2, float lon2, float alt2)
    {
        return CoordinatesTransformer.ConvertGeoToCartesian(lat1, lon1, alt1, lat2, lon2, alt2);
    }

}
