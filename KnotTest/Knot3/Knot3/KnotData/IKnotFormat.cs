using System;

namespace Knot3.KnotData
{
	public interface IKnotFormat
	{
		string[] FileExtensions { get; }

		KnotInfo LoadInfo (string filename);

		Knot LoadKnot (string filename);

		void SaveKnot (Knot knot);

		string FindFilename (string knotName);
	}
}

