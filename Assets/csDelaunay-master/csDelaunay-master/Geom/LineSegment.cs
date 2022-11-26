using System.Collections;
using System.Collections.Generic;

namespace csDelaunay {

	public class LineSegment {

		public static List<LineSegment> VisibleLineSegments(List<Edge> edges) {
			List<LineSegment> segments = new List<LineSegment>();

			foreach (Edge edge in edges) {
				if (edge.Visible()) {
					Vector2f p1 = edge.ClippedEnds[LR.LEFT];
					Vector2f p2 = edge.ClippedEnds[LR.RIGHT];
					segments.Add(new LineSegment(p1,p2));
				}
			}

			return segments;
		}

		public static float CompareLengths_MAX(LineSegment segment0, LineSegment segment1) {
			float length0 = (segment0.p0 - segment0.p1).magnitude;
			float length1 = (segment1.p0 - segment1.p1).magnitude;
			if (length0 < length1) {
				return 1;
			}
			if (length0 > length1) {
				return -1;
			}
			return 0;
		}

		public static float CompareLengths(LineSegment edge0, LineSegment edge1) {
			return - CompareLengths_MAX(edge0, edge1);
		}

		public Vector2f p0;
		public Vector2f p1;

		public LineSegment (Vector2f p0, Vector2f p1) {
			this.p0 = p0;
			this.p1 = p1;
		}

		public static bool operator == (LineSegment l1, LineSegment l2)
        {
			return l1.Equals(l2);
        }

		public static bool operator !=(LineSegment l1, LineSegment l2)
		{
			return !l1.Equals(l2);
		}

        public override bool Equals(object obj)
        {
			LineSegment ls = (LineSegment)obj;
			return ls.p0.Equals(this.p0) && ls.p1.Equals(this.p1) 
				|| ls.p1.Equals(this.p0) && ls.p0.Equals(this.p1);
		}

        public override int GetHashCode()
        {
			return p0.GetHashCode() + p1.GetHashCode();
        }
    }
}