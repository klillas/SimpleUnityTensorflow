using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExperimentReset : MonoBehaviour
{
    public int ResetTimeMS = 6000;
    public float MaxStartDiff = (float)1.5;
    public float MaxStartVelocity = (float)1.5;

    Vector3 startPosition;
    DateTime lastReset;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        lastReset = DateTime.Now;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (DateTime.Now - lastReset >= TimeSpan.FromMilliseconds(ResetTimeMS))
        {
            lastReset = DateTime.Now;
            transform.position = startPosition + new Vector3(
                Random.Range(-MaxStartDiff, MaxStartDiff),
                Random.Range(-MaxStartDiff, MaxStartDiff),
                Random.Range(-MaxStartDiff, MaxStartDiff));

            var newVelocity = new Vector3(
                    Random.Range(-MaxStartVelocity, MaxStartVelocity),
                    Random.Range(-MaxStartVelocity, MaxStartVelocity),
                    Random.Range(-MaxStartVelocity, MaxStartVelocity));

            var rigidBody = GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                GetComponent<Rigidbody>().velocity = newVelocity;
            }

            var tensorflowPredictor = GetComponent<TensorflowPredictor>();
            if (tensorflowPredictor != null)
            {
                tensorflowPredictor.SetVelocity(newVelocity);
            }

            GetComponent<TensorflowTrainer>().SkipNextStep();
        }
    }
}
