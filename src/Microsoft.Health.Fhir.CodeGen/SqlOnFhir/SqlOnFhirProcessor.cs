// <copyright file="SqlOnFhirProcessor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.FhirPath;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.FhirPath;
using Hl7.FhirPath.Expressions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;

namespace Microsoft.Health.Fhir.CodeGen.SqlOnFhir;

public class SqlOnFhirProcessor
{
    /// <summary>The logger.</summary>
    private ILogger _logger;

    private ConfigSql _config;

    private List<ViewDefinition> _viewDefinitions = [];

    /// <summary>(Immutable) The primitive type map - see https://sqlite.org/datatype3.html.</summary>
    private static readonly Dictionary<string, string> _primitiveTypeMap = new Dictionary<string, string>()
    {
        { "base64Binary", "TEXT" },
        { "boolean", "INTEGER" },
        { "canonical", "TEXT" },
        { "code", "TEXT" },
        { "date", "TEXT" },
        { "dateTime", "TEXT" },
        { "decimal", "REAL" },
        { "id", "TEXT" },
        { "instant", "TEXT" },
        { "integer", "INTEGER" },
        { "integer64", "INTEGER" },
        { "oid", "TEXT" },
        { "positiveInt", "INTEGER" },
        { "string", "TEXT" },
        { "time", "TEXT" },
        { "unsignedInt", "INTEGER" },
        { "uri", "TEXT" },
        { "url", "TEXT" },
        { "uuid", "TEXT" },
        { "xhtml", "TEXT" },
        { "markdown", "TEXT" }
    };

    public SqlOnFhirProcessor(ConfigSql? config = null)
    {
        _logger = (config?.LogFactory ?? LoggerFactory.Create(builder => builder.AddConsole())).CreateLogger<PackageLoader>();
        _config = config ?? new();

        // sanity checks
        if (string.IsNullOrEmpty(_config.ViewDefinitionDirectory))
        {
            throw new ArgumentNullException(nameof(_config.ViewDefinitionDirectory));
        }

        if (string.IsNullOrEmpty(_config.OutputDirectory))
        {
            throw new ArgumentNullException(nameof(_config.OutputDirectory));
        }

        if (string.IsNullOrEmpty(_config.ExportDatabaseName))
        {
            throw new ArgumentNullException(nameof(_config.ExportDatabaseName));
        }

        LoadViewDefinitions(_config.ViewDefinitionDirectory);
    }

    public void Process(DefinitionCollection dc)
    {
        SymbolTable st = new SymbolTable().AddStandardFP().AddFhirExtensions();
        FhirPathCompiler fpc = new(st);

        string dbPath = Path.Combine(_config.OutputDirectory, _config.ExportDatabaseName);

        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }

        string connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();

        using SqliteConnection dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();

        // iterate over our view definitions
        foreach (ViewDefinition vd in _viewDefinitions)
        {
            createTable(dbConnection, vd);

            // act depending on the resource the view applies to
            switch (vd.Resource)
            {
                case ResourceType.StructureDefinition:
                    {
                        // TODO(ginoc): need to apply to all Structure Definitions...
                        // iterate over resources
                        foreach (StructureDefinition sd in dc.ResourcesByName.Values)
                        {
                            applyViewDefinition(dbConnection, vd, sd);
                        }
                    }
                    break;
            }
        }

        dbConnection.Close();
    }

    private void applyViewDefinition(SqliteConnection db,ViewDefinition vd, StructureDefinition sd)
    {
        FhirEvaluationContext fpContext = new()
        {
            Environment = new Dictionary<string, IEnumerable<ITypedElement>>()
                {
                },
        };

        //// iterate over our columns to compile 
        //foreach (ViewDefinition.ColumnComponent col in vd.Select.SelectMany(s => s.Column))
        //{
        //    // compile the expression
        //    ExpressionNode sp = FhirPathCompiler.Compile(vc.Expression);

        //    // evaluate the expression
        //    IEnumerable<ITypedElement> extracted = sp.Invoke(sd.ToTypedElement(), fpContext);

        //    // insert into the table
        //    SqliteCommand command = db.CreateCommand();
        //    command.CommandText = $"INSERT INTO {vd.Name} (RowKey, {vc.Name}) VALUES ({sd.Id}, {extracted.First().Value})";
        //    command.ExecuteNonQuery();
        //}

        //IEnumerable<ITypedElement> extracted = sp.CompiledExpression.Invoke(rootNode, fpContext);

    }

    private void createTable(SqliteConnection db, ViewDefinition vd)
    {
        SqliteCommand command = db.CreateCommand();
        command.CommandText = $"""
            CREATE TABLE {vd.Name} (
            RowKey integer NOT NULL,
            {string.Join(", ", vd.Select.SelectMany(s => s.Column).Select(c => $"{c.Name} {_primitiveTypeMap[c.Type]}"))},
            PRIMARY KEY (RowKey))
            """;

        command.ExecuteNonQuery();
    }

    public void LoadViewDefinitions(string viewDefinitionDirectory)
    {
        if (string.IsNullOrEmpty(viewDefinitionDirectory))
        {
            throw new ArgumentNullException(nameof(viewDefinitionDirectory));
        }

        PackageLoader loader = new((ConfigRoot)_config, new()
        {
            JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
        });

        // first, we want to load the type map
        string[] files = Directory.GetFiles(viewDefinitionDirectory, "*.json", SearchOption.AllDirectories);

        foreach (string path in files)
        {
            //object? parsed = loader.ParseContentsSystemTextStream("application/fhir+json", typeof(ViewDefinition), path: path);
            object? parsed = loader.ParseContents50("application/fhir+json", typeof(ViewDefinition), path: path);

            if (parsed is ViewDefinition vd)
            {
                _viewDefinitions.Add(vd);
            }
        }

        //// Load view definitions from the directory
        //_viewDefinitions = loader.LoadPackages([viewDefinitionDirectory], fhirVersion: "5.0.0").Result;
    }

    private ViewDefinition? tryParseViewDefinition(string json)
    {
        return null;

        //// always use lenient parsing
        //ViewDefinition? parsed = _jsonParser.DeserializeResource(json);


        //return parsed;
    }
}
