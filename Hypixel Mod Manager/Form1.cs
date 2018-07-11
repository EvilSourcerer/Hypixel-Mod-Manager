using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Http;
namespace Hypixel_Mod_Manager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static List<string> urls = new List<string>();
        static List<string> filenames = new List<string>();
        static WebClient wb = new WebClient();
        static int pagecount = 1;
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                textBox1.Text = dlg.SelectedPath;
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            if (Directory.Exists(textBox1.Text))
            {
                panel2.Visible = false;
                if (!File.Exists("modspath"))
                {
                    using (StreamWriter sw = new StreamWriter("modspath", true))
                    {
                        sw.WriteLine(textBox1.Text);
                    }

                }
                else
                {
                    File.WriteAllText("modspath", textBox1.Text);
                }
                DirectoryInfo d = new DirectoryInfo(textBox1.Text);
                FileInfo[] Files = d.GetFiles("*.jar");
                List<string> filenames = new List<string>();
                foreach (FileInfo file in Files)
                {
                    filenames.Add(file.Name);
                }
                listBox1.Items.Clear();

                for (int i = 0; i < filenames.Count; i++)
                {
                    listBox1.Items.Add(filenames[i].ToString());
                    listBox1.Items.Add("\n");
                }
            }
            else
            {
                MessageBox.Show("The Directory doesn't exist");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WebClient updater = new WebClient();
            updater.DownloadStringAsync(new Uri("OBFUSCATED [FOR SECURITY REASONS]"));
            updater.DownloadStringCompleted += Updater_DownloadStringCompleted;
            if(File.Exists("modspath"))
            {
                textBox1.Text = File.ReadAllText("modspath");
            }
            
        }

        private void Updater_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if(e.Result!="1.3")
            {
                MessageBox.Show("An update is available!");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                if (listBox1.SelectedItem.ToString() != "\n")
                {
                    File.Delete(textBox1.Text + "\\" + listBox1.SelectedItem);
                    listBox1.Items.RemoveAt(listBox1.SelectedIndex + 1);
                    listBox1.Items.RemoveAt(listBox1.SelectedIndex);

                }
            }
        }

        private void visualStudioTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(visualStudioTabControl1.SelectedTab.Text=="Download Mods")
            {
                wb.DownloadStringAsync(new Uri("OBFUSCATED [FOR SECURITY REASONS]" + pagecount));
                wb.DownloadStringCompleted += Wb_DownloadStringCompleted;
                wb.DownloadProgressChanged += Wb_DownloadProgressChanged;
            }
            if(visualStudioTabControl1.SelectedTab.Text=="Manage Mods")
            {
                DirectoryInfo d = new DirectoryInfo(textBox1.Text);
                FileInfo[] Files = d.GetFiles("*.jar");
                List<string> filenames = new List<string>();
                foreach (FileInfo file in Files)
                {
                    filenames.Add(file.Name);
                }
                listBox1.Items.Clear();

                for (int i = 0; i < filenames.Count; i++)
                {
                    listBox1.Items.Add(filenames[i].ToString() + "\n");
                    listBox1.Items.Add("\n");
                }
            }
            if (visualStudioTabControl1.SelectedTab.Text == "Suggest A Mod")
            {
                System.Diagnostics.Process.Start("OBFUSCATED [FOR SECURITY REASONS]");
            }
        }

        private void Wb_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
        }

        private void Wb_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            urls.Clear();
            filenames.Clear();
            int count = e.Result.Split('\n').Length-1;
            listBox2.Items.Clear();
            for(int i=0; i<count; i++)
            {
                listBox2.Items.Add(Regex.Matches(e.Result.Split('\n')[i], "(?<={).*?(?=})")[0] + " - " + Regex.Matches(e.Result.Split('\n')[i], "(?<={).*?(?=})")[1]+"\n");
                listBox2.Items.Add("\n");
                urls.Add(Regex.Matches(e.Result.Split('\n')[i], "(?<={).*?(?=})")[2].Value);
                filenames.Add(Regex.Matches(e.Result.Split('\n')[i], "(?<={).*?(?=})")[0].Value);
            }
            pictureBox1.Visible = false;
            int visibleItems = listBox2.ClientSize.Height / listBox2.ItemHeight;
            listBox2.TopIndex = Math.Max(listBox2.Items.Count - visibleItems + 1, 0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            pictureBox1.Visible = true;
            wb.DownloadStringAsync(new Uri("OBFUSCATED [FOR SECURITY REASONS]" + pagecount));
            wb.DownloadStringCompleted += Wb_DownloadStringCompleted;
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem.ToString() != "\n")
            {
                WebClient installer = new WebClient();
                string finalurl;
                HttpClient client = new HttpClient();
                if (File.ReadAllText("downloadedmods").Contains(urls[listBox2.SelectedIndex / 2]))
                {
                    finalurl = Regex.Match(urls[listBox2.SelectedIndex / 2] + "&redownload=true", "(?<=url=).*?(?=&redownload)").Value;
                    var responseString = await client.GetStringAsync(urls[listBox2.SelectedIndex / 2] + "&redownload=true");
                }
                else
                {
                    File.AppendAllText("downloadedmods", "\n" + urls[listBox2.SelectedIndex / 2]);
                    finalurl = Regex.Match(urls[listBox2.SelectedIndex / 2] + "&redownload=false","(?<=url=).*?(?=&redownload)").Value;
                    var responseString = await client.GetStringAsync(urls[listBox2.SelectedIndex / 2] + "&redownload=false");
                }

                Uri downloaduri = new Uri(finalurl);
                installer.DownloadFileAsync(downloaduri, textBox1.Text + "\\" + filenames[listBox2.SelectedIndex/2] + ".jar");
                installer.DownloadProgressChanged += Installer_DownloadProgressChanged;
                installer.DownloadFileCompleted += Installer_DownloadFileCompleted;
            }
        }

        private void Installer_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            colourProgressBar1.Value = 0;
            
        }

        private void Installer_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            colourProgressBar1.Value = e.ProgressPercentage / 10;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pagecount++;
            listBox2.Items.Clear();
            pictureBox1.Visible = true;
            
            wb.DownloadStringAsync(new Uri("OBFUSCATED [FOR SECURITY REASONS]" + pagecount));
            wb.DownloadStringCompleted += Wb_DownloadStringCompleted;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            pictureBox1.Visible = true;
            
            wb.DownloadStringAsync(new Uri("OBFUSCATED [FOR SECURITY REASONS]" + pagecount + "&query=" + textBox2.Text.Replace(" ","%20")));
            wb.DownloadStringCompleted += Wb_DownloadStringCompleted1;
        }

        private void Wb_DownloadStringCompleted1(object sender, DownloadStringCompletedEventArgs e)
        {
            urls.Clear();
            filenames.Clear();
            int count = e.Result.Split('\n').Length - 1;
            listBox2.Items.Clear();
            for (int i = 0; i < count; i++)
            {
                listBox2.Items.Add(Regex.Matches(e.Result.Split('\n')[i], "(?<={).*?(?=})")[0] + " - " + Regex.Matches(e.Result.Split('\n')[i], "(?<={).*?(?=})")[1] + "\n");
                listBox2.Items.Add("\n");
                urls.Add(Regex.Matches(e.Result.Split('\n')[i], "(?<={).*?(?=})")[2].Value);
                filenames.Add(Regex.Matches(e.Result.Split('\n')[i], "(?<={).*?(?=})")[0].Value);
            }
            pictureBox1.Visible = false;
            
            
        }
    }
}
