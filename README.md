=============
Maze Puzzle
=============

This is a command line program in C# that takes a maze image as input and solves the maze and 
writes it to an output image. The attached sample maze inputs (../input) are solved accordinly 
(../output). Note that the samples do not represent all types of mazes.

The syntax of the program is:

maze.exe “source.[bmp,png,jpg]” “destination.[bmp,png,jpg]”

Rules:

1) Start at Red

2) Finish at Blue

3) All walls are black

4) Maze is completely surrounded by black walls

5) Draw the solution on the image in green

This illustrates the wall following solution which isn’t the shortest one. DFS solution is about 20 lines 
but it is recursively intensive on stack with large images. More optimal solution would involve A* Pathfinder,
Pledge, Tremaux, etc, algo or the likes. Mine hugs the nearest wall one pixel over and moves along the wall 
until exit is found. Fast and no extra memory. O(n).This is assuming maze wall is non-continuous and the exit 
isn’t in the center of the maze.

One can also optimize the solution using BitmapData instead of GetPixel(), converting all ARGB data into RGB 
arrays for manipulation.

Recursive solution code snippet:

```
if (mSource.GetPixel (x, y).ToArgb () == Color.White.ToArgb ()) 
{
    if (Solve (visited, x - 1, y)
        || Solve (visited, x + 1, y)
        || Solve (visited, x, y - 1)
        || Solve (visited, x, y + 1)) 
    {
        mDestination.SetPixel (x, y, Color.Green);
        return true;
    }
    return false;

} else if ( mSource.GetPixel (x, y).ToArgb() == Color.Red.ToArgb()) 
{
    if (Solve (visited, x - 1, y)
        || Solve (visited, x + 1, y)
        || Solve (visited, x, y - 1)
        || Solve (visited, x, y + 1)) 
    {
        return true;
    }
    return false;
} else if (mSource.GetPixel (x, y).ToArgb() == Color.Blue.ToArgb())
    return true;
else {
    return false;
}
```
