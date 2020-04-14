using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BloomHarvester
{
	public enum EnvironmentSetting
	{
		Unknown,
		Default,
		Local,
		Dev,
		Test,
		Prod
	}

	internal class EnvironmentUtils
	{
		/// <summary>
		/// Returns a non-zero (aka non-Default) value of Environment to use, based on the higher-precedence value and the fallback value.
		/// </summary>
		/// <param name="resourceEnv">The value with higher precedence</param>
		/// <param name="fallbackEnv">The value to use if the higher precedence is not set to a non-Default value</param>
		/// <returns></returns>
		internal static EnvironmentSetting GetEnvOrFallback(EnvironmentSetting resourceEnv, EnvironmentSetting fallbackEnv)
		{
			EnvironmentSetting parsedEnv = resourceEnv;

			if (resourceEnv != EnvironmentSetting.Unknown && resourceEnv != EnvironmentSetting.Default)
			{
				// Individual resource's environment was specified. Lets use it directly.
				parsedEnv = resourceEnv;
			}
			else if (fallbackEnv != EnvironmentSetting.Unknown && fallbackEnv != EnvironmentSetting.Default)
			{
				// Resource environment not specified, but general environment parameter was.
				// Fallback to general environment parameter
				parsedEnv = fallbackEnv;
			}
			else
			{
				// Neither one specified. Set it to Dev by default because less cost if something goes wrong.
				parsedEnv = EnvironmentSetting.Dev;
			}

			// Verify Postcondition: Should not return Environment.Default
			Debug.Assert(parsedEnv != EnvironmentSetting.Unknown && parsedEnv != EnvironmentSetting.Default, "GetEnvironment should determine a specific, non-default value of Environment");

			return parsedEnv;
		}
	}
}
