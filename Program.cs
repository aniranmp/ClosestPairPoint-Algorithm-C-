using System;

public class FindClosestPairOfPoints
{

	class Point
	{
		public double x;
		public double y;
		public Point(double x, double y)
		{
			this.x = x;
			this.y = y;
		}
		public override string ToString()
		{
			return "(" + x + "," + y + ")";
		}
	}
	public static void Main(string[] args)
	{
		FindClosestPairOfPoints cpp = new FindClosestPairOfPoints();


		int pointSize = 10;
		//value range
		double maxX = 1000;
		double minX = 0;
		double maxY = 1000;
		double minY = 0;


		Console.WriteLine("Starting With {0} Random Points",pointSize);

		Point[] ps = cpp.generateRandomPoints(pointSize, maxX, minX, maxY, minY);



		Point[] pair = cpp.getClosestPair(ps);
		Console.WriteLine("Pairs : ({0},{1})", pair[0].x, pair[0].y);
		double distance = cpp.getDistance(pair);


		pair = cpp.UsingEasyWay(ps);
		Console.WriteLine("Pairs : ({0},{1})", pair[0].x, pair[0].y);
		double distance2 = cpp.getDistance(pair);

		Console.WriteLine("Distance : {0}",distance);

	}
	private Point[] getClosestPair(Point[] ps)
	{
		//Step 0: sort by x
		sortPointsByX(ps);
		//then
		return getClosestPair(ps, 0, ps.Length - 1);
	}
	private Point[] getClosestPair(Point[] ps, int start, int end)
	{

		if (start == end)
		{
			return null;
		}
		if (start + 1 == end)
		{
			return new Point[] { ps[start], ps[end] };
		}

		//System.out.println("Executing from:"+start+" to:"+end);

		//Step 1, find medium X
		int mediumX = (start + end) / 2;

		/*
		 * Step 2. Solve the problem recursively
		 */
		Point[] leftClosestPair = this.getClosestPair(ps, start, mediumX);
		Point[] rightCloestPair = this.getClosestPair(ps, mediumX, end);
		Point[] returnPair;
		double returnDelta;

		double leftDelta = this.getDistance(leftClosestPair);
		double rightDelta = this.getDistance(rightCloestPair);
		double delta;
		if (leftDelta < rightDelta)
		{
			delta = leftDelta;
			returnPair = leftClosestPair;
		}
		else
		{
			delta = rightDelta;
			returnPair = rightCloestPair;
		}
		returnDelta = delta;


		/*
		 * Step 3. Find the minimal distance among the pair of points in which one lies on one side and the other lies on the other side
		 */

		int leftEdge = this.getLeftEdgeOfDelta(ps, mediumX, leftDelta);
		if (leftEdge < start)
		{
			leftEdge = start;
		}
		int rightEdge = this.getRightEdgeOfDelta(ps, mediumX, rightDelta);
		if (rightEdge > end)
		{
			rightEdge = end;
		}
		int[][] yOrderInfo = this.sortPointsByY(ps, leftEdge, rightEdge);
		int[] idxToOrderY = yOrderInfo[0];
		int[] orderYToIdx = yOrderInfo[1];
		for (int i = leftEdge; i <= mediumX; i++)
		{

			int orderY = idxToOrderY[i - leftEdge];

			for (int j = orderY - 1; j >= 0; j--)
			{

				Point[] pair = new Point[] { ps[i], ps[orderYToIdx[j]] };
				if (this.getYDistance(pair) > delta)
				{
					break;
				}
				double distance = this.getDistance(pair);
				if (distance < returnDelta)
				{
					returnDelta = distance;
					returnPair = pair;
				}
			}
			for (int j = orderY + 1; j < idxToOrderY.Length; j++)
			{

				Point[] pair = new Point[] { ps[i], ps[orderYToIdx[j]] };

				//break when Y distance is bigger than delta
				if (this.getYDistance(pair) > delta)
				{
					break;
				}

				//check if this distance is smaller
				double distance = this.getDistance(pair);
				if (distance < returnDelta)
				{
					returnDelta = distance;
					returnPair = pair;
				}
			}

		}
		return returnPair;
	}
	private int[][] sortPointsByY(Point[] ps, int leftEdge, int rightEdge)
	{
		int size = rightEdge - leftEdge + 1;
		int[] orderToIdx = new int[size];
		for (int i = 0; i < size; i++)
		{
			orderToIdx[i] = leftEdge + i;
		}

		quickSortForDeltaArea(ps, orderToIdx, 0, size - 1);

		int[] idxToOrder = new int[size];
		for (int i = 0; i < size; i++)
		{
			idxToOrder[orderToIdx[i] - leftEdge] = i;
		}

		return new int[][] { idxToOrder, orderToIdx };
	}
	private void quickSortForDeltaArea(Point[] ps, int[] indexs, int left, int right)
	{
		if (left >= right)
		{
			return;
		}

		//partition
		int i = left, j = right;
		int tmp;
		double pivot = (ps[indexs[left]].y + ps[indexs[right]].y) / 2;

		while (i <= j)
		{
			//comparing the value of the specific column
			while (ps[indexs[i]].y < pivot)
			{
				i++;
			}
			//comparing the value of the specific column
			while (ps[indexs[j]].y > pivot)
			{
				j--;
			}
			if (i <= j)
			{
				tmp = indexs[i];
				indexs[i] = indexs[j];
				indexs[j] = tmp;
				i++;
				j--;
			}
		}


		if (left < i - 1)
		{
			quickSortForDeltaArea(ps, indexs, left, i - 1);
		}
		if (i < right)
		{
			quickSortForDeltaArea(ps, indexs, i, right);
		}

	}
	private int getRightEdgeOfDelta(Point[] ps, int mediumX, double rightDelta)
	{
		for (int i = mediumX; i < ps.Length; i++)
		{
			if (ps[i].x - ps[mediumX].x > rightDelta)
			{
				return i - 1;
			}
		}
		return ps.Length - 1;
	}
	private int getLeftEdgeOfDelta(Point[] ps, int mediumX, double leftDelta)
	{
		for (int i = mediumX; i >= 0; i--)
		{
			if (ps[mediumX].x - ps[i].x > leftDelta)
			{
				return i + 1;
			}
		}
		return 0;
	}
	private void sortPointsByX(Point[] ps)
	{
		int size = ps.Length;
		quickSort(ps, 0, size - 1);
	}
	private void quickSort(Point[] ps, int left, int right)
	{
		if (left >= right)
		{
			return;
		}

		//partition
		int i = left, j = right;
		Point tmp;
		double pivot = (ps[left].x + ps[right].x) / 2;

		while (i <= j)
		{
			//comparing the value of the specific column
			while (ps[i].x < pivot)
			{
				i++;
			}
			//comparing the value of the specific column
			while (ps[j].x > pivot)
			{
				j--;
			}
			if (i <= j)
			{
				tmp = ps[i];
				ps[i] = ps[j];
				ps[j] = tmp;
				i++;
				j--;
			}
		}


		if (left < i - 1)
		{
			quickSort(ps, left, i - 1);
		}
		if (i < right)
		{
			quickSort(ps, i, right);
		}

	}
	private double getDistance(Point[] ps)
	{
		if (ps == null || ps.Length != 2)
		{
			return -1;
		}
		return Math.Pow((ps[0].x - ps[1].x) * (ps[0].x - ps[1].x) + (ps[0].y - ps[1].y) * (ps[0].y - ps[1].y), 0.5);
	}
	private double getYDistance(Point[] ps)
	{
		return Math.Abs(ps[0].y - ps[1].y);
	}
	private Point[] UsingEasyWay(Point[] ps)
	{
		if (ps.Length == 2)
		{
			return ps;
		}
		Point[] pair = new Point[2];
		Point[] temp = new Point[2];
		double min = double.MaxValue;
		for (int i = 0; i < ps.Length; i++)
		{
			for (int j = i + 1; j < ps.Length; j++)
			{
				temp[0] = ps[i];
				temp[1] = ps[j];
				double dis = this.getDistance(temp);
				if (dis < min)
				{
					min = dis;
					pair[0] = temp[0];
					pair[1] = temp[1];
				}
			}
		}
		return pair;
	}
	private Point[] generateRandomPoints(int size, double maxX, double minX, double maxY, double minY)
	{
		var rand = new Random();

		Point[] points = new Point[size];
		for (int i = 0; i < size; i++)
			points[i] = new Point((rand.NextDouble()) * (maxX - minX) + minX, rand.NextDouble() * (maxY - minY) + minY);

		return points;
	}
}
