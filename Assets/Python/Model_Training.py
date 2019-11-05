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
    batch_train_x = None
    batch_train_y = None
    batch_size = None

    def _Create_Model(self, inputs, outputs):
        input_layer = Input(shape=(inputs), name='input')
        model = Sequential()
        model.add(input_layer)
        model.add(Dense(60))
        model.add(Activation("relu"))
        model.add(Dense(60))
        model.add(Activation("relu"))
        model.add(Dense(60))
        model.add(Activation("relu"))
        model.add(Dense(60))
        model.add(Activation("relu"))
        model.add(Dense(60))
        model.add(Activation("relu"))
        model.add(Dense(60))
        model.add(Activation("relu"))
        model.add(Dense(60))
        model.add(Activation("relu"))
        model.add(Dense(60))
        model.add(Activation("relu"))                                        
        model.add(Dense(60))
        model.add(Activation("relu"))        
        model.add(Dense(outputs))
        # model.add(Activation('sigmoid'))
        return model

    def _Compile_Model(self, model):
        optimizer = Adam(lr=0.000010)
        model.compile(loss='mean_squared_error', optimizer=optimizer,metrics=['accuracy'])
        print(model.summary())
        return model

    def Add_Training_Data(self, train_x, train_y):
        if (self.batch_train_x is None):
            self.batch_train_x = train_x
            self.batch_train_y = train_y
        else:
            self.batch_train_x = np.append(self.batch_train_x, train_x, axis=0)
            self.batch_train_y = np.append(self.batch_train_y, train_y, axis=0)

        if ((self.batch_train_x.shape[0] % self.batch_size) == 0):
            self.Train_Epoch(self.batch_train_x, self.batch_train_y)
            self.batch_train_x = None
            self.batch_train_y = None

    def Train_Epoch(self, train_x, train_y):
        self.model.fit(train_x, train_y, epochs=1, batch_size=32)

    def __init__(self, inputs, outputs, batch_size = 20000):
        self.batch_size = batch_size
        self.model = self._Create_Model(inputs, outputs)
        self._Compile_Model(self.model)
