using System;
using System.Collections.Generic;

namespace M4.Portfolios
{
    [Serializable]
    public class Portfolio
    {
        public string m_name = null;
        public List<string> m_symbols = new List<string>();
    }
}
