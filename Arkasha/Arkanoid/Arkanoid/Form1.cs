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
        private ClassClassPaddle ClassClassPaddle;
        private List<Block> blocks; 

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            ball = new Ball { X = 200, Y = 200 };
            ClassClassPaddle = new ClassClassPaddle { X = 150 };

            // Создаем блоки
            CreateBlocks();

            gameTimer.Tick += GameLoop;
            this.DoubleBuffered = true; 
        }

        private void InitializeGame()
        {
            ball = new Ball { X = 200, Y = 200 };
            ClassClassPaddle = new ClassClassPaddle { X = 150 };
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
            CenterCursorOnClassClassPaddle(); 
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            CenterCursorOnClassClassPaddle(); 
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            
            ClassClassPaddle.X = e.X - (ClassClassPaddle.Width / 2);
            Invalidate(); 
        }

        private void CenterCursorOnClassClassPaddle()
        {
            int cursorX = ClassClassPaddle.X + (ClassClassPaddle.Width / 2);
            int cursorY = this.ClientSize.Height - ClassClassPaddle.Height / 2; 
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
            // Проверка границ
            if (ball.X <= 0 || ball.X >= this.ClientSize.Width - ball.Radius * 2)

                ball.XSpeed *= -1;
            if (ball.Y <= 0)
                ball.YSpeed *= -1;
            if (ball.Y >= this.ClientSize.Height)
                ResetBall();

            // Проверка столкновения с ракеткой
            if (ball.Y + ball.Radius * 2 >= this.ClientSize.Height - ClassClassPaddle.Height &&
                ball.X + ball.Radius * 2 >= ClassClassPaddle.X &&
                ball.X <= ClassClassPaddle.X + ClassClassPaddle.Width)
            {
                ball.YSpeed *= -1;
            }
        }

        private void CheckBlockCollision()
        {
            foreach (var block in blocks.ToList()) // Используем ToList для изменения коллекции во время итерации
            {
                if (block.IsActive && ball.GetRectangle().IntersectsWith(block.GetRectangle()))
                {
                    ball.YSpeed *= -1;
                    block.IsActive = false; // Деактивируем блок при столкновении
                }
            }
        }

        private void ResetBall()
        {
            ball.X = this.ClientSize.Width / 2;
            ball.Y = this.ClientSize.Height / 2;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillEllipse(Brushes.Red, ball.X, ball.Y, ball.Radius * 2, ball.Radius * 2);
            e.Graphics.FillRectangle(Brushes.Blue, ClassClassPaddle.X, this.ClientSize.Height - ClassClassPaddle.Height, ClassClassPaddle.Width, ClassClassPaddle.Height);

            // Рисуем блоки
            foreach (var block in blocks)
            {
                if (block.IsActive)
                {
                    e.Graphics.FillRectangle(Brushes.Green, block.GetRectangle());
                }
            }
        }
    }
}
