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
    public class FileSystemWatcherAsync<T>
    {
        private readonly FileSystemWatcher _watcher;
        private readonly SynchronizationContext _syncContext;
        private readonly T _customObject;

        public event FileSystemEventHandler Changed;
        public event FileSystemEventHandler Created;
        public event FileSystemEventHandler Deleted;
        public event RenamedEventHandler Renamed;

        public FileSystemWatcherAsync(string path, T customObject,
            bool includeSubdirectories, NotifyFilters notifyFilters)
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

            _watcher.Changed += (sender, e) => _syncContext.Post(_ => OnChanged(sender, e), null);
            _watcher.Created += (sender, e) => _syncContext.Post(_ => OnCreated(sender, e), null);
            _watcher.Deleted += (sender, e) => _syncContext.Post(_ => OnDeleted(sender, e), null);
            _watcher.Renamed += (sender, e) => _syncContext.Post(_ => OnRenamed(sender, e), null);

            _watcher.EnableRaisingEvents = true;
        }

        protected virtual void OnChanged(object sender, FileSystemEventArgs e)
        {
            Changed?.Invoke(sender, e);
        }

        protected virtual void OnCreated(object sender, FileSystemEventArgs e)
        {
            Created?.Invoke(sender, e);
        }

        protected virtual void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Deleted?.Invoke(sender, e);
        }

        protected virtual void OnRenamed(object sender, RenamedEventArgs e)
        {
            Renamed?.Invoke(sender, e);
        }

        public void StartWatching()
        {
            _watcher.EnableRaisingEvents = true;
        }

        public void StopWatching()
        {
            _watcher.EnableRaisingEvents = false;
        }

        public T CustomObject => _customObject;
    }
}