using Arch.Core;
using Arch.System;
using System;
using System.Threading.Tasks;

namespace Isotope.Core.Systems;

/// <summary>
/// An abstract base class for systems that process entities in parallel.
/// </summary>
public abstract class ParallelSystem : BaseSystem<World, float>
{
    private readonly QueryDescription _queryDesc;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParallelSystem"/> class.
    /// </summary>
    /// <param name="world">The game world.</param>
    /// <param name="desc">The query description for the entities to be processed.</param>
    protected ParallelSystem(World world, QueryDescription desc) : base(world)
    {
        _queryDesc = desc;
    }

    /// <summary>
    /// Updates a chunk of entities.
    /// </summary>
    /// <param name="chunk">The chunk to update.</param>
    /// <param name="deltaTime">The time since the last update.</param>
    protected abstract void UpdateChunk(ref Chunk chunk, in float deltaTime);

    /// <summary>
    /// Updates the system, processing all matching entities in parallel.
    /// </summary>
    /// <param name="deltaTime">The time since the last update.</param>
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