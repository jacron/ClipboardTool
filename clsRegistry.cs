using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

namespace ClipboardTool
{
    /// <summary>
    /// Ieder scherm dat bijv. zijn windowstate wil opslaan,
    /// instantieert deze klasse met zijn eigen modulenaam.
    /// </summary>
    /// <remarks>Let op dat je bij het afsluiten van het programma geen exception werpt zonder waarschuwing!</remarks>
    public class clsRegistry
    {
        const string userRoot = "HKEY_CURRENT_USER";
        const string subkey = "SOFTWARE\\ClipboardTool";
        const string keyName = userRoot + "\\" + subkey;
        private string mModulenaam = "";

        #region Properties
        public string module
        {
            get { return mModulenaam; }
            set { mModulenaam = value; }
        }
        #endregion

        /// <summary>
        /// Controleer of er al een van de met het venster verbonden waarden
        /// in de registry opgeslagen is.
        /// </summary>
        /// <returns></returns>
        public bool keyXExists()
        {
            object o = getValue(module + "X");  // is locatie bekend?
            return o != null;
        }

        #region overloaded setters for value
        /// <summary>
        /// Sla een waarde onder een bepaalde naam op in de registry.
        /// </summary>
        /// <param name="naam"></param>
        /// <param name="value"></param>
        public static void setValue(string naam, string value)
        {
            Registry.SetValue(keyName, naam, value);
        }

        public static void setValue(string naam, int value)
        {
            Registry.SetValue(keyName, naam, value.ToString(CultureInfo.CurrentCulture));
        }

        public static void setValue(string naam, long value)
        {
            Registry.SetValue(keyName, naam, value.ToString(CultureInfo.CurrentCulture));
        }

        public static void setValue(string naam, bool value)
        {
            Registry.SetValue(keyName, naam, value.ToString(CultureInfo.CurrentCulture));
        }
        #endregion

        #region typed getters
        /// <summary>
        /// Haal een object met de opgegeven naam op uit de registry.
        /// Als er geen waarde is, levert dit null op.
        /// </summary>
        /// <param name="naam"></param>
        /// <returns></returns>
        public static object getValue(string naam)
        {
            return Registry.GetValue(keyName, naam, null);
        }

        /// <summary>
        /// Haal een string op. Als de sleutel niet bestaat, geef lege string.
        /// </summary>
        /// <param name="naam"></param>
        /// <returns></returns>
        public static string getString(string naam)
        {
            try
            {
                return Registry.GetValue(keyName, naam, "").ToString();

            }
            catch (ArgumentException)
            {
                return "";
            }
        }

        public static string getString(string naam, string deflt)
        {
            try
            {
                return Registry.GetValue(keyName, naam, "").ToString();

            }
            catch (ArgumentException)
            {
                return deflt;
            }
        }

        /// <summary>
        /// Lees een bool uit de registry. Geen waarde aanwezig, dan
        /// geef default terug.
        /// </summary>
        public static bool getBoolean(string naam, bool deflt)
        {
            object o = Registry.GetValue(keyName, naam, null);

            if (o == null) return deflt;
            else
            {
                bool result;
                if (bool.TryParse(o.ToString(), out result))
                    return result;
                else
                    return deflt;
            }
        }

        /// <summary>
        /// Lees een bool uit de registry. Geen waarde aanwezig, dan
        /// geef false terug.
        /// </summary>
        public static bool getBoolean(string naam)
        {
            object o = Registry.GetValue(keyName, naam, null);

            if (o == null) return false;
            else
            {
                bool result;
                if (bool.TryParse(o.ToString(), out result))
                    return result;
                else
                    return false;
            }
        }

        /// <summary>
        /// Lees een int uit de registry. Geen waarde aanwezig, dan
        /// levert dit -1 terug.
        /// </summary>
        public static int getInt32(string naam)
        {
            object o = Registry.GetValue(keyName, naam, null);
            if (o == null) return -1;
            else
            {
                int result;
                if (int.TryParse(o.ToString(), out result))
                    return result;
                else
                    return -1;
            }
        }

        /// <summary>
        /// Lees een int uit de registry. Geen waarde aanwezig, dan
        /// levert dit de meegegeven defaultwaarde terug.
        /// </summary>
        public static int getInt32(string naam, int deflt)
        {
            object o = Registry.GetValue(keyName, naam, null);
            if (o == null) return deflt;
            else
            {
                int result;
                if (int.TryParse(o.ToString(), out result))
                    return result;
                else
                    return deflt;
            }
        }

        /// <summary>
        /// Lees een int uit de registry. Geen waarde aanwezig, dan
        /// levert dit de meegegeven defaultwaarde terug.
        /// </summary>
        public static long getInt64(string naam, long deflt)
        {
            object o = Registry.GetValue(keyName, naam, null);
            if (o == null) return deflt;
            else
            {
                long result;
                if (long.TryParse(o.ToString(), out result))
                    return result;
                else
                    return deflt;
            }
        }

