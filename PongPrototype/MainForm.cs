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
using SlimDX.XAudio2; // new
using SlimDX.DirectSound; // new
using SlimDX.Multimedia; // new
using SlimDX.Windows; // new
using System.Threading;

namespace Pong
{

    public partial class MainForm : Form
    {
        private Stopwatch gameTimer;
        private List<IGameObject> gameObjects = new List<IGameObject>();
        private double lastGameTime = 0.0;

        private Factory factory;
        private RenderTarget renderTarget;
        private bool firstPaint = true;

        private Paddle leftPaddle, rightPaddle;
        private Ball ball;
                     
        bool m_UseDirectSound = true; // new
        DirectSound m_DirectSound; // new
        PrimarySoundBuffer m_DSoundPrimaryBuffer; // new
        SecondarySoundBuffer m_DSoundBuffer; // new


        private void MainForm_Load(object sender, EventArgs e) // new splash/titlescreen
        {
            TitleScreen Splash = new TitleScreen();
            Splash.Show();         
        }


        public MainForm()
        {
            //SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            InitializeComponent();
            gameTimer = new Stopwatch();
            gameTimer.Start();
            InitializeGameObjects();
            Application.Idle += ApplicationIdle;
            if (m_UseDirectSound) // new
            InitDirectSound();
        }

        private void InitializeDirect2D()
        {
            Size drawingPanelSize = this.ClientSize;
            Debug.WriteLine("Width " + drawingPanelSize.Width + ", Height " + drawingPanelSize.Height + ", HWND " + this.Handle);

            factory = new Factory();
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
            gameObjects.Add(leftPaddle);
            gameObjects.Add(rightPaddle);
            gameObjects.Add(ball);
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
                            gameObj.Render(factory, renderTarget);
                        }
                        renderTarget.EndDraw();
                    }
                    Debug.WriteLine("deltaTime = " + deltaTime);
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
            if ((ballBounds.Left < 0 && ballVel.X < 0) || (ballBounds.Right > windowWidth && ballVel.X > 0))
            {
                ball.SetVelocity(new PointF(ballVel.X * -1, ballVel.Y));
            }

            // check collision with top and bottom screen bounds
            if ((ballBounds.Top < 0 && ballVel.Y < 0) || (ballBounds.Bottom > windowHeight && ballVel.Y > 0))
            {
                ball.SetVelocity(new PointF(ballVel.X, ballVel.Y * -1));
            }


            RectangleF leftPaddleBounds = leftPaddle.GetBoundingBox();
            RectangleF rightPaddleBounds = rightPaddle.GetBoundingBox();

            if (ballBounds.IntersectsWith(leftPaddleBounds) || ballBounds.IntersectsWith(rightPaddleBounds))
            {
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
            
            //Graphics g2d = e.Graphics;
            // bufferedGraphics.Render(g2d);
            // g2d.Clear(Color.Black);

            /*
            Graphics g2d = e.Graphics;
            foreach (IGameObject gameObj in gameObjects)
            {
                gameObj.Render(g2d);
            }
            */


            
            /*
            Brush whiteBrush = new SolidBrush(Color.White);

            // draw left paddle
            g2d.FillRectangle(whiteBrush, new Rectangle(5, 5, 10, 100));

            // draw right paddle
            g2d.FillRectangle(whiteBrush, new Rectangle(400, 280, 10, 100));

            // draw ball
            g2d.FillEllipse(whiteBrush, new Rectangle(200, 140, 10, 10));
            */
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
            // renderTarget.FillGeometry(triangle, brush);
            renderTarget.FillEllipse(brush, ellipse);
            renderTarget.EndDraw();
        }

        private void InitDirectSound()
        {
            // create DirectSound object.
            m_DirectSound = new DirectSound ();

            // set cooperative level.
            m_DirectSound.SetCooperativeLevel (this.Handle, SlimDX.DirectSound.CooperativeLevel.Priority);
            
            // create the primary sound buffer.
            SoundBufferDescription desc = new SoundBufferDescription ();
            desc.Flags = SlimDX.DirectSound.BufferFlags.PrimaryBuffer;
            m_DSoundPrimaryBuffer = new PrimarySoundBuffer (m_DirectSound, desc);

            // create secondary sound buffer.
            using (WaveStream wavFile = new WaveStream(Application.StartupPath + "\\" + "hustlepong.wav"))
            {
                SoundBufferDescription DSoundBufferDesc;
                DSoundBufferDesc = new SoundBufferDescription();
                DSoundBufferDesc.SizeInBytes = (int) wavFile.Length;
                DSoundBufferDesc.Flags = SlimDX.DirectSound.BufferFlags.ControlVolume;
                DSoundBufferDesc.Format = wavFile.Format;

                m_DSoundBuffer = new SecondarySoundBuffer (m_DirectSound, DSoundBufferDesc);

                // now load the sound.
                byte [] wavData = new byte[DSoundBufferDesc.SizeInBytes];
                wavFile.Read(wavData, 0, (int)wavFile.Length);
                m_DSoundBuffer.Write(wavData, 0, LockFlags.None);

                // play our music and have it loop continuously.
                m_DSoundBuffer.Play(0, SlimDX.DirectSound.PlayFlags.Looping); 
            }

               
        }

       
        


            
        

            



    }
}
