using System.Collections.Generic;

namespace JsonTypeDefinition
{
    /// <summary>
    /// Base implementation of a Json Type Definition schema as per Section 2 of RFC 8927 (see https://datatracker.ietf.org/doc/html/rfc8927#section-2).
    /// </summary>
    /// <remarks>
    /// If none of these properties is set this schema will equal the empty schema (see https://datatracker.ietf.org/doc/html/rfc8927#section-2.2.1).
    /// </remarks>
    public record JsonTypeDefinitionSchema
    {
        /// <summary>
        /// Used by ref schemas (see https://datatracker.ietf.org/doc/html/rfc8927#section-2.2.2).
        /// If set, the referenced definition must be provided in <see cref="RootSchema.Definitions"/>.
        /// </summary>
        public string? Ref { get; set; }

        /// <summary>
        /// Used by type schemas (see https://datatracker.ietf.org/doc/html/rfc8927#section-2.2.3).
        /// </summary>
        public JsonTypeDefinitionPrimitiveType? Type { get; set; }

        /// <summary>
        /// Used by enum schemas (see https://datatracker.ietf.org/doc/html/rfc8927#section-2.2.4).
        /// The enum values must be unique values as per string equality rules defined in https://datatracker.ietf.org/doc/html/rfc8259#section-8.3.
        /// </summary>
        public IEnumerable<string>? Enum { get; set; }

        /// <summary>
        /// Used by elements schemas (see https://datatracker.ietf.org/doc/html/rfc8927#section-2.2.5).
        /// Most .NET collection types are mapped to this schema. The property value contains the schema of the type contained by the collection.
        /// </summary>
        public JsonTypeDefinitionSchema? Elements { get; set; }

        /// <summary>
        /// Used by property schemas (see https://datatracker.ietf.org/doc/html/rfc8927#section-2.2.6).
        /// The key of every key-value-pair describes the property name. The appropriate value can be any other type of <see cref="JsonTypeDefinitionSchema"/>, 
        /// e.g. a string property should map to a type schema, whereas a collection property would map to an elements schema.
        /// As per definition objects that are validated against this schema must provide a value for each of these properties as they are not optional.
        /// </summary>
        public IDictionary<string, JsonTypeDefinitionSchema>? Properties { get; set; }

        /// <summary>
        /// Used by property schemas (see https://datatracker.ietf.org/doc/html/rfc8927#section-2.2.6).
        /// The key of every key-value-pair describes the property name. The appropriate value can be any other type of <see cref="JsonTypeDefinitionSchema"/>, 
        /// e.g. a string property should map to a type schema, whereas a collection property would map to an elements schema.
        /// As per definition objects that are validated against this schema can omit values for these properties as they are optional.
        /// </summary>
        public IDictionary<string, JsonTypeDefinitionSchema>? OptionalProperties { get; set; }

        /// <summary>
        /// Used by values schemas (see https://datatracker.ietf.org/doc/html/rfc8927#section-2.2.7).
        /// This schema is used specifically by <see cref="IDictionary{string, TValue}"/> where TValue is described by another <see cref="JsonTypeDefinitionSchema"/>.
        /// </summary>
        public JsonTypeDefinitionSchema? Values { get; set; }
    }
}
