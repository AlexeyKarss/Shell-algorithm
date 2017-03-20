using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFtest {
	class Algoritms {
		public static double rotate(CPoint A, CPoint B, CPoint C) {
			//та же функция, только принимает точки как аргументы 
			//return rotate(A.X,A.Y,B.X,B.Y,C.X,C.Y);
			return (B.X-A.X)*(C.Y-B.Y) - (B.Y-A.Y)*(C.X-B.X);
		}

		public static List<CPoint> GrahamScan(List<CPoint> points, TextBox tb) {
			if (points==null)
				return null;
			Stopwatch stopWatch=new Stopwatch();
			stopWatch.Start();
			//Ищем начальную точку - самую левую - гарантированно входит в Минимальную Выпуклую Оболочку
			var P = new List<int>(points.Count);  //Для того, чтобы не изменять исходный массив точек, будем оперировать с их номерами
			for (int i = 0;i<points.Count;++i)  //нумеруем точки
				P.Add(i);
			for (int i = 1;i<points.Count;++i)	//Находим самую левую точку
				if (points[P[i]].X < points[P[0]].X) {  
					int tmp = P[i];
					P[i]=P[0];
					P[0]=tmp;
				}
			tb.Text+="Найдена самая левая точка: " + points[P[0]].Num +"\n";
			//P[0] - самая левая точка. Не трогаем ее, сортируем остальные точки по степени их "левизны"

			for (int i = 2;i<P.Count;++i) {  //сортировка вставками
				int j = i;
				while ((j>1) && (rotate(points[P[0]],points[P[j-1]],points[P[j]]) <0)) { //Если точка j левее точки j-1
					int tmp = P[j-1];												 //меняем их местами
					P[j-1]=P[j];
					P[j]=tmp;
					--j;
				}
			}
			tb.Text+="Точки отсортированы:\n ";
			for (int i = 0;i<P.Count;++i)
				tb.Text+=points[P[i]].Num+" ";
			tb.Text+="\n";

			//Строим оболочку
			var shell = new List<int> {P[0], P[1]}; //список точек, входящих в оболочку (будем использовать его как стэк)
													// 0 точка - самая левая, включаем её
													// 1 точка - самая левая для 0, включаем ёё тоже
			tb.Text+="Добавлены точки: "+points[P[0]].Num+", "+points[P[1]].Num+"\n";

			for (int i = 2;i<P.Count;++i) {  //Просматриваем остальные точки и строим между ними линии так, чтобы не было правых поворотов, тогда оболочка будет выпуклой, т.к. мы просматриваем все точки в порядке против часовой стрелки
				while (shell.Count>=2  &&		
						rotate(points[shell[shell.Count-2]],points[shell[shell.Count-1]],points[P[i]])<0) { //Если прямая к следующей точке будет правым поворотом,
					tb.Text+="Удалена точка: "+points[shell[shell.Count-1]].Num+"\n";
					shell.RemoveAt(shell.Count-1);                                                          //то удаляем предыдущие точки, пока условие не нарушится
					
				}
				shell.Add(P[i]); //Добавляем новую точку
				tb.Text+="Добавлена точка: "+points[P[i]].Num+"\n";
			}

			stopWatch.Stop();
			TimeSpan ts = stopWatch.Elapsed;
			string elapsedTime = string.Format("{0}.{1:0000}",ts.Seconds,
			ts.Milliseconds);

			//Составляем результирующий список
			var resShell = new List<CPoint>();
			for(int i=shell.Count-1; i>=0; --i)  //перекидываем все из "стэка" в список
				resShell.Add(points[shell[i]]);
			tb.Text+="Алгоритм завершен.\nВремя выполнения: "+elapsedTime+ " сек.";
			return resShell; //Возвращаем его
		}

		public static List<CPoint> JarvisMarch(List<CPoint> points,TextBox tb) {
			if (points==null)
				return null;
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			//Ищем начальную точку - самую левую - гарантированно входит в Минимальную Выпуклую Оболочку
			var P = new List<int>(points.Count);  //Для того, чтобы не изменять исходный массив точек, будем оперировать с их номерами
			for (int i = 0;i<points.Count;++i)	  //нумеруем точки
				P.Add(i);
			for (int i = 1;i<points.Count;++i)  //Находим самую левую точку
				if (points[P[i]].X < points[P[0]].X) {
					int tmp = P[i];
					P[i]=P[0];
					P[0]=tmp;
				}
			tb.Text+="Найдена и добавлена самая левая точка: " + points[P[0]].Num +"\n";
			//P[0] - самая левая точка

			var H = new List<int>() { P[0] }; //Здесь мы будем хранить точки, входящие в оболочку. Сразу заносим стартовую точку
			P.RemoveAt(0); //Удаляем стартовую точку и заносим ее в конец списка
			P.Add(H[0]); //Если мы встретим стартовую точку, значит алгоритм закончил работу
			tb.Text+="Стартовая точка занесена в конец списка просмотра.\n";

			while (true) { //бесконечный цикл
				var right = 0;
				tb.Text+="Поиск самой левой точки.\n";
				for (int i = 1;i<P.Count;++i) //на каждой итерации ищем самую левую точку P относительно последней точки в H
					if (rotate(points[H[H.Count-1]],points[P[right]],points[P[i]])<0)
						right=i;
				if (P[right]==H[0]) { //если эта точка  стартовая, то прерываем цикл
					tb.Text+="Достигнута начальная точка.\n";
					break;
				}
				tb.Text+="Добавлена точка: "+points[P[right]].Num+"\n";
				H.Add(P[right]); //иначе переносим найденную точку из P в H
				P.RemoveAt(right);
			}

			stopWatch.Stop();
			TimeSpan ts = stopWatch.Elapsed;
			string elapsedTime = string.Format("{0}.{1:0000}",ts.Seconds,
			ts.Milliseconds);

			var Result = new List<CPoint>(); //Составляем результирующий список
			foreach (var p in H) {
				Result.Add(points[p]);
			}

			tb.Text+="Конец алгоритма.\nВремя выполнения: "+elapsedTime+" сек.";
			return Result; //И возвращаем его
		}

	}
}