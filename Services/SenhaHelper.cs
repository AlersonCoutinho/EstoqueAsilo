using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EstoqueAsilo.Services
{
    internal class SenhaHelper
    {
        private const int TamanhoSalt = 16;
        private const int TamanhoHash = 32;
        private const int Iteracoes = 100_000;

        public static (string hash, string salt) Gerar(string senha)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                senha, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);

            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        public static bool Verificar(string senha, string hashSalvo, string saltSalvo)
        {
            byte[] salt = Convert.FromBase64String(saltSalvo);
            byte[] esperado = Convert.FromBase64String(hashSalvo);
            byte[] calculado = Rfc2898DeriveBytes.Pbkdf2(
                senha, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);

            return CryptographicOperations.FixedTimeEquals(calculado, esperado);
        }
    }
}
