using System;
using System.IO;

namespace CsUtil.Util
{
    public class FileUtil
    {
        public static bool WriteFile(string path, byte[] data)
        {
            FileStream stream = null;
            try
            {
                string dir = Directory.GetParent(path).FullName;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                stream = new FileStream(path, FileMode.Create);
                stream.Write(data, 0, data.Length);
                stream.Flush();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error("WriteFile error: path = '{0}', error = {0}", path, e.Message);
                return false;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
        }

        public static bool OpenFile(string path, System.Action<FileStream> action)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open);
                action(stream);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error("OpenFile error: path = '{0}', error = {0}", path, e.Message);
                return false;
            }
            finally
            {
                if (stream != null) stream.Close();
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
                Logger.Error("DeleteFile error: path = '{0}', error = {0}", path, e.Message);
                return false;
            }
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
    }
}
