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
    }
}
