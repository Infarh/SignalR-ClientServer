using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR_Client
{
    internal static class TaskEx
    {
        public static async Task<T> WithCancellation<T>([NotNull] this Task<T> task, CancellationToken cancel)
        {
            var source = new TaskCompletionSource<object>();
            await using (cancel.Register(tcs => ((TaskCompletionSource<object>)tcs).TrySetResult(default), source))
                if (task != await Task.WhenAny(task, source.Task))
                    throw new TaskCanceledException(task);
            return await task;
        }
    }
}