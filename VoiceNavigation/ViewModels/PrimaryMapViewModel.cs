// <copyright file="PrimarymapViewModel.cs" company="Moravec Labs, LLC">
//     MIT License
//
//     Copyright (c) Moravec Labs, LLC.
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// </copyright>

namespace VoiceNavigation.ViewModels
{
    using System;
    using Esri.ArcGISRuntime.Mapping;
    using Esri.ArcGISRuntime.Security;
    using Esri.ArcGISRuntime.Tasks.Geocoding;
    using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
    using MoravecLabs.Atom;

    /// <summary>
    /// Primary map view model.
    /// </summary>
    public class PrimaryMapViewModel : MoravecLabs.Infrastructure.BaseViewModel
    {
        /// <summary>
        /// The map.
        /// </summary>
        public Atom<Map> Map { get; set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VoiceNavigation.ViewModels.PrimaryMapViewModel"/> class.
        /// </summary>
        public PrimaryMapViewModel()
        {
            this.Map = new Atom<Map>(null, this, nameof(this.Map));
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        public void Initialize()
        {
            this.Map.Value = MapUtils.MakeDefaultMap();
        }

        /// <summary>
        /// Computes the directions.
        /// </summary>
        /// <param name="destination">Destination.</param>
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
