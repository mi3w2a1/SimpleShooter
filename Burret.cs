using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SimpleShooter
{
    class Burret
    {
        public Burret(Bitmap bitmap, int startX, int startY, double vx, double vy)
        {
            Bitmap = bitmap;
            X = startX;
            Y = startY;
            VX = vx;
            VY = vy;
            IsDead = false;
        }

        public Bitmap Bitmap
        {
            get;
            set;
        }

        public double X
        {
            get;
            set;
        }

        public double Y
        {
            get;
            set;
        }

        public double VX
        {
            get;
            set;
        }

        public double VY
        {
            get;
            set;
        }

        public bool IsDead
        {
            get;
            set;
        }

        public void Move()
        {
            X += VX;
            Y += VY;

            if (X + Bitmap.Width < 0)
                IsDead = true;
            if (Form1.FormClientSize.Width < X)
                IsDead = true;
            if (Y + Bitmap.Height < 0)
                IsDead = true;
            if (Form1.FormClientSize.Height < Y)
                IsDead = true;
        }

        public void Draw(Graphics graphics)
        {
            if (!IsDead)
                graphics.DrawImage(Bitmap, new Point((int)X, (int)Y));
        }
    }
}
