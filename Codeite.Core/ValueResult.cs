using System;
using System.Text;

namespace Codeite.Core
{
    public class ValueResult<T> : Result
    {
        public ValueResult()
        {}

        public ValueResult(bool success, string message, T value)
            : base(success, message)
        {
            Value = value;   
        }

        public T Value { get; set; }

    }
}
