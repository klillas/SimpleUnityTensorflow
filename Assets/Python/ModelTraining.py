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

def Create_Model():
    input_layer = Input(shape=(1), name='input')
    model.add(Dense(30))
    model.add(Activation("relu"))
    model.add(Dense(30))
    model.add(Activation("relu"))
    model.add(Dense(30))
    model.add(Activation("relu"))
    
    model.add(Dense(classes))
    model.add(Activation('sigmoid'))

    return model