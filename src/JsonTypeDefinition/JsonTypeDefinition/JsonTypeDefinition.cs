using System.Collections.Generic;

namespace JsonTypeDefinition
{
    public abstract class JTDSchema
    {
        public string? Name { get; set; }
    }

    public enum JTDPrimitiveType
    {
        Boolean,
        String,
        TimeStamp,
        Float32,
        Float64,
        Int8,
        Uint8,
        Int16,
        Uint16,
        Int32,
        Uint32
    }

    public class JTDTypePropertyDefinition : JTDSchema
    {
        public JTDPrimitiveType Type { get; set; }
    }

    public class JTDEnumPropertyDefinition : JTDSchema
    {
        public IEnumerable<string>? Values { get; set; }
    }

    public class JTDElementsPropertyDefinition : JTDSchema
    {
        public IEnumerable<JTDTypePropertyDefinition>? Properties { get; set; }
    }

    public class JTDTypeDefinition
    {
        public IEnumerable<JTDSchema>? Properties { get; set; }
    }
}
