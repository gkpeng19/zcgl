using System;
using System.Collections.Generic;
using NM.Model;

namespace NM.OP
{
    public partial class CertifiedProviderOP : NamedProviderOP
    {
        public CertifiedProviderOP(LoginInfo user, DataProvider dp)
            : base(dp)
        {
            Account = user;
        }

        public LoginInfo Account { get; private set; }
    }
}
