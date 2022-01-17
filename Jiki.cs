using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SimpleShooter
{
    public class Jiki
    {
        public Jiki(Bitmap bitmap)
        {
            Bitmap = bitmap;
            X = (Form1.FormClientSize.Width - Bitmap.Width) / 2;
            Y = (int)(Form1.FormClientSize.Height * 0.8);
            IsDead = false;
        }

        public Bitmap Bitmap
        {
            get;
            set;
        }

        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public bool IsDead
        {
            get;
            set;
        }

        public bool MoveLeft = false;
        public bool MoveRight = false;
        public bool MoveUp = false;
        public bool MoveDown = false;

        public void Move()
        {
            if (MoveRight && X + Bitmap.Width -3 < Form1.FormClientSize.Width)
                X += 3;
            if (MoveLeft && X  > 3)
                X -= 3;
            if (MoveUp && Y > 3)
                Y -= 3;
            if (MoveDown && Y + Bitmap.Height - 3 < Form1.FormClientSize.Height)
                Y += 3;
        }

        public void Draw(Graphics graphics)
        {
            if (!IsDead)
                graphics.DrawImage(Bitmap, new Point(X, Y));
        }
    }
}

