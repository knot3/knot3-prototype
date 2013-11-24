#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Knot3
{
	static class Program
	{
		private static Game game;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main ()
		{
			game = new Game ();
			game.Run ();
		}
	}
}
