# SadConsole Editor
An editor for creating consoles and animations used by SadConsole.

SadConsole Editor provides three types of editors: Consoles, Game Objects, and Scenes. The current code base has been updated for the latest [SadConsole](http://github.com/thraka/sadconsole) release.

A config file named `settings.json` is provided which controls how SadConsole Editor behaves. You can adjust which font file you're using (to match whatever font your game is going to use). the size of the program window (in characters), the default size of each editor's console, and the bounds of those.

The bounds setting is especially important. If the viewport you're going to use in your game is small, like 40x50, but the maps are going to be 200x200, you can set the default width/height of the console to 200x200 and configure the bounds to be 40x50. This means that every 40x50 square in your map will be shown with a light-gray dotted line in the editor.

## Console editor
This is your basic editor. It loads and saves straight SadConsole LayeredConsole objects. It uses the basic tool set:

##### Pencil
The basic drawing tool. Places the selected color-character combination. You can also mirror the character vertically or horizontally.
- `Left-click` to places the character and color combination.  
- `Right-click` to change the pencil to whatever is underneath the mouse.
- The [character bar](#character-bar) is active with this tool.

##### Recolor
It operates just like the pencil tool but does not change the character of a cell. You can also choose to have the recolor ignore the foreground or background colors.
- `Left-click` to recolors the cell.  
- `Right-click` to change the selected colors to what is under the mouse.

##### Fill
Fills an area of the console with the specified character and color combination. Works exactly like the pencil tool except fills an area of the console.
- The [character bar](#character-bar) is active with this tool.

##### Text
Allows you to type text into the console using the selected foreground and background colors.
- `Left-click` to move the cursor and start typing. 
- `ESC` to stop typing.

##### Selection
Selects a portion of the console and then allows you to move, clone, or clear the selection. You can optionally export the selection into a brush to be imported later. 
- `Left-click` **once** to choose the first corner of the selection, then click again when you have finished selecting the area.
- `Right-click` to clear the selection.

##### Line
Draws a line using the specified character and color combination. Works exactly like the pencil tool except draws a line.
- `Left-click` **once** to start drawing the line, then click again when you have finished placing the line.
- `Right-click` to cancel drawing the line.
- The [character bar](#character-bar) is active with this tool.
  
##### Box
Draws a box using the specified foreground and background. The box is drawn using characters to connect corners and edges. You can check `Char. Border` to change the line characters to a single character of your choice.
- `Left-click` **once** to choose the first corner of the box, then click again when you have finished drawing the box.
- `Right-click` to cancel drawing the box.

##### Circle
Draws a line using the specified character and color combination. Works exactly like the pencil tool except draws a circle.
- `Left-click` **once** to choose the first edge of the circle, then click again when you have finished drawing the circle.
- `Right-click` to cancel drawing the circle.
- The [character bar](#character-bar) is active with this tool.


## Scene editor

The Scene editor allows you to define hidden objects that your code can respond to, zones and hotspots. You can also place game objects in the scene. 

##### Object yool
Places the selected object on the selected layer. The list of objects are templates. Meaning, after you place them, you can change the placed item which does not change the template or other objects placed with that template.
- `Left-click` to place the selected template.
- `Right-click` to edit the object under the mouse.
- `SHIFT-left-click` to copy the object under the mouse. This allows you to clone customized objects in the console without having to edit templates.
- `CTRL-left-click` to delete the object under the mouse.



## Game object editor

The game object editor allows you to create entity objects which are animated consoles.

.. more to come ..


## Character bar
The character bar is enabled with specific tools. It allows you quick access to a specific character by pressing the `F1-F12` keys. You can cycle sets of characters by pressing `SHIFT+UP` or `SHIFT+DOWN`. Characters are defined in the **quickselect.json** config file.

This file consists of a JSON array of character-code arrays. For example, this defines two character sets:

    [[176,177,178,219,223,220,221,222,254,250,249,0],[218,191,192,217,196,179,195,180,193,194,197,0]]

The three character sets in this example are:

- [176,177,178,219,223,220,221,222,254,250,249,0]
- [218,191,192,217,196,179,195,180,193,194,197,0]

Each character set is an array of character codes from the font sheets. You must define exactly twelve character codes per character set.

These items map to the function keys of your keyboard like so:

- [F1,F2,F3,F4,F5,F6,F7,F8,F9,F10,F11,F12]