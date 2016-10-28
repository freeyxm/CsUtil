using System;
using System.IO;
using System.Text;

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
            if (path1.EndsWith("/") || path2.StartsWith("/"))
                return path1 + path2;
            else
                return new StringBuilder(path1).Append("/").Append(path2).ToString();
        }
    }
}
