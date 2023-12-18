using Bygdrift.Warehouse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Module.Services
{
    public class FTPService
    {
        private readonly FTPClientHelper ftp;
        public FTPService(AppBase<Settings> app, string connectionString)
        {
            App = app;
            ftp = new FTPClientHelper(connectionString);
            try
            {
                ftp.Client.Connect();
                App.Log.LogInformation($"Connected to ftp: {ftp.Host}, at path: {ftp.Path}.");
            }
            catch (Exception)
            {
                App.Log.LogError("Could not connect to FTP.");
                throw;
            }
        }

        public AppBase<Settings> App { get; }

        public IEnumerable<(DateTime Saved, string Name, Stream stream)> GetData(string path = null, int? take = null)
        {
            var count = 0;
            
            string combinedPath = ftp.Path;
            if(!string.IsNullOrEmpty(path))
                combinedPath = Path.Combine(combinedPath, path);

            foreach (var item in ftp.Client.ListDirectory(combinedPath).Where(o => !o.IsDirectory))
            {
                if (take != null && count++ == take)
                    break;

                App.Log.LogInformation($"- File: {item.FullName}. Created: {item.LastWriteTime}. Bytes: {item.Length}");
                var sourceFilePath = ftp.Path + "/" + item.Name;
                var extension = Path.GetExtension(item.FullName).ToLower();
                var stream = new MemoryStream();
                ftp.Client.DownloadFile(sourceFilePath, stream);
                yield return (item.LastAccessTime, item.Name, stream);
            }
        }

        /// <summary>
        /// Copies content from current folder and over to another folder
        /// </summary>
        public void AddContent(string name, Stream stream)
        {
            if (!ftp.Client.Exists(ftp.Path))
                ftp.Client.CreateDirectory(ftp.Path);

            var path = ftp.Path + "/" + name;
            stream.Position = 0;
            ftp.Client.UploadFile(stream, path);
        }

        /// <summary>
        /// Moves content from current folder and over to another folder
        /// </summary>
        /// <param name="folderName">A name like "backup"</param>
        public void MoveFolderContent(string folderName)
        {
            var backupFolder = ftp.Path + "/" + folderName;
            if (!ftp.Client.Exists(backupFolder))
                ftp.Client.CreateDirectory(backupFolder);

            foreach (var item in ftp.Client.ListDirectory(ftp.Path).Where(o => !o.IsDirectory))
            {
                var sourceFilePath = ftp.Path + "/" + item.Name;
                var backupFilePath = backupFolder + "/" + item.Name;
                if (!ftp.Client.Exists(backupFilePath))
                {
                    using var stream = new MemoryStream();
                    ftp.Client.DownloadFile(sourceFilePath, stream);
                    ftp.Client.UploadFile(stream, backupFilePath);
                    ftp.Client.DeleteFile(sourceFilePath);
                }
            }
        }

        /// <summary>
        /// Deletes the content of the ftp folder
        /// </summary>
        public void DeleteFolderContent()
        {
            foreach (var item in ftp.Client.ListDirectory(ftp.Path).Where(o => !o.IsDirectory))
            {
                var sourceFilePath = ftp.Path + "/" + item.Name;
                ftp.Client.DeleteFile(sourceFilePath);
            }
        }

        /// <summary>
        /// Closes connection
        /// </summary>
        public void Close()
        {
            if(ftp != null)
            {
                ftp.Client.Disconnect();
                ftp.Client.Dispose();
            }
        }
    }
}