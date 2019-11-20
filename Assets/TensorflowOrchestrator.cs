using Assets;
using System.Collections.Generic;
using System.Threading;
using Telegrams;
using UnityEngine;
using Random = UnityEngine.Random;

public class TensorflowOrchestrator : MonoBehaviour
{
    public GameObject TrainPrefab;
    public GameObject PredictPrefab;
    public string TrainScriptName;
    public string PredictScriptName;
    public int TrainAmount = 1;
    public int PredictAmount = 1;
    public int CollectTraindataTime = 0;
    public int TrainModelTime = 0;
    public bool Train = false;
    public bool Predict = true;
    public float MaxStartDiff = (float)1.5;
    public float MaxStartVelocity = (float)1.5;
    public int StepsToRemember = 1;

    private List<GameObject> trainGameObjects;
    private List<GameObject> predictGameObjects;
    private ExternalCommunication externalCommunication;

    void Start()
    {
        externalCommunication = ExternalCommunication.GetSingleton();
        trainGameObjects = new List<GameObject>();
        predictGameObjects = new List<GameObject>();
        if (Train)
        {
            System.Type mType = System.Type.GetType(TrainScriptName);
            gameObject.AddComponent(mType);

            for (int i = 0; i < TrainAmount; i++)
            {
                transform.position = transform.position + new Vector3(
                    Random.Range(-MaxStartDiff, MaxStartDiff),
                    Random.Range(-MaxStartDiff, MaxStartDiff),
                    Random.Range(-MaxStartDiff, MaxStartDiff));

                var newVelocity = new Vector3(
                        Random.Range(-MaxStartVelocity, MaxStartVelocity),
                        Random.Range(-MaxStartVelocity, MaxStartVelocity),
                        Random.Range(-MaxStartVelocity, MaxStartVelocity));

                var trainObject = (Instantiate(TrainPrefab, transform.position, Quaternion.identity));
                trainObject.GetComponent<ExperimentReset>().MaxStartDiff = MaxStartDiff;
                trainObject.GetComponent<ExperimentReset>().MaxStartVelocity = MaxStartVelocity;
                var stateHistory = trainObject.GetComponent<StateHistory>();
                stateHistory.HistoryParameterTypes.Add(StateHistory.HistoryParameterType.Position);
                stateHistory.HistoryParameterTypes.Add(StateHistory.HistoryParameterType.Velocity);
                stateHistory.StepsToRemember = StepsToRemember;
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
            System.Type mType = System.Type.GetType(PredictScriptName);
            gameObject.AddComponent(mType);

            for (int i = 0; i < PredictAmount; i++)
            {
                var predictObject = (Instantiate(PredictPrefab, transform.position, Quaternion.identity));
                predictGameObjects.Add(predictObject);
            }
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
