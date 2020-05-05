using System;
using System.Linq;

namespace ENBManager.TestUtils.Utils
{
    public static class TestValues
    {
        private static Random random = new Random();

        public static string GetRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ0123456789!?&%=_-éèáà";
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
