using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using SlimDX.Direct2D;
using SlimDX;

namespace Pong
{
    class Ball : IGameObject
    {
        // current position
        private float positionX;
        private float positionY;

        // velocity in pixels/sec
        private float velocityX;
        private float velocityY;

        public Ball(float startPosX, float startPosY, float startVelX, float startVelY)
        {
            this.positionX = startPosX;
            this.positionY = startPosY;
            this.velocityX = startVelX;
            this.velocityY = startVelY;
        }

        public float GetPositionX()
        {
            return positionX;
        }

        public float GetPositionY()
        {
            return positionY;
        }

        public float GetVelocityX()
        {
            return velocityX;
        }

        public float GetVelocityY()
        {
            return velocityY;
        }


        public void SetVelocity(float velX, float velY)
        {
            this.velocityX = velX;
            this.velocityY = velY;
        }

        public void Update(float deltaTime)
        {
            this.positionX = (velocityX * deltaTime) + positionX;
            this.positionY = (velocityY * deltaTime) + positionY;
        }

        public void Render(Factory factory, RenderTarget renderTarget)
        {
            SolidColorBrush brush = new SolidColorBrush(renderTarget, new Color4(1.0f, 1.0f, 1.0f));
            Ellipse ellipse = new Ellipse
            {
                Center = new PointF { X = positionX, Y = positionY },
                RadiusX = 5,
                RadiusY = 5
            };

            renderTarget.BeginDraw();
            renderTarget.Transform = Matrix3x2.Identity;
            renderTarget.Clear(new Color4(0.0f, 0.0f, 0.0f));
            renderTarget.FillEllipse(brush, ellipse);
            renderTarget.EndDraw();
        }

        public void Render(System.Drawing.Graphics g2d)
        {
            // draw ball
            System.Drawing.Brush whiteBrush = new SolidBrush(Color.White);
            g2d.FillEllipse(whiteBrush, new Rectangle((int) Math.Round(positionX), (int) Math.Round(positionY), 10, 10));   
        }
    }

}
