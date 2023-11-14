using System;
using System.Collections.Generic;
using System.Text;

namespace fhirCsR2;

internal static class Shim
{
    public static void Write(this MemoryStream ms, ReadOnlySpan<byte> buffer)
    {
        buffer.CopyTo(new Span<byte>(ms.GetBuffer(), (int)ms.Position, buffer.Length));
    }
}
