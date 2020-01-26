import socket, sys, os, json, threading


import tensorflow as tf
import numpy as np
import argparse

from ppo_model import PPOModel
from agent_worker import AgentWorker

class TrainModel:
    def __init__(self ,args):
        self.num_actions = 6
        # TODO: Dynamically set the number of states and actions
        self.ppo_model = PPOModel(num_states=args.num_states, should_load_models=args.load_models, num_actions=self.num_actions, epochs=args.epochs, num_hidden_layers=args.hidden_layers)
        #self.ppo_model.build_actor_and_critic()
        self.header_len = 8
        self.port = 12001


    def start(self):
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
            s.bind(("", self.port))
            s.listen(20)
            print("Starting accepting")
            while True:
                print("WAITING TO ACCEPT")
                conn, client_adr = s.accept()
                print("Connected by", client_adr)
                worker = AgentWorker(self.ppo_model)
                training_worker = threading.Thread(target=worker.start_training_agent, args=(conn,))
                training_worker.start()
                #training_worker.join()
                


def main(args):
    if os.path.exists("ai_controller"):
        os.remove("ai_controller")

    agent = TrainModel(args)
    # agent.build_model()
    agent.start()


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--load_models", action="store_true",
        help="Load the saved models.")
    parser.add_argument("--epochs", type=int, default=5,
        help="Number of epochs to train on each episode.")
    parser.add_argument("--use_conv", action="store_true",
        help="Use CNN architecture.")
    parser.add_argument("--hidden_size", type=int, default=52,
        help="Number of hidden units in hidden layers.")
    parser.add_argument("--hidden_layers", type=int, default=2,
        help="Number of hidden_layers.")
    parser.add_argument("--num_states", type=int, default=558,
        help="Number of states for input size.")
    args = parser.parse_args()
    main(args)
