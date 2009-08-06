using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.Management.Smo;

namespace ExportDatabaseObjects
{
    class DbUserDefinedFunction : DbSchemaObjectBase
    {
        public DbUserDefinedFunction(UserDefinedFunction smoObject)
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
            ScriptHelper.ScriptDrop(smoObject.Schema, smoObject.Name, typeCode[smoObject.FunctionType], dbObjectType, tw);
            ScriptHelper.ScriptObject(smoObject, tw);
            ScriptHelper.ScriptGrantPermissions(permissionType[smoObject.FunctionType], smoObject.Schema, smoObject.Name, dbObjectType, tw);
        }

        static DbUserDefinedFunction()
        {
            typeCode = new Dictionary<UserDefinedFunctionType, string>();
            typeCode.Add(UserDefinedFunctionType.Inline, "IF");
            typeCode.Add(UserDefinedFunctionType.Scalar, "FN");
            typeCode.Add(UserDefinedFunctionType.Table, "TF");

            permissionType = new Dictionary<UserDefinedFunctionType, string>();
            permissionType.Add(UserDefinedFunctionType.Inline, "SELECT");
            permissionType.Add(UserDefinedFunctionType.Scalar, "EXECUTE");
            permissionType.Add(UserDefinedFunctionType.Table, "SELECT");
        }

        UserDefinedFunction smoObject;
        static readonly DbObjectType dbObjectType = DbObjectType.Function;
        static readonly Dictionary<UserDefinedFunctionType, string> typeCode;
        static readonly Dictionary<UserDefinedFunctionType, string> permissionType;

    }
}