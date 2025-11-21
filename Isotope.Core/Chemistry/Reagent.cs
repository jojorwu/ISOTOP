using Raylib_cs;
using System.Collections.Generic;

namespace Isotope.Core.Chemistry
{
    public class Reagent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public string Description { get; set; }
    }

    public static class ReagentRegistry
    {
        public static Dictionary<string, Reagent> Reagents = new();

        public static void RegisterDefaults()
        {
            Reagents["water"] = new Reagent { Id="water", Name="Water", Color=Color.Blue };
            Reagents["fuel"] = new Reagent { Id="fuel", Name="Welding Fuel", Color=Color.Orange };
            Reagents["blood"] = new Reagent { Id="blood", Name="Blood", Color=Color.Red };
        }
    }
}
