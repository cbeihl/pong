using SlimDX;
using SlimDX.Direct2D;
using SlimDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Pong
{
    class Score : IGameObject
    {
        private readonly SizeF textLayoutSize = new SizeF(100, 70);
        private RectangleF rect;
        private PointF posOffset;
        private SizeF windowSize;

        private SolidColorBrush brush;
        private TextFormat txtFormat;

        private int val = 0;

        public Score(PointF posOffset)
        {
            this.posOffset = posOffset;
        }

        public void IncScore()
        {
            val++;
        }

        public void Update(float deltaTime)
        {
        }
        public void Render(SlimDX.Direct2D.Factory factory, SlimDX.DirectWrite.Factory dwFactory, RenderTarget renderTarget)
        {
            if (this.brush == null)
            {
                brush = new SolidColorBrush(renderTarget, new Color4(1.0f, 1.0f, 1.0f));
                txtFormat = dwFactory.CreateTextFormat("lcd phone", FontWeight.Regular, SlimDX.DirectWrite.FontStyle.Normal, FontStretch.Normal, 60, "en-us");
            }

            SizeF newWindowSize = renderTarget.Size;
            if (windowSize == null || !windowSize.Equals(newWindowSize))
            {
                windowSize = newWindowSize;
                float centerX = windowSize.Width / 2;
                rect = new RectangleF(new PointF(centerX + posOffset.X, posOffset.Y), textLayoutSize);
            }

            renderTarget.DrawText(val.ToString(), txtFormat, rect, brush);
        }
    }
}
