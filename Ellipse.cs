using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SimpleShooter
{
    public class Ellipse
    {
        public Ellipse(int x, int y, double vx, double vy)
        {
            X = x;
            Y = y;
            VX = vx;
            VY = vy;
            IsDead = false;

            Colors.Add(Color.FromArgb(0xff, 0x7f, 0x50));//ff7f50
            Colors.Add(Color.FromArgb(0xff, 0x63, 0x47));//ff6347
            Colors.Add(Color.FromArgb(0xff, 0x45, 0x00));//ff4500
            Colors.Add(Color.FromArgb(0xff, 0x00, 0x00));//ff0000
            Colors.Add(Color.FromArgb(0xff, 0x8c, 0x00));//ff8c00
            Colors.Add(Color.FromArgb(0xff, 0xff, 0x00));//ffff00
        }

        List<Color> Colors = new List<Color>();

        double X
        {
            get;
            set;
        }

        double Y
        {
            get;
            set;
        }

        double VX
        {
            get;
            set;
        }

        double VY
        {
            get;
            set;
        }

        public bool IsDead
        {
            get;
            set;
        }

        int MoveCount = 0;
        public void Move()
        {
            X += VX;
            Y += VY;
            MoveCount++;
        }

        static Random Random = new Random();
        public void Draw(Graphics graphics)
        {
            if (MoveCount < 16)
            {
                Color color = Colors[Random.Next(Colors.Count)];
                graphics.FillEllipse(new SolidBrush(color), new Rectangle((int)X, (int)Y, 4, 4));
            }
            else
                IsDead = true;
        }
    }
}
