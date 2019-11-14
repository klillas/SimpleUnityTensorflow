import h5py
import numpy as np
import random

class Training_Database:
    file = None
    dataset_training_x = None
    dataset_training_y = None

    def __init__(self, database_file_path, parameters_x, parameters_y, database_size = 1000000):
        self.file = h5py.File(database_file_path, 'w')
        self.dataset_training_x = self.file.create_dataset("training_x", shape=(0, parameters_x), dtype=float, maxshape=(None, parameters_x))
        self.dataset_training_y = self.file.create_dataset("training_y", shape=(0, parameters_y), dtype=float, maxshape=(None, parameters_y))

    def Add_Training(self, training_x, training_y):
        self.dataset_training_x.resize((self.dataset_training_x.shape[0] + 1, self.dataset_training_x.shape[1]))
        self.dataset_training_y.resize((self.dataset_training_y.shape[0] + 1, self.dataset_training_y.shape[1]))
        self.dataset_training_x[self.dataset_training_x.shape[0]-1:] = training_x
        self.dataset_training_y[self.dataset_training_y.shape[0]-1:] = training_y

        if (self.dataset_training_x.shape[0] % 1000 == 0):
            print(str(self.dataset_training_x.shape[0]) + " items added into training database")

    def Get_Training_Data(self, shuffle = True):
        dataset_x = self.dataset_training_x[0:]
        dataset_y = self.dataset_training_y[0:]
        if (shuffle):
            self.shuffle((dataset_x, dataset_y))
        return dataset_x, dataset_y

    def shuffle(self, datasets, random_seed = 654):
        for d in datasets:
            random.seed(random_seed)
            random.shuffle(d)