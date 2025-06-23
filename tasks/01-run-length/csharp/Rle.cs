using System.Text;

namespace Rle;

public static class Encoder
{
    // Return run‑length encoding of `input`.
    public static string Encode(string input)
    {
        // TODO: implement
        //throw new NotImplementedException("Implement me!");
        List<Rune> current = new();
        string data = string.Empty;

        foreach (Rune rune in input.EnumerateRunes())
        {
            if (current.Count > 0 && current[0] != rune)
            {
                data += string.Concat(current[0].ToString(), current.Count);
                current.Clear();
            }
            current.Add(rune);
        }
        data += current.Any() ? string.Concat(current[0].ToString(), current.Count) : input;
        return data;
    }
}
