using ConsoleDeckV2.core;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ConsoleDeckV2
{
    public partial class Form1 : Form
    {

        private const int WM_NCLBUTTDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        private AppConfig appConfig;
        private Communication comm;

        public Form1()
        {
            appConfig = ConfigManager.LoadConfig();
            InitializeComponent();

            comm = new Communication(appConfig);
            Task.Run(() => comm.StartListening());

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Open", null, OnOpen);
            trayMenu.Items.Add("Quit", null, OnExit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "ConsoleDeckV2";
            trayIcon.BalloonTipText = "ConsoleDeckV2";
            trayIcon.Icon = this.Icon;
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = false;

            trayIcon.DoubleClick += OnOpen;

            this.MouseDown += Form1_MouseDown;
            foreach (Control ctrl in this.Controls)
            {
                ctrl.MouseDown += Form1_MouseDown;
            }
        }



        private void OnOpen(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            trayIcon.Visible = false;
        }

        private void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.ShowInTaskbar = false;
            trayIcon.Visible = true;


            MessageBox.Show(
                $"👀 The application has been minimized. To close it, go to the System Tray.",
                "Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTDOWN, HTCAPTION, 0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectedButton.Text = "Button 1 Selected";
            selectedButton.Visible = true;

            comboBox1.Text = appConfig.Buttons["BUTTON_1"].Type;
            TextBox.Text = appConfig.Buttons["BUTTON_1"].Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            selectedButton.Text = "Button 2 Selected";
            selectedButton.Visible = true;

            comboBox1.Text = appConfig.Buttons["BUTTON_2"].Type;
            TextBox.Text = appConfig.Buttons["BUTTON_2"].Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            selectedButton.Text = "Button 3 Selected";
            selectedButton.Visible = true;
            comboBox1.Text = appConfig.Buttons["BUTTON_3"].Type;
            TextBox.Text = appConfig.Buttons["BUTTON_3"].Value;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            selectedButton.Text = "Button 4 Selected";
            selectedButton.Visible = true;
            comboBox1.Text = appConfig.Buttons["BUTTON_4"].Type;
            TextBox.Text = appConfig.Buttons["BUTTON_4"].Value;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            selectedButton.Text = "Button 5 Selected";
            selectedButton.Visible = true;
            comboBox1.Text = appConfig.Buttons["BUTTON_5"].Type;
            TextBox.Text = appConfig.Buttons["BUTTON_5"].Value;

        }

        private void button6_Click(object sender, EventArgs e)
        {
            selectedButton.Text = "Button 6 Selected";
            selectedButton.Visible = true;
            comboBox1.Text = appConfig.Buttons["BUTTON_6"].Type;
            TextBox.Text = appConfig.Buttons["BUTTON_6"].Value;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            selectedButton.Text = "Button 7 Selected";
            selectedButton.Visible = true;
            comboBox1.Text = appConfig.Buttons["BUTTON_7"].Type;
            TextBox.Text = appConfig.Buttons["BUTTON_7"].Value;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            selectedButton.Text = "Button 8 Selected";
            selectedButton.Visible = true;
            comboBox1.Text = appConfig.Buttons["BUTTON_8"].Type;
            TextBox.Text = appConfig.Buttons["BUTTON_8"].Value;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            selectedButton.Text = "Button 9 Selected";
            selectedButton.Visible = true;
            comboBox1.Text = appConfig.Buttons["BUTTON_9"].Type;
            TextBox.Text = appConfig.Buttons["BUTTON_9"].Value;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text.ToLower() == "exe")
            {
                pathFinderButton.Visible = true;
            }
            else
            {
                pathFinderButton.Visible = false;
            }

            if(comboBox1.Text.ToLower() == "none")
            {
                TextBox.Clear();
            }
        }

        private void pathFinderButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Executable";
                ofd.Filter = "Executable Files (*.exe)|*.exe";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string exePath = ofd.FileName;
                    if (TextBox.TextLength != 0)
                    {
                        TextBox.Clear();
                    }
                    TextBox.Text = exePath;
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.Length != 0)
            {
                if (selectedButton.Text.Length != 0 && selectedButton.Text.ToLower() != "button not selected")
                {
                    if (comTextBox.Text.Length != 0 && braudTextBox.Text.Length != 0)
                    {
                        string buttonID = "BUTTON_" + Regex.Match(selectedButton.Text, @"\d+").Value;

                        if (!appConfig.Buttons.ContainsKey(buttonID))
                        {
                            appConfig.Buttons[buttonID] = new ButtonAction();
                        }

                        appConfig.Buttons[buttonID].Type = comboBox1.Text;
                        appConfig.Buttons[buttonID].Value = TextBox.Text;

                        ConfigManager.SaveConfig(appConfig);

                        MessageBox.Show(
                            $"✅ Configuration for {buttonID} has been successfully saved!\n\nType: {comboBox1.Text}\nValue: {TextBox.Text}",
                            "Save Successful",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
            }
        }
    }
}