using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Smo;

namespace ExportDatabaseObjects
{
    public class DbSchemaCollection
    {
        internal DbSchemaCollection(Database database, DbSchemaCollectionType dbSchemaCollectionType)
        {
            this.database = database;
            this.dbSchemaCollectionType = dbSchemaCollectionType;
        }
        public string CollectionName { get { return this.dbSchemaCollectionType.ToString(); } }
        public FileSet FileSet { get { return this.fileSet; } }
        public Database Database { get { return this.database; } }

        public IEnumerable<DbSchemaObjectBase> GetDbSchemaObjects()
        {
            FileSet.FileSetMatcher matcher = fileSet.GetMatcher();

            foreach (DbSchemaObjectBase dbObject in GetObjectsUnfiltered())
            {
                if (!dbObject.IsSystemObject && (matcher == null || matcher.IsMatch(dbObject.Name)))
                    yield return dbObject;
            }
        }
        public void Prefetch()
        {
            if (!hasPrefetched)
            {
                Type smoType = GetSmoObjectType();
                database.PrefetchObjects(smoType);
                hasPrefetched = true;
            }
        }

        private IEnumerable<DbSchemaObjectBase> GetObjectsUnfiltered()
        {
            Prefetch();
            switch (dbSchemaCollectionType)
            {
                case DbSchemaCollectionType.Procedures:
                    foreach (StoredProcedure smoObject in database.StoredProcedures) yield return new DbStoredProcedure(smoObject);
                    break;
                case DbSchemaCollectionType.Functions:
                    foreach (UserDefinedFunction smoObject in database.UserDefinedFunctions) yield return new DbUserDefinedFunction(smoObject);
                    break;
                case DbSchemaCollectionType.Tables:
                    foreach (Table smoObject in database.Tables) yield return new DbTable(smoObject);
                    break;
                case DbSchemaCollectionType.Views:
                    foreach (View smoObject in database.Views) yield return new DbView(smoObject);
                    break;
                default:
                    yield break;
            }
        }

        static readonly Dictionary<DbSchemaCollectionType, Type> smoObjectTypeMap;
        static DbSchemaCollection()
        {
            smoObjectTypeMap = new Dictionary<DbSchemaCollectionType, Type>();
            smoObjectTypeMap.Add(DbSchemaCollectionType.Procedures, typeof(StoredProcedure));
            smoObjectTypeMap.Add(DbSchemaCollectionType.Functions, typeof(UserDefinedFunction));
            smoObjectTypeMap.Add(DbSchemaCollectionType.Tables, typeof(Table));
            smoObjectTypeMap.Add(DbSchemaCollectionType.Views, typeof(View));
        }
        Type GetSmoObjectType()
        {
            return smoObjectTypeMap[this.dbSchemaCollectionType];
        }

        Database database;
        DbSchemaCollectionType dbSchemaCollectionType;
        FileSet fileSet = new FileSet();
        bool hasPrefetched = false;
    }
}