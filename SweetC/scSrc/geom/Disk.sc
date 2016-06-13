Disk
{
	position: Vec2;
	radius: float;


	new(position: Vec2, radius: float)
	{
		.position = position;
		.radius = radius;
	}

	copy()->Disk
	{
		return new Disk(.position, .radius);
	}

	area()->float
	{
		return 3.14159f * .radius * .radius;
	}
}