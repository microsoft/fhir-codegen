// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// The type of the measure report.
  /// </summary>
  public static class MeasureReportTypeCodes
  {
    /// <summary>
    /// A data collection report that contains data-of-interest for the measure.
    /// </summary>
    public static readonly Coding DataCollection = new Coding
    {
      Code = "data-collection",
      Display = "Data Collection",
      System = "http://hl7.org/fhir/measure-report-type"
    };
    /// <summary>
    /// An individual report that provides information on the performance for a given measure with respect to a single subject.
    /// </summary>
    public static readonly Coding Individual = new Coding
    {
      Code = "individual",
      Display = "Individual",
      System = "http://hl7.org/fhir/measure-report-type"
    };
    /// <summary>
    /// A subject list report that includes a listing of subjects that satisfied each population criteria in the measure.
    /// </summary>
    public static readonly Coding SubjectList = new Coding
    {
      Code = "subject-list",
      Display = "Subject List",
      System = "http://hl7.org/fhir/measure-report-type"
    };
    /// <summary>
    /// A summary report that returns the number of members in each population criteria for the measure.
    /// </summary>
    public static readonly Coding Summary = new Coding
    {
      Code = "summary",
      Display = "Summary",
      System = "http://hl7.org/fhir/measure-report-type"
    };
  };
}
