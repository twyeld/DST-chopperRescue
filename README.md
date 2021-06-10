# DST-chopperRescue
 Unity3D ML-agents chopper rescue test

This is an early pre-release example of a DST project that was developed to demonstrate visualisation as a service.

It was built to demonstrate extensibility of docker with an xpra VNC x-window on multiple machines.

It shows performance of FPS is dependent on platform.

The linegraph has a known memory leak that affects performance over time.

This was fixed in later versions.

The final training NN is classified.

## Presentation of project to the 4th Modelling Complex Warfighting Symposium 11-12 December 2019

<img src="https://user-images.githubusercontent.com/29798223/121505913-652bcc00-ca22-11eb-82c4-1d973d7e826b.jpg" width="25%" height="25%">

## Overview
The purpose of this project was to allow realtime monitoring of reinforced learning (RL) training cycles of combat simulation using the Unity game engine on any device anywhere and be able to change the seed values on the fly. With this, various outcomes can be measured against each other to realise optimal solutions and assist decision-making about combinations of capability and resources.

In order to achieve this, a method was necessary for synchronised streaming of simulation runs that could be viewed on different devices at the same time. 

Docker containerisation was used so that the application had access to GPU parallel and graphics processing on the server so that only a very thin VNCwas needed on the client side.

The main problem to overcome was how to run Unity on a server and make a video stream of the output available to a remote device at a reasonable frame rate in realtime?

While both these issues were resolved, considerably more work is needed to optimise the solution to improve performance. For example, while up to 1000fps is achievable on the HPC-server with its GPU cluster, typically only 30fps is achievable from most VNC clients. This is both a bandwidth issue and the overhead of an over-specced VNC. Therefore, more work is needed on developing a customising VNC.  XPRA is one of the only x-forwarding VNCs that supports GPU rendering server-side.

## Realtime monitoring of combat simulation RL training runs
The ability to monitor combat simulation RL training runs in realtime means we can see how the agents’ interactions evolve over time. Although we are using 3D models of choppers, terrain, troops and so on, it is the processing that happens in the background that matters. And, the time scale has no real relevance other than as a linear variable. In fact, typically, ML training of multi-agent interactions has no graphics component just nominal tokens performing transactions according to some rules producing repeatable outcomes. The raw data can then be visualised in many different formats to tell us different things about the underlying processes. But, by using models on the other hand, we can interact with these processes to see what effect they might have in an unfolding epoch or training run. That way, we don’t have to wait until the run has finished before changing the seed values and run again.

It allows for rapid testing and retesting of ideas that can be seen to take effect. Thus, decision-making about particular combinations of capability and resource scenarios can be tested and examined by groups of stakeholders at the same time.

Visualisation as a service on any device anywhere
For a group of stakeholders wanting to be able to test and examine an unfolding epoch in realtime, viewed on different devices, we need a system that supports multiple different devices that can also access the same data stream synchronously. 

But, there is no easy way to do this – that is efficient anyway.

The Unity game engine provides a method for viewing its output in realtime.

Most compiled unity game-engine applications run on a local machine. The engine either comes bundled with the game data or as an editor. For reasonable performance, a local device with decent graphics acceleration capabilities is required. Even though unity will run in a browser window, it still needs to access some GPU acceleration for reasonable frame rates. If it is a multi-user game, it needs a network connection with enough bandwidth to transfer the state and coordinate updates. But, while a multi-user game needs to update state and coordinates, we only need to port a video stream of the results as they are rendered.

<img src="https://user-images.githubusercontent.com/29798223/121506666-13d00c80-ca23-11eb-848e-df46965cc381.jpg" width="50%" height="50%">

 
Video streaming is an obvious solution. But it has many barriers to overcome, the least of which is bandwidth. To ensure all connected devices are receiving the same data at the same time all the GPU processing is done server-side. This ensure that, even if frames are dropped, they do not get out of sequence.

This means the video data from the server comes from only a single source. This is served by an x-forwarded desktop environment to whatever device is connected.

<img src="https://user-images.githubusercontent.com/29798223/121506747-28aca000-ca23-11eb-9f22-5c2bc5934b08.png" width="50%" height="50%">

To orchestrate this, a Docker container is used to support the particular specifications for each device. Docker spins up an x-forwarded video stream for each device connected with its own port number. Using a HPC-server with a cluster of GPU cards, there is enough processing power to ensure each container has access to the maximum resources it needs to get the optimal throughput of the video stream.

<img src="https://user-images.githubusercontent.com/29798223/121506829-40842400-ca23-11eb-8519-77e8f921d12e.jpg" width="50%" height="50%">

## Reinforcement Learning
The test case uses a combat simulation involving multi-agent reinforcement learning. Over time, the agents learn how to achieve the goals set for them more efficiently. RL training epochs require GPU processing on the HPC-server.

<img src="https://user-images.githubusercontent.com/29798223/121506961-601b4c80-ca23-11eb-921d-2a7fb50d6b11.png" width="50%" height="50%">

## Virtual Network Client
There are lots of VNC clients that can be used for video streaming that also allow for some interactivity with the source app. In order to be able to change the seed values from our remote device we needed to use a VNC. 

## X11docker
Mviereck’s x11docker base image allows GUI apps to run inside a Docker container with access to the host machine’s GPU processors:
–	It runs an X display server on the host system and provides it to our Docker container.
–	It does some security setup to avoid X security leaks in a sandbox environment.
–	There are no obliging dependencies on the host besides X and Docker.
–	Remote access with SSH, VNC or HTML5 is possible, and
–	hardware acceleration for OpenGL using the NVidia CUDA drivers.
Its also possible to run containers with different backends following the OCI runtime specification using a json manifest which gives us the flexibility to support multiple different client devices from the same base image.

<img src="https://user-images.githubusercontent.com/29798223/121507177-8c36cd80-ca23-11eb-9f5c-f5f27c23bc5d.jpg" width="50%" height="50%">


With the x-forwarding application xpra installed on the host machine, this was used for the VNC client to attach to. Xpra supports ssh or tcp over ip. We just need to specify a port and display number for the x server to broadcast from and another port for the client to attach to.

<img src="https://user-images.githubusercontent.com/29798223/121507223-98bb2600-ca23-11eb-87c0-6ee1d2b8d4b7.png" width="50%" height="50%">

And there it is. A unity game engine server, running in Docker on a HPC with a GPU cluster using all that processing power server-side simply streaming the results to a very thin client but with interactivity so we can adjust the seed values on the fly.

<img src="https://user-images.githubusercontent.com/29798223/121507292-a83a6f00-ca23-11eb-9cb0-4ca4f7eefd16.jpg" width="50%" height="50%">

 
