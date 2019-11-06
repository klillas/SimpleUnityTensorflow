using System;
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
    List<GameObject> spawnedPredictionObjects;
    DateTime previousResetTime;
    bool predictionRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        spawnedObjects = new List<GameObject>();
        spawnedPredictionObjects = new List<GameObject>();
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

        if (spawnedPredictionObjects.Count < 1)
        {
            float xPos = 0 + (spawnedPredictionObjects.Count * xDelta);
            float yPos = 2;
            float zPos = -30;
            var predictionObject = Instantiate(SpawnObject, new Vector3(xPos, yPos, zPos), Quaternion.identity);
            predictionObject.GetComponent<TensorflowTrainer>().Predict = true;
            predictionObject.GetComponent<TensorflowTrainer>().Paused = true;
            Destroy(predictionObject.GetComponent<Rigidbody>());
            Destroy(predictionObject.GetComponent<SphereCollider>());
            spawnedPredictionObjects.Add(predictionObject);
        }

        var secondsToWait = TimeSpan.FromSeconds(40);
        if (predictionRunning == true)
        {
            secondsToWait = TimeSpan.FromSeconds(10);
        }

        if (DateTime.Now - previousResetTime > secondsToWait)
        {
            predictionRunning = !predictionRunning;
            previousResetTime = DateTime.Now;

            if (!predictionRunning)
            {
                System.Threading.Thread.Sleep(2000);
            }

            foreach (var spawnedObject in spawnedObjects)
            {
                var position = new Vector3(
                    spawnedObject.transform.position.x,
                    yStart,
                    spawnedObject.transform.position.z);

                spawnedObject.GetComponent<TensorflowTrainer>().ResetTraining(position);
                spawnedObject.GetComponent<TensorflowTrainer>().Paused = predictionRunning;
            }

            if (predictionRunning)
            {
                System.Threading.Thread.Sleep(2000);
            }

            foreach (var spawnedPredictionObject in spawnedPredictionObjects)
            {
                float xPos = 0 + (spawnedPredictionObjects.IndexOf(spawnedPredictionObject) * xDelta);
                float yPos = 2;
                float zPos = -30;

                var position = new Vector3(
                    xPos,
                    yPos,
                    zPos);

                spawnedPredictionObject.GetComponent<TensorflowTrainer>().ResetTraining(position);
                spawnedPredictionObject.GetComponent<TensorflowTrainer>().Paused = !predictionRunning;
            }
        }
    }
}
