namespace DipswTool
{
    partial class DebugControlForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugControlForm));
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.HorizontalScrollbar = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "192 - Enable DMAC6",
            "193 - Enable SDbgSdio, deci4p_sdfmgr, deci4p_sttyp",
            "194 - Enable CP modules",
            "195 - Disable USB Debug",
            "196 - Enable Kernel UART0 console logging. And disable remote power control.",
            "197 - Enable Kernel UART1 console logging",
            "198 - Enable System TTY",
            "199 - Enable TTY stdio",
            "200 - Enable stop when an assertion fails",
            "201 - Enable Assert Level 1",
            "202 - Enable Assert Level 2",
            "203",
            "204 - Enable Debug Level 1",
            "205 - Enable Debug Level 2",
            "206 - Enable Syscall Debug",
            "207",
            "208",
            "209",
            "210 - Enable SCE_DIPSW_ENABLE_TOOL_PHYMEMPART",
            "211 - Enable usermode UART console logging",
            "212 - Enable PA memory mapping for usermode",
            "213 - Enable Tiny PA memory range",
            "214 - Disable ASLR",
            "215 - Disable System Debug process Trace",
            "216 - Enable wipe kernel stack by 0xFF",
            "217 - Enable path logging",
            "218 - Ignore app keystone error",
            "219",
            "220",
            "221",
            "222 - Enable KBL Simple Memory Test over ScePowerScratchPad32KiB",
            "223 - Enable KBL Simple Memory Test over Secure DRAM"});
            this.checkedListBox1.Location = new System.Drawing.Point(12, 12);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(260, 242);
            this.checkedListBox1.TabIndex = 0;
            // 
            // DebugControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.checkedListBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DebugControlForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "debug control";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox1;
    }
}