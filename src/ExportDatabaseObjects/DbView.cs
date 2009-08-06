using System;
using System.IO;
using Microsoft.SqlServer.Management.Smo;

namespace ExportDatabaseObjects
{
    class DbView : DbSchemaObjectBase
    {
        public DbView(View smoObject)
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
            ScriptHelper.ScriptOptions(tw);
            ScriptHelper.ScriptDrop(smoObject.Schema, smoObject.Name, objectTypeCode, dbObjectType, tw);
            ScriptHelper.ScriptObject(smoObject, tw);
            ScriptHelper.ScriptGrantPermissions(permissionType, smoObject.Schema, smoObject.Name, dbObjectType, tw);
        }

        View smoObject;
        static readonly DbObjectType dbObjectType = DbObjectType.View;
        static readonly string objectTypeCode = "V";
        static readonly string permissionType = "SELECT";
    }
}