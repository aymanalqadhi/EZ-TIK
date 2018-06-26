using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Windows.Input;
using EZ_TIK.Events;
using MaterialDesignThemes.Wpf.Transitions;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;


namespace EZ_TIK.ViewModels
{
    public class LoginViewModel : ValidatedBindableBase
    {
        #region Constructors

        public LoginViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            // Subscribes to SelectedRouter
            eventAggregator.GetEvent<SelectedRouter>().Subscribe(r => SelectedRouter = r);

            #region Init Commands

            // Init Login Command
            LoginCommand = new DelegateCommand(async () =>
            {
                // Lock the UI
                IsLoading = true;
                Error = string.Empty;

                // Get the address to connect
                var address = SelectedRouter?.Domain ?? SelectedRouter?.IpAddress;
                if (address == null) Transitioner.MovePreviousCommand.Execute(null, null);

                Status = "Checking Server...";

                // Sends ping request to check if the server is up
                var ping = new Ping();
                PingReply reply;

                // tries sending the request
                try
                {
                    // sends the request
                    reply = await ping.SendPingAsync(address);
                }
                catch
                {
                    // Terminate the proccess
                    IsLoading = false;
                    Error = "Connection Error.";
                    return;
                }


                switch (reply.Status)
                {
                    // Checks if the reuqest is timed out
                    case IPStatus.TimedOut:
                        IsLoading = false;
                        Error = "Connection Timed out.";
                        return;

                    // Checks if the reuqest is ended with a not reachable server status
                    case IPStatus.DestinationHostUnreachable:
                        IsLoading = false;
                        Error = "This address is unreachable or it doesn't exisit.";
                        return;
                }


                // Checks if the reuqest has a status other than success status
                if (reply.Status != IPStatus.Success)
                {
                    IsLoading = false;
                    Error = "Connection Error.";
                    return;
                }

                // TODO:
                // 1. Do login proccess
                // 2. Create connection in the unity container


                // Navigates to the main application content
                IsLoading = false;
                regionManager.RequestNavigate(My.Regions[Region.MainContent], My.Views[View.MainView]);
                regionManager.RequestNavigate(My.Regions[Region.MainRegion], My.Views[View.HomeView]);

            }, () => !string.IsNullOrEmpty(_username));

            #endregion
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Command to perform the login proccess
        /// </summary>
        public ICommand LoginCommand { get; set; }

        #endregion

        #region Private members

        /// <summary>
        ///     Username field
        /// </summary>
        private string _username;

        #endregion

        #region Public properties

        /// <summary>
        ///     The username from the login view
        /// </summary>
        [Required]
        public string Username
        {
            get => _username; set
            {
                if (value != _username)
                    SetProperty(ref _username, value);

                ((DelegateCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///     The password from the login view
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Selected Router
        /// </summary>
        public RouterOsDeviceViewModel SelectedRouter { get; set; }

        /// <summary>
        ///     Field to determine if there is proccesses in background
        /// </summary>
        public bool IsLoading { get; set; }

        /// <summary>
        ///     The error message ( if exists )
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        ///     The current proccess status ( if exists )
        /// </summary>
        public string Status { get; set; }

        #endregion
    }
}