using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.LangSQLite.Models;

[CgSQLiteTable(tableName: "CodeSystemConceptProperties")]
[CgSQLiteIndex(nameof(CodeSystemPropertyDefinitionKey))]
public partial class CgDbCodeSystemConceptProperty : CgDbPackageContentBase
{
    [CgSQLiteForeignKey(referenceTable: "CodeSystemConcepts", referenceColumn: nameof(CgDbCodeSystemConcept.Key))]
    public required int CodeSystemConceptKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "CodeSystemPropertyDefinitions", referenceColumn: nameof(CgDbCodeSystemPropertyDefinition.Key))]
    public required int CodeSystemPropertyDefinitionKey { get; set; }

    public required string Code { get; set; }
    public required Hl7.Fhir.Model.CodeSystem.PropertyType Type { get; set; }
    public required string Value { get; set; }
}
