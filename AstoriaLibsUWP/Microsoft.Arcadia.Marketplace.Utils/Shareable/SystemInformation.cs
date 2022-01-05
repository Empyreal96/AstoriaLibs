using System;
using System.Globalization;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Utils.Shareable
{
	public class SystemInformation : ISystemInformation
	{
		public SystemArchitecture Architecture
		{
			get
			{
				string text = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").ToUpper(); //(CultureInfo.InvariantCulture)
				if (text.Contains("ARM"))
				{
					return SystemArchitecture.Arm;
				}
				if (text.Contains("X86"))
				{
					return SystemArchitecture.X86;
				}
				if (text.Contains("AMD64"))
				{
					return SystemArchitecture.X64;
				}
				return SystemArchitecture.Other;
			}
		}
	}
}