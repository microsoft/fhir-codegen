// <copyright file="InterProcessSync.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Collections.Concurrent;

namespace Microsoft.Health.Fhir.CodeGen.Utils;

/// <summary>
/// Represents a synchronization mechanism for inter-process communication.
/// </summary>
public class InterProcessSync : IDisposable
{
    private const int _defaultDelay = 60 * 1000;

    private bool _disposing = false;
    private bool _disposedValue = false;

    /// <summary>
    /// Represents the possible states of the inter-process synchronization.
    /// </summary>
    public enum InterProcessSyncStates
    {
        /// <summary>
        /// Requesting ownership of the synchronization resource.
        /// </summary>
        RequestOwnership,

        /// <summary>
        /// Currently owning the synchronization resource.
        /// </summary>
        Owned,

        /// <summary>
        /// Represents the state when waiting for the elapsed time.
        /// </summary>
        WaitElapsed,

        /// <summary>
        /// Requesting release of the synchronization resource.
        /// </summary>
        RequestRelease,

        /// <summary>
        /// Released the synchronization resource.
        /// </summary>
        Released,
    }

    /// <summary>
    /// Represents a record for a mutex request.
    /// </summary>
    private record class MutexRequestRec
    {
        /// <summary>
        /// Gets or sets the request ID.
        /// </summary>
        public required Guid RequestId { get; init; }

        /// <summary>
        /// Gets or sets the name of the mutex.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Gets or sets the timeout value for the mutex request.
        /// </summary>
        public int Timeout { get; init; } = _defaultDelay;

        /// <summary>
        /// Gets or sets the state of the mutex request.
        /// </summary>
        public InterProcessSyncStates State { get; init; } = InterProcessSyncStates.RequestOwnership;

        /// <summary>
        /// Gets or sets the named mutex object.
        /// </summary>
        public Mutex? NamedMutex { get; init; } = null;
    }

    private ConcurrentDictionary<Guid, MutexRequestRec> _mutexRequests;
    private Thread _thread;

    /// <summary>
    /// Initializes a new instance of the <see cref="InterProcessSync"/> class.
    /// </summary>
    public InterProcessSync()
    {
        _mutexRequests = new();
        _thread = new Thread(MutexManagementThreadFunc);
        _thread.Start();
    }

    /// <summary>
    /// Attempts to acquire a lock with the specified name and optional delay.
    /// </summary>
    /// <param name="name">The name of the lock.</param>
    /// <param name="delay">The optional delay in milliseconds. If set to 0, it will return immediately.</param>
    /// <returns>A tuple containing the lock handle and a boolean indicating whether the lock was acquired.</returns>
    public async Task<(Guid? handle, bool ownLock)> TryGetLock(string name, int delay = _defaultDelay)
    {
        Guid handle = Guid.NewGuid();

        _mutexRequests.TryAdd(handle, new MutexRequestRec
        {
            RequestId = handle,
            Name = name,
            Timeout = delay,
            State = InterProcessSyncStates.RequestOwnership,
        });

        if (delay <= 0)
        {
            return (handle, false);
        }

        long maxTicks = DateTime.Now.Ticks + delay;

        while (DateTime.Now.Ticks < maxTicks)
        {
            await Task.Delay(100);

            if (_mutexRequests[handle].State == InterProcessSyncStates.Owned)
            {
                return (handle, true);
            }

            if (_mutexRequests[handle].State == InterProcessSyncStates.WaitElapsed)
            {
                return (handle, false);
            }
        }

        return (handle, false);
    }

    /// <summary>
    /// Attempts to wait for the specified amount of time to acquire the lock associated with the given handle.
    /// </summary>
    /// <param name="handle">The handle associated with the lock.</param>
    /// <param name="delay">The maximum amount of time to wait in milliseconds. If set to 0, it will return immediately.</param>
    /// <returns>True if the lock was acquired within the specified time; otherwise, false.</returns>
    public async Task<bool> TryWait(Guid handle, int delay = _defaultDelay)
    {
        if (!_mutexRequests.TryGetValue(handle, out MutexRequestRec? mrc))
        {
            return false;
        }

        if (delay <= 0)
        {
            return mrc.State == InterProcessSyncStates.Owned;
        }

        // check for needing to re-request
        if ((mrc.State == InterProcessSyncStates.WaitElapsed) ||
            (mrc.State == InterProcessSyncStates.Released))
        {
            _mutexRequests[handle] = mrc with
            {
                State = InterProcessSyncStates.RequestOwnership,
            };
        }

        long maxTicks = DateTime.Now.Ticks + delay;

        while (DateTime.Now.Ticks < maxTicks)
        {
            if (_mutexRequests[handle].State == InterProcessSyncStates.Owned)
            {
                return true;
            }

            if (_mutexRequests[handle].State == InterProcessSyncStates.WaitElapsed)
            {
                return false;
            }

            await Task.Delay(100);
        }

        return false;
    }

    /// <summary>
    /// Releases the lock associated with the specified handle.
    /// </summary>
    /// <param name="handle">The handle associated with the lock.</param>
    public void ReleaseLock(Guid handle)
    {
        if (!_mutexRequests.TryGetValue(handle, out MutexRequestRec? mrc))
        {
            return;
        }

        if (mrc.State == InterProcessSyncStates.Owned)
        {
            _mutexRequests[handle] = mrc with
            {
                State = InterProcessSyncStates.RequestRelease,
            };

            return;
        }

        _mutexRequests[handle] = mrc with
        {
            State = InterProcessSyncStates.Released,
        };

        return;
    }


    /// <summary>
    /// Represents the method that will be executed in the mutex management thread.
    /// </summary>
    private void MutexManagementThreadFunc()
    {
        while (!_disposing)
        {
            System.Threading.Thread.Sleep(100);

            foreach (Guid key in _mutexRequests.Keys)
            {
                try
                {
                    MutexRequestRec mrc = _mutexRequests[key];

                    switch (mrc.State)
                    {
                        case InterProcessSyncStates.RequestOwnership:
                            {
                                Mutex mutex = mrc.NamedMutex ?? new(false, mrc.Name);

                                bool acquired = false;

                                if (mrc.Timeout > 0)
                                {
                                    acquired = mutex.WaitOne(mrc.Timeout);
                                }
                                else
                                {
                                    acquired = mutex.WaitOne(0);
                                }

                                _mutexRequests[key] = mrc with
                                {
                                    State = acquired ? InterProcessSyncStates.Owned : InterProcessSyncStates.WaitElapsed,
                                    NamedMutex = mutex,
                                };
                            }
                            break;

                        case InterProcessSyncStates.RequestRelease:
                            {
                                if (mrc.NamedMutex == null)
                                {
                                    _mutexRequests[key] = mrc with
                                    {
                                        State = InterProcessSyncStates.Released,
                                        NamedMutex = null,
                                    };

                                    continue;
                                }

                                Mutex mutex = mrc.NamedMutex!;

                                mutex.ReleaseMutex();
                                mutex.Dispose();

                                _mutexRequests[key] = mrc with
                                {
                                    State = InterProcessSyncStates.Released,
                                    NamedMutex = null,
                                };
                            }
                            break;
                    }
                }
                catch (ThreadInterruptedException)
                {
                    return;
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in MutexManagementThreadFunc: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="InterProcessSync"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _disposing = true;

                // wait at least one cycle for the thread to exit
                Task.Delay(100).Wait();

                _thread.Interrupt();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
