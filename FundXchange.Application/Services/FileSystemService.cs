using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FundXchange.Model.ViewModels.RadarView;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FundXchange.Model.Serialization;

namespace FundXchange.Application.Services
{
    public enum SaveLocations
    {
        UserFolder,
        TemporaryFolder
    }

    public static class FileSystemService
    {
        private static readonly string BaseUserFilePath;
        private static readonly string BaseTemporaryFilePath;

        static FileSystemService()
        {
            BaseUserFilePath = string.Format("{0}\\FundXchange", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            if (!Directory.Exists(BaseUserFilePath))
            {
                Directory.CreateDirectory(BaseUserFilePath);
            }

            BaseTemporaryFilePath = string.Format("{0}\\FundXchange", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            if (!Directory.Exists(BaseTemporaryFilePath))
            {
                Directory.CreateDirectory(BaseTemporaryFilePath);
            }
        }

        public static IDictionary<string, string> GetFilesOfType(SaveLocations saveLocation, params string[] extensions)
        {
            IDictionary<string, string> foundFiles = new Dictionary<string, string>();
            string extensionFilter = BuildExtensionFilterString(extensions);

            DirectoryInfo directoryInfo = new DirectoryInfo(GetPath(saveLocation));
            foreach (FileInfo file in directoryInfo.GetFileSystemInfos(extensionFilter).OrderBy(fi => fi.LastWriteTime))
            {
                foundFiles.Add(file.FullName, Path.GetFileNameWithoutExtension(file.FullName));
            }
            return foundFiles;
        }

        private static string BuildExtensionFilterString(string[] extensions)
        {
            StringBuilder extensionString = new StringBuilder(extensions.Length);
            for (int i = 0; i < extensions.Length; i++)
            {
                if (!extensions[i].StartsWith("*"))
                {
                    extensions[i] = string.Format("*{0}", extensions[i]);
                }
                if (extensions[i][1] != '.')
                {
                    extensions[i] = extensions[i].Insert(1, ".");
                }

                if (i > extensions.Length - 1)
                {
                    extensionString.AppendFormat("{0},", extensions[i]);
                }
                else
                {
                    extensionString.Append(extensions[i]);
                }
            }
            return extensionString.ToString();
        }

        public static void SaveObjectToFile<TObject>(string filePath, TObject objectToPersist)
        {            
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, objectToPersist);

                if (objectToPersist is IPersistable)
                {
                    (objectToPersist as IPersistable).IsSavedToDisk = true;
                    (objectToPersist as IPersistable).FilePath = filePath;
                    (objectToPersist as IPersistable).FileName = Path.GetFileName(filePath);
                }
            }
        }

        public static void SaveObjectToFile<TObject>(SaveLocations saveLocation, string fileName, TObject objectToPersist)
        {
            string filePath = GetFilePath(saveLocation, fileName);
            SaveObjectToFile(filePath, objectToPersist);
        }

        public static TObject LoadObjectFromFile<TObject>(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    BinaryFormatter binarySerializer = new BinaryFormatter();
                    var deserializedObject = (TObject)binarySerializer.Deserialize(fileStream);

                    if (deserializedObject is IPersistable)
                    {
                        (deserializedObject as IPersistable).IsSavedToDisk = true;
                        (deserializedObject as IPersistable).FilePath = filePath;
                        (deserializedObject as IPersistable).FileName = Path.GetFileName(filePath);
                    }
                    
                    return deserializedObject;
                }
            }
            return default(TObject);
        }

        public static string GetPath(SaveLocations saveLocation)
        {
            switch (saveLocation)
            {
                case SaveLocations.UserFolder:
                    return BaseUserFilePath;
                case SaveLocations.TemporaryFolder:
                    return BaseTemporaryFilePath;
                default:
                    return BaseTemporaryFilePath;
            }
        }

        public static string GetFilePath(SaveLocations saveLocation, string fileName)
        {
            return saveLocation.Equals(SaveLocations.TemporaryFolder) ?
                string.Format("{0}\\{1}", BaseTemporaryFilePath, fileName) :
                string.Format("{0}\\{1}", BaseUserFilePath, fileName);
        }

        public static bool IsTemporaryFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                return filePath.Contains(BaseTemporaryFilePath);
            }
            return false;
        }
    }
}
