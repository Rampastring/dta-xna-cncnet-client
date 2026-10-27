using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace DTAClient.Domain.Multiplayer.CnCNet;

// The upstream networking code targets modern .NET; DTA still targets Framework 4.8.
internal static class V3FrameworkCompatibility
{
    public static uint CreateNonce()
    {
        var bytes = new byte[4];
        FillRandomBytes(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }

    public static void FillRandomBytes(byte[] bytes)
    {
        using (var random = RandomNumberGenerator.Create())
            random.GetBytes(bytes);
    }

    public static async Task<T> WaitAsync<T>(this Task<T> task, CancellationToken cancellationToken)
    {
        var cancelled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        using (cancellationToken.Register(() => cancelled.TrySetResult(true)))
        {
            if (task != await Task.WhenAny(task, cancelled.Task).ConfigureAwait(false))
                throw new OperationCanceledException(cancellationToken);

            return await task.ConfigureAwait(false);
        }
    }
}
