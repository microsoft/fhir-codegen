// <copyright file="ComparisonDbUtils.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using fhir_codegen.SQLiteGenerator;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.CompareTool;

public static class ComparisonDbUtils
{
    /// <summary>
    /// Gets the SQL type for a given type.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string getSqlType(Type type) => _sqliteTypeMap.TryGetValue(type, out string? name) ? name : "TEXT";

    // Mapping pulled from https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/types
    private static Dictionary<Type, string> _sqliteTypeMap = new Dictionary<Type, string>
    {
        { typeof(bool), "INTEGER" },
        { typeof(byte), "INTEGER" },
        { typeof(byte[]), "BLOB" },
        { typeof(char), "TEXT" },
        { typeof(DateOnly), "TEXT" },
        { typeof(DateTime), "TEXT" },
        { typeof(DateTimeOffset), "TEXT" },
        { typeof(Decimal), "TEXT" },
        { typeof(double), "REAL" },
        { typeof(Guid), "TEXT" },
        //{ typeof(Int16), "INTEGER" },
        { typeof(short), "INTEGER" },
        //{ typeof(Int32), "INTEGER" },
        { typeof(int), "INTEGER" },
        //{ typeof(Int64), "INTEGER" },
        { typeof(long), "INTEGER" },
        { typeof(sbyte), "INTEGER" },
        { typeof(Single), "REAL" },
        { typeof(string), "TEXT" },
        { typeof(TimeOnly), "TEXT" },
        { typeof(TimeSpan), "TEXT" },
        //{ typeof(UInt16), "INTEGER" },
        { typeof(ushort), "INTEGER" },
        //{ typeof(UInt32), "INTEGER" },
        { typeof(uint), "INTEGER" },
        //{ typeof(UInt64), "INTEGER" },
        { typeof(ulong), "INTEGER" },
        { typeof(Uri), "TEXT" },
    };

    public static bool CreateTable<T>(IDbConnection dbConnection, string? name = null)
    {
        // use the class name if we do not have an explicit one
        name ??= typeof(T).Name;

        List<string> foreignKeys = [];

        StringBuilder sb = new();

        sb.Append($@"CREATE TABLE IF NOT EXISTS {name} (");
        int colIndex = 0;
        foreach (System.Reflection.PropertyInfo prop in typeof(T).GetProperties())
        {
            object[] attributes = prop.GetCustomAttributes(true);

            ComparisonPrimaryKeyAttribute? pk = attributes.Where(o => o is ComparisonPrimaryKeyAttribute).Select(o => (ComparisonPrimaryKeyAttribute)o).FirstOrDefault<ComparisonPrimaryKeyAttribute>() ?? null;
            ComparisonForeignKeyAttribute? fk = attributes.Where(o => o is ComparisonForeignKeyAttribute).Select(o => (ComparisonForeignKeyAttribute)o).FirstOrDefault<ComparisonForeignKeyAttribute>() ?? null;

            //bool nullable = Nullable.GetUnderlyingType(prop.PropertyType) != null;
            bool nullable = new NullabilityInfoContext().Create(prop).WriteState is NullabilityState.Nullable;

            if (colIndex++ > 0)
            {
                sb.Append(", ");
            }

            sb.Append(
                $"{prop.Name} {getSqlType(prop.PropertyType)}" +
                $"{(pk != null ? " IDENTITY(1,1) PRIMARY KEY" : string.Empty)}" +
                $"{(nullable ? string.Empty : " NOT NULL")}");

            // check for foreign key attribute on this property (add after columns)
            if (fk != null)
            {
                foreignKeys.Add($"FOREIGN KEY ({prop.Name}) REFERENCES {fk.ForeignTable}({fk.ForeignColumn})");
            }
        }

        foreach (string fkLine in foreignKeys)
        {
            sb.Append(fkLine);
        }

        sb.Append(")");

        dbConnection.Execute(sb.ToString());

        return true;
    }

    //public static List<T> Insert<T>(IDbConnection dbConnection, List<T> recs, string? tableName)
    //{
    //    tableName ??= typeof(T).Name;


    //}

    public static List<T> SelectList<T>(IDbConnection dbConnection, string? tableName = null, int? id = null)
    {
        tableName ??= typeof(T).Name;

        if (id == null)
        {
            return dbConnection.Query<T>($"SELECT * FROM {tableName}").ToList();
        }

        return dbConnection.Query<T>($"SELECT * FROM {tableName} WHERE Id = {id}").ToList();
    }

