using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SlimDX.Direct2D;

namespace Pong
{
    interface IGameObject
    {
        PointF GetPosition();
        RectangleF GetBoundingBox();
        PointF GetVelocity();
        void SetVelocity(PointF newVel);
        void Update(float deltaTime);
        void Render(Factory factory, RenderTarget renderTarget);
    }
}
