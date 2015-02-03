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
        private readonly float radius = 5;

        // current position
        private PointF position;

        // velocity in pixels/sec
        private PointF velocity;

        public Ball(PointF startPos, PointF startVel)
        {
            this.position = startPos;
            this.velocity = startVel;
        }

        public PointF GetPosition()
        {
            return position;
        }

        public RectangleF GetBoundingBox()
        {
            float diameter = radius * 2;
            RectangleF rect = new RectangleF(new PointF(position.X - radius, position.Y - radius), new SizeF(diameter, diameter));
            return rect;
        }

        public PointF GetVelocity()
        {
            return velocity;
        }

        public void SetVelocity(PointF newVel)
        {
            this.velocity = newVel;
        }

        public void Update(float deltaTime)
        {
            position.X = (velocity.X * deltaTime) + position.X;
            position.Y = (velocity.Y * deltaTime) + position.Y;
        }

        public void Render(Factory factory, RenderTarget renderTarget)
        {
            SolidColorBrush brush = new SolidColorBrush(renderTarget, new Color4(1.0f, 1.0f, 1.0f));
            Ellipse ellipse = new Ellipse
            {
                Center = new PointF { X = position.X, Y = position.Y },
                RadiusX = this.radius,
                RadiusY = this.radius
            };

            renderTarget.BeginDraw();
            renderTarget.Transform = Matrix3x2.Identity;
            renderTarget.Clear(new Color4(0.0f, 0.0f, 0.0f));
            renderTarget.FillEllipse(brush, ellipse);
            renderTarget.EndDraw();
        }

    }

}
