using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Renderers.Constants;
using SadRogue.Primitives;

namespace SadConsole.Tests.Extended;

[TestClass]
public class EntityManagerZoned
{
    [TestInitialize]
    public void SetupHost()
    {
        new BasicGameHost();
    }

    [TestMethod]
    public void Create_Remove()
    {
        Entities.EntityManagerZoned manager = new();
        SadConsole.ScreenSurface surfaceObject = new(20, 20);

        surfaceObject.SadComponents.Add(manager);

        Assert.IsNotNull(surfaceObject.Renderer.Steps.Where(r => r.Name == RenderStepNames.EntityManager).FirstOrDefault());

        surfaceObject.SadComponents.Remove(manager);

        Assert.IsNull(surfaceObject.Renderer.Steps.Where(r => r.Name == RenderStepNames.EntityManager).FirstOrDefault());
    }

    [TestMethod]
    public void AddAndRemove_Entity()
    {
        Entities.EntityManagerZoned manager = new();
        SadConsole.ScreenSurface surfaceObject = new(20, 20);

        surfaceObject.SadComponents.Add(manager);

        SadConsole.Entities.Entity ent1 = new Entities.Entity(Color.Blue, Color.AliceBlue, 1, 0);
        SadConsole.Entities.Entity ent2 = new Entities.Entity(AnimatedScreenObject.CreateStatic(4, 4, 2, 0.5d), 0);

        manager.Add(ent1);
        manager.Add(ent2);

        Assert.IsTrue(manager.Contains(ent1));
        Assert.IsTrue(manager.Contains(ent2));

        manager.Remove(ent1);
        manager.Remove(ent2);

        Assert.IsFalse(manager.Contains(ent1));
        Assert.IsFalse(manager.Contains(ent2));
    }

    [TestMethod]
    public void AddAndRemove_Zone()
    {
        Entities.EntityManagerZoned manager = new();
        SadConsole.ScreenSurface surfaceObject = new(20, 20);

        surfaceObject.SadComponents.Add(manager);

        SadConsole.Entities.Zone zone1 = new Entities.Zone(new Area(new Rectangle(2, 2, 4, 4).Positions()));

        manager.Add(zone1);

        Assert.IsTrue(manager.Contains(zone1));

        manager.Remove(zone1);

        Assert.IsFalse(manager.Contains(zone1));
    }

    [TestMethod]
    public void Entity_MoveEnterZone()
    {
        Entities.EntityManagerZoned manager = new();
        SadConsole.ScreenSurface surfaceObject = new(20, 20);

        surfaceObject.SadComponents.Add(manager);

        SadConsole.Entities.Zone zone1 = new Entities.Zone(new Area(new Rectangle(2, 2, 4, 4).Positions()));
        SadConsole.Entities.Entity ent1 = new Entities.Entity(Color.Blue, Color.AliceBlue, 1, 0);

        manager.Add(zone1);
        manager.Add(ent1);

        bool enterFlag = false;
        Point targetPosition = (2, 2);

        manager.EnterZone += (s, e) =>
        {
            enterFlag = true;
            Assert.AreEqual(zone1, e.Zone);
            Assert.AreEqual(ent1, e.Entity);
        };

        Assert.IsFalse(zone1.Entities.Contains(ent1));

        Assert.IsFalse(enterFlag);

        ent1.Position = targetPosition;

        Assert.IsTrue(zone1.Entities.Contains(ent1));

        Assert.IsTrue(enterFlag);
    }

    public void Entity_MoveInsideZone()
    {

    }

    [TestMethod]
    public void EnableDisable()
    {
        Entities.EntityManagerZoned manager = new();
        SadConsole.ScreenSurface surfaceObject = new(20, 20);

        surfaceObject.SadComponents.Add(manager);

        SadConsole.Entities.Zone zone1 = new Entities.Zone(new Area(new Rectangle(2, 2, 4, 4).Positions()));
        SadConsole.Entities.Entity ent1 = new Entities.Entity(Color.Blue, Color.AliceBlue, 1, 0);

        
    }

    [TestMethod]
    public void Entity_MoveExitZone()
    {
        Entities.EntityManagerZoned manager = new();
        SadConsole.ScreenSurface surfaceObject = new(20, 20);

        surfaceObject.SadComponents.Add(manager);

        SadConsole.Entities.Zone zone1 = new Entities.Zone(new Area(new Rectangle(2, 2, 4, 4).Positions()));
        SadConsole.Entities.Entity ent1 = new Entities.Entity(Color.Blue, Color.AliceBlue, 1, 0);

        Point sourcePosition = (2, 2);
        Point targetPosition = (0, 0);

        ent1.Position = sourcePosition;

        manager.Add(zone1);
        manager.Add(ent1);

        bool exitFlag = false;
        
        manager.ExitZone += (s, e) =>
        {
            exitFlag = true;
            Assert.AreEqual(zone1, e.Zone);
            Assert.AreEqual(ent1, e.Entity);
        };

        Assert.IsTrue(zone1.Entities.Contains(ent1));

        Assert.IsFalse(exitFlag);

        ent1.Position = targetPosition;

        Assert.IsFalse(zone1.Entities.Contains(ent1));

        Assert.IsTrue(exitFlag);
    }

    [TestMethod]
    public void Entity_InsertDeleteTriggerZone()
    {
        Entities.EntityManagerZoned manager = new();
        SadConsole.ScreenSurface surfaceObject = new(20, 20);

        surfaceObject.SadComponents.Add(manager);

        SadConsole.Entities.Zone zone1 = new Entities.Zone(new Area(new Rectangle(2, 2, 4, 4).Positions()));
        SadConsole.Entities.Entity ent1 = new Entities.Entity(Color.Blue, Color.AliceBlue, 1, 0);
        ent1.Position = (2, 2);

        manager.Add(zone1);

        bool enterFlag = false;
        bool exitFlag = false;

        manager.EnterZone += (s, e) =>
        {
            enterFlag = true;
            Assert.AreEqual(zone1, e.Zone);
            Assert.AreEqual(ent1, e.Entity);
        };

        manager.ExitZone += (s, e) =>
        {
            exitFlag = true;
            Assert.AreEqual(zone1, e.Zone);
            Assert.AreEqual(ent1, e.Entity);
        };

        // Add entity
        Assert.IsFalse(enterFlag);
        Assert.IsFalse(zone1.Entities.Contains(ent1));
        manager.Add(ent1);
        Assert.IsTrue(zone1.Entities.Contains(ent1));
        Assert.IsTrue(enterFlag);

        // Remove entity
        Assert.IsFalse(exitFlag);
        manager.Remove(ent1);
        Assert.IsFalse(zone1.Entities.Contains(ent1));
        Assert.IsTrue(exitFlag);
    }
}
