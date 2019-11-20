using UnityEngine;

namespace Assets
{
    class UpdateTrackerClient : MonoBehaviour
    {
        private void Awake()
        {
            UpdateTracker.SubscribeUpdateObject(gameObject);
        }

        private void FixedUpdate()
        {
            UpdateTracker.AddUpdate(gameObject, UpdateTracker.UpdateType.FixedUpdate);
        }
    }
}
