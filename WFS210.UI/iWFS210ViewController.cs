using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using WFS210;
using WFS210.IO;
using System.Timers;

namespace WFS210.UI
{
	partial class iWFS210ViewController : UIViewController
	{
		Oscilloscope wfs210;
		Service service;
		SettingsViewController settingsViewController;
		MeasurementsViewController measurementsViewController;
		SignalMeasurement[] signalMeasurements = new SignalMeasurement[2];
		MarkerMeasurement[] markerMeasurements = new MarkerMeasurement[2];

		public iWFS210ViewController (IntPtr handle) : base (handle)
		{
			wfs210 = new Oscilloscope ();
			service = new DemoService (wfs210);
			service.SettingsChanged += SettingsChanged;
			signalMeasurements [0] = new SignalMeasurement (){ Channel = 0, SelectedUnit = SignalUnit.Vdc };
			signalMeasurements [1] = new SignalMeasurement (){ Channel = 1,  SelectedUnit = SignalUnit.Vdc };
			markerMeasurements [0] = new MarkerMeasurement (){ Channel = 0,  SelectedUnit = MarkerUnit.dt };
			markerMeasurements [1] = new MarkerMeasurement (){ Channel = 1,  SelectedUnit = MarkerUnit.dt };
			Timer timer = new Timer (500);
			timer.Elapsed += (object sender, ElapsedEventArgs e) => {
				service.Update ();
				ScopeView.UpdateScopeView();
			};
			timer.AutoReset = true;
			timer.Enabled = true;
			timer.Start ();
		}

		void SettingsChanged (object sender, EventArgs e)
		{
			UpdateScopeControls ();

		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			MainView.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("BACKGROUND/BG-0x0.png"));
			ScopeView.Wfs210 = wfs210;
			ScopeView.SelectedChannel = wfs210.Channels [0];
			//wfs210.Channels [0].GenerateTestSignal ();

