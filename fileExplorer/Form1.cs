using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace fileExplorer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            var res = fbd.ShowDialog();
            if (res == DialogResult.OK)
            {
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(DirectoryNode(new DirectoryInfo(fbd.SelectedPath)));
            }
        }


        private static TreeNode DirectoryNode(DirectoryInfo di)
        {
            TreeNode tn;
            DirectoryInfo[] dirs;
            FileInfo[] files;
            try
            {
                tn = new TreeNode(di.Name) {Tag = di};
                dirs = di.GetDirectories();
                files = di.GetFiles();
            }
            catch (Exception)
            {
                return null;
            }
            foreach (var dir in dirs)
            {
                TreeNode newNode = DirectoryNode(dir);
                if (newNode != null)
                {
                    tn.Nodes.Add(newNode);
                }
            }

            foreach (var file in files)
            {
                TreeNode newNode = FileNode(file);
                if (newNode != null)
                {
                    tn.Nodes.Add(newNode);
                }
            }

            return tn;
        }

        private static TreeNode FileNode(FileInfo fi)
        {
            var tn = new TreeNode(fi.Name) {Tag = fi};
            return tn;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode;
            if (node != null)
            {
                var info = (FileSystemInfo) node.Tag;
                if ((info.Attributes & FileAttributes.ReadOnly) != 0)
                    info.Attributes &= ~FileAttributes.ReadOnly;

                if (info is DirectoryInfo)
                {
                    try
                    {
                        ((DirectoryInfo) info).Delete();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Unable to delete the directory!\n" + exception.Message);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        info.Delete();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Unable to delete the file!\n" + exception.Message);
                        return;
                    }
                }
                node.Remove();
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                treeView1.SelectedNode = e.Node;
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode;
            var info = (FileSystemInfo) node?.Tag;

            if (!(info is DirectoryInfo)) return;

            var popup = new Form2(info);
            popup.ShowDialog();

            while (node.FirstNode != null)
                node.FirstNode.Remove();

            var info2 = (DirectoryInfo) info;

            foreach (var dir in info2.GetDirectories())
                node.Nodes.Add(DirectoryNode(dir));

            foreach (var file in info2.GetFiles())
                node.Nodes.Add(FileNode(file));
        }

        private void treeView1_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
            var fi = (FileSystemInfo) treeView1.SelectedNode.Tag;

            var attribs = fi.Attributes;

            toolStripStatusLabel1.Text = ((attribs & FileAttributes.ReadOnly) > 0 ? "r" : "-") +
                                         ((attribs & FileAttributes.Archive) > 0 ? "a" : "-") +
                                         ((attribs & FileAttributes.Hidden) > 0 ? "h" : "-") +
                                         ((attribs & FileAttributes.System) > 0 ? "s" : "-");

            if (Regex.IsMatch(fi.Name, @"\.txt"))
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader(fi.FullName);
                    textBox1.Text = sr.ReadToEnd();
                    sr.Close();
                }
                catch (Exception)
                {
                    sr?.Close();
                }
            }
            else
            {
                textBox1.Text = "";
            }
        }
    }
}