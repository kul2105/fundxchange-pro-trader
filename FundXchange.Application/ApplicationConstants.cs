using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FundXchange
{
    public static class ApplicationConstants
    {
        public const string TradeScriptLicense = "XRT93NQR79ABTW788XR48";
        public static string HelpFileLocation = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), @"Documents\Fundxchange_Help_Document.pdf");
    }
}
