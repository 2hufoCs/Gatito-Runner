# Gatito-Runner
My first mobile game ever, based on a plush that I own : Gatito!! It's also the first time that I make a game in 3D, so I'm completely out of my comfort zone there.
It's an endless runner, where you need to dodge obtacles left and right along the way.There are 2 twists to it:
1. At some milestones, the player gets larger (as well as the ground platform) so it becomes harder and harder to dodge
2. The player can rotate along the z axis (forward), so they take much place vertically instead of horizontally. This can make for some interesting ways to dodge obstacles.

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
