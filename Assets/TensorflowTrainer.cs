using Assets;
using UnityEngine;

public class TensorflowTrainer : MonoBehaviour
{
    ExternalCommunication externalCommunication;
    Rigidbody rigidBody;
    Vector3 lastPos;
    Vector3 lastVelocity;
    GameObject[] avoidableObjects;
    bool isTraining;

    // Start is called before the first frame update
    void Start()
    {
        isTraining = false;
        externalCommunication = ExternalCommunication.GetSingleton();
        rigidBody = GetComponent<Rigidbody>();

        lastPos = transform.position;
        lastVelocity = new Vector3(0, 0, 0);
        avoidableObjects = GameObject.FindGameObjectsWithTag("AvoidableObject");

        while (true)
        {
            var request = TelegramFactory.CreateBeginTrainingRequest();
            externalCommunication.SendAsynch(request);
            System.Threading.Thread.Sleep(100);
        }
    }

    public void StartTensorflowTrainer()
    {
        isTraining = true;
    }

    public void StopTensorflowTrainer()
    {
        isTraining = false;
    }

    public void SkipNextStep()
    {
        lastVelocity = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {
        if (!isTraining)
        {
            return;
        }

        float[] training_x = {
                lastPos.x,
                lastPos.y,
                lastPos.z,
                lastVelocity.x,
                lastVelocity.y,
                lastVelocity.z
            };

        float[] training_y = {
                transform.position.x - lastPos.x,
                transform.position.y - lastPos.y,
                transform.position.z - lastPos.z,
                rigidBody.velocity.x - lastVelocity.x,
                rigidBody.velocity.y - lastVelocity.y,
                rigidBody.velocity.z - lastVelocity.z
            };

        if (lastVelocity != new Vector3(0, 0, 0))
        {
            var request = TelegramFactory.CreateAddTrainingDataRequest(training_x, training_y);
            externalCommunication.SendAsynch(request);
        }

        lastPos = transform.position;
        lastVelocity = rigidBody.velocity;
    }
}
