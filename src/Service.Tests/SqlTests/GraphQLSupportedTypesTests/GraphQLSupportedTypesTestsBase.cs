// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Azure.DataApiBuilder.Service.GraphQLBuilder.GraphQLTypes.SupportedTypes;

namespace Azure.DataApiBuilder.Service.Tests.SqlTests.GraphQLSupportedTypesTests
{

    [TestClass]
    public abstract class GraphQLSupportedTypesTestBase : SqlTestBase
    {
        protected const string TYPE_TABLE = "TypeTable";

        #region Tests

        [DataTestMethod]
        [DataRow(BYTE_TYPE, 1)]
        [DataRow(BYTE_TYPE, 2)]
        [DataRow(BYTE_TYPE, 3)]
        [DataRow(BYTE_TYPE, 4)]
        [DataRow(SHORT_TYPE, 1)]
        [DataRow(SHORT_TYPE, 2)]
        [DataRow(SHORT_TYPE, 3)]
        [DataRow(SHORT_TYPE, 4)]
        [DataRow(INT_TYPE, 1)]
        [DataRow(INT_TYPE, 2)]
        [DataRow(INT_TYPE, 3)]
        [DataRow(INT_TYPE, 4)]
        [DataRow(LONG_TYPE, 1)]
        [DataRow(LONG_TYPE, 2)]
        [DataRow(LONG_TYPE, 3)]
        [DataRow(LONG_TYPE, 4)]
        [DataRow(SINGLE_TYPE, 1)]
        [DataRow(SINGLE_TYPE, 2)]
        [DataRow(SINGLE_TYPE, 3)]
        [DataRow(SINGLE_TYPE, 4)]
        [DataRow(FLOAT_TYPE, 1)]
        [DataRow(FLOAT_TYPE, 2)]
        [DataRow(FLOAT_TYPE, 3)]
        [DataRow(FLOAT_TYPE, 4)]
        [DataRow(DECIMAL_TYPE, 1)]
        [DataRow(DECIMAL_TYPE, 2)]
        [DataRow(DECIMAL_TYPE, 3)]
        [DataRow(DECIMAL_TYPE, 4)]
        [DataRow(STRING_TYPE, 1)]
        [DataRow(STRING_TYPE, 2)]
        [DataRow(STRING_TYPE, 3)]
        [DataRow(STRING_TYPE, 4)]
        [DataRow(BOOLEAN_TYPE, 1)]
        [DataRow(BOOLEAN_TYPE, 2)]
        [DataRow(BOOLEAN_TYPE, 3)]
        [DataRow(BOOLEAN_TYPE, 4)]
        [DataRow(DATETIME_TYPE, 1)]
        [DataRow(DATETIME_TYPE, 2)]
        [DataRow(DATETIME_TYPE, 3)]
        [DataRow(DATETIME_TYPE, 4)]
        [DataRow(BYTEARRAY_TYPE, 1)]
        [DataRow(BYTEARRAY_TYPE, 2)]
        [DataRow(BYTEARRAY_TYPE, 3)]
        [DataRow(BYTEARRAY_TYPE, 4)]
        [DataRow(GUID_TYPE, 1)]
        [DataRow(GUID_TYPE, 2)]
        [DataRow(GUID_TYPE, 3)]
        [DataRow(GUID_TYPE, 4)]
        public async Task QueryTypeColumn(string type, int id)
        {
            if (!IsSupportedType(type))
            {
                Assert.Inconclusive("Type not supported");
            }

            string field = $"{type.ToLowerInvariant()}_types";
            string graphQLQueryName = "supportedType_by_pk";
            string gqlQuery = "{ supportedType_by_pk(typeid: " + id + ") { " + field + " } }";

            string dbQuery = MakeQueryOnTypeTable(new List<string> { field }, id);

            JsonElement actual = await ExecuteGraphQLRequestAsync(gqlQuery, graphQLQueryName, isAuthenticated: false);
            string expected = await GetDatabaseResultAsync(dbQuery);

            PerformTestEqualsForExtendedTypes(type, expected, actual.ToString());
        }

