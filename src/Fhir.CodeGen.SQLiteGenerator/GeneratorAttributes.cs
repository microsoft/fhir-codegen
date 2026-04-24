using System;
using System.Collections.Generic;
using System.Text;

namespace Fhir.CodeGen.SQLiteGenerator;

public class GeneratorAttributes
{
    internal const string _cgSQLiteBaseClass =  "CgSQLiteBaseClass";
    internal const string _cgSQLiteTable = "CgSQLiteTable";
    internal const string _cgSQLiteIndex =  "CgSQLiteIndex";
    internal const string _cgSQLiteKey =  "CgSQLiteKey";
    internal const string _cgSQLiteForeignKey =  "CgSQLiteForeignKey";
    internal const string _cgSQLiteIgnore = "CgSQLiteIgnore";

    internal static HashSet<string> _cgAttributes = [
        _cgSQLiteBaseClass,
        _cgSQLiteTable,
        _cgSQLiteIndex,
        _cgSQLiteKey,
        _cgSQLiteForeignKey,
        _cgSQLiteIgnore,
        ];

    internal static HashSet<string> _cgClassAttributes = [
        _cgSQLiteBaseClass,
        _cgSQLiteTable,
        ];

    private const string _protectionModifier = "internal";

    internal const string CgAttributes = $$$"""
        #nullable enable
        namespace Fhir.CodeGen.SQLiteGenerator
        {
            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            {{{_protectionModifier}}} class {{{_cgSQLiteBaseClass}}} : System.Attribute
            {
                public {{{_cgSQLiteBaseClass}}}()
                {
                }
            }

            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            {{{_protectionModifier}}} class {{{_cgSQLiteTable}}} : System.Attribute
            {
                public string? TableName { get; set; }
                public bool DynamicTableNames { get; set; }

                public {{{_cgSQLiteTable}}}(string? tableName = null, bool dynamicTableNames = false)
                {
                    TableName = tableName;
                    DynamicTableNames = dynamicTableNames;
                }
            }

            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
            {{{_protectionModifier}}} class {{{_cgSQLiteIndex}}} : System.Attribute
            {
                public string[] Columns { get; set; }
        
                public {{{_cgSQLiteIndex }}}(params string[] columns)
                {
                    Columns = columns;
                }
            }
        
            [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
            {{{_protectionModifier}}} class {{{_cgSQLiteKey}}} : System.Attribute
            {
                public bool AutoIncrement { get; set; }
                public {{{_cgSQLiteKey}}}(bool autoIncrement = true)
                {
                    AutoIncrement = autoIncrement;
                }
            }

            [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
            {{{_protectionModifier}}} class {{{_cgSQLiteForeignKey}}} : System.Attribute
            {
                public string? ReferenceTable { get; set; }
                public string? ReferenceColumn { get; set; }
                public string? ModelTypeName { get; set; }
                public {{{_cgSQLiteForeignKey}}}(string? referenceTable = null, string? referenceColumn = null, string? modelTypeName = null)
                {
                    ReferenceTable = referenceTable;
                    ReferenceColumn = referenceColumn;
                    ModelTypeName = modelTypeName;
                }
            }

            [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
            {{{_protectionModifier}}} class {{{_cgSQLiteIgnore}}} : System.Attribute
            {
                public {{{_cgSQLiteIgnore}}}()
                {
                }
            }
        }
        #nullable restore
        """;
}
