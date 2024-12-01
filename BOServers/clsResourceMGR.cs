using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;
//using MyResources;

namespace PALSA
{
    public class clsResourceMGR
    {
        public ResourceManager resourceManager = new ResourceManager("BOServers.MYRES", Assembly.GetAssembly(typeof(MyResources.ResLibraryClass)));

        //private const string resxFile = @".\MYRES.resx";
        //public ResXResourceSet resourceManager = new ResXResourceSet(resxFile);
        public static clsResourceMGR _instance;

        public static clsResourceMGR INSTANCE
        {
            get { return _instance ?? (_instance = new clsResourceMGR()); }
        }

        private clsResourceMGR()
        {

        }
    }
}
