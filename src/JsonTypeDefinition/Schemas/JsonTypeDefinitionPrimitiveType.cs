namespace JsonTypeDefinition
{
    /// <summary>
    /// Primitive types as defined in Section 2 of RFC 8927 (see https://datatracker.ietf.org/doc/html/rfc8927#section-2).
    /// </summary>
    public enum JsonTypeDefinitionPrimitiveType
    {
        /// <summary>
        /// Similar to <see cref="bool"/>.
        /// </summary>
        Boolean,

        /// <summary>
        /// Similar to <see cref="string"/>.
        /// </summary>
        String,

        /// <summary>
        /// Similar to <see cref="System.DateTimeOffset"/>.
        /// </summary>
        TimeStamp,

        /// <summary>
        /// Similar to <see cref="float"/>.
        /// </summary>
        Float32,

        /// <summary>
        /// Similar to <see cref="double"/>.
        /// </summary>
        Float64,

        /// <summary>
        /// Similar to <see cref="sbyte"/>.
        /// </summary>
        Int8,

        /// <summary>
        /// Similar to <see cref="byte"/>.
        /// </summary>
        Uint8,

        /// <summary>
        /// Similar to <see cref="short"/>.
        /// </summary>
        Int16,

        /// <summary>
        /// Similar to <see cref="ushort"/>.
        /// </summary>
        Uint16,

        /// <summary>
        /// Similar to <see cref="int"/>.
        /// </summary>
        Int32,

        /// <summary>
        /// Similar to <see cref="uint"/>.
        /// </summary>
        Uint32
    }
}
