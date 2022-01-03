using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osu_trainer.Controls
{
    public class SelectableLabel : TextBox
    {
        public SelectableLabel()
        {
            // https://stackoverflow.com/a/7748496/1220067
            this.ReadOnly = true;
            this.BorderStyle = 0;
            this.BackColor = this.BackColor;
            this.TabStop = false;
            // this.Multiline = true; // If needed
        }
    }
}
