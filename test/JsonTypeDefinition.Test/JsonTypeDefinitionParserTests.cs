using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Xunit;
using J = Jtd.Jtd;

namespace JsonTypeDefinition.Test
{
    public class JsonTypeDefinitionParserTests
    {
        private static void CheckForRFC8927Compliancy(RootSchema schema)
        {
            var json = JsonConvert.SerializeObject(schema);
            var ex = Record.Exception(() =>
            {
                var jSchema = JsonConvert.DeserializeObject<J.Schema>(json);
                jSchema.Verify();
            });
            Assert.Null(ex?.Message);
        }

        [Fact]
        [Trait("Category", "Parse")]
        public void NullParameter_Throws()
        {
            Assert.Throws<JsonTypeDefinitionParserException>(() => JsonTypeDefinitionParser.Parse(null));
        }

        private class EmptySchemaType { }

        [Fact]
        [Trait("Category", "Parse")]
        public void EmptySchema_ParseType()
        {
            var schema = JsonTypeDefinitionParser.Parse(typeof(EmptySchemaType));
            AssertEmptySchema(schema);
        }

        [Fact]
        [Trait("Category", "Parse")]
        public void EmptySchema_ParseT()
        {
            var schema = JsonTypeDefinitionParser.Parse<EmptySchemaType>();
            AssertEmptySchema(schema);
        }

        private static void AssertEmptySchema(RootSchema schema)
        {
            Assert.Null(schema.Type);
            Assert.Null(schema.Values);
            Assert.Null(schema.Enum);
            Assert.Null(schema.Definitions);
            Assert.Null(schema.Ref);
            Assert.Null(schema.Elements);
            Assert.Null(schema.OptionalProperties);
            Assert.Null(schema.Properties);
        }

        [Fact]
        [Trait("Category", "RFC Compliance")]
        public void EmptySchema_RFC8927Compliant()
        {
            var schema = JsonTypeDefinitionParser.Parse(typeof(EmptySchemaType));
            CheckForRFC8927Compliancy(schema);
        }

        [Theory]
        [Trait("Category", "Parse")]
        [InlineData(typeof(bool), JsonTypeDefinitionPrimitiveType.Boolean)]
        [InlineData(typeof(string), JsonTypeDefinitionPrimitiveType.String)]
        [InlineData(typeof(DateTime), JsonTypeDefinitionPrimitiveType.TimeStamp)]
        [InlineData(typeof(DateTimeOffset), JsonTypeDefinitionPrimitiveType.TimeStamp)]
        [InlineData(typeof(float), JsonTypeDefinitionPrimitiveType.Float32)]
        [InlineData(typeof(double), JsonTypeDefinitionPrimitiveType.Float64)]
        [InlineData(typeof(sbyte), JsonTypeDefinitionPrimitiveType.Int8)]
        [InlineData(typeof(short), JsonTypeDefinitionPrimitiveType.Int16)]
        [InlineData(typeof(int), JsonTypeDefinitionPrimitiveType.Int32)]
        [InlineData(typeof(byte), JsonTypeDefinitionPrimitiveType.Uint8)]
        [InlineData(typeof(ushort), JsonTypeDefinitionPrimitiveType.Uint16)]
        [InlineData(typeof(uint), JsonTypeDefinitionPrimitiveType.Uint32)]
        public void PrimitiveTypeSchemas_ParseType(Type src, JsonTypeDefinitionPrimitiveType dest)
        {
            var schema = JsonTypeDefinitionParser.Parse(src);
            AssertTypeSchema(dest, schema);
        }

