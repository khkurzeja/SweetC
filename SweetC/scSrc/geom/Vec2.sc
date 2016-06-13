Vec2
{
	x, y: float;  // Might want to limit this to setting literals. If someone wants to set to a function, the function will be called multiple times, which may be unexpected.
	              // You can't even set default values in C anyway. Maybe it should not be allowed.

	// No function overloading for now. Need to create symbol table first to know which names to mangle (ie. don't mangle names we don't have a symbol for. These are from C)

	new(x, y: float)
	{
		.x = x;
		.y = y;
	}

	copy()->Vec2
	{
		return new Vec2(.x, .y);
	}

	add(v: *Vec2)->*Vec2
	{
		.x += v!.x;
		.y += v!.y;
		return this;
	}

	mul(s: float)->*Vec2
	{
		.x *= s;
		.y *= s;
		return this;
	}
}