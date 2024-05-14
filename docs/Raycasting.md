# Raycasting

As a prelude, it should be noted that, despite the name, these classes are primarily in charge of handling multi-casting.

This is as these classes were written during work to implement multi-casting.

However, the calls to the Unity API and the overall logic are very similar between single and multi-casting.

The calls to these classes, and the logic for single-casting may be found in `ScreenSaver.cs`.

Summarised briefly, the logic is as follows :
1. For each `unityfile.mat` timestamp (which holds the movement data of the virtual avatar in experiment) :
    1. Perform operations to sync the timestamps between Unity location data & Eyelink eye-tracking data.
    2. Extract each eye-tracker data point linked to this timestamp.
    3. For each eye-tracking data point :
        1. If this is the start of a fixation event, start multi-casting process.
        2. If not, convert pixel coordinates to a Unity `Ray` object
        3. Perform raycasting via a call to `Physics.Raycast` (which is part of the Unity API)

### AreaRaycastManager.cs
Manages multi-casting. 
Initialised with a constructor that takes in a `RayCastSettings` instance :
1. Angular radius to multi-cast in.
2. Angular step size between rays.
3. Distance to screen in the experimental setup.
4. Dimensions of screen in the experimental setup (in cm).
5. Dimensions of screen in the experimental setup (in pixels).

Multi-casting may be actually scheduled via a call to `ScheduleAreaCasting`.

## OffsetData
A struct used to hold data about the offset from original gaze in the offset-gaze point.

### RangeCorrector.cs

Utility class that converts a point in one `Rect` to the equivalent point in another `Rect`.

E.g., if initialised to convert from a `Rect` of type (0,0,1,1) to one of type (0,0,1,2), it will convert (0.5,0.5) to (0.5,1). 

### RayCastWriteData.cs

Class that holds all data required to write to file.
Uses `Optional`s for empty fields.
Best used with `RayCastWriteDataBuilder`.

### RayCastWriteDataBuilder.cs

Class that constructs instances of `RayCastWriteData`.
Construct, and supply arguments with the respective `With{DATA}` methods, before calling `Build()` to build an instance of `RayCastWriteData`.


### RayCastWriteManager.cs

Manages writing to file.
Currently only supports .csv files, and write `RayCastWriteData` instances to file, by calling the `DataString` attribute, which is a string representation of the data.


### RaycastGazesJob.cs

Deprecated, but still used in single-casting in `Screensaver.cs`

### RaycastSettings.cs

Class that specifies the raycast settings. 

Holds default arguments and provides support via a factory method for construction from floats or strings.


### RelativeHitLocFinder.cs
Is a static class.

Finds relative hit given a raycast hit on an object. Takes reference from center point of the object's master, and takes into account normal.
Convention is for positive y in the relative hit to be going up, and cross product of normal and positive y vector is positive x. 