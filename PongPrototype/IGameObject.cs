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
        void Render(Factory factory, RenderTarget renderTarget);
        float GetPositionX();
        float GetPositionY();
        float GetVelocityX();
        float GetVelocityY();
        void SetVelocity(float x, float y);
    }
}
