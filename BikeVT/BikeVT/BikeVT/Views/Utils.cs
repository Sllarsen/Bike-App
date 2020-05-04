// Used for ensuring/requesting device (location) permissions 
// File created by https://github.com/jamesmontemagno/PermissionsPlugin/blob/master/samples/PermissionsSample/PermissionsSample/Utils.cs
// unless otherwise indicated.
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BikeVT.Views
{
	public static class Utils
	{
		public static async Task<PermissionStatus> CheckPermissions(BasePermission permission)
		{
			var permissionStatus = await permission.CheckPermissionStatusAsync();
			bool request = false;
			if (permissionStatus == PermissionStatus.Denied)
			{
				if (Device.RuntimePlatform == Device.iOS)
				{

					// The following lines have been added
					// `permission.ToString()` Starts as "Plugin.Permissions.LocationPermission", trim to "LocationPermission"
					var permissionString = permission.ToString().Split('.')[2];
					permissionString = permissionString.Substring(0, permissionString.Length - 10);
					// The above lines have been added

					// The following 4 lines have been edited
					var title = $"{permissionString} permissions required";
					var question = $"Please open Settings and allow {permissionString}.";
					var positive = "Open Settings";
					var negative = "Deny permission";
					// The above 4 lines have been edited
					
					var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
					if (task == null)
						return permissionStatus;

					var result = await task;
					if (result)
					{
						CrossPermissions.Current.OpenAppSettings();
					}

					return permissionStatus;
				}

				request = true;

			}

			if (request || permissionStatus != PermissionStatus.Granted)
			{
				permissionStatus = await permission.RequestPermissionAsync();


				if (permissionStatus != PermissionStatus.Granted)
				{
					// The following lines have been added
					// `permission.ToString()` Starts as "Plugin.Permissions.LocationPermission", trim to "LocationPermission"
					var permissionString = permission.ToString().Split('.')[2];
					permissionString = permissionString.Substring(0, permissionString.Length - 10);
					// The above lines have been added

					// The following 4 lines have been edited
					var title = $"{permissionString} permissions required";
					var question = $"Please open Settings and allow {permissionString}.";
					var positive = "Open Settings";
					var negative = "Deny permission";
					// The above 4 lines have been edited

					var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
					if (task == null)
						return permissionStatus;

					var result = await task;
					if (result)
					{
						CrossPermissions.Current.OpenAppSettings();
					}
					return permissionStatus;
				}
			}

			return permissionStatus;
		}
	}
}
