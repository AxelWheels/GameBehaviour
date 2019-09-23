using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AC_Physics
{
    public class Polygon_Collider
    {
        private Vec2[] vertices;
        
        public Vec2[] Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }
    }
}
