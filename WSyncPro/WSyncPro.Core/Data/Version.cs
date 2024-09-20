using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Core.Data
{
    internal static class Version
    {
        private static readonly int Major = 0;
        private static readonly int Minor = 0;
        private static readonly int Patch = 0;
        private static readonly int Build = 0;

        public static string GetVersion => $"{Major}.{Minor}.{Patch}b{Build}";
    }
}
