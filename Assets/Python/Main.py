import Unity_Controller
import Telegram_Factory
import Model_Training
import Training_Database
import protobuf.Telegrams_pb2 as Telegrams
import numpy as np
import time;

class Main:
    unity_controller = None
    telegram_factory = None
    model_training = None
    training_database = None

    def command_handler(self):
        incoming_telegram_queue = self.unity_controller.get_incoming_queue()
        while (True):
            (telegram, address) = incoming_telegram_queue.get(block=True)

            if (telegram.command == Telegrams.Request.ADD_TRAINING_DATA):
                # print("Request received: ADD TRAINING DATA")
                train_x = np.array(telegram.training_x)
                train_x = np.expand_dims(train_x, axis=0)

                train_y = np.array(telegram.training_y)
                train_y = np.expand_dims(train_y, axis=0)

                self.training_database.Add_Training(train_x, train_y)

            if (telegram.command == Telegrams.Request.BEGIN_TRAINING):
                print("Request received: BEGIN TRAINING")
                training_x, training_y = self.training_database.Get_Training_Data()
                while (True):
                    self.model_training.Train_Epochs(training_x, training_y, 1)

                request = self.telegram_factory.Create_Training_Finished_Request(telegram.transaction_id)
                self.unity_controller.send(request, address)

            if (telegram.command == Telegrams.Request.PREDICT):
                start_time = time.time()
                # print("Request received: PREDICT")
                predict_x = np.array(telegram.predict_x)
                predict_x = np.expand_dims(predict_x, axis=0)
                prediction_y = self.model_training.Predict(predict_x)[0]
                request = self.telegram_factory.Create_Predict_Request(prediction_y, telegram.transaction_id)
                self.unity_controller.send(request, address)

                elapsed_time = time.time() - start_time
                print('Time taken to handle request: ' + str(elapsed_time))

    def train_manually(self):
        training_x, training_y = self.training_database.Get_Training_Data(shuffle=True, ratio=1)
        while (True):
            self.model_training.Train_Epochs(training_x, training_y, 1)        

    def __init__(self):
        model_path = "g:/temp/tensorflow/models/physicsEmulationModel"
        tensorboard_log_dir = 'g:\\temp\\tensorboard\\'
        training_database_path = "g:/temp/database.dat"
        #model_inputs = 18
        model_inputs = 15
        model_outputs = 3
        load_existing_dataset = True
        load_existing_model = True

        ip = "127.0.0.1"
        port = 11000

        self.training_database = Training_Database.Training_Database(
            training_database_path,
            model_inputs,   
            model_outputs,
            1000000,
            load_database=load_existing_dataset)
        
        self.model_training = Model_Training.Model_Training(
            model_inputs,
            model_outputs,
            model_path,
            tensorboard_log_dir=tensorboard_log_dir,
            load_existing_model=load_existing_model)

        self.telegram_factory = Telegram_Factory.Telegram_Factory()

        self.unity_controller = Unity_Controller.Unity_Controller(ip, port)

        self.command_handler()
        self.training_database.Print_Random()
        self.train_manually()

main = Main()