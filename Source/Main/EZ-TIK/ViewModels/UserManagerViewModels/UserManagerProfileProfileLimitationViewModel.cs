using System;
using System.CodeDom;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EZ_TIK.Models;
using Prism.Mvvm;

namespace EZ_TIK.ViewModels
{
    public class UserManagerProfileProfileLimitationViewModel : BindableBase
    {
        #region Private Field
        
        /// <summary>
        /// THe days boolean array
        /// </summary>
        private BitArray _days;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="model">The model of the viewmodel</param>
        public UserManagerProfileProfileLimitationViewModel(UserManagerProfileProfileLimitation model)
        {
            _days = new BitArray(7);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The backing model of this viewmodel
        /// </summary>
        public UserManagerProfileProfileLimitation ProfileLimitationModel { get; set; }

        /// <summary>
        /// The time of the viewmodel
        /// </summary>
        public string FromTime
        {
            get => ProfileLimitationModel.FromTime;
            set
            {
                ProfileLimitationModel.FromTime = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The time of the viewmodel
        /// </summary>
        public string TillTime
        {
            get => ProfileLimitationModel.TillTime;
            set
            {
                ProfileLimitationModel.TillTime = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The Limitations of this ProfileLimitation
        /// </summary>
        public ObservableCollection<UserManagerProfileLimitation> Limitations { get; set; }

        #region Days

        public bool Saturday => _days[0];
        public bool Sunday => _days[1];
        public bool Monday => _days[2];
        public bool Tuseday => _days[3];
        public bool Wednesday => _days[4];
        public bool Thursday => _days[5];
        public bool Friday => _days[6];

        #endregion


        #endregion

        #region Commnds

        /// <summary>
        /// A Command to load limitations form the server
        /// </summary>
        public ICommand LoadLimitationsCommand { get; set; }

        #endregion
    }
}
