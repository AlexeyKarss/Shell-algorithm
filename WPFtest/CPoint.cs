using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFtest {
	class CPoint {
		public int Num { //Номер точки
			get; set;
		}
		public double X { //Центральная координата Х
			get; set; }

		public double Y { //Центральная координата Y
			get; set; }

		public double DrawX => X-Size/2; //самая верхняя координата X точки - нужна для правильного рисования эллипса
		public double DrawY => Y-Size/2; //самая верхняя координата Y точки - нужна для правильного рисования эллипса

		public double Size { get; } = 20; //Размер точки

		private Color color = Colors.DeepSkyBlue; //Цвет
		public Ellipse Elp {
			get;
		} //Эллипс точки - рисуется на канвасе
		public Label Lbl {
			get;
		} //Лэйбл точки - 


		Ellipse CreateEllipse() { //Назначаем эллипс
			var elp = new Ellipse {  //Создаем эллипс	
				Fill = new SolidColorBrush(color), //Красим
				Width = Size, //Устанавливаем размеры
				Height = Size //Одинаковые = круг
			};	
			elp.SetValue(Canvas.LeftProperty, DrawX); //Координата Х на канвасе
			elp.SetValue(Canvas.TopProperty, DrawY);  //Координата У на канвасе
			return elp;
		}
		Label CreateLabel() {	//Создаем лэйбл по аналогии с эллипсом
			Label l=new Label();
			l.Content=Num;
			l.SetValue(Canvas.LeftProperty, X);
			l.SetValue(Canvas.TopProperty, Y);
			return l;
		}

		public bool isHitted(double x,double y) { //Принадлежит ли точка (x,y) нашей точке
			if(x>=DrawX && x<=DrawX+Size && //Если х внутри точки
				y>=DrawY && y<=DrawY+Size)  //и у тоже внутри
				return true; //значит принадлежит
			return false; //иначе - нет
		}

		public CPoint(int num, double x,double y) { //Конструктор
			Num=num;
			X=x; //Назначаем координаты
			Y=y;
			Elp=CreateEllipse(); //Создаем эллипс
			Lbl=CreateLabel();
		}
	}

}
