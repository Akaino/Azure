//Just a Sample, not actually working Code

int retryCount = 3;
readonly TimeSpan delay = TimeSpan.FromSeconds(5);
public async Task OperationWithBasicRetryAsync()
{
    int currentRetry = 0;

    for (;;)
    {
        try
        {
            await TransientOperationAsync(); break;
        }
        catch (Exception ex)
        {
            Trace.TraceError("Operation Exception");

            if (currentRetry++; > this.retryCount || !IsTransient(ex))
            {
                throw;
            }
        }

        await Task.Delay(delay);
    }
}


private bool IsTransient(Exception ex)
{
    if (ex is OperationTransientException)
    {
        return true;
    }

    var webException = ex as WebException;

    if (webException != null)
    {
        return new[] {
            WebExceptionStatus.ConnectionClosed,
            WebExceptionStatus.Timeout,
            WebExceptionStatus.RequestCanceled
        }.Contains(webException.Status);
    }

    return false;
}
