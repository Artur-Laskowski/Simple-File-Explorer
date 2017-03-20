using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace pt_lab2
{
    public partial class Form2 : Form
    {
        private readonly FileSystemInfo _info;
        private bool _archive;
        private bool _hidden;
        private bool _isFile = true;

        private string _name = "";
        private bool _readOnly;
        private bool _system;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(FileSystemInfo info)
        {
            _info = info;
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _readOnly = !_readOnly;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            _archive = !_archive;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            _hidden = !_hidden;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            _system = !_system;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            _isFile = true;
            radioButton2.Checked = false;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            _isFile = false;
            radioButton1.Checked = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _name = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_isFile)
            {
                if (Regex.IsMatch(_name, @"[a-zA-Z0-9_~-]{1,8}\.(txt|php|htm)"))
                {
                    var fs = File.Create(_info.FullName + "\\" + _name);

                    fs.Close();
                }
                else
                {
                    MessageBox.Show("Niepoprawna nazwa pliku!");
                    return;
                }
            }
            else
            {
                Directory.CreateDirectory(_info.FullName + "\\" + _name);
            }

            File.SetAttributes(_info.FullName + "\\" + _name, (_archive ? FileAttributes.Archive : 0) |
                                                              (_readOnly ? FileAttributes.ReadOnly : 0) |
                                                              (_hidden ? FileAttributes.Hidden : 0) |
                                                              (_system ? FileAttributes.System : 0));
            Close();
        }
    }
}