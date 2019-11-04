using Google.Protobuf;
using UnityEngine;

[RequireComponent(typeof(ExternalCommunication))]
[RequireComponent(typeof(Rigidbody))]
public class TensorflowTrainer : MonoBehaviour
{
    ExternalCommunication externalCommunication;
    Rigidbody rigidBody;
    Vector3 lastPos;
    Vector3 lastVelocity;

    // Start is called before the first frame update
    void Start()
    {
        externalCommunication = GetComponent<ExternalCommunication>();
        rigidBody = GetComponent<Rigidbody>();

        lastPos = transform.position;
        lastVelocity = rigidBody.velocity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float[] training_x =
        {
            lastPos.x,
            lastPos.y,
            lastPos.z,
            lastVelocity.x,
            lastVelocity.y,
            lastVelocity.z
        };

        float[] training_y =
        {
            transform.position.x,
            transform.position.y,
            transform.position.z,
            rigidBody.velocity.x,
            rigidBody.velocity.y,
            rigidBody.velocity.z
        };

        lastPos = transform.position;
        lastVelocity = rigidBody.velocity;

        var request = new Telegrams.Request();
        request.Command = Telegrams.Request.Types.Command.AddTrainingData;
        request.Message = "Training data";
        request.TrainingX.AddRange(training_x);
        request.TrainingY.AddRange(training_y);

        var outputByteArray = new byte[request.CalculateSize()];
        request.WriteTo(new CodedOutputStream(outputByteArray));

        externalCommunication.SendAsynch(outputByteArray);
    }
}
