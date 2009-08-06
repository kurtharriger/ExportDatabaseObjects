using System;
using System.Diagnostics;
using System.IO;

namespace ExportDatabaseObjects
{
    public class ScriptGenerator
    {
        public delegate void ProgressNotificationHandler(string message);
        public void LoadConfigurationOptions()
        {
            ServerName = System.Configuration.ConfigurationManager.AppSettings["Server"];
            UserName = System.Configuration.ConfigurationManager.AppSettings["User"];
            Password = System.Configuration.ConfigurationManager.AppSettings["Password"];
            DatabaseName = System.Configuration.ConfigurationManager.AppSettings["Database"];
            ScriptFolder = System.Configuration.ConfigurationManager.AppSettings["ScriptFolder"];

            Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["ScriptProcedures"], out ScriptProcedures);
            Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["ScriptFunctions"], out ScriptFunctions);
            Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["ScriptViews"], out ScriptViews);
            Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["ScriptTables"], out ScriptTables);
            Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["ScriptModifiedObjectsOnly"], out ScriptModifiedObjectsOnly);
        }
        public void ValidateOptions()
        {
            if (String.IsNullOrEmpty(ServerName))
            {
                throw new ApplicationException("Server not specified.");
            }
            if (String.IsNullOrEmpty(DatabaseName))
            {
                throw new ApplicationException("Database not specified.");
            }
            if (String.IsNullOrEmpty(ScriptFolder))
            {
                throw new ApplicationException("ScriptFolder not specified.");
            }
        }
        
        public void Script(string[] objectNames)
        {
            DbDatabase database = new DbDatabase(this.ServerName, this.UserName, this.Password, this.DatabaseName);
            DbSchemaCollection procedures = database.GetSchemaCollection(DbSchemaCollectionType.Procedures);
            DbSchemaCollection functions = database.GetSchemaCollection(DbSchemaCollectionType.Functions);
            DbSchemaCollection tables = database.GetSchemaCollection(DbSchemaCollectionType.Tables);
            DbSchemaCollection views = database.GetSchemaCollection(DbSchemaCollectionType.Views);

            if (objectNames == null || objectNames.Length == 0)
            {
                if (ScriptProcedures) ScriptSchemaCollection(procedures);
                if (ScriptFunctions) ScriptSchemaCollection(functions);
                if (ScriptViews) ScriptSchemaCollection(views);
                if (ScriptTables) ScriptSchemaCollection(tables);
            }
            else
            {
                foreach (string objectName in objectNames)
                {
                    DbSchemaObjectBase dbObject = database.FindDbSchemaObject(objectName);
                    if (dbObject != null)
                    {
                        ScriptDbSchemaObject(dbObject);
                    }
                    else OnProgressNotification(String.Format("{0} was not found.", objectName));
                }
            }
        }

        public event ProgressNotificationHandler ProgressNotification;
        protected virtual void OnProgressNotification(string message)
        {
            if (ProgressNotification != null) ProgressNotification(message);
        }

        void ScriptSchemaCollection(DbSchemaCollection schemaCollection)
        {
            OnProgressNotification(String.Format("Loading {0}.", schemaCollection.CollectionName.ToLower()));
            schemaCollection.Prefetch();
            OnProgressNotification("Checking files.");

            foreach (DbSchemaObjectBase dbObject in schemaCollection.GetDbSchemaObjects())
            {
                ScriptDbSchemaObject(dbObject);
            }
            OnProgressNotification(String.Format("Finished scripting {0}.", schemaCollection.CollectionName.ToLower()));
        }
        bool ScriptDbSchemaObject(DbSchemaObjectBase dbObject)
        {
            string folder = GetSubFolder(dbObject.DbSchemaObjectType);
            string fileName = Path.Combine(folder, dbObject.Name + ".sql");

            if (File.Exists(fileName))
            {
                if (ScriptModifiedObjectsOnly && File.GetLastWriteTime(fileName) >= dbObject.DateLastModified)
                    return false;
                File.Delete(fileName);
            }
            else if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            try
            {
                using (System.IO.FileStream file = System.IO.File.Create(fileName))
                {
                    // Specify encoding
                    using (StreamWriter sw = new StreamWriter(file))
                    {
                        dbObject.Script(sw);
                        sw.Close();
                    }
                    file.Close();
                }
                File.SetLastWriteTime(fileName, dbObject.DateLastModified);
                OnProgressNotification(String.Format("Scripted {0}.", dbObject.Name));
                return true;
            }
            catch
            {
                if (File.Exists(fileName)) File.Delete(fileName);
                throw;
            }
        }

        string GetSubFolder(DbObjectType dbObjectType)
        {
            return Path.Combine(ScriptFolder, dbObjectType.ToString() + "s");
        }



        public string ServerName = null;
        public string UserName = null;
        public string Password = null;
        public string DatabaseName = null;
        public string ScriptFolder = null;

        public bool ScriptProcedures = true;
        public bool ScriptFunctions = true;
        public bool ScriptViews = true;
        public bool ScriptTables = false;

        public bool ScriptModifiedObjectsOnly = true;
    }
}