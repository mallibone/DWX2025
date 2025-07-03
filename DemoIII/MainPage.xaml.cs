using System.Diagnostics;

namespace DemoIII;

public partial class MainPage : ContentPage
{

	IBleService _bleService = new BleService();
	// IBleService _bleService = new FakeBleService();
	public MainPage()
	{
		InitializeComponent();

		_bleService.HeartRateValue += (sender, heartRate) =>
		{
			MainThread.BeginInvokeOnMainThread(() => { hybridWebView.SendRawMessage($"{heartRate}"); });
			// MainThread.BeginInvokeOnMainThread( async () => { await hybridWebView.InvokeJavaScriptAsync<int>("updateHeartRate", HeartRateJsContext.Default.Int32, [heartRate], [HeartRateJsContext.Default.Int32]); });
			Debug.WriteLine(heartRate);
		};

		if (_bleService is FakeBleService fakeBleService)
		{
			slider.Value = 83;
			slider.ValueChanged += (sender, e) =>
			{
				// hybridWebView.SendRawMessage($"{Math.Round(e.NewValue)}");
				// Debug.WriteLine(Math.Round(e.NewValue));
				fakeBleService.BaseRate = (int)Math.Round(e.NewValue);
			};
		}
		else
		{
			slider.IsVisible = false;
		}
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		ActivityIndicator.IsRunning = true;
		ActivityIndicator.IsVisible = true;
		await _bleService.ConnectToPolar();
		ActivityIndicator.IsRunning = false;
		ActivityIndicator.IsVisible = false;
		if(slider.IsVisible) hybridWebView.SendRawMessage($"{slider.Value}");
	}
}
