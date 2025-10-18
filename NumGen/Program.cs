using System.Data.SqlTypes;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

class CLI_Handler
{
    public static void HandleCLI()
    {
        Console.WriteLine("Program Usage: dotnet run <bits> <option> <count>");
        Console.WriteLine("Note that the bit count must be a multiple of 8 and at least 32");
        Console.WriteLine("'Option': 'prime' or 'odd'");
        Console.WriteLine("Count represents the number of numbers to generate, defaults to 1");
        while (true)
        {
            try
            {
                string? input = Console.ReadLine();
                if (input == null)
                {
                    Console.WriteLine("No input received. Please enter a valid command.");
                    continue;
                }
                string[] inputs = input.Split(' ');
                string bits = inputs[0];
                string option = inputs[1];
                if (option != "prime" && option != "odd")
                {
                    Console.WriteLine("Wrong option inputted, only 'prime' or 'odd' allowed");
                    continue;
                }
                string count = inputs[2];
                Console.WriteLine($"BitLength: {bits} bits");
                byte[] utf8bytes = System.Text.Encoding.UTF8.GetBytes(bits);
                BigInteger n = new(utf8bytes);
                Console.WriteLine($"Number: {n}");
                var x = MillerRabin.MillerRabinTest(n);
                Console.WriteLine($"Miller-Rabin test result: {x}");

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
        BigInteger d = n - 1;
        while (d % 2 == 0)
        {
            d /= 2;
            s += 1;
        }
        // Console.WriteLine($"s: {s}, d: {d}");
        BigInteger y = 0;
        for (int i = 0; i < k; i++)
        {
            var a = RandomNumberGenerator.GetInt32(2, (int)(n - 2));
            // this is the only int in this code that could overflow
            // TODO: fix this
            BigInteger x = BigInteger.ModPow(a, d, n);
            for (int j = 0; j < s; j++)
            {
                y = BigInteger.ModPow(x, 2, n);
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
        CLI_Handler.HandleCLI();
        // string x = MillerRabin.MillerRabinTest(10007);
        // Console.WriteLine($"result: {x}");
    }
}