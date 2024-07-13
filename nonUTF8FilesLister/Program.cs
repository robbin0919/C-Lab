using System;
using System.IO;
using System.Text;
using System.Linq;
namespace nonUTF8FilesLister;
  class Program
    {
        static void Main(string[] args)
        {
            string directoryPath = args[0]; // Get the directory path from the first command-line argument

            var nonUTF8Files = EnumerateNonUTF8Files(directoryPath);

            if (nonUTF8Files.Any())
            {
                Console.WriteLine("Non-UTF-8 Files:");
                foreach (string file in nonUTF8Files)
                {
                    Console.WriteLine(file);
                }
            }
            else
            {
                Console.WriteLine("No non-UTF-8 files found in the specified directory.");
            }
        }

        static IEnumerable<string> EnumerateNonUTF8Files(string directoryPath)
        {
            return Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories)
                .Where(file => !IsUTF8File(file))
                .Select(file => file);
        }

        static bool IsUTF8File(string filePath)
        {
            try
            {
                using (var streamReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    streamReader.ReadToEnd(); // Read the entire file to check if UTF-8 decoding succeeds
                    return true;
                }
            }
            catch (DecoderFallbackException)
            {
                // UTF-8 decoding failed, indicating non-UTF-8 encoding
                return false;
            }
            catch (Exception ex)
            {
                // Handle other exceptions related to file reading
                Console.WriteLine($"Error processing file: {filePath}. Exception: {ex.Message}");
                return false;
            }
        }
    }