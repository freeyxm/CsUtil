using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CsUtil.Util
{
    public class FileUtil
    {
        public static bool OpenFile(string path, FileMode mode, Action<FileStream> action)
        {
            try
            {
                FileStream stream = new FileStream(path, mode);
                try
                {
                    action(stream);
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                    return false;
                }
                finally
                {
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
        }

        public static bool OpenFile(string path, FileMode mode, Func<FileStream, bool> action)
        {
            try
            {
                FileStream stream = new FileStream(path, mode);
                try
                {
                    return action(stream);
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                    return false;
                }
                finally
                {
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
        }

        public static bool OpenFile(string path, Func<string, FileStream> streamCreater, Func<FileStream, bool> action)
        {
            try
            {
                FileStream stream = streamCreater(path);
                try
                {
                    return action(stream);
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                    return false;
                }
                finally
                {
                    if (stream != null) stream.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
        }

        public static bool OpenFile(string path, Action<FileStream> action)
        {
            return OpenFile(path, FileMode.Open, action);
        }

        public static bool OpenFile(string path, Func<FileStream, bool> action)
        {
            return OpenFile(path, FileMode.Open, action);
        }

        public static bool OpenFile(string inFile, string outFile, Action<FileStream, FileStream> action)
        {
            return OpenFile(inFile, FileMode.Open, (inStream) =>
            {
                return OpenFile(outFile, FileMode.Create, (outStream) =>
                {
                    action(inStream, outStream);
                });
            });
        }

        public static bool OpenFile(string inFile, string outFile, Func<FileStream, FileStream, bool> action)
        {
            return OpenFile(inFile, FileMode.Open, (inStream) =>
            {
                return OpenFile(outFile, FileMode.Create, (outStream) =>
                {
                    return action(inStream, outStream);
                });
            });
        }

        public static bool WriteFile(string path, byte[] data)
        {
            try
            {
                string dir = Directory.GetParent(path).FullName;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                return OpenFile(path, FileMode.Create, (stream) =>
                {
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
        }

        public static bool DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
        }

        public static string ReadFile(string path)
        {
            string result = null;
            OpenFile(path, (stream) =>
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            });
            return result;
        }

        /// <summary>
        /// 获取指定目录的父目录。
        /// 1. 需自行保证目录分割符为"/".
        /// 2. 返回目录中一般带有"/".
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string GetDirParent(string dir)
        {
            for (int i = dir.Length - 2; i >= 0; --i)
            {
                if (dir[i] == '/')
                    return dir.Substring(0, i + 1);
            }
            return "";
        }

        /// <summary>
        /// 获取文件的父目录。
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileParent(string file)
        {
            int index = file.LastIndexOf('/');
            if (index != -1)
                return file.Substring(0, index + 1);
            else
                return "";
        }

        /// <summary>
        /// 连接两个路径
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string CombinePath(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1))
                return path2;
            else if (string.IsNullOrEmpty(path2))
                return path1;
            else if (path1.EndsWith("/") || path2.StartsWith("/"))
                return path1 + path2;
            else
                return new StringBuilder(path1).Append("/").Append(path2).ToString();
        }

        /// <summary>
        /// 格式化路径。
        /// 替换"\\"为"/"；若是目录，结尾为"/"。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isDir"></param>
        /// <returns></returns>
        public static string FormatPath(string path, bool isDir)
        {
            if (path.Contains("\\"))
                path = path.Replace("\\", "/");
            if (isDir && !path.EndsWith("/"))
                path += "/";
            return path;
        }

        /// <summary>
        /// 遍历指定目录下的所有文件。
        /// </summary>
        /// <param name="inPath">输入路径</param>
        /// <param name="excludePattern">排除文件的正则表达式</param>
        /// <param name="action">回调，参数为当前文件</param>
        /// <param name="progress">进度，参数分别为：当前索引(从1开始)，总个数，当前文件</param>
        public static void ForeachFiles(string inPath, string excludePattern,
            Action<string> action, Action<int, int, string> progress = null)
        {
            string[] files = Directory.GetFiles(inPath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; ++i)
            {
                string inFile = files[i];

                if (progress != null)
                {
                    progress(i + 1, files.Length, inFile);
                }

                if (!string.IsNullOrEmpty(excludePattern) && Regex.IsMatch(inFile, excludePattern))
                    continue;

                action(inFile);
            }
        }

        /// <summary>
        /// 遍历指定目录下的所有文件，并拼接成对应的输出文件（不创建文件）。
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="outPath"></param>
        /// <param name="excludePattern"></param>
        /// <param name="action"></param>
        /// <param name="progress"></param>
        public static void ForeachFiles(string inPath, string outPath, string excludePattern,
            Action<string, string> action, Action<int, int, string> progress = null)
        {
            inPath = FormatPath(inPath, true);
            outPath = FormatPath(outPath, true);

            ForeachFiles(inPath, excludePattern, (inFile) =>
            {
                string outFile = outPath + inFile.Substring(inPath.Length);
                action(inFile, outFile);
            }, progress);
        }

        /// <summary>
        /// 遍历指定目录下的所有文件，并创建对应的输出文件。
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="outPath"></param>
        /// <param name="excludePattern"></param>
        /// <param name="action"></param>
        /// <param name="progress"></param>
        public static void ForeachFiles(string inPath, string outPath, string excludePattern,
            Action<Stream, Stream> action, Action<int, int, string> progress = null)
        {
            ForeachFiles(inPath, outPath, excludePattern, (inFile, outFile) =>
            {
                string outDir = Directory.GetParent(outFile).FullName;
                if (!Directory.Exists(outDir))
                {
                    Directory.CreateDirectory(outDir);
                }
                OpenFile(inFile, outFile, (inStream, outStream) =>
                {
                    action(inStream, outStream);
                });
            }, progress);
        }
    }
}
