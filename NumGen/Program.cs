using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
class CLI_Handler(string[] args)
{
    private string[] Args { get; set; } = args;

    public (int bitLength, int count) HandleCLI()
    {
        int count = 1;

        // help check
        if (Args.Length > 0 && (Args[0] == "help" || Args[0] == "h"))
        {
            PrintBasicInfo();
            return (0, 0);
        }
        // argument check
        if (Args.Length < 2)
        {
            Console.WriteLine("Not enough arguments provided. Please provide <bits> <option> <count>.");
            return (0, 0);
        }
        if (Args.Length > 3)
        {
            Console.WriteLine("Too many arguments provided. Please provide only <bits> <option> <count>.");
            return (0, 0);
        }

        // option check
        if (Args[1] != "prime" && Args[1] != "odd")
        {
            Console.WriteLine("Wrong option inputted, only 'prime' or 'odd' allowed");
            return (0, 0);
        }
        if (Args.Length == 3)
        {
            if (int.TryParse(Args[2], out int number))
            {
                count = number;
            }
            else
            {
                Console.WriteLine("Invalid number format.");
                return (0, 0);
            }

        }

        // correct number of bits check
        if (int.TryParse(Args[0], out int bitLength))
        {
            if (bitLength < 32 || bitLength % 8 != 0)
            {
                Console.WriteLine("The bit count must be at least 32 and a multiple of 8");
                return (0, 0);
            }
        }
        else
        {
            Console.WriteLine("Invalid number format.");
            return (0, 0);
        }

        return (bitLength, count);
    }
    public static void PrintBasicInfo()
    {
        Console.WriteLine("Program Usage: dotnet run <bits: int> <option: prime|odd> <count: int>");
        Console.WriteLine("Bit count must be a multiple of 8 and at least 32");
        Console.WriteLine("Count represents the number of numbers to generate, defaults to 1");
    }
}

class NumGen
{
    /// <summary>
    /// Handles all number generation
    /// </summary>
    private static BigInteger RandomPositiveBigInteger(int byteCount)
    {
        byte[] randomBytes = new byte[byteCount + 1];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes, 0, byteCount);
        BigInteger result = new BigInteger(randomBytes) | 1;
        return result;
    }
    public static BigInteger GenerateBigInteger(int bitLength, string mode)
    {
        int byteCount = bitLength / 8;
        switch (mode)
        {
            case "prime":
                while (true)
                {
                    BigInteger candidate = RandomPositiveBigInteger(byteCount);
                    if (MillerRabin.IsProbablyPrime(candidate) == "probably prime") return candidate;
                }

            case "odd":
                return RandomPositiveBigInteger(byteCount);
            default:
                throw new ArgumentException("Invalid mode. Use 'prime' or 'odd'.");
        }
    }
    public static BigInteger GenerateBigInteger(int bitLength)
    {
        int byteCount = bitLength / 8;
        byte[] randomBytes = new byte[byteCount];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        if (byteCount > 0)
        {
            randomBytes[^1] &= 0x7F; // this clears the sign bit
        }

        BigInteger randomBigInt = new(randomBytes);
        return randomBigInt;
    }
}

class MillerRabin
{
    /// <summary>
    /// Functionality for Miller-Rabin Primality Test
    /// </summary>
    private static readonly int[] smallPrimes = [
        2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541
    ];
    private static BigInteger RandomBigInteger(BigInteger min, BigInteger max, Random rng)
    {
        byte[] bytes = max.ToByteArray();
        BigInteger result;
        do
        {
            rng.NextBytes(bytes);
            bytes[^1] &= 0x7F; // this clears the sign bit
            result = new BigInteger(bytes);
        } while (result < min || result > max);
        return result;
    }
    public static string IsProbablyPrime(BigInteger n, int k = 10)
    {
        // basic checks
        if (n == 2)
        {
            return "probably prime";
        }
        if (n == 3) {
            return "probably prime";
        }
        if (n % 2 == 0)
        {
            return "composite";
        }

        // quick check with small primes for performance
        foreach (int p in smallPrimes)
        {
            if (n == p) return "probably prime";
            if (n % p == 0) return "composite";
        }

        int s = 0;
        BigInteger d = n - 1;
        while (d % 2 == 0)
        {
            d /= 2;
            s += 1;
        }
        for (int i = 0; i < k; i++)
        {
            Random rng = new();
            BigInteger a = RandomBigInteger(2, n - 2, rng);

            BigInteger x = BigInteger.ModPow(a, d, n);

            for (int j = 0; j < s; j++)
            {
                BigInteger y = BigInteger.ModPow(x, 2, n);
                if (y == 1 && x != 1 && x != n - 1)
                {
                    return "composite";
                }
                x = y;
            }
            if (x != 1)
            {
                return "composite";
            }

        }
        return "probably prime";
    }
}


class Factorization
{
    /// <summary>
    /// Functionality for counting number of divisors
    /// </summary>
    public static int CountDivisors(BigInteger n)
    {
        Dictionary<BigInteger, int> primeCounts = [];

        for (BigInteger i = 2; i * i <= n; i++)
        {
            while (n % i == 0)
            {
                if (!primeCounts.ContainsKey(i)) 
                    primeCounts[i] = 0;
                primeCounts[i]++;
                n /= i;
            }
        }

        if (n > 1)
        {
            primeCounts[n] = 1;
        }

        int divisorCount = 1;
        foreach (var exp in primeCounts.Values)
        {
            divisorCount *= exp + 1;
        }

        return divisorCount;
    }
}


class Program
{
    public static void Main(string[] args)
    {
        int count = 1;
        int bitLength = 0;

        (bitLength, count) = new CLI_Handler(args).HandleCLI();
        if (bitLength == 0 && count == 0)
        {
            return;
        }
        // successful argument parsing
        // assume from here on out that args are valid
        Console.WriteLine($"BitLength: {bitLength} bits");


        BigInteger[] results = new BigInteger[count];
        int[] factors = new int[count];

        // main generation
        Stopwatch stopwatch = Stopwatch.StartNew();
        Parallel.For(0, count, i =>
        {
            results[i] = NumGen.GenerateBigInteger(bitLength, args[1]);
            if (args[1] == "odd") {
                factors[i] = Factorization.CountDivisors(results[i]);
            }
        });

        stopwatch.Stop();

        // output
        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"{i + 1}: {results[i]}");
            if (args[1] == "odd") {
                Console.WriteLine($"Number of factors: {factors[i]}");
            }
        }
        Console.WriteLine($"Time to Generate: {stopwatch.Elapsed}");
        return;
    }
}