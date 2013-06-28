using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeite.Core.Json
{
    public static class JsonPath
    {
        public static dynamic ReadValue(this Dictionary<string, object> json, string path)
        {
            if (!path.StartsWith("$"))
            {
                throw new NotSupportedException("Only rooted paths are currently supported so queries must start with $");
            }

            return null;
        }
    }
}
