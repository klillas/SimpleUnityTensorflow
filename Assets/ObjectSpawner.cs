using Assets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    private delegate void MainThreadCallback(object varOne, object varTwo);

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
    DateTime startTrainTime;
    ExternalCommunication externalCommunication;
    System.Random rand = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        spawnedObjects = new List<GameObject>();
        spawnedPredictionObjects = new List<GameObject>();
        previousResetTime = DateTime.Now;
        startTrainTime = DateTime.Now;
        xNow = xStart;
        zNow = zStart;
        externalCommunication = ExternalCommunication.GetSingleton();

        while (spawnedObjects.Count < 200)
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

        while (spawnedPredictionObjects.Count < 1)
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

        Task.Delay(300000).ContinueWith(t => BeginTraining());
    }

    void BeginTraining()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(delegate()
        {
            foreach (var spawnedObject in spawnedObjects)
            {
                spawnedObject.GetComponent<TensorflowTrainer>().Paused = true;
                Destroy(spawnedObject);
            }
            spawnedObjects.Clear();

            foreach (var predictionObject in spawnedPredictionObjects)
            {
                predictionObject.GetComponent<TensorflowTrainer>().Paused = true;
            }

            var request = TelegramFactory.CreateBeginTrainingRequest();
            externalCommunication.SendAsynch(request, TrainingFinishedBeginPrediction);
        });
    }

    void TrainingFinishedBeginPrediction(Telegrams.Request requestAnswer)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(delegate ()
        {
            if (requestAnswer.Command != Telegrams.Request.Types.Command.TrainingFinished)
            {
                throw new ArgumentException("Expected Training Finished request from tensorflow");
            }

            foreach (var predictionObject in spawnedPredictionObjects)
            {
                predictionObject.GetComponent<TensorflowTrainer>().Paused = false;
            }

            Task.Delay(20000).ContinueWith(t => BeginTraining());
            //Task.Delay(5000).ContinueWith(t => BeginTrainingDataCollection());
        });
    }

    void BeginTrainingDataCollection()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(delegate ()
        {
            foreach (var spawnedObject in spawnedObjects)
            {
                spawnedObject.GetComponent<TensorflowTrainer>().Paused = false;
            }

            foreach (var predictionObject in spawnedPredictionObjects)
            {
                predictionObject.GetComponent<TensorflowTrainer>().Paused = true;
            }

            Task.Delay(5000).ContinueWith(t => BeginTraining());
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (DateTime.Now - previousResetTime > TimeSpan.FromSeconds(5))
        {
            previousResetTime = DateTime.Now;
            
            foreach (var spawnedObject in spawnedObjects)
            {
                var position = new Vector3(
                    rand.Next(-1000, 1000),
                    yStart,
                    rand.Next(-1000, 1000));

                spawnedObject.GetComponent<TensorflowTrainer>().ResetTraining(position);
            }

            foreach (var predictionObject in spawnedPredictionObjects)
            {
                float xPos = 0 + (spawnedPredictionObjects.Count * xDelta);
                float yPos = 2;
                float zPos = -30;
                var position = new Vector3(
                    xPos,
                    yPos,
                    zPos);

                predictionObject.GetComponent<TensorflowTrainer>().ResetTraining(position);
            }
        }
    }
}
