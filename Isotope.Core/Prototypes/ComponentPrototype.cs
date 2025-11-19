using System.Collections.Generic;

namespace Isotope.Core.Prototypes;

/// <summary>
/// Represents a prototype for a component, defining its type and values.
/// </summary>
public class ComponentPrototype
{
    /// <summary>
    /// The type of the component.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The values of the component's properties.
    /// </summary>
    public Dictionary<string, object> Values { get; set; } = new();
}
