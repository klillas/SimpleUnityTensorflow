using Assets;
using System.Collections.Generic;
using UnityEngine;

public class TensorflowTrainer : MonoBehaviour
{
    ExternalCommunication externalCommunication;
    Rigidbody rigidBody;
    Vector3 lastPos;
    int stepsToPredict = 5;
    Queue<Vector3> lastVelocity;
    GameObject[] avoidableObjects;
    bool isTraining = false;

    // Start is called before the first frame update
    void Start()
    {
        externalCommunication = ExternalCommunication.GetSingleton();
        rigidBody = GetComponent<Rigidbody>();

        lastPos = transform.position;
        lastVelocity = new Queue<Vector3>(stepsToPredict);
        avoidableObjects = GameObject.FindGameObjectsWithTag("AvoidableObject");
    }

    public void StartTensorflowTrainer()
    {
        isTraining = true;
    }

    public void StopTensorflowTrainer()
    {
        isTraining = false;
    }

    public void ResetTraining()
    {
        lastVelocity.Clear();
    }

    void FixedUpdate()
    {
        if (!isTraining)
        {
            return;
        }

        if (lastVelocity.Count == stepsToPredict)
        {
            var inputVelocity = lastVelocity.Dequeue();

            float[] training_x = {
                lastPos.x,
                lastPos.y,
                lastPos.z,
                inputVelocity.x,
                inputVelocity.y,
                inputVelocity.z
            };

            float[] training_y = {
                transform.position.x - lastPos.x,
                transform.position.y - lastPos.y,
                transform.position.z - lastPos.z,
                rigidBody.velocity.x - inputVelocity.x,
                rigidBody.velocity.y - inputVelocity.y,
                rigidBody.velocity.z - inputVelocity.z
            };

            var request = TelegramFactory.CreateAddTrainingDataRequest(training_x, training_y);
            externalCommunication.SendAsynch(request);
        }

        lastPos = transform.position;
        lastVelocity.Enqueue(rigidBody.velocity);
    }
}
