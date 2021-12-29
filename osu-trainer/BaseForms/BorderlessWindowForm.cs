using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osu_trainer.BaseForms
{
    public partial class BorderlessWindowForm : Form
    {
        public BorderlessWindowForm()
        {
            InitializeComponent();
        }

        #region borderless window title bar

        private bool Drag;
        private int MouseX;
        private int MouseY;
        private void titlePanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Icon smallIcon = new Icon(this.Icon, 16, 16);
            e.Graphics.DrawIcon(smallIcon, 10, 10);
            e.Graphics.DrawString(this.Text, new Font(Font, FontStyle.Regular), Brushes.White, 10 + 16 + 4, 10);
        }

        private void PanelMove_MouseDown(object sender, MouseEventArgs e)
        {
            Drag = true;
            MouseX = Cursor.Position.X - this.Left;
            MouseY = Cursor.Position.Y - this.Top;
        }

        private void PanelMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (Drag)
            {
                Top = Cursor.Position.Y - MouseY;
                Left = Cursor.Position.X - MouseX;
            }
        }

        private void PanelMove_MouseUp(object sender, MouseEventArgs e) => Drag = false;

        private void closeButton_Click(object sender, EventArgs e) => Close();

        private void minimizeButton_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        #endregion borderless window title bar
    }
}
