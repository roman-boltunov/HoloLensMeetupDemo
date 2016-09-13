# HoloLens Meetup Demo

My HoloLens application demo for Akvelon IT-garage meetup. <br/>

In this demo you control a little kitten using XBox controller.<br/>
You can walk or jump on the spatial mapping as well as show some kitten emotions (like meow or itching).

<b>Tools used in this application:</b><br/>
1. Brekel Gamepad Server/Client (http://brekel.com/gamepad-serverclient/gamepad-serverclient-retail-installer-download/)<br/>
2. Cute Kitten 3D model (https://www.assetstore.unity3d.com/en/#!/content/33121)<br/>
<br/>
<b>How to install:</b><br/>
1. Clone this repository<br/>
2. Open it in Unity3D for HoloLens<br/>
3. Download Brekel Gamepad Server/Client (Alternatively you can use XBox One S controller and connect it to HoloLens via bluetooth. If this is your case, you can proceed to step 10)</br>
4. Start Brekel Gamepad Server </br>
5. Connect XBox controller to PC </br>
6. Observe it in the Brekel Gamepad Server console </br>
7. In Unity3D open MainScene. </br>
8. Select Logic game object </br>
9. Find Gamepad_Client component and write your server (computer) name to Server Host Name </br>
10. Deactivate Plane game object<br/>
11. Build project for Windows Store Windows 10 Direct3D</br>
12. Open Visual Studio after build is done</br>
13. Deploy to your HoloLens device</br>

<b>How to control:</b><br/> 
Left Stick - move kitten <br/> 
Right Bumper - run <br/> 
Left Bumper - itching emotion <br/> 
A - jump <br/> 
B - meow emotion <br/> 
