#include <stdio.h>

main()->int
{
	a: *Vec2 = new *Vec2(3, 7);
	b: Vec2 = new Vec2(1, 2);
	c: Vec2 = _copy_Vec2(_add_Vec2(@b, a));
	//c: Vec2 = b.add(a!).copy();  // To call member functions, we first need the type of what is being called. Guess it is time to start working on a symbol table.

	printf("%f, %f\n", c.x, c.y);

	//q: *Vec2 = @b;

	return 0;
}