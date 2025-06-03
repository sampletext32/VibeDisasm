
using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Repositories;

public class UserProgramDataRepository
{
    private readonly Dictionary<Guid, byte[]> _userProgramData = [];

    public async Task<byte[]> GetOrLoad(UserProgram program)
    {
        if (_userProgramData.TryGetValue(program.Id, out var bytes))
        {
            return bytes;
        }
        else
        {
            var fileData = await File.ReadAllBytesAsync(program.FilePath);
            _userProgramData[program.Id] = fileData;

            return fileData;
        }
    }
}
