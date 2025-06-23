using System.Threading;

namespace BuggyLib;

public static class BuggyCounter
{
    private static object _lock = new object();
    private static long _current = 0;

    public static long NextId()
    {
        lock (_lock)
        {
            long value = _current;
            Thread.Sleep(0);
            _current++;
            return value;
        }
    }
}
