// <copyright file="Bundle.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Newtonsoft.Json.Linq;

namespace Fhir.CodeGen.CrossVersionLoader.Convert_20_50;

public class Bundle_20_50 : ICrossVersionProcessor<Bundle>
{
    private Converter_20_50 _converter;
    internal Bundle_20_50(Converter_20_50 converter)
    {
        _converter = converter;
    }

    public Bundle Extract(ISourceNode node)
    {
        Bundle current = new Bundle();
        foreach (ISourceNode child in node.Children())
        {
            Process(child, current);
        }
        return current;
    }

    public void Process(ISourceNode node, Bundle current)
    {
        switch (node.Name)
        {
            case "identifier":
                current.Identifier = _converter._identifier.Extract(node);
                break;

            case "type":
                current.TypeElement = new Code<Bundle.BundleType>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Bundle.BundleType>(node.Text));
                break;

            case "timestamp":
                current.Timestamp = Hl7.Fhir.ElementModel.Types.DateTime.Parse(node.Text)?.ToDateTimeOffset(TimeSpan.Zero);
                break;

            case "total":
                if (int.TryParse(node.Text, out int total))
                {
                    current.Total = total;
                }
                break;

            case "link":
                current.Link.Add(Extract20BundleLinkComponent(node));
                break;

            case "entry":
                current.Entry.Add(Extract20BundleEntryComponent(node));
                break;

            case "signature":
                current.Signature = _converter._signature.Extract(node);
                break;

            // process inherited elements
            default:
                _converter._resource.Process(node, current);
                break;
        }
    }

    private Bundle.LinkComponent Extract20BundleLinkComponent(ISourceNode parent)
    {
        Bundle.LinkComponent current = new Bundle.LinkComponent();
        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "relation":
                    current.Relation = node.Text;
                    break;
                case "url":
                    current.Url = node.Text;
                    break;
                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
        }
        return current;
    }

    private Bundle.EntryComponent Extract20BundleEntryComponent(ISourceNode parent)
    {
        Bundle.EntryComponent current = new Bundle.EntryComponent();
        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "link":
                    current.Link.Add(Extract20BundleLinkComponent(node));
                    break;
                case "fullUrl":
                    current.FullUrl = node.Text;
                    break;
                case "resource":
                    current.Resource = _converter._resource.Extract(node);
                    break;
                case "search":
                    current.Search = Extract20BundleSearchComponent(node);
                    break;
                case "request":
                    current.Request = Extract20BundleRequestComponent(node);
                    break;
                case "response":
                    current.Response = Extract20BundleResponseComponent(node);
                    break;
                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
        }
        return current;
    }

    private Bundle.ResponseComponent Extract20BundleResponseComponent(ISourceNode parent)
    {
        Bundle.ResponseComponent current = new Bundle.ResponseComponent();
        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "status":
                    current.Status = node.Text;
                    break;
                case "location":
                    current.Location = node.Text;
                    break;
                case "etag":
                    current.Etag = node.Text;
                    break;
                case "lastModified":
                    current.LastModified = Hl7.Fhir.ElementModel.Types.DateTime.Parse(node.Text)?.ToDateTimeOffset(TimeSpan.Zero);
                    break;
                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
        }
        return current;
    }

    private Bundle.RequestComponent Extract20BundleRequestComponent(ISourceNode parent)
    {
        Bundle.RequestComponent current = new Bundle.RequestComponent();
        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "method":
                    current.Method = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Bundle.HTTPVerb>(node.Text);
                    break;
                case "url":
                    current.Url = node.Text;
                    break;
                case "ifNoneMatch":
                    current.IfNoneMatch = node.Text;
                    break;
                case "ifModifiedSince":
                    current.IfModifiedSince = Hl7.Fhir.ElementModel.Types.DateTime.Parse(node.Text)?.ToDateTimeOffset(TimeSpan.Zero);
                    break;
                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
        }
        return current;
    }

    private Bundle.SearchComponent Extract20BundleSearchComponent(ISourceNode parent)
    {
        Bundle.SearchComponent current = new Bundle.SearchComponent();
        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "mode":
                    current.Mode = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Bundle.SearchEntryMode>(node.Text);
                    break;
                case "score":
                    if (decimal.TryParse(node.Text, out decimal score))
                    {
                        current.Score = score;
                    }
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
