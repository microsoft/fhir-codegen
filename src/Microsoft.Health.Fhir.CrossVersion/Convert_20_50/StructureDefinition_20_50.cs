// <copyright file="StructureDefinition.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Snapshot;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class StructureDefinition_20_50 : ICrossVersionProcessor<StructureDefinition>, ICrossVersionExtractor<StructureDefinition>
{
    private Converter_20_50 _converter;
    internal StructureDefinition_20_50(Converter_20_50 converter)
    {
        _converter = converter;
    }

    private string _lastKindLiteral = string.Empty;
    //string _lastConstrainedType = string.Empty;

    public StructureDefinition Extract(ISourceNode node)
    {
        StructureDefinition v = new();

        _lastKindLiteral = string.Empty;
        //_lastConstrainedType = string.Empty;

        foreach (ISourceNode child in node.Children())
        {
            Process(child, v);
        }

        v.Derivation = string.IsNullOrEmpty(v.Type) ? StructureDefinition.TypeDerivationRule.Specialization : StructureDefinition.TypeDerivationRule.Constraint;

        // check for incorrect mapping of quantity-derived types
        if (v.Type == "Quantity")
        {
            v.Derivation = StructureDefinition.TypeDerivationRule.Specialization;
        }

        // determine the kind - need to map odd cases from DSTU2 to later element after other processing
        switch (_lastKindLiteral)
        {
            case "datatype":
                {
                    // check for primitive types (known list)
                    if (FhirTypeUtils.TryGetPrimitiveInfo(v.Id, out FhirTypeUtils.FhirPrimitiveInfoRec primitiveInfo))
                    {
                        Normalization.ReconcilePrimitiveType(v, primitiveInfo);
                    }
                    // extension is really a resource, not a type
                    else if (v.Type?.Equals("Extension", StringComparison.Ordinal) ?? false)
                    {
                        v.Kind = StructureDefinition.StructureDefinitionKind.Resource;
                    }
                    else
                    {
                        v.Kind = StructureDefinition.StructureDefinitionKind.ComplexType;

                        if (v.Type == null)
                        {
                            if (!string.IsNullOrEmpty(v.BaseDefinition))
                            {
                                v.Type = v.BaseDefinition.Split('/').Last();
                            }
                            else
                            {
                                v.Type = v.Id;
                            }
                        }
                    }
                }
                break;

            case "resource":
                {
                    v.Kind = StructureDefinition.StructureDefinitionKind.Resource;
                    v.Type ??= v.Id;
                }
                break;

            case "logical":
                v.Kind = StructureDefinition.StructureDefinitionKind.Logical;
                break;
        }

        // check for a title in the name
        if (v.Name.Contains(' '))
        {
            v.Title = v.Name;
            v.Name = string.Join(string.Empty, v.Name.Split(' ', '-', StringSplitOptions.RemoveEmptyEntries).Select(n => n = char.ToUpperInvariant(n[0]) + n.Substring(1)));
        }

        // reconcile the slice names for this structure manually
        BuildElementIds(v);

        // normalize element repetitions (slicing was separate)
        ReconcileElementRepetitions(v);

        // ensure the root element has a base type and valid min/max values
        Normalization.VerifyRootElementType(v);

        return v;
    }


    private void BuildElementIds(StructureDefinition sd)
    {
        List<string> pathComponents = [];
        List<string?> sliceNameAtLoc = [];

        // process the snapshot if we have one
        if (sd.Snapshot?.Element.Any() ?? false)
        {
            processElements(sd.Snapshot.Element);
        }

        pathComponents.Clear();
        sliceNameAtLoc.Clear();

        // process the differential if we have one
        if (sd.Differential?.Element.Any() ?? false)
        {
            processElements(sd.Differential.Element);
        }

        return;

        void processElements(IEnumerable<ElementDefinition> source)
        {
            HashSet<string> allPaths = [];

            // iterate over the the elements in the snapshot
            foreach (ElementDefinition ed in source)
            {
                // ignore the root element so we don't pollute our slice names
                if (!ed.Path.Contains('.'))
                {
                    ed.ElementId = ed.Path;
                    pathComponents = [ed.Path];
                    sliceNameAtLoc = [null];
                    continue;
                }

                // split the path into components
                string[] edPathComponents = ed.Path.Split('.');

                // determine if this is a new slice
                bool edIsNewSlice = !string.IsNullOrEmpty(ed.SliceName);

                // the slice name could be an alias, determine by seeing if we have seen this path before
                if (edIsNewSlice)
                {
                    // if we have not seen this element before, it is an alias
                    if (!allPaths.Contains(ed.Path))
                    {
                        allPaths.Add(ed.Path);
                        ed.SliceName = null;
                        edIsNewSlice = false;
                        ed.AliasElement.Add(new FhirString(ed.SliceName));
                    }
                }
                else if (!allPaths.Contains(ed.Path))
                {
                    allPaths.Add(ed.Path);
                }

                // determine if this is a child element
                if (edPathComponents.Length > pathComponents.Count)
                {
                    // add our path and slice info
                    for (int i = pathComponents.Count; i < edPathComponents.Length; i++)
                    {
                        pathComponents.Add(edPathComponents[i]);
                        sliceNameAtLoc.Add(null);
                    }
                    sliceNameAtLoc[^1] = edIsNewSlice ? getFilteredSliceName(ed.SliceName!, edPathComponents) : null;

                    // build our id
                    ed.ElementId = getId();

                    // no need to process further
                    continue;
                }

                // determine if this is a sibling element
                if (edPathComponents.Length == pathComponents.Count)
                {
                    // update our path and slice info
                    pathComponents[^1] = edPathComponents[^1];
                    sliceNameAtLoc[^1] = edIsNewSlice ? getFilteredSliceName(ed.SliceName!, edPathComponents) : null;

                    // build our id
                    ed.ElementId = getId();

                    // no need to process further
                    continue;
                }

                // determine if we are moving up the tree
                if (edPathComponents.Length < pathComponents.Count)
                {
                    // remove the extra elements from our path and slice info
                    while (edPathComponents.Length < pathComponents.Count)
                    {
                        pathComponents.RemoveAt(pathComponents.Count - 1);
                        sliceNameAtLoc.RemoveAt(sliceNameAtLoc.Count - 1);
                    }

                    // update our path and slice info
                    pathComponents[^1] = edPathComponents[^1];
                    sliceNameAtLoc[^1] = edIsNewSlice ? getFilteredSliceName(ed.SliceName!, edPathComponents) : null;

                    // build our id
                    ed.ElementId = getId();

                    // no need to process further
                    continue;
                }
            }
        }

        string? getFilteredSliceName(string sliceName, string[] slicePathComponents)
        {
            if (string.IsNullOrEmpty(sliceName))
            {
                return null;
            }

            if (!sliceName.Contains('.'))
            {
                return sliceName;
            }

            string[] sliceComponents = sliceName.Split('.').Where(sc => !slicePathComponents.Any(pc => pc == sc)).ToArray();

            return sliceComponents.ToCamelCaseWord();
        }

        string getId()
        {
            StringBuilder sb = new();

            for (int i = 0; i < pathComponents.Count; i++)
            {
                sb.Append(pathComponents[i]);

                if (!string.IsNullOrEmpty(sliceNameAtLoc[i]))
                {
                    sb.Append(":");
                    sb.Append(sliceNameAtLoc[i]);
                }

                if (i < pathComponents.Count - 1)
                {
                    sb.Append(".");
                }
            }

            return sb.ToString();
        }
    }


    internal static void ReconcileElementRepetitions(StructureDefinition sd)
    {
        bool hasSnapshot = sd.Snapshot?.Element.Any() ?? false;
        bool hasDifferential = sd.Differential?.Element.Any() ?? false;

        if ((!hasSnapshot) && (!hasDifferential))
        {
            return;
        }

        Dictionary<string, ElementDefinition> elementsById = [];
        List<ElementDefinition> elementsToRemove = [];

        if (hasSnapshot)
        {
            discoverDuplicates(sd.Snapshot!.Element);

            if (elementsToRemove.Count == 0)
            {
                return;
            }

            // remove these elements from the snapshot
            elementsToRemove.ForEach(ed => sd.Snapshot.Element.Remove(ed));

            // remove matching elements from the differential
            if (hasDifferential)
            {
                elementsToRemove.ForEach(ed => sd.Differential!.Element.RemoveAll(d => d.ElementId == ed.ElementId));
            }

            return;
        }

        discoverDuplicates(sd.Differential!.Element);

        if (elementsToRemove.Count == 0)
        {
            return;
        }

        // remove these elements from the snapshot
        elementsToRemove.ForEach(ed => sd.Differential.Element.Remove(ed));

        return;

        void discoverDuplicates(List<ElementDefinition> source)
        {
            Dictionary<string, ElementDefinition> edById = [];

            for (int i = 0; i < source.Count; i++)
            {
                ElementDefinition ed = source[i];

                // if this id is not present, add it and continue
                if (!edById.TryGetValue(ed.ElementId, out ElementDefinition? existing))
                {
                    edById.Add(ed.ElementId, ed);
                    continue;
                }

                // reconcile slicing to the first element we ran into
                if ((existing.Slicing == null) && (ed.Slicing != null))
                {
                    existing.Slicing = (ElementDefinition.SlicingComponent)ed.Slicing.DeepCopy();

                    // remove this element and move  our index back
                    source.RemoveAt(i);
                    i--;
                    continue;
                }

                if ((existing.Slicing != null) && (ed.Slicing == null))
                {
                    // remove this element and move  our index back
                    source.RemoveAt(i);
                    i--;
                    continue;
                }

                // iterate over our new element discriminators
                foreach (ElementDefinition.DiscriminatorComponent edD in ed.Slicing?.Discriminator ?? Enumerable.Empty<ElementDefinition.DiscriminatorComponent>())
                {
                    // if this discriminator is not already present, add it
                    if (!existing.Slicing!.Discriminator.Any(d => d.Type == edD.Type && d.Path == edD.Path))
                    {
                        existing.Slicing!.Discriminator.Add((ElementDefinition.DiscriminatorComponent)edD.DeepCopy());
                    }
                }

                // remove this element and move  our index back
                source.RemoveAt(i);
                i--;
                continue;
            }
        }
    }

    public void Process(ISourceNode node, StructureDefinition current)
    {
        switch (node.Name)
        {
            case "url":
                current.UrlElement = new FhirUri(node.Text);
                break;

            case "_url":
                _converter._element.Process(node, current.UrlElement);
                break;

            case "identifier":
                current.Identifier.Add(_converter._identifier.Extract(node));
                break;

            case "version":
                current.VersionElement = new FhirString(node.Text);
                break;

            case "_version":
                _converter._element.Process(node, current.VersionElement);
                break;

            case "name":
                //current.TitleElement = new FhirString(node.Text);
                current.NameElement = new FhirString(node.Text);
                break;

            case "_name":
                //_converter._element.Process(node, current.TitleElement);
                _converter._element.Process(node, current.NameElement);
                break;

            case "title":
                current.TitleElement = new FhirString(node.Text);
                break;

            case "_title":
                _converter._element.Process(node, current.TitleElement);
                break;

            case "status":
                current.StatusElement = new Code<PublicationStatus>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<PublicationStatus>(node.Text));
                break;

            case "_status":
                _converter._element.Process(node, current.StatusElement);
                break;

            case "experimental":
                current.ExperimentalElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
                break;

            case "_experimental":
                _converter._element.Process(node, current.ExperimentalElement);
                break;

            case "date":
                current.DateElement = new FhirDateTime(node.Text);
                break;

            case "_date":
                _converter._element.Process(node, current.DateElement);
                break;

            case "publisher":
                current.PublisherElement = new FhirString(node.Text);
                break;

            case "_publisher":
                _converter._element.Process(node, current.PublisherElement);
                break;

            case "contact":
                current.Contact.Add(_converter._contactDetail.Extract(node));
                break;

            case "description":
                current.DescriptionElement = new Markdown(node.Text);
                break;

            case "_description":
                _converter._element.Process(node, current.DescriptionElement);
                break;

            case "useContext":
                current.UseContext.Add(_converter._usageContext.Extract(node));
                break;

            case "jurisdiction":
                current.Jurisdiction.Add(_converter._codeableConcept.Extract(node));
                break;

            case "purpose":
                current.PurposeElement = new Markdown(node.Text);
                break;

            case "_purpose":
                _converter._element.Process(node, current.PurposeElement);
                break;

            case "copyright":
                current.CopyrightElement = new Markdown(node.Text);
                break;

            case "_copyright":
                _converter._element.Process(node, current.CopyrightElement);
                break;

            case "keyword":
                current.Keyword.Add(_converter._coding.Extract(node));
                break;

            case "fhirVersion":
                current.FhirVersionElement = new Code<FHIRVersion>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<FHIRVersion>(node.Text));
                break;

            case "mapping":
                current.Mapping.Add(Extract20StructureDefinitionMappingComponent(node));
                break;

            case "kind":
                _lastKindLiteral = node.Text;
                break;

            case "_kind":
                _converter._element.Process(node, current.KindElement);
                break;

            case "abstract":
                current.AbstractElement = new FhirBoolean(_converter._primitive.GetBool(node));
                break;

            case "_abstract":
                _converter._element.Process(node, current.AbstractElement);
                break;

            case "contextType":
                // element StructureDefinition.contextType has been removed in the target spec
                //contextTypeLiteral = node.Text;
                break;

            case "context":
                // element StructureDefinition.context has been changed in the target spec too much for first pass conversion
                {
                    current.Context.Add(new StructureDefinition.ContextComponent()
                    {
                        TypeElement = new Code<StructureDefinition.ExtensionContextType>(StructureDefinition.ExtensionContextType.Element),
                        ExpressionElement = new FhirString(node.Text)
                    });


                    //foreach (ISourceNode item in node.Children())
                    //{
                    //    current.Context.Add(new StructureDefinition.ContextComponent()
                    //    {
                    //        TypeElement = new Code<StructureDefinition.ExtensionContextType>(StructureDefinition.ExtensionContextType.Element),
                    //        ExpressionElement = new FhirString(item.Text)
                    //    });
                    //}
                }
                break;

            case "contextInvariant":
                current.ContextInvariantElement.Add(new FhirString(node.Text));
                break;

            case "type":
            case "constrainedType":
                current.TypeElement = new FhirUri(node.Text);
                //_lastConstrainedType = node.Text;
                break;

            case "base":
            case "baseDefinition":
                current.BaseDefinitionElement = new Canonical(node.Text);
                break;

            case "derivation":
                current.Derivation = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<StructureDefinition.TypeDerivationRule>(node.Text);
                break;

            case "_derivation":
                _converter._element.Process(node, current.DerivationElement);
                break;

            case "snapshot":
                current.Snapshot = Extract20StructureDefinitionSnapshotComponent(node);
                break;

            case "differential":
                current.Differential = Extract20StructureDefinitionDifferentialComponent(node);
                break;

            // process inherited elements
            default:
                _converter._domainResource.Process(node, current);
                break;

        }
    }


	private StructureDefinition.MappingComponent Extract20StructureDefinitionMappingComponent(ISourceNode parent)
	{
		StructureDefinition.MappingComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "identity":
					current.IdentityElement = new Id(node.Text);
					break;

				case "_identity":
					_converter._element.Process(node, current.IdentityElement);
					break;

				case "uri":
					current.UriElement = new FhirUri(node.Text);
					break;

				case "_uri":
					_converter._element.Process(node, current.UriElement);
					break;

				case "name":
					current.NameElement = new FhirString(node.Text);
					break;

				case "_name":
					_converter._element.Process(node, current.NameElement);
					break;

				case "comment":
					current.CommentElement = new FhirString(node.Text);
					break;

				case "_comment":
					_converter._element.Process(node, current.CommentElement);
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private StructureDefinition.SnapshotComponent Extract20StructureDefinitionSnapshotComponent(ISourceNode parent)
	{
		StructureDefinition.SnapshotComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "element":
					current.Element.Add(_converter._elementDefinition.Extract(node));
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private StructureDefinition.DifferentialComponent Extract20StructureDefinitionDifferentialComponent(ISourceNode parent)
	{
		StructureDefinition.DifferentialComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "element":
					current.Element.Add(_converter._elementDefinition.Extract(node));
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}
}
