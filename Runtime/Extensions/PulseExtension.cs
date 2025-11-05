using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class PulseTask
{
    private CancellationTokenSource _cts;
    private Task _task;
    private int _updatesPerSecond;
    private double _updateIntervalMilliseconds;
    private readonly object _lock = new object();

    // 外部传入的同步更新委托
    private readonly Action _updateAction;

    public PulseTask(int updatesPerSecond, Action updateAction)
    {
        if (updatesPerSecond <= 0)
            throw new ArgumentException("updatesPerSecond must be greater than zero.");
        _updatesPerSecond = updatesPerSecond;
        _updateIntervalMilliseconds = 1000.0 / _updatesPerSecond;

        _updateAction = updateAction ?? throw new ArgumentNullException(nameof(updateAction));
    }

    public void Start()
    {
        lock (_lock)
        {
            if (_task != null && !_task.IsCompleted)
                return;

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            _task = Task.Run(
                async () =>
                {
                    var stopwatch = Stopwatch.StartNew();
                    double nextTick = stopwatch.Elapsed.TotalMilliseconds;

                    try
                    {
                        while (!token.IsCancellationRequested)
                        {
                            _updateAction.Invoke();

                            nextTick += _updateIntervalMilliseconds;
                            var sleepTime = nextTick - stopwatch.Elapsed.TotalMilliseconds;

                            if (sleepTime > 1)
                            {
                                try
                                {
                                    await Task.Delay(TimeSpan.FromMilliseconds(sleepTime), token);
                                }
                                catch (TaskCanceledException)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                // 短时忙等待，防止任务频繁执行导致CPU占用过高
                                await Task.Yield();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"PulseTask异常: {ex.Message}");
                    }
                },
                token
            );
        }
    }

    public void Stop()
    {
        lock (_lock)
        {
            if (_cts == null)
                return;

            _cts.Cancel();

            try
            {
                _task?.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Handle(e => e is TaskCanceledException);
            }
            finally
            {
                _cts.Dispose();
                _cts = null;
                _task = null;
            }
        }
    }

    public void SetUpdatesPerSecond(int updatesPerSecond)
    {
        if (updatesPerSecond <= 0)
            throw new ArgumentException("updatesPerSecond must be greater than zero.");

        lock (_lock)
        {
            _updatesPerSecond = updatesPerSecond;
            _updateIntervalMilliseconds = 1000.0 / _updatesPerSecond;
        }
    }
}
