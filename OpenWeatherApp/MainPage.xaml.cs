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
            GetWheather(_broadcast);
            GetCurrentLocation();
        }

        public async void GetWheather(object parameters)
        {
            string response = await _broadcast.GetStringAsync(new Uri("https://api.openweathermap.org/data/2.5/weather?lat=44.34&lon=10.99&appid=0bf31966051443f8bd4d70bfd5f3e356&units=metric"));

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
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (Exception ex)
            {
                // Unable to get location
            }
            finally
            {
                _isCheckingLocation = false;
            }
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
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
