using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class ImageManager : Form
    {
        private ListView ImageListView;

        public ImageManager()
        {
            ImageListView = new ListView();
            ImageListView.Dock = DockStyle.Fill;
            this.Controls.Add(ImageListView);
            this.Size = new Size(800, 600);

            ImageList il = new ImageList();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Choose Images";
            ofd.Filter = "JPG|*.jpg|JPEG|*.jpeg|GIF|*.gif|PNG|*.png|All|*.*";
            ofd.Multiselect = true;
            var dr = ofd.ShowDialog();
            if(dr == DialogResult.OK)
            {
                int k = 0;
                string[] files = ofd.FileNames;
                foreach(string filename in files)
                {
                    Image image = Image.FromFile(filename);
                    il.Images.Add(image);
                    ImageListView.Items.Add("key", $"obrazkie {k}", k++);
                }
            }
            ImageListView.View = View.LargeIcon;
            ImageListView.LargeImageList = il;

            ImageListView.SelectedIndexChanged += new EventHandler((object o, EventArgs e) =>
            {
                Console.WriteLine(ImageListView.SelectedIndices);
            });
        }
    }
}
