using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Util;
using Android;
using static Android.Manifest;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Android.Content.PM;
using Xamarin.Forms;
using System;
using Android.Widget;
using Java.Util;
using System.Collections.Generic;

namespace Beacons2.Droid
{
    [Activity(Label = "Beacons2", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private BluetoothAdapter bluetoothAdapter;
        private BluetoothLeScanner bluetoothLeScanner;
        private BluetoothScanCallback scanCallback;
        private Handler handler;
        private static int menorFreq = 0; 
        private static TextView valorMinimoHoly;
        private static TextView valorAtualHoly;
        private static TextView valorMinimoRDL;
        private static TextView valorAtualRDL;
        private static string beaconAtual = "Holy";
        private static TextView txtHoly;
        private static TextView txtRDL;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            valorMinimoHoly = FindViewById<TextView>(Resource.Id.valorMinimoHoly);
            valorAtualHoly = FindViewById<TextView>(Resource.Id.valorAtualHoly);
            valorMinimoRDL = FindViewById<TextView>(Resource.Id.valorMinimoRDL);
            valorAtualRDL = FindViewById<TextView>(Resource.Id.valorAtualRDL);
            txtHoly = FindViewById<TextView>(Resource.Id.txtHoly);
            txtRDL = FindViewById<TextView>(Resource.Id.txtRDL);

            BluetoothManager bluetoothManager = (BluetoothManager)GetSystemService(Context.BluetoothService);
            if (bluetoothManager != null)
            {
                bluetoothAdapter = bluetoothManager.Adapter;
            }

            if (bluetoothAdapter != null)
            {
                // Verifica se o Bluetooth está ativado 
                if (!bluetoothAdapter.IsEnabled)
                {
                    Intent enableBluetoothIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                    StartActivity(enableBluetoothIntent);
                }

                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothScan) != Android.Content.PM.Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.BluetoothScan }, 2);
                }

                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessFineLocation }, 2);
                }

                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothConnect) != Android.Content.PM.Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.BluetoothConnect }, 3);
                }

                scanCallback = new BluetoothScanCallback();
                handler = new Handler();
                handler.PostDelayed(() =>
                {
                    StopScanning();
                }, 900000); 

                StartScanning();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopScanning();
        }

        private void StartScanning()
        {
            if (bluetoothAdapter != null && bluetoothAdapter.IsEnabled)
            {
                bluetoothLeScanner = bluetoothAdapter.BluetoothLeScanner;

                // Define o filtro de scan para beacons Bluetooth
                ScanFilter scanFilter1 = new ScanFilter.Builder()
                    .SetDeviceName("Holy")
                    .Build();

                ScanFilter scanFilter2 = new ScanFilter.Builder()
                    .SetDeviceName("RDL52832")
                    .Build();

                // Configura as opções de scan
                ScanSettings scanSettings = new ScanSettings.Builder()
                    .SetScanMode(Android.Bluetooth.LE.ScanMode.LowLatency)
                    .Build();

                //bluetoothLeScanner.StartScan(scanCallback);
                bluetoothLeScanner.StartScan(new List<ScanFilter> { scanFilter1, scanFilter2 }, scanSettings, scanCallback);
            }
        }

        private void StopScanning()
        {
            if (bluetoothAdapter != null && bluetoothAdapter.IsEnabled)
            {
                bluetoothLeScanner?.StopScan(scanCallback);
            }
        }

        private class BluetoothScanCallback : ScanCallback
        {
            public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
            {
                BluetoothDevice device = result.Device;
                int rssi = result.Rssi;
                Log.Debug("Bluetooth", $"Dispositivo: {device.Name} - Endereço: {device.Address} - RSSI: {rssi}");

         


                if (device.Name == beaconAtual)
                {
                    //valorAtual.Text = "Valor atual: " + rssi;

                    if (rssi < menorFreq)
                    {
                        menorFreq = rssi;
                        //valorMinimo.Text = "Valor minimo: " + menorFreq;

                    }

                    if (rssi >= -50)
                    {
                        if (beaconAtual == "Holy")
                        {
                            txtHoly.Visibility = Android.Views.ViewStates.Visible;
                        } else
                        {
                            txtRDL.Visibility = Android.Views.ViewStates.Visible;
                        }
                        
                    } else {
                        txtHoly.Visibility = Android.Views.ViewStates.Gone;
                        txtRDL.Visibility = Android.Views.ViewStates.Gone;
                    }


                }

            }
        }
    }
}
