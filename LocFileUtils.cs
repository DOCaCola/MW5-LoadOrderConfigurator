using System;
using System.IO;
using System.Threading;

namespace MW5_Mod_Manager
{
    internal class LocFileUtils
    {
        public static long GetFileSize(string filePath)
        {
            var fi = new FileInfo(filePath);
            long fileSize = fi.Length;

            // Symlinked files report their file size as zero
            if (fileSize == 0)
            {
                if (fi.LinkTarget != null && fi.DirectoryName != null)
                {
                    string linkTargetPath = Path.GetFullPath(Path.Combine(fi.DirectoryName, fi.LinkTarget));
                    var fiLinkTarget = new FileInfo(linkTargetPath);
                    fileSize = fiLinkTarget.Length;
                }
            }

            return fileSize;
        }

        public static bool IsDirectSubdirectory(string parentPath, string subPath)
        {
            // Get the full paths
            string fullParentPath = Path.GetFullPath(parentPath);
            string fullSubPath = Path.GetFullPath(subPath);

            // Get the directory info
            DirectoryInfo parentDir = new DirectoryInfo(fullParentPath);
            DirectoryInfo subDir = new DirectoryInfo(fullSubPath);

            // Check if the parent of the subdirectory is the parent directory
            return subDir.Parent != null && subDir.Parent.FullName == parentDir.FullName;
        }
    }

    // Async FileWatcher. Allows passing custom type to OnXY functions
    public class FileSystemWatcherAsync<T> : IDisposable
    {
        private FileSystemWatcher _watcher;
        private readonly SynchronizationContext _syncContext;
        private readonly T _customObject;
        private bool _disposed = false;
        private readonly object _lock = new object();

        public event FileSystemEventHandler Changed;
        public event FileSystemEventHandler Created;
        public event FileSystemEventHandler Deleted;
        public event RenamedEventHandler Renamed;

        private bool _watchSuspended = false;

        public FileSystemWatcherAsync(string path, T customObject,
            bool includeSubdirectories, NotifyFilters notifyFilters, bool startSuspended)
        {
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
            _customObject = customObject;
            _watcher = new FileSystemWatcher
            {
                Path = path,
                InternalBufferSize = 64 * 1024,
                IncludeSubdirectories = includeSubdirectories,
                NotifyFilter = notifyFilters,
            };

            _watcher.Changed += OnChangedInternal;
            _watcher.Created += OnCreatedInternal;
            _watcher.Deleted += OnDeletedInternal;
            _watcher.Renamed += OnRenamedInternal;

            _watcher.EnableRaisingEvents = !startSuspended;
            _watchSuspended = startSuspended;
        }

        private void OnChangedInternal(object sender, FileSystemEventArgs e)
        {
            if (_watchSuspended) return;
            _syncContext.Post(_ => OnChanged(sender, e), null);
            //System.Diagnostics.Debug.WriteLine("changed: " + e.FullPath);
        }

        private void OnCreatedInternal(object sender, FileSystemEventArgs e)
        {
            if (_watchSuspended) return;
            _syncContext.Post(_ => OnCreated(sender, e), null);
            //System.Diagnostics.Debug.WriteLine("create: " + e.FullPath);
        }

        private void OnDeletedInternal(object sender, FileSystemEventArgs e)
        {
            if (_watchSuspended) return;
            _syncContext.Post(_ => OnDeleted(sender, e), null);
            //System.Diagnostics.Debug.WriteLine("delete: " + e.FullPath);
        }

        private void OnRenamedInternal(object sender, RenamedEventArgs e)
        {
            if (_watchSuspended) return;
            _syncContext.Post(_ => OnRenamed(sender, e), null);
            //System.Diagnostics.Debug.WriteLine("rename: " + e.FullPath + " " + e.OldFullPath);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_watcher != null)
                    {
                        _watcher.Changed -= OnChangedInternal;
                        _watcher.Created -= OnCreatedInternal;
                        _watcher.Deleted -= OnDeletedInternal;
                        _watcher.Renamed -= OnRenamedInternal;
                        _watcher.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileSystemWatcherAsync()
        {
            Dispose(false);
        }

        protected virtual void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (_watchSuspended) return;
            Changed?.Invoke(sender, e);
        }

        protected virtual void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (_watchSuspended) return;
            Created?.Invoke(sender, e);
        }

        protected virtual void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (_watchSuspended) return;
            Deleted?.Invoke(sender, e);
        }

        protected virtual void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (_watchSuspended) return;
            Renamed?.Invoke(sender, e);
        }

        public void StartWatching()
        {
            lock (_lock)
            {
                _watchSuspended = false;
                _watcher.EnableRaisingEvents = true;
            }
        }

        public void StopWatching()
        {
            lock (_lock)
            {
                _watchSuspended = true;
                _watcher.EnableRaisingEvents = false;
            }
        }

        public T CustomObject => _customObject;
    }

}