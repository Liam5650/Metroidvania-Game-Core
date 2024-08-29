# Metroidvania Game Core

![DemoImage](https://github.com/Liam5650/Metroidvania-Game-Core/blob/main/READMEImages/DemoImage.jpg)

Metroidvania Game Core is a comprehensive game framework designed to provide an accessible way to create Metroidvania-style games in Unity. A web demo is [available on itch.io](https://liam5650.itch.io/metroidvania-demo) and highlights many of the framework's key features. 

## Features

This framework features a variety of essential Metroidvania gameplay mechanics, such as the ability to build complex maps with interconnected rooms and seamless transitions, ability-based progression to create more dynamic, non-linear gameplay, as well as a robust save system that tracks detailed game states and manages event triggers like boss battles. All features have been designed with ease-of-use in mind, making it quick and straightforward to effectively use the framework.

## Installation

Simply click on the green "Code" button at the top-right of the project's main page, and select "Download ZIP". Unzip the file, open Unity Hub and click "Add", then navigate to and select the project folder. If you are prompted that the project was made using a different version of Unity, you can download editor version 2021.3.14.f1 to ensure compatibility. Later editor versions are likely to work without issue as well, but have not been tested. 

## Usage

The map is structured so that individual rooms are made up of cells on a grid, as seen here:

![MapStructure](https://github.com/Liam5650/Metroidvania-Game-Core/blob/main/READMEImages/MapStructure.jpg)

Each cell measures 40 by 22.5 x/y units, which was chosen to fit a 16:9 aspect ratio that is common on many displays while also allowing for adequate space to add tiles (each tile measures 1x1). When creating rooms, you can right click on the scene titled "World Grid" and select "Open Scene Additive" to help make sure everything is aligned as expected.

To make a room, simply create a new scene and add the "CameraBounds" prefab from the "Camera" folder, and the "Interactable Grid" prefab from the "Environment folder. Some basic information about each prefab:

### CameraBounds

This prefab sets the bounds of the camera for the room. You can think of it as the active play area for the player. The size of the bounds by default is the same 40 x 22.5 units as one cell in the world grid. This allows for easy alignment of the bounds with the grid by adjusting the x and y position of its transform. To create a bigger room, you can simply set a multiple for the prefab's transform x or y scale. Here is an example use and explanation:

