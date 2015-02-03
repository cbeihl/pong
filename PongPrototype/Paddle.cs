using SlimDX;
using SlimDX.Direct2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Pong
{
    class Paddle : IGameObject
    {
        private readonly SizeF paddleSize = new SizeF(10, 100);
        private PointF velocity = new PointF(0, 0);

        // current position
        private RectangleF rect;

        public Paddle(PointF startPos)
        {
            this.rect = new RectangleF(startPos, paddleSize);
        }

        public PointF GetPosition()
        {
            return rect.Location;
        }

        public void SetPosition(PointF newPos)
        {
            rect = new RectangleF(new PointF(newPos.X, newPos.Y), paddleSize);
        }

        public RectangleF GetBoundingBox()
        {
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
            // ignore x component
            PointF newLoc = new PointF(rect.Location.X, (velocity.Y * deltaTime) + rect.Location.Y);
            rect = new RectangleF(newLoc, rect.Size);
        }
        public void Render(Factory factory, RenderTarget renderTarget)
        {
            SolidColorBrush brush = new SolidColorBrush(renderTarget, new Color4(1.0f, 1.0f, 1.0f));
            renderTarget.FillRectangle(brush, rect);
        }
    }
}
