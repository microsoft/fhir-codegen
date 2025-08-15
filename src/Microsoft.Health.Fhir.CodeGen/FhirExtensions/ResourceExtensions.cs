using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class ResourceExtensions
{
    /// <summary>
    /// Gets the standards status of this definition (e.g., trial-use, normative).
    /// </summary>
    /// <param name="vs">The ValueSet to act on.</param>
    /// <returns>A string representing the standards status.</returns>
    public static string cgStandardStatus(this DomainResource r) => r.GetExtensionValue<Code>(CommonDefinitions.ExtUrlStandardStatus)?.ToString() ?? string.Empty;

    /// <summary>
    /// Gets the FHIR Maturity Model (FMM) level of this definition, or 0 if not specified.
    /// </summary>
    /// <param name="vs">The ValueSet to act on.</param>
    /// <returns>An int representing the FMM level.</returns>
    public static int? cgMaturityLevel(this DomainResource r) => r.GetExtensionValue<Integer>(CommonDefinitions.ExtUrlFmm)?.Value;

    /// <summary>
    /// Gets the Work Group responsible for this definition.
    /// </summary>
    /// <param name="vs">The ValueSet to act on.</param>
    /// <returns>A string representing the Work Group.</returns>
    public static string cgWorkGroup(this DomainResource r)
    {
        IEnumerable<Extension> extensions = r.GetExtensions(CommonDefinitions.ExtUrlWorkGroup);

        foreach (Extension extension in extensions)
        {
            switch (extension.Value)
            {
                case FhirString fhirString:
                    return fhirString.Value ?? string.Empty;
                case Code code:
                    return code.Value ?? string.Empty;
                case Markdown markdown:
                    return markdown.Value ?? string.Empty;
                default:
                    continue;
            }
        }

        return string.Empty;
    }

    public static void cgAddPackageSource(this DomainResource r, string packageId, string? packageVersion, string? packageUri)
    {
        if (string.IsNullOrEmpty(packageId))
        {
            return;
        }

        Extension extPackageSource = new()
        {
            Url = CommonDefinitions.ExtUrlPackageSource,
            Extension = [
                new Extension()
                {
                    Url = "packageId",
                    Value = new Id(packageId),
                }
            ],
        };

        if (!string.IsNullOrEmpty(packageVersion))
        {
            extPackageSource.Extension.Add(new Extension()
            {
                Url = "version",
                Value = new FhirString(packageVersion),
            });
        }

        if (!string.IsNullOrEmpty(packageUri))
        {
            extPackageSource.Extension.Add(new Extension()
            {
                Url = "uri",
                Value = new FhirUri(packageUri),
            });
        }

        if (r.Extension.Any(e => e.Url == CommonDefinitions.ExtUrlPackageSource))
        {
            // If the extension already exists, remove it before adding the new one
            r.Extension.RemoveAll(e => e.Url == CommonDefinitions.ExtUrlPackageSource);
        }

        r.Extension.Add(extPackageSource);
    }
}
