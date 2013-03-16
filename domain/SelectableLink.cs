using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClipboardTool.domain
{
    public class SelectableLink
    {
        CheckBox chk;
        LinkLabel lnklbl;
        
        public SelectableLink(CheckBox chk, LinkLabel lnklbl)
        {
            this.chk = chk;
            this.lnklbl = lnklbl;
        }
    }
}
