# Part 2 - Character and map

In this article we'll create objects that represent the player, monsters, and walls. We'll also handle keyboard input to move our player character around the screen. 

##### Tutorial index

- [Part 1 - Create the project]
- **Part 2 - Character and map**
- [Part 3 - Random maps]
- More coming soon...

## Create an entity

In a lot of text engines like SadConsole where you want to move characters around a console, you get some additional overhead. Normally you print the character on the console. Then when it moves, you erase it, and draw it in the new position. After that, you also have to make sure to restore whatever was under the original spot of the character.

SadConsole can avoid that overhead by using a sort of mini-console. The `SadConsole.GameHelpers.GameObject` type provides a positionable console-like object that can also be animated. This type doesn't have a virtual cursor or some of those other console specific things. The `GameObject` is used when you have a bunch of moving objects that are layered on top of a background.

We'll create a class named `EntityBase` that represents things placed on the map. Right-click on the project file and choose **New** > **Add Folder** and name it `Entities`. Right-click on the new folder and choose **New** > **Class...** and name the class `EntityBase`. In this class we'll inherit from `SadConsole.GameHelpers.GameObject`. This will represent the base class for monsters, the player, things like that.

```csharp
using Microsoft.Xna.Framework;

namespace SadRogueSharp.Entities
{
    public class EntityBase: SadConsole.GameHelpers.GameObject
    {
        public EntityBase(Color foreground, Color background, int glyph) : base()
        {
            Animation = new SadConsole.Surfaces.AnimatedSurface("default", 1, 1);
            var frame = Animation.CreateFrame();
            frame[0].Foreground = foreground;
            frame[0].Background = background;
            frame[0].Glyph = glyph;
        }

        public void MoveBy(Point change)
        {
            Position += change;
        }
    }
}
```

If you noticed, there is a `MoveBy` method on the `EntityBase` class. This will adjust the position of the entity by the amount provided. It will be easy to move something X or Y spaces on the map.

Next, add another class to the `Entities` folder named `Player.cs`. This class will represent the character the player controls in your game.

```csharp
using Microsoft.Xna.Framework;

namespace SadRogueSharp.Entities
{
    class Player : EntityBase
    {
        public Player(): base(Color.Yellow, Color.Black, '@')
        {
        }
    }
}
```

## Create a map

Now that we have a player we can display in the game we need to create a class to hold information about the game map.

First we need to create the console that will hold the map and player. Create another folder in the project named `Consoles` and add a new class named `Map.cs`. This console will be the visual presentor of the map.

```csharp
using Microsoft.Xna.Framework;

namespace SadRogue.Consoles
{
    class Map : SadConsole.Console
    {
        public Map(int width, int height): base(new SadConsole.Surfaces.BasicSurface(width, height, new Rectangle(0, 0, Program.ScreenWidth, Program.ScreenHeight)))
        {

        }
    }
}
```

This class takes a width and height that representing the size of the map. In the constructor we create the backing surface of the console at that width and height, but we restrict the viewing size (with a `Rectangle`) to the size of the screen.

