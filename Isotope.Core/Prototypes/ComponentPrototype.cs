using System.Collections.Generic;

namespace Isotope.Core.Prototypes;

public class ComponentPrototype
{
    public string Type { get; set; }
    public Dictionary<string, object> Values { get; set; } = new();
}
