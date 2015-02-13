using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;
using SlimDX.Direct2D;
using SlimDX;
using System.Threading;
using System.Drawing.Text;

namespace Pong
{

    public partial class MainForm : Form
    {
        private Stopwatch gameTimer;
        private List<IGameObject> gameObjects = new List<IGameObject>();
        private double lastGameTime = 0.0;

        private Factory factory;
        private SlimDX.DirectWrite.Factory dwFactory;
        private WindowRenderTarget renderTarget;
        private bool firstPaint = true;

        private Paddle leftPaddle, rightPaddle;
        private Ball ball;
        private Score leftScore, rightScore;

        private SoundManager soundMgr;


        private void MainForm_Load(object sender, EventArgs e) // new splash/titlescreen
        {
            TitleScreen Splash = new TitleScreen();
            Splash.Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // TODO : dispose of all COM objects
        }


        public MainForm()
        {
            //SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            InitializeComponent();
            gameTimer = new Stopwatch();
            gameTimer.Start();
            InitializeGameObjects();
            Application.Idle += ApplicationIdle;

            soundMgr = new SoundManager(this.Handle);
            soundMgr.PlayMusicLoop();
        }

        private void InitializeDirect2D()
        {
            Size drawingPanelSize = this.ClientSize;
            Debug.WriteLine("Width " + drawingPanelSize.Width + ", Height " + drawingPanelSize.Height + ", HWND " + this.Handle);

            factory = new Factory();
            dwFactory = new SlimDX.DirectWrite.Factory();
            SlimDX.DirectWrite.FontCollection installedFonts = dwFactory.GetSystemFontCollection(false);
            
            renderTarget = new WindowRenderTarget(factory, new WindowRenderTargetProperties
            {
                Handle = this.Handle,
                PixelSize = new Size(drawingPanelSize.Width, drawingPanelSize.Height)
            });

        }


        private void InitializeGameObjects()
        {
            leftPaddle = new Paddle(new PointF(10, 10));
            rightPaddle = new Paddle(new PointF(400, 10));
            ball = new Ball(new PointF(0, 0), new PointF(400, 200));
            leftScore = new Score(new PointF(-80, 20));
            rightScore = new Score(new PointF(60, 20));
            gameObjects.Add(leftPaddle);
            gameObjects.Add(rightPaddle);
            gameObjects.Add(ball);
            gameObjects.Add(leftScore);
            gameObjects.Add(rightScore);
        }

        private void ApplicationIdle(object sender, EventArgs e)
        {
            while (IsApplicationIdle())
            {
                // TODO : process input

                // update game objects
                double gameTime = (double) gameTimer.ElapsedTicks / Stopwatch.Frequency;
                float deltaTime = (float) (gameTime - lastGameTime);
                if (deltaTime >= 0.008)
                {
                    lastGameTime = gameTime;
                    foreach (IGameObject gameObj in gameObjects)
                    {
                        gameObj.Update(deltaTime);
                    }

                    moveAIPaddle();
                    checkBallCollisions();

                    if (!firstPaint)
                    {
                        renderTarget.BeginDraw();
                        renderTarget.Transform = Matrix3x2.Identity;
                        renderTarget.Clear(new Color4(0.0f, 0.0f, 0.0f));
                        foreach (IGameObject gameObj in gameObjects)
                        {
                            gameObj.Render(factory, dwFactory, renderTarget);
                        }
                        renderTarget.EndDraw();
                    }
                    // Debug.WriteLine("deltaTime = " + deltaTime);
                }
            }
        }

        private void checkBallCollisions()
        {
            SizeF windowSize = renderTarget.Size;
            float windowWidth = windowSize.Width;
            float windowHeight = windowSize.Height;
            PointF ballPos = ball.GetPosition();
            PointF ballVel = ball.GetVelocity();
            RectangleF ballBounds = ball.GetBoundingBox();

            // check collision with left and right screen bounds
            if (ballBounds.Right < 0 && ballVel.X < 0)
            {
                rightScore.IncScore();
                ball.SetPosition(new PointF(windowHeight / 2, windowWidth / 2));
            } 
            else if (ballBounds.Left > windowWidth && ballVel.X > 0)
            {
                leftScore.IncScore();
                ball.SetPosition(new PointF(windowHeight / 2, windowWidth / 2));
            }

            // check collision with top and bottom screen bounds
            if ((ballBounds.Top < 0 && ballVel.Y < 0) || (ballBounds.Bottom > windowHeight && ballVel.Y > 0))
            {
                soundMgr.PlayWallHit();
                ball.SetVelocity(new PointF(ballVel.X, ballVel.Y * -1));
            }


            RectangleF leftPaddleBounds = leftPaddle.GetBoundingBox();
            RectangleF rightPaddleBounds = rightPaddle.GetBoundingBox();

            if ((ballBounds.IntersectsWith(leftPaddleBounds) && ball.GetVelocity().X < 0) || (ballBounds.IntersectsWith(rightPaddleBounds) && ball.GetVelocity().X > 0))
            {
                soundMgr.PlayPaddleHit();
                ball.SetVelocity(new PointF(ballVel.X * -1, ballVel.Y));
            }
        }

        private void moveAIPaddle()
        {
            rightPaddle.SetPosition(new PointF(renderTarget.Size.Width - 20, ball.GetPosition().Y - 50));
        }

        private void handleKeyDown(object sender, KeyEventArgs e) 
        {
            if (e.KeyCode == Keys.Up)
            {
                leftPaddle.SetVelocity(new PointF(0, -200));
            } 
            else if (e.KeyCode == Keys.Down)
            {
                leftPaddle.SetVelocity(new PointF(0, 200));
            }

        }

        private void handleKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                leftPaddle.SetVelocity(new PointF(0, 0));
            }
        }

        private void handleResize(object sender, EventArgs e)
        {
            renderTarget.Resize(this.ClientSize);
        }

        private bool IsApplicationIdle()
        {
            NativeMessage result;
            return PeekMessage(out result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr Handle;
            public uint Message;
            public IntPtr WParameter;
            public IntPtr LParameter;
            public uint Time;
            public Point Location;
        }
        [DllImport("user32.dll")]
        public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);

        private void PaintDrawingPanel(object sender, PaintEventArgs e)
        {
            if (firstPaint)
            {
                InitializeDirect2D();
                firstPaint = false;
            }
        }

        private void RenderDirect2D()
        {
            PathGeometry triangle = new PathGeometry(factory);
            using (GeometrySink sink = triangle.Open())
            {
                PointF p0 = new PointF(0.50f * 200, 0.25f * 200);
                PointF p1 = new PointF(0.75f * 200, 0.75f * 200);
                PointF p2 = new PointF(0.25f * 200, 0.75f * 200);

                sink.BeginFigure(p0, FigureBegin.Filled);
                sink.AddLine(p1);
                sink.AddLine(p2);
                sink.EndFigure(FigureEnd.Closed);

                // Note that Close() and Dispose() are not equivalent like they are for
                // some other IDisposable() objects.
                sink.Close();
            }

            SolidColorBrush brush = new SolidColorBrush(renderTarget, new Color4(1.0f, 1.0f, 1.0f));

            Ellipse ellipse = new Ellipse
            {
                Center = new PointF { X = 10.0f, Y = 10.0f },
                RadiusX = 5,
                RadiusY = 5
            };

            renderTarget.BeginDraw();
            renderTarget.Transform = Matrix3x2.Identity;
            renderTarget.Clear(new Color4(0.0f, 0.0f, 0.0f));
            renderTarget.FillEllipse(brush, ellipse);
            renderTarget.EndDraw();
        }


    }
}
