using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ClipboardTool.domain;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;

namespace ClipboardTool
{
    public struct LinkItem
    {
        public string Href;
        public string Text;

        public override string ToString()
        {
            return Href + "\n\t" + Text;
        }
    }
    public partial class frmMain : Form
    {
        #region HotKey
        /*
        //http://www.codeproject.com/KB/miscctrl/ashsimplehotkeys.aspx
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        private const int MOD_CONTROL = 2;
        //private const int MOD_ALT = 1;
        private const int KEY_F5 = (int)Keys.F5;
        private const int WM_HOTKEY = 0x315;

        // reageer op hotkey

        [System.Diagnostics.DebuggerHidden]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                getClipboardText();
            }
            base.WndProc(ref m);
        }
         * */
        #endregion

        public frmMain()
        {
            InitializeComponent();
        }

        string googleUrl = "http://www.google.nl/search?q=";
        string clipboardtextfilename = @"clipboard.txt";

        #region Properties
        public static string module { get { return "main"; } }
        public string savedClipboardText { get; set; }

        #endregion

        #region Event Handling

        private void frmMain_Activated(object sender, EventArgs e)
        {
            txtText.Select();
            txtText.SelectAll();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            // save registry values
            clsRegistry reg = new clsRegistry();
            reg.saveWindow(this, module);

            clsRegistry.setValue("tctool.index", tcTool.SelectedIndex);
            clsRegistry.setValue("downloads", txtDownload.Text);
            clsRegistry.setValue("savedhtml", lblFileNameHtml.Text);
            clsRegistry.setValue("filter", txtFilterLinksHtml.Text);

            // save last clipboard text (used if clipboard empty)
            using (StreamWriter outfile = new StreamWriter(clipboardtextfilename))
            {
                outfile.Write(savedClipboardText);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // empty strings
            lblFileNamePlat.Text = lblFileNameHtml.Text = lblImageFileName.Text = lblHtmlMessage.Text = string.Empty;
            lblHtmlText.Text = lblHtmlLinksItemCount.Text = lblHtmlSelectedLinksItemCount.Text = string.Empty;

            // get registry values
            clsRegistry reg = new clsRegistry();
            reg.loadWindow(this, module);
            tcTool.SelectedIndex = clsRegistry.getInt32("tctool.index", 0);
            txtDownload.Text = clsRegistry.getString("downloads");
            checkExistingPath(gbDownloads, txtDownload.Text);
            lblFileNameHtml.Text = clsRegistry.getString("savedhtml");
            txtFilterLinksHtml.Text = clsRegistry.getString("filter");

            // get last clipboard text (not used?)
            savedClipboardText = getFromFile();

            getClipboardText();
        }

        private void getClipboardText()
        {
            if (Clipboard.ContainsText())
            {
                savedClipboardText = txtText.Text = Clipboard.GetText();
            }
                /*
            else
            {
                MessageBox.Show("No text on clipboard.");
            }*/
        }

        private string getFromFile()
        {
            if (!File.Exists(clipboardtextfilename))
            {
                return string.Empty;
            }
            using (StreamReader sr = File.OpenText(clipboardtextfilename))
            {
                return sr.ReadToEnd();
            }
        }

        private void btnGetPlat_Click(object sender, EventArgs e)
        {
            txtPlat.Text = Clipboard.GetText();
            lblFileNamePlat.Text = "";
            lblPlatItemCount.Text = countRegels(txtPlat.Text) + " lines";
        }

        private int countRegels(string p)
        {
            char[] delim = { '\n' };
            string[] lines = p.Split(delim);
            return lines.Length;
        }

        private void btnSchrijfPlat_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtPlat.Text);
        }

