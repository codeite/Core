using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Codeite.Core.Json
{
    public static class DynamicXmlObject
    {
        private static string WrapXmlBuilder(this object jsonObject, bool cannonical, StringBuilder builder = null)
        {
            bool madeBuilder = (builder == null);
            if (madeBuilder)
            {
                builder = new StringBuilder();
            }

            ToXmlStringCommon(jsonObject, cannonical, builder);

            if (madeBuilder)
            {
                return builder.ToString();
            }

            return null;
        }

        private static void ToXmlStringCommon(this object jsonObject, bool cannonical, StringBuilder builder)
        {
            if (jsonObject is IEnumerable<KeyValuePair<string, dynamic>>)
            {
                ObjectToCannonicalString(jsonObject as IEnumerable<KeyValuePair<string, dynamic>>, cannonical, builder);
            }
            else if (jsonObject is IEnumerable<dynamic>)
            {
                ArrayToCannonicalString(jsonObject as IEnumerable<dynamic>, builder);
            }
            else if (jsonObject is string)
            {
                builder.Append("\"");
                builder.Append(jsonObject as string);
                builder.Append("\"");
            }
            else if (jsonObject is long)
            {
                builder.Append((long)jsonObject);
            }
            else if (jsonObject is int)
            {
                builder.Append((int)jsonObject);
            }
            else if (jsonObject is short)
            {
                builder.Append((short)jsonObject);
            }
            else if (jsonObject is sbyte)
            {
                builder.Append((sbyte)jsonObject);
            }
            else if (jsonObject is ulong)
            {
                builder.Append((ulong)jsonObject);
            }
            else if (jsonObject is uint)
            {
                builder.Append((uint)jsonObject);
            }
            else if (jsonObject is ushort)
            {
                builder.Append((ushort)jsonObject);
            }
            else if (jsonObject is byte)
            {
                builder.Append((byte)jsonObject);
            }
            else if (jsonObject is float || jsonObject is double || jsonObject is decimal)
            {
                builder.Append(jsonObject);
            }
            else if (jsonObject is BigInteger)
            {
                builder.Append(jsonObject);
            }
            else if (jsonObject is bool)
            {
                builder.Append(((bool)jsonObject) ? "true" : "false");
            }
            else if (jsonObject == null)
            {
                builder.Append("null");
            }
            else
            {
                throw new Exception("What do I do with a " + jsonObject.GetType());
            }
        }

        private static void ObjectToCannonicalString(IEnumerable<KeyValuePair<string, dynamic>> jsonObject, bool cannonical, StringBuilder builder)
        {

            builder.Append("{");
            bool first = true;

            if (cannonical)
            {
                jsonObject = jsonObject.OrderBy(x => x.Key);
            }

            foreach (var o in jsonObject)
            {
                if (first)
                {
                    builder.Append("\"");
                    first = false;
                }
                else
                {
                    builder.Append(",\"");
                }

                builder.Append(o.Key);
                builder.Append("\":");
                ToCannonicalString(o.Value, builder);
            }

            builder.Append("}");
        }

        private static void ArrayToCannonicalString(IEnumerable<dynamic> jsonObject, StringBuilder builder)
        {
            builder.Append("[");
            bool first = true;

            foreach (var o in jsonObject)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(",");
                }

                ToCannonicalString(o, builder);
            }

            builder.Append("]");
        }

        public static object ToXmlString(this object jsonObject, StringBuilder builder = null)
        {
            return WrapXmlBuilder(jsonObject, false, builder);
        }

        private static string ToCannonicalString(this object jsonObject, StringBuilder builder = null)
        {
            return WrapXmlBuilder(jsonObject, true, builder);
        }
    }
}
