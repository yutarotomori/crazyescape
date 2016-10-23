[System.Serializable]
public struct Point {

	public	int x;
	public	int y;


	#region operators
	public static Point operator+ (Point a, Point b) {
		return	new Point (a.x+b.x, a.y+b.y);
	}
	
	public static Point operator- (Point a, Point b) {
		return	new Point (a.x-b.x, a.y-b.y);
	}
	
	public static Point operator* (Point a, int b) {
		return	new Point (a.x*b, a.y*b);
	}
	
	public static Point operator/ (Point a, int b) {
		return	new Point (a.x/b, a.y/b);
	}
	
	public static Point operator% (Point a, int b) {
		return	new Point (a.x%b, a.y%b);
	}
	
	public static bool operator== (Point a, Point b) {
		return	(a.x==b.x && a.y==b.y);
	}
	
	public static bool operator!= (Point a, Point b) {
		return	(a.x!=b.x || a.y!=b.y);
	}
	#endregion

	#region standard overrides
	public override bool Equals (object obj) {
		if (obj is Point) {
			Point	p	= (Point)obj;
			return	p == this;
		} else {
			return	false;
		}
	}

	public override int GetHashCode () {
		return base.GetHashCode ();
	}

	public override string ToString ()
	{
		return string.Format ("({0:0},{1:0})", this.x, this.y);
	}
	#endregion

	#region constructors
	public Point (int x, int y) {
		this.x	= x;
		this.y	= y;
	}
	#endregion


	#region static propaties
	[System.Obsolete("System.Drawing")]
	public	static	Point	Empty {
		get {
			return	new Point (0,0);
		}
	}

	public	static	Point	zero {
		get {
			return	new Point (0,0);
		}
	}

	public	static	Point	one {
		get {
			return	new Point (1,1);
		}
	}

	public	static	Point	right {
		get {
			return	new Point (1,0);
		}
	}

	public	static	Point	up {
		get {
			return	new Point (0,1);
		}
	}
	#endregion

	#region propaties
	public	bool	IsEmpty {
		get {
			return	(this.x == 0 && this.y == 0);
		}
	}
	#endregion

	#region public methods
	public	void	Set (int x, int y) {
		this.x	= x;
		this.y	= y;
	}

	public	void	Offset (Point p) {
		this	+= p;
	}

	public	void	Offset (int x, int y) {
		this.x	+= x;
		this.y	+= y;
	}
	#endregion
}