        [Theory]
        [Trait("Category", "Parse")]
        [InlineData(typeof(bool), JsonTypeDefinitionPrimitiveType.Boolean)]
        [InlineData(typeof(string), JsonTypeDefinitionPrimitiveType.String)]
        [InlineData(typeof(DateTime), JsonTypeDefinitionPrimitiveType.TimeStamp)]
        [InlineData(typeof(DateTimeOffset), JsonTypeDefinitionPrimitiveType.TimeStamp)]
        [InlineData(typeof(float), JsonTypeDefinitionPrimitiveType.Float32)]
        [InlineData(typeof(double), JsonTypeDefinitionPrimitiveType.Float64)]
        [InlineData(typeof(sbyte), JsonTypeDefinitionPrimitiveType.Int8)]
        [InlineData(typeof(short), JsonTypeDefinitionPrimitiveType.Int16)]
        [InlineData(typeof(int), JsonTypeDefinitionPrimitiveType.Int32)]
        [InlineData(typeof(byte), JsonTypeDefinitionPrimitiveType.Uint8)]
        [InlineData(typeof(ushort), JsonTypeDefinitionPrimitiveType.Uint16)]
        [InlineData(typeof(uint), JsonTypeDefinitionPrimitiveType.Uint32)]
        public void PrimitiveTypeSchemas_ParseT(Type src, JsonTypeDefinitionPrimitiveType dest)
        {
            var methodInfo = typeof(JsonTypeDefinitionParser).GetMethod(
                nameof(JsonTypeDefinitionParser.Parse),
                genericParameterCount: 1,
                types: Array.Empty<Type>()) ?? throw new MissingMethodException();
            if (methodInfo.MakeGenericMethod(src).Invoke(null, null) is not RootSchema schema)
                throw new InvalidOperationException();
            AssertTypeSchema(dest, schema);
        }

        private static void AssertTypeSchema(JsonTypeDefinitionPrimitiveType expected, RootSchema schema)
        {
            Assert.Null(schema.Values);
            Assert.Null(schema.Enum);
            Assert.Null(schema.Definitions);
            Assert.Null(schema.Ref);
            Assert.Null(schema.Elements);
            Assert.Null(schema.OptionalProperties);
            Assert.Null(schema.Properties);
            Assert.Equal(expected, schema.Type);
        }

