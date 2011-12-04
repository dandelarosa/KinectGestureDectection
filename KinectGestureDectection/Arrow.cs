using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;

namespace KinectGestureDectection
{
    sealed class Arrow : Shape
    {
        private static readonly Geometry ArrowGeometry;

        static Arrow()
        {
            var geo = Geometry.Parse("F1 M 255.651,142.812L 290.667,142.812L 279.8,153.675C 278.644,154.832 278.644,156.708 279.8,157.866C 280.957,159.022 282.833,159.022 283.989,157.864L 299.041,142.812L 302.004,139.851L 299.041,136.888L 283.989,121.836C 283.411,121.258 282.653,120.97 281.895,120.97C 281.137,120.97 280.379,121.258 279.8,121.836C 278.644,122.994 278.644,124.868 279.8,126.024L 290.667,136.888L 255.651,136.888C 254.015,136.888 252.689,138.215 252.689,139.851C 252.689,141.486 254.015,142.812 255.651,142.812 Z ");
            geo.Freeze();
            ArrowGeometry = geo; 
        }

        public Arrow()
        {
            this.Stretch = System.Windows.Media.Stretch.Uniform;
        }

        protected override Geometry DefiningGeometry
        {
            get { return ArrowGeometry; }
        }
    }
}
