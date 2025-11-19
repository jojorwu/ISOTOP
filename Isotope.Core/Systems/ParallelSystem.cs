using Arch.Core;
using Arch.System;
using System;
using System.Threading.Tasks;

namespace Isotope.Core.Systems;

public abstract class ParallelSystem : BaseSystem<World, float>
{
    private readonly QueryDescription _queryDesc;

    protected ParallelSystem(World world, QueryDescription desc) : base(world)
    {
        _queryDesc = desc;
    }

    protected abstract void UpdateChunk(ref Chunk chunk, in float deltaTime);

    public override void Update(in float deltaTime)
    {
        World.InlineParallelQuery(in _queryDesc, new ForEach(this, deltaTime));
    }

    private struct ForEach : IChunkJob
    {
        private readonly ParallelSystem _system;
        private readonly float _deltaTime;

        public ForEach(ParallelSystem system, float deltaTime)
        {
            _system = system;
            _deltaTime = deltaTime;
        }

        public void Execute(ref Chunk chunk)
        {
            _system.UpdateChunk(ref chunk, in _deltaTime);
        }
    }
}