# Dinosaur-Game-AI
 AI to play Google Chrome Dinosaur Game made with neural networks and based on evolution.

![Screenshot](https://github.com/cau777/Dinosaur-Game-AI/blob/main/Screenshot.png?raw=true)

## How the Algorithm Works
The program generates the specified number of dinosaurs and assigns AIs with random weights and biases.
These randomly generated AIs compete, and the one with the best score passes its weights and biases to all the other AIs. Random mutations happen in this process.
Most of the process happens on the GPU, so the program supports hundreds of AIs.

## How the AI works
The AI is created with neural networks. This model is inspired by the behavior of the human brain. There are several layers and, in each layer, neurons that are connected to the ones in the next layer. For this project, I used a simple model with four layers constituted by seven, six, four, and three neurons.
Each neuron has a bias value, and each connection between neurons has a weight value. These values go from -2 to 2.
The first layer represents the inputs, and the last one describes the outputs. In this case, the neuron with the highest value shows the decision the AI made.
Another critical part of this type of AI is the activation function. I chose "1 / (1 + e⁻ˣ)".
To know the value of a neuron, the program sums the bias with the values of all neurons in the previous layers multiplied by their respective weights. After that, it applies the activation function.

## Spotboo.Unity.dll
This is a custom class library that I made to speed up working with Unity. It contains some utility methods (like SaveJson and Screenshot), some extension methods (like Vector3 ToVector4) and some classes that I almost alway use (like Initializer and SoundManager).