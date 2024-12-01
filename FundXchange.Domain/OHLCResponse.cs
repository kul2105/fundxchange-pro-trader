using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundXchange.Domain
{

    public class OHLCData
    {       
        public string feedDateTime;        
        public string Open;        
        public string High;        
        public string Low;        
        public string Close;        
        public long Volume;
    }
    
    public class OHLCResponse
    {
    
        public bool result;    
        public string message;    
        public List<OHLCData> lstOHLCData;                      // List Of OHLCData
    }

}
