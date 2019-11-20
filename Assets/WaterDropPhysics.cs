using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterDropPhysics : MonoBehaviour
{
    List<GameObject> waterDrops;

    // Start is called before the first frame update
    void Start()
    {
        waterDrops = GameObject.FindGameObjectsWithTag("WaterDrop").Except(new List<GameObject> { gameObject }).ToList();
    }

    void FixedUpdate()
    {
        var myPosition = gameObject.transform.position;
        var forceVector = new Vector3();
        foreach (var avoid in waterDrops)
        {
            var distanceVector = myPosition - avoid.transform.position;
            forceVector += (distanceVector / (float)Math.Pow(distanceVector.magnitude, 5));
        }

        if (float.IsNaN(forceVector.x) || float.IsNaN(forceVector.y) || float.IsNaN(forceVector.z))
        {
            throw new System.ArgumentOutOfRangeException("Got NaN when calculating water drop physics");
        }
        GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);
    }
}
