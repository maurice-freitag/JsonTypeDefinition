using System;
using System.Runtime.Serialization;

namespace JsonTypeDefinition
{
    /// <summary>
    /// Base exception used by the <see cref="JsonTypeDefinitionParser"/> when the parsing operation could not finish properly.
    /// </summary>
    [Serializable]
    public class JsonTypeDefinitionParserException : InvalidOperationException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message containing the error reason.</param>
        public JsonTypeDefinitionParserException(string message)
            : base(message) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="innerException">The inner exception caught by the context.</param>
        public JsonTypeDefinitionParserException(Exception innerException)
            : base("An unexpected error occured, see inner exception.", innerException) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">For serialization support.</param>
        /// <param name="context">For serialization support.</param>
        protected JsonTypeDefinitionParserException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
