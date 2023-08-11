using System.Collections.Generic;
using Hl7.Fhir.Model;

var edu = new SampleSourceGen.Education
{
    Study = "abcd",
    Graduated = new FhirBoolean(false),
    Subject = new ResourceReference("/Study/abcd")
};

var patient = new SampleSourceGen.Models.Patient
{
    Contact = new List<SampleSourceGen.Models.Patient.ContactComponent> { },
};
