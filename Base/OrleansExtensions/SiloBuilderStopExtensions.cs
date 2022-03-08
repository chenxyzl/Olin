using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime;

namespace Base.OrleansExtensions;

public interface IStopTask
{
    /// <summary>
    /// Called after the silo has stop.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token which is canceled when the method must abort.</param>
    /// <returns>A <see cref="Task"/> representing the work performed.</returns>
    Task Execute(CancellationToken cancellationToken);
}

/// <summary>
/// The silo builder stop extensions.
/// </summary>
public static class SiloBuilderStopExtensions
{
    /// <summary>
    /// Adds a stop task to be executed when the silo has stop.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    /// <param name="stage">
    /// The stage to execute the stop task, see values in <see cref="ServiceLifecycleStage"/>.
    /// </param>
    /// <typeparam name="TStop">
    /// The stop task type.
    /// </typeparam>
    /// <returns>
    /// The provided <see cref="ISiloBuilder"/>.
    /// </returns>
    public static ISiloHostBuilder AddStopTask<TStop>(
        this ISiloHostBuilder builder,
        int stage = ServiceLifecycleStage.Last)
        where TStop : class, IStopTask
    {
        return builder.AddStopTask((sp, ct) => ActivatorUtilities.GetServiceOrCreateInstance<TStop>(sp).Execute(ct),
            stage);
    }

    /// <summary>
    /// Adds a stop task to be executed when the silo has stop.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    /// <param name="stopTask">
    /// The stop task.
    /// </param>
    /// <param name="stage">
    /// The stage to execute the stop task, see values in <see cref="ServiceLifecycleStage"/>.
    /// </param>
    /// <returns>
    /// The provided <see cref="ISiloBuilder"/>.
    /// </returns>
    public static ISiloHostBuilder AddStopTask(
        this ISiloHostBuilder builder,
        IStopTask stopTask,
        int stage = ServiceLifecycleStage.Last)
    {
        return builder.AddStopTask((sp, ct) => stopTask.Execute(ct), stage);
    }

    /// <summary>
    /// Adds a stop task to be executed when the silo has stop.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    /// <param name="stopTask">
    /// The stop task.
    /// </param>
    /// <param name="stage">
    /// The stage to execute the stop task, see values in <see cref="ServiceLifecycleStage"/>.
    /// </param>
    /// <returns>
    /// The provided <see cref="ISiloBuilder"/>.
    /// </returns>
    public static ISiloHostBuilder AddStopTask(
        this ISiloHostBuilder builder,
        Func<IServiceProvider, CancellationToken, Task> stopTask,
        int stage = ServiceLifecycleStage.Last)
    {
        builder.ConfigureServices(services =>
            services.AddTransient<ILifecycleParticipant<ISiloLifecycle>>(sp =>
                new StopTask(
                    sp,
                    stopTask,
                    stage)));
        return builder;
    }

    /// <inheritdoc />
    private class StopTask : ILifecycleParticipant<ISiloLifecycle>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Func<IServiceProvider, CancellationToken, Task> stopTask;

        private readonly int stage;

        public StopTask(
            IServiceProvider serviceProvider,
            Func<IServiceProvider, CancellationToken, Task> stopTask,
            int stage)
        {
            this.serviceProvider = serviceProvider;
            this.stopTask = stopTask;
            this.stage = stage;
        }

        /// <inheritdoc />
        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe<StopTask>(
                this.stage,
                cancellation => this.stopTask(this.serviceProvider, cancellation));
        }
    }
}

static class TTT
{
    public static int TT(this int self)
    {
        return self + 1;
    }
}