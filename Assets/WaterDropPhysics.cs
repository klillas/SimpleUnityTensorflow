using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterDropPhysics : MonoBehaviour
{
    List<GameObject> waterDrops;
    List<GameObject> planes;

    // Start is called before the first frame update
    void Start()
    {
        waterDrops = GameObject.FindGameObjectsWithTag("WaterDrop").Except(new List<GameObject> { gameObject }).ToList();
        planes = GameObject.FindGameObjectsWithTag("Plane").ToList();
    }

    void FixedUpdate()
    {
        var myPosition = gameObject.transform.position;
        var forceVector = new Vector3();
        foreach (var avoid in waterDrops)
        {
            var distanceVector = myPosition - avoid.transform.position;
            forceVector += (distanceVector / (float)Math.Pow(distanceVector.magnitude, 5));

            //Color color = Color.blue;
            //Debug.DrawLine(myPosition, myPosition - distanceVector, color);
        }

        foreach (var avoid in planes)
        {
            var avoidCollider = avoid.GetComponent<Collider>();
            // var closestPlanePosition = avoidCollider.ClosestPointOnBounds(myPosition);
            var closestPlanePosition = avoidCollider.ClosestPoint(myPosition);
            var distanceVector = myPosition - closestPlanePosition;
            forceVector += (distanceVector / (float)Math.Pow(distanceVector.magnitude, 5));

            //Color color = Color.white;
            //Debug.DrawLine(myPosition, myPosition - distanceVector, color);
        }

        if (float.IsNaN(forceVector.x) || float.IsNaN(forceVector.y) || float.IsNaN(forceVector.z))
        {
            throw new System.ArgumentOutOfRangeException("Got NaN when calculating water drop physics");
        }
        GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);
    }
}
