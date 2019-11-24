using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegrams;
using UnityEngine;

public class TensorflowPredictor : MonoBehaviour
{
    ExternalCommunication externalCommunication;
    Queue<Vector3> bufferedVelocityPredictions;
    List<GameObject> waterDrops;
    List<GameObject> planes;
    float positionReduceFactor = 1000;
    float velocityReduceFactor = 1;

    private void Start()
    {
        bufferedVelocityPredictions = new Queue<Vector3>();
        externalCommunication = ExternalCommunication.GetSingleton();
        waterDrops = GameObject.FindGameObjectsWithTag("WaterDrop").ToList();
        planes = GameObject.FindGameObjectsWithTag("Plane").ToList();
        CreatePredictRequest();
    }

    private void CreatePredictRequest()
    {
        var predict_x = new List<float>();

        // Find the group middle position
        Vector3 startPos = new Vector3();
        foreach (var waterDrop in waterDrops)
        {
            startPos += waterDrop.transform.position;
        }
        startPos /= waterDrops.Count();

        foreach (var waterDrop in waterDrops)
        {
            var reducedPosition = (waterDrop.transform.position - startPos) / positionReduceFactor;
            var reducedVelocity = waterDrop.GetComponent<WaterDropBody>().Velocity / velocityReduceFactor;

            predict_x.Add(reducedPosition.x);
            predict_x.Add(reducedPosition.y);
            predict_x.Add(reducedPosition.z);
        }

        foreach (var plane in planes)
        {
            var verticeList = plane.GetComponent<MeshFilter>().sharedMesh.vertices;
            List<int> verticeIndexes = new List<int> { 0, 10, 110, 120 };
            foreach (var verticeIndex in verticeIndexes)
            {
                var corner = (plane.transform.TransformPoint(verticeList[verticeIndex]) - startPos) / positionReduceFactor;
                predict_x.Add(corner.x);
                predict_x.Add(corner.y);
                predict_x.Add(corner.z);
            }
        }

        var request = TelegramFactory.CreatePredictRequest(predict_x);
        externalCommunication.SendAsynch(request, CreatePredictRequestAnswer);
    }

    private void CreatePredictRequestAnswer(Request requestAnswer)
    {
        var prediction_y = requestAnswer.PredictionY;
        int predictionIndex = 0;
        foreach (var waterDrop in waterDrops)
        {
            var velocityDiff = new Vector3(
                prediction_y[predictionIndex],
                prediction_y[predictionIndex + 1],
                prediction_y[predictionIndex + 2]);

            velocityDiff *= velocityReduceFactor;

            predictionIndex += 3;

            var waterDropBody = waterDrop.GetComponent<WaterDropBody>();
            waterDropBody.Velocity += velocityDiff;
            Vector3 newPosition = waterDrop.transform.position;
            newPosition.x += waterDropBody.Velocity.x * Time.fixedDeltaTime;
            newPosition.y += waterDropBody.Velocity.y * Time.fixedDeltaTime;
            newPosition.z += waterDropBody.Velocity.z * Time.fixedDeltaTime;
            waterDrop.transform.position = newPosition;
        }

        CreatePredictRequest();

        /*
        for (int i = 0; i < prediction_y.Count; i = i + 3)
        {
            bufferedVelocityPredictions.Enqueue(new Vector3(
                prediction_y[i],
                prediction_y[i + 1],
                prediction_y[i + 2]));
        }
        */
    }

    private void FixedUpdate()
    {
        /*
        if (bufferedVelocityPredictions.Count == 10)
        {
            CreatePredictRequest();
        }

        if (bufferedVelocityPredictions.Count == 0)
        {
            return;
        }

        var prediction = bufferedVelocityPredictions.Dequeue();
        currentVelocity.x += prediction[0];
        currentVelocity.y += prediction[1];
        currentVelocity.z += prediction[2];
        Debug.Log(currentVelocity.x + " " + currentVelocity.y + " " + currentVelocity.z);
        var newPos = transform.position;
        newPos.x += currentVelocity.x * Time.fixedDeltaTime;
        newPos.y += currentVelocity.y * Time.fixedDeltaTime;
        newPos.z += currentVelocity.z * Time.fixedDeltaTime;
        transform.position = newPos;
        */
    }
}
