using System;
using System.Windows.Forms;
using PSP2TMAPILib;

namespace DipswTool
{
    public partial class TopForm : Form
    {
        private PowerStateClass ps;
        private ITarget tgt;

        public TopForm()
        {
            InitializeComponent();
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            tm = new PSP2TMAPI();
            tm.CheckCompatibility((uint)eCompatibleVersion.BuildVersion);

            tgt = tm.Targets.DefaultTarget;

            this.Text = tgt.Name + " - Dipsw Tool for PlayStation®Vita";

            dipsw_ASLR.Maximum = Int32.MaxValue;
            dipsw_ASLR.Minimum = Int32.MinValue;

            ps = new PowerStateClass(tgt);
            tgt.AdviseTargetEvents(ps);

            try
            {
                switch (tgt.PowerStatus)
                {
                    case ePowerStatus.POWER_STATUS_NO_SUPPLY:
                    case ePowerStatus.POWER_STATUS_OFF:
                        ps.OnPowerState(ePowerOperation.POWEROP_SHUTDOWN, ePowerProgress.POWER_OP_STATUS_COMPLETED);
                        break;
                    case ePowerStatus.POWER_STATUS_ON:
                        ps.OnPowerState(ePowerOperation.POWEROP_BOOT, ePowerProgress.POWER_OP_STATUS_COMPLETED);
                        break;
                    case ePowerStatus.POWER_STATUS_SUSPENDED:
                        ps.OnPowerState(ePowerOperation.POWEROP_SUSPEND, ePowerProgress.POWER_OP_STATUS_COMPLETED);
                        break;
                }

                this.load_dipsw();
            }
            catch
            {
                Console.Write("Target is not connected\n");
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            Console.Write("Dipsw Tool is be exiting...\n");
            tgt.UnadviseTargetEvents(ps);

            // ApplicationExitイベントハンドラを削除
            Application.ApplicationExit -= new EventHandler(Application_ApplicationExit);
        }

        private void load_dipsw()
        {
            ISetting curr = tgt.GetCurrentSetting("kernel:/bootparam");
            ISetting next = tgt.GetSetting("kernel:/bootparam");

            if(curr.BinaryValue.Length != 32)
            {
                Console.Write("Dipsw load error : Invalid size -> " + curr.BinaryValue.Length + "\n");
            }

            update_sdk_flags((uint)BitConverter.ToChar(curr.BinaryValue, 16) | ((uint)BitConverter.ToChar(curr.BinaryValue, 17) << 8) | ((uint)BitConverter.ToChar(curr.BinaryValue, 18) << 16) | ((uint)BitConverter.ToChar(curr.BinaryValue, 19) << 24));
            update_SHELL((uint)BitConverter.ToChar(curr.BinaryValue, 20) | ((uint)BitConverter.ToChar(curr.BinaryValue, 21) << 8) | ((uint)BitConverter.ToChar(curr.BinaryValue, 22) << 16) | ((uint)BitConverter.ToChar(curr.BinaryValue, 23) << 24));
            update_debug_ctrl((uint)BitConverter.ToChar(curr.BinaryValue, 24) | ((uint)BitConverter.ToChar(curr.BinaryValue, 25) << 8) | ((uint)BitConverter.ToChar(curr.BinaryValue, 26) << 16) | ((uint)BitConverter.ToChar(curr.BinaryValue, 27) << 24));
            update_system_ctrl(
                (uint)curr.BinaryValue[28] | ((uint)curr.BinaryValue[29] << 8) | ((uint)curr.BinaryValue[30] << 16) | ((uint)curr.BinaryValue[31] << 24)
            );


            uint cp_rtc = (uint)BitConverter.ToChar(curr.BinaryValue, 0) | ((uint)BitConverter.ToChar(curr.BinaryValue, 1) << 8) | ((uint)BitConverter.ToChar(curr.BinaryValue, 2) << 16) | ((uint)BitConverter.ToChar(curr.BinaryValue, 3) << 24);
            var cp_date = DateTimeOffset.FromUnixTimeSeconds((long)cp_rtc);
            Console.Write("CP RTC (0) : " + cp_rtc + " -> " + cp_date.ToString() + "\n");
            this.dateTimePicker1.Value = DateTime.Parse(cp_date.Year.ToString() + "/" + cp_date.Month.ToString() + "/" + cp_date.Day.ToString());
            this.numericUpDown1.Value = cp_date.Hour;
            this.numericUpDown2.Value = cp_date.Minute;
            this.numericUpDown3.Value = cp_date.Second;

            cp_rtc = (uint)BitConverter.ToChar(curr.BinaryValue, 8) | ((uint)BitConverter.ToChar(curr.BinaryValue, 9) << 8) | ((uint)BitConverter.ToChar(curr.BinaryValue, 10) << 16) | ((uint)BitConverter.ToChar(curr.BinaryValue, 11) << 24);
            cp_date = DateTimeOffset.FromUnixTimeSeconds((long)cp_rtc);
            Console.Write("CP RTC (1) : " + cp_rtc + " -> " + cp_date.ToString() + "\n");
            this.dateTimePicker2.Value = DateTime.Parse(cp_date.Year.ToString() + "/" + cp_date.Month.ToString() + "/" + cp_date.Day.ToString());
            this.numericUpDown4.Value = cp_date.Hour;
            this.numericUpDown5.Value = cp_date.Minute;
            this.numericUpDown6.Value = cp_date.Second;

            board_info = (uint)BitConverter.ToChar(curr.BinaryValue, 4) | ((uint)BitConverter.ToChar(curr.BinaryValue, 5) << 8) | ((uint)BitConverter.ToChar(curr.BinaryValue, 6) << 16) | ((uint)BitConverter.ToChar(curr.BinaryValue, 7) << 24);
            dipsw_ASLR.Value = (uint)BitConverter.ToChar(curr.BinaryValue, 12) | ((uint)BitConverter.ToChar(curr.BinaryValue, 13) << 8) | ((uint)BitConverter.ToChar(curr.BinaryValue, 14) << 16) | ((uint)BitConverter.ToChar(curr.BinaryValue, 15) << 24);

            Console.Write("    board info " + board_info.ToString("X8") + "\n");
            Console.Write("          ASLR " + ((long)this.dipsw_ASLR.Value).ToString("X8") + "\n");
            Console.Write("     SDK (SCE) " + this.sdk_flags.ToString("X8") + "\n");
            Console.Write("         SHELL " + this.shell_flags.ToString("X8") + "\n");
            Console.Write(" debug control " + this.debug_flags.ToString("X8") + "\n");
            Console.Write("system control " + this.system_flags.ToString("X8") + "\n");
        }

        public void update_sdk_flags(uint dipsw)
        {
            this.sdk_flags = dipsw;
            this.label5.Text = "0x" + dipsw.ToString("X8");
        }

        public uint get_sdk_flags()
        {
            return this.sdk_flags;
        }

        public void update_SHELL(uint dipsw)
        {
            this.shell_flags = dipsw;
            this.label8.Text = "0x" + dipsw.ToString("X8");
        }

        public uint get_SHELL()
        {
            return this.shell_flags;
        }

        public void update_debug_ctrl(uint dipsw)
        {
            this.debug_flags = dipsw;
            this.label9.Text = "0x" + dipsw.ToString("X8");
        }

        public uint get_debug_ctrl()
        {
            return this.debug_flags;
        }

        public void update_system_ctrl(uint dipsw)
        {
            this.system_flags = dipsw;
            this.label10.Text = "0x" + dipsw.ToString("X8");
        }

        public uint get_system_ctrl()
        {
            return this.system_flags;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form f = new SDKForm(this);
            f.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form f = new SHELLForm(this);
            f.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form f = new DebugControlForm(this);
            f.ShowDialog();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form f = new SystemControlForm(this);
            f.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if(DialogResult.No == MessageBox.Show("Write current editing dipsw to CP?", "Write to CP", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk))
            {
                Console.Write("Dipsw write canceled by user\n");
                return;
            }

            DateTimeOffset cp_rtc_date_0 = new DateTimeOffset(
                this.dateTimePicker1.Value.Year,
                this.dateTimePicker1.Value.Month,
                this.dateTimePicker1.Value.Day,
                (int)this.numericUpDown1.Value,
                (int)this.numericUpDown2.Value,
                (int)this.numericUpDown3.Value,
                TimeSpan.Zero
            );
            DateTimeOffset cp_rtc_date_1 = new DateTimeOffset(
                this.dateTimePicker2.Value.Year,
                this.dateTimePicker2.Value.Month,
                this.dateTimePicker2.Value.Day,
                (int)this.numericUpDown4.Value,
                (int)this.numericUpDown5.Value,
                (int)this.numericUpDown6.Value,
                TimeSpan.Zero
            );
            Console.WriteLine("{0} --> Unix Seconds: {1}", cp_rtc_date_0, cp_rtc_date_0.ToUnixTimeSeconds());
            Console.WriteLine("{0} --> Unix Seconds: {1}", cp_rtc_date_1, cp_rtc_date_1.ToUnixTimeSeconds());

            long cp_rtc_0 = cp_rtc_date_0.ToUnixTimeSeconds();
            long cp_rtc_1 = cp_rtc_date_1.ToUnixTimeSeconds();

            System.Byte[] dipsw_buffer = {
                (Byte)((cp_rtc_0 >> 0) & 0xFF),
                (Byte)((cp_rtc_0 >> 8) & 0xFF),
                (Byte)((cp_rtc_0 >> 16) & 0xFF),
                (Byte)((cp_rtc_0 >> 24) & 0xFF),
                (Byte)((board_info >> 0) & 0xFF),
                (Byte)((board_info >> 8) & 0xFF),
                (Byte)((board_info >> 16) & 0xFF),
                (Byte)((board_info >> 24) & 0xFF),
                (Byte)((cp_rtc_1 >> 0) & 0xFF),
                (Byte)((cp_rtc_1 >> 8) & 0xFF),
                (Byte)((cp_rtc_1 >> 16) & 0xFF),
                (Byte)((cp_rtc_1 >> 24) & 0xFF),
                (Byte)(((long)dipsw_ASLR.Value >> 0) & 0xFF),
                (Byte)(((long)dipsw_ASLR.Value >> 8) & 0xFF),
                (Byte)(((long)dipsw_ASLR.Value >> 16) & 0xFF),
                (Byte)(((long)dipsw_ASLR.Value >> 24) & 0xFF),
                (Byte)((sdk_flags >> 0) & 0xFF),
                (Byte)((sdk_flags >> 8) & 0xFF),
                (Byte)((sdk_flags >> 16) & 0xFF),
                (Byte)((sdk_flags >> 24) & 0xFF),
                (Byte)((shell_flags >> 0) & 0xFF),
                (Byte)((shell_flags >> 8) & 0xFF),
                (Byte)((shell_flags >> 16) & 0xFF),
                (Byte)((shell_flags >> 24) & 0xFF),
                (Byte)((debug_flags >> 0) & 0xFF),
                (Byte)((debug_flags >> 8) & 0xFF),
                (Byte)((debug_flags >> 16) & 0xFF),
                (Byte)((debug_flags >> 24) & 0xFF),
                (Byte)((system_flags >> 0) & 0xFF),
                (Byte)((system_flags >> 8) & 0xFF),
                (Byte)((system_flags >> 16) & 0xFF),
                (Byte)((system_flags >> 24) & 0xFF)
            };

            try
            {
                if(dipsw_buffer.Length != 32)
                {
                    throw new Exception("Should be not in here\n");
                }

                Setting setting = new Setting();
                setting.Key = "kernel:/bootparam";
                setting.BinaryValue = dipsw_buffer;
                tgt.SetSetting(setting);

                MessageBox.Show("Successful to write the dipsw", "Reload Dipsw", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception excp)
            {
                Console.Write(excp.ToString());
                MessageBox.Show("Failed to write the dipsw", "Reload Dipsw", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private PSP2TMAPI tm;
        private uint board_info = 0;
        private uint sdk_flags = 0;
        private uint shell_flags = 0;
        private uint debug_flags = 0;
        private uint system_flags = 0;

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                this.load_dipsw();
                MessageBox.Show("Successful to reload the dipsw", "Reload Dipsw", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception excp)
            {
                Console.Write(excp.ToString());
                MessageBox.Show("Failed to reload the dipsw", "Reload Dipsw", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.dateTimePicker2.Value = this.dateTimePicker1.Value;
            this.numericUpDown4.Value = this.numericUpDown1.Value;
            this.numericUpDown5.Value = this.numericUpDown2.Value;
            this.numericUpDown6.Value = this.numericUpDown3.Value;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.dateTimePicker1.Value = this.dateTimePicker2.Value;
            this.numericUpDown1.Value = this.numericUpDown4.Value;
            this.numericUpDown2.Value = this.numericUpDown5.Value;
            this.numericUpDown3.Value = this.numericUpDown6.Value;
        }
    }

    class NullTargetEventHandler : IEventTarget, IEventTarget2, IEventTarget3, IEventTarget4, IEventTarget5, IEventTarget6, IEventTarget7, IEventTarget8, IEventTarget9
    {
        protected ITarget m_Target = null;

        public NullTargetEventHandler(ITarget target)
        {
            m_Target = target;
        }

        ~NullTargetEventHandler()
        {
        }

        virtual public void OnProgress(string Task, uint Id, uint Percentage){}

        virtual public void OnFileServingRootChanged(string Root){}
        virtual public void OnFileServingCaseSensitivityChanged(bool cs){}

        virtual public void OnConnect(){}
        virtual public void OnDisconnect(){}

        virtual public void OnSettingsFailed(ISettingCollection c){}
        virtual public void OnSettingsApplied(ISettingCollection c){}
        virtual public void OnSettingsAccepted(ISettingCollection c){}
        virtual public void OnSettingsSynchronized(uint uFlags){}

        virtual public void OnNameUpdate(string s){}
        virtual public void OnPowerState(ePowerOperation op, ePowerProgress progress){}
        virtual public void OnSystemError(eSystemError err, string DumpFile){}

        virtual public void OnPowerInfo(IChannelData data){}
        virtual public void OnForcedPowerOff(){}

        virtual public void OnInstallComplete(string Task, uint Result, uint RawResult){}
        virtual public void OnInstallProgress(string Task, uint Phase, uint Percentage, uint Flags, uint EstimatedDuration){}
        virtual public void OnExpiryTime(uint ExpTime){}

        virtual public void OnConnected(string ConnectionInfo){}
        virtual public void OnPowerData(IChannelData data, eErrorCode Error){}

        virtual public void OnBusy(){}
        virtual public void OnIdle(){}

        virtual public void OnVoltageData(uint uSequenceNum, IVoltageInfoCollection pData, eErrorCode eError){}

        virtual public void OnIPAddressChanged(string bstrIPAddress){}

        virtual public void OnDevKitMode(eDevKitMode uMode){}

        virtual public void OnPairingComplete(string bstrDescription, uint uResult, uint uRawResult){}
        virtual public void OnPairingProgress(string bstrTask, uint uPhase, uint uPercentage, uint uFlags, uint uEstimatedDuration){}

        virtual public void OnSaveDataInstallProgress(string bstrTask, uint uPhase, uint uPercentage, uint uFlags, uint uEstimatedDuration){}
        virtual public void OnSaveDataInstallComplete(string bstrDescription, uint uResult, uint uRawResult){}
        virtual public void OnSaveDataFormatProgress(string bstrTask, uint uPhase, uint uPercentage, uint uFlags, uint uEstimatedDuration){}
        virtual public void OnSaveDataFormatComplete(string bstrDescription, uint uResult, uint uRawResult){}
    }

    class PowerStateClass : NullTargetEventHandler
    {
        public PowerStateClass(ITarget target)
            : base(target)
        {
        }

        public override void OnPowerState(ePowerOperation op, ePowerProgress progress)
        {
            Console.Write(op.ToString() + " " + progress.ToString() + "\n");
        }
    }
}
