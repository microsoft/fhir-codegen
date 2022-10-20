// <copyright file="Fnv1a52.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.SpecManager.Language.TypeScriptSdk;

/// <summary>A FNV 1a implementation, based on https://github.com/tjwebb/fnv-plus.</summary>
internal class Fnv1a
{
    /// <summary>Hash 52 1a fast.</summary>
    /// <param name="val">The value.</param>
    /// <returns>An ulong.</returns>
    internal static ulong Hash52_1a_fast(string val)
    {
        int i;
        int l = val.Length - 3;
        ulong t0 = 0;
        ulong v0 = 0x2325;
        ulong t1 = 0;
        ulong v1 = 0x8422;
        ulong t2 = 0;
        ulong v2 = 0x9ce4;
        ulong t3 = 0;
        ulong v3 = 0xcbf2;

        for (i = 0; i < l;)
        {
            v0 ^= val[i++];
            t0 = v0 * 435;
            t1 = v1 * 435;
            t2 = v2 * 435;
            t3 = v3 * 435;
            t2 += v0 << 8;
            t3 += v1 << 8;
            t1 += t0 >> 16;
            v0 = t0 & 65535;
            t2 += t1 >> 16;
            v1 = t1 & 65535;
            v3 = (t3 + (t2 >> 16)) & 65535;
            v2 = t2 & 65535;

            v0 ^= val[i++];
            t0 = v0 * 435;
            t1 = v1 * 435;
            t2 = v2 * 435;
            t3 = v3 * 435;
            t2 += v0 << 8;
            t3 += v1 << 8;
            t1 += t0 >> 16;
            v0 = t0 & 65535;
            t2 += t1 >> 16;
            v1 = t1 & 65535;
            v3 = (t3 + (t2 >> 16)) & 65535;
            v2 = t2 & 65535;

            v0 ^= val[i++];
            t0 = v0 * 435;
            t1 = v1 * 435;
            t2 = v2 * 435;
            t3 = v3 * 435;
            t2 += v0 << 8;
            t3 += v1 << 8;
            t1 += t0 >> 16;
            v0 = t0 & 65535;
            t2 += t1 >> 16;
            v1 = t1 & 65535;
            v3 = (t3 + (t2 >> 16)) & 65535;
            v2 = t2 & 65535;

            v0 ^= val[i++];
            t0 = v0 * 435;
            t1 = v1 * 435;
            t2 = v2 * 435;
            t3 = v3 * 435;
            t2 += v0 << 8;
            t3 += v1 << 8;
            t1 += t0 >> 16;
            v0 = t0 & 65535;
            t2 += t1 >> 16;
            v1 = t1 & 65535;
            v3 = (t3 + (t2 >> 16)) & 65535;
            v2 = t2 & 65535;
        }

        while (i < l + 3)
        {
            v0 ^= val[i++];
            t0 = v0 * 435;
            t1 = v1 * 435;
            t2 = v2 * 435;
            t3 = v3 * 435;
            t2 += v0 << 8;
            t3 += v1 << 8;
            t1 += t0 >> 16;
            v0 = t0 & 65535;
            t2 += t1 >> 16;
            v1 = t1 & 65535;
            v3 = (t3 + (t2 >> 16)) & 65535;
            v2 = t2 & 65535;
        }

        return ((v3 & 15) * 281474976710656) + (v2 * 4294967296) + (v1 * 65536) + (v0 ^ (v3 >> 4));
    }
}
