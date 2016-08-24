using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    internal class WrongMemberFormatException : Exception
    {
        public WrongMemberFormatException() { }

        public WrongMemberFormatException(string message) : base(message) { }

        public WrongMemberFormatException(string message, Exception inner) : base(message, inner) { }
    }
}
