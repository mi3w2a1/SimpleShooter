using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SimpleShooter
{
    public class Enemy
    {
        public Enemy(Bitmap bitmap, int startX, int startY, bool moveRight)
        {
            Bitmap = bitmap;
            X = startX;
            Y = startY;
            MoveRight = moveRight;
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
        } = false;

        public bool MoveRight
        {
            get;
            set;
        }

        public void Move()
        {
            if (Form1.FormClientSize.Width < X)
                MoveRight = false;
            if (0 > X)
                MoveRight = true;

            Y++;
            if(MoveRight)
                X += 3;
            else
                X -= 3;

            if (Form1.FormClientSize.Height < Y)
                IsDead = true;
        }

        public void Draw(Graphics graphics)
        {
            if(!IsDead)
                graphics.DrawImage(Bitmap, new Point(X, Y));
        }

    }
}
