using MaterialDesignThemes.Wpf.Transitions;
using System.Windows.Controls;

namespace EZ_TIK.Views
{
    /// <summary>
    ///     Interaction logic for WelcomeView.xaml
    /// </summary>
    public partial class WelcomeView : UserControl
    {
        public WelcomeView()
        {
            InitializeComponent();

            NextButton.Click += (s, e) => Transitioner.MoveNextCommand.Execute(null, null);
        }
    }
}