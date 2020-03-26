using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//For Binding:
using System.Runtime.CompilerServices;
using System.Windows.Input;


using Xamarin.Essentials;

/**
 * some code from 
 * https://github.com/jamesmontemagno/app-essentials/tree/master/AppEssentials.Shared/Pages
 */
namespace BikeVT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();
            GetPositionCommand = new Command(async () => await OnGetPosition());
            BindingContext = this;  // Allows you to use "{Binding VAR}" in .xaml
        }

        string destination;
        string coordMsg;
        double latitude;
        double longitude;

        public ICommand GetPositionCommand { get; }


        public String Destination
        {
            get => destination;
            set => SetProperty(ref destination, value);

        }
        public String CoordMsg
        {
            get => coordMsg;
            set => SetProperty(ref coordMsg, value);

        }

        /**
         * Code from https://github.com/jamesmontemagno/app-essentials/tree/master/AppEssentials.Shared/Pages
         * but modified
         */
        async Task OnGetPosition()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                var locations = await Geocoding.GetLocationsAsync(Destination);
                Location location = locations.FirstOrDefault();
                if (location == null)
                {
                    CoordMsg = "Unable to detect locations";
                    //TODO: Check if they just typed in coordinates
                }
                else
                {
                    latitude = location.Latitude;
                    longitude = location.Longitude;
                    CoordMsg = "(" + latitude + ", " + longitude + ")";
                }
            }
            catch (Exception ex)
            {
                CoordMsg = $"Unable to detect locations: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }


        protected virtual bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "", Action onChanged = null, Func<T, T, bool> validateValue = null)
        {
            // Copied from https://github.com/jamesmontemagno/app-essentials/tree/master/AppEssentials.Shared/Pages
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            if (validateValue != null && !validateValue(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
