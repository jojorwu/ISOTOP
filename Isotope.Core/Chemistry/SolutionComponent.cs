using System.Collections.Generic;
using System.Linq;
using Raylib_cs;

namespace Isotope.Core.Chemistry
{
    public class SolutionComponent
    {
        public float MaxVolume { get; set; } = 100.0f;
        public float CurrentVolume => Contents.Values.Sum();
        public float Temperature { get; set; } = 293.15f;

        public Dictionary<string, float> Contents { get; set; } = new();

        public void AddReagent(string id, float amount)
        {
            if (!Contents.ContainsKey(id)) Contents[id] = 0;
            Contents[id] += amount;
        }

        public Color GetColor()
        {
            if (CurrentVolume <= 0) return Color.White;

            float r=0, g=0, b=0;
            foreach(var kvp in Contents)
            {
                var color = ReagentRegistry.Reagents[kvp.Key].Color;
                float share = kvp.Value / CurrentVolume;
                r += color.R * share;
                g += color.G * share;
                b += color.B * share;
            }
            return new Color((byte)r, (byte)g, (byte)b, 255);
        }
    }
}
