using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegrams;
using UnityEngine;

public class TensorflowOrchestrator : MonoBehaviour
{
    public GameObject TrainPrefab;
    public GameObject PredictPrefab;
    public int TrainAmount = 1;
    public int PredictAmount = 1;
    public int CollectTraindataTime = 0;
    public int TrainModelTime = 0;
    public bool Train = false;
    public bool Predict = true;

    private List<GameObject> trainGameObjects;
    private List<GameObject> predictGameObjects;
    private ExternalCommunication externalCommunication;

    // Start is called before the first frame update
    void Start()
    {
        externalCommunication = ExternalCommunication.GetSingleton();
        trainGameObjects = new List<GameObject>();
        predictGameObjects = new List<GameObject>();
        if (Train)
        {
            for (int i = 0; i < TrainAmount; i++)
            {
                var trainObject = (Instantiate(TrainPrefab, transform.position, Quaternion.identity));
                trainObject.GetComponent<TensorflowTrainer>().StartTensorflowTrainer();
                trainGameObjects.Add(trainObject);
            }

            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(CollectTraindataTime * 1000);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    foreach (var trainGameObject in trainGameObjects)
                    {
                        Destroy(trainGameObject);
                    }
                    var beginTrainingRequest = TelegramFactory.CreateBeginTrainingRequest();
                    externalCommunication.SendAsynch(beginTrainingRequest, TrainEpoch);
                });
            })).Start();
        }

        if (Predict)
        {
            var predictObject = (Instantiate(PredictPrefab, transform.position, Quaternion.identity));
            predictObject.GetComponent<TensorflowPredictor>().StartTensorflowPredictor();
            predictGameObjects.Add(predictObject);
        }
    }

    void TrainEpoch(Request requestAnswer)
    {
        var beginTrainingRequest = TelegramFactory.CreateBeginTrainingRequest();
        externalCommunication.SendAsynch(beginTrainingRequest, TrainEpoch);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
