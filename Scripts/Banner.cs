using UnityEngine;
using System;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class Banner : MonoBehaviour {

	public static Banner Instance;

	private BannerView bannerView;
	private InterstitialAd interstitial;
	private float fTimeFromLastInterstilialAd;



	private void RequestBanner()
	{
		string adUnitId = "____________________________________________";
		
		bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);
		bannerView.AdLoaded += HandleAdLoaded;
		bannerView.AdFailedToLoad += HandleAdFailedToLoad;
		bannerView.AdOpened += HandleAdOpened;
		bannerView.AdClosing += HandleAdClosing;
		bannerView.AdClosed += HandleAdClosed;
		bannerView.AdLeftApplication += HandleAdLeftApplication;
		// Load a banner ad.
		bannerView.LoadAd(createAdRequest());

	}

	public void RequestInterstitial()
	{
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "____________________________________________";
		#elif UNITY_IPHONE
		string adUnitId = "____________________________________________";
		#else
		string adUnitId = "unexpected_platform";
		#endif
		
		// Create an interstitial.
		interstitial = new InterstitialAd(adUnitId);
		// Register for ad events.
		interstitial.AdLoaded += HandleInterstitialLoaded;
		interstitial.AdFailedToLoad += HandleInterstitialFailedToLoad;
		interstitial.AdOpened += HandleInterstitialOpened;
		interstitial.AdClosing += HandleInterstitialClosing;
		interstitial.AdClosed += HandleInterstitialClosed;
		interstitial.AdLeftApplication += HandleInterstitialLeftApplication;
		// Load an interstitial ad.
		interstitial.LoadAd(createAdRequest());
	}
	
	// Returns an ad request with custom ad targeting.
	private AdRequest createAdRequest()
	{
		return new AdRequest.Builder()
			.AddTestDevice(AdRequest.TestDeviceSimulator)
//				.AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
//				.AddKeyword("game")
//				.SetGender(Gender.Male)
//				.SetBirthday(new DateTime(1985, 1, 1))
//				.TagForChildDirectedTreatment(false)
//				.AddExtra("color_bg", "9B30FF")
				.Build();
	}

	public void ShowInterstitial()
	{
		if (Time.realtimeSinceStartup - fTimeFromLastInterstilialAd > 90.0f) {
			//print ("Show Interstitial @!!!!!!!!!!!!!!!!!!!!!!" + Time.realtimeSinceStartup + " " + fTimeFromLastInterstilialAd);
			if (interstitial.IsLoaded())
			{
				fTimeFromLastInterstilialAd = Time.realtimeSinceStartup;
				interstitial.Show();
			}
			else
			{
				print("Interstitial is not ready yet.");
			}
		}
	}

	#region Banner callback handlers
	
	public void HandleAdLoaded(object sender, EventArgs args)
	{
		print("HandleAdLoaded event received.");
	}
	
	public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		print("HandleFailedToReceiveAd event received with message: " + args.Message);
	}
	
	public void HandleAdOpened(object sender, EventArgs args)
	{
		print("HandleAdOpened event received");
	}
	
	void HandleAdClosing(object sender, EventArgs args)
	{
		print("HandleAdClosing event received");
	}
	
	public void HandleAdClosed(object sender, EventArgs args)
	{
		print("HandleAdClosed event received");
	}
	
	public void HandleAdLeftApplication(object sender, EventArgs args)
	{
		print("HandleAdLeftApplication event received");
	}
	
	#endregion
	
	#region Interstitial callback handlers
	
	public void HandleInterstitialLoaded(object sender, EventArgs args)
	{
		print("HandleInterstitialLoaded event received.");
	}
	
	public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		print("HandleInterstitialFailedToLoad event received with message: " + args.Message);
	}
	
	public void HandleInterstitialOpened(object sender, EventArgs args)
	{
		print("HandleInterstitialOpened event received");
	}
	
	void HandleInterstitialClosing(object sender, EventArgs args)
	{
		print("HandleInterstitialClosing event received");
	}
	
	public void HandleInterstitialClosed(object sender, EventArgs args)
	{
		print("HandleInterstitialClosed event received");
	}
	
	public void HandleInterstitialLeftApplication(object sender, EventArgs args)
	{
		print("HandleInterstitialLeftApplication event received");
	}
	
	#endregion

	void Awake ()
	{
		// First we check if there are any other instances conflicting
		if (Instance != null && Instance != this) {
			// If that is the case, we destroy other instances
			Destroy (gameObject);
			return;
		}
		
		DontDestroyOnLoad (this);
		
		// Here we save our singleton instance
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		RequestBanner ();
		RequestInterstitial ();
		Hide ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public void Show(){
		bannerView.Show();
	}

	public void Hide(){
		bannerView.Hide();
	}
}
