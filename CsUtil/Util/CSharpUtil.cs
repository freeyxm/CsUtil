﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

namespace CsUtil.Util
{
    public class CSharpUtil
    {
        public static void ShallowCopy<T>(T dst, T src, bool onlyValueType = false)
        {
            FieldInfo[] fields = dst.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; ++i)
            {
                var field = fields[i];
                if (!onlyValueType || field.FieldType.IsValueType || field.FieldType == typeof(string))
                {
                    field.SetValue(dst, field.GetValue(src));
                }
            }
        }

        public static void ResetDefault<T>(T obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; ++i)
            {
                TypeCode type = Type.GetTypeCode(fields[i].FieldType);
                switch (type)
                {
                    case TypeCode.Int32:
                        fields[i].SetValue(obj, 0);
                        break;
                    case TypeCode.Int64:
                        fields[i].SetValue(obj, 0L);
                        break;
                    case TypeCode.UInt32:
                        fields[i].SetValue(obj, 0u);
                        break;
                    case TypeCode.UInt64:
                        fields[i].SetValue(obj, 0uL);
                        break;
                    case TypeCode.Single:
                        fields[i].SetValue(obj, 0f);
                        break;
                    case TypeCode.Double:
                        fields[i].SetValue(obj, 0d);
                        break;
                    case TypeCode.Boolean:
                        fields[i].SetValue(obj, false);
                        break;
                    case TypeCode.Int16:
                        {
                            const short value = 0;
                            fields[i].SetValue(obj, value);
                        }
                        break;
                    case TypeCode.UInt16:
                        {
                            const ushort value = 0;
                            fields[i].SetValue(obj, value);
                        }
                        break;
                    case TypeCode.SByte:
                        {
                            const sbyte value = 0;
                            fields[i].SetValue(obj, value);
                        }
                        break;
                    case TypeCode.Byte:
                        {
                            const byte value = 0;
                            fields[i].SetValue(obj, value);
                        }
                        break;
                    case TypeCode.Decimal:
                        fields[i].SetValue(obj, 0m);
                        break;
                    case TypeCode.Char:
                        fields[i].SetValue(obj, '\0');
                        break;
                    case TypeCode.String:
                        fields[i].SetValue(obj, "");
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 结构体转byte数组
        /// </summary>
        /// <param name="structObj">要转换的结构体</param>
        /// <param name="buffer">返回值</param>
        /// <param name="offset">偏移</param>
        /// <returns></returns>
        public static bool StructToBytes(object structObj, ref byte[] buffer, int offset)
        {
            // 得到结构体的大小
            int size = Marshal.SizeOf(structObj);
            // 长度检查
            if (offset + size > buffer.Length)
                return false;
            // 分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            // 将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            // 从内存空间拷到byte数组
            Marshal.Copy(structPtr, buffer, offset, size);
            // 释放内存空间
            Marshal.FreeHGlobal(structPtr);

            return true;
        }

        /// <summary>
        /// 结构体转byte数组
        /// </summary>
        /// <param name="structObj">要转换的结构体</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] StructToBytes(object structObj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(structObj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }

        /// <summary>
        /// byte数组转结构体
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <param name="type">结构体类型</param>
        /// <returns>转换后的结构体</returns>
        public static object BytesToStruct(byte[] bytes, Type type)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
                return null; //返回空
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }

        /// <summary>
        /// byte数组转结构体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool BytesToStruct<T>(byte[] bytes, int offset, T obj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(typeof(T));
            //byte数组长度小于结构体的大小
            if (size > bytes.Length - offset)
                return false;
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            Marshal.PtrToStructure(structPtr, obj);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return true;
        }
    }
}
