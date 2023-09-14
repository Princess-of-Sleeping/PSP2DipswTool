using System;
using System.Windows.Forms;

namespace DipswTool
{
    public partial class DebugControlForm : Form
    {
        public DebugControlForm(TopForm f)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.FormClosing += this.my_FormClosing;
            from1 = f;

            uint dipsw = from1.get_debug_ctrl();

            for (int i = 0; i < 32; i++)
            {
                if ((dipsw & ((uint)1 << i)) != 0)
                {
                    this.checkedListBox1.SetItemChecked(i, true);
                }
            }
        }

        private void my_FormClosing(object sender, FormClosingEventArgs e)
        {
            uint dipsw = 0;

            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                if (this.checkedListBox1.GetItemChecked(i))
                {
                    dipsw |= ((uint)1 << i);
                }
            }

            Console.Write("debug control : 0x" + dipsw.ToString("X8") + "\n");
            from1.update_debug_ctrl(dipsw);
        }

        private TopForm from1;
    }
}
