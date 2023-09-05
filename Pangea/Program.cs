using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangea {

	class Program {

		public static Random rand = new Random();
		public static int width = 50;
		public static int height = 50;
		public static Tile[,] Map = new Tile[width, height];
		public static List<Continent> continents = new List<Continent>() { new Continent((int)width / 2, (int)height / 2) }; // Add Super Continent

		public static void Main(string[] args) {

			continents[0].Create((width * height) / 4);
			continents[0] = fracture(continents[0]);
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					for (int i = 0; i < continents[0].provinces.Count; i++) {
						if (continents[0].provinces[i].x == x && continents[0].provinces[i].y == y && continents[0].provinces[i].index == 1) {
							Console.Write("# ");
							break;
						}
						if (i == continents[0].provinces.Count - 1) {
							Console.Write("  ");
						}
					}
				}
				Console.WriteLine();
			}

			while (continents[0].provinces.Count > 0) { // Convert Super Continent to Continent

				continents.Add(breakup(continents[0]));

				for (int i = 0; i < continents[continents.Count - 1].provinces.Count; i++) { // In Newest Continent
					for (int j = 0; j < continents[0].provinces.Count; j++) { // In Super Continent
						if (continents[continents.Count - 1].provinces[i].x == continents[0].provinces[j].x && continents[continents.Count - 1].provinces[i].y == continents[0].provinces[j].y) {
							continents[0].provinces.RemoveAt(j); // Remove Duplicate
						}
					}
				}

			}
			continents.RemoveAt(0);
			drift();

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					for (int i = 0; i < continents.Count; i++) {

						bool cont = true;

						for (int j = 0; j < continents[i].provinces.Count; j++) {
							if (continents[i].provinces[j].x == x && continents[i].provinces[j].y == y) {
								Console.ForegroundColor = continents[i].color;
								Console.Write("# ");
								Console.ResetColor();
								cont = false;
								break;
							}
							if (j == continents[i].provinces.Count - 1 && i == continents.Count - 1) {
								Console.Write("  ");
								cont = false;
								break;
							}
						}
						if (cont == false) {
							break;
						}
					}
				}
				Console.WriteLine();
			}
			Console.ReadLine();

		}

		public static void Print(List<Continent> continents) {
			for(int y = 0; y < height; y++) {
				for(int x = 0; x < width; x++) {

					foreach(Continent continent in continents) {
						for(int i = 0; i < continent.provinces.Count; i++) {
							if(continent.provinces[i].x == x && continent.provinces[i].y == y && continent.provinces[i].index == 1) {
								Console.Write("# ");
								break;
							}
							if(i == continents[0].provinces.Count - 1) {
								Console.Write("  ");
							}
						}
					}
				}
				Console.WriteLine();
			}
		}


		public static int safetynet(int num, int min, int max) {
			if (num < min) {
				num = min;
			}
			else if (num > max) {
				num = max;
			}
			return num;
		}

		public static Continent fracture(Continent continent) {

			int breakups = 4;

			for (int i = 0; i < breakups; i++) {
				Tile fracturepoint = continent.provinces[Program.rand.Next(continent.provinces.Count)];

				bool direction = Program.rand.NextDouble() >= 0.5; // true -> vertical, false -> horizontal
				List<Tile> remove = new List<Tile>();

				for (int k = 0; k < 2; k++) {

					int x = fracturepoint.x;
					int y = fracturepoint.y;

					bool cont = false;

					do {

						cont = false;
						for (int j = 0; j < continent.provinces.Count; j++) {
							if (x == continent.provinces[j].x && y == continent.provinces[j].y) {
								cont = true;
								remove.Add(continent.provinces[j]);
							}
						}

						if (k == 0) {
							if (direction == true) {
								if (Program.rand.NextDouble() >= 0.5 == true) {
									x++;
								}
								y++;
							}
							else {
								if (Program.rand.NextDouble() >= 0.5 == true) {
									y++;
								}
								x++;
							}
						}
						else {
							if (direction == true) {
								if (Program.rand.NextDouble() >= 0.5 == true) {
									x--;
								}
								y--;
							}
							else {
								if (Program.rand.NextDouble() >= 0.5 == true) {
									y--;
								}
								x--;
							}
						}

					} while (cont == true);

				}

				for (int j = 0; j < remove.Count; j++) {
					for (int k = 0; k < continent.provinces.Count; k++) {
						if (remove[j] == continent.provinces[k]) {
							continent.provinces.RemoveAt(k);
						}
					}
				}
			}

			return continent;
		}

		public static Continent breakup(Continent continent) {

			List<Tile> Open = new List<Tile>() { continent.provinces[Program.rand.Next(continent.provinces.Count)] }; // Append FIrst Tile
			List<Tile> Closed = new List<Tile>();

			while (Open.Count > 0) {

				/*
				// Debug
				for (int y = 0; y < Program.height; y++) {
					for (int x = 0; x < Program.width; x++) {
						while (true) {
							bool print = false;
							for (int i = 0; i < Open.Count; i++) { // In Open List
								if (Open[i].x == x && Open[i].y == y) {
									if (i == 0) {
										Console.ForegroundColor = ConsoleColor.DarkYellow;
									}
									else {
										Console.ForegroundColor = ConsoleColor.Green;
									}
									Console.Write("o ");
									Console.ResetColor();
									print = true;
									break;
								}
							}
							if (print == true) {
								break;
							}
							for (int i = 0; i < Closed.Count; i++) {
								if (Closed[i].x == x && Closed[i].y == y) {
									Console.ForegroundColor = ConsoleColor.Red;
									Console.Write("x ");
									Console.ResetColor();
									print = true;
									break;
								}
							}
							if (print == true) {
								break;
							}
							Console.ForegroundColor = ConsoleColor.White;
							Console.Write("  ");
							Console.ResetColor();
							break;
						}
					}
					Console.WriteLine();
				} // This Method is my baby
				//Console.ReadLine();
				*/


				for (int i = 0; i < 4; i++) { // Open Neighbors

					int x = Open[0].x;
					int y = Open[0].y;

					switch (i) {
						case 0:
							y++;
							break;
						case 1:
							y--;
							break;
						case 2:
							x++;
							break;
						case 3:
							x--;
							break;
						case 4:
							y++;
							x--;
							break;
						case 5:
							y++;
							x++;
							break;
						case 6:
							y--;
							x--;
							break;
						case 7:
							y--;
							x++;
							break;
					}

					for (int j = 0; j < continent.provinces.Count; j++) { // Is a province?
						if (x == continent.provinces[j].x && y == continent.provinces[j].y) { // Yes

							bool ok = true;

							for (int k = 0; k < Closed.Count; k++) { // In CLosed List -> Doesn't Work
								if (x == Closed[k].x && y == Closed[k].y) {
									ok = false;
									break;
								}
							}

							if (ok == true) {
								Closed.Add(new Tile(x, y, 1));
								Open.Add(new Tile(x, y, 1));
							}

							break;
						}
					}
				}
				Closed.Add(Open[0]);
				Open.Remove(Open[0]);
			}

			Continent newContinent = new Continent(0, 0);
			for (int i = 0; i < Closed.Count; i++) {
				newContinent.provinces.Add(Closed[i]);
			}

			return newContinent;
		}

		public static void drift() {
			for (int k = rand.Next(10) + 1; k > 0; k--) {
				for (int i = 0; i < continents.Count; i++) {
					while (Program.rand.NextDouble() <= 0.5) { // Drift
						continents[i].x += Program.rand.Next(3) - 1;
						continents[i].y += Program.rand.Next(3) - 1;
					}
					for (int j = 0; j < continents[i].provinces.Count; j++) {
						continents[i].provinces[j].x += continents[i].x;
						continents[i].provinces[j].x += continents[i].y;
						// Safety Net -> Remove out of Bounds
						if (continents[i].provinces[j].x >= height - 1 || continents[i].provinces[j].x < 0 || continents[i].provinces[j].y >= width - 1 || continents[i].provinces[j].y < 0) {
							continents[i].provinces.RemoveAt(j);
						}
					}
				}
			}
		}

	}

	public class Continent {
		public List<Tile> provinces = new List<Tile>();
		public int x = 0;
		public int y = 0;
		public ConsoleColor color;

		public Continent(int x, int y) {
			this.x = x;
			this.y = y;


			this.color = (ConsoleColor)Program.rand.Next(15) + 1;
			/*
			switch (Program.rand.Next(10)) {
				case 0:
					this.color = ConsoleColor.Red;
					break;
				case 1:
					this.color = ConsoleColor.Blue;
					break;
				case 3:
					this.color = ConsoleColor.Green;
					break;
				case 4:
					this.color = ConsoleColor.Yellow;
					break;
				case 5:
					this.color = ConsoleColor.DarkRed;
					break;
				case 6:
					this.color = ConsoleColor.DarkBlue;
					break;
				case 7:
					this.color = ConsoleColor.DarkGreen;
					break;
				case 8:
					this.color = ConsoleColor.DarkYellow;
					break;
				default:
					this.color = ConsoleColor.White;
					break;
			}
			*/
		}

		public void Create(int counter) {
			this.provinces.Add(new Tile(this.x, this.y, 1)); // Append First Province

			for (int i = 0; i < counter - 1; i++) { // In Counter

				List<Tile> viable = new List<Tile>(); // Tile + Viable Directions

				for (int j = 0; j < this.provinces.Count; j++) { // Find Viable Tiles

					bool[] directions = new bool[4] { true, true, true, true };

					for (int k = 0; k < 4; k++) { // Find Avialiable Points

						int x = this.provinces[j].x;
						int y = this.provinces[j].y;

						switch (k) {
							case 0:
								x++;
								break;
							case 1:
								x--;
								break;
							case 2:
								y++;
								break;
							case 3:
								y--;
								break;
							default:
								break;
						}
						x = Program.safetynet(x, 0, Program.width - 1);
						y = Program.safetynet(y, 0, Program.height - 1);

						for (int l = 0; l < this.provinces.Count; l++) { // Find if Tile exists
							if (this.provinces[l].x == x && this.provinces[l].y == y) { // Tile exists
								directions[k] = false;
							}
						}
					}

					if (directions[0] == false && directions[1] == false && directions[2] == false && directions[3] == false) { // Has All Neighbors

					}
					else { // Has Open Neighbor

						int index;

						do {
							index = Program.rand.Next(directions.Length);
						} while (directions[index] == false);

						x = this.provinces[j].x;
						y = this.provinces[j].y;

						switch (index) {
							case 0:
								x++;
								break;
							case 1:
								x--;
								break;
							case 2:
								y++;
								break;
							case 3:
								y--;
								break;
							default:
								break;
						}
						x = Program.safetynet(x, 0, Program.width - 1);
						y = Program.safetynet(y, 0, Program.height - 1);

						viable.Add(new Tile(x, y, 1));

					}

				}
				int foo = Program.rand.Next(viable.Count);
				this.provinces.Add(viable[foo]);
			}
		}
	}
	public class Tile {
		public int x;
		public int y;
		public int index = -1;

		public Tile(int x, int y, int index = -1) {
			this.x = x;
			this.y = y;
			this.index = index;
		}
	}
}