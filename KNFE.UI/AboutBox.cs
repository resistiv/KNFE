using System;
using System.Reflection;
using System.Windows.Forms;

namespace KNFE.UI
{
    internal partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = KNFE.Helper.Globals.ProgramName;
            this.labelVersion.Text = KNFE.Helper.Globals.Version;
            this.labelCopyright.Text = KNFE.Helper.Globals.ProgramCopyright;
            this.labelCompanyName.Text = KNFE.Helper.Globals.ProgramAuthor;
            this.textBoxDescription.Text = $"{AssemblyDescription}{Environment.NewLine}{Environment.NewLine}{KNFE.Helper.Globals.ProgramRepo}{Environment.NewLine}{Environment.NewLine}{KNFE.Helper.Globals.ProgramLicense}";
        }

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }
    }
}
