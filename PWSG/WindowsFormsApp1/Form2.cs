using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.listView1.TileSize = new Size(150, 60);
            var columnHeader1 = new ColumnHeader();
            columnHeader1.Width = 150 + 120;
            columnHeader1.Text = "Nazwa Pliku";
            var columnHeader2 = new ColumnHeader();
            columnHeader2.Width = 200;
            columnHeader2.Text = "Nazwa Katalogu";
            var columnHeader3 = new ColumnHeader();
            columnHeader3.Width = 100;
            columnHeader3.Text = "Rozdzielczość";
            this.listView1.View = View.SmallIcon;
            this.listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2,columnHeader3 });

            this.FormClosing += Form2_FormClosing;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (MessageBox.Show("Do you want to save changes before closing?", "Save changes", MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Yes:
                    //SaveState(false);
                    break;
                case DialogResult.Cancel:
                    return;
            }
        }

        public ImageList il = new ImageList();
        public List<Image> obrazki = new List<Image>();

        private void loadImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            il.ImageSize = new Size(150, 60);
           
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Choose Images";
            ofd.Filter = "Allzd|*.*|JPG|*.jpg|JPEG|*.jpeg|GIF|*.gif|PNG|*.png";
            ofd.Multiselect = true;
            var dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                int k = this.obrazki.Count();
                string[] files = ofd.FileNames;
                foreach (string filename in files)
                {
                    FileInfo fi = new FileInfo(filename);
                    Image image = Image.FromFile(filename);
                    il.Images.Add(image);
                    obrazki.Add((Image)image.Clone());
                    ListViewItem item = new ListViewItem(new string[] { fi.Name, fi.DirectoryName, image.VerticalResolution.ToString() + " X " + image.HorizontalResolution.ToString() },k++);
                    this.listView1.Items.Add(item);
                }
            }
            ImageList lista = new ImageList();
            lista.Images.AddRange(obrazki.ToArray());
            il = lista;

            this.listView1.LargeImageList = il;
            this.listView1.SmallImageList = il;
            this.ResumeLayout();
        }

        private void listaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageList lista = new ImageList();
            lista.ImageSize = new Size(150, 60);
            lista.Images.AddRange(obrazki.ToArray());
            il = lista;
            this.listView1.LargeImageList = il;
            this.listView1.SmallImageList = il;
            this.listView1.View = View.List;
        }

        private void sczegółyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.View = View.Details;
            ImageList lista = new ImageList();
            lista.ImageSize = new Size(40, 20);
            lista.Images.AddRange(obrazki.ToArray());
            il = lista;
            this.listView1.LargeImageList = il;
            this.listView1.SmallImageList = il;
        }

        private void minitaturyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.View = View.LargeIcon;
            ImageList lista = new ImageList();
            lista.ImageSize = new Size(80, 40);
            lista.Images.AddRange(obrazki.ToArray());
            il = lista;
            this.listView1.LargeImageList = il;
            this.listView1.SmallImageList = il;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            RemoveItems();
        }

        private void RemoveItems()
        {
            var collection = listView1.SelectedIndices.Cast<int>();
            collection =  collection.OrderByDescending(x => x);
            foreach (var item in collection)
            {
                
                int index = (int)item;
                il.Images.RemoveAt(index);
                this.listView1.Items.RemoveAt(index);
                this.obrazki.RemoveAt(index);

            }
        }

        private void listView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine(e.KeyChar);
            Console.WriteLine(sender.GetType());
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveItems();
            }
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            Console.WriteLine(sender.GetType());
            var list = sender as ListView;
            Form form = new Form();
            PictureBox picture = new PictureBox();
            if (list.SelectedIndices.Count == 0) return;
            picture.Image = obrazki[(int)list.SelectedIndices[0]];
            picture.Dock = DockStyle.Fill;
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            form.Controls.Add(picture);
            form.Size = new Size(1000, 500);
            form.Show();
        }
    }
}