        [Theory]
        [Trait("Category", "RFC Compliance")]
        [InlineData(typeof(bool))]
        [InlineData(typeof(string))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(DateTimeOffset))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(short))]
        [InlineData(typeof(int))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(uint))]
        public void PrimitiveTypeSchemas_RFC8927Compliant(Type src)
        {
            var schema = JsonTypeDefinitionParser.Parse(src);
            CheckForRFC8927Compliancy(schema);
        }

        private class RootType { public RefSchemaType ContainedType { get; set; } = default; }

        private class RefSchemaType { public string InnerProperty { get; set; } }

        [Fact]
        [Trait("Category", "Parse")]
        public void RefSchema_ParseType()
        {
            var schema = JsonTypeDefinitionParser.Parse(typeof(RootType));
            AssertRefSchema(schema);
        }

        [Fact]
        [Trait("Category", "Parse")]
        public void RefSchema_ParseT()
        {
            var schema = JsonTypeDefinitionParser.Parse<RootType>();
            AssertRefSchema(schema);
        }

        [Fact]
        [Trait("Category", "RFC Compliance")]
        public void RefSchema_RFC8927Compliant()
        {
            var schema = JsonTypeDefinitionParser.Parse(typeof(RootType));
            CheckForRFC8927Compliancy(schema);
        }

        private static void AssertRefSchema(RootSchema schema)
        {
            Assert.Null(schema.Type);
            Assert.Null(schema.Values);
            Assert.Null(schema.Enum);
            Assert.Null(schema.Ref);
            Assert.Null(schema.Elements);
            Assert.Null(schema.Properties);
            Assert.NotNull(schema.Definitions);
            Assert.NotNull(schema.OptionalProperties);

            var schemaProperty = Assert.Single(schema.OptionalProperties);
            Assert.Equal(nameof(RootType.ContainedType), schemaProperty.Key);
            Assert.Null(schemaProperty.Value.Type);
            Assert.Null(schemaProperty.Value.Values);
            Assert.Null(schemaProperty.Value.Enum);
            Assert.Null(schemaProperty.Value.Elements);
            Assert.Null(schemaProperty.Value.OptionalProperties);
            Assert.Null(schemaProperty.Value.Properties);
            Assert.NotNull(schemaProperty.Value.Ref);
            Assert.Equal(nameof(RefSchemaType), schemaProperty.Value.Ref);

            var definitionSchema = Assert.Single(schema.Definitions);
            Assert.Equal(nameof(RefSchemaType), definitionSchema.Key);
            Assert.Null(definitionSchema.Value.Type);
            Assert.Null(definitionSchema.Value.Values);
            Assert.Null(definitionSchema.Value.Enum);
            Assert.Null(definitionSchema.Value.Ref);
            Assert.Null(definitionSchema.Value.Elements);
            Assert.Null(definitionSchema.Value.Properties);
            Assert.NotNull(definitionSchema.Value.OptionalProperties);

            var definitionSchemaProperty = Assert.Single(definitionSchema.Value.OptionalProperties);
            Assert.Equal(nameof(RefSchemaType.InnerProperty), definitionSchemaProperty.Key);
            Assert.Null(definitionSchemaProperty.Value.Values);
            Assert.Null(definitionSchemaProperty.Value.Enum);
            Assert.Null(definitionSchemaProperty.Value.Ref);
            Assert.Null(definitionSchemaProperty.Value.Elements);
            Assert.Null(definitionSchemaProperty.Value.Properties);
            Assert.Null(definitionSchemaProperty.Value.OptionalProperties);
            Assert.Equal(JsonTypeDefinitionPrimitiveType.String, definitionSchemaProperty.Value.Type);
        }

        public enum EnumSchemaType { A, B, C }

        [Fact]
        [Trait("Category", "Parse")]
        public void EnumSchema_ParseType()
        {
            var schema = JsonTypeDefinitionParser.Parse(typeof(EnumSchemaType));
            AssertEnumSchema(schema);
        }

        [Fact]
        [Trait("Category", "Parse")]
        public void EnumSchema_ParseT()
        {
            var schema = JsonTypeDefinitionParser.Parse<EnumSchemaType>();
            AssertEnumSchema(schema);
        }

        [Fact]
        [Trait("Category", "RFC Compliance")]
        public void EnumSchema_RFC8927Compliant()
        {
            var schema = JsonTypeDefinitionParser.Parse(typeof(EnumSchemaType));
            CheckForRFC8927Compliancy(schema);
        }

        private static void AssertEnumSchema(RootSchema schema)
        {
            Assert.Null(schema.Type);
            Assert.Null(schema.Values);
            Assert.Null(schema.Ref);
            Assert.Null(schema.Elements);
            Assert.Null(schema.Properties);
            Assert.Null(schema.Definitions);
            Assert.Null(schema.OptionalProperties);
            Assert.NotNull(schema.Enum);
            Assert.Contains("A", schema.Enum);
            Assert.Contains("B", schema.Enum);
            Assert.Contains("C", schema.Enum);
        }

        public enum EnumWithoutValues { }

        [Fact]
        [Trait("Category", "Parse")]
        public void EnumSchema_WithoutValues_Throws()
        {
            Assert.Throws<JsonTypeDefinitionParserException>(() => JsonTypeDefinitionParser.Parse<EnumWithoutValues>());
        }

        public enum EnumWithDuplicateValues { Foo, foo }

        [Fact]
        [Trait("Category", "Parse")]
        public void EnumSchema_WithDuplicateValues_Throws()
        {
            Assert.Throws<JsonTypeDefinitionParserException>(() => JsonTypeDefinitionParser.Parse<EnumWithDuplicateValues>());
        }

        public class PropertySchemaType { [Required] public string RequiredProperty { get; set; } }

        [Fact]
        [Trait("Category", "Parse")]
        public void PropertySchema_ParseType()
        {
            var schema = JsonTypeDefinitionParser.Parse(typeof(PropertySchemaType));
            AssertPropertySchema(schema, nameof(PropertySchemaType.RequiredProperty), JsonTypeDefinitionPrimitiveType.String);
        }

        [Fact]
        [Trait("Category", "Parse")]
        public void PropertySchema_ParseT()
        {
            var schema = JsonTypeDefinitionParser.Parse<PropertySchemaType>();
            AssertPropertySchema(schema, nameof(PropertySchemaType.RequiredProperty), JsonTypeDefinitionPrimitiveType.String);
        }

        [Fact]
        [Trait("Category", "RFC Compliance")]
        public void PropertySchema_RFC8927Compliant()
        {
            var schema = JsonTypeDefinitionParser.Parse(typeof(PropertySchemaType));
            CheckForRFC8927Compliancy(schema);
        }

        private static void AssertPropertySchema(RootSchema schema, string expectedName, JsonTypeDefinitionPrimitiveType expectedType)
        {
            Assert.Null(schema.Type);
            Assert.Null(schema.Values);
            Assert.Null(schema.Ref);
            Assert.Null(schema.Elements);
            Assert.Null(schema.Definitions);
            Assert.Null(schema.OptionalProperties);
            Assert.Null(schema.Enum);
            Assert.NotNull(schema.Properties);

            var (name, property) = Assert.Single(schema.Properties);
            Assert.Equal(expectedName, name);
            Assert.Equal(expectedType, property.Type);
        }

        public class OptionalPropertySchemaType { public string OptionalProperty { get; set; } }

        [Fact]
        [Trait("Category", "Parse")]
        public void OptionalPropertySchema_ParseType()
        {
            var schema = JsonTypeDefinitionParser.Parse(typeof(OptionalPropertySchemaType));
            AssertOptionalPropertySchema(schema, nameof(OptionalPropertySchemaType.OptionalProperty), JsonTypeDefinitionPrimitiveType.String);
        }

        [Fact]
        [Trait("Category", "Parse")]
        public void OptionalPropertySchema_ParseT()
        {
            var schema = JsonTypeDefinitionParser.Parse<OptionalPropertySchemaType>();
            AssertOptionalPropertySchema(schema, nameof(OptionalPropertySchemaType.OptionalProperty), JsonTypeDefinitionPrimitiveType.String);
        }

        [Fact]
        [Trait("Category", "RFC Compliance")]
        public void OptionalPropertySchema_RFC8927Compliant()
        {
            var schema = JsonTypeDefinitionParser.Parse(typeof(OptionalPropertySchemaType));
            CheckForRFC8927Compliancy(schema);
        }

        private static void AssertOptionalPropertySchema(RootSchema schema, string expectedName, JsonTypeDefinitionPrimitiveType expectedType)
        {
            Assert.Null(schema.Type);
            Assert.Null(schema.Values);
            Assert.Null(schema.Ref);
            Assert.Null(schema.Elements);
            Assert.Null(schema.Definitions);
            Assert.Null(schema.Properties);
            Assert.Null(schema.Enum);
            Assert.NotNull(schema.OptionalProperties);

            var (name, property) = Assert.Single(schema.OptionalProperties);
            Assert.Equal(expectedName, name);
            Assert.Equal(expectedType, property.Type);
        }

        [Theory]
        [Trait("Category", "Parse")]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(ICollection<string>))]
        [InlineData(typeof(IList<string>))]
        public void ElementsSchema_ParseType(Type src)
        {
            var schema = JsonTypeDefinitionParser.Parse(src);
            AssertElementsSchema(schema, JsonTypeDefinitionPrimitiveType.String);
        }

        [Theory]
        [Trait("Category", "Parse")]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(ICollection<string>))]
        [InlineData(typeof(IList<string>))]
        public void ElementsSchema_ParseT(Type src)
        {
            var methodInfo = typeof(JsonTypeDefinitionParser).GetMethod(
                            nameof(JsonTypeDefinitionParser.Parse),
                            genericParameterCount: 1,
                            types: Array.Empty<Type>()) ?? throw new MissingMethodException();
            if (methodInfo.MakeGenericMethod(src).Invoke(null, null) is not RootSchema schema)
                throw new InvalidOperationException();
            AssertElementsSchema(schema, JsonTypeDefinitionPrimitiveType.String);
        }

        [Theory]
        [Trait("Category", "RFC Compliance")]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(ICollection<string>))]
        [InlineData(typeof(IList<string>))]
        public void ElementsSchema_RFC8927Compliant(Type src)
        {
            var schema = JsonTypeDefinitionParser.Parse(src);
            CheckForRFC8927Compliancy(schema);
        }

        private static void AssertElementsSchema(RootSchema schema, JsonTypeDefinitionPrimitiveType expectedType)
        {
            Assert.Null(schema.Type);
            Assert.Null(schema.Values);
            Assert.Null(schema.Ref);
            Assert.Null(schema.Properties);
            Assert.Null(schema.Definitions);
            Assert.Null(schema.OptionalProperties);
            Assert.Null(schema.Enum);
            Assert.NotNull(schema.Elements);
            Assert.Equal(expectedType, schema.Elements?.Type);
        }


        [Theory]
        [Trait("Category", "Parse")]
        [InlineData(typeof(IDictionary<string, string>))]
        public void ValuesSchema_ParseType(Type src)
        {
            var schema = JsonTypeDefinitionParser.Parse(src);
            AssertValuesSchema(schema, JsonTypeDefinitionPrimitiveType.String);
        }

        [Theory]
        [Trait("Category", "Parse")]
        [InlineData(typeof(IDictionary<string, string>))]
        public void ValuesSchema_ParseT(Type src)
        {
            var methodInfo = typeof(JsonTypeDefinitionParser).GetMethod(
                            nameof(JsonTypeDefinitionParser.Parse),
                            genericParameterCount: 1,
                            types: Array.Empty<Type>()) ?? throw new MissingMethodException();
            if (methodInfo.MakeGenericMethod(src).Invoke(null, null) is not RootSchema schema)
                throw new InvalidOperationException();
            AssertValuesSchema(schema, JsonTypeDefinitionPrimitiveType.String);
        }

        [Theory]
        [Trait("Category", "RFC Compliance")]
        [InlineData(typeof(IDictionary<string, string>))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "<Pending>")]
        public void ValuesSchema_RFC8927Compliant(Type src)
        {
            var schema = JsonTypeDefinitionParser.Parse(src);
            CheckForRFC8927Compliancy(schema);
        }

        private static void AssertValuesSchema(RootSchema schema, JsonTypeDefinitionPrimitiveType expectedType)
        {
            Assert.Null(schema.Type);
            Assert.Null(schema.Ref);
            Assert.Null(schema.Properties);
            Assert.Null(schema.Definitions);
            Assert.Null(schema.OptionalProperties);
            Assert.Null(schema.Enum);
            Assert.Null(schema.Elements);
            Assert.NotNull(schema.Values);
            Assert.Equal(expectedType, schema.Values?.Type);
        }

        public class StaticPropertyType { public static string Foo { get; set; } }

        [Fact]
        [Trait("Category", "Parse")]
        public void IgnoresStaticProperties()
        {
            var schema = JsonTypeDefinitionParser.Parse<StaticPropertyType>();
            Assert.Null(schema.Properties);
            Assert.Null(schema.OptionalProperties);
        }

        public class OnlyRequiredPropertyType { [Required] public string Foo { get; set; } }

        [Fact]
        [Trait("Category", "Parse")]
        public void WithRequiredAttribute_Property()
        {
            var schema = JsonTypeDefinitionParser.Parse<OnlyRequiredPropertyType>();
            Assert.Single(schema.Properties);
            Assert.Null(schema.OptionalProperties);
        }

        public class OnlyOptionalPropertyType { public string Foo { get; set; } }

        [Fact]
        [Trait("Category", "Parse")]
        public void WithoutRequiredAttribute_OptionalProperty()
        {
            var schema = JsonTypeDefinitionParser.Parse<OnlyOptionalPropertyType>();
            Assert.Null(schema.Properties);
            Assert.Single(schema.OptionalProperties);
        }
    }
}