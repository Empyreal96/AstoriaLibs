using System.Runtime.InteropServices;

namespace AowUser
{
    [ComImport]
    [CoClass(typeof(AoWUserSessionClass))]
    [Guid("623BBB59-1AA9-46B2-A2E6-E4A749305FCD")]
    public interface AoWUserSession : IAoWSession
    {
    }
}
