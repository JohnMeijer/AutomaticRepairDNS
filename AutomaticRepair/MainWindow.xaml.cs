using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows;

namespace AutomaticRepair
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            OutputTextBox.Text += "AutomaticRepair for setting an alternative DNS server";

            OutputTextBox.Text += Environment.NewLine;

            OutputTextBox.Text += "By John P.D. Meijer.";

            OutputTextBox.Text += Environment.NewLine;
            OutputTextBox.Text += Environment.NewLine;

            OutputTextBox.Text += "This program was created to help normal computer users to change their DNS server to a alternative public DNS server.";

            OutputTextBox.Text += Environment.NewLine;
            OutputTextBox.Text += Environment.NewLine;

            OutputTextBox.Text += "When you choose to apply this repair your DNS server configuration will be changed to a public DNS server that are selected below.";
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private string PrimaryDNSServerGoogle = "8.8.8.8";
        private string SecondaryDNSServerGoogle = "8.8.4.4";

        private string PrimaryDNSServerOpenDNS = "208.67.222.222";
        private string SecondaryDNSServerOpenDNS = "208.67.220.220";

        private string PrimaryDNSServerOpenNIC = "31.220.43.191";
        private string SecondaryDNSServerOpenNIC = "151.236.29.92";

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            ButtonApply.IsEnabled = false;

            if (GoogleRadioButton.IsChecked == true)
            {
                PrimaryDNSServer = PrimaryDNSServerGoogle;
                SecondaryDNSServer = SecondaryDNSServerGoogle;
            }
            else if (OpenDNSRadioButton.IsChecked == true)
            {
                PrimaryDNSServer = PrimaryDNSServerOpenDNS;
                SecondaryDNSServer = SecondaryDNSServerOpenDNS;
            }
            else if (OpenNICRadioButton.IsChecked == true)
            {
                PrimaryDNSServer = PrimaryDNSServerOpenNIC;
                SecondaryDNSServer = SecondaryDNSServerOpenNIC;
            }

            OutputTextBox.Text += Environment.NewLine;
            OutputTextBox.Text += Environment.NewLine;

            OutputTextBox.Text += "AutomaticRepair found the following compatible network adapters:";

            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();

            List<NetworkInterface> SupportedInterfaces = null;

            foreach (NetworkInterface Adapter in Interfaces)
            {
                if (Adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet && Adapter.Description.Contains("Virtual") == false || Adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && Adapter.Description.Contains("Virtual") == false)
                {

                    OutputTextBox.Text += Environment.NewLine;

                    OutputTextBox.Text += Environment.NewLine + ("Name: " + Adapter.Name);

                    OutputTextBox.Text += Environment.NewLine + (Adapter.Description);
                    OutputTextBox.Text += Environment.NewLine + (String.Empty.PadLeft(Adapter.Description.Length, '='));

                    if (SupportedInterfaces == null)
                    {
                        SupportedInterfaces = new List<NetworkInterface>();
                    }

                    SupportedInterfaces.Add(Adapter);
                }
            }

            if (SupportedInterfaces != null)
            {
                OutputTextBox.Text += Environment.NewLine;
                OutputTextBox.Text += Environment.NewLine;

                OutputTextBox.Text += "AutomaticRepair is starting the command prompt with the command to change all adapters to the selected DNS servers...";

                foreach (NetworkInterface Adapter in SupportedInterfaces)
                {
                    FixNetworkAdapter(Adapter);
                }
            }

            MessageBox.Show("Your DNS server was succesfully changed, all actions are completed.", "", MessageBoxButton.OK, MessageBoxImage.Information);

            if (AdvancedSettingsExpander.IsExpanded ==  false)
            {
                Close();
            }
        }

        private string PrimaryDNSServer = null;
        private string SecondaryDNSServer = null;

        private void FixNetworkAdapter(NetworkInterface Adapter)
        {
            OutputTextBox.Text += Environment.NewLine + (String.Empty.PadLeft(20, '='));
            OutputTextBox.Text += Environment.NewLine;
            OutputTextBox.Text += Environment.NewLine;

            OutputTextBox.Text += "Configuring adapter: " + Adapter.Name + "(" +  Adapter.Description + ")";

            string Quote = @"""";

            string AdapterCommand = Quote + Adapter.Name + Quote;

            RunCommand(@"/C netsh interface ipv4 add dnsserver " + AdapterCommand + " address=" + PrimaryDNSServer + " index=1");

            RunCommand(@"/C netsh interface ipv4 add dnsserver " + AdapterCommand + " address=" + SecondaryDNSServer + " index=2");

            OutputTextBox.Text += Environment.NewLine;
            OutputTextBox.Text += Environment.NewLine;

            OutputTextBox.Text += "Action successful!";
        }

        private void RunCommand(string CommandLine)
        {
            Process CommandProcessHost = new Process();

            CommandProcessHost.StartInfo.FileName = "CMD.exe";
            CommandProcessHost.StartInfo.Arguments = CommandLine;

            CommandProcessHost.StartInfo.UseShellExecute = false;
            CommandProcessHost.StartInfo.RedirectStandardOutput = true;

            CommandProcessHost.Start();

            //string CommandOutput = CommandProcessHost.StandardOutput.ReadToEnd();

            CommandProcessHost.WaitForExit();
        }

        private void AdvancedSettingsExpander_Expanded(object sender, RoutedEventArgs e)
        {
            MinHeight = 300;

            if (Height < 300)
            {
                Height = 300;
            }
        }

        private void AdvancedSettingsExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            MinHeight = 150;
        }
    }
}
