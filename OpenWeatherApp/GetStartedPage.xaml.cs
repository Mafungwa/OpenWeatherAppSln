namespace OpenWeatherApp;

public partial class GetStartedPage : ContentPage
{
	public GetStartedPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync($"MainPage");
    }
}