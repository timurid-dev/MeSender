﻿using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace MeSender.Identity.Services;

internal static class PasswordService
{
    public static (string Hash, string Salt) HashPassword(string password)
    {
        var saltBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        var salt = Convert.ToBase64String(saltBytes);
        using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(password)))
        {
            hasher.Salt = saltBytes;
            hasher.DegreeOfParallelism = 8; // Количество потоков
            hasher.MemorySize = 65536; // 64 МБ
            hasher.Iterations = 4; // Количество итераций

            var hashBytes = hasher.GetBytes(32); // 32 байта хеша
            var hash = Convert.ToBase64String(hashBytes);
            return (hash, salt);
        }
    }

    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        using var hasher = new Argon2id(Encoding.UTF8.GetBytes(password));
        hasher.Salt = saltBytes;
        hasher.DegreeOfParallelism = 8;
        hasher.MemorySize = 65536;
        hasher.Iterations = 4;

        var hashBytes = hasher.GetBytes(32);
        var hash = Convert.ToBase64String(hashBytes);
        return hash == storedHash;
    }
}
