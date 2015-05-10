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


/* 
 * @author Joseph Su
 * 
 * Wall follower schema
 */

namespace MazeBase
{
	public class Maze
	{
		private Bitmap mSource;
		private Bitmap mDestination;

		public Bitmap MazeDestination {
			get {
				return mDestination;
			}
		}
		public Bitmap MazeSource {
			get {
				return mSource;
			}
		}
			
		public bool LoadMaze(string src, string dest) {

			try {
				mSource = (Bitmap)Image.FromFile (src);
				mDestination = new Bitmap(mSource);
			} catch (OutOfMemoryException ome) {
				Console.WriteLine ("Invalid source image format, possibly: " + ome.StackTrace);
				return false;
			}
			return true;
		}

		public bool SaveMaze(string name, ImageFormat format) {
			if (MazeDestination != null || String.IsNullOrEmpty (name)) {
				try {
					MazeDestination.Save (name, format);
					return true;
				} catch (Exception e) {					
					Console.WriteLine ("Maze cannot be saved: " + e.StackTrace);
				} 
			} 
			return false;
		}

		public void FindStartingPointInMaze(out int x, out int y) {
			x = -1;
			y = -1;
			for (int i = 0; i < MazeDestination.Height; i++) 
			{ 
				for (int j = 0; j < MazeSource.Width; j++) 
				{ 					
					// once red is found, break to find nearest wall to follow
					if (MazeSource.GetPixel(j,i).ToArgb()==Color.Red.ToArgb()) {
						x = j;
						y = i;
						return;
					}
				} 
			}
		}

		public bool Solve(int x,int y)
		{
			if (!IsValid (x, y)) return false;
			while (IsValid (x, y) && mSource.GetPixel (x, y).ToArgb () != Color.Blue.ToArgb ()) {
				if (MoveAlongWall (ref x, ref y) && IsValid (x, y))
					MazeDestination.SetPixel (x, y, Color.Green);				
				else if (MoveToNearestWall (ref x, ref y) && IsValid (x, y))				
					MazeDestination.SetPixel (x, y, Color.Green);
				else
					break;		
			} 
			return true;
		}

		public bool MoveAlongWall(ref int x, ref int y) {

			bool isRightBlocked = false, isLeftBlocked = false, isTopBlocked = false, isBottomBlocked = false;

			if (IsValidWall (x - 1, y))
				isLeftBlocked = true;
			if (IsValidWall (x + 1, y))
				isRightBlocked = true;
			if (IsValidWall (x, y + 1))
				isBottomBlocked = true;
			if (IsValidWall (x, y - 1))
				isTopBlocked = true;

			// corner: top left 
			if (isLeftBlocked && isTopBlocked) {
				if (isRightBlocked) {
					y++;
					CheckMovingThroughTube (ref x, ref y, false, false, false, true);
				} else {
					x++;
					CheckMovingThroughTube (ref x, ref y, false, true, false, false);
				}
			}

			// corner: top right 
			else if (isTopBlocked && isRightBlocked) {
				if (isBottomBlocked) {
					x--;
					CheckMovingThroughTube (ref x, ref y, true, false, false, false);
				} else {
					y++;
					CheckMovingThroughTube (ref x, ref y, false, false, false, true);
				}
			}

			// corner: bottom right 
			else if (isRightBlocked && isBottomBlocked) {
				if (isLeftBlocked) {
					y--;
					CheckMovingThroughTube (ref x, ref y, false, false, true, false);
				} else {
					x--;
					CheckMovingThroughTube (ref x, ref y, true, false, false, false);
				}
			}

			// corner: bottom left
			else if (isBottomBlocked && isLeftBlocked) {
				if (isTopBlocked) {
					x++;
					CheckMovingThroughTube (ref x, ref y, false, true, false, false);
				} else {
					y--;
					CheckMovingThroughTube (ref x, ref y, false, false, true, false);
				}
			}

			// side: left 
			else if (isLeftBlocked) {
				y--;
				CheckMovingThroughTube (ref x, ref y, false, false, true, false);
			}

			// side: right 
			else if (isRightBlocked) {
				y++;
				CheckMovingThroughTube (ref x, ref y, false, false, false, true);
			}

			// side: bottom
			else if (isBottomBlocked) {
				x--;
				CheckMovingThroughTube (ref x, ref y, true, false, false, false);
			}

			// side: top
			else if (isTopBlocked) {
				x++;
				CheckMovingThroughTube (ref x, ref y, false, true, false, false);

			} else {
				return false;
			}
			return true;
		}

