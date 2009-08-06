using System;
using Microsoft.SqlServer.Management.Smo;
using System.IO;

namespace ExportDatabaseObjects
{
    static class ScriptHelper
    {
        public static void ScriptOptions(TextWriter tw)
        {
            tw.WriteLine("SET ANSI_NULLS ON\r\nSET QUOTED_IDENTIFIER ON\r\nGO");
        }

        public static void ScriptObject(IScriptable smoObject, TextWriter tw)
        {
            ScriptObject(smoObject, tw, defaultScriptingOptions);
        }

        public static void ScriptObject(IScriptable smoObject, TextWriter tw, ScriptingOptions options)
        {
            System.Collections.Specialized.StringCollection script = smoObject.Script(options);
            foreach (string line in script)
            {
                tw.WriteLine(line);
                // there is a bug in the current version of SMO.  When scripting procedures and functions it does not script a GO after setting quoted_identifier option.
                if (line.StartsWith("SET QUOTED_IDENTIFIER")) tw.WriteLine("GO");
            }
            tw.WriteLine("GO");
        }

        public static void ScriptDrop(string schema, string objectName, string objectTypeCode, DbObjectType dbObjectType, TextWriter tw)
        {
            tw.WriteLine(String.Format(DropFormatString, schema, objectName, objectTypeCode, dbObjectType.ToString().ToUpper(), dbObjectType.ToString().ToLower()));
        }

        public static void ScriptGrantPermissions(string permission, string schema, string objectName, DbObjectType dbObjectType, TextWriter tw)
        {
            tw.WriteLine(String.Format(GrantPermissionFormatString, permission, schema, objectName, dbObjectType.ToString()));
        }

        static ScriptHelper()
        {

            defaultScriptingOptions = new ScriptingOptions();
            defaultScriptingOptions.AllowSystemObjects = false;
            defaultScriptingOptions.IncludeHeaders = false;
        }

        static readonly ScriptingOptions defaultScriptingOptions;

        //0 - schema, 1 - object name, 2 - sys.objects type code, 3 - object type name (upper), 4 - object type name (lower)
        static readonly string DropFormatString = "IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type = N'{2}')\r\n" +
                                                  "BEGIN\r\n" +
                                                  "  DROP {3} [{0}].[{1}]\r\n" +
                                                  "  Print 'Dropped {4} {1}.'\r\n" +
                                                  "END\r\nGO\r\n";
        // 0 - permission, 1 - schema, 2 - object name, 3 - object type name (lower)
        static readonly string GrantPermissionFormatString = "GRANT {0} ON [{1}].[{2}] TO PUBLIC\r\nPrint '{3} ({2}) created.'\r\nGO\r\n";
    }
}
