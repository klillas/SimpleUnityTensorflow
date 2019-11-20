using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHistory : MonoBehaviour
{
    public enum HistoryParameterType
    {
        Velocity,
        Position
    };

    public List<HistoryParameterType> HistoryParameterTypes;
    public int StepsToRemember = 1;

    private Dictionary<HistoryParameterType, ParameterCache> historyDictionary;

    StateHistory()
    {
        HistoryParameterTypes = new List<HistoryParameterType>();
        historyDictionary = new Dictionary<HistoryParameterType, ParameterCache>();
    }

    void Start()
    {
        foreach (var historyParameterType in HistoryParameterTypes)
        {
            switch(historyParameterType)
            {
                case HistoryParameterType.Position:
                case HistoryParameterType.Velocity:
                    historyDictionary.Add(historyParameterType, new ParameterCache<Vector3>(StepsToRemember));
                    break;
                default:
                    throw new System.NotSupportedException(historyParameterType.ToString() + " not supported yet");
            }
        }
    }

    /// <summary>
    /// Returns the history queue. Modifying the queue will modify statehistory.
    /// TODO: Change interface to enable modifications only from inside StateHistory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameterType"></param>
    /// <returns></returns>
    public Queue<T> GetHistory<T>(HistoryParameterType parameterType)
    {
        return (historyDictionary[parameterType] as ParameterCache<T>).Parameters;
    }

    void FixedUpdate()
    {
        foreach (var historyItem in historyDictionary)
        {
            switch(historyItem.Key)
            {
                case HistoryParameterType.Position:
                    UpdatePositionHistory();
                    break;
                case HistoryParameterType.Velocity:
                    UpdateVelocityHistory();
                    break;
                default:
                    throw new System.NotSupportedException(historyItem.Key.ToString() + " not supported yet");
            }
        }
    }

    private void UpdatePositionHistory()
    {
        var cache = historyDictionary[HistoryParameterType.Position] as ParameterCache<Vector3>;
        cache.Parameters.Enqueue(transform.position);
    }

    private void UpdateVelocityHistory()
    {
        var cache = historyDictionary[HistoryParameterType.Velocity] as ParameterCache<Vector3>;
        cache.Parameters.Enqueue(GetComponent<Rigidbody>().velocity);
    }

    private class ParameterCache<T> : ParameterCache
    {
        public Queue<T> Parameters;

        public ParameterCache(int stepsToRemember)
        {
            Parameters = new Queue<T>(stepsToRemember);
        }

    }

    private class ParameterCache
    {
        
    }
}