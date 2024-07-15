using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGen._ForPackages;

public class DiskPackageCache : Firely.Fhir.Packages.DiskPackageCache
{
    private bool _syncCacheIniFile;

    public DiskPackageCache(string? rootDirectory = null, bool syncCacheIniFile = false)
        : base(rootDirectory)
    {
        _syncCacheIniFile = syncCacheIniFile;
    }
}
