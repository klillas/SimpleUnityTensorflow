import h5py
import numpy as np
import random
import math

class Training_Database:
    file = None
    dataset_training_x = None
    dataset_training_y = None

    def __init__(self, database_file_path, parameters_x, parameters_y, database_size = 1000000, load_database = False):
        if load_database:
            self.file = h5py.File(database_file_path, 'a')
            self.dataset_training_x = self.file['training_x']
            self.dataset_training_y = self.file['training_y']
        else:
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

    def Get_Training_Data(self, shuffle = True, ratio = 1):
        samples = math.floor(self.dataset_training_x.shape[0]*ratio)
        dataset_x = self.dataset_training_x[0:]
        dataset_y = self.dataset_training_y[0:]
        if (shuffle):
            dataset_x, dataset_y = self.shuffle((dataset_x, dataset_y))
        dataset_x = dataset_x[0:samples]
        dataset_y = dataset_y[0:samples]
        return dataset_x, dataset_y

    def Print_Random(self, amount = 10):
        dataset_x, dataset_y = self.Get_Training_Data()
        for index in range(amount):
            print('x: ', end='')
            for value in dataset_x[index]:
                print('%.5f' % value + ' ', end='')
            print('')
            print('y: ', end='')
            for value in dataset_y[index]:
                print('%.5f' % value + ' ', end='')
            print('')
            print('')

    def shuffle(self, datasets, random_seed = 654):
        for d in datasets:
            random.seed(random_seed)
            random.shuffle(d)

        return datasets

    def Close(self):
        self.file.close()