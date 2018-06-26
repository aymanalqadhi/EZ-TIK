using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using EZ_TIK.Events;
using EZ_TIK.Models;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace EZ_TIK.ViewModels
{
    public class AddMultipleUserManagerUsersViewModel : BindableBase
    {
        public AddMultipleUserManagerUsersViewModel(IUserManagerClient userManagerClient, IEventAggregator eventAggregator)
        {
            Height = 560;
            CurrentViewIndex = 0;

            UsernameAndPasswordViewModel = new AddMultipleUsersUsernameAndPasswordViewModel();
            ProfileAndOwnerViewModel = new AddMultipleUserManagerUsersProfileAndOwnerViewModel(userManagerClient);
            ((DelegateCommand)UsernameAndPasswordViewModel.NextCommand).RaiseCanExecuteChanged();

            #region Init Cmmands

            // Init the next command of the first view
            UsernameAndPasswordViewModel.NextCommand = new DelegateCommand(() =>
            {
                Height = 400;
                CurrentViewIndex = 1;
            }, () => UsernameAndPasswordViewModel.CanNavigate);


            // init the finish command of the second view
            ProfileAndOwnerViewModel.FinishCommand = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<AddMultipleUserManagerUsersEvent>().Publish(this);
                DialogHost.CloseDialogCommand.Execute(null, null);
            }, () => ProfileAndOwnerViewModel.SelectedCustomerIndex > -1 && (!ProfileAndOwnerViewModel.ActivateUserNow || ProfileAndOwnerViewModel.SelectedProfileIndex > -1));

            #endregion
        }

        public AddMultipleUsersUsernameAndPasswordViewModel UsernameAndPasswordViewModel { get; set; }
        public AddMultipleUserManagerUsersProfileAndOwnerViewModel ProfileAndOwnerViewModel { get; set; }

        public int Height { get; set; }
        public int CurrentViewIndex { get; set; }
    }

    public class AddMultipleUserManagerUsersProfileAndOwnerViewModel : AddUserManagerUserProfileViewModel
    {
        private int _sharedUsers = 0;
        private int _selectedCustomerIndex = -1;

        public AddMultipleUserManagerUsersProfileAndOwnerViewModel(IUserManagerClient userManagerClient) : base(userManagerClient)
        {
            RefreshCustomersCommand = new DelegateCommand(async () =>
            {
               Customers = new ObservableCollection<string>((await userManagerClient.LoadAllCustomersAsync()).Select(c => c.Login));
               if (Customers.Count > 0) SelectedCustomerIndex = 0;
            });

            RefreshCustomersCommand.Execute(null);
        }

        #region Public Properties

        /// <summary>
        /// The Max Users that can share this account
        /// </summary>
        public string SharedUsers
        {
            get => _sharedUsers.ToString(); set => SetIfNumeric(ref _sharedUsers, value);
        }

        /// <summary>
        /// The list of customers
        /// </summary>
        public ObservableCollection<string> Customers { get; set; }

        /// <summary>
        /// The index of the selecte user
        /// </summary>
        public int SelectedCustomerIndex
        {
            get => _selectedCustomerIndex; set
            {
                _selectedCustomerIndex = value;
                ((DelegateCommand)FinishCommand).RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// A Command to refresh the customers list
        /// </summary>
        public ICommand RefreshCustomersCommand { get; set; }        

        #endregion

        private void SetIfNumeric(ref int source, string value)
        {
            if (Regex.IsMatch(value, @"^\d+$")) source = int.Parse(value);
        }
    }
}