        [DataTestMethod]
        [DataRow(BYTE_TYPE, "255")]
        [DataRow(BYTE_TYPE, "0")]
        [DataRow(BYTE_TYPE, "null")]
        [DataRow(SHORT_TYPE, "0")]
        [DataRow(SHORT_TYPE, "30000")]
        [DataRow(SHORT_TYPE, "-30000")]
        [DataRow(SHORT_TYPE, "null")]
        [DataRow(INT_TYPE, "9999")]
        [DataRow(INT_TYPE, "0")]
        [DataRow(INT_TYPE, "-9999")]
        [DataRow(INT_TYPE, "null")]
        [DataRow(LONG_TYPE, "0")]
        [DataRow(LONG_TYPE, "9000000000000000000")]
        [DataRow(LONG_TYPE, "-9000000000000000000")]
        [DataRow(LONG_TYPE, "null")]
        [DataRow(STRING_TYPE, "\"aaaaaaaaaa\"")]
        [DataRow(STRING_TYPE, "\"\"")]
        [DataRow(STRING_TYPE, "null")]
        [DataRow(SINGLE_TYPE, "-3.33")]
        [DataRow(SINGLE_TYPE, "2E35")]
        [DataRow(SINGLE_TYPE, "null")]
        [DataRow(FLOAT_TYPE, "-3.33")]
        [DataRow(FLOAT_TYPE, "2E150")]
        [DataRow(FLOAT_TYPE, "null")]
        [DataRow(DECIMAL_TYPE, "-3.333333")]
        [DataRow(DECIMAL_TYPE, "1222222.00000929292")]
        [DataRow(DECIMAL_TYPE, "null")]
        [DataRow(BOOLEAN_TYPE, "true")]
        [DataRow(BOOLEAN_TYPE, "false")]
        [DataRow(BOOLEAN_TYPE, "null")]
        [DataRow(DATETIME_NONUTC_TYPE, "\"1999-01-08 10:23:54+8:00\"")]
        [DataRow(DATETIME_TYPE, "\"1999-01-08 09:20:00\"")]
        [DataRow(DATETIME_TYPE, "\"1999-01-08\"")]
        [DataRow(DATETIME_TYPE, "null")]
        [DataRow(BYTEARRAY_TYPE, "\"U3RyaW5neQ==\"")]
        [DataRow(BYTEARRAY_TYPE, "\"V2hhdGNodSBkb2luZyBkZWNvZGluZyBvdXIgdGVzdCBiYXNlNjQgc3RyaW5ncz8=\"")]
        [DataRow(BYTEARRAY_TYPE, "null")]
        public async Task InsertIntoTypeColumn(string type, string value)
        {
            if (!IsSupportedType(type))
            {
                Assert.Inconclusive("Type not supported");
            }

            // Datetime non utc type is a characterization of the value added to the datetime type,
            // so before executing the query reset it to mean the actually underlying type.
            if (DATETIME_NONUTC_TYPE.Equals(type))
            {
                type = DATETIME_TYPE;
            }

            string field = $"{type.ToLowerInvariant()}_types";
            string graphQLQueryName = "createSupportedType";
            string gqlQuery = "mutation{ createSupportedType (item: {" + field + ": " + value + " }){ " + field + " } }";

            string dbQuery = MakeQueryOnTypeTable(new List<string> { field }, id: 5001);

            JsonElement actual = await ExecuteGraphQLRequestAsync(gqlQuery, graphQLQueryName, isAuthenticated: true);
            string expected = await GetDatabaseResultAsync(dbQuery);

            PerformTestEqualsForExtendedTypes(type, expected, actual.ToString());

            await ResetDbStateAsync();
        }

