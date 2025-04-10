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
    private ComparisonDatabase? _db;
    private bool _open = false;

    private string _dbPath;
    private string _dbName;

    private FhirDbComparer? _fhirDbComparer = null;

    public XVerService(XverAppConfig config)
    {
        _config = config;
        _logger = config.LogFactory.CreateLogger<XVerProcessor>();

        if (_config.CrossVersionDbPath.EndsWith(".db"))
        {
            _dbPath = Path.GetDirectoryName(_config.CrossVersionDbPath) ?? _config.CrossVersionDbPath;
            _dbName = Path.GetFileName(_config.CrossVersionDbPath) ?? _config.CrossVersionDbPath;
        }
        else
        {
            _dbPath = _config.CrossVersionDbPath;
            _dbName = "fhir-comparison.db";
        }
    }

    public IDbConnection DbConnection => _db?.DbConnection ?? throw new InvalidOperationException("Database not initialized.");
    public ComparisonDatabase ComparisonDb => _db ?? throw new InvalidOperationException("Database not initialized.");
    public FhirDbComparer Comparer => _fhirDbComparer ?? throw new InvalidOperationException("FhirDbComparer not initialized.");

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
        await Task.Run(() => xverProcessor.WriteDocsFromDatabase());
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_db == null)
        {
            _db = new(_dbPath, _dbName);
            _open = true;
        }

        if (!_open)
        {
            _db.DbConnection.Open();
            _open = true;
        }

        _fhirDbComparer = new(_db, _config.LogFactory);

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
