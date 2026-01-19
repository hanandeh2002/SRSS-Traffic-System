SRSS – Smart Road Safety System
User Manual & Installation Guide
1.	Introduction
This document explains how to install, run, and test the Smart Traffic Safety System (SRSS) project.
The system is a simulation-based project developed using Unity and a web-based control panel to monitor:
•	Traffic accidents
•	Speed violations
•	Adaptive traffic signal control (based on congestion)
•	Police and ambulance dispatch notifications
•	This guide is intended for trainers and assessors to run the project from scratch without prior setup.

2.	System Requirements
2.1 Software Requirements
The following software must be installed before running the project:
A.	Unity
•	Unity Hub
•	Unity Editor (2021.3.31f1 LTS version)
•	Windows Build Support (recommended)
B.	Visual Studio
Visual Studio 2022
Required workloads:
•	Game development using Unity
C.	Visual Studio Code
Visual Studio Code (VS Code)
Add-on:
Live Server
D.	Web browser
Google Chrome or Microsoft Edge
3.	Project Folder Structure
The project consists of two main parts:
3.1 Unity Project
This contains the city simulation, vehicles, sensors, and logic.
3.2 Web Dashboard
This displays real-time monitoring data.
The folder structure must be as follows:
Desktop / SRSS_Dashbord (index.html, script.js ,style.css ,data.json ,
Images (accident_*.png , violation_*.png))
Important Note:
The SRSS_Dashboard folder must be located on your desktop.
The Unity project automatically writes data to this specific location.
4.	Running the Web Dashboard
Steps by Step
1.	Open Visual Studio Code
2.	Click File → Open Folder
3.	Select the folder: Desktop/SRSS_Dashboard
4.	Right-click the index.html file
5.	Choose "Open with live server"
 The web dashboard will open in your browser and update automatically.

5.	Running the Unity Emulator
Steps by Step
1.	Open Unity Hub
2.	Click "Open Project"
3.	Select the Unity project folder
4.	Open the main scene (e.g., SampleScene)
Click Run 
Unity will begin writing live data to: Desktop/SRSS_Dashbord/data.json
Captured images will be saved to: Desktop/SRSS_Dashbord/images
6.	System Features & How to Test Them
6.1 Accident Detection
How to test:
A.	Control the player vehicle
B.	Crash into any vehicle in the city
Expected results:
	Accident image is captured
	Image saved in images/ folder
	Dashboard displays accident image
	Total accidents counter increases
	Police & ambulance status changes to Dispatched
6.2  Speed Violation Detection
How to test:
A.	Drive the player car through a speed sensor zone
B.	Exceed the speed limit
Expected results:
	Speed violation image is captured
	Image saved in images/
	Total violations counter increases
	Dashboard updates automatically

6.3 Adaptive Traffic Light (Congestion Control)
How it works:
	Each lane has a sensor that counts vehicles
	If 8 or more vehicles are detected, priority is given to that direction
	Green light duration is adjusted dynamically
Expected results:
	Congested direction stays green longer
	Less congested directions wait
	Fairness logic prevents starvation
7.	Data & Dashboard Explanation
The dashboard reads data from data.json, which includes:
•	Traffic light states (North, South, East, West)
•	Accident status
•	Speed violation status
•	Total accidents (cumulative)
•	Police and ambulance status
•	Timestamp of last update
The dashboard updates automatically every few seconds.
8.	Technologies Used
•	Unity Engine  : Simulation and AI logic
•	C#  : Accident detection, speed sensors, adaptive control
•	HTML / CSS / JavaScript  Dashboard UI
•	Chart.js Data visualization
•	VS Code + Live Server Dashboard execution
•	Visual Studio  Unity scripting
 Notes for Evaluators
•	Unity must be running for the dashboard to update
•	Dashboard must be opened using Live Server
•	The system simulates real-world intelligent traffic management concepts
•	No external server is required

