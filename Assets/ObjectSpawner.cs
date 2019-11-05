using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject SpawnObject;
    float xStart = -44;
    float xEnd = 44;
    float xDelta = 2;
    float zStart = -44;
    float zEnd = 44;
    float zDelta = 2;
    float yStart = (float)2.78;

    float xNow;
    float zNow;

    List<GameObject> spawnedObjects;
    DateTime previousResetTime;

    // Start is called before the first frame update
    void Start()
    {
        spawnedObjects = new List<GameObject>();
        previousResetTime = DateTime.Now;
        xNow = xStart;
        zNow = zStart;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedObjects.Count < 100)
        {
            xNow = xNow + xDelta;

            if (xNow > xEnd)
            {
                xNow = xStart;
                zNow = zNow + zDelta;
            }

            if (zNow > zEnd)
            {
                zNow = zStart;
            }

            spawnedObjects.Add(Instantiate(SpawnObject, new Vector3(xNow, yStart, zNow), Quaternion.identity));
        }

        if (DateTime.Now - previousResetTime > TimeSpan.FromSeconds(15))
        {
            previousResetTime = DateTime.Now;

            foreach (var spawnedObject in spawnedObjects)
            {
                spawnedObject.transform.position = new Vector3(
                    spawnedObject.transform.position.x,
                    yStart,
                    spawnedObject.transform.position.z);
            }
        }
    }
}
