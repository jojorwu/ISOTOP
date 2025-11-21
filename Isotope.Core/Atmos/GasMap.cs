using System;

namespace Isotope.Core.Atmos
{
    public class GasMap
    {
        public int Width { get; }
        public int Height { get; }

        public float[] Oxygen;
        public float[] Nitrogen;
        public float[] Plasma;
        public float[] CO2;

        public float[] Temperature;

        public GasMap(int w, int h)
        {
            Width = w; Height = h;
            int size = w * h;

            Oxygen = new float[size];
            Nitrogen = new float[size];
            Plasma = new float[size];
            CO2 = new float[size];
            Temperature = new float[size];

            Array.Fill(Oxygen, 20f);
            Array.Fill(Nitrogen, 80f);
            Array.Fill(Temperature, 293.15f);
        }

        public float GetPressure(int i)
        {
            return Oxygen[i] + Nitrogen[i] + Plasma[i] + CO2[i];
        }
    }
}
