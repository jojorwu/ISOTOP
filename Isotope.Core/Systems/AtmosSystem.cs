using System.Threading.Tasks;
using Isotope.Core.Atmos;
using Isotope.Core.Map;
using Isotope.Core.Tiles;

namespace Isotope.Core.Systems
{
    public class AtmosSystem
    {
        private GasMap _gasMap;
        private WorldMap _tileMap;

        public AtmosSystem(GasMap gasMap, WorldMap tileMap)
        {
            _gasMap = gasMap;
            _tileMap = tileMap;
        }

        public void Update(float dt)
        {
            Parallel.For(0, _gasMap.Height, y =>
            {
                for (int x = 0; x < _gasMap.Width; x++)
                {
                    ProcessTile(x, y, dt);
                }
            });
        }

        private void ProcessTile(int x, int y, float dt)
        {
            int i = y * _gasMap.Width + x;

            var tileDef = _tileMap.GetTileDefForPhysics(x,y);
            if (tileDef != null && tileDef.IsSolid) return;

            float totalPressure = _gasMap.GetPressure(i);
            if (totalPressure < 0.1f) return;

            ProcessNeighbor(i, x + 1, y, totalPressure);
            ProcessNeighbor(i, x - 1, y, totalPressure);
            ProcessNeighbor(i, x, y + 1, totalPressure);
            ProcessNeighbor(i, x, y - 1, totalPressure);
        }

        private void ProcessNeighbor(int myIdx, int nx, int ny, float myPressure)
        {
            if (nx < 0 || nx >= _gasMap.Width || ny < 0 || ny >= _gasMap.Height) return;

            int nIdx = ny * _gasMap.Width + nx;

            var tileDef = _tileMap.GetTileDefForPhysics(nx,ny);
            if (tileDef != null && tileDef.IsSolid) return;

            float neighborPressure = _gasMap.GetPressure(nIdx);

            if (myPressure > neighborPressure)
            {
                float difference = myPressure - neighborPressure;
                float flow = difference * 0.1f;

                if (myPressure > 0)
                {
                    float shareO2 = _gasMap.Oxygen[myIdx] / myPressure;
                    float shareN2 = _gasMap.Nitrogen[myIdx] / myPressure;

                    _gasMap.Oxygen[myIdx] -= flow * shareO2;
                    _gasMap.Nitrogen[myIdx] -= flow * shareN2;

                    _gasMap.Oxygen[nIdx] += flow * shareO2;
                    _gasMap.Nitrogen[nIdx] += flow * shareN2;
                }
            }
        }
    }
}
