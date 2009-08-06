using System;
using System.IO;

namespace ExportDatabaseObjects
{
    public abstract class DbSchemaObjectBase
    {
        public abstract DbObjectType DbSchemaObjectType { get; }
        public abstract bool IsSystemObject { get; }
        public abstract string Name { get; }
        public abstract void Script(TextWriter tw);
        public abstract DateTime DateLastModified { get; }
    }
}