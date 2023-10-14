import os
import gym
import torch
import torch.nn as nn
from torch.distributions import Categorical
import numpy as np
from itertools import count
from matplotlib import pyplot as plt
import torch.nn.functional as F
from gym.wrappers import RecordVideo
import cv2

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

env = gym.make("ALE/Pong-v5", render_mode = 'human')

observation = env.reset()
observation = observation[0]
#print(observation)
print("Actions Meaning", env.get_action_meanings())  
print("Observation", env.observation_space.shape)     

def state_to_tensor(state):
    if state is None: 
          return np.zeros(6400)  
    state = state[35:195]       
    state = state[::2, ::2, 0]  
    state[state == 144] = 0    
    state[state == 109] = 0    
    state[state != 0] = 1       
    return state


def pre_process(cur_x, prev_x):
    cur_state = state_to_tensor(cur_x).reshape(1,1,80,80)

    prev_state = state_to_tensor(prev_x).reshape(1,1,80,80)
    '''
    if prev_x is None:
        prev_x = np.zeros_like(cur_x)
    '''
    diff_state = cur_state - prev_state
    return diff_state

# actor neutral network
# returns the policy with the probability distribution over our actions

def get_conv_out(conv, shape):
    with torch.no_grad():
        x = torch.zeros(1, *shape)
        for layer in conv:
            x = layer(x)
        return int(np.prod(x.size()))

class ActorNet(nn.Module):
    def __init__(self, input_shape, num_actions):
        super(ActorNet, self).__init__()

        self.conv = nn.ModuleList([
            nn.Conv2d(1, 32, kernel_size=8, stride=4),
            nn.ReLU(),
            nn.Conv2d(32, 64, kernel_size=4, stride=2),
            nn.ReLU(),
            nn.Conv2d(64, 64, kernel_size=3, stride=1),
            nn.ReLU()
        ])

        conv_out_size = get_conv_out(self.conv, input_shape)
        self.fc = nn.Sequential(
            nn.Linear(conv_out_size, 512),
            nn.ReLU(),
            nn.Linear(512, num_actions),
            nn.Softmax(dim=1)
        )

    def forward(self, x):
        for layer in self.conv:
            x = layer(x)
        x = torch.flatten(x, start_dim=1)
        x = self.fc(x)
        dist = Categorical(x)
        return dist


class CriticNet(nn.Module):
    def __init__(self, input_shape):
        super(CriticNet, self).__init__()

        self.conv = nn.ModuleList([
            nn.Conv2d(1, 32, kernel_size=8, stride=4),
            nn.ReLU(),
            nn.Conv2d(32, 64, kernel_size=4, stride=2),
            nn.ReLU(),
            nn.Conv2d(64, 64, kernel_size=3, stride=1),
            nn.ReLU()
        ])

        conv_out_size = get_conv_out(self.conv, input_shape)
        self.fc = nn.Sequential(
            nn.Linear(conv_out_size, 512),
            nn.ReLU(),
            nn.Linear(512, 1)
        )

    def forward(self, x):
        for layer in self.conv:
            x = layer(x)
        x = torch.flatten(x, start_dim=1)
        x = F.relu(self.fc(x))
        return x
 


action_size = env.action_space.n
input_shape = [1,80,80]
pnet = ActorNet(input_shape, action_size).to(device)
cnet = CriticNet(input_shape).to(device)



def play_game_with_saved_agent(agent_path):
    # Load saved actor agent
    pnet.load_state_dict(torch.load(agent_path, map_location=device))

    # Play game with saved actor agent
    state = env.reset()
    state = state[0]
    prev_state = None
    while True:
        x = pre_process(state,prev_state)
        prev_state = state
        action_prob = pnet(torch.FloatTensor(x).to(device))
        action = action_prob.sample()
        next_state, reward, is_done, _, _ = env.step(action.item())
        #env.render()
        state = next_state
        if is_done:
            break
play_game_with_saved_agent('a2c_1000.pth')
#print(os.listdir(video_dir))
env.close()
