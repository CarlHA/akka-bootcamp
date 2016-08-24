using System;
using System.IO;
using Akka.Actor;

namespace WinTail
{
    internal class FileObserver : IDisposable
    {
        private readonly IActorRef tailActor;

        private readonly string fileDir;

        private readonly string fileNameOnly;

        private FileSystemWatcher watcher;

        public FileObserver(IActorRef tailActor, string absoluteFilePath)
        {
            this.tailActor = tailActor;
            fileDir = Path.GetDirectoryName(absoluteFilePath);
            fileNameOnly = Path.GetFileName(absoluteFilePath);
        }

        public void Start()
        {
            watcher = new FileSystemWatcher(fileDir, fileNameOnly);
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            watcher.Changed += OnFileChanged;
            watcher.Error += OnFileError;
            watcher.EnableRaisingEvents = true;
        }

        private void OnFileError(object sender, ErrorEventArgs e)
        {
            tailActor.Tell(new TailActor.FileError(fileNameOnly, e.GetException().Message), ActorRefs.NoSender);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                tailActor.Tell(new TailActor.FileWrite(e.Name), ActorRefs.NoSender);
            }
        }

        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}
