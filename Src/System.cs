using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class MissingMemberException : SystemException
    {
        #region Constructors
        public MissingMemberException(string message = null, Exception innerException = null) : base(message, innerException)
        {
        }
        #endregion
    }
}
