using Assets;
using System;
using Telegrams;
using UnityEngine;

public class TensorflowPredictor : MonoBehaviour
{
    DateTime lastPhysicsUpdate;
    Vector3 currentVelocity;
    bool fixedUpdateRun = false;
    ExternalCommunication externalCommunication;
    bool isPredicting;

    // Start is called before the first frame update
    public TensorflowPredictor()
    {
        isPredicting = false;
        lastPhysicsUpdate = DateTime.Now;
        externalCommunication = ExternalCommunication.GetSingleton();
        currentVelocity = new Vector3(0, 0, 0);
    }

    public void StartTensorflowPredictor()
    {
        isPredicting = true;
        CreatePredictRequest();
    }

    public void StopTensorflowPredictor()
    {
        isPredicting = false;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        currentVelocity = newVelocity;
    }

    private void CreatePredictRequest()
    {
        if (!isPredicting)
        {
            return;
        }

        float[] predict_x = {
                transform.position.x,
                transform.position.y,
                transform.position.z,
                currentVelocity.x,
                currentVelocity.y,
                currentVelocity.z
            };
        var request = TelegramFactory.CreatePredictRequest(predict_x);
        externalCommunication.SendAsynch(request, CreatePredictRequestAnswer);
    }

    private void CreatePredictRequestAnswer(Request requestAnswer)
    {
        if (DateTime.Now - lastPhysicsUpdate < TimeSpan.FromSeconds(Time.fixedDeltaTime))
        {
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(Time.fixedDeltaTime) - (DateTime.Now - lastPhysicsUpdate));
        }
        else
        {
            Debug.Log("Time since last physics update: " + (DateTime.Now - lastPhysicsUpdate).TotalSeconds);
        }

        lastPhysicsUpdate = DateTime.Now;

        var prediction_y = requestAnswer.PredictionY;
        var newPos = transform.position;
        newPos.x += prediction_y[0];
        newPos.y += prediction_y[1];
        newPos.z += prediction_y[2];
        transform.position = newPos;

        currentVelocity.x += prediction_y[3];
        currentVelocity.y += prediction_y[4];
        currentVelocity.z += prediction_y[5];

        CreatePredictRequest();
    }
}
