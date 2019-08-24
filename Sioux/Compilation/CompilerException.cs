using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingWebSerwer.Compilation
{
    public class CompilerException : Exception
    {
        public CompilerException()
        {
        }

        public CompilerException(string message) : base(message)
        {
        }

        public CompilerException(string message, Exception e) : base(message, e)
        {
        }

        public CompilerException(string message, ScriptStateEnum errorEnum) : base(message)
        {
            CompilationErrorTypeEnum = errorEnum;
        }

        public CompilerException(string message, Exception e, ScriptStateEnum errorEnum) : base(message, e)
        {
            CompilationErrorTypeEnum = errorEnum;
        }

        public ScriptStateEnum CompilationErrorTypeEnum { get; private set; }
    }

    public enum ScriptStateEnum
    {
        InCompilation,
        Ready,
        RuntimeError,
        Corrupted,
        ScriptNotExist,
        CompilerError,
    }
}
