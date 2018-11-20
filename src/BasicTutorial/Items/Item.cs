using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicTutorial.Items
{
    class Item
    {
        public int HealthModifier;
        public int AttackModifier;
        public int DefenseModifier;
        public int VisibilityModifier;
        public int LightingModifier;

        public string Title;
        public InventorySpot Spot;

        public Cell Visual;

        public (ColoredString title, ColoredString attributes) GetColoredString()
        {
            StringBuilder modifierString = new StringBuilder(20);

            if (AttackModifier > 0)
                modifierString.Append($" [c:r f:InvGreen]A+{AttackModifier}");
            else if (AttackModifier < 0)
                modifierString.Append($" [c:r f:InvRed]A{AttackModifier}");

            if (DefenseModifier > 0)
                modifierString.Append($" [c:r f:InvGreen]D+{DefenseModifier}");
            else if (DefenseModifier < 0)
                modifierString.Append($" [c:r f:InvRed]D{DefenseModifier}");

            if (HealthModifier > 0)
                modifierString.Append($" [c:r f:InvGreen]H+{HealthModifier}");
            else if (HealthModifier < 0)
                modifierString.Append($" [c:r f:InvRed]H{HealthModifier}");

            if (LightingModifier > 0)
                modifierString.Append($" [c:r f:InvGreen]L+{LightingModifier}");
            else if (LightingModifier < 0)
                modifierString.Append($" [c:r f:InvRed]L{LightingModifier}");

            return (Title.CreateColored(), ColoredString.Parse(modifierString.ToString()));
        }
    }
}
