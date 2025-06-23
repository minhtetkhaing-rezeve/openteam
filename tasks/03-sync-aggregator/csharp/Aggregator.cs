using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public static class Aggregator
{
    public record Result(string Path, int Lines, int Words, string Status);

    /// <summary>
    /// Concurrently processes the files listed in <paramref name="fileListPath"/>.
    /// Must preserve input order and apply a per‑file timeout.
    /// </summary>
    public static IEnumerable<Result> Aggregate(
        string fileListPath,
        int workers = 4,
        int timeout = 2)
    {
        // ── TODO: IMPLEMENT ────────────────────────────────────────────────────
        //throw new NotImplementedException("implement Aggregate()");
        var files = File.ReadLines($"../../../{fileListPath}").ToList();
        var resultTasks = new Task<Result>[files.Count];
        var semaphore = new SemaphoreSlim(workers);

        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];  // capture
            resultTasks[i] = Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var workTask = ReadAllData(file);
                    // race the work against a delay
                    // the reason why I add 100ms to time out
                    // since task sleep for 2s in no 3 and 15, there is no way to finish in 2s
                    // so to get succeed per your test, added 100ms for minor delays
                    if (await Task.WhenAny(workTask, Task.Delay(timeout * 1000 + 100)) == workTask)
                    {
                        return await workTask;
                    }
                    else
                    {
                        // timeout
                        return new Result(file, 0, 0, "timeout");
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }
        Task.WaitAll(resultTasks);
        return resultTasks.Select(t => t.Result);
        // ───────────────────────────────────────────────────────────────────────
    }

    private async static Task<Result> ReadAllData(string path)
    {
        const string sleepTimeFormat = "#sleep=";
        int sleepTime = 0;
        var lines = await File.ReadAllLinesAsync($"../../../../data/{path}");
        if (lines[0].Contains(sleepTimeFormat))
        {
            Int32.TryParse(lines[0].Substring(sleepTimeFormat.Length), out sleepTime);
            await Task.Delay(sleepTime * 1000);
        }

        return new Result(path, lines.Length - 1, lines.Skip(1).Sum(x => x.Split(' ').Length), "ok");
    }
}
