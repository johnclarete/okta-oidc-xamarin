﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace Okta.Xamarin
{
	/// <summary>
	/// Stores configuration for the Okta iOS OIDC client
	/// </summary>
	public partial class OktaConfig : IOktaConfig
	{
		// There might be some iOS-specific config in the future.  That would go here.


		/// <summary>
		/// Converts an iOS-typed NSDictionary into an OktaConfig, casting or parsing each field and then validating the resulting config.
		/// </summary>
		/// <param name="dict">The <see cref="NSDictionary"/> to convert from</param>
		/// <returns>An <see cref="OktaConfig"/> with config values filled and validated</returns>
		private static OktaConfig FromNSDictionary(NSDictionary dict)
		{
			OktaConfig config = new OktaConfig();

			try
			{
				if (dict.ContainsKey(new NSString("ClientId")))
				{
					config.ClientId = (dict["ClientId"] as NSString);
				}

				if (dict.ContainsKey(new NSString("Scope")))
				{
					config.Scope = (dict["Scope"] as NSString);
				}

				if (dict.ContainsKey(new NSString("OktaDomain")))
				{
					config.OktaDomain = (dict["OktaDomain"] as NSString);
				}

				if (dict.ContainsKey(new NSString("AuthorizationServerId")))
				{
					config.AuthorizationServerId = (dict["AuthorizationServerId"] as NSString);
				}

				if (dict.ContainsKey(new NSString("RedirectUri")))
				{
					config.RedirectUri = (dict["RedirectUri"] as NSString);
				}

				if (dict.ContainsKey(new NSString("PostLogoutRedirectUri")))
				{
					config.PostLogoutRedirectUri = (dict["PostLogoutRedirectUri"] as NSString);
				}

				if (dict.ContainsKey(new NSString("ClockSkew")))
				{
					config.ClockSkew = TimeSpan.FromSeconds((dict["ClockSkew"] as NSNumber).DoubleValue);
				}
			}
			catch (Exception ex)
			{
				throw new FormatException("The Okta Config PList could not be parsed.  Make sure values are the correct type/format.", ex);
			}

			OktaConfigValidator<OktaConfig> validator = new OktaConfigValidator<OktaConfig>();
			validator.Validate(config);

			return config;
		}



		/// <summary>
		/// Instantiates a <see cref="OktaConfig"/> from a property list file and validates it.  Throws an exception if required fields are missing or invalid.
		/// </summary>
		/// <param name="filename">The plist file to parse.  This is treated as a simple file path.</param>
		/// <returns>Returns the <see cref="OktaConfig"/> with fields filled from <paramref name="filename"/>.</returns>
		public static OktaConfig LoadFromPList(string filename)
		{
			NSDictionary dict = new NSDictionary(filename);
			if (dict == null)
			{
				throw new Exception("There was an error loading the specified plist file.  Ensure the filename is correct, the file build action is set to \"Content\", and it is set to copy to output directory.");
			}

			return FromNSDictionary(dict);
		}
	}
}