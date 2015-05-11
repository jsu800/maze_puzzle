##Maze Puzzle
This is a command line program in C# that takes a maze image as input and solves the maze and 
writes it to an output image. The attached sample maze inputs (../input) are solved accordinly 
(../output). Note that the samples do not represent all types of mazes.

The syntax of the program is:

maze.exe “source.[bmp,png,jpg]” “destination.[bmp,png,jpg]”

##Rules:
- Entry Point in Red

- Destination Point in Blue

- All walls are in Black

- Maze may be completely surrounded by Black walls or with "crevices" or break in Walls

- Solution route is drawn in Green

##Thoughts
- This illustrates the wall following solution which isn’t the shortest one. DFS solution is about 20 lines 
but it is recursively intensive on stack with large images. More optimal solution would involve A* Pathfinder,
Pledge, Tremaux, etc, algo or the likes. Mine hugs the nearest wall one pixel over and moves along the wall 
until exit is found. Fast and no extra memory. O(n).This is assuming maze wall is non-continuous and the exit 
isn’t in the center of the maze.

- Solution route can be made shorter by provisioning a "dirty bit" vector consisting of visited pixel coordinates; revisiting the same route again can eliminate all of the "cycles" (i.e., out and back routes that can be avoided) to make the entire route much shorter. We trade space for efficiency.

##Observations
Even though this console outputs its soluiton on a relatively complex maze (see maze2 and maze3) in under a few seconds, one may optimize the solution using BitmapData instead of GetPixel(), converting all ARGB data into RGB 
arrays for manipulation. 

##Recursive code snippet:

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
## Example Input
![](https://github.com/jsu800/maze_puzzle/blob/master/CSharpMazeProblem/CSharpMazeProblem/input/maze2.png)

## Solved Output
![](https://github.com/jsu800/maze_puzzle/blob/master/CSharpMazeProblem/CSharpMazeProblem/output/output_maze2.png)

##License
The MIT License (MIT)

Copyright (c) 2014 github.com/jsu800
