import tensorflow as tf
import numpy as np


class PPOModel:
    def __init__(self, num_states, hidden_size=256, num_hidden_layers = 5):
        self.num_states = num_states
        self.hidden_size = hidden_size
        self.num_hidden_layers = num_hidden_layers
        pass
    
    def build_actor(self):
        inputs = tf.keras.Input(shape=(self.num_states,))
        for _ in range(self.num_hidden_layers):

            tf.keras.layers.Dense
        self.actor = tf.keras.models.Sequential()
        pass

    def build_critc(self):

