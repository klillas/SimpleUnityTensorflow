using System.Collections;
using System.Collections.Generic;
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

    private List<GameObject> predictGameObjects;

    // Start is called before the first frame update
    void Start()
    {
        predictGameObjects = new List<GameObject>();
        if (Train)
        {
            // TODO: Initialize training
        }

        if (Predict)
        {
            var predictObject = (Instantiate(PredictPrefab, transform.position, Quaternion.identity));
            predictObject.GetComponent<TensorflowPredictor>().StartTensorflowPredictor();
            predictGameObjects.Add(predictObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