			ScopeView.NewData += (object sender, NewDataEventArgs e) => {
				UpdateScopeControls ();
			};
		}

		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

		}

		#endregion

		#region Events Channel1

		partial void btnSelectChannel1_TouchUpInside (UIButton sender)
		{
			ScopeView.SelectedChannel = wfs210.Channels [0];
			UpdateScopeControls();
		}

		partial void btnAC1_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [0].InputCoupling = InputCoupling.AC;
			service.ApplySettings();
		}

		partial void btnDC1_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [0].InputCoupling = InputCoupling.DC;
			service.ApplySettings();
		}

		partial void btnGND1_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [0].InputCoupling = InputCoupling.GND;
			service.ApplySettings();
		}

		partial void btnProbe1_TouchUpInside (UIButton sender)
		{
			if (wfs210.Channels [0].AttenuationFactor == AttenuationFactor.X10)
				wfs210.Channels [0].AttenuationFactor = AttenuationFactor.X1;
			else
				wfs210.Channels [0].AttenuationFactor = AttenuationFactor.X10;
			service.ApplySettings();
		}

		partial void btnVoltDown1_TouchUpInside (UIButton sender)
		{
			if (wfs210.Channels [0].VoltsPerDivision != VoltsPerDivision.Vdiv5mV)
				wfs210.Channels [0].VoltsPerDivision = wfs210.Channels [0].VoltsPerDivision + 1;
			service.ApplySettings();
		}

		partial void btnVoltUp1_TouchUpInside (UIButton sender)
		{
			if (wfs210.Channels [0].VoltsPerDivision != VoltsPerDivision.VdivNone)
				wfs210.Channels [0].VoltsPerDivision = wfs210.Channels [0].VoltsPerDivision - 1;
			service.ApplySettings();
		}

		partial void btnMarkerMeasurements_TouchUpInside (UIButton sender)
		{
			//throw new NotImplementedException();
			measurementsViewController = this.Storyboard.InstantiateViewController ("MeasurementsViewController") as MeasurementsViewController;
			// You need to specify the controller you are presenting 
			measurementsViewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			measurementsViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
			MeasurementsViewController.isMarkerMeasurement = true;
			MeasurementsViewController.SelectedChannel = 0;
			MeasurementsViewController.SelectedMeasurement = markerMeasurements [0].SelectedUnit.ToString ("G"); //gives the name back not the index
			PresentViewController (measurementsViewController, true, null);
		}

		partial void btnSignalMeasurements_TouchUpInside (UIButton sender)
		{
			//throw new NotImplementedException();
			measurementsViewController = this.Storyboard.InstantiateViewController ("MeasurementsViewController") as MeasurementsViewController;
			// You need to specify the controller you are presenting 
			measurementsViewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			measurementsViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
			MeasurementsViewController.isMarkerMeasurement = false;
			PresentViewController (measurementsViewController, true, null);
		}

		#endregion

		#region Events Trigger

		partial void btnTriggerCH1_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Channel = 0;
			service.ApplySettings();
		}

		partial void btnTriggerCH2_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Channel = 1;
			service.ApplySettings();
		}

		partial void btnTriggerSlopeUp_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Slope = TriggerSlope.Rising;
			service.ApplySettings(); 
		}

		partial void btnTriggerSlopeDown_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Slope = TriggerSlope.Falling;
			service.ApplySettings();  
		}

		partial void btnTriggerRun_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Mode = TriggerMode.Run;
			service.ApplySettings();  
		}

		partial void btnTriggerNrml_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Mode = TriggerMode.Normal;
			service.ApplySettings();  
		}

		partial void btnTriggerOnce_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Mode = TriggerMode.Once;
			service.ApplySettings();  
		}

		partial void btnTriggerHold_TouchUpInside (UIButton sender)
		{
			if (wfs210.Hold)
				wfs210.Hold = false;
			else
				wfs210.Hold = true;
			service.ApplySettings();  
		}

		partial void btnTimeLeft_TouchUpInside (UIButton sender)
		{
			if (wfs210.TimeBase != TimeBase.Tdiv1us)
				wfs210.TimeBase = wfs210.TimeBase - 1;
			service.ApplySettings();  
		}

		partial void btnTimeRight_TouchUpInside (UIButton sender)
		{
			if (wfs210.TimeBase != TimeBase.Tdiv1s)
				wfs210.TimeBase = wfs210.TimeBase + 1;
			service.ApplySettings();  
		}

		partial void btnAutorange_TouchUpInside (UIButton sender)
		{
			if (wfs210.AutoRange)
				wfs210.AutoRange = false;
			else
				wfs210.AutoRange = true;
			service.ApplySettings();  
		}

		#endregion

		#region Events Channel2

		partial void btnSelectChannel2_TouchUpInside (UIButton sender)
		{
			ScopeView.SelectedChannel = wfs210.Channels [1];
			UpdateScopeControls();
		}

		partial void btnAC2_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [1].InputCoupling = InputCoupling.AC;
			service.ApplySettings();  
		}

		partial void btnDC2_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [1].InputCoupling = InputCoupling.DC;
			service.ApplySettings();  
		}

		partial void btnGND2_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [1].InputCoupling = InputCoupling.GND;
			service.ApplySettings();  
		}

		partial void btnProbe2_TouchUpInside (UIButton sender)
		{
			if (wfs210.Channels [1].AttenuationFactor == AttenuationFactor.X10)
				wfs210.Channels [1].AttenuationFactor = AttenuationFactor.X1;
			else
				wfs210.Channels [1].AttenuationFactor = AttenuationFactor.X10;
			service.ApplySettings();  
		}

		partial void btnVoltDown2_TouchUpInside (UIButton sender)
		{
			if (wfs210.Channels [1].VoltsPerDivision != VoltsPerDivision.Vdiv5mV)
				wfs210.Channels [1].VoltsPerDivision = wfs210.Channels [1].VoltsPerDivision + 1;
			service.ApplySettings();  
		}

		partial void btnVoltUp2_TouchUpInside (UIButton sender)
		{
			if (wfs210.Channels [1].VoltsPerDivision != VoltsPerDivision.VdivNone)
				wfs210.Channels [1].VoltsPerDivision = wfs210.Channels [1].VoltsPerDivision - 1;
			service.ApplySettings();  
		}


		partial void btnMarkerMeasurements2_TouchUpInside (UIButton sender)
		{

			measurementsViewController = this.Storyboard.InstantiateViewController ("MeasurementsViewController") as MeasurementsViewController;
			measurementsViewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			measurementsViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
			MeasurementsViewController.isMarkerMeasurement = true;
			MeasurementsViewController.SelectedChannel = 1;
			MeasurementsViewController.SelectedMeasurement = markerMeasurements [1].SelectedUnit.ToString ("G");
			PresentViewController (measurementsViewController, true, null);
		}

		partial void btnSignalMeasurements2_TouchUpInside (UIButton sender)
		{
			measurementsViewController = this.Storyboard.InstantiateViewController ("MeasurementsViewController") as MeasurementsViewController;
			// You need to specify the controller you are presenting 
			measurementsViewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			measurementsViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
			MeasurementsViewController.isMarkerMeasurement = false;
			MeasurementsViewController.SelectedChannel = 1;
			PresentViewController (measurementsViewController, true, null);
		}

		#endregion

		#region Events Settings

		partial void btnSettings_TouchUpInside (UIButton sender)
		{
			//throw new NotImplementedException();
			settingsViewController = this.Storyboard.InstantiateViewController ("SettingsViewController") as SettingsViewController;
			// You need to specify the controller you are presenting 
			settingsViewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			settingsViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
			PresentViewController (settingsViewController, true, null);
		}

		public void DismissSettingsViewController ()
		{

			settingsViewController.DismissViewController (true, null);
		}

		public void DismissMeasurementsViewController ()
		{
			var test = MarkerUnit.dt;
			if (MeasurementsViewController.SelectedMeasurement != "Enable/Disable Markers")
				test = (MarkerUnit)Enum.Parse (typeof(MarkerUnit), MeasurementsViewController.SelectedMeasurement, true);
			else
				test = MarkerUnit.EnableDisableMarkers; 

			if (MeasurementsViewController.isMarkerMeasurement)
				markerMeasurements [MeasurementsViewController.SelectedChannel].SelectedUnit = test;
			else
				signalMeasurements [MeasurementsViewController.SelectedChannel].SelectedUnit = (SignalUnit)Enum.Parse (typeof(SignalUnit), MeasurementsViewController.SelectedMeasurement, true);

			measurementsViewController.DismissViewController (true, null);
		}

		#endregion


		private void UpdateScopeControls ()
		{
			UpdateSelectedChannel ();
			UpdateChannel1UI ();
			UpdateChannel2UI ();
			UpdateTriggerUI ();
			UpdateMeasurement1UI ();
			UpdateMeasurement2UI ();
		}


		void UpdateChannel1UI ()
		{
			UpdateInputCoupling1 ();
			UpdateAttenuationFactor1 ();
			UpdateVoltText1 ();
		}

		void UpdateChannel2UI ()
		{
			UpdateInputCoupling2 ();
			UpdateAttenuationFactor2 ();
			UpdateVoltText2 ();
		}

		void UpdateTriggerUI ()
		{
			UpdateTriggerChannelUI ();
			UpdateTriggerSlopeUI ();
			UpdateTriggerModeUI ();
			UpdateHoldUI ();
			UpdateAutorangeUI ();
			UpdateTimeBaseText ();
		}

		void UpdateMeasurement1UI ()
		{
			UpdateMarkerMeasurement1 ();
			UpdateSignalMeasurement1 ();
		}

		void UpdateMeasurement2UI ()
		{
			UpdateMarkerMeasurement2 ();
			UpdateSignalMeasurement2 ();
		}

		void UpdateSelectedChannel ()
		{
			if (ScopeView.SelectedChannel == wfs210.Channels [0]) {
				btnSelectChannel1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-ON-6x6.png"), UIControlState.Normal);
				btnSelectChannel2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-OFF-6x710.png"), UIControlState.Normal);
			} else {
				btnSelectChannel1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-OFF-6x6.png"), UIControlState.Normal);
				btnSelectChannel2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-ON-6x710.png"), UIControlState.Normal);
			}
		}

		void UpdateInputCoupling1 ()
		{
			switch (wfs210.Channels [0].InputCoupling) {
			case InputCoupling.AC:
				btnAC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-AC-ON-129x6.png"), UIControlState.Normal);
				btnDC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-DC-OFF-196x6.png"), UIControlState.Normal);
				btnGND1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-GND-OFF-263x6.png"), UIControlState.Normal);
				break;
			case InputCoupling.DC:
				btnAC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-AC-OFF-129x6.png"), UIControlState.Normal);
				btnDC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-DC-ON-196x6.png"), UIControlState.Normal);
				btnGND1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-GND-OFF-263x6.png"), UIControlState.Normal);
				break;
			case InputCoupling.GND:
				btnAC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-AC-OFF-129x6.png"), UIControlState.Normal);
				btnDC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-DC-OFF-196x6.png"), UIControlState.Normal);
				btnGND1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-GND-ON-263x6.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateAttenuationFactor1 ()
		{
			switch (wfs210.Channels [0].AttenuationFactor) {
			case AttenuationFactor.X1:
				btnProbe1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-1X-OFF-344x6.png"), UIControlState.Normal);
				break;
			case AttenuationFactor.X10:
				btnProbe1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-10X-ON-344x6.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateVoltText1 ()
		{
			string text;
			text = "Incorrect Value";
			text = VoltsPerDivisionConverter.ToString (wfs210.Channels [0].VoltsPerDivision);
			lblVolt1.Text = text;
		}

		void UpdateInputCoupling2 ()
		{
			switch (wfs210.Channels [1].InputCoupling) {
			case InputCoupling.AC:
				btnAC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-AC-ON-129x710.png"), UIControlState.Normal);
				btnDC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-DC-OFF-196x710.png"), UIControlState.Normal);
				btnGND2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-GND-OFF-263x710.png"), UIControlState.Normal);
				break;
			case InputCoupling.DC:
				btnAC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-AC-OFF-129x710.png"), UIControlState.Normal);
				btnDC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-DC-ON-196x710.png"), UIControlState.Normal);
				btnGND2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-GND-OFF-263x710.png"), UIControlState.Normal);
				break;
			case InputCoupling.GND:
				btnAC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-AC-OFF-129x710.png"), UIControlState.Normal);
				btnDC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-DC-OFF-196x710.png"), UIControlState.Normal);
				btnGND2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-GND-ON-263x710.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateAttenuationFactor2 ()
		{
			switch (wfs210.Channels [1].AttenuationFactor) {
			case AttenuationFactor.X1:
				btnProbe2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-1X-OFF-344x710.png"), UIControlState.Normal);
				break;
			case AttenuationFactor.X10:
				btnProbe2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-10X-ON-344x710.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateVoltText2 ()
		{
			string text;
			text = "Incorrect Value";
			text = VoltsPerDivisionConverter.ToString (wfs210.Channels [1].VoltsPerDivision);
			lblVolt2.Text = text;
		}

		void UpdateTriggerChannelUI ()
		{
			if (wfs210.Trigger.Channel == 0) {
				btnTriggerCH1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-CHAN1-ON-6x96.png"), UIControlState.Normal);
				btnTriggerCH2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-CHAN2-OFF-60x96.png"), UIControlState.Normal);
			} else {
				btnTriggerCH1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-CHAN1-OFF-6x96.png"), UIControlState.Normal);
				btnTriggerCH2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-CHAN2-ON-60x96.png"), UIControlState.Normal);
			}
		}

		void UpdateTriggerSlopeUI ()
		{
			if (wfs210.Trigger.Slope == TriggerSlope.Rising) {
				btnTriggerSlopeUp.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-UP-ON-6x156.png"), UIControlState.Normal);
				btnTriggerSlopeDown.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-DOWN-OFF-60x156.png"), UIControlState.Normal);
			} else {
				btnTriggerSlopeUp.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-UP-OFF-6x156.png"), UIControlState.Normal);
				btnTriggerSlopeDown.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-DOWN-ON-60x156.png"), UIControlState.Normal);
			}
		}

		void UpdateTriggerModeUI ()
		{
			switch (wfs210.Trigger.Mode) {
			case TriggerMode.Normal:
				btnTriggerNrml.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-NRML-ON-6x276.png"), UIControlState.Normal);
				btnTriggerOnce.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-ONCE-OFF-60x276.png"), UIControlState.Normal);
				btnTriggerRun.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-RUN-OFF-6x216.png"), UIControlState.Normal);
				break;
			case TriggerMode.Once:
				btnTriggerNrml.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-NRML-OFF-6x276.png"), UIControlState.Normal);
				btnTriggerOnce.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-ONCE-ON-60x276.png"), UIControlState.Normal);
				btnTriggerRun.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-RUN-OFF-6x216.png"), UIControlState.Normal);
				break;
			case TriggerMode.Run:
				btnTriggerNrml.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-NRML-OFF-6x276.png"), UIControlState.Normal);
				btnTriggerOnce.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-ONCE-OFF-60x276.png"), UIControlState.Normal);
				btnTriggerRun.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-RUN-ON-6x216.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateHoldUI ()
		{
			if (wfs210.Hold)
				btnTriggerHold.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-HOLD-ON-6x336.png"), UIControlState.Normal);
			else
				btnTriggerHold.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-HOLD-OFF-6x336.png"), UIControlState.Normal);
		}

		void UpdateAutorangeUI ()
		{
			if (wfs210.AutoRange)
				btnAutorange.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/AUTO RANGE/AUTORANGE-ON-6x541.png"), UIControlState.Normal);
			else
				btnAutorange.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/AUTO RANGE/AUTORANGE-OFF-6x541.png"), UIControlState.Normal);
		}

		void UpdateTimeBaseText ()
		{
			string text;
			text = "Incorrect Value";
			text = TimeBaseConverter.ToString (wfs210.TimeBase);
			lblTime.Text = text;
		}

		void UpdateMarkerMeasurement1 ()
		{
			switch (markerMeasurements [0].SelectedUnit) {
			case MarkerUnit.dt:
				SetSignalWithDtValue (0);
				break;
			case MarkerUnit.Frequency:
				SetSignalWithFrequency (0);
				break;
			case MarkerUnit.dV1:
				SetSignalWithDV1 (0);
				break;
			case MarkerUnit.dV2:
				SetSignalWithDV2 (0);
				break;
			case MarkerUnit.EnableDisableMarkers:
				EnableDisableMarkers ();
				break;
			default:
				btnMarkerMeasurements.SetTitle ("Unsupported Measurement", UIControlState.Normal);
				break;
			}
		}

		private void SetSignalWithDtValue (int channel)
		{
			var value = MarkerDataCalculator.CalculateTime (wfs210.TimeBase, ScopeView.xMarkers [0].Value, ScopeView.xMarkers [1].Value, new DeviceContext (), ScopeView.Frame);
			value = Math.Round (value, 6);
			var title = ToEngineeringNotation (value);
			title += "s";
			if (channel == 0)
				btnMarkerMeasurements.SetTitle (title, UIControlState.Normal);
			else
				btnMarkerMeasurements2.SetTitle (title, UIControlState.Normal);
		}

		void SetSignalWithFrequency (int channel)
		{
			var value = MarkerDataCalculator.CalculateFrequency (wfs210.TimeBase, ScopeView.xMarkers [0].Value, ScopeView.xMarkers [1].Value, new DeviceContext (), ScopeView.Frame);
			value = Math.Round (value, 2);
			var title = ToEngineeringNotation (value);
			title += "Hz";
			if (channel == 0)
				btnMarkerMeasurements.SetTitle (title, UIControlState.Normal);
			else
				btnMarkerMeasurements2.SetTitle (title, UIControlState.Normal);
		}



		void SetSignalWithDV1 (int channel)
		{
			var value = MarkerDataCalculator.CalculateDV (wfs210.Channels [0].VoltsPerDivision, ScopeView.yMarkers [0].Value, ScopeView.yMarkers [1].Value, new DeviceContext (), ScopeView.Frame);
			value = Math.Round (value, 2);
			var title = ToEngineeringNotation (value);
			title += "V";
			if (channel == 0)
				btnMarkerMeasurements.SetTitle (title, UIControlState.Normal);
			else
				btnMarkerMeasurements2.SetTitle (title, UIControlState.Normal);
		}

		void SetSignalWithDV2 (int channel)
		{
			var value = MarkerDataCalculator.CalculateDV (wfs210.Channels [1].VoltsPerDivision, ScopeView.yMarkers [0].Value, ScopeView.yMarkers [1].Value, new DeviceContext (), ScopeView.Frame);
			value = Math.Round (value, 2);
			var title = ToEngineeringNotation (value);
			title += "V";
			if (channel == 0)
				btnMarkerMeasurements.SetTitle (title, UIControlState.Normal);
			else
				btnMarkerMeasurements2.SetTitle (title, UIControlState.Normal);

		}

		void EnableDisableMarkers()
		{
			if (ScopeView.MarkersAreVisible)
				ScopeView.DisableMarkers ();
			else
				ScopeView.EnableMarkers ();
		}

		void UpdateSignalMeasurement1 ()
		{
			switch (signalMeasurements [0].SelectedUnit) {
			case SignalUnit.DbGain:
				SetSignalWithDtValue (0);
				break;
			case SignalUnit.Dbm1:
				SetSignalWithFrequency (0);
				break;
			case SignalUnit.Dbm2:
				SetSignalWithDV1 (0);
				break;
			case SignalUnit.RMS:
				SetSignalWithDV2 (0);
				break;
			case SignalUnit.TRMS:
				EnableDisableMarkers ();
				break;
			case SignalUnit.Vdc:
				break;
			case SignalUnit.VMax:
				break;
			case SignalUnit.VMin:
				break;
			case SignalUnit.Vptp:
				break;
			case SignalUnit.WRMS16:
				break;
			case SignalUnit.WRMS2:
				break;
			case SignalUnit.WRMS32:
				break;
			case SignalUnit.WRMS4:
				break;
			case SignalUnit.WRMS8:
				break;
			default:
				btnMarkerMeasurements.SetTitle ("Unsupported Measurement", UIControlState.Normal);
				break;
			}
		}

		void UpdateMarkerMeasurement2 ()
		{
			switch (markerMeasurements [1].SelectedUnit) {
			case MarkerUnit.dt:
				SetSignalWithDtValue (1);
				break;
			case MarkerUnit.Frequency:
				SetSignalWithFrequency(1);
				break;
			case MarkerUnit.dV1:
				SetSignalWithDV1 (1);
				break;
			case MarkerUnit.dV2:
				SetSignalWithDV2 (1);
				break;
			case MarkerUnit.EnableDisableMarkers:
				EnableDisableMarkers ();
				break;
			default:
				btnMarkerMeasurements.SetTitle ("Unsupported Measurement", UIControlState.Normal);
				break;
			}
		}

		void UpdateSignalMeasurement2 ()
		{
			//throw new NotImplementedException ();
		}

		private static string ToEngineeringNotation (double d)
		{
			double exponent = Math.Log10 (Math.Abs (d));
			if (Math.Abs (d) >= 1) {
				switch ((int)Math.Floor (exponent)) {
				case 0:
				case 1:
				case 2:
					return d.ToString ();
				case 3:
				case 4:
				case 5:
					return (d / 1e3).ToString () + "k";
				case 6:
				case 7:
				case 8:
					return (d / 1e6).ToString () + "M";
				case 9:
				case 10:
				case 11:
				default:
					return (d / 1e24).ToString () + "Y";
				}
			} else if (Math.Abs (d) > 0) {
				switch ((int)Math.Floor (exponent)) {
				case -1:
				case -2:
				case -3:
					return (d * 1e3).ToString () + "m";
				case -4:
				case -5:
				case -6:
					return (d * 1e6).ToString () + "μ";
				case -7:
				case -8:
				case -9:
					return (d * 1e9).ToString () + "n";
				default:
					return (d * 1e15).ToString () + "y";
				}
			} else {
				return "0";
			}
		}
	}
}
