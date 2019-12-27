using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Assembler
{
    class AssemblerException : Exception
    {
        public AssemblerException() : base() { }
        public AssemblerException(string message) : base(message) { }
        public AssemblerException(string message, Exception innerException) : base(message, innerException) { }
        protected AssemblerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
