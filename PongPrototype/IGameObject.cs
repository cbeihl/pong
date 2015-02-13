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
        void Update(float deltaTime);
        void Render(Factory factory, SlimDX.DirectWrite.Factory dwFactory, RenderTarget renderTarget);
    }
}
