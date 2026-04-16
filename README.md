## The Force Unleashed Sandbox Mod<br/>

**Right now this requires the removal of steam drm from the game executable via [steamless](https://github.com/atom0s/Steamless) to function**<br/>
Allows for the creation of mods that replace files in LevelPacks with loose files. This is a Reloaded II mod and thus requires it to function, it can be found [here](https://reloaded-project.github.io/Reloaded-II/)<br/>

**Features**<br/>
Loading of loose files allowing replacing files with larger ones<br/>
Enabling costumes/lightsaber customisation/mission selection during the prologue*<br/>
Logging file accesses*<br/>
Allowing selection of prologue from mission selection*<br/>
Enabling full moveset during prologue*<br/>

Optional changes denotes with *<br/>
<br/>

**Planned**<br/>
Loading of new non-replacment files<br/>
Exposing Lua functions<br/>
Allowing saving after using cheats<br/>
<br/>

**Mod Creation**<br/>
This assumes you know how to create a basic Reloaded II mod already.<br/>

Create a folder named "Sandbox" in your mod folder and add your files as if they are in the games directory.
Ex. "YourMod/Sandbox/Scum/runtime/prefs/TFUDisk.costumecat.xml"
File merging is a little beyond the scope of this so the load order determines which file will load if there is a conflict