        private void btnHelpCaps_Click(object sender, EventArgs e)
        {
            helpCaps();
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            herstel();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            getClipboardText();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtText.Text);
        }

        private int linesSelected = -1;

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            previousLine();
        }

        private void lstLines_SelectedIndexChanged(object sender, EventArgs e)
        {
            linesSelected = lstLines.SelectedIndex;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            nextLine();
        }

        private void btnGetLines_Click(object sender, EventArgs e)
        {
            getClipboardLines();
            setScheidingsteken();
        }

        private void setScheidingsteken()
        {
            if (lstLines.Items.Count > 3)
            {
                string steken = "-";
                if (!lstLines.Items[2].ToString().Contains(steken))
                    steken = ":";
                txtScheidingstekenEac.Text = steken;
            }
        }

        private void lnklblHelpCaps_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            helpCaps();
        }

        private void lnklblHelpLines_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            helpLines();
        }

        private void lnklblCapSentense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            formatClipboard("capsentense");
        }

        private void lnklblCapWords_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            formatClipboard("capwords");
        }

        private void lnklblUndercast_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            formatClipboard("underall");
        }

        private void lnklblCap_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            formatClipboard("capall");
        }

        private void btnF1_Click(object sender, EventArgs e)
        {
            //getAndFormatClipboard("capsentense");
            Clipboard.SetText(txtStartSentenceUppercase.Text);
        }

        private void btnF2_Click(object sender, EventArgs e)
        {
            //getAndFormatClipboard("capwords");
            Clipboard.SetText(txtStartWordsUppercase.Text);
        }

        private void btnF3_Click(object sender, EventArgs e)
        {
            //getAndFormatClipboard("underall");
            Clipboard.SetText(txtAllCapitals.Text);
        }

        private void btnF4_Click(object sender, EventArgs e)
        {
            //getAndFormatClipboard("capall");
            Clipboard.SetText(txtAllLowercase.Text);
        }
        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            handleMainKeydown(e);
        }

        private void btnKleineLetters_Click(object sender, EventArgs e)
        {
            formatClipboard("underall");
        }

        private void btnClearClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
        }

        private void btnDQ2SQ_Click(object sender, EventArgs e)
        {
            formatClipboard("dq2sq");
        }

        private void btnAdjustAllmusicList_Click(object sender, EventArgs e)
        {
            adjustAllmusicList();
        }

        private void btnAdjustAllmusicList_Click_1(object sender, EventArgs e)
        {
            adjustAllmusicList();
            putLinesToClipboard();
        }

        private void btnOpenUrls_Click(object sender, EventArgs e)
        {
            getClipboardLines();
            openUrls();
        }

        private void btnOpenUrls2_Click(object sender, EventArgs e)
        {
            openUrls();
        }
        #endregion

        #region Methods

        private void previousLine()
        {
            if (lstLines.Items.Count > 0 && linesSelected > 0)
            {
                linesSelected--;
                lstLines.SelectedIndex--;
                Clipboard.SetText(lstLines.SelectedItem.ToString());
            }
        }

        private void nextLine()
        {
            if (lstLines.SelectedIndex < lstLines.Items.Count - 2)
            {
                lstLines.SelectedIndex++;
                linesSelected++;
                Clipboard.SetText(lstLines.SelectedItem.ToString());
            }
        }

        private static void helpCaps()
        {
            string msg = "Haal op haalt de inhoud van het klembord op.\n" +
                "Herstel zet de laatst opgehaalde waarde terug.\n" +
                "Na iedere gekozen bewerking (F1 - F4) wordt de gewijzigde tekst naar het klembord geschreven.\n" +
                "Via F1 t/m F4 (knop of toets) wordt steeds de inhoud van het klembord eerst opgehaald.\n" +
                "Schrijf zet de tekst (na een handmatige wijziging) op het klembord.";
            MessageBox.Show(msg, "Uitleg ClipboardTool commando's");
        }

        private static void helpLines()
        {
            string msg = "Haal op haalt de inhoud van het klembord op.\n" +
                "Volgende regel zet steeds een volgende regel in het klembord.\n" +
                "Deze wordt in het tekstvak gemarkeerd.\n" +
                "Markeer een (vorige) regel door erop te klikken.";
            MessageBox.Show(msg, "Uitleg ClipboardTool commando's");
        }

        private static string capFirstLetter(string s)
        {
            if (s.Length < 2) return s;
            return s.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture) + 
                s.Substring(1).ToLower(CultureInfo.CurrentCulture);
        }

        static string capWords(string s)
        {
            char[] delim = { ' ' };
            string[] words = s.Split(delim);
            string text = "";

            for (int i = 0; i < words.Length; i++)
            {
                if (text.Length > 0)
                    text += " ";
                text += capFirstLetter(words[i]);
            }
            return text;
        }

        private void getAndFormatClipboard(string modus)
        {
            getClipboardText();
            formatClipboard(modus);
        }

        private void formatClipboard(string modus)
        {
            switch (modus)
            {
                case "capsentense":
                    txtText.Text = capFirstLetter(txtText.Text);
                    break;
                case "capwords":
                    txtText.Text = capWords(txtText.Text);
                    break;
                case "capall":
                    txtText.Text = txtText.Text.ToUpper(CultureInfo.CurrentCulture);
                    break;
                case "underall":
                    txtText.Text = txtText.Text.ToLower(CultureInfo.CurrentCulture);
                    break;
                case "dq2sq":
                    txtText.Text = txtText.Text.Replace("\"", "'");
                    break;
            }
            Clipboard.SetText(txtText.Text);
        }

        private void herstel()
        {
            txtText.Text = savedClipboardText;
            Clipboard.SetText(txtText.Text);
        }

        private void handleMainKeydown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    getAndFormatClipboard("capsentense");
                    e.Handled = true;
                    break;
                case Keys.F2:
                    getAndFormatClipboard("capwords");
                    e.Handled = true;
                    break;
                case Keys.F3:
                    getAndFormatClipboard("underall");
                    e.Handled = true;
                    break;
                case Keys.F4:
                    getAndFormatClipboard("capall");
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    this.Close();
                    e.Handled = true;
                    break;
            }
            if (e.Control && !e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.V:
                        if (tcTool.SelectedTab == tabPageLines)
                        {
                            getClipboardLines();
                        }
                        if (tcTool.SelectedTab == tabPageHtml)
                        {
                            haalHtmlOp();
                        }
                        e.Handled = true;
                        break;
                    case Keys.I:
                        openSettings();
                        e.Handled = true;
                        break;
                }
            }
        }

        private void openSettings()
        {
            frmSettings frm = new frmSettings();
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
            }
        }

        private void getClipboardLines()
        {
            if (Clipboard.ContainsText())
            {
                savedClipboardText = Clipboard.GetText();

                lstLines.Items.Clear();
                char[] delim = { '\n' };
                string[] lines = Clipboard.GetText().Split(delim);
                foreach (string line in lines)
                {
                    lstLines.Items.Add(line);
                }
            }
            else
            {
                MessageBox.Show("No text on clipboard.");
            }
            lblRegelCount.Text = lstLines.Items.Count.ToString(CultureInfo.CurrentCulture);
        }

        private void btnPutLinesToClipboard_Click(object sender, EventArgs e)
        {
            putLinesToClipboard();
        }

        private void putLinesToClipboard()
        {
            string text = "";
            foreach (string line in lstLines.Items)
            {
                /*
                if (chkCrLf.Checked)
                    text += line + "\r\n";
                else*/
                text += line + "\n";
            }
            Clipboard.SetText(text);
        }

        private void adjustAllmusicList()
        {
            getClipboardLines();
            for (int i = 0; i < lstLines.Items.Count; i++)
            {
                string line = lstLines.Items[i].ToString();
                line = normalizeLine(line);
                lstLines.Items[i] = line;
            }
        }

        /// <summary>
        /// Pas cd gegevens aan, zodat redundantie wordt verminderd.
        /// </summary>
        private void adjustEacList()
        {
            if (lstLines.Items.Count < 3)
                return;

            string scheidingsteken = txtScheidingstekenEac.Text;
            if (scheidingsteken.Length == 0)
                return;

            string[] tracks = initEacLines();

            // pass 1
            TrackRecord[] trs = getTracks(tracks, scheidingsteken);

            // pass 2
            addToLines(trs, scheidingsteken);
        }

        private string[] initEacLines()
        {
            // Bewaar de bovenste regel, de titel van de cd
            string titel = lstLines.Items[0].ToString();

            // Bewaar de regels vanaf de derde, de trackbeschrijvingen
            string[] tracks = new string[lstLines.Items.Count - 2];
            for (int j = 2; j < lstLines.Items.Count; j++)
            {
                tracks[j - 2] = lstLines.Items[j].ToString();
            }

            lstLines.Items.Clear();

            // De bovenste regel is de titel; de tweede regel is leeg.
            lstLines.Items.Add(titel);
            lstLines.Items.Add("");

            return tracks;
        }

        private void addToLines(TrackRecord[] trs, string scheidingsteken)
        {
            for (int k = 0; k < trs.Length; )
            {
                TrackRecord tr = trs[k++];

                if (tr.nr == "")
                {
                    lstLines.Items.Add(tr.muziekstuknaam);
                }
                else
                {
                    if (tr.aantaldelen == 1)
                    {
                        lstLines.Items.Add(tr.nr + "\t" + tr.muziekstuknaam + scheidingsteken + tr.muziekstukdeel);
                    }
                    else
                    {
                        // lege regel
                        lstLines.Items.Add("");

                        // regel met muziekstuk
                        lstLines.Items.Add(tr.muziekstuknaam);

                        // regels met de delen
                        lstLines.Items.Add(tr.nr + "\t" + tr.muziekstukdeel);
                        for (int i = 1; i < tr.aantaldelen; i++)
                        {
                            TrackRecord tr2 = trs[k++];
                            lstLines.Items.Add(tr2.nr + "\t" + tr2.muziekstukdeel);
                        }
                    }
                }
            }
        }

        private TrackRecord[] getTracks(string[] tracks, string scheidingsteken)
        {
            List<TrackRecord> tracklist = new List<TrackRecord>();
            TrackRecord savedtrack = null;
            string nr_eerstetrackvanstuk = "";
            int aantaldelen = 1;

            for (int i = 0; i < tracks.Length; i++)
            {
                string track = tracks[i];
                TrackRecord trackrecord = new TrackRecord(track);

                if (track.Contains(scheidingsteken))
                {
                    int pos;
                    pos = track.IndexOf("\t");
                    if (pos == -1)
                    {
                        tracklist.Add(trackrecord);
                        continue;
                    }
                    trackrecord.nr = track.Substring(0, pos);
                    string title = track.Substring(pos + 1);

                    pos = title.IndexOf(scheidingsteken);
                    trackrecord.muziekstuknaam = title.Substring(0, pos);
                    trackrecord.muziekstukdeel = title.Substring(pos + 1);

                    if (savedtrack == null || trackrecord.muziekstuknaam != savedtrack.muziekstuknaam)
                    {
                        // Nieuw stuk
                        adjustAantalDelenInTrack(tracklist, nr_eerstetrackvanstuk, aantaldelen);
                        nr_eerstetrackvanstuk = trackrecord.nr;
                        aantaldelen = 1;
                    }
                    else
                    {
                        // Vervolgdeel
                        aantaldelen++;
                    }
                    savedtrack = trackrecord;
                }
                tracklist.Add(trackrecord);
            }
            adjustAantalDelenInTrack(tracklist, nr_eerstetrackvanstuk, aantaldelen);
            TrackRecord[] trs = (TrackRecord[])tracklist.ToArray();
            return trs;
        }

        private static void adjustAantalDelenInTrack(List<TrackRecord> tracklist, string nr_eerstetrackvanstuk, int aantaldelen)
        {
            foreach (TrackRecord tr in tracklist)
            {
                if (tr.nr == nr_eerstetrackvanstuk)
                {
                    tr.aantaldelen = aantaldelen;
                    break;
                }
            }
        }

        private static string normalizeLine(string line)
        {
            line = line.Trim();
            char[] delim = { '\t' };

            // splits regel in tabgescheiden woorden
            string[] words = line.Split(delim);
            if (words.Length < 2)
                return line;

            // eerste woord bevat nummer + titel
            string nr = words[0];

            // vervang eerste spatie in nummer + titel door tab
            int pos = nr.IndexOf(" ");
            if (pos != -1)
                nr = nr.Substring(0, pos) + "\t" + nr.Substring(pos + 1);

            string text = "\t" + nr;

            for (int i = 1; i < words.Length; i++)
            {
                if (words[i] == "")
                    continue;
                text += "\t" + words[i];
            }
            return text;
        }

        #region Open Urls

        private void openUrls()
        {
            if (lstLines.SelectedIndices.Count == 0)
            {
                for (int i = 0; i < lstLines.Items.Count; i++)
                {
                    string line = lstLines.Items[i].ToString();
                    openUrl(line);
                }
            }
            else
            {
                for (int i = 0; i < lstLines.SelectedIndices.Count; i++)
                {
                    int ix = lstLines.SelectedIndices[i];
                    string line = lstLines.Items[ix].ToString();
                    openUrl(line);
                }
            }
        }

        /// <summary>
        /// Als link inhoud heeft, vul eventueel aan met http://.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string effectiveUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            if (chkAddHttp.Checked)
            {
                if (!url.StartsWith(@"http://") && !url.StartsWith(@"https://"))
                    return "http://" + url;
                else
                    return url;
            }
            else
            {
                return url;
            }
        }

        public static bool openLink(string prog)
        {
            try
            {
                System.Diagnostics.Process.Start(prog);
                return true;
            }
            catch (FileNotFoundException)
            {
                //System.Windows.Forms.MessageBox.Show(exc.Message);
                //MessageBox.Show(prog + " niet gevonden.");
            }
            catch (Win32Exception)
            {
                //MessageBox.Show("Programma " + prog + " is niet uitvoerbaar.");
            }
            return false;
        }

        /// <summary>
        /// Open valide string (start met http en niet leeg) als url. Controleer of het bestand niet al bestaat.
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool openValidUrl(string url)
        {
            if (url.StartsWith("http") && !string.IsNullOrEmpty(url))
            {
                //if (!downloadExists(url))
                {
                    string dest = txtDownload.Text + "\\" + Path.GetFileName(url);
                    return openLink(url);
                    //return getFileFromNetReq(url, dest);
                }
            }
            return false;
        }
        
        /*
        private bool getFileFromNetReq(string url, string dest)
        {
            HttpWebRequest site = (HttpWebRequest)WebRequest.Create(url);
            site.Credentials = new NetworkCredential("info@jcroonen.nl", "chaf06");
            HttpWebResponse response = (HttpWebResponse)site.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader read = new StreamReader(dataStream);
            String data = read.ReadToEnd();
            writeFile(data, dest);
            return true;
        }
        */

        private bool getFileFromNet(string url, string dest)
        {
            WebClient wc = new WebClient();
            try
            {
                wc.Credentials = new NetworkCredential("info@jcroonen.nl", "chaf06");
                wc.DownloadFile(url, dest);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private bool downloadExists(string url)
        {
            //string fname = Path.GetFileName(Util.stripHttp(url));
            return File.Exists(txtDownload.Text + "\\" + url);
        }

        /// <summary>
        /// Als link niet leeg is, vul eventueel aan en open hem in de standaard browser.
        /// </summary>
        /// <param name="url"></param>
        private bool openUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                return openLink(effectiveUrl(url));
            }
            return false;
        }

        #endregion

        #endregion

        private void btnNormaliseerEac_Click(object sender, EventArgs e)
        {
            adjustEacList();
        }

        private void btnEacTitel_Click(object sender, EventArgs e)
        {
            if (lstLines.Items.Count > 0)
            {
                openLink(googleUrl + lstLines.Items[0].ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string bestandsnaam = lstLines.Items[0].ToString().
                Replace("&", "_").
                Replace(":", "_");
            Clipboard.SetText(bestandsnaam);
        }

        private void btnSetSchMinus_Click(object sender, EventArgs e)
        {
            txtScheidingstekenEac.Text = "-";
        }

        private void btnSetSchSemicolon_Click(object sender, EventArgs e)
        {
            txtScheidingstekenEac.Text = ":";
        }

        private void btnHaalHtmlOp_Click(object sender, EventArgs e)
        {
            haalHtmlOp();
        }

        private void haalHtmlOp()
        {
            if (Clipboard.ContainsData(DataFormats.Html))
            {
                rtbHtml.Text = savedClipboardText = Clipboard.GetData(DataFormats.Html).ToString();
                lblHtmlText.Text = "Html";
            }
            else if (Clipboard.ContainsData(DataFormats.Text))
            {
                rtbHtml.Text = savedClipboardText = Clipboard.GetData(DataFormats.Text).ToString();
                lblHtmlText.Text = "Text";
            }
            maakLinksVanHtml();
        }

        Collection<LinkItem> savedLinks = null;

        private void btnMaakLinksVanHtml_Click(object sender, EventArgs e)
        {
            maakLinksVanHtml();
        }

        bool passFilter(string s)
        {
            char[] delim = { ' ', ',', ';' };
            string filterreeks = txtFilterLinksHtml.Text;
            string url = s;
            if (!chkFilterCaseSensitive.Checked)
            {
                // maak filter hoofdletter-ongevoelig
                url = s.ToUpper();
                filterreeks = txtFilterLinksHtml.Text.ToUpper();
            }
            string[] filters = filterreeks.Split(delim);

            foreach (string filter in filters)
            {
                // met minteken? deze mag er niet in zitten
                if (filter.Length > 1 && filter.Substring(0, 1) == "-")
                {
                    string f = filter.Substring(1);
                    if (url.Contains(f))
                    {
                        return false;
                    }
                }

                // anders, deze moet er wel in zitten
                else if (!url.Contains(filter))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Haal links op uit ingeplakte html, en pas het filter toe.
        /// </summary>
        private void maakLinksVanHtml()
        {
            List<string> links = new List<string>();
            savedLinks = GetLinks.FindLinks(savedClipboardText);
            if (savedLinks == null)
            {
                // Rapidshare?
                savedLinks = GetLinks.get(savedClipboardText);
            }
            if (savedLinks != null)
            {
                int i = 0;
                foreach (LinkItem li in savedLinks)
                {
                    if (li.Href != null)
                    {
                        if (passFilter(li.Href))
                        {
                            string link = Util.trimEndSlash(li.Href);

                            // voorkom dubbelen
                            if (!links.Contains<string>(link))
                            {
                                links.Add(link);
                                i++;
                            }
                        }
                    }
                }
                lstLinks.Items.Clear();
                lstLinks.Items.AddRange(links.ToArray());
                toonAantalHtmlLinks(lstLinks.Items.Count);
            }
        }
        
        private void btnSurfHtml_Click(object sender, EventArgs e)
        {
            openOntbrekendHtml();
        }

        private void openOntbrekendHtml()
        {
            string s = null;
            if (rtbHtml.SelectedText.Length > 0)
            {
                s = rtbHtml.SelectedText;
            }
            else
            {
                s = rtbHtml.Text;
            }
            int surfed = openOntbrekendHtml(s);
            if (surfed == -1) return;

            lblHtmlMessage.Text = surfed + " links geopend.";
            rtbHtml.Select();
        }

        private void openOntbrekendPlat()
        {
            string s = null;
            if (txtPlat.SelectedText.Length > 0)
            {
                s = txtPlat.SelectedText;
            }
            else
            {
                s = txtPlat.Text;
            }
            int surfed = openOntbrekendHtml(s);
            if (surfed == -1) return;

            //lblHtmlMessage.Text = surfed + " links geopend.";
            //rtbHtml.Select();
        }

        private int openOntbrekendHtml(string s)
        {
            if (!checkExistingPath(gbDownloads, txtDownload.Text))
            {
                MessageBox.Show("Vul eerst het juiste downloadpad in");
                return -1;
            }
            char[] delim = { '\n' };
            string[] lines = s.Split(delim);
            int surfed = 0;
            foreach (string line in lines)
            {
                delim[0] = ' ';
                string[] words = line.Split(delim);
                string url = words[0].Trim();
                string u = Util.stripHttp(url);
                string w = Util.haalBestandsnaamUitPad(u);
                if (!string.IsNullOrEmpty(w) && !downloadExists(w))
                {
                    if (openValidUrl(url))
                    {
                        surfed++;
                        System.Threading.Thread.Sleep(400);
                    }
                }
            }

            return surfed;
        }

        private void telOntbrekend(string s)
        {
            char[] delim = { '\n' };
            string[] lines = s.Split(delim);
            string msg = "";
            int tel = 0;

            foreach (string line in lines)
            {
                delim[0] = ' ';
                string[] words = line.Split(delim);
                string w = words[0].Trim();
                string u = Util.stripHttp(w);
                string url = Util.haalBestandsnaamUitPad(u);

                if (!string.IsNullOrEmpty(url) && !downloadExists(url))
                {
                    msg += url + "\n";
                    tel++;
                }
            }
            MessageBox.Show(msg + "\n" + tel + " bestanden", "Nog niet gedownload");
        }

        void telOntbrekendHtml()
        {
            string s = null;
            if (rtbHtml.SelectedText.Length > 0)
            {
                s = rtbHtml.SelectedText;
            }
            else
            {
                s = rtbHtml.Text;
            }
            telOntbrekend(s);
        }

        void telOntbrekendPlat()
        {
            string s = null;
            if (txtPlat.SelectedText.Length > 0)
            {
                s = txtPlat.SelectedText;
            }
            else
            {
                s = txtPlat.Text;
            }
            telOntbrekend(s);
        }

        int getLimit()
        {
            int limit = 0;
            if (txtAantalOpTeHalenLinks.Text.Length > 0)
            {
                try
                {
                    limit = int.Parse(txtAantalOpTeHalenLinks.Text);
                }
                catch (Exception)
                {
                    txtAantalOpTeHalenLinks.Text = "";
                }
            }
            return limit;
        }

        /// <summary>
        /// Open n links in browser.
        /// </summary>
        private void surfHtml30()
        {
            int limit = getLimit();

            if (limit == 0)
            {
                foreach (string item in lstLinks.Items)
                {
                    openValidUrl(getUrlFromLine(item));
                }
            }
            else
            {
                for (int i = 0; i < lstLinks.Items.Count && i < limit; i++)
                {
                    if (openValidUrl(getUrlFromLine(lstLinks.Items[i].ToString())))
                    {
                        lstLinks.Items.RemoveAt(0);
                    }
                }
            }
            toonAantalHtmlLinks(lstLinks.Items.Count);
        }

        private string getUrlFromLine(string line)
        {
            char[] delim = { ' ' };
            string[] words = line.Split(delim);
            return words[0];
        }

        private void tabPageHtml_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Html) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Link;
            }
        }

        private void tabPageHtml_DragDrop(object sender, DragEventArgs e)
        {
            Object obj = e.Data.GetData(DataFormats.Html);
            if (obj != null)
            {
                rtbHtml.Text = savedClipboardText = obj.ToString();
                lblHtmlText.Text = "Html";
            }
            else
            {
                obj = e.Data.GetData(DataFormats.Text);
                if (obj != null)
                {
                    rtbHtml.Text = savedClipboardText = obj.ToString();
                    lblHtmlText.Text = "Text";
                }
            }
        }

        private void tabPageTools_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Link;
            }
        }

        private void rtbHtml_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            openUrl(e.LinkText);
        }

        /// <summary>
        /// Stel hoofdletter/kleine letters tekstvakken in
        /// </summary>
        private void setEditedStrings()
        {
            txtStartSentenceUppercase.Text = capFirstLetter(txtText.Text);
            txtStartWordsUppercase.Text = capWords(txtText.Text);
            txtAllCapitals.Text = txtText.Text.ToUpper(CultureInfo.CurrentCulture);
            txtAllLowercase.Text = txtText.Text.ToLower(CultureInfo.CurrentCulture);
        }

        private void txtText_TextChanged(object sender, EventArgs e)
        {
            setEditedStrings();
        }

        private void btnSaveAsPlat_Click(object sender, EventArgs e)
        {
            saveTextAs(txtPlat.Text, lblFileNamePlat);
        }

        private void saveTextAs(string p, Label lbl)
        {
            saveFileDialog1.FileName = lbl.Text;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (writeFile(p, saveFileDialog1.FileName))
                {
                    lbl.Text = saveFileDialog1.FileName;
                }
            }
        }

        void saveImageAs(Image img, Label lbl)
        {
            saveFileDialog1.FileName = lbl.Text;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string bestandsnaam = addExtensionIfOmitted(saveFileDialog1.FileName, ".jpg"); 
                    img.Save(bestandsnaam);
                    lbl.Text = bestandsnaam;
                }
                catch (ArgumentException ae)
                {
                    MessageBox.Show(ae.Message);
                }
            }
        }

        private static string addExtensionIfOmitted(string filename, string extension)
        {
            if (!filename.Contains("."))
                return filename + extension;
            else
                return filename;
        }

        private static string readFile(string fname)
        {
            StreamReader sr = null;
            string s = "";

            sr = new StreamReader(fname);
            if (sr != null)
            {
                s = sr.ReadToEnd();
                sr.Close();
            }
            return s;
        }

        private static bool writeFile(string text, string fname)
        {
            StreamWriter sw = null;
            FileStream fs;

            fs = new FileStream(fname, FileMode.OpenOrCreate);
            sw = new StreamWriter(fs);
            sw.Write(text);
            if (sw != null)
                sw.Close();
            return sw != null;
        }

        private void btnSaveAsHtml_Click(object sender, EventArgs e)
        {
            saveTextAs(rtbHtml.Text, lblFileNameHtml);
        }

        private void btnHtmlNaarClipboard_Click(object sender, EventArgs e)
        {
            if (rtbHtml.Text != string.Empty && rtbHtml.Text != null)
                Clipboard.SetText(rtbHtml.Text);
        }

        private void btnLoadPlat_Click(object sender, EventArgs e)
        {
            getTextFromFile(lblFileNamePlat, txtPlat);
        }

        private void getTextFromFile(Label lbl, TextBox tB)
        {
            openFileDialog1.FileName = lbl.Text;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string txt = readFile(openFileDialog1.FileName);
                if (!string.IsNullOrEmpty(txt))
                {
                    tB.Text = txt;
                    lbl.Text = openFileDialog1.FileName;
                }
            }
        }

        private void btnLoadHtml_Click(object sender, EventArgs e)
        {
            getTextFromFile(lblFileNameHtml, rtbHtml);
        }

        private void getTextFromFile(Label lbl, RichTextBox rtb)
        {
            openFileDialog1.FileName = lbl.Text;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string txt = readFile(openFileDialog1.FileName);
                if (!string.IsNullOrEmpty(txt))
                {
                    rtb.Text = txt;
                    lbl.Text = openFileDialog1.FileName;
                }
            }
            toonAantalHtmlLinks();
        }

        void getImageFromFile(Label lbl)
        {
            openFileDialog1.FileName = lbl.Text;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Bitmap bm = new Bitmap(openFileDialog1.FileName);
                    pictureBox1.Image = bm;
                    lbl.Text = openFileDialog1.FileName;
                }
                catch (ArgumentException ae)
                {
                    MessageBox.Show(ae.Message);
                }
            }
        }

        private void btnSurfPlat_Click(object sender, EventArgs e)
        {
            string txt = txtPlat.Text;
            if (txtPlat.SelectionLength > 0)
            {
                txt = txtPlat.SelectedText;
            }
            string[] lines = txt.Split();
            foreach (string line in lines)
            {
                openUrl(line);
            }
        }

        private void lnklblFileSonicFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtFilterLinksHtml.Text = "/file/";
        }

        private void lnklblFileViewFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtFilterLinksHtml.Text = "/dl/";
        }

        private void rtbHtml_SelectionChanged(object sender, EventArgs e)
        {
            toonAantalSelectedHtmlLinks();
        }

        private void btnPlakAfbeelding_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Clipboard.GetImage();
        }

        private void btnKopieerAfbeelding_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(pictureBox1.Image);
        }

        private void btnSaveAfbeelding_Click(object sender, EventArgs e)
        {
            saveImageAs(pictureBox1.Image, lblImageFileName);
        }

        private void btnOpenAfbeelding_Click(object sender, EventArgs e)
        {
            getImageFromFile(lblImageFileName);
        }

        private void toonAantalSelectedHtmlLinks()
        {
            int cnt = getAantalSelectedHtmlLinks();
            if (cnt == 0)
                lblHtmlSelectedLinksItemCount.Text = string.Empty;
            else
                lblHtmlSelectedLinksItemCount.Text = "selected: " + cnt;
        }

        private void toonAantalHtmlLinks()
        {
            toonAantalHtmlLinks(getAantalHtmlLinks());
        }

        private int getAantalSelectedHtmlLinks()
        {
            string s = rtbHtml.SelectedRtf;

            //skip first line \rtf1\...
            s = skipFirstLine(s);
            if (s.Length == 0)
                return 0;

            string[] delim = { "\r\n" };
            string[] lines = s.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            return lines.Length;
        }

        string skipFirstLine(string s)
        {
            int pos = s.IndexOf("\r\n");
            if (pos == -1) return s;
            else return s.Substring(pos + 2);
        }

        private int getAantalHtmlLinks()
        {
            string s = rtbHtml.Rtf;

            //skip first line \rtf1\...
            s = skipFirstLine(s);
            if (s.Length == 0)
                return 0;

            if (s.StartsWith("\\"))
            {
                s = skipFirstLine(s);
            }

            if (s.Length == 0)
                return 0;

            string[] delim = { "\r\n" };
            string[] lines = s.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            int len = lines.Length;
            if (lines[len - 1].StartsWith("\\"))
                len--;
            return len;
        }

        private void toonAantalHtmlLinks(int cnt)
        {
            if (cnt == 0)
                lblHtmlLinksItemCount.Text = string.Empty;
            else if (cnt==1)
                lblHtmlLinksItemCount.Text = cnt + " link";
            else
                lblHtmlLinksItemCount.Text = cnt + " links";
        }

        private void lstLines_DoubleClick(object sender, EventArgs e)
        {
            if (lstLines.SelectedIndices.Count == 1)
            {
                string line = lstLines.SelectedItem.ToString();
                openUrl(line);
            }
        }

        private void btnSurfVolgende_Click(object sender, EventArgs e)
        {
            if (lstLines.SelectedIndex < lstLines.Items.Count - 2)
            {
                int ix = lstLines.SelectedIndex;
                lstLines.SelectedIndices.Clear();
                lstLines.SelectedIndex = ix + 1;
            }
        }

        private void btnPlakSelect_Click(object sender, EventArgs e)
        {
            string text = string.Empty;
            if (Clipboard.ContainsData(DataFormats.Html))
            {
                text = savedClipboardText = Clipboard.GetData(DataFormats.Html).ToString();
                lblSelectHtmlText.Text = "Html";
            }
            else if (Clipboard.ContainsData(DataFormats.Text))
            {
                text = savedClipboardText = Clipboard.GetData(DataFormats.Text).ToString();
                lblSelectHtmlText.Text = "Text";
            }
            savedLinks = GetLinks.FindLinks(savedClipboardText);
            int i = 0;
            
            foreach (LinkItem li in savedLinks)
            {
                if (li.Href != null && li.Href.StartsWith("http://"))
                {
                    // prepare linklabel
                    LinkLabel lnklbl = new LinkLabel();
                    lnklbl.Name = "lnklbl_select_" + i;
                    lnklbl.Text = li.Href;
                    lnklbl.Click += new EventHandler(lnklbl_Click);

                    // prepare checkbox
                    CheckBox chk = new CheckBox();
                    chk.Name = "chk_select_" + i;

                    //SelectableLink sl = new SelectableLink(chk, lnklbl);
                    
                    // add to container


                    i++;
                }
            }

        }

        void lnklbl_Click(object sender, EventArgs e)
        {
            LinkLabel lnklbl = sender as LinkLabel;
            openUrl(lnklbl.Text);
        }

        private void btnHaal30_Click(object sender, EventArgs e)
        {
            surfHtml30();
        }

        private void btnKiesDownloadPad_Click(object sender, EventArgs e)
        {
            string nieuwPad = kiesBestandenMap(txtDownload.Text);
            if (!string.IsNullOrEmpty(nieuwPad))
            {
                txtDownload.Text = nieuwPad;
                checkExistingPath(gbDownloads, txtDownload.Text);
            }
        }

        string kiesBestandenMap(string map)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.SelectedPath = map;
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                return folderBrowserDialog1.SelectedPath;
            }
            else
            {
                return string.Empty;
            }
        }

        private bool checkExistingPath(GroupBox gb, string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                gb.ForeColor = Color.Red;
                return false;
            }
            else
            {
                gb.ForeColor = Color.Black;
                return true;
            }
            //this.Refresh();
        }

        private void btnTelOntbrekend_Click(object sender, EventArgs e)
        {
            telOntbrekendHtml();
        }

        private void btnSamenvoegen_Click(object sender, EventArgs e)
        {
            string txt = txtPlat.Text;
            if (txtPlat.SelectionLength > 0)
            {
                txt = txtPlat.SelectedText;
            }
            char[] delim = { '\r','\n'};
            string[] lines = txt.Split(delim);
            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(txtScheidingsteken.Text);
                    }
                    sb.Append(line);
                }
            }
            txtPlat.Text = sb.ToString();
        }

        private void btnMaakCompleet_Click(object sender, EventArgs e)
        {

        }

        private void btnCheckDir_Click(object sender, EventArgs e)
        {
            string nieuwPad = kiesBestandenMap(txtCheck.Text);
            if (!string.IsNullOrEmpty(nieuwPad))
            {
                txtCheck.Text = nieuwPad;
                checkExistingPath(gbCheck, txtCheck.Text);
            }

        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            string begin = Path.GetFileNameWithoutExtension(txtBegin.Text);
            string einde = Path.GetFileNameWithoutExtension(txtEinde.Text);

        }

        private void btnTelPlatOntbrekend_Click(object sender, EventArgs e)
        {
            telOntbrekendPlat();
        }

        private void btnOpenPlatOntbrekend_Click(object sender, EventArgs e)
        {
            openOntbrekendPlat();            
        }

        private void btnCrlf2Space_Click(object sender, EventArgs e)
        {
            txtPlat.Text = txtPlat.Text.
                Replace("\r\n\r\n", "@@@").
                Replace("\r\n", " ").
                Replace("@@@", "\r\n");
        }

        private void txtFilterLinksHtml_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                maakLinksVanHtml();
                e.Handled = true;
            }
        }

        private void lstLinks_MouseClick(object sender, MouseEventArgs e)
        {
            ListBox lb = sender as ListBox;
            int index = lb.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                openLink(lb.Items[index].ToString());
                lb.Items.RemoveAt(index);
            }
        }

        private void chkFilterCaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            maakLinksVanHtml();
        }


    }
}
