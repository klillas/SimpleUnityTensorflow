using Assets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TensorflowTrainer : MonoBehaviour, ITensorflowTrainer
{
    ExternalCommunication externalCommunication;
    int stepsToPredict = 1;
    List<GameObject> waterDrops;

    // Start is called before the first frame update
    void Start()
    {
        UpdateTracker.SubscribeNotifyEvent(UpdateTracker.UpdateEventType.AllFixedUpdateSet, NotifyAllFixedUpdateSet);
        externalCommunication = ExternalCommunication.GetSingleton();
        waterDrops = GameObject.FindGameObjectsWithTag("WaterDrop").ToList();
    }

    static float largestPosition = float.MinValue;
    void NotifyAllFixedUpdateSet(UpdateTracker.UpdateEventType updateEventType)
    {
        float positionReduceFactor = 1000;
        var training_x = new List<float>();
        var training_y = new List<float>();

        // Find the group middle position
        Vector3 startPos = new Vector3();
        foreach (var waterDrop in waterDrops)
        {
            startPos += waterDrop.transform.position;
        }
        startPos /= waterDrops.Count();

        // Resize to position the first positionReduceFactor^3 within 0-1
        startPos /= positionReduceFactor;

        if (startPos.magnitude > largestPosition)
        {
            largestPosition = startPos.magnitude;
            Debug.Log("New max position: " + largestPosition);
        }

        foreach (var waterDrop in waterDrops)
        {
            var stateHistory = waterDrop.GetComponent<StateHistory>();
            var rigidBody = waterDrop.GetComponent<Rigidbody>();
            var positionHistory = stateHistory.GetHistory<Vector3>(StateHistory.HistoryParameterType.Position);
            var velocityHistory = stateHistory.GetHistory<Vector3>(StateHistory.HistoryParameterType.Velocity);

            if (velocityHistory.Count() == stepsToPredict)
            {
                var lastPos = (positionHistory.Dequeue() - startPos) / positionReduceFactor;
                var earlierVelocity = velocityHistory.Dequeue();
                training_x.Add(earlierVelocity.x);
                training_x.Add(earlierVelocity.y);
                training_x.Add(earlierVelocity.z);
                training_x.Add(lastPos.x);
                training_x.Add(lastPos.y);
                training_x.Add(lastPos.z);

                foreach (var laterVelocity in velocityHistory)
                {
                    training_y.Add(laterVelocity.x - earlierVelocity.x);
                    training_y.Add(laterVelocity.y - earlierVelocity.y);
                    training_y.Add(laterVelocity.z - earlierVelocity.z);
                    earlierVelocity = laterVelocity;
                }
                training_y.Add(rigidBody.velocity.x - earlierVelocity.x);
                training_y.Add(rigidBody.velocity.y - earlierVelocity.y);
                training_y.Add(rigidBody.velocity.z - earlierVelocity.z);
            }
        }

        var request = TelegramFactory.CreateAddTrainingDataRequest(training_x.ToArray(), training_y.ToArray());
        externalCommunication.SendAsynch(request);
    }

    void FixedUpdate()
    {

    }
}
