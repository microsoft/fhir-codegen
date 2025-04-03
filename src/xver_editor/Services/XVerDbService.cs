using System.Data;
using Microsoft.Health.Fhir.Comparison.Models;
using Microsoft.Health.Fhir.Comparison.XVer;
using xver_editor.Config;

namespace xver_editor.Services;

public class XVerDbService : IXverDbService
{
    private XverAppConfig _config;
    private ILogger _logger;
    private ComparisonDatabase? _db;
    private bool _open = false;

    private string _dbPath;
    private string _dbName;

    public XVerDbService(XverAppConfig config)
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

    public IDbConnection DB => _db?.DbConnection ?? throw new InvalidOperationException("Database not initialized.");

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

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _db?.DbConnection.Close();
        _open = false;

        return Task.CompletedTask;
    }
}