        [DataTestMethod]
        [DataRow(BYTE_TYPE, 255)]
        [DataRow(SHORT_TYPE, 30000)]
        [DataRow(INT_TYPE, 9999)]
        [DataRow(LONG_TYPE, 9000000000000000000)]
        [DataRow(STRING_TYPE, "aaaaaaaaaa")]
        [DataRow(FLOAT_TYPE, -3.33)]
        [DataRow(DECIMAL_TYPE, 1222222.00000929292)]
        [DataRow(BOOLEAN_TYPE, true)]
        [DataRow(DATETIME_NONUTC_TYPE, "1999-01-08 10:23:54+8:00")]
        [DataRow(DATETIME_TYPE, "1999-01-08 10:23:54")]
        [DataRow(BYTEARRAY_TYPE, "V2hhdGNodSBkb2luZyBkZWNvZGluZyBvdXIgdGVzdCBiYXNlNjQgc3RyaW5ncz8=")]
        public async Task InsertIntoTypeColumnWithArgument(string type, object value)
        {
            if (!IsSupportedType(type))
            {
                Assert.Inconclusive("Type not supported");
            }

            // Datetime non utc type is a characterization of the value added to the datetime type,
            // so before executing the query reset it to mean the actually underlying type.
            if (DATETIME_NONUTC_TYPE.Equals(type))
            {
                type = DATETIME_TYPE;
            }

            string field = $"{type.ToLowerInvariant()}_types";
            string graphQLQueryName = "createSupportedType";
            string gqlQuery = "mutation($param: " + type + "){ createSupportedType (item: {" + field + ": $param }){ " + field + " } }";

            string dbQuery = MakeQueryOnTypeTable(new List<string> { field }, id: 5001);

            JsonElement actual = await ExecuteGraphQLRequestAsync(gqlQuery, graphQLQueryName, isAuthenticated: true, new() { { "param", value } });
            string expected = await GetDatabaseResultAsync(dbQuery);

            PerformTestEqualsForExtendedTypes(type, expected, actual.ToString());

            await ResetDbStateAsync();
        }

        [DataTestMethod]
        [DataRow(BYTE_TYPE, "255")]
        [DataRow(BYTE_TYPE, "0")]
        [DataRow(BYTE_TYPE, "null")]
        [DataRow(SHORT_TYPE, "0")]
        [DataRow(SHORT_TYPE, "30000")]
        [DataRow(SHORT_TYPE, "-30000")]
        [DataRow(SHORT_TYPE, "null")]
        [DataRow(INT_TYPE, "9999")]
        [DataRow(INT_TYPE, "0")]
        [DataRow(INT_TYPE, "-9999")]
        [DataRow(INT_TYPE, "null")]
        [DataRow(LONG_TYPE, "0")]
        [DataRow(LONG_TYPE, "9000000000000000000")]
        [DataRow(LONG_TYPE, "-9000000000000000000")]
        [DataRow(LONG_TYPE, "null")]
        [DataRow(STRING_TYPE, "\"aaaaaaaaaa\"")]
        [DataRow(STRING_TYPE, "\"\"")]
        [DataRow(STRING_TYPE, "null")]
        [DataRow(SINGLE_TYPE, "-3.33")]
        [DataRow(SINGLE_TYPE, "2E35")]
        [DataRow(SINGLE_TYPE, "null")]
        [DataRow(FLOAT_TYPE, "-3.33")]
        [DataRow(FLOAT_TYPE, "2E150")]
        [DataRow(FLOAT_TYPE, "null")]
        [DataRow(DECIMAL_TYPE, "-3.333333")]
        [DataRow(DECIMAL_TYPE, "1222222.00000929292")]
        [DataRow(DECIMAL_TYPE, "null")]
        [DataRow(BOOLEAN_TYPE, "true")]
        [DataRow(BOOLEAN_TYPE, "false")]
        [DataRow(BOOLEAN_TYPE, "null")]
        [DataRow(DATETIME_NONUTC_TYPE, "\"1999-01-08 10:23:54+8:00\"")]
        [DataRow(DATETIME_TYPE, "\"1999-01-08 09:20:00\"")]
        [DataRow(DATETIME_TYPE, "\"1999-01-08\"")]
        [DataRow(DATETIME_TYPE, "null")]
        [DataRow(BYTEARRAY_TYPE, "\"U3RyaW5neQ==\"")]
        [DataRow(BYTEARRAY_TYPE, "\"V2hhdGNodSBkb2luZyBkZWNvZGluZyBvdXIgdGVzdCBiYXNlNjQgc3RyaW5ncz8=\"")]
        [DataRow(BYTEARRAY_TYPE, "null")]
        [DataRow(GUID_TYPE, "\"3a1483a5-9ac2-4998-bcf3-78a28078c6ac\"")]
        [DataRow(GUID_TYPE, "null")]
        public async Task UpdateTypeColumn(string type, string value)
        {
            if (!IsSupportedType(type))
            {
                Assert.Inconclusive("Type not supported");
            }

            // Datetime non utc type is a characterization of the value added to the datetime type,
            // so before executing the query reset it to mean the actually underlying type.
            if (DATETIME_NONUTC_TYPE.Equals(type))
            {
                type = DATETIME_TYPE;
            }

            string field = $"{type.ToLowerInvariant()}_types";
            string graphQLQueryName = "updateSupportedType";
            string gqlQuery = "mutation{ updateSupportedType (typeid: 1, item: {" + field + ": " + value + " }){ " + field + " } }";

            string dbQuery = MakeQueryOnTypeTable(new List<string> { field }, id: 1);

            JsonElement actual = await ExecuteGraphQLRequestAsync(gqlQuery, graphQLQueryName, isAuthenticated: true);
            string expected = await GetDatabaseResultAsync(dbQuery);

            PerformTestEqualsForExtendedTypes(type, expected, actual.ToString());

            await ResetDbStateAsync();
        }

