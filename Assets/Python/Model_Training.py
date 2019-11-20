import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.optimizers import Adam, SGD
from tensorflow.keras.utils import plot_model
from tensorflow.keras.models import Model
from tensorflow.keras.applications import MobileNet
from tensorflow.keras import optimizers
from tensorflow.keras.callbacks import ModelCheckpoint
from tensorflow.keras.layers import Dense, Flatten, Convolution2D, GlobalAveragePooling2D, Input, AveragePooling2D, Activation, Dropout, MaxPooling2D
from tensorflow.keras.callbacks import TensorBoard
from tensorflow.keras.models import load_model
from tensorflow.keras.layers import concatenate, Lambda
from tensorflow.keras.layers import BatchNormalization
from tensorflow.python.keras.layers.advanced_activations import LeakyReLU
from tensorflow.keras import Input, Model, callbacks
import os
import numpy as np
import datetime

class Model_Training:
    model = None
    model_path = None
    save_model_after_epoch = None
    tensorboard_callback = None
    tensorboard_log_dir = None

    def _Create_Model(self, inputs, outputs):
        input_layer = Input(shape=(inputs), name='input')
        model = Sequential()
        model.add(input_layer)
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))
        model.add(Dense(300))
        model.add(BatchNormalization())
        model.add(LeakyReLU(alpha=0.1))                                        
        model.add(Dense(outputs))
        #model.add(Activation("LeakyReLU"))
        return model

    def _Compile_Model(self, model):
        optimizer = Adam(lr=0.0010000)
        #optimizer = SGD(learning_rate=0.0003, momentum=0.5, nesterov=False)
        model.compile(loss='mean_squared_error', optimizer=optimizer,metrics=['accuracy'])
        print(model.summary())
        return model

    def _Setup_Tensorboard(self):
        # Tensorboard logging
        if not os.path.exists(self.tensorboard_log_dir):
            os.makedirs(self.tensorboard_log_dir)        
        log_dir=self.tensorboard_log_dir + datetime.datetime.now().strftime("%Y%m%d-%H%M%S")
        file_writer = tf.summary.create_file_writer(log_dir + "\\metrics")
        file_writer.set_as_default()
        self.tensorboard_callback = TensorBoard(log_dir=log_dir, profile_batch = 3, update_freq=30000000)        

    def Train_Epochs(self, train_x, train_y, epochs = 10000):
        reduce_lr = callbacks.ReduceLROnPlateau(
            monitor='loss',
            factor=0.2,
            patience=5,
            min_lr=0.001,
            verbose=1)

        #self.model.fit(train_x, train_y, epochs=epochs, batch_size=10000, callbacks=[self.tensorboard_callback])
        self.model.fit(train_x, train_y, epochs=epochs, batch_size=10000)
        if (self.save_model_after_epoch):
            self.model.save_weights(self.model_path)

    def Predict(self, predict_x):
        prediction_y = self.model.predict(predict_x)
        return prediction_y

    def __init__(
        self,
        inputs,
        outputs,
        model_path,
        tensorboard_log_dir,
        load_existing_model = True,
        save_model_after_epoch = True):

        self.tensorboard_log_dir = tensorboard_log_dir
        self.save_model_after_epoch = save_model_after_epoch
        self.model_path = model_path
        self.model = self._Create_Model(inputs, outputs)
        self._Setup_Tensorboard()

        if not os.path.exists(self.model_path):
            os.makedirs(self.model_path)
        if load_existing_model:
            self.model.load_weights(self.model_path)
        self.model.save(self.model_path)
        self._Compile_Model(self.model)
