using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using EZ_TIK.Events;
using EZ_TIK.LLDP;
using EZ_TIK.Models;
using MaterialDesignThemes.Wpf.Transitions;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Net.Sockets;
using System.Net;

namespace EZ_TIK.ViewModels
{
    public class ChooseDeviceViewModel : BindableBase
    {
        #region Constructors

        public ChooseDeviceViewModel(IEventAggregator eventAggregator, IDialogService dialogService)
        {
            // Init the private members
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;

            // Init Commands

            #region Init Commands

            // Init the command for the next button in select router view

            #region SelectRouterCommand

            SelectRouterCommand = new DelegateCommand(() =>
            {
                // Navigates to login page
                Transitioner.MoveNextCommand.Execute(null, null);

                // Publishes Event with the selected router
                eventAggregator.GetEvent<SelectedRouter>().Publish(_selectedRouter);
            }, () => SelectedRouter != null);

            #endregion

            // Init the command for the refresh button

            #region RefreshDevicesCommand

            RefreshDevicesCommand = new DelegateCommand(async () =>
            {
                IsLoading = true;

                RoutersList = new ObservableCollection<RouterOsDeviceViewModel>();

                // Refresh the devices by sending a broad cast
                using (var udpClient = new UdpClient { EnableBroadcast = true })
                {
                    for (var i = 0; i < 4; i++)
                        await udpClient.SendAsync(new byte[] { 0x0, 0x0, 0x0, 0x0 }, 4, new IPEndPoint(IPAddress.Broadcast, 5678));
                }

                RaisePropertyChanged(nameof(RoutersList));

                await Task.Delay(6500);
                IsLoading = false;
            });

            #endregion

            // Init command to get input from user to use as default gateway

            #region GetDomainCommand

            GetDomainCommand = new DelegateCommand(async () =>
            {
                var message = "Enter your host domain";
                string input;

                while (true)
                {
                    input = (await _dialogService.ShowInputAsync("Doamin", message))?.ToString();
                    if (input == null) return;

                    // TODO: 
                    // Do Check for server

                    if (input.Trim() != string.Empty) break;

                    // Set message to error message
                    message = "Invalid domain name, try again";
                }

                _selectedRouter = new RouterOsDeviceViewModel(input);
                SelectRouterCommand.Execute(null);
            });

            #endregion

            // Command to use default gateways as router ip address

            #region UseDefaultGatewayCommand

            UseDefaultGatewayCommand = new DelegateCommand(async () =>
            {
                var gatway = await Task.Run(() => NetworkService.GetDefaultGateways().FirstOrDefault());

                if(gatway == null)
                {
                    await dialogService.ShowMessageAsync("Error", "There is no active network interfaces!", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative);
                    return;
                }

                SelectedRouter = new RouterOsDeviceViewModel(new RouterOsDevice("XXXXX", "vX.X", "XX:XX:XX:XX:XX:XX", gatway));
                SelectRouterCommand.Execute(null);
            });

            #endregion

            // Command to let users enter thier ip manually

            #region EnterIpManually

            EnterIpManually = new DelegateCommand(async () =>
            {
                var settings = My.MaterialDialogSettings;

                var message = "Enter ip address";
                var input = await Task.Run(() => NetworkService.GetDefaultGateways().FirstOrDefault());

                while (true)
                {
                    settings.DefaultText = input;

                    input = (await _dialogService.ShowInputAsync("Ip Address", message, settings))?.ToString();
                    if (input == null) return;

                    // TODO: 
                    // Do Check for server

                    if (Regex.IsMatch(input.Trim(), @"^(\d+)\.(\d+)\.(\d+)\.(\d+)$") && input.Trim() != "0.0.0.0")
                        break;

                    message = "Invalid IP Address, Please try again!";
                }

                _selectedRouter = new RouterOsDeviceViewModel(new RouterOsDevice("XXXXX", "vX.X", "XX:XX:XX:XX:XX", input));
                SelectRouterCommand.Execute(null);
            });

            #endregion

            #endregion

            // Refresh the routers list
            Task.Run(async () =>
            {
                // Clears the RoutersList and notifies the UI
                RoutersList = new ObservableCollection<RouterOsDeviceViewModel>();
                RaisePropertyChanged(nameof(RoutersList));

                using (var mndpClient = new MndpClient())
                {
                    await mndpClient.StartAsync(async mndpPacket =>
                    {
                        await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (RoutersList.All(r => r.IpAddress != mndpPacket.IpAddress.ToString()))
                                RoutersList.Add(new RouterOsDeviceViewModel(new RouterOsDevice(mndpPacket.Identity ?? "Unknown", mndpPacket.Version ?? "Unknown", mndpPacket.MacAddress != null ? string.Join(":", mndpPacket.MacAddress.ToList().Select(b => b.ToString("X2"))) : "00:00:00:00:00:00", mndpPacket.IpAddress.ToString())));
                        }));

                        RaisePropertyChanged(nameof(RoutersList));
                    });
                }
            });

            // Refresh the devices list
            RefreshDevicesCommand.Execute(null);
        }

        #endregion

        #region Private members

        /// <summary>
        ///     Event aggregator injected by <see cref="Prism.Unity" />
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        ///     Dialog service injected by dependency injection by <see cref="Prism.Unity" />
        /// </summary>
        private readonly IDialogService _dialogService;

        /// <summary>
        ///     The selected router from selection view
        /// </summary>
        private RouterOsDeviceViewModel _selectedRouter;

        /// <summary>
        ///     Check if the is background proccess
        /// </summary>
        private bool _isLoading;

        #endregion

        #region Public properties

        /// <summary>
        ///     The router list from the local network
        /// </summary>
        public ObservableCollection<RouterOsDeviceViewModel> RoutersList { get; set; }

        /// <summary>
        ///     The selected router from routers list
        /// </summary>
        public RouterOsDeviceViewModel SelectedRouter
        {
            get => _selectedRouter; set
            {
                if (value != _selectedRouter)
                    SetProperty(ref _selectedRouter, value);

                ((DelegateCommand)SelectRouterCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Check if the is background proccess
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading; set
            {
                if (value != _isLoading)
                    SetProperty(ref _isLoading, value);
            }
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Command to get selected router from routers list
        /// </summary>
        public ICommand SelectRouterCommand { get; set; }

        /// <summary>
        ///     Command to get a domain name
        /// </summary>
        public ICommand GetDomainCommand { get; set; }

        /// <summary>
        ///     Command to refresh devices list
        /// </summary>
        public ICommand RefreshDevicesCommand { get; set; }

        /// <summary>
        ///     Command to use default gateways as router ip address
        /// </summary>
        public ICommand UseDefaultGatewayCommand { get; set; }

        /// <summary>
        ///     Command to enter router ip address manually
        /// </summary>
        public ICommand EnterIpManually { get; set; }

        #endregion
    }
}