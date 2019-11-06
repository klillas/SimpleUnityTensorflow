import Unity_Controller
import Telegram_Factory
import Model_Training
import protobuf.Telegrams_pb2 as Telegrams
import numpy as np

class Main:
    unity_controller = None
    telegram_factory = None
    model_training = None

    def message_received(self, telegram, address):
        if (telegram.command == Telegrams.Request.ADD_TRAINING_DATA):
            train_x = np.array(telegram.training_x)
            train_x = np.expand_dims(train_x, axis=0)

            train_y = np.array(telegram.training_y)
            train_y = np.expand_dims(train_y, axis=0)            

            self.model_training.Add_Training_Data(train_x, train_y)

        if (telegram.command == Telegrams.Request.PREDICT):
            predict_x = np.array(telegram.predict_x)
            predict_x = np.expand_dims(predict_x, axis=0)
            prediction_y = self.model_training.Predict(predict_x)[0]
            request = self.telegram_factory.Create_Predict_Request(prediction_y, telegram.transaction_id)
            self.unity_controller.send(request, address)

    def __init__(self):
        model_inputs = 6
        model_outputs = 6

        ip = "127.0.0.1"
        port = 11000

        self.model_training = Model_Training.Model_Training(model_inputs, model_outputs, 10000)
        self.telegram_factory = Telegram_Factory.Telegram_Factory()
        self.unity_controller = Unity_Controller.Unity_Controller(ip, port, self.message_received)


main = Main()