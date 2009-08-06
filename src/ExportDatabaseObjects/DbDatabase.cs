using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace ExportDatabaseObjects
{
    public class DbDatabase
    {
        public DbDatabase(string serverName, string userName, string password, string databaseName)
        {
            ServerConnection connection = (String.IsNullOrEmpty(userName)) ? new ServerConnection(serverName) : new ServerConnection(serverName, userName, password);
            Server server = new Server(connection);
            this.database = server.Databases[databaseName];

            schemaCollections = new Dictionary<DbSchemaCollectionType, DbSchemaCollection>();
            schemaCollections.Add(DbSchemaCollectionType.Procedures, new DbSchemaCollection(database, DbSchemaCollectionType.Procedures));
            schemaCollections.Add(DbSchemaCollectionType.Functions, new DbSchemaCollection(database, DbSchemaCollectionType.Functions));
            schemaCollections.Add(DbSchemaCollectionType.Views, new DbSchemaCollection(database, DbSchemaCollectionType.Views));
            schemaCollections.Add(DbSchemaCollectionType.Tables, new DbSchemaCollection(database, DbSchemaCollectionType.Tables));
        }

        public DbSchemaCollection GetSchemaCollection(DbSchemaCollectionType collectionType)
        {
            return schemaCollections[collectionType];
        }

        public DbSchemaObjectBase FindDbSchemaObject(string objectName)
        {
            StoredProcedure sp = database.StoredProcedures[objectName];
            if (sp != null)
                return new DbStoredProcedure(sp);

            UserDefinedFunction udf = database.UserDefinedFunctions[objectName];
            if (udf != null)
                return new DbUserDefinedFunction(udf);

            View v = database.Views[objectName];
            if (v != null)
                return new DbView(v);

            Table t = database.Tables[objectName];
            if (t != null)
                return new DbTable((Table)t);

            return null;
        }

        Database database;
        Dictionary<DbSchemaCollectionType, DbSchemaCollection> schemaCollections;

    }
}