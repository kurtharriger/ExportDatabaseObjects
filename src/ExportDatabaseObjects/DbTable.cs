using System;
using System.IO;
using Microsoft.SqlServer.Management.Smo;

namespace ExportDatabaseObjects
{
    class DbTable : DbSchemaObjectBase
    {
        public DbTable(Table smoObject)
        {
            this.smoObject = smoObject;
        }

        public override DbObjectType DbSchemaObjectType
        {
            get { return dbObjectType; }
        }

        public override string Name
        {
            get { return smoObject.Name; }
        }

        public override bool IsSystemObject
        {
            get { return smoObject.IsSystemObject; }
        }

        public override DateTime DateLastModified
        {
            get { return smoObject.DateLastModified; }
        }

        public override void Script(TextWriter tw)
        {
            ScriptHelper.ScriptObject(smoObject, tw, scriptingOptions);
        }

        static DbTable()
        {
            scriptingOptions = new ScriptingOptions();
            scriptingOptions.AllowSystemObjects = false;
            scriptingOptions.IncludeHeaders = false;
            scriptingOptions.IncludeIfNotExists = true;
            scriptingOptions.DriAll = true;
        }

        Table smoObject;
        readonly static DbObjectType dbObjectType = DbObjectType.Table;
        readonly static ScriptingOptions scriptingOptions;
    }
}