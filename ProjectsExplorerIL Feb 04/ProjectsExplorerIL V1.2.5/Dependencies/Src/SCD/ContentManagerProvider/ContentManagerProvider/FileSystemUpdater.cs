using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using ContentManagerProvider.General;
using TraceExceptionWrapper;

namespace ContentManagerProvider
{
    internal class FileSystemUpdater
    {

        internal static String CreateFolder(TreeNode content)
        {
            String relativePath;
            return CreateFolder(content, out relativePath);
        }

        internal static String CreateFolder(TreeNode content, out String relativePath)
        {
            int index = 0;
            string folderName;
            string contentName = UpdateFsString(content.Name);
            string rootFolder = Locator.SystemParameters["RootPathFS"];

            try
            {
                do
                {
                    index++;
                    relativePath = contentName + index;

                    folderName = rootFolder + "\\" + relativePath;

                } while (Directory.Exists(folderName));

                try
                {
                    Directory.CreateDirectory(folderName);
                }
                catch (Exception e)
                {
                    throw new TraceException("Create directory", new List<string>() { folderName, e.Message }, new StackFrame(1, true), e, Locator.ApplicationName);
                }

                return folderName;
            }
            catch (Exception e)
            {
                TraceException te = new TraceException(new StackFrame(1, true), e, Locator.ApplicationName);
                throw te;
            }
        }

        private static String UpdateFsString(string fsName)
        {

            fsName = fsName.Replace("<", "_");
            fsName = fsName.Replace(">", "_");
            fsName = fsName.Replace("*", "_");
            fsName = fsName.Replace("|", "_");
            fsName = fsName.Replace("?", "_");
            fsName = fsName.Replace(":", "_");
            fsName = fsName.Replace("\"", "_");
            fsName = fsName.Replace("/", "_");
            fsName = fsName.Replace("\\", "_");
            return fsName;
        }

        internal static String AddFile(string destinationFullPath, string fileFullPath)
        {
            try
            {
                String destFileName = destinationFullPath + "\\" + Path.GetFileName(fileFullPath);

                try
                {
                    File.Copy(fileFullPath, destFileName);
                }
                catch (Exception e)
                {
                    throw new TraceException("Copy file", new List<string>() { fileFullPath, destFileName, e.Message }, new StackFrame(1, true), e, Locator.ApplicationName);
                }

                return destFileName;
            }
            catch (Exception e)
            {
                TraceException te = new TraceException(new StackFrame(1, true), e, Locator.ApplicationName);
                throw te;
            }
        }

        internal static void DeleteFile(string sourceFullPath)
        {
            try
            {
                File.Delete(sourceFullPath);
            }
            catch (Exception e)
            {
                throw new TraceException("Delete file", new List<string>() { sourceFullPath, e.Message }, new StackFrame(1, true), e, Locator.ApplicationName);
            }
        }

        public static void DeleteFolder(string folderFullPath)
        {
            try
            {
                Directory.Delete(folderFullPath,true);
            }
            catch (Exception e)
            {
                throw new TraceException("Delete folder recursive", new List<string>() { folderFullPath, e.Message }, new StackFrame(1, true), e, Locator.ApplicationName);
            }
        }

        internal static void AddFile(string sourceFile, string destinationFullPath, string destinationDirectoryPath)
        {

            CreateDerectory(destinationDirectoryPath);

            try
            {
                File.Copy(sourceFile, destinationFullPath);
            }
            catch (Exception e)
            {
                throw new TraceException("Copy file", new List<string>() { sourceFile, destinationFullPath, e.Message }, new StackFrame(1, true), e, Locator.ApplicationName);
            }
        }

        internal static void CreateDerectory(string directory)
        {
            try
            {
                if(!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
            }
            catch (Exception e)
            {
                throw new TraceException("Create directory", new List<string>() { directory, e.Message }, new StackFrame(1, true), e, Locator.ApplicationName);
            }
        }

        public static string GetFileFolder(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }
    }
}
