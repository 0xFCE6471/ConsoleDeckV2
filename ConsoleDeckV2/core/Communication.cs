using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ConsoleDeckV2.core
{
    public class Communication
    {
        private SerialPort serialPort;
        private AppConfig config;
        private int lastVolumeValue = 0;
        private bool isMuted = false;

        public Communication(AppConfig appConfig)
        {
            config = appConfig;
        }

        public void StartListening()
        {
            while (true)
            {
                try
                {
                    string portName = config.SerialConfig.Port;
                    int baudRate = config.SerialConfig.BaudRate;

                    serialPort = new SerialPort(portName, baudRate);
                    serialPort.Open();
                    MessageBox.Show($"[INFO] Connected to {portName}", "Serial Info");

                    while (serialPort.IsOpen)
                    {
                        string line = serialPort.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(line))
                        {
                            HandleSerialInput(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"[ERROR] Serial connection failed: {ex.Message}", "Serial Error");
                    Thread.Sleep(3000);
                }
            }
        }

        private void HandleSerialInput(string line)
        {
            if (line.StartsWith("VOLUME_"))
            {
                string value = line.Replace("VOLUME_", "");
                HandleVolume(value);
            }
            else if (line == "MUTE")
            {
                HandleMute();
            }
            else if (line == "MEDIA")
            {
                HandleMedia();
            }
            else if (config.Buttons.ContainsKey(line))
            {
                ExecuteAction(config.Buttons[line]);
            }
            else
            {
                var match = System.Text.RegularExpressions.Regex.Match(line, @"\d+");
                if (match.Success)
                {
                    string buttonKey = "BUTTON_" + match.Value;
                    if (config.Buttons.ContainsKey(buttonKey))
                        ExecuteAction(config.Buttons[buttonKey]);
                }
            }
        }

        private void ExecuteAction(ButtonAction action)
        {
            try
            {
                if (action.Type.ToLower() == "link" && !string.IsNullOrWhiteSpace(action.Value))
                {
                    Process.Start(new ProcessStartInfo(action.Value) { UseShellExecute = true });
                }
                else if (action.Type.ToLower() == "exe" && !string.IsNullOrWhiteSpace(action.Value))
                {
                    Process.Start(new ProcessStartInfo(action.Value) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void HandleVolume(string value)
        {
            if (!int.TryParse(value, out int newValue))
            {
                return;
            }

            int delta = newValue - lastVolumeValue;
            if (delta == 0) return;

            byte vk = delta > 0 ? VK_VOLUME_UP : VK_VOLUME_DOWN;
            for (int i = 0; i < Math.Abs(delta); i++)
            {
                SimulateKeyPress(vk);
            }

            lastVolumeValue = newValue;
        }

        private void HandleMute()
        {
            SimulateKeyPress(VK_VOLUME_MUTE);
            isMuted = !isMuted;
        }

        private void HandleMedia()
        {
            SimulateKeyPress(VK_MEDIA_PLAY_PAUSE);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const byte VK_VOLUME_UP = 0xAF;
        private const byte VK_VOLUME_DOWN = 0xAE;
        private const byte VK_VOLUME_MUTE = 0xAD;
        private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;

        private void SimulateKeyPress(byte vkCode)
        {
            keybd_event(vkCode, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
            keybd_event(vkCode, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}
