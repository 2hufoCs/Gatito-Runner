# Gatito-Runner
My first mobile game ever, based on a plush that I own : Gatito!!


## Pre-written chunks of code
  ```
	public void RotateToVertical()
	{
		float halflength = boxCollider.x / 2
		
		hit = Physics.Raycast(transform.position, Vector2.down, halfLength, obsacleLayer)
		if (hit)
		{
			transform.position = bottomLimit + halfLength;
			return;
		}
		
		hit = Physics.Raycast(transform.position, Vector2.up, halfLength, obsacleLayer)
		if (hit)
		{
			transform.position = upperLimit - halfLength;
			return;
		}
	}
 ```
