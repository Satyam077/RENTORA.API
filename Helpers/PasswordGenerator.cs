using System.Security.Cryptography;
using System.Text;

namespace RENTORA.API.Helpers
{
    public static class PasswordGenerator
    {
        public static string GenerateSecurePassword(int length = 12, bool includeSpecialChars = true)
        {
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*";

            string validChars = lowercase + uppercase + digits;
            if (includeSpecialChars)
            {
                validChars += specialChars;
            }

            StringBuilder password = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                // Ensure at least one character from each category
                password.Append(lowercase[randomBytes[0] % lowercase.Length]);
                password.Append(uppercase[randomBytes[1] % uppercase.Length]);
                password.Append(digits[randomBytes[2] % digits.Length]);
                
                if (includeSpecialChars)
                {
                    password.Append(specialChars[randomBytes[3] % specialChars.Length]);
                }

                // Fill the rest with random characters
                int startIndex = includeSpecialChars ? 4 : 3;
                for (int i = startIndex; i < length; i++)
                {
                    password.Append(validChars[randomBytes[i] % validChars.Length]);
                }
            }

            // Shuffle the password to randomize character positions
            return ShuffleString(password.ToString());
        }

        private static string ShuffleString(string input)
        {
            char[] array = input.ToCharArray();
            using (var rng = RandomNumberGenerator.Create())
            {
                int n = array.Length;
                while (n > 1)
                {
                    byte[] box = new byte[1];
                    do rng.GetBytes(box);
                    while (!(box[0] < n * (byte.MaxValue / n)));
                    int k = box[0] % n;
                    n--;
                    char temp = array[k];
                    array[k] = array[n];
                    array[n] = temp;
                }
            }
            return new string(array);
        }
    }
}