        [DataTestMethod]
        [DataRow(BYTE_TYPE, 255)]
        [DataRow(SHORT_TYPE, 30000)]
        [DataRow(INT_TYPE, 9999)]
        [DataRow(LONG_TYPE, 9000000000000000000)]
        [DataRow(STRING_TYPE, "aaaaaaaaaa")]
        [DataRow(FLOAT_TYPE, -3.33)]
        [DataRow(DECIMAL_TYPE, 1222222.00000929292)]
        [DataRow(BOOLEAN_TYPE, true)]
        [DataRow(DATETIME_TYPE, "1999-01-08 10:23:54")]
        [DataRow(DATETIME_NONUTC_TYPE, "1999-01-08 10:23:54+8:00")]
        [DataRow(BYTEARRAY_TYPE, "V2hhdGNodSBkb2luZyBkZWNvZGluZyBvdXIgdGVzdCBiYXNlNjQgc3RyaW5ncz8=")]
        [DataRow(GUID_TYPE, "3a1483a5-9ac2-4998-bcf3-78a28078c6ac")]
        [DataRow(GUID_TYPE, null)]
        public async Task UpdateTypeColumnWithArgument(string type, object value)
        {
            if (!IsSupportedType(type))
            {
                Assert.Inconclusive("Type not supported");
            }

            // Datetime non utc type is a characterization of the value added to the datetime type,
            // so before executing the query reset it to mean the actually underlying type.
            if (DATETIME_NONUTC_TYPE.Equals(type))
            {
                type = DATETIME_TYPE;
            }

            string field = $"{type.ToLowerInvariant()}_types";
            string graphQLQueryName = "updateSupportedType";
            string gqlQuery = "mutation($param: " + TypeNameToGraphQLType(type) + "){ updateSupportedType (typeid: 1, item: {" + field + ": $param }){ " + field + " } }";

            string dbQuery = MakeQueryOnTypeTable(new List<string> { field }, id: 1);

            JsonElement actual = await ExecuteGraphQLRequestAsync(gqlQuery, graphQLQueryName, isAuthenticated: true, new() { { "param", value } });
            string expected = await GetDatabaseResultAsync(dbQuery);

            PerformTestEqualsForExtendedTypes(type, expected, actual.ToString());

            await ResetDbStateAsync();
        }

