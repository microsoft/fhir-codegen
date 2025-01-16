using System;
using System.Collections.Generic;
using System.Text;

namespace fhir_codegen.SQLiteGenerator;

public class GeneratorAttributes
{
    internal static HashSet<string> _cgAttributes = [
        "CgSQLiteTable",
        "CgSQLiteKey",
        "CgSQLiteForeignKey",
        "CgSQLiteIgnore",
        ];

    internal static HashSet<string> _cgClassAttributes = [
        "CgSQLiteTable"
        ];

    internal const string CgAttributes = """
        #nullable enable
        namespace fhir_codegen.SQLiteGenerator
        {
            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            public class CgSQLiteTable : System.Attribute
            {
                public string? TableName { get; set; }
                public bool DynamicTableNames { get; set; }

                public CgSQLiteTable(string? tableName = null, bool dynamicTableNames = false)
                {
                    TableName = tableName;
                    DynamicTableNames = dynamicTableNames;
                }
            }

            [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
            public class CgSQLiteKey : System.Attribute
            {
                public bool AutoIncrement { get; set; }
                public CgSQLiteKey(bool autoIncrement = true)
                {
                    AutoIncrement = autoIncrement;
                }
            }

            [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
            public class CgSQLiteForeignKey : System.Attribute
            {
                public string? ReferenceTable { get; set; }
                public string? ReferenceColumn { get; set; }
                public CgSQLiteForeignKey(string? referenceTable = null, string? referenceColumn = null)
                {
                    ReferenceTable = referenceTable;
                    ReferenceColumn = referenceColumn;
                }
            }

            [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
            public class CgSQLiteIgnore : System.Attribute
            {
                public CgSQLiteIgnore()
                {
                }
            }
        }
        #nullable restore
        """;
}
