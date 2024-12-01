using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace M4.ClientData
{
    [Serializable]
    public class RiskProfile
    {        
        public string Name = string.Empty;        
        public List<RiskQuestion> lstQuest = new List<RiskQuestion>();
        public Dictionary<string, List<string>> lstDiscret = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> lstRetire = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> lstPort = new Dictionary<string, List<string>>();

        public ClientPorfolio DiscretPort = null;
        public ClientPorfolio RetirePort = null;
        public List<ClientPorfolio> TimeHorizonPort = new List<ClientPorfolio>();
        public bool IsPort4Retirement = false;

        public double PerAns1 = 0;
        public double PerAns2 = 0;
        public double PerAns3 = 0;
    }
    [Serializable]
    public class RiskQuestion
    {
        public int QNum = 0;
        public string Quest = string.Empty;
        public string Ans1 = string.Empty;
        public string Ans2 = string.Empty;
        public string Ans3 = string.Empty;
        public string Ans4 = string.Empty;
                   
        public bool Ansflag1 = false;
        public bool Ansflag2 = false;
        public bool Ansflag3 = false;
        public bool Ansflag4 = false;
    }
}
