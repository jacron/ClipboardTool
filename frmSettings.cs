using System;
using System.Data;
using System.Windows.Forms;

[assembly: CLSCompliant(true)]
namespace ClipboardTool
{
    public partial class frmSettings : Form
    {

        #region Constructor

        public frmSettings()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public static string module
        {
            get { return "settings"; }
        }
        public bool showWarningEmpty
        {
            set
            {
                lblWarningEmpty.Visible = value;
                if (value)
                    tabControl1.SelectedTab = tabPaden;
            }
        }
        public string message = "";
        public string xmlPad
        {
            get { return txtBackupPad.Text; }
            set { txtBackupPad.Text = value; }
        }

        public string picturePath
        {
            get { return txtLocalPicPath.Text; }
            set { txtLocalPicPath.Text = value; }
        }
        public string ie32Path
        {
            get { return txtBrowserIE32.Text; }
            set { txtBrowserIE32.Text = value; }
        }
        public string ie64Path
        {
            get { return txtBrowserIE64.Text; }
            set { txtBrowserIE64.Text = value; }
        }
        public string ffPath
        {
            get { return txtBrowserFF.Text; }
            set { txtBrowserFF.Text = value; }
        }


        #endregion

        #region Event Handlers Form

        private void dlgSettings_Load(object sender, EventArgs e)
        {
            clsRegistry reg = new clsRegistry();
            reg.loadWindow(this, module);
            if (savedSelectedTabPage != null)
                tabControl1.SelectedTab = savedSelectedTabPage;
            else
                tabControl1.SelectedIndex = clsRegistry.getInt32("tcSettingsIndex", 0);
        }

        private void dlgSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            clsRegistry reg = new clsRegistry();
            reg.saveWindow(this, module);
            clsRegistry.setValue("tcSettingsIndex", tabControl1.SelectedIndex);
        }

        #endregion

        #region Event Handlers

        private void btnVerkenAfbeedlingenPad_Click(object sender, EventArgs e)
        {
            verken(picturePath);
        }

        private void btnVerkenBakPad_Click(object sender, EventArgs e)
        {
            verken(xmlPad);
        }

        private void btnEllipsAfbeeldingenmap_Click(object sender, EventArgs e)
        {
            getDirPath(txtLocalPicPath);
        }

        private void btnEllipsBackupPad_Click(object sender, EventArgs e)
        {
            getDirPath(txtBackupPad);
        }

        private void btnKiesBrowserIE32_Click(object sender, EventArgs e)
        {
            Util.choosePathForExecutable(txtBrowserIE32);
        }

        private void btnKiesBrowserIE64_Click(object sender, EventArgs e)
        {
            Util.choosePathForExecutable(txtBrowserIE64);
        }

        private void btnKiesBrowserFF_Click(object sender, EventArgs e)
        {
            Util.choosePathForExecutable(txtBrowserFF);
        }

        #endregion Event Handlers

        #region Actions

        public static void getDirPath(TextBox tb)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.SelectedPath = tb.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tb.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        public static void getAppPath(TextBox tb)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.FileName = tb.Text;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tb.Text = openFileDialog1.FileName;
            }
        }

        private static void verken(string pad)
        {
            Util.startProgram(pad);
        }

        TabPage savedSelectedTabPage = null;

        public void setPadenTab()
        {
            savedSelectedTabPage = tabPaden;
            //tabControl1.SelectedTab = tabPaden;
        }

        #endregion

    }
}
