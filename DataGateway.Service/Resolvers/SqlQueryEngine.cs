using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.DataGateway.Service.Models;
using Azure.DataGateway.Services;

namespace Azure.DataGateway.Service.Resolvers
{
    //<summary>
    // SqlQueryEngine to execute queries against Sql like databases.
    //</summary>
    public class SqlQueryEngine : IQueryEngine
    {
        private readonly IMetadataStoreProvider _metadataStoreProvider;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IQueryBuilder _queryBuilder;

        // <summary>
        // Constructor.
        // </summary>
        public SqlQueryEngine(IMetadataStoreProvider metadataStoreProvider, IQueryExecutor queryExecutor, IQueryBuilder queryBuilder)
        {
            _metadataStoreProvider = metadataStoreProvider;
            _queryExecutor = queryExecutor;
            _queryBuilder = queryBuilder;
        }

        // <summary>
        // Registers the given resolver with this query engine.
        // </summary>
        public void RegisterResolver(GraphQLQueryResolver resolver)
        {
            // Registration of Resolvers is already done at startup.
            // no-op
        }

        // <summary>
        // Executes the given named graphql query on the backend.
        // </summary>
        public async Task<JsonDocument> ExecuteAsync(string graphQLQueryName, IDictionary<string, object> parameters, bool isContinuationQuery)
        {
            // TODO: add support for nesting
            // TODO: add support for join query against another table
            // TODO: add support for TOP and Order-by push-down

            GraphQLQueryResolver resolver = _metadataStoreProvider.GetQueryResolver(graphQLQueryName);
            JsonDocument jsonDocument = JsonDocument.Parse("{ }");

            string queryText = _queryBuilder.Build(resolver.ParametrizedQuery, false);

            // Open connection and execute query using _queryExecutor
            //
            DbDataReader dbDataReader = await _queryExecutor.ExecuteQueryAsync(queryText, parameters);

            // Parse Results into Json and return
            //
            if (await dbDataReader.ReadAsync())
            {
                jsonDocument = JsonDocument.Parse(dbDataReader.GetString(0));
            }
            else
            {
                Console.WriteLine("Did not return enough rows in the JSON result.");
            }

            return jsonDocument;
        }

        // <summary>
        // Executes the given named graphql query on the backend and expecting a list of Jsons back.
        // </summary>
        public async Task<IEnumerable<JsonDocument>> ExecuteListAsync(string graphQLQueryName, IDictionary<string, object> parameters)
        {
            // TODO: add support for nesting
            // TODO: add support for join query against another container
            // TODO: add support for TOP and Order-by push-down

            GraphQLQueryResolver resolver = _metadataStoreProvider.GetQueryResolver(graphQLQueryName);
            List<JsonDocument> resultsAsList = new();
            string queryText = _queryBuilder.Build(resolver.ParametrizedQuery, true);
            DbDataReader dbDataReader = await _queryExecutor.ExecuteQueryAsync(queryText, parameters);

            // Deserialize results into list of JsonDocuments and return
            //
            if (await dbDataReader.ReadAsync())
            {
                resultsAsList = JsonSerializer.Deserialize<List<JsonDocument>>(dbDataReader.GetString(0));
            }
            else
            {
                Console.WriteLine("Did not return enough rows in the JSON result.");
            }

            return resultsAsList;
        }

        // <summary>
        // Given the FindQuery structure, obtains the query text and executes it against the backend. Useful for REST API scenarios.
        // </summary>
        public async Task<JsonDocument> ExecuteAsync(FindQueryStructure queryStructure)
        {
            string queryText = _queryBuilder.Build(queryStructure);

            // Open connection and execute query using _queryExecutor
            //
            DbDataReader dbDataReader = await _queryExecutor.ExecuteQueryAsync(queryText, queryStructure.Parameters);
            JsonDocument jsonDocument = null;

            // Parse Results into Json and return
            //
            if (await dbDataReader.ReadAsync())
            {
                jsonDocument = JsonDocument.Parse(dbDataReader.GetString(0));
            }
            else
            {
                Console.WriteLine("Did not return enough rows in the JSON result.");
            }

            return jsonDocument;
        }
    }
}