        #endregion

        /// <summary>
        /// Utility function to do special comparisons for some of the extended types
        /// if json compare doesn't suffice
        /// </summary>
        private static void PerformTestEqualsForExtendedTypes(string type, string expected, string actual)
        {
            if (type == SINGLE_TYPE || type == FLOAT_TYPE || type == DECIMAL_TYPE)
            {
                CompareFloatResults(type, actual.ToString(), expected);
            }
            else if (type == DATETIME_TYPE)
            {
                CompareDateTimeResults(actual.ToString(), expected);
            }
            else
            {
                SqlTestHelper.PerformTestEqualJsonStrings(expected, actual.ToString());
            }
        }

        /// <summary>
        /// HotChocolate will parse large floats to exponential notation
        /// while the db will return the number fully printed out. Because
        /// the json deep compare function we are using does not account for such scenario
        /// a special comparison is needed to test floats
        /// </summary>
        private static void CompareFloatResults(string floatType, string actual, string expected)
        {
            string fieldName = $"{floatType.ToLowerInvariant()}_types";

            using JsonDocument actualJsonDoc = JsonDocument.Parse(actual);
            using JsonDocument expectedJsonDoc = JsonDocument.Parse(expected);

            string actualFloat = actualJsonDoc.RootElement.GetProperty(fieldName).ToString();
            string expectedFloat = expectedJsonDoc.RootElement.GetProperty(fieldName).ToString();

            // handles cases when one of the values is null
            if (string.IsNullOrEmpty(actualFloat) || string.IsNullOrEmpty(expectedFloat))
            {
                Assert.AreEqual(expectedFloat, actualFloat);
                return;
            }

            switch (floatType)
            {
                case SINGLE_TYPE:
                    Assert.AreEqual(float.Parse(expectedFloat), float.Parse(actualFloat));
                    break;
                case FLOAT_TYPE:
                    Assert.AreEqual(double.Parse(expectedFloat), double.Parse(actualFloat));
                    break;
                case DECIMAL_TYPE:
                    Assert.AreEqual(decimal.Parse(expectedFloat), decimal.Parse(actualFloat));
                    break;
                default:
                    Assert.Fail($"Calling compare on unrecognized float type {floatType}");
                    break;
            }
        }

        /// <summary>
        /// Required due to different format between mysql datetime and HotChocolate datetime
        /// result
        /// </summary>
        private static void CompareDateTimeResults(string actual, string expected)
        {
            string fieldName = "datetime_types";

            using JsonDocument actualJsonDoc = JsonDocument.Parse(actual);
            using JsonDocument expectedJsonDoc = JsonDocument.Parse(expected);

            string actualDateTime = actualJsonDoc.RootElement.GetProperty(fieldName).ToString();
            string expectedDateTime = expectedJsonDoc.RootElement.GetProperty(fieldName).ToString();

            // handles cases when one of the values is null
            if (string.IsNullOrEmpty(actualDateTime) || string.IsNullOrEmpty(expectedDateTime))
            {
                Assert.AreEqual(expectedDateTime, actualDateTime);
            }
            else
            {
                Assert.AreEqual(DateTimeOffset.Parse(expectedDateTime), DateTimeOffset.Parse(actualDateTime));
            }
        }

        /// <summary>
        /// Needed to map the type name to a graphql type in argument tests
        /// where the argument type need to be specified.
        /// </summary>
        private static string TypeNameToGraphQLType(string typeName)
        {
            if (typeName is GUID_TYPE)
            {
                return STRING_TYPE;
            }

            return typeName;
        }

        protected abstract string MakeQueryOnTypeTable(List<string> columnsToQuery, int id);
        protected virtual bool IsSupportedType(string type)
        {
            return true;
        }
    }
}
