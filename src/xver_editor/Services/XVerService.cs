using System.Data;
using Microsoft.Health.Fhir.Comparison.CompareTool;
using Microsoft.Health.Fhir.Comparison.Models;
using Microsoft.Health.Fhir.Comparison.XVer;
using xver_editor.Config;

namespace xver_editor.Services;

public class XVerService : IXverService
{
    private XverAppConfig _config;
    private ILogger _logger;
    private ComparisonDatabase? _db = null;
    private bool _open = false;

    private string? _dbFilename = null;

    private FhirDbComparer? _fhirDbComparer = null;

    public XVerService(XverAppConfig config)
    {
        _config = config;
        _logger = config.LogFactory.CreateLogger<XVerProcessor>();
    }

    public IDbConnection DbConnection => _db?.DbConnection ?? throw new InvalidOperationException("Database not initialized.");
    public ComparisonDatabase ComparisonDb => _db ?? throw new InvalidOperationException("Database not initialized.");
    public FhirDbComparer Comparer => _fhirDbComparer ?? throw new InvalidOperationException("FhirDbComparer not initialized.");
    public IQueryable<DbFhirPackage> Packages { get; private set; } = Enumerable.Empty<DbFhirPackage>().AsQueryable();
    public bool IsOpen => _open;

    public (bool success, string? message) Init(string? path = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            if (string.IsNullOrEmpty(_config.CrossVersionDbPath))
            {
                return (false, "No database path provided.");
            }

            path = _config.CrossVersionDbPath;
        }

        string dbPath;
        string dbFile;

        if (path.EndsWith(".db"))
        {
            if (!File.Exists(path))
            {
                return (false, $"Database file not found: {path}");
            }

            dbPath = Path.GetDirectoryName(path) ?? "/";
            dbFile = Path.GetFileName(path);
        }
        else
        {
            if (!Directory.Exists(path))
            {
                return (false, $"Database directory not found: {path}");
            }

            if (!File.Exists(Path.Combine(path, "fhir-comparison.db")))
            {
                return (false, $"Database file not specified and fhir-comparison.db not found in path: {path}");
            }

            dbPath = Path.GetDirectoryName(path) ?? "/";
            dbFile = "fhir-comparison.db";
        }

        _dbFilename = Path.Combine(dbPath, dbFile);

        if (_fhirDbComparer != null)
        {
            _fhirDbComparer = null;
        }

        if (_db != null)
        {
            _open = false;
            _db.DbConnection.Close();
            _db = null;
        }

        _db = new(dbPath, dbFile);
        _open = true;

        _db.DbConnection.Open();
        _open = true;

        Packages = DbFhirPackage
            .SelectList(_db.DbConnection, orderByProperties: [nameof(DbFhirPackage.Key)])
            .AsQueryable();

        _fhirDbComparer = new(_db, _config.LogFactory);

        return (true, $"Opened database {_dbFilename}");
    }

    public void CloseDb()
    {
        if (_fhirDbComparer != null)
        {
            _fhirDbComparer = null;
        }

        if (_db != null)
        {
            _db.DbConnection.Close();
            _db = null;
        }

        Packages = Enumerable.Empty<DbFhirPackage>().AsQueryable();
        _open = false;
    }

    public async Task WriteDocsFromDatabase(string? outputDirectory)
    {
        if (_db == null)
        {
            throw new InvalidOperationException("Database not initialized.");
        }

        outputDirectory ??= _config.OutputDirectory;

        if (string.IsNullOrEmpty(outputDirectory))
        {
            throw new ArgumentNullException(nameof(outputDirectory));
        }

        XVerProcessor xverProcessor = new(_db, outputDirectory, _config.LogFactory);
        await Task.Run(() => xverProcessor.WriteDocsFromDatabase(outputDir: outputDirectory));
    }

    public async Task WriteFhirFromDatabase(string? outputDirectory, string? version)
    {
        if (_db == null)
        {
            throw new InvalidOperationException("Database not initialized.");
        }

        outputDirectory ??= _config.OutputDirectory;

        if (string.IsNullOrEmpty(outputDirectory))
        {
            throw new ArgumentNullException(nameof(outputDirectory));
        }

        XVerProcessor xverProcessor = new(_db, outputDirectory, _config.LogFactory);
        await Task.Run(() => xverProcessor.WriteFhirFromDatabase(outputDir: outputDirectory, version: version));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // try to initialize in case there was a database provided via CLI or environment variable
        try
        {
            _ = Init();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize XVerService, user will need to specify a database and initialize.");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _fhirDbComparer = null;
        _db?.DbConnection.Close();
        _open = false;

        return Task.CompletedTask;
    }
}
