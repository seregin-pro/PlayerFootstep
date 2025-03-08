# Player Footstep Sound for Unity 3D

The script adds the ability to walk to the character with footstep sound effects. The sounds change depending on the material of the object on which the character is located and by the texture index when on the terrain.

## Setup

### Footstep.cs

1. Add the script Footstep.cs to the Character Controller of your Player.

2. Add Terrain Layers.

<p width="400" align="center">
  <img src="https://github.com/seregin-pro/PlayerFootstep/blob/main/Screenshots/001.png" />
</p>

3. Set index of a Terrain Layer to Element and add Audio Clips for the Terrain Layer

<p align="center">
  <img width="400" src="https://github.com/seregin-pro/PlayerFootstep/blob/main/Screenshots/002.png" />
</p>

### TextureTerrainData.cs

Add the script TextureTerrainData.cs to the Terrain.