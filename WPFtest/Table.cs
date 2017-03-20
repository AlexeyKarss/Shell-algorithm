using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFtest {
	class Table {
		private CPoint _point;
		public int X {
			get; set;
		}
		public int Y {
			get; set;
		}
		public CPoint GetPoint() {
			return _point;
		}
		public Table(CPoint p, int x,int y) {
			_point=p;
			X=x;
			Y=y;
		}
		public Table(CPoint p,double x,double y) {
			_point=p;
			X=(int)x;
			Y=(int)y;
		}
	}
}
