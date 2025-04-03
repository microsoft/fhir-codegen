using System.Data;

namespace xver_editor.Services;

public interface IXverDbService : IHostedService
{
    IDbConnection DB { get; }
}
