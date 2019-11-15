import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.optimizers import Adam
from tensorflow.keras.utils import plot_model
from tensorflow.keras.models import Model
from tensorflow.keras.applications import MobileNet
from tensorflow.keras import optimizers
from tensorflow.keras.callbacks import ModelCheckpoint
from tensorflow.keras.layers import Dense, Flatten, Convolution2D, GlobalAveragePooling2D, Input, AveragePooling2D, Activation, Dropout, MaxPooling2D
from tensorflow.keras.callbacks import TensorBoard
from tensorflow.keras.models import load_model
from tensorflow.keras.layers import concatenate
from tensorflow.keras.layers import Lambda
from tensorflow.keras import Input, Model, callbacks
import os
import numpy as np

class Model_Training:
    model = None
    model_path = None
    save_model_after_epoch = None

    def _Create_Model(self, inputs, outputs):
        input_layer = Input(shape=(inputs), name='input')
        model = Sequential()
        model.add(input_layer)
        model.add(Dense(60))
        model.add(Activation("tanh"))
        model.add(Dense(60))
        model.add(Activation("tanh"))
        model.add(Dense(60))
        model.add(Activation("tanh"))
        model.add(Dense(60))
        model.add(Activation("tanh"))
        model.add(Dense(60))
        model.add(Activation("tanh"))
        model.add(Dense(60))
        model.add(Activation("tanh"))
        model.add(Dense(60))
        model.add(Activation("tanh"))
        model.add(Dense(60))
        model.add(Activation("tanh"))
        model.add(Dense(60))
        model.add(Activation("tanh"))
        model.add(Dense(60))
        model.add(Activation("tanh"))                                         
        model.add(Dense(outputs))
        #model.add(Activation("tanh"))
        return model

    def _Compile_Model(self, model):
        optimizer = Adam(lr=0.0010000)
        model.compile(loss='mean_squared_error', optimizer=optimizer,metrics=['accuracy'])
        print(model.summary())
        return model

    def Train_Epochs(self, train_x, train_y, epochs = 1000):
        reduce_lr = callbacks.ReduceLROnPlateau(
            monitor='loss',
            factor=0.2,
            patience=5,
            min_lr=0.001)

        self.model.fit(train_x, train_y, epochs=epochs, batch_size=32, callbacks=[reduce_lr])
        if (self.save_model_after_epoch):
            self.model.save_weights(self.model_path)

    def Predict(self, predict_x):
        prediction_y = self.model.predict(predict_x)
        return prediction_y

    def __init__(self, inputs, outputs, model_path, load_existing_model = True, save_model_after_epoch = True):
        self.save_model_after_epoch = save_model_after_epoch
        self.model_path = model_path
        self.model = self._Create_Model(inputs, outputs)

        if not os.path.exists(self.model_path):
            os.makedirs(self.model_path)
        if load_existing_model:
            self.model.load_weights(self.model_path)
        self.model.save(self.model_path)
        self._Compile_Model(self.model)
