using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAvoider : MonoBehaviour
{
    GameObject[] avoidableObjects;

    // Start is called before the first frame update
    void Start()
    {
        avoidableObjects = GameObject.FindGameObjectsWithTag("AvoidableObject");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        var myPosition = gameObject.transform.position;
        var forceVector = new Vector3();
        foreach (var avoid in avoidableObjects)
        {
            var distanceVector = myPosition - avoid.transform.position;
            forceVector += (distanceVector / (float)Math.Pow(distanceVector.magnitude, 5));
        }

        GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);
    }
}
