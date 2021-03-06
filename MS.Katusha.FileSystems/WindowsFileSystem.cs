using System;
using System.Collections.Generic;
using System.IO;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Service;
using MS.Katusha.Enumerations;
using MS.Katusha.Configuration;

namespace MS.Katusha.FileSystems
{
    public class WindowsFileSystem : IKatushaFileSystem
    {
        private readonly string _baseFolderName;

        public WindowsFileSystem(string baseFolderName)
        {
            _baseFolderName = baseFolderName;
        }

        public void Add(string path, Stream stream)
        {
            using (var outStream = new FileStream(_baseFolderName + "\\" + path, FileMode.Create, FileAccess.Write)) {
                var length = (int)stream.Length;
                var bytes = new byte[length];
                stream.Read(bytes, 0, length);
                outStream.Write(bytes, 0, length);
            }
        }

        private static void Delete(string path) { File.Delete(path); }

        public void DeletePhoto(Guid photoGuid)
        {
            for (byte i = 0; i <= (byte)PhotoType.MAX; i++) {
                Delete(String.Format("{0}/{1}/{2}-{3}.jpg", _baseFolderName, Folders.Photos, i, photoGuid));
            }
        }

        public void DeleteBackupPhoto(Guid guid)
        {
            Delete(String.Format("{0}/{1}/{2}.jpg", _baseFolderName, Folders.PhotoBackups, guid));
        }

        public bool FileExists(string path) { return File.Exists(_baseFolderName + "\\" + path); }

        public IList<PhotoFile> GetPhotoNames(out IList<string> unparseableFiles, string prefix = "")
        {
            unparseableFiles = new List<string>();
            var list = new List<PhotoFile>();
            var files = Directory.GetFiles(_baseFolderName + "\\" + Folders.Photos, prefix + "*.jpg");
            foreach (var fn in files) {
                var pos = fn.LastIndexOf('\\');
                var fileName = fn.Substring(pos);
                pos = fileName.LastIndexOf(".jpg", StringComparison.Ordinal);
                if (pos <= 0) unparseableFiles.Add(fileName);
                else {
                    if (fileName[1] != '-') unparseableFiles.Add(fileName);
                    else {
                        try {
                            var type = (byte)(fileName[0] - 48);
                            var guid = fileName.Substring(2, fileName.Length - 6);
                            var photoFile = new PhotoFile { PhotoType = type, Guid = Guid.Parse(guid) };
                            list.Add(photoFile);
                        } catch {
                            unparseableFiles.Add(fileName);
                        }
                    }
                }
            }
            return list;
        }

        private string GetUrl(string path) { return KatushaConfigurationManager.Instance.VirtualPath + ((path[0] == '/') ? path.Substring(1) : path); }

        public string GetPhotoUrl(Guid photoGuid, PhotoType photoType, bool encode = false)
        {
            if (encode) return GetUrl(String.Format("/{0}/{1}-{2}.jpg", ((photoGuid == Guid.Empty) ? Folders.Images : Folders.Photos), (byte) photoType, photoGuid));
            var path = String.Format("{0}/{1}/{2}-{3}.jpg", _baseFolderName, Folders.Photos, (byte) photoType, photoGuid);
            var bytes = File.ReadAllBytes(path);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        public string GetPhotoBaseUrl() { return KatushaConfigurationManager.Instance.VirtualPath; }

        public byte[] GetData(string path)
        {
            byte[] bytes;
            using (var memory = new MemoryStream()) {
                using (var stream = new FileStream(String.Format("{0}/{1}", _baseFolderName, path), FileMode.Open, FileAccess.Read)) {
                    var data = new byte[32768];
                    int bytesRead;
                    do {
                        bytesRead = stream.Read(data, 0, data.Length);
                        memory.Write(data, 0, bytesRead);
                    } while (bytesRead > 0);
                    stream.Flush();
                    bytes = memory.ToArray();
                }
            }
            return bytes;
        }

        public void ClearPhotos(bool clearBackups = false)
        {
            var photosFolder = String.Format("{0}/{1}", _baseFolderName, Folders.Photos);
            if (!Directory.Exists(photosFolder))
                Directory.CreateDirectory(photosFolder);
            else { //file system won't have backups
                foreach (var fileName in Directory.EnumerateFiles(photosFolder))
                    File.Delete(fileName);
            }
        }

        public void WritePhoto(Photo photo, PhotoType photoType, byte[] bytes)
        {
            var path = String.Format("{0}/{1}/{2}-{3}.jpg", _baseFolderName, Folders.Photos, (byte) photoType, photo.Guid);
            using(var writer = new FileStream(path, FileMode.Create, FileAccess.Write)) {
                writer.Write(bytes, 0, bytes.Length);
            }
        }


    }
}