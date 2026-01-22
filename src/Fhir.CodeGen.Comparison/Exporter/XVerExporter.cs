using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.Outcomes;
using Microsoft.Extensions.Logging;

namespace Fhir.CodeGen.Comparison.Exporter;

public class XVerExporter
{
    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private string _outputPath;
    private string? _crossVersionSourcePath;

    internal string _crossDefinitionVersion;
    internal readonly DateTimeOffset _runTime = DateTimeOffset.UtcNow;

    public XVerExporter(
        IDbConnection db,
        ILoggerFactory loggerFactory,
        string outputPath,
        string? crossVersionSourcePath,
        string xverVersion)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<XVerExporter>();

        _db = db;

        _outputPath = outputPath;
        _crossVersionSourcePath = string.IsNullOrEmpty(crossVersionSourcePath) ? null : crossVersionSourcePath;
        _crossDefinitionVersion = string.IsNullOrEmpty(xverVersion) ? "0.0.1-snapshot-3" : xverVersion;
    }

    public void Export(
        bool includeIgScripts = true,
        bool processVocabulary = true,
        bool processStructures = true,
        int? maxStepSize = null)
    {
        // build the main package structure and supporting files
        IgExporter igExporter = new IgExporter(
            _db,
            _loggerFactory,
            _outputPath,
            _crossVersionSourcePath,
            this);

        IgExporter.XVerExportTrackingRecord tr = igExporter.CreateInitialXVerIgs(
            includeIgScripts,
            maxStepSize);

        // export vocabulary (if requested)
        if (processVocabulary)
        {
            VocabularyExporter vocabularyExporter = new VocabularyExporter(
                _db,
                _loggerFactory,
                this);
            vocabularyExporter.Export(tr);
        }

        // export structures (if requested)
        if (processStructures)
        {
        }
    }
}