		public bool MoveToNearestWall(ref int x, ref int y) {
			//bool isUp, isDown, isLeft, isRight;
			int moveCnt = 1;

			while (true) {

				// if we are completely out of bound in all directions, no solution, exit right away
				if (!IsValid (x + moveCnt, y)
					&& !IsValid (x - moveCnt, y)
					&& !IsValid (x, y + moveCnt)
					&& !IsValid (x, y - moveCnt)
					&& !IsValid (x - moveCnt, y - moveCnt)
					&& !IsValid (x + moveCnt, y - moveCnt)
					&& !IsValid (x + moveCnt, y + moveCnt)
					&& !IsValid (x - moveCnt, y + moveCnt))
					return false;


				// check corners first, then move to its next position
				if (IsValidWall (x - moveCnt, y - moveCnt)) {
					// upper left
					x = x - moveCnt + 1;
					y = y - moveCnt + 1;

					if (!IsValidWall (x, y - moveCnt)) {
						y--;
						CheckMovingThroughTube (ref x, ref y, false, false, true, false);
					} else {
						x++;
						CheckMovingThroughTube (ref x, ref y, false, true, false, false);
					}												
					break;
				}
				else if (IsValidWall (x + moveCnt, y + moveCnt)) {
					// lower right
					x = x + moveCnt - 1;
					y = y + moveCnt - 1;

					if (!IsValidWall (x, y + moveCnt)) {
						y++;
						CheckMovingThroughTube (ref x, ref y, false, false, false, true);
					} else {
						x--;
						CheckMovingThroughTube (ref x, ref y, true, false, false, false);
					}					
					break;
				}
				else if (IsValidWall (x + moveCnt, y - moveCnt)) {
					//upper right
					x = x + moveCnt - 1;
					y = y - moveCnt + 1;

					if (!IsValidWall (x + moveCnt, y)) {
						x++;
						CheckMovingThroughTube (ref x, ref y, false, true, false, false);
					} else {
						y++;
						CheckMovingThroughTube (ref x, ref y, false, false, false, true);
					}					
					break;
				}
				else if (IsValidWall (x - moveCnt, y + moveCnt)) {
					// lower left
					x = x - moveCnt + 1;
					y = y + moveCnt - 1;

					if (!IsValidWall (x - moveCnt, y)) {
						x--;
						CheckMovingThroughTube (ref x, ref y, true, false, false, false);
					} else {
						y--;
						CheckMovingThroughTube (ref x, ref y, false, false, true, false);
					}
					break;
				}

				// then check sides, i.e., immediate up, down, left, and right sides and move towards it
				else if (IsValidWall (x + moveCnt, y)) {
					x = x + moveCnt - 1;
					break;
				}
				else if (IsValidWall (x - moveCnt, y)) {
					x = x - moveCnt + 1;
					break;
				}
				else if (IsValidWall (x, y + moveCnt)) {
					y = y + moveCnt - 1;
					break;
				}
				else if (IsValidWall (x, y - moveCnt)) {
					y = y - moveCnt + 1;
					break;
				}

				moveCnt++;
			}
			return true;
		}

		private void CheckMovingThroughTube(ref int x, ref int y, bool isLeft, bool isRight, bool isUp, bool isDown) {

			MazeDestination.SetPixel (x, y, Color.Green);

			if (isLeft || isRight) {
				// check if we are moving through tube

				while (IsInHorizontalTube(x, y)) {

					if (isLeft && !IsValidWall (x - 1, y)) {
						x--;
					} else if (isRight && !IsValidWall (x + 1, y)) {
						x++;					
					}

					// if wall is eminent turn around and move back
					else if (isLeft && IsValidWall (x - 1, y)) {
						isLeft = false;
						isRight = true;
						x++;
					} else if (isRight && IsValidWall (x + 1, y)) {
						isLeft = true;
						isRight = false;
						x--;
					}
				}
			}
			if (isUp || isDown) {
				// check if we are moving through tube
				while (IsInVerticalTube(x, y)) {

					if (isUp && !IsValidWall (x, y - 1)) {
						y--;
					} else if (isDown && !IsValidWall (x, y + 1)) {
						y++;					
					}

					// if wall is eminent turn around and move back
					else if (isUp && IsValidWall (x, y - 1)) {
						isUp = false;
						isDown = true;
						y++;
					} else if (isDown && IsValidWall (x, y + 1)) {
						isUp = true;
						isDown = false;
						y--;
					}
				}
			}

			// now out of the tube, make sure we position element along wall and it isn't another tube to go through
			if (isUp && !IsValidWall (x - 1, y) && !IsInHorizontalTube(x - 1, y)) {
				x--;
			} if (isDown && !IsValidWall (x + 1, y) && !IsInHorizontalTube(x + 1, y)) {
				x++;
			}
			if (isLeft && !IsValidWall (x, y + 1) && !IsInVerticalTube(x, y + 1)) {
				y++;
			}
			if (isRight && !IsValidWall (x, y - 1) && !IsInVerticalTube(x, y - 1)) {
				y--;
			}
		}

		private bool IsInVerticalTube(int x, int y) {
			return (IsValidWall (x + 1, y) && IsValidWall (x - 1, y));
		}

		private bool IsInHorizontalTube(int x, int y) {
			return (IsValidWall (x, y - 1) && IsValidWall (x, y + 1));
		}

		private bool IsValid(int x,int y)
		{
			// returning true if in-range
			if( x>=0
				&& x<mSource.Width	 
				&& y>=0 
				&& y<mSource.Height
			)		
				return true;
			return false;
		}

		private bool IsValidWall(int a, int b) {
			if (!IsValid (a, b))
				return true;
			else 
				return (IsValid(a,b) && MazeSource.GetPixel (a, b).ToArgb () == Color.Black.ToArgb ());
		}

	}
}

