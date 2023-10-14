# Code is inspired by Monsaleh's code.
# check out github.com/monsaleh/Pong_game_A2C/blob/main/ping_pong_a2c.ipynb

import os
import gym
import torch
import torch.nn as nn
from torch.distributions import Categorical
import numpy as np
from itertools import count
from matplotlib import pyplot as plt
import torch.nn.functional as F

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
print('You are using:', device, 'to train the agents')
env = gym.make("ALE/Pong-v5")
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

'''
def state_to_tensor(state):
    state = state[35:195, :, :]  # crop the screen
    state = state[::2, ::2, :]  # downsample by a factor of 2
    state = state.mean(axis=2)  # convert to grayscale
    state = state / 255.0  # normalize
    return torch.tensor(state, dtype=torch.float32).unsqueeze(0).unsqueeze(0)
'''

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
    
def discounted_returns(rewards, gamma=0.9):
    R = 0
    returns = []
    for reward in reversed(rewards):
        if reward != 0: 
            R=0      
        R = reward + gamma * R   
        returns.insert(0, R)
    
    returns = torch.tensor(returns)
    return returns


action_size = env.action_space.n
input_shape = [1,80,80]


pnet = ActorNet(input_shape, action_size).to(device)
cnet = CriticNet(input_shape).to(device)


p_optimizer = torch.optim.Adam(pnet.parameters(), lr=1e-4)
c_optimizer = torch.optim.Adam(cnet.parameters(), lr=1e-4)

running_reward = None

episodes_list = []
score_history = []
num_episodes = 1000

def reset_optimizer(optimizer,losses):
    optimizer.zero_grad()                                           
    loss = torch.cat(losses).sum()
    loss.backward()
    optimizer.step()
'''
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
        env.render()
        state = next_state
        if is_done:
            break
'''
for e in range(num_episodes):
    action_log_probs = list()
    rewards = list()
    values = list()
    state = env.reset() 
    state = state[0]
  
    prev_state = None     
    #print(state)
    for t in range(100000):
        #print(prev_x)
        #print(preprocess_state(state))
        x = pre_process(state,prev_state)   
        prev_state = state 
        action_prob = pnet(torch.FloatTensor(x).to(device))
        action = action_prob.sample()                       
        action_log_probs.append(action_prob.log_prob(action))    
        value = cnet(torch.FloatTensor(x).to(device))            
        values.append(value[0])
        #print(env.step(action.item()))
        next_state, reward, is_done, _, _ = env.step(action.item()) 
        rewards.append(reward)         
        #print(next_state)
        state = next_state           
        if is_done:

            break

    returns = discounted_returns(rewards)           
    action_losses = []
    critic_losses = []

     # collect the action losses to a list
    for ret, l_prob, v in zip(returns, action_log_probs, values):
     # this tells us how much better it is to take a specific action compared to the average        
        advantage = ret - v                             
     # policy gradients equal to probability for the actions taken scaled with advantage 
        action_losses.append(-l_prob * advantage.detach())            
        critic_losses.append(advantage.pow(2))                        

    reset_optimizer(p_optimizer,action_losses)
    reset_optimizer(c_optimizer,critic_losses)

    ep_reward = sum(rewards)
    score_history.append(ep_reward)  
    episodes_list.append(e)
    # print last and average reward          
    running_reward = ep_reward if running_reward is None else running_reward * 0.99 + ep_reward * 0.01
    print('Episode: %d Last Reward: %.2f Avg Reward: %.2f' % (e, ep_reward, running_reward))  
    # Save actor agent after 10 episodes of training
    if e == 0:
        torch.save(pnet.state_dict(), 'a2c_init.pth')
    if e == 100:
        torch.save(pnet.state_dict(), 'a2c_100.pth')
    if e == 500:
        torch.save(pnet.state_dict(), 'a2c_500.pth')
    if e == 999:
        torch.save(pnet.state_dict(), 'a2c_1000.pth')
np.savez("score_and_episodes.npz", score_history=score_history, episodes_list=episodes_list)

Filename = 'A2C_Pong.png'
plt.plot(episodes_list, score_history)
plt.xlabel('Episodes')
plt.ylabel('Total Reward')
plt.title('Reward History')
plt.show()
plt.savefig(Filename)
