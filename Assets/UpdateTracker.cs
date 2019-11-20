using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpdateTracker
{
    public delegate void NotifyEventCallback(UpdateEventType updateEventType);

    public enum UpdateType
    {
        FixedUpdate
    };

    public enum UpdateEventType
    {
        AllFixedUpdateSet
    }

    static Dictionary<GameObject, UpdateObjectInformation> subscribedUpdateObjects = new Dictionary<GameObject, UpdateObjectInformation>();
    static List<UpdateEventSubscription> updateEventSubscriptions = new List<UpdateEventSubscription>();

    public static void SubscribeUpdateObject(GameObject gameObject)
    {
        subscribedUpdateObjects.Add(gameObject, new UpdateObjectInformation());
    }

    public static void SubscribeNotifyEvent(UpdateEventType updateEventType, NotifyEventCallback callback)
    {
        updateEventSubscriptions.Add(new UpdateEventSubscription(callback, updateEventType));
    }

    public static void AddUpdate(GameObject gameobject, UpdateType updateType)
    {
        switch (updateType)
        {
            case UpdateType.FixedUpdate:
                subscribedUpdateObjects[gameobject].fixedUpdate = true;
                CheckUpdateAllFixedUpdateSetCallbacks();
                break;
            default:
                throw new System.NotSupportedException("UpdateTracker does not support " + updateType.ToString());
        }
    }

    public static void ResetUpdates(GameObject gameobject)
    {
        subscribedUpdateObjects[gameobject].fixedUpdate = false;
    }

    private static void CheckUpdateAllFixedUpdateSetCallbacks()
    {
        foreach (var updateObjectInformation in subscribedUpdateObjects.Values)
        {
            if (!updateObjectInformation.fixedUpdate)
            {
                return;
            }
        }

        foreach (var updateEventSubscription in updateEventSubscriptions.Where(x => x.updateEventType == UpdateEventType.AllFixedUpdateSet))
        {
            updateEventSubscription.callback(UpdateEventType.AllFixedUpdateSet);
        }

        foreach (var updateObjectInformation in subscribedUpdateObjects.Values)
        {
            updateObjectInformation.fixedUpdate = false;
        }
    }

    private class UpdateObjectInformation
    {
        public bool fixedUpdate = false;
    }

    private class UpdateEventSubscription
    {
        public NotifyEventCallback callback;
        public UpdateEventType updateEventType;

        public UpdateEventSubscription(NotifyEventCallback callback, UpdateEventType updateEventType)
        {
            this.callback = callback;
            this.updateEventType = updateEventType;
        }
    }
}