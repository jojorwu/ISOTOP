using System.Collections.Generic;

namespace Isotope.Core.Prototypes;

public class EntityPrototype
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<ComponentPrototype> Components { get; set; } = new();
}
