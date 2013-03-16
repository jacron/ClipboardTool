using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Net;
using System.IO;
using System.Data;
using System.Diagnostics;

namespace ClipboardTool
{
    public class Util
    {

        #region Dialogen

        /// <summary>
        /// (Als een record gewijzigd is). De keuze stellen uit 
        /// Opslaan, Niet opslaan, Annuleren.
        /// </summary>
        /// <param name="doorgaanmessage"></param>
        /// <param name="naam"></param>
        /// <returns></returns>
        internal static DialogResult askSave(string msg, string naam)
        {
            return MessageBox.Show(string.Format(msg, naam), Application.ProductName,
                 MessageBoxButtons.YesNoCancel);
        }

        public static void choosePathForExecutable(TextBox tb)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter =
                "Executable files (*.exe, *.lnk)|*.exe;*.lnk|" +
                "All files|*.*";
            ofd.Title = "Kies een bestand";
            ofd.FileName = tb.Text;
            if (ofd.ShowDialog() == DialogResult.OK)
                tb.Text = ofd.FileName;
        }

        public static bool zekerWeten(string prompt)
        {
            return (MessageBox.Show(
                prompt, 
                "Zeker weten?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2
            ) == DialogResult.Yes);
        }
        public static void meldValidatie(string prompt)
        {
            MessageBox.Show(prompt, "Foutje in invoer");
        }

        #endregion

        #region string manipulatie en testen

        public static string trimEndSlash(string s)
        {
            if (s == string.Empty)
                return s;

            // trim last slash
            if (s.Substring(s.Length - 1) == "/")
                s = s.Substring(0, s.Length - 1);

            return s;
        }

        public static string haalBestandsnaamUitPad(string url)
        {
            url = trimEndSlash(url);

            if (url == string.Empty)
                return url;

            int pos = url.LastIndexOf("/");
            if (pos == -1 || url.Length < pos + 1) return url;
            string u = url.Substring(pos + 1);
            return u;
        }

        public static string kapaf(string s, int maxlen)
        {
            if (s.Length > maxlen)
                return s.Substring(0, maxlen) + "..";
            else
                return s;
        }

        public static string padToFit(string bedrag, int len)
        {
            len -= bedrag.Length;
            len--;
            while (bedrag.Length < len)
            {
                bedrag = " " + bedrag;
            }
            return bedrag;
        }

        public static void trimPostcode(TextBox tb)
        {
            tb.Text = tb.Text.Replace(" ", "").ToUpper();
        }

        internal static string stripHttp(string s)
        {
            if (s.StartsWith("http://"))
                return s.Substring("http://".Length);

            if (s.StartsWith("https://"))
                return s.Substring("https://".Length);

            return s;
        }

        static public string effectiveUrl(string url)
        {
            if (!url.StartsWith(@"http://") && !url.StartsWith(@"https://"))
                return "http://" + url;
            else
                return url;
        }

        static public string stripQueryString(string url)
        {
            int pos = url.IndexOf('?');
            if (pos == -1)
                pos = url.IndexOf(";");
            if (pos == -1)
                return url;
            else
                return url.Substring(0, pos);
        }

        /// <summary>
        /// Maak naam uit drie delen en voeg adequaat spaties toe.
        /// </summary>
        /// <param name="voornaam"></param>
        /// <param name="tussenvoegsel"></param>
        /// <param name="achternaam"></param>
        /// <returns></returns>
        public static string maakNaam(string voornaam, string tussenvoegsel, string achternaam)
        {
            string volledigenaam = voornaam;
            if (tussenvoegsel.Length > 0)
                volledigenaam += " " + tussenvoegsel;
            volledigenaam += " " + achternaam;
            return volledigenaam;
        }

        /// <summary>
        /// Zet string met lange datum om in string met korte datum.
        /// </summary>
        /// <param name="langedatum"></param>
        /// <returns></returns>
        public static string toShortDate(string langedatum)
        {
            if (langedatum == "") return "";
            DateTime dt = DateTime.Parse(langedatum);
            return dt.ToShortDateString();
        }

