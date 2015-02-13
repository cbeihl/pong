using SlimDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pong
{
    class FontEnumerator : FontFileEnumerator
    {
        private Factory factory;

        public override FontFile Current
        {
            get { 
                return null; 
            }
            set {}
        }

        public override bool MoveNext()
        {
            return false;
        }
        public override void Reset()
        {
        }

    }
}
