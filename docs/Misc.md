# Misc

### ConfigurableComponent.cs
* A serializable subclass must be created and filled with the parameters to save.
* Create a subclass which extends SerializableSettings and add the [System.Serializable] attribute.
* Update the implemented methods to return the subclass.
* To add your ConfigurableComponent to SaveLoad, use SaveLoad.registerConfigurableComponent and the Configs will be saved.

### CueController.cs
#### Public methods:
* ShowCue
* HideCue
* ShowHint
* HideHint
* HideAll

### DataViewer.cs
Display data in GUI

### DirectoryContents.cs

### EventManager.cs

### ExperimentController
Gets settings from Experiment GUI and controls progress of whole experiment

### ExperimentLogger.cs
Logs information regarding the Experiment in to a text file.
Each experiment should have its own ExperimentLogger.
Each Session will be logged into its individual file.

### Extensions.cs
Class to contain all extension methods for faster development

### Eyelink.cs
Initialises Eyelink and sends messages to be saved in EDF file

### FileBrowser.cs
Notifies the listeners when Filebrower hides itself

### FileItem.cs

### FileWriter.cs

### GameController.cs
MonoBehaviour that affects VirtualMaze globally

### GazePointPool.cs
Canvas to display the individual sample points. An object pool is used for efficiency.
Attach this to a canvas component.

### InputRewardNo.cs

### InterfaceTest.cs
Random script for quick testing of code. Free to change

### JoystickController.cs

### LevelController.cs
Controls progress within each session

### MenuController.cs
To improve efficiency and allow scripts to be executed when a menu is to be hidden, set alpha of canvasgroup to 0 instead of disabling the GameObject
Although online sources state to just disable the canvas component, disabled canvas components still receive click events. Therefore, this method is used

### NetworkConnection.cs

### ParallelPort.cs

### Persist.cs
Any child gameobject or the gameobject attached to this script will not be destroyed on load
Used to preserve gameobjects from start scene when transitioning to other maze scenes

### ProgressCalibPoint.cs

### RewardsController.cs
Gets settings from Rewards GUI

### RobotMovement.cs
Script to control the movement of the robot
If robotmovement.enable = false, the script will be unable to broadcast the robot's location, therefore use SetMovmentActive(bool active)

#### RandomiseDirection:
Public method for randomising robot direction (y rotation)

#### MovetoWaypoint:
Public method for moving robot to waypoint

#### SetMovementActive:
Public method for enabling/disabling robot movement (e.g. in-between sessions/trials)

### SaveLoad.cs

### ScreenSaver.cs

### SessionController.cs
Contains public methods used by ExperimentController.cs and LevelController.cs to perform session-level tasks

### SessionPrefabScript.cs

### SetCounter.cs

### SliderValue.cs

### VersionInfo.cs