        #endregion

        /// <summary>
        /// Haal de positie van een venster op uit de registry.
        /// </summary>
        /// <returns></returns>
        public Point getLocation()
        {
            if (module == "")onbekendeModule();
            
            int x = getInt32(module + "X");
            int y = getInt32(module + "Y");
            return new Point(x, y);
        }

        /// <summary>
        /// Bewaar de positie van een venster in de registry.
        /// </summary>
        /// <param name="loc"></param>
        public void setLocation(Point loc)
        {
            if (module == "") onbekendeModule();
            
            setValue(module + "X", loc.X.ToString(CultureInfo.CurrentCulture));
            setValue(module + "Y", loc.Y.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Bewaar de afmetingen van een venster in de registry.
        /// </summary>
        /// <param name="size"></param>
        public void setSize(Size size)
        {
            if (module == "")onbekendeModule();
            
            setValue(module + "W", size.Width.ToString(CultureInfo.CurrentCulture));
            setValue(module + "H", size.Height.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Haal de afmetingen van een venster op uit de registry.
        /// </summary>
        /// <returns></returns>
        public Size getSize()
        {
            if (module == "") onbekendeModule();
            
            int w = getInt32(module + "W");
            int h = getInt32(module + "H");
            return new Size(w, h);
        }

        /// <summary>
        /// Als het dektopgebied veranderd is (denk aan resolutie wijziging of tweede
        /// monitor aan/uit), moet de locatie soms worden aangepast.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static Point adjustLocation(Point p)
        {
            int index;
            int upperBound;
            int primaryIndex = 0;
            int x, y, w, h;
            bool validLocation = false;
            // Gets an array of all the screens connected to the system.

            Screen[] screens = Screen.AllScreens;
            upperBound = screens.GetUpperBound(0);

            for (index = 0; index <= upperBound; index++)
            {
                // For each screen, check if Location fals within bounds.
                x = screens[index].Bounds.X;
                y = screens[index].Bounds.Y;
                w = screens[index].Bounds.Width;
                h = screens[index].Bounds.Height;
                if (p.X > x && p.X < x + w - 10 &&
                    p.Y > y && p.Y < y + h - 10)
                    validLocation = true;
                // Remember which screen is primary
                if (screens[index].Primary) primaryIndex = index;
            }
            if (!validLocation)
            {
                // Geen geldige locatie, dan venster linksboven zetten binnen actuele hoofdvenster
                x = screens[primaryIndex].Bounds.X;
                y = screens[primaryIndex].Bounds.Y;

                p.X = x + 10;
                p.Y = y + 10;
            }
            return p;
        }

        public static void onbekendeModule()
        {
            MessageBox.Show("Module: naam niet ingevuld");
            throw new Exception("Onbekende module");
        }

        /// <summary>
        /// Haal afmetingen en positie (en state) van een venster op uit de registry.
        /// </summary>
        /// <param name="frm"></param>
        public void loadWindow(Form frm, string module)
        {
            this.module = module;

            if (module == "") onbekendeModule();

            if (!keyXExists())
                return;
            
            frm.StartPosition = FormStartPosition.Manual;

            // positie van venster
            Point loc = getLocation();
            if (loc.X != -1)
                frm.Location = adjustLocation(loc);

            // afmetingen van venster
            Size size = getSize();
            if (size.Width != -1)
                frm.Size = size;

            // toestand van venster
            string state = getString(module + "State");
            switch (state)
            {
                case "Maximized":
                    frm.WindowState = FormWindowState.Maximized;
                    break;
                case "Normal":
                    frm.WindowState = FormWindowState.Normal;
                    break;
            }
        }

        /// <summary>
        /// Bewaar een windowstate in de registry.
        /// </summary>
        /// <param name="state"></param>
        public void setState(FormWindowState state)
        {
            if (module == "") onbekendeModule();

            setValue(module + "State", state.ToString());
        }

        /// <summary>
        /// Bewaar afmetingen en positie van een venster in de registry.
        /// </summary>
        /// <param name="frm"></param>
        public void saveWindow(Form frm, string module)
        {
            this.module = module;

            if (module == "") onbekendeModule();

            switch (frm.WindowState)
            {
                case FormWindowState.Maximized:
                    setState(frm.WindowState);
                    // van gemaximaliseerd venster willen we locatie en afmeting niet onthouden
                    // want dan zou het overschakelen naar normaal een gemaximaliseerde toestand geven.
                    break;
                case FormWindowState.Minimized:
                    // geminimaliseerd? dan niets te onthouden
                    break;
                case FormWindowState.Normal:
                    // normaal: onthoud alles
                    setState(frm.WindowState);
                    setLocation(frm.Location);
                    setSize(frm.Size);
                    break;
            }
        }

    }
}
