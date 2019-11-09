using Assets;
using UnityEngine;

public class TensorflowTrainer : MonoBehaviour
{
    public bool Predict = false;
    public bool Paused = false;
    ExternalCommunication externalCommunication;
    Rigidbody rigidBody;
    Vector3 lastPos;
    Vector3 lastVelocity;
    bool waitingForCallback = false;

    // Start is called before the first frame update
    void Start()
    {
        externalCommunication = ExternalCommunication.GetSingleton();
        rigidBody = GetComponent<Rigidbody>();

        lastPos = transform.position;
        lastVelocity = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetTraining(Vector3 position)
    {
        lastPos = position;
        transform.position = position;
        lastVelocity = new Vector3(0, -0.19f, 0);
        if (!Predict)
        {
            rigidBody.velocity = lastVelocity;
        }
    }

    private void RequestAnswerCallback(Telegrams.Request requestAnswer)
    {
        var prediction_y = requestAnswer.PredictionY;
        lastPos.x += prediction_y[0];
        lastPos.y += prediction_y[1];
        lastPos.z += prediction_y[2];

        lastVelocity.x += prediction_y[3];
        lastVelocity.y += prediction_y[4];
        lastVelocity.z += prediction_y[5];

        waitingForCallback = false;
    }

    private void FixedUpdate()
    {
        if (Paused)
        {
            waitingForCallback = false;
            return;
        }

        if (Predict)
        {
            if (waitingForCallback == false)
            {
                float[] predict_x = {
                    lastPos.x,
                    lastPos.y,
                    lastPos.z,
                    lastVelocity.x,
                    lastVelocity.y,
                    lastVelocity.z
                };

                var request = TelegramFactory.CreatePredictRequest(predict_x);
                externalCommunication.SendAsynch(request, RequestAnswerCallback);
                waitingForCallback = true;
            }

            transform.position = new Vector3(
                                        lastPos.x,
                                        lastPos.y,
                                        lastPos.z);
        }
        else
        {
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

            // print(training_x[0] + " " + training_x[1] + " " + training_x[2] + " " + training_x[3] + " " + training_x[4] + " " + training_x[5] + " ");
            // print(training_y[0] + " " + training_y[1] + " " + training_y[2] + " " + training_y[3] + " " + training_y[4] + " " + training_y[5] + " ");

            if (lastVelocity != new Vector3(0, 0, 0))
            {
                var request = TelegramFactory.CreateAddTrainingDataRequest(training_x, training_y);
                externalCommunication.SendAsynch(request);
            }

            lastPos = transform.position;
            lastVelocity = rigidBody.velocity;
        }
    }
}
