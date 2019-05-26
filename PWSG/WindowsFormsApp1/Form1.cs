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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static int i = 0;
        public static int TopMargin = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            CheckBox checkbox = new CheckBox();
            checkbox.Top = TopMargin += 30;
            checkbox.Left = 10;
            checkbox.Text = $"{i+=10}";
            checkbox.CheckedChanged += button1_Click;
            this.panel1.Controls.Add(checkbox);
            this.tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel1.ColumnCount++;
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,20f));
            Console.WriteLine( this.tableLayoutPanel1.ColumnStyles.Count); 

            this.ResumeLayout();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            button.BackColor = Color.Bisque;
            button.Text = "2";

            Button button3 = new Button();
            button3.AutoSize = true;
            button3.Dock = System.Windows.Forms.DockStyle.Fill;
            button3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            button3.FlatAppearance.BorderSize = 20;
            button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.BlanchedAlmond;
            button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            button3.Location = new System.Drawing.Point(50, 50);
            button3.Margin = new System.Windows.Forms.Padding(50);
            button3.Name = "button2";
            button3.Size = new System.Drawing.Size(200, 125);
            button3.TabIndex = 0;
            button3.Text = "?";
            button3.UseVisualStyleBackColor = true;
            this.tableLayoutPanel1.Controls.Add(button3, 1, 1);
        }

        private Button GetButton()
        {
            Button button3 = new Button();
            button3.AutoSize = true;
            button3.Dock = System.Windows.Forms.DockStyle.Fill;
            button3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            button3.FlatAppearance.BorderSize = 5;
            button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Chocolate;
            button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button3.Font = new System.Drawing.Font("Tahoma", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            button3.Location = new System.Drawing.Point(10, 10);
            button3.Margin = new System.Windows.Forms.Padding(10);
            button3.Name = "button2";
            button3.Size = new System.Drawing.Size(200, 125);
            button3.TabIndex = 0;
            button3.Text = "?";
            //button3.MinimumSize = new Size(100, 80);
            button3.UseVisualStyleBackColor = true;
            return button3;
        }

        public Thread thread;
        public Dictionary<int, Image> dict = new Dictionary<int, Image>();
        private GridManager gridManager;
        private static Random rnd = new Random();
        public void CreateGame(int size)
        {
            
            if (GameGrid == null)
                this.Controls.Remove(tableLayoutPanel1);
            else this.Controls.Remove(GameGrid);

            this.GameGrid = new TableLayoutPanel();
            gridManager = new GridManager(size,this);
           

            this.progressBar1.Value = 2000;
            thread = new Thread(() =>
            {

                while (true)
                {
                    Thread.CurrentThread.IsBackground = true;
                    int ilesekund = size * size *2;
                    Thread.Sleep(ilesekund );
                    progressBar1.BeginInvoke(new Action(() => {if(progressBar1.Value>0) progressBar1.Value -= 1;}));
                    if (progressBar1.Value == 0)
                    {
                        MessageBox.Show("Przegrałeś xdd");
                        gridManager.CanBeClicked = false;
                        return;
                    }
                }

            });
            thread.Start();

            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            GameGrid.SuspendLayout();
            this.SuspendLayout();

            GameGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            GameGrid.ColumnCount = size;
            GameGrid.RowCount = size;
            for(int i =0; i < size; i++)
            {
                GameGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100 / size));
                GameGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100 / size));
            }

            GameGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            GameGrid.AutoSize = true;
            GameGrid.AutoScroll = true;
            //GameGrid.Anchor = AnchorStyles.Right;
            //GameGrid.Anchor = AnchorStyles.Bottom;
            GameGrid.Location = new System.Drawing.Point(200, 100);
            GameGrid.Name = "GameGrid";
            GameGrid.Size = new System.Drawing.Size(876, 557);
            GameGrid.TabIndex = 1;
            GameGrid.Margin = new Padding(50);
            GameGrid.Click += new EventHandler((object o,EventArgs e) => { gridManager.GridClicked(); });

            this.Controls.Remove(this.panel1);
            this.Controls.Remove(this.menuStrip1);

            List<int> Numerki = new List<int>();
            Stack<int> stack = new Stack<int>();
            for (int i = 0; i < size*size/2; i++)
            {
                Numerki.Add(i);Numerki.Add(i);
            }
            while (Numerki.Any())
            {
                int where = rnd.Next() % Numerki.Count();
                stack.Push(Numerki[where]);
                Numerki.RemoveAt(where);
            }
            dict = new Dictionary<int, Image>();
            if (manager != null)
            {
                if (manager.obrazki != null)
                {
                    
                    for (int i = 0; i < manager.obrazki.Count;i++)
                    {
                        dict.Add(i, manager.obrazki[i]);
                    }
                }
            }
            

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    Button button = GetButton();
                    button.Click += GridManager.ButtonFunction1;
                    button.Click += (object sender, EventArgs e) =>
                    {
                        var button1 = sender as Button;
                        gridManager.ButtonClicked(button1);
                    };
                    int cyfra = stack.Pop();
                    button.Tag = new ButtonInfo(i, j, cyfra);
                    
                    //if(cyfra < manager.il.Images.Count)
                    //{
                    //    button.Image = dict[cyfra];
                    //}
                    GameGrid.Controls.Add(button, i, j);

                }

            
            this.Controls.Add(GameGrid);

            //this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);

            this.panel1.ResumeLayout(false);
            GameGrid.ResumeLayout(false);
            GameGrid.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
            
        }

        private System.Windows.Forms.TableLayoutPanel GameGrid = null;

        private void x6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine("xdd");
            CreateGame(6);
        }

        private void x4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateGame(4);
        }
        private void x2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateGame(2);
        }

        private void x8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateGame(8);
        }

        private void x10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateGame(10);
        }

        private Form2 manager;
        private void aboutTheGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            manager = new Form2();
            manager.Show();

        }
    }
}
