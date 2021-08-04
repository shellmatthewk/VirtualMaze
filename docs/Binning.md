# Binning

### Bin.cs
Initialise Bin class

### BinMapper.cs
Abstract methods for:
* GetGroupID
* BinWallConfig
* PlaceBinWall
* MapBinToId
* MapGroupToWallCache
* GetSpecialCacheId
* IsSingleWallGroup
* MaxPossibleSqDistance

Location:
Public structure - Get position and rotation values

### BinningRaycastTest.cs
Test script for binning raycast

### BinRecorder.cs
Creating and saving bin data

### BinWall.cs
Create bin walls

BinWallConfig:
Public structure - set bin/fill width/height

### BinWallManager.cs
Perform raycasting on bins using eye gaze data

### DoubleTeeBinMapper.cs
BinMapper for DoubleTee Maze
