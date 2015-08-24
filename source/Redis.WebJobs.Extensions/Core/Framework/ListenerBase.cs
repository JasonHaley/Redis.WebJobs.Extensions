using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Redis.WebJobs.Extensions.Config;

namespace Redis.WebJobs.Extensions.Framework
{
    internal abstract class ListenerBase : IListener
    {
        protected readonly CancellationTokenSource _cancellationTokenSource;
        protected bool _disposed;
        
        public ListenerBase()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        protected virtual void OnDisposing()
        {
            
        }
        
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_cancellationTokenSource")]
        public void Dispose()
        {
            if (!_disposed)
            {
                _cancellationTokenSource.Cancel();

                OnDisposing();
                
                _disposed = true;
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
            
            _cancellationTokenSource.Cancel();

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
            _cancellationTokenSource.Cancel();
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(null);
            }
        }
    }
}
