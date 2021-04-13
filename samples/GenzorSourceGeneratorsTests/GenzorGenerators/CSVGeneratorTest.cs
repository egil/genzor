using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvGenerator;
using Genzor;
using GenzorSourceGenerators.GenzorGenerators;
using GenzorSourceGenerators.GenzorGenerators.GenzorCSVGenerator;
using Xunit;
using Xunit.Abstractions;

namespace GenzorSourceGeneratorsTests.GenzorGenerators
{
	public class CSVGeneratorTest : GenzorTestBase
	{
		private const string CarsCsv = @"Brand, Model, Year, cc
Fiat, Punto, 2008, 12.3
Ford, Wagon, 1956, 20.3";

		public CSVGeneratorTest(ITestOutputHelper outputHelper) : base(outputHelper)
		{
		}

		[Fact]
		public async Task MyTestMethod()
		{
			var option = new CsvGeneratorOption(
				CarsCsv,
				"Cars",
				CsvLoadType.OnDemand,
				false);

			await Host.InvokeGeneratorAsync<CSVClassFileGenerator>(ps => ps.Add(p => p.Option, option));
		}
	}
}
