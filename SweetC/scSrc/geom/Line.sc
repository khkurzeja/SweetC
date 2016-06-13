Line
{
	a, b: Vec2;


	new(a, b: Vec2)
	{
		.a = a;
		.b = b;
	}

	copy()->Line
	{
		return new Line(.a, .b);
	}

	length()->float
	{
		return sqrt( (.a.x-.b.x)*(.a.x-.b.x) + (.a.y-.b.y)*(.a.y-.b.y) );
	}
}