    public static T? SelectSingle<T>(IDbConnection dbConnection, string? tableName = null, int? id = null)
    {
        tableName ??= typeof(T).Name;

        if (id == null)
        {
            return dbConnection.Query<T>($"SELECT * FROM {tableName}").SingleOrDefault();
        }

        return dbConnection.Query<T>($"SELECT * FROM {tableName} WHERE Id = {id}").SingleOrDefault();
    }
}

public class ComparisonPrimaryKeyAttribute : Attribute
{ }

public class ComparisonForeignKeyAttribute : Attribute
{
    public required string ForeignTable { get; init; }
    public required string ForeignColumn { get; init; }

    [SetsRequiredMembers]
    public ComparisonForeignKeyAttribute(string foreignTable, string foreignColumn)
    {
        ForeignTable = foreignTable;
        ForeignColumn = foreignColumn;
    }
}

public partial class ComparisonPackageInfo
{
    [ComparisonPrimaryKeyAttribute]
    public required int Id { get; set; }

    public required string SourcePackageId { get; init; }
    public required string SourcePackageVersion { get; init; }
    
    public required string TargetPackageId { get; init; }
    public required string TargetPackageVersion { get; init; }
}

/// <summary>
/// Represents a value set mapping record.
/// </summary>
/// <remarks>
/// These records appear in a table named ValueSetMappings.
/// </remarks>
public class ValueSetMapping
{
    [ComparisonPrimaryKeyAttribute]
    public required int Id { get; set; }

    public required string SourceCanonical { get; set; }
    public required string SourceName { get; set; }

    public required string? TargetCanonical { get; set; }
    public required string? TargetName { get; set; }

    public required string? CompositeName { get; set; }

    public required CMR? AggregateRelationship { get; set; }
    public required string? Comment { get; set; }

    public required string? LastReviewedBy { get; set; }
    public required DateTime? LastReviewedOn { get; set; }
}

/// <summary>
/// Represents a value set code mapping record.
/// </summary>
/// <remarks>
/// These records appear in tables named "ValueSet-" + ValueSetMapping.CompositeName.
/// </remarks>
public class ValueSetCodeMappingRec
{
    [ComparisonPrimaryKeyAttribute]
    public required int Id { get; set; }

    [ComparisonForeignKeyAttribute("ValueSetMappings", "Id")]
    public required int ValueSetMappingId { get; set; }

    public required string SourceSystem { get; set; }
    public required string SourceCode { get; set; }
    public required string SourceDisplay { get; set; }

    public required bool IsNotMapped { get; set; }

    public required string? TargetSystem { get; set; }
    public required string? TargetCode { get; set; }
    public required string? TargetDisplay { get; set; }

    public required CMR? Relationship { get; set; }
    public required string? Comment { get; set; }
    public required bool IsGenerated { get; set; }
    public bool HasBeenReviewed { get; set; } = false;
}


/// <summary>
/// Represents a structure definition mapping record.
/// </summary>
/// <remarks>
/// These records appear in a table named StructureMappings.
/// </remarks>
public class StructureMapping
{
    [ComparisonPrimaryKeyAttribute]
    public required int Id { get; set; }

    public required string SourceCanonical { get; set; }
    public required string SourceName { get; set; }
    public required FhirArtifactClassEnum SourceArtifactClass { get; set; }

    public required string TargetCanonical { get; set; }
    public required string TargetName { get; set; }
    public required FhirArtifactClassEnum TargetArtifactClass { get; set; }

    public required string CompositeName { get; set; }

    public required CMR? AggregateRelationship { get; set; }
    public required string? Comment { get; set; }

    public required string? LastReviewedBy { get; set; }
    public required DateTimeOffset? LastReviewedOn { get; set; }
}


/// <summary>
/// Represents a Structure Element mapping record.
/// </summary>
/// <remarks>
/// These records appear in tables named "Structure-" + StructureMappingRec.CompositeName.
/// </remarks>
public class StructureElementMappingRec
{
    [ComparisonPrimaryKeyAttribute]
    public required int Id { get; set; }

    [ComparisonForeignKeyAttribute("StructureMapping", "Id")]
    public required int StructureMappingId { get; set; }

    public required string SourceResourceId { get; set; }
    public required string SourcePath { get; set; }
    public required string? SourceType { get; set; }

    public required bool IsNotMapped { get; set; }

    public required string? TargetResourceId { get; set; }
    public required string? TargetPath { get; set; }
    public required string? TargetType { get; set; }

    public required CMR? Relationship { get; set; }
    public required CMR? ConceptDomainRelationship { get; set; }
    public required CMR? ValueDomainRelationship { get; set; }
    public required string? Comment { get; set; }
    public required bool IsGenerated { get; set; }
    public bool HasBeenReviewed { get; set; } = false;
}
