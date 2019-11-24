using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TensorflowTrainer : MonoBehaviour, ITensorflowTrainer
{
    ExternalCommunication externalCommunication;
    int stepsToPredict = 1;
    List<GameObject> waterDrops;
    List<GameObject> planes;

    private DateTime scriptStart;
    private float delayStartMs = 300;

    // Start is called before the first frame update
    void Start()
    {
        scriptStart = DateTime.Now;
        UpdateTracker.SubscribeNotifyEvent(UpdateTracker.UpdateEventType.AllFixedUpdateSet, NotifyAllFixedUpdateSet);
        externalCommunication = ExternalCommunication.GetSingleton();
        waterDrops = GameObject.FindGameObjectsWithTag("WaterDrop").ToList();
        planes = GameObject.FindGameObjectsWithTag("Plane").ToList();
    }

    void NotifyAllFixedUpdateSet(UpdateTracker.UpdateEventType updateEventType)
    {
        if (DateTime.Now - scriptStart < TimeSpan.FromMilliseconds(delayStartMs))
        {
            return;
        }

        float positionReduceFactor = 1000;
        float velocityReduceFactor = 1;
        var training_x = new List<float>();
        var training_y = new List<float>();

        // Find the group middle position
        Vector3 startPos = new Vector3();
        foreach (var waterDrop in waterDrops)
        {
            startPos += waterDrop.transform.position;
        }
        startPos /= waterDrops.Count();

        foreach (var waterDrop in waterDrops)
        {
            var stateHistory = waterDrop.GetComponent<StateHistory>();
            var rigidBody = waterDrop.GetComponent<Rigidbody>();
            var positionHistory = stateHistory.GetHistory<Vector3>(StateHistory.HistoryParameterType.Position);
            var velocityHistory = stateHistory.GetHistory<Vector3>(StateHistory.HistoryParameterType.Velocity);

            if (velocityHistory.Count() == stepsToPredict)
            {
                var lastPosReduced = (positionHistory.Dequeue() - startPos) / positionReduceFactor;
                var earlierVelocity = velocityHistory.Dequeue();
                var reducedEarlierVelocity = earlierVelocity / velocityReduceFactor;
                //training_x.Add(reducedEarlierVelocity.x);
                //training_x.Add(reducedEarlierVelocity.y);
                //training_x.Add(reducedEarlierVelocity.z);
                training_x.Add(lastPosReduced.x);
                training_x.Add(lastPosReduced.y);
                training_x.Add(lastPosReduced.z);

                foreach (var laterVelocity in velocityHistory)
                {
                    training_y.Add(laterVelocity.x - earlierVelocity.x);
                    training_y.Add(laterVelocity.y - earlierVelocity.y);
                    training_y.Add(laterVelocity.z - earlierVelocity.z);
                    earlierVelocity = laterVelocity;
                }
                var velocityNow = rigidBody.velocity;
                training_y.Add(velocityNow.x - earlierVelocity.x);
                training_y.Add(velocityNow.y - earlierVelocity.y);
                training_y.Add(velocityNow.z - earlierVelocity.z);
            }
        }

        //VerticeListToShow.Clear();
        foreach (var plane in planes)
        {
            var verticeList = plane.GetComponent<MeshFilter>().sharedMesh.vertices;
            List<int> verticeIndexes = new List<int> { 0, 10, 110, 120 };
            foreach (var verticeIndex in verticeIndexes)
            {
                var corner = (plane.transform.TransformPoint(verticeList[verticeIndex]) - startPos) / positionReduceFactor;
                training_x.Add(corner.x);
                training_x.Add(corner.y);
                training_x.Add(corner.z);
            }

            //VerticeListToShow.Add(plane.transform.TransformPoint(verticeList[0]));
            //VerticeListToShow.Add(plane.transform.TransformPoint(verticeList[10]));
            //VerticeListToShow.Add(plane.transform.TransformPoint(verticeList[110]));
            //VerticeListToShow.Add(plane.transform.TransformPoint(verticeList[120]));  
        }

        bool badTraining = false;
        foreach (var trainingData in training_y)
        {
            if (trainingData > 10 || trainingData < -10)
            {
                badTraining = true;
                Debug.Log("Bad training data: " + trainingData);
            }
        }

        if (!badTraining)
        {
            if (Math.Abs(training_y[1]) < 0.01)
            {
                Debug.LogError("Bad training data which is being sent: " + training_y[1]);
            }
            var request = TelegramFactory.CreateAddTrainingDataRequest(training_x.ToArray(), training_y.ToArray());
            externalCommunication.SendAsynch(request);
        }
    }

    List<Vector3> VerticeListToShow = new List<Vector3>();
    void OnDrawGizmos()
    {
        foreach (var vertice in VerticeListToShow)
        {
            Gizmos.DrawSphere(vertice, 1);
        }
    }

    void FixedUpdate()
    {

    }
}
