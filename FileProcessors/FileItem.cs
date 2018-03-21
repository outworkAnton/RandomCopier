using System;
using System.IO;

namespace Sorter.FileProcessors
{
    public class FileItem : IDisposable
    {
        public FileInfo OriginalFile { get; private set; }
        public string DestinationFile { get; private set; }

        public FileItem(FileInfo origFile, string destFile)
        {
            OriginalFile = origFile;
            DestinationFile = destFile;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OriginalFile = null;
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}