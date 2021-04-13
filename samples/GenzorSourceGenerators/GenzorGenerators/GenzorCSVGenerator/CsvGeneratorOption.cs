using CsvGenerator;

namespace GenzorSourceGenerators.GenzorGenerators
{
	public class CsvGeneratorOption
	{
		public CsvLoadType LoadType { get; }

		public bool CacheObjects { get; }

		public string ClassName { get; }

		public string CsvText { get; }

		public CsvGeneratorOption(string csvText, string className, CsvLoadType loadType, bool cacheObjects)
		{
			LoadType = loadType;
			CsvText = csvText;
			ClassName = className;
			CacheObjects = cacheObjects;
		}
	}
}
