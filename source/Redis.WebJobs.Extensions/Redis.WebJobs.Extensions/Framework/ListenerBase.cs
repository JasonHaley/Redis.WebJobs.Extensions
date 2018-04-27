
using Microsoft.Azure.WebJobs.Host.Listeners;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Framework
{
    internal abstract class ListenerBase : IListener
    {
        protected readonly CancellationTokenSource CancellationTokenSource;
        protected bool Disposed;

        public ListenerBase()
        {
            CancellationTokenSource = new CancellationTokenSource();
        }

        protected virtual void OnDisposing()
        {

        }

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_cancellationTokenSource")]
        public void Dispose()
        {
            if (!Disposed)
            {
                CancellationTokenSource.Cancel();

                OnDisposing();

                Disposed = true;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            OnStarting();


            return StartAsyncCore(cancellationToken);
        }

        protected abstract Task StartAsyncCore(CancellationToken cancellationToken);

        protected virtual void OnStarting()
        {
            // override if needed
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            OnStopping();

            CancellationTokenSource.Cancel();

            return StopAsyncCore(cancellationToken);
        }

        protected abstract Task StopAsyncCore(CancellationToken cancellationToken);

        protected virtual void OnStopping()
        {
            // override if needed
        }

        public void Cancel()
        {
            ThrowIfDisposed();
            CancellationTokenSource.Cancel();
        }

        protected void ThrowIfDisposed()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(null);
            }
        }
    }
}
