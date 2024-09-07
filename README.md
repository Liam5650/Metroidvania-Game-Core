# Metroidvania Game Core

![DemoImage](https://github.com/Liam5650/Metroidvania-Game-Core/blob/main/READMEImages/DemoImage.jpg)

Metroidvania Game Core is a comprehensive game framework designed to provide an accessible way to create Metroidvania-style games in Unity. A web demo is [available on itch.io](https://liam5650.itch.io/metroidvania-demo) and highlights many of the framework's key features. 

## Features

This framework features a variety of essential Metroidvania gameplay mechanics, such as the ability to build complex maps with interconnected rooms and seamless transitions, ability-based progression to create more dynamic, non-linear gameplay, as well as a robust save system that tracks detailed game states and manages event triggers like boss battles. All features have been designed with ease-of-use in mind, making it quick and straightforward to effectively use the framework.

## Installation

Simply click on the green "Code" button at the top-right of the project's main page, and select "Download ZIP". Unzip the file, open Unity Hub and click "Add", then navigate to and select the project folder. If you are prompted that the project was made using a different version of Unity, you can download editor version 2021.3.14.f1 to ensure compatibility. Later editor versions are likely to work without issue as well, but have not been tested. 

## Basic Usage and Room Components

The map is structured so that individual rooms are made up of cells on a grid, as seen here:

![MapStructure](https://github.com/Liam5650/Metroidvania-Game-Core/blob/main/READMEImages/MapStructure.jpg)

Each cell measures 40 by 22.5 x/y units, which was chosen to fit a 16:9 aspect ratio that is common on many displays while also allowing for adequate space to add tiles (each tile measures 1x1). When creating rooms, you can right click on the scene titled "World Grid" and select "Open Scene Additive" to help make sure everything is aligned as expected.

To make a room, simply create a new scene and add the "CameraBounds" prefab from the "Camera" folder, and the "Interactable Grid" prefab from the "Environment folder. Some basic information about each prefab:

### CameraBounds

This prefab sets the bounds of the camera for the room. You can think of it as the active play area for the player. The size of the bounds by default is the same 40 x 22.5 units as one cell in the world grid. This allows for easy alignment of the bounds with the grid by adjusting the x and y positions of its transform. To create a bigger room, you can simply set a multiple for the prefab's transform x or y scale. Here is a more detailed explanation:

![CameraBounds1](https://github.com/Liam5650/Metroidvania-Game-Core/blob/main/READMEImages/CameraBounds1.jpg)

This is the spawn room of the player. The camera bounds position is bottom-left aligned, so its transform's position is at 0,0 (x,y). The x scale has been increased to two, which gives us 2 x 1 cells of space. 

![CameraBounds2](https://github.com/Liam5650/Metroidvania-Game-Core/blob/main/READMEImages/CameraBounds2.jpg)

This is the room immediately to the right of the spawn room. The scale of the camera bounds has been set to 1 x 5 to give more verticality. Since we want the bounds to start two cells to the right and one below the spawn room, we set the x position to 80 (2 x 40), and the y position to -22.5 (-1 * 22.5). Using this placement technique based on the dimensions of one cell makes it easy to ensure proper alignment between rooms. 

### Interactable Grid

The "Interactable Grid" prefab is for placing physical tiles that the player interacts with, and is used for creating ground, walls, platforms, etc. 

![Grid](https://github.com/Liam5650/Metroidvania-Game-Core/blob/main/READMEImages/Grid.jpg)

To add tiles, make sure the grid is selected, click on the "Tile Palette" tab, and then you can begin painting. You can click on the dropdown where "Tileset" is specified, and change it to "Objects" for even more tile variety. 

The "Environment" folder also contains a "Background Grid" and a "Detail Grid" that you can add to a room if you wish to do so. These are tilemap grids that do not have any interaction with the player, and are used for detailing. "Interactable Grid" always appears on top of the other two, and "Detail Grid" appears on top of "Background Grid". Make sure you click on the tilemap you would like to edit before adding tiles, otherwise you may unknowingly be adding tiles to the wrong one. The "Effects/Atmospheric" folder within the "Environment" folder contains some premade particle systems called "World Particles" and "Cave Particles" you can add as well to further increase room detail. The emission rates of particles and emission shapes can also be easily adjusted to match the size of the room.

## Additional Components

### Room Transitions

Transitioning between rooms / scenes is handled using a prefab named "SceneTransition" that employs a trigger area that loads a new scene when entered by the player. It can be found in the "Environment/Doors" folder. To use it, position it within a room, and enter the name of the room you would like to transition to on its "Door Transition" script in the "Room To Load" field. I recommend adjusting its transform's position to be 1 - 1.25 units beyond the edge of the bounds of the camera, as this makes the transition appear smoother as it occurs as soon as the player is off of the screen. For example, if the camera bounds end at 80 units in the x axis, set the x position of the SceneTransition transform to 81 - 81.25 units. Here is an example image with the trigger area highlighted in white to show placement relative to the bounds of the camera: 

![RoomTransition](https://github.com/Liam5650/Metroidvania-Game-Core/blob/main/READMEImages/RoomTransition.jpg)

### Map UI

To edit the world map UI, open the "Map Test" scene and open the "World Map" prefab. The overall structure is as follows:

![WorldMap](https://github.com/Liam5650/Metroidvania-Game-Core/blob/main/READMEImages/WorldMap.jpg)

Each cell in the map grid is equivalent to a cell in the world grid, i.e., the 1 x 1 cells map to 40 x 22.5 x/y world coordinates. This allows the map script to easily keep track of the position of the player in the world and color cells on the map to show that they have been visited using the "Room Visited Fill" tilemap. The "Room Default Fill" tilemap can be manually painted in using the "map" tile palette as seen on the right side of the image, and represents the rooms that make up the world. The "Outline Grid" is used to show connections between rooms, walls, etc. The color of any of these tilemaps can also be modified by changing the "Color" field on the tilemap in the inspector. 

### Pickups

A variety of pickups are included, including "Upgrades" that unlock player abilities, "Missile-Tanks" that increase the player's max missile count, and "E-Tanks" that increase the player's max health by 100 units. These can be placed anywhere in the world, and are triggered upon contact with the player. Each contains an "Upgrade ID" field, which allows the pickup to be uniquely identified by the save controller and ensures that it deactivates upon pickup. The "Upgrade" prefab contains additional toggles on its script that allow you to easily change what upgrade it unlocks. There is also an "Ammo Pickup" and "Health Pickup" prefab, which are dropped upon defeating enemies and restore the player's ammo and health by a small amount that can be set on their respective scripts. 

![Pickups](https://github.com/Liam5650/Metroidvania-Game-Core/blob/main/READMEImages/Pickups.jpg)

### Saving

To allow the player to save the game, place the "Save Station" prefab from the "Environment/Save Points" folder anywhere in the world. The upper platform will begin to depress when the player stands on it, and will trigger a save to be made once it is fully depressed. The save data contains a variety of information in regards to player state, as well as lists that hold the state of various upgrade / event identifiers set using their respective "Upgrade ID" / "Event ID" fields on the objects. It is important to make sure each pickup / event uses a unique integer in a range from 0 to the size of the array for its "ID" field, as each integer maps to only a single position in the coressponding array. A value of 0 at an index means that the object has not been collected / triggered, whereas a value of 1 means it has. Here is a closer look at everything that the "PlayerData" class contains:

![SaveData](https://github.com/Liam5650/Metroidvania-Game-Core/blob/main/READMEImages/SaveData.jpg)

When starting a new game, the player loads into the room specified by the "roomName" string (by default, this is set to "Room1"), and at the position specified by "playerPosition" (by default, x=27, y=9, z=0). Make sure to set these appropriately if your initial room name or player position is different.

### Player 

The "Player" prefab contains four script components with modifiable fields to easily adjust gameplay. The "Player Combat" script handles everything combat-related, such as the player beam / missile / bomb to fire. The "Player Health" script handles things such as tracking player health and allows you to modify the duration of which the player is briefly invincible after taking damage. The "Player Movement" script contains numerous values that can be changed to adjust the gameplay fluidity and feel. Finally, the "Unlocks" script tracks what abilities the player currently has access to. 

### Enemies

The project includes two basic enemy types in the "Characters/Enemies" folder. The "Drifter" is a flying enemy that slowly tracks and tries to hit the player once within a certain distance. The "Walker" enemy patrols on the ground, and its prefab contains two child transforms that you can position and serve as the waypoints that it moves between. There is also a more advanced enemy type named "Boss Gunner", which alternates between shooting at and charging at the player.
