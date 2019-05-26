using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace WindowsFormsApp1
{
    public class GridManager
    {
        private int MaxLocked;
        private int AlreadyLocked = 0;
        bool HasOne = false;
        private Button LastButton = null;
        private Button button = null;
        private System.Timers.Timer HoldTimer;
        Form1 form1;
        int size;
        public GridManager(int size,Form1 form1)
        {
            this.size = size;
            this.form1 = form1;
            MaxLocked = size * size / 2;
        }

        private void RemoveClickEvent(Button b)
        {
            FieldInfo f1 = typeof(Control).GetField("EventClick",
                BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(b);
            PropertyInfo pi = b.GetType().GetProperty("Events",
                BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(b, null);
            list.RemoveHandler(obj, list[obj]);
        }

        public bool CanBeClicked = true;

        public void ButtonClicked(Button button)
        {
            if (!CanBeClicked)
            {
                GridClicked();
                return;
            }
            this.button = button;
            if (HasOne) //Jest cos
            {
                int cyfra = ((ButtonInfo)button.Tag).numer;
                if (cyfra < form1.dict.Count)
                {
                    button.Image = form1.dict[cyfra];
                    button.Text = "";
                }
                else button.Text = ((ButtonInfo)button.Tag).numer.ToString();

                ButtonInfo ButtonInfo1 = (ButtonInfo)LastButton.Tag;
                ButtonInfo ButtonInfo2 = (ButtonInfo)button.Tag;
                if (((ButtonInfo)LastButton.Tag).numer == ((ButtonInfo)button.Tag).numer )
                {
                    button.BackColor = Color.Bisque;
                    LastButton.BackColor = Color.Bisque;
                    //button.Text = ((ButtonInfo)button.Tag).numer.ToString();
                    RemoveClickEvent(LastButton);
                    RemoveClickEvent(button);
                    LastButton.FlatAppearance.MouseOverBackColor = Color.Bisque;
                    button.FlatAppearance.MouseOverBackColor = Color.Bisque;

                    this.AlreadyLocked++;
                    if(AlreadyLocked == MaxLocked)
                    {
                        form1.thread.Abort();
                        MessageBox.Show("Wygrał jebany", "Ale Słabo");
                        form1.CreateGame(size);
                    }
                }
                else // nie znalazło pary
                {
                    int cyfra1 = ((ButtonInfo)button.Tag).numer;
                    if (cyfra1 < form1.dict.Count)
                    {
                        button.Image = form1.dict[cyfra1];
                        button.Text = "";
                    }
                    else button.Text = ((ButtonInfo)button.Tag).numer.ToString();

                        button.BackColor = Color.SeaGreen;
                    
                    HoldTimer = new System.Timers.Timer(1500);
                    HoldTimer.Elapsed += Timer_Elapsed;
                    HoldTimer.Start();
                    CanBeClicked = false;
                }
                HasOne = false;
            }
            else //Pusto
            {
                button.BackColor = Color.SeaGreen;
                button.Text = ((ButtonInfo)button.Tag).numer.ToString();
                int cyfra = ((ButtonInfo)button.Tag).numer;
                if (cyfra < form1.dict.Count)
                {
                    button.Image = form1.dict[cyfra];
                    button.Text = "";
                }
                LastButton = button;
                HasOne = true;
            }

        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            LastButton.BeginInvoke(new Action(() =>
            {
                LastButton.Text = "?";
                LastButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
                button.Text = "?";
                button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
                button.Image = null;
                LastButton.Image = null;
            }));
            
            var timer = sender as System.Timers.Timer;
            timer.Dispose();
            CanBeClicked = true;
        }

        public static void ButtonFunction1(object sender, EventArgs e) 
            {
                var button = sender as Button;
                
            }

        internal void GridClicked()
        {
            Console.WriteLine("got Clicked On");
            if (CanBeClicked) return;
            Console.WriteLine("next step");
            LastButton.Text = "?";
            LastButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            button.Text = "?";
            button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            HoldTimer.Dispose();
            button.Image = null;
            LastButton.Image = null;
            CanBeClicked = true;
        }
    }

    public struct ButtonInfo
    {
        public int x;
        public int y;
        public int numer;

        public ButtonInfo(int x, int y, int numer)
        {
            this.x = x;
            this.y = y;
            this.numer = numer;
        }
    }
}
