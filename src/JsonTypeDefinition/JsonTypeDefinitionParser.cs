﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace JsonTypeDefinition
{
    /// <summary>
    /// Provides functionality to parse .NET types into RFC 8927 compliant Json Type Definitions.
    /// </summary>
    public static class JsonTypeDefinitionParser
    {
        private static readonly Dictionary<Type, JsonTypeDefinitionPrimitiveType> typeMapping = new()
        {
            { typeof(bool), JsonTypeDefinitionPrimitiveType.Boolean },
            { typeof(string), JsonTypeDefinitionPrimitiveType.String },
            { typeof(DateTime), JsonTypeDefinitionPrimitiveType.TimeStamp },
            { typeof(DateTimeOffset), JsonTypeDefinitionPrimitiveType.TimeStamp },
            { typeof(float), JsonTypeDefinitionPrimitiveType.Float32 },
            { typeof(double), JsonTypeDefinitionPrimitiveType.Float64 },
            { typeof(sbyte), JsonTypeDefinitionPrimitiveType.Int8 },
            { typeof(byte), JsonTypeDefinitionPrimitiveType.Uint8 },
            { typeof(short), JsonTypeDefinitionPrimitiveType.Int16 },
            { typeof(ushort), JsonTypeDefinitionPrimitiveType.Uint16 },
            { typeof(int), JsonTypeDefinitionPrimitiveType.Int32 },
            { typeof(uint), JsonTypeDefinitionPrimitiveType.Uint32 }
        };

        /// <summary>
        /// Creates a <see cref="RootSchema"/> from the provided <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type from which to derive the <see cref="RootSchema"/>.</param>
        /// <returns>The <see cref="RootSchema"/> representing the provided <see cref="Type"/>.</returns>
        /// <exception cref="JsonTypeDefinitionParserException">Generic exception that is thrown when the operation cannot finish properly.</exception>
        public static RootSchema Parse(Type type)
        {
            return ParseInternal(type);
        }

        /// <summary>
        /// Creates a <see cref="RootSchema"/> from the provided <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The type from which to derive the <see cref="RootSchema"/>.</typeparam>
        /// <returns>The <see cref="RootSchema"/> representing the provided <see cref="Type"/>.</returns>
        /// <exception cref="JsonTypeDefinitionParserException">Generic exception that is thrown when the operation cannot finish properly.</exception>
        public static RootSchema Parse<T>()
        {
            return ParseInternal(typeof(T));
        }

        private static RootSchema ParseInternal(Type type)
        {
            var definitions = new Dictionary<string, JsonTypeDefinitionSchema>();
            var schema = GetJsonTypeDefinitionSchemaByType(type, definitions, true);
            return new RootSchema
            {
                Definitions = definitions.Any() ? definitions : null,
                Type = schema.Type,
                Values = schema.Values,
                Elements = schema.Elements,
                Enum = schema.Enum,
                Ref = schema.Ref,
                Properties = schema.Properties,
                OptionalProperties = schema.OptionalProperties
            };
        }

        private static JsonTypeDefinitionSchema GetJsonTypeDefinitionSchemaByType(Type type, IDictionary<string, JsonTypeDefinitionSchema> definitions, bool isRoot)
        {
            if (type is null)
                throw new JsonTypeDefinitionParserException(new ArgumentNullException(nameof(type)));

            if (typeMapping.ContainsKey(Nullable.GetUnderlyingType(type) ?? type))
                return GetTypeSchema(type);
            else if (typeof(IDictionary<,>).IsAssignableFrom(type) && type.GenericTypeArguments[0] == typeof(string))
                return GetValuesSchema(type.GenericTypeArguments[1], definitions);
            else if (typeof(IEnumerable<>).IsAssignableFrom(type))
                return GetElementsSchema(type.GenericTypeArguments[0], definitions);
            else if (type.IsEnum)
                return GetEnumSchema(type);
            else if (type.IsClass)
                return GetRefSchema(type, definitions, isRoot);
            else
                throw new JsonTypeDefinitionParserException($"Unsupported type '{type.FullName}'.");
        }

        private static JsonTypeDefinitionSchema GetTypeSchema(Type type)
        {
            if (!typeMapping.TryGetValue(type, out var jsonType))
                throw new JsonTypeDefinitionParserException($"Unable to map type '{type.FullName}' to any {nameof(JsonTypeDefinitionPrimitiveType)}.");

            return new() { Type = jsonType };
        }

        private static JsonTypeDefinitionSchema GetValuesSchema(Type containedType, IDictionary<string, JsonTypeDefinitionSchema> definitions)
        {
            return new() { Values = GetJsonTypeDefinitionSchemaByType(containedType, definitions, false) };
        }

        private static JsonTypeDefinitionSchema GetElementsSchema(Type containedType, IDictionary<string, JsonTypeDefinitionSchema> definitions)
        {
            return new() { Elements = GetJsonTypeDefinitionSchemaByType(containedType, definitions, false) };
        }

        private static JsonTypeDefinitionSchema GetEnumSchema(Type type)
        {
            var enumValues = Enum.GetNames(type);
            if (!enumValues.Any())
                throw new JsonTypeDefinitionParserException($"Unable to parse enum '{type.FullName}': It has no elements.");

            var duplicateValues = enumValues.GroupBy(x => x.ToLower()).Where(x => x.Count() > 1).SelectMany(x => x);
            if (duplicateValues.Any())
                throw new JsonTypeDefinitionParserException($"Unable to parse enum '{type.FullName}': The values '{string.Join(";", duplicateValues)}' are not distinct.");

            return new() { Enum = enumValues };
        }

        private static JsonTypeDefinitionSchema GetRefSchema(Type type, IDictionary<string, JsonTypeDefinitionSchema> definitions, bool isRoot)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).AsEnumerable();

            var optionalProperties = properties.Where(x => x.GetCustomAttribute<RequiredAttribute>() is null);
            properties = properties.Except(optionalProperties);

            var schema = new JsonTypeDefinitionSchema()
            {
                Properties = properties.Any()
                    ? properties.ToDictionary(x => x.Name, x => GetJsonTypeDefinitionSchemaByType(x.PropertyType, definitions, false)) : null,
                OptionalProperties = optionalProperties.Any()
                    ? optionalProperties.ToDictionary(x => x.Name, x => GetJsonTypeDefinitionSchemaByType(x.PropertyType, definitions, false)) : null
            };

            if (isRoot)
                return schema;

            var refName = type.Name;
            // edge case: same simple type name but is actually a different type
            if (definitions.TryGetValue(type.Name, out var definition) && definition != schema)
                refName = type.FullName;

            if (!string.IsNullOrEmpty(refName) && !definitions.ContainsKey(refName))
                definitions[refName] = schema;
            return new() { Ref = refName };
        }
    }
}