using System;

namespace Knot3.KnotData
{
	/// <summary>
	/// Dieses Interface muss von einem Knoten-Dateiformat implementiert werden.
	/// </summary>
	public interface IKnotFormat
	{
		string[] FileExtensions { get; }

		KnotInfo LoadInfo (string filename);

		Knot LoadKnot (string filename);

		void SaveKnot (Knot knot);

		string FindFilename (string knotName);
	}
}

