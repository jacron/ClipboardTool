using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClipboardTool.domain
{
    public class TrackRecord
    {
        public TrackRecord(string track)
        {
            this.muziekstuknaam = track;
            this.nr = "";
            this.aantaldelen = 1;
        }
        public string nr { get; set; }
        public string muziekstuknaam { get; set; }
        public string muziekstukdeel { get; set; }
        public int aantaldelen { get; set; }
    }
}