        public static String emptyStringIfNull(String veldnaam, DataRow dr)
        {
            string content = "";
            if (dr[veldnaam] != System.DBNull.Value)
                content = dr[veldnaam].ToString();
            return content;
        }

        static private string[] extensions = { "jpg", "bmp", "gif", "jpeg", "png" };

        /// <summary>
        /// Test of de extensie grafisch is (jpg, bmp, gif, jpeg, png).
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static public bool hasGraphicExtension(string s)
        {
            int pos = s.LastIndexOf(".");
            if (pos == -1) return false;

            string extension = s.Substring(pos + 1);
            foreach (string ext in extensions)
            {
                if (ext == extension.ToLower())
                    return true;
            }
            return false;
        }

        public static string decimalStringWithComma(decimal d)
        {
            return d.ToString("n", new CultureInfo("nl-NL", false));
        }

        public static string stripExtension(string s)
        {
            int pospoint = s.IndexOf(".");

            if (pospoint == -1 || s.Length == pospoint) return s;
            return s.Substring(0, pospoint);
        }
        #endregion

        #region Overige

        public static void startProgram(string prog)
        {
            try
            {
                System.Diagnostics.Process.Start(prog);
            }
            catch (FileNotFoundException)
            {
                //System.Windows.Forms.MessageBox.Show(exc.Message);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // melding verschijnt al
            }
        }

        public static void startProgram(string prog, string args)
        {
            try
            {
                System.Diagnostics.Process.Start(prog, args);
            }
            catch (FileNotFoundException exc)
            {
                System.Windows.Forms.MessageBox.Show(exc.Message);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // melding verschijnt al
            }
        }

        public static void startProgramIfPathExists(string path, string url)
        {
            if (path == "")
            {
                MessageBox.Show("Het pad naar de gekozen browser is niet ingevuld.\nZie Instellingen", "Probleem");
                return;
            }
            startProgram(path, url);
        }

        public static int defaultIfNull(String veldnaam, DataRow dr, int deflt)
        {
            if (dr[veldnaam] != System.DBNull.Value)
            {
                try
                {
                    return (int)dr[veldnaam];
                }
                catch (ArgumentException exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            return deflt;
        }

        public static void maakCurrencyKolom(DataGridViewColumn dgvC)
        {
            DataGridViewCellStyle currencyCellStyle = new DataGridViewCellStyle();
            currencyCellStyle.Format = "C";
            currencyCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvC.DefaultCellStyle = currencyCellStyle;
        }

        public static void maakAfgerondeBedragen(DataGridViewColumn dgvC)
        {
            DataGridViewCellStyle roundCurrencyCellStyle = new DataGridViewCellStyle();
            roundCurrencyCellStyle.Format = "N0";
            roundCurrencyCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvC.DefaultCellStyle = roundCurrencyCellStyle;
        }

        public static void maakVullendeKolom(DataGridViewColumn dgvC, int min)
        {
            dgvC.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvC.MinimumWidth = min;
        }

        public static void maakRechtsuitlijnendeKolom(DataGridViewColumn dgvC)
        {
            dgvC.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        internal static bool savePicFromClipboard(System.Drawing.Bitmap bm, string dest)
        {
            if (bm == null || dest == string.Empty)
                return false;

            try
            {
                bm.Save(dest);
                return true;
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                return false;
            }
        }

        #endregion


        internal static string quote(string p)
        {
            return "\"" + p + "\"";
        }


        internal static string addExtensionIfOmitted(string filename, string extension)
        {
            if (!filename.Contains("."))
                return filename + extension;
            else
                return filename;
        }

        public static void openContainingFolder(string path, string file)
        {
            Process p = new Process();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.WorkingDirectory = path;
            p.StartInfo.Arguments = "/select," + file;
            p.Start();
        }

    }
}
