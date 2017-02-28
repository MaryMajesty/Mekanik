# Mekanik

Mekanik is a 2D video game engine that utilizes the .NET-Framework.

Features:
* An intuitive entity and graphics system, as position, rotation, color and scale automatically get applied to every sub-entity and sub-graphic, just as you would expect.
* An easy control system with local multiplayer and gamepad capabilities and support for custom remapping.
* A level system complete with an automatically integrated level editor that works out of the box. Simply tell it what types of entities there are in your game, and you can automatically put them in your levels and edit their properties.
* Support for settings and multiple save states, including automated saving of a level's current state.
* Full debug functionality: You can test how fast your game can possibly run, how many entities of which type there currently are, how many resources have been created that haven't been disposed of yet, and if there are memory leaks, you can run an automated memory leak diagnosis that checks which entities are causing the leak.
* Mekanik prioritizes actual coding, as opposed to game engines that are shipped with a full IDE. There are no fancy code or entity editors or anything like that aside from the level editor, because let's be honest, having to create levels without an editor would not exactly be user friendly.
* Scripting support using the scripting language [Zero](https://github.com/TodesBrot/Zero). Scripts can be edited from within the level editor to give entities custom behaviors.
* It comes with an exporter that can export your game into a single exe file. It makes everything easier, especially for those people who don't know how to unzip a zip file. Yes, those exist. Sadly.
* Using FarseerPhysics, you can add colliders to entities to create physics-based interactions. Levels automatically create their own colliders so you can't walk through walls.
* You can easily add online functionality to your game, as Mekanik comes with built in features for online connecting and data synchronization. By default, the state of each player's controls gets sent automatically, so with very little code you could easily get a rudimentary online mode running.
* Mekanik also has lots of built in interface entities, such as textboxes, tabs, file menus, buttons, collapsible groups, reorderable lists, and more. There's even an entity for editing any type of data at all.

Documentation is not available yet, so if you're interested in using Mekanik, just send me a message.
