using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Comparison.Models;
using Microsoft.Extensions.Logging;

namespace Fhir.CodeGen.Comparison.Outcomes;

public class OutcomeGenerator
{
    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;


    public OutcomeGenerator(
        ComparisonDatabase db,
        ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<FhirDbComparer>();

        _db = db.DbConnection;
    }

    public void GenerateOutcomes(
        bool processValueSets = true,
        bool processStructures = true,
        int? maxStepSize = null)
    {
        // ensure out tables exist and are empty
        DbOutcomeClasses.DropTables(_db, forValueSets: processValueSets, forStructures: processStructures);
        DbOutcomeClasses.CreateTables(_db, forValueSets: processValueSets, forStructures: processStructures);

        if (processValueSets)
        {
            ValueSetOutcomeGenerator vsGenerator = new(_db, _loggerFactory);
            vsGenerator.CreateOutcomesForValueSets(maxStepSize: maxStepSize);
        }

        if (processStructures)
        {
            //// create our structure comparer
            //StructureComparer sdComparer = new(_db, _loggerFactory);

            //// run our structure comparisons
            //sdComparer.CompareStructures(maxStepSize: maxStepSize);
        }
    }
}
