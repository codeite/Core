using System.Collections.Generic;
using System.Linq;

namespace Codeite.Core
{
    public class Result
    {
        public Result()
        {}

        public Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; set; }

        public string Message { get; set; }

        public IEnumerable<string> Messages()
        {
            var message = Message;
            if (message == null)
            {
                return new string[0];
            }

            return message.Split(';').Select(x => x.Trim());
        }        
    }
}