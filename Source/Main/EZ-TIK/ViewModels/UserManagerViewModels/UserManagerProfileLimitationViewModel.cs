using EZ_TIK.Models;
using EZ_TIK.Parsers;
using Prism.Mvvm;

namespace EZ_TIK.ViewModels
{
    public class UserManagerProfileLimitationViewModel : BindableBase
    {
        #region Public Properties

        /// <summary>
        /// The backing model of this viewmodel
        /// </summary>
        public UserManagerProfileLimitation LimitationModel { get; private set; }

        /// <summary>
        /// The name of the limitation
        /// </summary>
        public string Name
        {
            get => LimitationModel.Name;
            set
            {
                LimitationModel.Name = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The Limit of allowed downloaded bytes
        /// </summary>
        public ByteSize DownloadLimit
        {
            get => ByteSize.FromBytes(LimitationModel.DownloadLimit);
            set
            {
                LimitationModel.DownloadLimit = (long)value.Bytes;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The Limit of allowed uploaded bytes
        /// </summary>
        public ByteSize UploadLimit
        {
            get => ByteSize.FromBytes(LimitationModel.UploadLimit);
            set
            {
                LimitationModel.UploadLimit = (long)value.Bytes;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The Limit of allowed transfered bytes
        /// </summary>
        public ByteSize TransferLimit
        {
            get => ByteSize.FromBytes(LimitationModel.TransferLimit);
            set
            {
                LimitationModel.TransferLimit = (long)value.Bytes;
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}
