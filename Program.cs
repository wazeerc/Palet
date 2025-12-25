using System.Text.Json;

class Program
{
  static readonly HttpClient client = new();
  static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

  public static string EndPoint = "https://colormagic.app/api/palette/search?q=";

  public static List<Palette> colorPalettes = new();
  public class Palette
  {
    required public List<string> Colors { get; set; }
  }

  static async Task Main()
  {
    Console.Title = "Palet - A Color Palette Generator";

    while (true)
    {
      Console.Clear();
      Console.WriteLine("\nEnter a search term to generate a color palette: ");
      string? input = Console.ReadLine();

      if (string.IsNullOrWhiteSpace(input)) break;
      await GenerateRandomPalette(input);

      Console.ReadKey();

      client.Dispose();
    }

    static async Task GenerateRandomPalette(string _input)
    {
      try
      {
        var response = await FetchPalettesFromColorMagic(_input);
        var generatedPalettes = ProcessResponse(response);

        Console.WriteLine("\nGenerated Palettes:");
        for (int i = 0; i < generatedPalettes.Count; i++)
        {
          Console.WriteLine($"{i + 1}: [{string.Join(", ", generatedPalettes[i].Colors)}]");
        }
      }
      catch
      {
        Console.WriteLine("\nSomething went wrong.");
      }
    }

    static async Task<string> FetchPalettesFromColorMagic(string _query)
    {
      var url = EndPoint + _query;

      try
      {
        var response = await client.GetStringAsync(url);
        return response;
      }
      catch
      {
        return string.Empty;
      }
    }

    static List<Palette> ProcessResponse(string _response)
    {
      if (string.IsNullOrWhiteSpace(_response)) return colorPalettes;

      var ColorMagicPalettes = JsonSerializer.Deserialize<List<Palette>>(_response, jsonOptions);
      if (ColorMagicPalettes is null) return colorPalettes;

      colorPalettes = [.. ColorMagicPalettes.Where(p => p is not null && p.Colors is not null)];
      return colorPalettes;
    }
  }
}
