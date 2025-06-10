using Core.Server.Managers;
using Quartz;

namespace BackgroundJobs.Jobs;

public abstract class BaseJob : IJob, IDisposable
{
    private readonly IMessageBusManager _messageBusManager;
    private readonly ILogger<BaseJob> _logger;
    private static readonly Dictionary<string, object> LockJob = new();

    public IServiceScope? ServiceScope { get; set; }

    protected BaseJob(IMessageBusManager messageBusManager, ILogger<BaseJob> logger, string jobName)
    {
        _messageBusManager = messageBusManager;
        _logger = logger;
        if (!LockJob.TryGetValue(jobName, out _))
            LockJob.Add(jobName, new object());
    }

    public Task Execute(IJobExecutionContext context)
    {
        if (!LockJob.TryGetValue(ToString(), out var jobLock) || 
            !Monitor.TryEnter(jobLock))
            return Task.CompletedTask;

        try
        {
            _logger.LogInformation("Джоба '{0}' начала работу", ToString());
            ExecuteJob().Wait();
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
        finally
        {
            Monitor.Exit(jobLock);
        }
        
        _logger.LogInformation("Джоба '{0}' закончила работу", ToString());

        return Task.CompletedTask;
    }

    protected abstract Task ExecuteJob();
    
    public new abstract string ToString();
    
    protected abstract void LogError(Exception ex);

    public void Dispose()
    {
        ServiceScope?.Dispose();
        GC.SuppressFinalize(this);
    }
}