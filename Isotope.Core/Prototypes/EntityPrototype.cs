using System.Collections.Generic;

namespace Isotope.Core.Prototypes;

/// <summary>
/// Represents a prototype for an entity, defining its properties and components.
/// </summary>
public class EntityPrototype
{
    /// <summary>
    /// The unique identifier of the prototype.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The name of the prototype.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The list of component prototypes that make up this entity prototype.
    /// </summary>
    public List<ComponentPrototype> Components { get; set; } = new();
}
