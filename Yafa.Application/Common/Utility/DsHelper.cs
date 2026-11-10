using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Yafa.Application.Common.Utility
{
	public class DsHelper
	{
		public static string PrepareFullPrivateKeyFilePath(string fileName)
		{
			const string defaultRsaPrivateKeyFileName = "private.key";

			var fileNameOnly = Path.GetFileName(fileName);
			if (string.IsNullOrEmpty(fileNameOnly))
			{
				fileNameOnly = defaultRsaPrivateKeyFileName;
			}

			var filePath = Path.GetDirectoryName(fileName);
			if (string.IsNullOrEmpty(filePath))
			{
				filePath = Directory.GetCurrentDirectory();
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && Directory.GetCurrentDirectory().Contains("bin"))
			{
				fileNameOnly = defaultRsaPrivateKeyFileName;
				filePath = Path.GetFullPath(filePath);
			}

			return Path.Combine(filePath, fileNameOnly);
		}

		public static byte[] ReadFileContent(string path)
		{
			return File.ReadAllBytes(path);
		}

		public string CapitalizeFirstLetter(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input; // Return the original string if it is null or empty
			}

			// Capitalize the first letter and concatenate with the rest of the string
			return char.ToUpper(input[0]) + input.Substring(1);
		}

		public static string? GetImageAsBase64(string relativePath)
		{

			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "couponBG", "bg_giftcard.png");

			if (!System.IO.File.Exists(filePath))
				return null;

			try
			{
				var imageBytes = System.IO.File.ReadAllBytes(filePath);

				var extension = Path.GetExtension(filePath).ToLowerInvariant();
				var mimeType = extension switch
				{
					".jpg" or ".jpeg" => "image/jpeg",
					".png" => "image/png",
					".gif" => "image/gif",
					".svg" => "image/svg+xml",
					_ => "application/octet-stream"
				};

				return $"data:{mimeType};base64,{Convert.ToBase64String(imageBytes)}";
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error reading image {filePath}: {ex.Message}");
				return null;
			}
		}

		public static string ToAcronymRegex(string input)
		{
			if (string.IsNullOrWhiteSpace(input)) return string.Empty;

			var matches = Regex.Matches(input, @"\b\p{L}"); // word-boundary + first letter (unicode letter)
			return string.Concat(matches.Cast<Match>().Select(m => m.Value.ToUpperInvariant()));
		}

		public static string? GetLogoImageAsBase64(string relativePath)
		{

			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

			if (!System.IO.File.Exists(filePath))
				return null;

			try
			{
				var imageBytes = System.IO.File.ReadAllBytes(filePath);

				var extension = Path.GetExtension(filePath).ToLowerInvariant();
				var mimeType = extension switch
				{
					".jpg" or ".jpeg" => "image/jpeg",
					".png" => "image/png",
					".gif" => "image/gif",
					".svg" => "image/svg+xml",
					_ => "application/octet-stream"
				};

				return $"data:{mimeType};base64,{Convert.ToBase64String(imageBytes)}";
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error reading image {filePath}: {ex.Message}");
				return null;
			}
		}

		public static string ToIndianNumberFormatNoDecimal(decimal amount)
		{
			return amount.ToString("#,##0", new System.Globalization.CultureInfo("en-US"));
		}
	}
}
