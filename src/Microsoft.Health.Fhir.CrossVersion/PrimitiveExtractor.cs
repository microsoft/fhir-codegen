// <copyright file="PrimitiveConverter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;

namespace Microsoft.Health.Fhir.CrossVersion;

internal class PrimitiveExtractor
{
    internal bool GetBool(ISourceNode node)
    {
        if (string.IsNullOrEmpty(node.Text) || (!bool.TryParse(node.Text, out bool v)))
        {
            throw new InvalidOperationException($"Invalid boolean value: {node.Text}");
        }

        return v;
    }

    internal bool? GetBoolOpt(ISourceNode node)
    {
        if (string.IsNullOrEmpty(node.Text) || (!bool.TryParse(node.Text, out bool v)))
        {
            return null;
        }

        return v;
    }

    internal long GetLong(ISourceNode node)
    {
        if (string.IsNullOrEmpty(node.Text) || (!long.TryParse(node.Text, out long v)))
        {
            throw new InvalidOperationException($"Invalid long value: {node.Text}");
        }

        return v;
    }

    internal long? GetLongOpt(ISourceNode node)
    {
        if (string.IsNullOrEmpty(node.Text) || (!long.TryParse(node.Text, out long v)))
        {
            return null;
        }

        return v;
    }

    internal int GetInt(ISourceNode node)
    {
        if (string.IsNullOrEmpty(node.Text) || (!int.TryParse(node.Text, out int v)))
        {
            throw new InvalidOperationException($"Invalid int value: {node.Text}");
        }
        
        return v;
    }

    internal int? GetIntOpt(ISourceNode node)
    {
        if (string.IsNullOrEmpty(node.Text) || (!int.TryParse(node.Text, out int v)))
        {
            return null;
        }

        return v;
    }

    internal uint GetUInt(ISourceNode node)
    {
        if (string.IsNullOrEmpty(node.Text) || (!uint.TryParse(node.Text, out uint v)))
        {
            throw new InvalidOperationException($"Invalid uint value: {node.Text}");
        }

        return v;
    }

    internal uint? GetUIntOpt(ISourceNode node)
    {
        if (string.IsNullOrEmpty(node.Text) || (!uint.TryParse(node.Text, out uint v)))
        {
            return null;
        }

        return v;
    }

    internal decimal GetDecimal(ISourceNode node)
    {
        if (string.IsNullOrEmpty(node.Text) || (!decimal.TryParse(node.Text, out decimal v)))
        {
            throw new InvalidOperationException($"Invalid decimal value: {node.Text}");
        }

        return v;
    }

    internal decimal? GetDecimalOpt(ISourceNode node)
    {
        if (string.IsNullOrEmpty(node.Text) || (!decimal.TryParse(node.Text, out decimal v)))
        {
            return null;
        }

        return v;
    }


    internal byte[] GetByteArray(ISourceNode node)
    {
        if (string.IsNullOrEmpty(node.Text))
        {
            throw new InvalidOperationException($"Invalid byte array value: {node.Text}");
        }

        return Convert.FromBase64String(node.Text);
    }

    internal byte[]? GetByteArrayOpt(ISourceNode node)
    {
        if (string.IsNullOrEmpty(node.Text))
        {
            return null;
        }

        try
        {
            return Convert.FromBase64String(node.Text);
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
