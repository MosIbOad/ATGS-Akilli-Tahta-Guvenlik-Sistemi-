using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Management;
using System.Diagnostics;
using System.Windows;

namespace ATGS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class USBSerialNumber
        {

            string _serialNumber;
            string _driveLetter;

            public string getSerialNumberFromDriveLetter(string driveLetter)
            {
                this._driveLetter = driveLetter.ToUpper();

                if (!this._driveLetter.Contains(":"))
                {
                    this._driveLetter += ":";
                }

                matchDriveLetterWithSerial();

                return this._serialNumber;
            }

            private void matchDriveLetterWithSerial()
            {

                string[] diskArray;
                string driveNumber;
                string driveLetter;

                ManagementObjectSearcher searcher1 = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDiskToPartition");
                foreach (ManagementObject dm in searcher1.Get())
                {
                    diskArray = null;
                    driveLetter = getValueInQuotes(dm["Dependent"].ToString());
                    diskArray = getValueInQuotes(dm["Antecedent"].ToString()).Split(',');
                    driveNumber = diskArray[0].Remove(0, 6).Trim();
                    if (driveLetter == this._driveLetter)
                    {
                        /* This is where we get the drive serial */
                        ManagementObjectSearcher disks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                        foreach (ManagementObject disk in disks.Get())
                        {

                            if (disk["Name"].ToString() == ("\\\\.\\PHYSICALDRIVE" + driveNumber) & disk["InterfaceType"].ToString() == "USB")
                            {
                                this._serialNumber = parseSerialFromDeviceID(disk["PNPDeviceID"].ToString());
                            }
                        }
                    }
                }
            }

            private string parseSerialFromDeviceID(string deviceId)
            {
                string[] splitDeviceId = deviceId.Split('\\');
                string[] serialArray;
                string serial;
                int arrayLen = splitDeviceId.Length - 1;

                serialArray = splitDeviceId[arrayLen].Split('&');
                serial = serialArray[0];

                return serial;
            }

            private string getValueInQuotes(string inValue)
            {
                string parsedValue = "";

                int posFoundStart = 0;
                int posFoundEnd = 0;

                posFoundStart = inValue.IndexOf("\"");
                posFoundEnd = inValue.IndexOf("\"", posFoundStart + 1);

                parsedValue = inValue.Substring(posFoundStart + 1, (posFoundEnd - posFoundStart) - 1);

                return parsedValue;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KilitEkran();
            timer2.Enabled = true;
            timer2.Interval = 1000;
            timer1.Enabled = true;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string saat = DateTime.Now.ToLongTimeString();
            string tarih = DateTime.Now.ToLongDateString();
            label1.Text = saat + Environment.NewLine + tarih;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("shutdown", "-f -s -t 1");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("ShutDown", "/r -f -t 1");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.SetSuspendState(PowerState.Suspend, false, false);
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Bu yazılım Berkan ŞAHİN ve Hakan DAĞLI tarafından kodlanmıştır/tasarlanmıştır.", "<?> Bilgi Mesajı");
        }

        bool sifre_dogrulandi = false;
        private void button1_Click(object sender, EventArgs e)
        {
            label2.Text = "";
            label3.Text = "";
            label4.Text = "";
            DriveInfo[] diskler = DriveInfo.GetDrives();

            foreach (DriveInfo disk in diskler)
            {
                if (disk.DriveType == DriveType.Removable)
                {
                    string sifre_oku, ogr_oku, 
                        klasor = @"sdfgsdgertdfhdfghcgvhbndfg\",
                        sifre_dosya = "w3fggxfvbxbdfghnhgjedrgdfgbxc.exe",
                        ogr = "ogr.exe";
                    string DosyaYolu = disk.Name + klasor + sifre_dosya;

                    if (System.IO.File.Exists(DosyaYolu))
                    {
                        StreamReader oku; //yol bilgisini akıştan okuyacak. yani dosyamızı okuyacak

                        oku = File.OpenText(disk.Name + klasor + sifre_dosya);
                        while ((sifre_oku = oku.ReadLine()) != null) //satır boş olana kadar satır satır okumaya devam eder
                        {
                            label2.Text = sifre_oku.ToString();
                        }
                        oku.Close();//okumayı kapat

                        // ogr list okuma //
                        StreamReader ogrisim_oku; //yol bilgisini akıştan okuyacak. yani dosyamızı okuyacak

                        if (System.IO.File.Exists(disk.Name + klasor + ogr))
                        {
                            ogrisim_oku = File.OpenText(disk.Name + klasor + ogr);
                            while ((ogr_oku = ogrisim_oku.ReadLine()) != null) //satır boş olana kadar satır satır okumaya devam eder
                            {
                                label3.Text = ogr_oku.ToString();
                            }
                            oku.Close();//okumayı kapat
                        }

                        //////////////////////////

                        label4.Text = disk.Name;
                        string Drive = label4.Text.Substring(0, 2);
                        USBSerialNumber usb = new USBSerialNumber();
                        string serial = usb.getSerialNumberFromDriveLetter(Drive);
                        string sifre = serial + "usbpassword"; // şifreleme yöntemi reader

                        if (label2.Text == sifre)
                        {
                            sifre_dogrulandi = true;
                            this.Hide();
                            notifyIcon1.Visible = true;
                            notifyIcon1.BalloonTipText = "Hoş geldiniz " + label3.Text + " " + "\nİyi dersler...";
                            notifyIcon1.BalloonTipTitle = "Akıllı Tahta Güvenlik Sistemi";
                            notifyIcon1.Text = "ATGS koruması aktif.";
                            notifyIcon1.ShowBalloonTip(1000);
                            MessageBox.Show("USB Flaş bellek içerisindeki şifre başarıyla doğrulandı akıllı tahtayı kullanabilirsiniz.", "<?> Bilgi Mesajı");
                        }
                    }
                    else
                    {
                        sifre_dogrulandi = false;
                    }
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.F4)
            {
                e.Handled = true;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            var diskler = DriveInfo.GetDrives();

            foreach (DriveInfo disk in diskler)
            {
                string yazi,
                sifre_dosya = "w3fggxfvbxbdfghnhgjedrgdfgbxc.exe",
                klasor = @"sdfgsdgertdfhdfghcgvhbndfg\",
                DosyaYolu = label4.Text + klasor + sifre_dosya;

                if (disk.DriveType == DriveType.Removable)
                {
                    if (File.Exists(DosyaYolu) == true) // şifre var mı ?
                    {
                        string Drive = label4.Text.Substring(0, 2);
                        USBSerialNumber usb = new USBSerialNumber();
                        string serial = usb.getSerialNumberFromDriveLetter(Drive);
                        string sifre = serial + "usbpassword"; // şifreleme yöntemi reader
                        // dosya varsa

                        if (File.Exists(DosyaYolu) == true)
                        {
                            StreamReader oku; //yol bilgisini akıştan okuyacak. yani dosyamızı okuyacak
                            oku = File.OpenText(label4.Text + klasor + sifre_dosya);
                            while ((yazi = oku.ReadLine()) != null) //satır boş olana kadar satır satır okumaya devam eder
                            {
                                label2.Text = yazi.ToString();
                            }
                            oku.Close();//okumayı kapat

                            if (label2.Text != sifre)
                            {
                                KilitEkran();
                                sifre_dogrulandi = false;
                            }
                        }
                    }
                    else
                    {
                        // dosya yoksa
                        KilitEkran();
                    }
                }
                if (File.Exists(DosyaYolu) == false) // dizindeki dosya yok mu ?
                {
                    KilitEkran();
                }
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (sifre_dogrulandi == false)
            {
                KilitEkran();
            }
        }

        private void KilitEkran()
        {
            Show();
            Activate();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("Bu yazılım Berkan ŞAHİN ve Hakan DAĞLI tarafından kodlanmıştır/tasarlanmıştır.", "<?> Bilgi Mesajı");
        }
    }
}