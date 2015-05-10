/*
 * Copyright (c) 2014 Joseph Su
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.Drawing; 
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using MazeBase;

/*
 * @author Joseph Su
 * 
 * Entry point console
 * 
 */
namespace CSharpMazeProblem
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string srcName = args [0];
			string destName = args [1];

			if (String.IsNullOrEmpty(srcName) || String.IsNullOrEmpty(destName)) {
				Console.WriteLine ("Please enter a valid image source");
				return;
			}
				
			var m = new Maze ();

			// #1 load maze
			if (m.LoadMaze (srcName, destName)) {

				// #2 get to the starting point
				int x, y;
				m.FindStartingPointInMaze (out x, out y);

				try {

					// #3 move to the nearest wall hugging it
					if (m.MoveToNearestWall(ref x, ref y)) {

						// #4 traverse through to end
						if (m.Solve (x, y))	{

							// #5 save maze
							m.SaveMaze(destName, ImageFormat.Png);				
							return;
						}
					} 
					Console.WriteLine("Sorry. This maze cannot be solved");

				} catch (Exception e) {
					Console.WriteLine ("Exception: " + e.Message);
				}
			}
		}
	} // end class
}