#include <stdio.h>

main()->int
{
	a: *Vec2 = new *Vec2(3, 7);
	b: Vec2 = new Vec2(1, 2);
	c: Vec2 = b.copy().add(a)!;

	for (i: int = 0; i < 10; i += 10)
	{
		doStuff(i);
	}

	printf("%f, %f\n", c.x, c.y);

	//q: *Vec2 = @b;

	return 0;
}