using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography;

class CLI_Handler
{
    public static void HandleCLI()
    {
        while (true)
        {
            Console.WriteLine("Program Usage: dotnet run <bits> <option> <count>");
            try
            {
                string? input = Console.ReadLine();
                if (input == null)
                {
                    Console.WriteLine("No input received. Please enter a valid command.");
                    continue;
                }
                string[] inputs = input.Split(' ');
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}

class MillerRabin
{
    public static string MillerRabinTest(BigInteger n, int k = 10)
    {
        if (n <= 2)
        {
            return "false";
        }
        int s = 0;
        int d = (int)(n - 1);
        while (d % 2 == 0)
        {
            d /= 2;
            s += 1;
        }
        Console.WriteLine($"s: {s}, d: {d}");
        BigInteger y = 0;
        for (int i = 0; i < k; i++)
        {
            var a = RandomNumberGenerator.GetInt32(2, (int)(n - 2));
            BigInteger x = BigInteger.ModPow(a, d, n);
            for (int j = 0; j < s; j++)
            {
                y = (int)BigInteger.ModPow(x, 2, n);
                if (y == 1 && x != 1 && x != (int)(n - 1))
                {
                    return "composite";
                }
                x = y;
            }
            if (y != 1)
            {
                return "composite";
            }

        }
        return "probably prime";
    }
}

class Program
{
    public static void Main(string[] args)
    {
        // CLI_Handler.HandleCLI();
        string x = MillerRabin.MillerRabinTest(17);
        Console.WriteLine($"result: {x}");
    }
}