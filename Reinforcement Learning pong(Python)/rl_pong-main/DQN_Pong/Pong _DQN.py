#Adopted from https://github.com/sudharsan13296/Deep-Reinforcement-Learning-With-Python/blob/master
#/09.%20%20Deep%20Q%20Network%20and%20its%20Variants/9.03.%20Playing%20Atari%20Games%20using%20DQN.ipynb

import gym
import tensorflow as tf
from tensorflow import keras
from keras.layers import Dense, Activation, Conv2D, Flatten
from keras.models import Sequential
from keras.optimizers import Adam
import numpy as np
from collections import deque
import random
import time, matplotlib.pyplot as plt


def preprocess_image(state):
    state = state[35:195, :, :]  # crop the screen
    state = state[::2, ::2, :]  # downsample by a factor of 2
    state = tf.image.rgb_to_grayscale(state)  # convert to grayscale
    state = tf.cast(state, dtype=tf.float32) / 255.0  # normalize
    state = tf.expand_dims(state, axis=0)
    state = tf.expand_dims(state, axis=0)
    return state
    

class DQN:
    def __init__(self, state_size, action_length):
        
        self.state_size = state_size
        self.start_time = time.time()
        
        self.action_length = action_length
        self.r_buf = deque(maxlen=10000)
        
        self.gamma = 0.99 
       
        self.epsilon = 1.0
        self.epsilon_min = 0.02
        self.epsilon_decay = 0.00002 

        self.main_network = self.build_network()
        
        self.final_network = self.build_base_network()
        self.final_network.set_weights(self.main_network.get_weights())


    def build_base_network(self):
        model  = Sequential()

        model.add(Conv2D(32, (8, 8), strides=4, padding='same', input_shape=self.state_size))
        model.add(Activation('relu'))
        
        model.add(Conv2D(64, (4, 4), strides=2, padding='same'))
        model.add(Activation('relu'))
        
        model.add(Conv2D(64, (3, 3), strides=1, padding='same'))
        model.add(Activation('relu'))
        model.add(Flatten())
        model.add(Dense(512, activation='relu'))
        model.add(Dense(self.action_length, activation='linear'))
        return model
    
    def build_network(self):
        model = self.build_base_network()
        model.compile(loss='mse', optimizer=Adam())
        return model

    def fill_buf(self, state, action, reward, next_state, done):
        self.r_buf.append((state, action, reward, next_state, done))
    
    def action_policy(self, state):
        if random.uniform(0,1) < self.epsilon:
            return np.random.randint(self.action_length)
        q_val = self.main_network.predict(state, verbose=0,max_queue_size=1000,workers=40,use_multiprocessing=True)
        return np.argmax(q_val[0])

    def get_tq_value(self, state):
        q_vals = self.main_network.predict(state, verbose=0,max_queue_size=1000,workers=40,use_multiprocessing=True)
        action = np.argmax(q_vals[0])
        tq = self.final_network.predict(state, verbose=0,max_queue_size=1000,workers=10,use_multiprocessing=True)[0][action]
        
        return tq
    
    #train_agent the network
    def train_agent(self, batch_length):
        start = time.time()
        batch = random.sample(self.r_buf, batch_length)
        for state, action, reward, next_state, done in batch:
            if not done:
                tq = (reward + self.gamma * 
                            np.amax(self.main_network.predict(next_state, verbose=0,max_queue_size=1000,workers=10,use_multiprocessing=True)))
            else:
                tq = reward
            q_vals = self.main_network.predict(state,verbose=0,max_queue_size=1000,workers=10,use_multiprocessing=True)
            q_vals[0][action] = tq

            self.main_network.fit(state, q_vals,verbose=0,max_queue_size=1000,workers=10,use_multiprocessing=True)
        if self.epsilon > self.epsilon_min:
                self.epsilon *= self.epsilon_decay

        end = time.time()
        print("Time to train_agent", end - start )
            
    def upgrade_final_network(self):
        self.final_network.set_weights(self.main_network.get_weights())
  
    def load_final_network(self):
        self.final_network.load_weights("dqn_weights.h5f")

    def save_final_network(self):
        start = time.time()
        self.final_network.save_weights("dqn_weights_pong.h5f")
        end = time.time()
        print("Time to save_final_network", end - start )


env = gym.make("ALE/Pong-v5")
state_size = (1,80,80)
action_length = env.action_space.n
colour = np.array([210, 164, 74]).mean()
episodes = 1500
batch_length = 32
num_screens = 4   
dqn = DQN(state_size, action_length)
rewards_map = np.zeros((episodes,2),dtype=int)
dqn.load_final_network()
for i in range(1,episodes):
    print("Started Episode: ", i)
    Return = 0
    start = time.time()
    t=0
    state = preprocess_image(env.reset()[0])
    done = False
    while not done:
        t=t+1
        if len(dqn.r_buf) > 20 and (t % 20) == 0:
            dqn.upgrade_final_network()
            dqn.train_agent(batch_length)
        
        if len(dqn.r_buf) < 1000:    
            action = dqn.action_policy(state)
        else:
            action = np.random.randint(4,action_length)
        next_state,reward, done, truncated, info = env.step(action)

        next_state = preprocess_image(next_state)
        dqn.fill_buf(state, action, reward, next_state, done)
        state = next_state
        Return += reward
        if done:
            print("Finished one Episode: ", i)
            break
    print('Episode: ',i, ',' 'Return', Return)
    end = time.time()
    rewards_map[i][0] = i
    rewards_map[i][1] = Return
    if len(dqn.r_buf) > batch_length:
        dqn.train_agent(batch_length)
    print("Episode time = ",end - start)
    dqn.save_final_network()
    print(rewards_map)
    np.savetxt("rewards_pong.csv", rewards_map,fmt="%d", delimiter=",")

print(rewards_map)
np.savetxt("rewards_pong.csv", rewards_map,fmt="%d", delimiter=",")
#dqn.save_final_network()



