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
from tensorflow.keras import Input, Model
import os
import numpy as np

class Model_Training:
    model = None

    def _Create_Model(self, inputs, outputs):
        input_layer = Input(shape=(inputs), name='input')
        model = Sequential()
        model.add(input_layer)
        model.add(Dense(5))
        #model.add(Activation("relu"))
        #model.add(Dense(60))
        #model.add(Activation("relu"))
        #model.add(Dense(60))
        #model.add(Activation("relu"))
        #model.add(Dense(60))
        #model.add(Activation("relu"))
        #model.add(Dense(60))
        #model.add(Activation("relu"))
        #model.add(Dense(60))
        #model.add(Activation("relu"))
        #model.add(Dense(60))
        #model.add(Activation("relu"))
        #model.add(Dense(60))
        #model.add(Activation("relu"))                                        
        #model.add(Dense(60))
        #model.add(Activation("relu"))
        model.add(Dense(outputs))
        #model.add(Activation("tanh"))
        return model

    def _Compile_Model(self, model):
        optimizer = Adam(lr=0.0001000)
        model.compile(loss='mean_squared_error', optimizer=optimizer,metrics=['accuracy'])
        print(model.summary())
        return model

    def Train_Epochs(self, train_x, train_y, epochs = 1000):
        self.model.fit(train_x, train_y, epochs=epochs, batch_size=32)

    def Predict(self, predict_x):
        prediction_y = self.model.predict(predict_x)
        return prediction_y

    def __init__(self, inputs, outputs):
        self.model = self._Create_Model(inputs, outputs)
        self._Compile_Model(self.model)
