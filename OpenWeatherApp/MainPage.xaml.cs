using Newtonsoft.Json;
using static OpenWeatherApp.Models.WeatherClass;

namespace OpenWeatherApp
{
    public partial class MainPage : ContentPage
    {
        private float _wheather;
        private float _wind;
        private int _cloud;
        private int _humidity;
        private int _currentlocation;

        private Location _gpsLocation;

        public float WheatherToday
        {
            get { return _wheather; }
            set
            {
                _wheather = value;
                OnPropertyChanged();
            }
        }

        public float Wind
        {
            get { return _wind; }
            set
            {
                _wind = value;
                OnPropertyChanged();
            }
        }

        public int Clouds
        {
            get { return _cloud; }
            set
            {
                _cloud = value;
                OnPropertyChanged();

            }
        }

        public string CurrentLocation { get; private set; }

        public int Humidity
        {
            get { return _humidity; }
            set
            {
                _humidity = value;
                OnPropertyChanged();
            }
        }

        private HttpClient _broadcast;
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            _broadcast = new HttpClient();
            _broadcast.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await GetCurrentLocation();
            GetWheather(_broadcast);

        }

        public async void GetWheather(object parameters)
        {
            string response = await _broadcast.GetStringAsync(new Uri($"https://api.openweathermap.org/data/2.5/weather?lat={_gpsLocation.Latitude}&lon={_gpsLocation.Longitude}&appid=0bf31966051443f8bd4d70bfd5f3e356&units=metric"));

            Rootobject todayWheather = JsonConvert.DeserializeObject<Rootobject>(response);

            if (todayWheather != null)
            {

                WheatherToday = todayWheather.main.temp;
                Wind = todayWheather.wind.speed;
                Humidity = todayWheather.main.humidity;
                Clouds = todayWheather.clouds.all;
            }
        }
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;

        public async Task GetCurrentLocation()
        {
            try
            {
                _gpsLocation = await Geolocation.Default.GetLocationAsync();

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        public async void GetWeatherForCity(string city)
        {
            try
            {
                HttpResponseMessage response = await _broadcast.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={city}&appid=0bf31966051443f8bd4d70bfd5f3e356&units=metric");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Rootobject todayWheather = JsonConvert.DeserializeObject<Rootobject>(responseBody);

                if (todayWheather != null)
                {

                    WheatherToday = todayWheather.main.temp;
                    Wind = todayWheather.wind.speed;
                    Humidity = todayWheather.main.humidity;
                    Clouds = todayWheather.clouds.all;
                    CurrentLocation = $"City: {todayWheather.name}, Country: {todayWheather.sys.country}";
                }
            
                
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error getting weather data: {ex.Message}");
            }
        }



    }

}
