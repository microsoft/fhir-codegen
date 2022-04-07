// <auto-generated />
// Built from: hl7.fhir.r4b.core version: 4.3.0-snapshot1
  // Option: "NAMESPACE" = "fhirCsR4B"

using fhirCsR4B.Models;

namespace fhirCsR4B.ValueSets
{
  /// <summary>
  /// Containers which may hold specimens or specimen containers. Include codes  SNOMED CT(http://snomed.info/sct) where concept is-a 434711009 (Specimen container (physical object))
  /// </summary>
  public static class SpecimenContainerTypeCodes
  {
    /// <summary>
    /// Specimen container (physical object)
    /// </summary>
    public static readonly Coding SpecimenContainerPhysicalObject = new Coding
    {
      Code = "434711009",
      Display = "Specimen container (physical object)",
      System = "http://snomed.info/sct"
    };
    /// <summary>
    /// Specimen vial (physical object)
    /// </summary>
    public static readonly Coding SpecimenVialPhysicalObject = new Coding
    {
      Code = "434746001",
      Display = "Specimen vial (physical object)",
      System = "http://snomed.info/sct"
    };
    /// <summary>
    /// Specimen well (physical object)
    /// </summary>
    public static readonly Coding SpecimenWellPhysicalObject = new Coding
    {
      Code = "434822004",
      Display = "Specimen well (physical object)",
      System = "http://snomed.info/sct"
    };
    /// <summary>
    /// Breath specimen container (physical object)
    /// </summary>
    public static readonly Coding BreathSpecimenContainerPhysicalObject = new Coding
    {
      Code = "713791004",
      Display = "Breath specimen container (physical object)",
      System = "http://snomed.info/sct"
    };

    /// <summary>
    /// Literal for code: SpecimenContainerPhysicalObject
    /// </summary>
    public const string LiteralSpecimenContainerPhysicalObject = "434711009";

    /// <summary>
    /// Literal for code: NONESpecimenContainerPhysicalObject
    /// </summary>
    public const string LiteralNONESpecimenContainerPhysicalObject = "http://snomed.info/sct#434711009";

    /// <summary>
    /// Literal for code: SpecimenVialPhysicalObject
    /// </summary>
    public const string LiteralSpecimenVialPhysicalObject = "434746001";

    /// <summary>
    /// Literal for code: NONESpecimenVialPhysicalObject
    /// </summary>
    public const string LiteralNONESpecimenVialPhysicalObject = "http://snomed.info/sct#434746001";

    /// <summary>
    /// Literal for code: SpecimenWellPhysicalObject
    /// </summary>
    public const string LiteralSpecimenWellPhysicalObject = "434822004";

    /// <summary>
    /// Literal for code: NONESpecimenWellPhysicalObject
    /// </summary>
    public const string LiteralNONESpecimenWellPhysicalObject = "http://snomed.info/sct#434822004";

    /// <summary>
    /// Literal for code: BreathSpecimenContainerPhysicalObject
    /// </summary>
    public const string LiteralBreathSpecimenContainerPhysicalObject = "713791004";

    /// <summary>
    /// Literal for code: NONEBreathSpecimenContainerPhysicalObject
    /// </summary>
    public const string LiteralNONEBreathSpecimenContainerPhysicalObject = "http://snomed.info/sct#713791004";

    /// <summary>
    /// Dictionary for looking up SpecimenContainerType Codings based on Codes
    /// </summary>
    public static Dictionary<string, Coding> Values = new Dictionary<string, Coding>() {
      { "434711009", SpecimenContainerPhysicalObject }, 
      { "http://snomed.info/sct#434711009", SpecimenContainerPhysicalObject }, 
      { "434746001", SpecimenVialPhysicalObject }, 
      { "http://snomed.info/sct#434746001", SpecimenVialPhysicalObject }, 
      { "434822004", SpecimenWellPhysicalObject }, 
      { "http://snomed.info/sct#434822004", SpecimenWellPhysicalObject }, 
      { "713791004", BreathSpecimenContainerPhysicalObject }, 
      { "http://snomed.info/sct#713791004", BreathSpecimenContainerPhysicalObject }, 
    };
  };
}