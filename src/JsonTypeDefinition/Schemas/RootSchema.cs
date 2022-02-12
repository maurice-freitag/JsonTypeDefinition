using System.Collections.Generic;

namespace JsonTypeDefinition
{
    /// <summary>
    /// The root schema that any given .NET type will be mapped to. Additionally to being able to take on the form of any given <see cref="JsonTypeDefinitionSchema"/>,
    /// the root schema contains <see cref="Definitions"/> that can be referenced by ref schemas contained by this type.
    /// See 'Root vs. Non-root Schemas': https://datatracker.ietf.org/doc/html/rfc8927#section-2.1
    /// </summary>
    public record RootSchema : JsonTypeDefinitionSchema
    {
        /// <summary>
        /// Used only by root schema (see https://datatracker.ietf.org/doc/html/rfc8927#section-2.1).
        /// Schemas contained by the root schema can reference these definitions. In practice properties that reference complex .NET types will be added to the definition collection.
        /// </summary>
        public IDictionary<string, JsonTypeDefinitionSchema>? Definitions { get; set; }
    }
}
