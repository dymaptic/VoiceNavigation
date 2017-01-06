using System;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;

namespace VoiceNavigation.ViewModels
{
	public class PrimaryMapViewModel : MoravecLabs.Infrastructure.BaseViewModel
	{
		public PrimaryMapViewModel()
		{
		}

		public async void ComputeDirections(GeocodeResult destination)
		{
			var routeUrl = "https://route.arcgis.com/arcgis/rest/services/World/Route/NAServer";
			//var routeUrl = "https://utility.arcgis.com/usrsvcs/appservices/BOgaSXpsgFKnF6kY/rest/services/World/Route/GPServer";
			//var cred = new Esri.ArcGISRuntime.Security.OAuthTokenCredential();
			var cred = await AuthenticationManager.Current.GenerateCredentialAsync(new Uri(routeUrl), "MoravecLabs", "g0%9OBp@21io");
			var routeTask = await RouteTask.CreateAsync(new Uri(routeUrl), cred);
		}
	}
}
