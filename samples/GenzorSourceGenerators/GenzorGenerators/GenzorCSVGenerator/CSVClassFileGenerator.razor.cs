using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Components;
using NotVisualBasic.FileIO;

namespace GenzorSourceGenerators.GenzorGenerators.GenzorCSVGenerator
{
	public partial class CSVClassFileGenerator : ComponentBase
	{
		private List<(Type TypeName, string Name, int ColumnIndex)> Properties { get; } = new();

		private List<string[]> Rows { get; } = new List<string[]>();

		[Parameter] public CsvGeneratorOption Option { get; set; } = default!;

		protected override void OnInitialized()
		{
			using var parser = new CsvTextFieldParser(new StringReader(Option.CsvText));
			var (types, names, fields) = ExtractProperties(parser);

			for (int i = 0; i < types.Length; i++)
			{
				Properties.Add((types[i], names[i], i));
			}

			if (fields is not null)
			{
				Rows.Add(fields);

				while (!parser.EndOfData)
				{
					Rows.Add(parser.ReadFields());
				}
			}
		}

		private static string GetCsvValue(Type type, string rawValue)
		{
			return type == typeof(string)
				? $"\"{rawValue.Trim().Trim(new char[] { '"' })}\""
				: rawValue;
		}

		static (Type[] types, string[] names, string[]? firstLineFields) ExtractProperties(CsvTextFieldParser parser)
		{
			string[]? headerFields = parser.ReadFields();
			if (headerFields == null) throw new Exception("Empty csv file!");

			string[]? firstLineFields = parser.ReadFields();

			if (firstLineFields == null)
			{
				var types = Enumerable.Repeat(typeof(string), headerFields.Length).ToArray();
				return (types, headerFields, firstLineFields);
			}
			else
			{
				var types = firstLineFields.Select(GetCsvFieldType).ToArray();
				var names = headerFields.Select(StringToValidPropertyName).ToArray();
				return (types, names, firstLineFields);
			}
		}

		static string StringToValidPropertyName(string s)
		{
			s = s.Trim();
			s = char.IsLetter(s[0]) ? char.ToUpper(s[0]) + s.Substring(1) : s;
			s = char.IsDigit(s.Trim()[0]) ? "_" + s : s;
			s = new string(s.Select(ch => char.IsDigit(ch) || char.IsLetter(ch) ? ch : '_').ToArray());
			return s;
		}

		// Guesses type of property for the object from the value of a csv field
		static Type GetCsvFieldType(string exemplar) => exemplar switch
		{
			_ when bool.TryParse(exemplar, out _) => typeof(bool),
			_ when int.TryParse(exemplar, out _) => typeof(int),
			_ when double.TryParse(exemplar, out _) => typeof(double),
			_ => typeof(string)
		};
	}
}
