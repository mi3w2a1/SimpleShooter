using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleShooter
{
	public partial class Form1 : Form
	{
		Bitmap CharactersBitmap = null;
		Bitmap JikiBitmap = null;
		Bitmap EnemyBitmap1 = null;
		Bitmap EnemyBitmap2 = null;
		Bitmap EnemyBitmap3 = null;
		Bitmap BurretBitmap = null;

		Timer timer = new Timer();
		Jiki Jiki = null;

		public Form1()
		{
			InitializeComponent();
			DoubleBuffered = true;
			BackColor = Color.Black;

			GetBitmaps();
			InitTimer();

			Jiki = new Jiki(JikiBitmap);
			PlayBGM();
		}

		WMPLib.WindowsMediaPlayer mediaPlayerHit = new WMPLib.WindowsMediaPlayer();
		void PlaySeHit()
		{
			string path = Application.StartupPath + "\\hit.mp3";
			if (System.IO.File.Exists(path))
			{
				mediaPlayerHit.settings.autoStart = true;
				mediaPlayerHit.URL = path;
			}
		}

		WMPLib.WindowsMediaPlayer mediaPlayerBGM = new WMPLib.WindowsMediaPlayer();
		void PlayBGM()
		{
			string path = Application.StartupPath + "\\bgm.mp3";
			if (System.IO.File.Exists(path))
			{
				mediaPlayerBGM.settings.autoStart = true;
				mediaPlayerBGM.URL = path;
			}
		}

		WMPLib.WindowsMediaPlayer mediaPlayerDead = new WMPLib.WindowsMediaPlayer();
		void PlaySeDead()
		{
			string path = Application.StartupPath + "\\dead.mp3";
			if (System.IO.File.Exists(path))
			{
				mediaPlayerDead.settings.autoStart = true;
				mediaPlayerDead.URL = path;
			}
		}

		void GetBitmaps()
		{
			CharactersBitmap = Properties.Resources.sprite2;
			JikiBitmap = GetJikiBitmap();
			EnemyBitmap1 = GetEnemyBitmap1();
			EnemyBitmap2 = GetEnemyBitmap2();
			EnemyBitmap3 = GetEnemyBitmap3();
			BurretBitmap = GetBurretBitmap();
		}

		void InitTimer()
		{
			timer.Interval = 1000 / 60;
			timer.Tick += Timer_Tick;
			timer.Start();
		}

		int TickCount = 0;
		List<Enemy> Enemies = new List<Enemy>();

		void MoveBurrets()
		{
			foreach (Burret burret in JikiBurrets)
				burret.Move();

			foreach (Burret burret in EnemyBurrets)
				burret.Move();
		}

		// 新たに敵をつくる
		Random Random = new Random();
		void CreateNewEnemy()
		{
			int r = TickCount % 15;
			if (r != 0)
				return;

			if (Random.Next(4) != 0)
			{
				int kind = Random.Next(3);
				int x = Random.Next(this.Width);
				bool isRight = Random.Next(2) == 0 ? true : false;

				if (kind == 0)
					Enemies.Add(new Enemy(EnemyBitmap1, x, 0, isRight));
				if (kind == 1)
					Enemies.Add(new Enemy(EnemyBitmap2, x, 0, isRight));
				if (kind == 2)
					Enemies.Add(new Enemy(EnemyBitmap3, x, 0, isRight));
			}
		}

		Burret EnemyShot(Enemy enemy)
		{
			int a = TickCount % 30;
			if (a != 0)
				return null;

			int r = Random.Next(3);
			if(r == 0)
			{
				int x = enemy.X + this.EnemyBitmap1.Width / 2 - BurretBitmap.Width / 2;

				double angle = Math.Atan2(Jiki.Y - enemy.Y, Jiki.X - enemy.X);
				double vx = Math.Cos(angle) * 5;
				double vy = Math.Sin(angle) * 5;
				return new Burret(BurretBitmap, x, enemy.Y, vx, vy);
			}
			return null;
		}

		void EnemyDead(Enemy enemy)
		{
			int x = enemy.X + this.EnemyBitmap1.Width / 2;
			int y = enemy.Y + this.EnemyBitmap1.Height / 2;
			Explosion(x, y);
			Score += 10;
			PlaySeHit();
		}

		void JikiDead()
		{
			int x = Jiki.X + this.JikiBitmap.Width / 2;
			int y = Jiki.Y + this.JikiBitmap.Height / 2;
			Explosion(x, y);
			PlaySeDead();
			mediaPlayerBGM.close();
		}

		void ShowGameOverIfDead(Graphics graphics)
		{
			if (!Jiki.IsDead)
				return;

			string gameOver = "Game Over";
			Font gameOverFont = new Font("ＭＳ ゴシック", 32);
			string retry = "Retry Press S Key";
			Font retryFont = new Font("ＭＳ ゴシック", 24);

			// 描画される文字の幅、高さを調べるために画面の外側に書いてみる
			Point pt = new Point(10, this.Height);
			Size gameOverSize = TextRenderer.MeasureText(graphics, gameOver, gameOverFont, new Size(this.ClientSize.Width, this.ClientSize.Height), TextFormatFlags.NoPadding);
			Size retrySize = TextRenderer.MeasureText(graphics, retry, retryFont, new Size(this.ClientSize.Width, this.ClientSize.Height), TextFormatFlags.NoPadding);

			// 描画される文字の幅、高さが取得できる
			int gameOverWidth = gameOverSize.Width;
			int gameOverHeight = gameOverSize.Height;
			int retryWidth = retrySize.Width;
			int retryHeight = retrySize.Height;

			// 画面の中央になるように「Game Over」と描画する
			int gameOverX = (this.ClientSize.Width - gameOverWidth) / 2;
			int gameOverY = (this.ClientSize.Height - gameOverHeight) / 2;
			graphics.DrawString(gameOver, gameOverFont, new SolidBrush(Color.White), new Point(gameOverX, gameOverY));

			// その下に「Retry Press S Key」と描画する
			int retryX = (this.ClientSize.Width - retryWidth) / 2;
			int retryY = gameOverY + gameOverHeight + 20;
			graphics.DrawString(retry, retryFont, new SolidBrush(Color.White), new Point(retryX, retryY));
		}

		void HitJudge()
		{
			foreach (Enemy enemy in Enemies)
			{
				Burret burret = JikiBurrets.FirstOrDefault(x => IsHit(enemy, x));
				if (burret != null)
				{
					JikiBurrets.Remove(burret);
					enemy.IsDead = true;
					EnemyDead(enemy);
				}
			}
			Enemies = Enemies.Where(x => !x.IsDead).ToList();
			JikiBurrets = JikiBurrets.Where(x => !x.IsDead).ToList();

			if (Jiki.IsDead)
				return;

			Burret enemyBurret = EnemyBurrets.FirstOrDefault(x => IsHit(Jiki, x));
			if (enemyBurret != null)
			{
				EnemyBurrets.Remove(enemyBurret);
				Jiki.IsDead = true;
				JikiDead();
			}
			EnemyBurrets = EnemyBurrets.Where(x => !x.IsDead).ToList();

			Enemy enemy1 = Enemies.FirstOrDefault(x => IsHit(Jiki, x));
			if (enemy1 != null)
			{
				Enemies.Remove(enemy1);
				Jiki.IsDead = true;
				JikiDead();
			}
		}

		bool IsHit(Rectangle rect1, Rectangle rect2)
		{
			if (rect2.Right < rect1.Left)
				return false;
			if (rect2.Bottom < rect1.Top)
				return false;
			if (rect1.Right < rect2.Left)
				return false;
			if (rect1.Bottom < rect2.Top)
				return false;
			return true;
		}

		bool IsHit(Enemy enemy, Burret burret)
		{
			Rectangle rect1 = new Rectangle(enemy.X, enemy.Y, EnemyBitmap1.Width, EnemyBitmap1.Height);
			Rectangle rect2 = new Rectangle((int)burret.X, (int)burret.Y, BurretBitmap.Width, BurretBitmap.Height);
			return IsHit(rect1, rect2);
		}

		bool IsHit(Jiki jiki, Burret burret)
		{
			Rectangle rect1 = new Rectangle(jiki.X, jiki.Y, JikiBitmap.Width, JikiBitmap.Height);
			Rectangle rect2 = new Rectangle((int)burret.X, (int)burret.Y, BurretBitmap.Width, BurretBitmap.Height);
			return IsHit(rect1, rect2);
		}

		bool IsHit(Jiki jiki, Enemy enemy)
		{
			Rectangle rect1 = new Rectangle(jiki.X, jiki.Y, JikiBitmap.Width, JikiBitmap.Height);
			Rectangle rect2 = new Rectangle((int)enemy.X, (int)enemy.Y, EnemyBitmap1.Width, EnemyBitmap1.Height);
			return IsHit(rect1, rect2);
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			TickCount++;

			// 自機を移動させる
			Jiki.Move();

			// 弾丸を移動させる
			MoveBurrets();

			foreach (Enemy enemy in Enemies)
			{
				// 敵を移動させる
				enemy.Move();

				// 敵に弾丸を発射させる
				Burret burret = EnemyShot(enemy);
				if (burret != null)
					EnemyBurrets.Add(burret);
			}

			// 当たり判定
			HitJudge();

			// 新たに敵をつくる
			CreateNewEnemy();

			Ellipses = Ellipses.Where(x => !x.IsDead).ToList();
			foreach (Ellipse ellipse in Ellipses)
				ellipse.Move();

			Invalidate();
		}

		Bitmap GetJikiBitmap()
		{
			//戦闘機であれば左上の座標は（57,1）、サイズは幅44、高さ48です。
			Rectangle sourceRectange = new Rectangle(new Point(57, 1), new Size(44, 48));

			int destWidth = 44;
			int destHeight = 46;
			Bitmap bitmap1 = new Bitmap(destWidth, destHeight);
			Graphics graphics = Graphics.FromImage(bitmap1);
			graphics.DrawImage(CharactersBitmap, new Rectangle(0, 0, destWidth, destHeight), sourceRectange, GraphicsUnit.Pixel);
			graphics.Dispose();
			return bitmap1;
		}

		Bitmap GetEnemyBitmap1()
		{
			// 黄色いひよこ。左上の座標は（4,61）、幅24、高さ28です。
			Rectangle sourceRectange = new Rectangle(new Point(4, 61), new Size(24, 28));

			int destWidth = 24;
			int destHeight = 28;
			Bitmap bitmap1 = new Bitmap(destWidth, destHeight);
			Graphics graphics = Graphics.FromImage(bitmap1);
			graphics.DrawImage(CharactersBitmap, new Rectangle(0, 0, destWidth, destHeight), sourceRectange, GraphicsUnit.Pixel);
			graphics.Dispose();
			return bitmap1;
		}

		Bitmap GetEnemyBitmap2()
		{
			// ピンク色のひよこ。左上の座標は（4,94）、幅24、高さ28
			Rectangle sourceRectange = new Rectangle(new Point(4, 94), new Size(24, 28));

			int destWidth = 24;
			int destHeight = 28;
			Bitmap bitmap1 = new Bitmap(destWidth, destHeight);
			Graphics graphics = Graphics.FromImage(bitmap1);
			graphics.DrawImage(CharactersBitmap, new Rectangle(0, 0, destWidth, destHeight), sourceRectange, GraphicsUnit.Pixel);
			graphics.Dispose();
			return bitmap1;
		}

		Bitmap GetEnemyBitmap3()
		{
			// 青色のひよこ。左上の座標は（4,125）、幅24、高さ28です。
			Rectangle sourceRectange = new Rectangle(new Point(4, 125), new Size(24, 28));

			int destWidth = 24;
			int destHeight = 28;
			Bitmap bitmap1 = new Bitmap(destWidth, destHeight);
			Graphics graphics = Graphics.FromImage(bitmap1);
			graphics.DrawImage(CharactersBitmap, new Rectangle(0, 0, destWidth, destHeight), sourceRectange, GraphicsUnit.Pixel);
			graphics.Dispose();
			return bitmap1;
		}

		Bitmap GetBurretBitmap()
		{
			// 弾丸。左上の座標は（41，46）、幅14、高さ14です。
			Rectangle sourceRectange = new Rectangle(new Point(41, 46), new Size(14, 14));

			int destWidth = 14;
			int destHeight = 14;
			Bitmap bitmap1 = new Bitmap(destWidth, destHeight);
			Graphics graphics = Graphics.FromImage(bitmap1);
			graphics.DrawImage(CharactersBitmap, new Rectangle(0, 0, destWidth, destHeight), sourceRectange, GraphicsUnit.Pixel);
			graphics.Dispose();
			return bitmap1;
		}

		List<Ellipse> Ellipses = new List<Ellipse>();
		void Shot2()
		{
		}

		void Explosion(int centerX, int centerY)
		{
			for (int i = 0; i < 128; i++)
			{
				double vx = (Random.Next(40) - 20) / 10.0;
				double vy = (Random.Next(40) - 20) / 10.0;
				Ellipses.Add(new Ellipse(centerX, centerY, vx, vy));
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			foreach (Burret burret in JikiBurrets)
				burret.Draw(e.Graphics);
			foreach (Burret burret in EnemyBurrets)
				burret.Draw(e.Graphics);

			Jiki.Draw(e.Graphics);
			foreach (Enemy enemy in Enemies)
				enemy.Draw(e.Graphics);

			foreach (Ellipse ellipse in Ellipses)
				ellipse.Draw(e.Graphics);

			ShowScore(e.Graphics);

			ShowGameOverIfDead(e.Graphics);

			base.OnPaint(e);
		}

		int Score = 0;

		void ShowScore(Graphics graphics)
		{
			string scoreString = Score.ToString("00000");
			graphics.DrawString(scoreString, new Font("ＭＳ ゴシック", 18), new SolidBrush(Color.White), new Point(10,10));
		}

		public static Size FormClientSize = Size.Empty;

		protected override void OnLoad(EventArgs e)
		{
			FormClientSize = this.ClientSize;

			base.OnLoad(e);
		}

		protected override void OnResize(EventArgs e)
		{
			FormClientSize = this.ClientSize;

			base.OnResize(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Left)
				Jiki.MoveLeft = true;
			if (e.KeyCode == Keys.Right)
				Jiki.MoveRight = true;
			if (e.KeyCode == Keys.Up)
				Jiki.MoveUp = true;
			if (e.KeyCode == Keys.Down)
				Jiki.MoveDown = true;
			if (e.KeyCode == Keys.Space && !Jiki.IsDead)
				Shot();
			if (e.KeyCode == Keys.S && Jiki.IsDead)
				Retry();
			base.OnKeyDown(e);
		}

		void Retry()
		{
			Enemies.Clear();
			EnemyBurrets.Clear();

			Jiki.X = (FormClientSize.Width - JikiBitmap.Width) / 2;
			Jiki.Y = (int)(FormClientSize.Height * 0.8);

			Jiki.IsDead = false;
			Score = 0;
			PlayBGM();
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Left)
				Jiki.MoveLeft = false;
			if (e.KeyCode == Keys.Right)
				Jiki.MoveRight = false;
			if (e.KeyCode == Keys.Up)
				Jiki.MoveUp = false;
			if (e.KeyCode == Keys.Down)
				Jiki.MoveDown = false;
			base.OnKeyDown(e);
		}

		List<Burret> JikiBurrets = new List<Burret>();
		List<Burret> EnemyBurrets = new List<Burret>();
		void Shot()
		{
			int burretX = Jiki.X + JikiBitmap.Width / 2 - BurretBitmap.Width / 2;
			JikiBurrets.Add(new Burret(BurretBitmap, burretX, Jiki.Y, 0, -5));
			JikiBurrets.Add(new Burret(BurretBitmap, burretX, Jiki.Y, 0.5, -5));
			JikiBurrets.Add(new Burret(BurretBitmap, burretX, Jiki.Y, -0.5, -5));
		}
	}
}

