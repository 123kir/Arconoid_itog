using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Arkanoid
{
    public partial class Form1 : Form
    {
        private Ball ball;
        private Paddle paddle;
        private List<Block> blocks;
        
        private int score = 0;
        private int lives = 3; 

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            ball = new Ball { X = 200, Y = 200 };
            paddle = new Paddle { X = 150 };
            
            CreateBlocks();

            gameTimer.Tick += GameLoop;
            this.DoubleBuffered = true; 
        }

        private void InitializeGame()
        {
            ball = new Ball { X = 200, Y = 200 };
            paddle = new Paddle { X = 150 };
            gameTimer.Tick += GameLoop;
        }

        private void CreateBlocks()
        {
            blocks = new List<Block>();
            for (int i = 0; i < 5; i++) 
            {
                for (int j = 0; j < 10; j++) 
                {
                    blocks.Add(new Block { X = j * 65 + 30, Y = i * 25 + 30 }); 
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            CenterCursorOnPaddle(); 
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            CenterCursorOnPaddle(); 
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            
            paddle.X = e.X - (paddle.Width / 2);
            Invalidate(); 
        }

        private void CenterCursorOnPaddle()
        {
            int cursorX = paddle.X + (paddle.Width / 2);
            int cursorY = this.ClientSize.Height - paddle.Height / 2; 
            Cursor.Position = this.PointToScreen(new Point(cursorX, cursorY));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S)
            {
                gameTimer.Start(); 
            }
            else if (e.KeyCode == Keys.P)
            {
                gameTimer.Stop(); 
            }

            base.OnKeyDown(e);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            ball.Move();
            CheckCollision();
            CheckBlockCollision();
            Invalidate();
        }

        private void CheckCollision()
        {
            if (ball.X <= 0 || ball.X >= this.ClientSize.Width - ball.Radius * 2)
                ball.XSpeed *= -1;
            if (ball.Y <= 0)
                ball.YSpeed *= -1;

            if (ball.Y >= this.ClientSize.Height)
            {
                gameTimer.Stop(); 
                UpdatePokaLive(); 
                //return;
            }

            if (ball.Y + ball.Radius * 2 >= this.ClientSize.Height - paddle.Height &&
                ball.X + ball.Radius * 2 >= paddle.X &&
                ball.X <= paddle.X + paddle.Width)
            {
                ball.YSpeed *= -1;
            }
        }

        private void UpdatePokaLive()
        {
            lives--;

            if (lives <= 0)
            { ShowGameOverMessage(); }
            else
            { ResetBall();}
        }
        private void ShowGameOverMessage()
        {
            var result = MessageBox.Show("Вы проиграли! Хотите начать заново?", "Игра окончена",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            { ResetGame(); }
            else
            { Application.Exit(); }
        }

        private void ResetGame()
        {
            ball.X = this.ClientSize.Width / 2; 
            ball.Y = this.ClientSize.Height - 60; 
            ball.XSpeed = 5; 
            ball.YSpeed = -5; 

            score = 0;
            lives = 3;

            CreateBlocks();

            gameTimer.Start(); 
        }

        private void UpdateGame()
        {
            ball.Move();

            CheckCollision();
            CheckBlockCollision();            
        }

        private void CheckBlockCollision()
        {
            foreach (var block in blocks.ToList()) 
            {
                if (block.IsActive && ball.GetRectangle().IntersectsWith(block.GetRectangle()))
                {
                    block.HitPoints--; 
                    block.Color = Color.LightGreen; 

                    if (block.HitPoints <= 0)
                    {
                        block.IsActive = false; 
                        score++; 
                    }

                    ball.YSpeed *= -1;

                    if (AreAllBlocksDestroyed())
                    {
                        ShowVictoryMessage(); 
                    }
                }
            }
        }

        private void ShowVictoryMessage()
        {
            gameTimer.Stop();

            var result = MessageBox.Show($"Поздравляем! Вы выиграли!\nВаш счет: {score}. Хотите начать заново?", "Победа", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                ResetGame();
            }
            else
            {
                Application.Exit();
            }
        }


        private bool AreAllBlocksDestroyed()
        {
            return blocks.All(block => !block.IsActive);
        }


        private void ResetBall()
        {
            ball.X = this.ClientSize.Width / 2; 
            ball.Y = this.ClientSize.Height - 60;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillEllipse(Brushes.Red, ball.X, ball.Y, ball.Radius * 2, ball.Radius * 2);
            e.Graphics.FillRectangle(Brushes.Blue, paddle.X, this.ClientSize.Height - paddle.Height, paddle.Width, paddle.Height);
            
            foreach (var block in blocks)
            {
                if (block.IsActive)
                {
                    using (Brush brush = new SolidBrush(block.Color))
                    {
                        e.Graphics.FillRectangle(brush, block.GetRectangle());
                    }
                }
            }
 
            e.Graphics.DrawString($"Счет: {score}", new Font("Arial", 16), Brushes.Black, new Point(10, 10));
            e.Graphics.DrawString($"Жизни: {lives}", new Font("Arial", 16), Brushes.Black, new Point(150, 10));
        }
    }
}
