using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class AsyncFileLoader
{
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isDebugMode = false;

    public async Task<MemoryStream> LoadFileAsync(string filePath, Action<MemoryStream> onLoaded)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var memoryStream = new MemoryStream();

        try
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length, _cancellationTokenSource.Token)) > 0)
                {
                    await memoryStream.WriteAsync(buffer, 0, bytesRead, _cancellationTokenSource.Token);

                    if (_isDebugMode)
                    {
                        await Task.Delay(500);
                    }
                }
            }

            memoryStream.Position = 0;
            onLoaded?.Invoke(memoryStream);
        }
        catch (OperationCanceledException)
        {
            //Loading cancelled
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }

        return memoryStream;
    }

    public void CancelLoad()
    {
        _cancellationTokenSource?.Cancel();
    }

    public void ToggleDebugMode(bool isDebug)
    {
        _isDebugMode = isDebug;
    }
}