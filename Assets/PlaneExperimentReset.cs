using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlaneExperimentReset : MonoBehaviour
{
    public int ResetTimeMS = 5000;
    public float MaxStartDiff = (float)1.5;
    public float MaxStartAngle = 5;

    Vector3 startPosition;
    DateTime lastReset;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        lastReset = DateTime.Now;
        Reset();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (DateTime.Now - lastReset >= TimeSpan.FromMilliseconds(ResetTimeMS))
        {
            lastReset = DateTime.Now;
            Reset();
        }
    }

    private void Reset()
    {
        transform.position = startPosition + new Vector3(
            Random.Range(-MaxStartDiff, MaxStartDiff),
            Random.Range(-MaxStartDiff, MaxStartDiff),
            Random.Range(-MaxStartDiff, MaxStartDiff));

        transform.rotation = Quaternion.Euler(
            Random.Range(-MaxStartAngle, MaxStartAngle),
            Random.Range(-MaxStartAngle, MaxStartAngle),
            Random.Range(-MaxStartAngle, MaxStartAngle));
    }
}
