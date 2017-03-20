using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPFtest {
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow:Window {
		public MainWindow() {
			InitializeComponent();
			comboBox.SelectedIndex=0;
		}

		List<CPoint> Points = new List<CPoint>();   //Список всех точек
		List<CPoint> Hull;							//Список точек, составляющих оболочку
		List<Line> HullLines;						//Список линий оболочки
		int Counter = 1;                            //Нумерация точек

		private void AddPoint(CPoint p) {           //Добавляем точку
			Points.Add(p);						//Добалвяем точку в список
			Canvas1.Children.Add(p.Elp);		//Добавляем эллипс на канвас
			Canvas1.Children.Add(p.Lbl);        //Добавляем подпись на канвас
		}
		private void RemovePoint(CPoint p) {		//Удаляем точку
			Canvas1.Children.Remove(p.Elp);		//Удаляем эллипс с канваса
			Canvas1.Children.Remove(p.Lbl);     //Удаляем подпись с канваса
			Points.Remove(p);					//Удаляем точку из списка
		}

		private void RemoveHull() {					//Удаляем оболочку
			if (Hull==null || HullLines==null)			//Если ее и так нет, ничего не делаем
				return;
			foreach(var l in HullLines)					//Удаляем все линии с канваса
				Canvas1.Children.Remove(l);		
			Hull.RemoveRange(0, Hull.Count);			//Удаляем все точки из оболочки
			HullLines.RemoveRange(0,HullLines.Count);	//Удаляем все линии оболочки
		}

		private void Canvas1_MouseDown(object sender,MouseButtonEventArgs e) {
			double x = e.GetPosition(Canvas1).X;								//GetPosition(Canvas1).X - координата Х относительно Canvas 
			double y = e.GetPosition(Canvas1).Y;								//координата Y относительно канваса
			if (e.LeftButton == MouseButtonState.Pressed) {						//Если нажата ЛКМ
				var point = new CPoint(Counter++, x,y);										//Создаем новую точку.
				AddPoint(point);												//Добавляем точку
			}
			else if(e.RightButton == MouseButtonState.Pressed){					//Если нажата ПКМ
				foreach (var p in Points) {
					if (p.isHitted(x,y)) {
						RemovePoint(p);
						break;
					}
				}
			}
		}

		private void button1_Click(object sender,RoutedEventArgs e) {
			textBox.Text="";
			if (Points.Count<=0) {												//Проверочные ограничения
				messageLabel.Foreground=new SolidColorBrush(Colors.Red);
				messageLabel.Content="Нет точек\n для построения оболочки!";
				return;
			}
			if (Points.Count<=1) {
				messageLabel.Foreground=new SolidColorBrush(Colors.Red);
				messageLabel.Content="Недостаточно точек\n для построения оболочки!";
				return;
			}
			messageLabel.Foreground=new SolidColorBrush(Colors.Black);
			messageLabel.Content="Оболочка построена\n алгоритмом ";
			RemoveHull();                          //Удаляем старую оболочку
			Color color;							//Цвет оболочки
			if (comboBox.SelectedIndex==0) {        //Выбор алгоритма
				Hull = Algoritms.GrahamScan(Points, textBox);
				color=Colors.Tomato;
				messageLabel.Content+="Грэхема.";
			} else if (comboBox.SelectedIndex==1) {
				Hull=Algoritms.JarvisMarch(Points, textBox);
				color=Colors.Gold;
				messageLabel.Content+="Джарвиса.";
			} else
				return;

			if (Hull==null)						//Если небыло точек - ничего не делаем
				return;

			HullLines=new List<Line>();			//рисуем линии и добавляем их в список линий
			for (int i = 1;i<Hull.Count;++i) {				//проходимся по списку точек оболочки
				var line = new Line() {						//создаем новую линию
					X1=Hull[i-1].X,							//координаты начала
					Y1=Hull[i-1].Y,
					X2=Hull[i].X,							//конца
					Y2=Hull[i].Y,
					Stroke=new SolidColorBrush(color), //Цвет линии
					StrokeThickness = 3,					   //Толщина
				};
				HullLines.Add(line);						//Добавляем линию в список	
				Canvas1.Children.Add(line);					//и на канвас

			}
			var lastLine = new Line() {			//Рисуем линию, соединяющую первую и последнюю точки
				X1=Hull[0].X,
				Y1=Hull[0].Y,
				X2=Hull[Hull.Count-1].X,
				Y2=Hull[Hull.Count-1].Y,
				Stroke=new SolidColorBrush(color),
				StrokeThickness = 3,
			};
			HullLines.Add(lastLine);
			Canvas1.Children.Add(lastLine);
		}

		private void button2_Click(object sender,RoutedEventArgs e) { //Удаление всех точек
			if (Points==null)		//Если точек нет, ничего не делаем
				return;
			if (Points.Count<=0)
				return;
			RemoveHull();			//Удаляем оболочку
			foreach (var p in Points) {			//Удаляем с канваса все
				Canvas1.Children.Remove(p.Elp);	//эллипсы
				Canvas1.Children.Remove(p.Lbl);	//лэйблы
			}
			Counter=0;
			Points.RemoveRange(0,Points.Count);	//Удаляем все точки из списка
			messageLabel.Foreground=new SolidColorBrush(Colors.Black);
			messageLabel.Content="Все точки\n удалены.";
		}

		private void ranGenBtn_Click(object sender,RoutedEventArgs e) { //Генерация случайных точек
			button2_Click(sender, e); //Удаляем все точки
			Random rnd = new Random();
			for (int i = 0;i<30;++i) { //генерируем 30 штук
				CPoint p=new CPoint(Counter++, rnd.Next(10, (int)Canvas1.Width-10), rnd.Next(10, (int)Canvas1.Height-10)); //Создаем точку со случайными координатами
				AddPoint(p);			//Добавляем её
			}
			messageLabel.Content="Случайные точки\n сгенерированы.";
		}
	}
}
