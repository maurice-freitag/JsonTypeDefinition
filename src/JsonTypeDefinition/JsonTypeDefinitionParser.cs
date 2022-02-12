using System;

namespace JsonTypeDefinition
{
    /// <summary>
    /// Provides functionality to parse .NET types into RFC 8927 compliant Json Type Definitions.
    /// </summary>
    public static class JsonTypeDefinitionParser
    {
        /// <summary>
        /// Creates a <see cref="RootSchema"/> from the provided <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type from which to derive the <see cref="RootSchema"/>.</param>
        /// <returns>The <see cref="RootSchema"/> representing the provided <see cref="Type"/>.</returns>
        /// <exception cref="JsonTypeDefinitionParserException">Generic exception that is thrown when the operation cannot finish properly.</exception>
        public static RootSchema Parse(Type type)
        {
            throw new JsonTypeDefinitionParserException("TODO");
        }

        /// <summary>
        /// Creates a <see cref="RootSchema"/> from the provided <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The type from which to derive the <see cref="RootSchema"/>.</typeparam>

        /// <returns>The <see cref="RootSchema"/> representing the provided <see cref="Type"/>.</returns>
        /// <exception cref="JsonTypeDefinitionParserException">Generic exception that is thrown w
        public static RootSchema Parse<T>()
        {
            throw new JsonTypeDefinitionParserException("TODO");
        }
    }
}
