using System;

namespace WSyncPro.Util.System
{
    public static class EnvGrabber
    {
        /// <summary>
        /// Grabs the value of an environment variable as a string.
        /// </summary>
        /// <param name="envVarName">The name of the environment variable.</param>
        /// <returns>The value of the environment variable, or null if not set.</returns>
        public static string Grab(string envVarName)
        {
            if (string.IsNullOrWhiteSpace(envVarName))
                throw new ArgumentException("Environment variable name cannot be null or empty.", nameof(envVarName));

            return Environment.GetEnvironmentVariable(envVarName);
        }

        /// <summary>
        /// Grabs the value of an environment variable as an integer if possible.
        /// </summary>
        /// <param name="envVarName">The name of the environment variable.</param>
        /// <param name="isInt">A flag indicating the caller expects an integer value.</param>
        /// <returns>The integer value of the environment variable if parsable, or null if not set or not parsable.</returns>
        public static int? Grab(string envVarName, bool isInt)
        {
            if (string.IsNullOrWhiteSpace(envVarName))
                throw new ArgumentException("Environment variable name cannot be null or empty.", nameof(envVarName));

            var value = Environment.GetEnvironmentVariable(envVarName);

            if (isInt && int.TryParse(value, out var result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Checks if an environment variable is set.
        /// </summary>
        /// <param name="envVarName">The name of the environment variable.</param>
        /// <returns>True if the environment variable is set; otherwise, false.</returns>
        public static bool IsSet(string envVarName)
        {
            if (string.IsNullOrWhiteSpace(envVarName))
                throw new ArgumentException("Environment variable name cannot be null or empty.", nameof(envVarName));

            return Environment.GetEnvironmentVariable(envVarName) != null;
        }
    }
}
