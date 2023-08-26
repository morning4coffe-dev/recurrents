namespace ProjectSBS.Presentation
{
    public partial class LoginViewModel : ObservableObject
    {
        private IAuthenticationService _authentication;

        private INavigator _navigator;


        public LoginViewModel(
            INavigator navigator,
            IAuthenticationService authentication)
        {
            _navigator = navigator;
            _authentication = authentication;
            Login = new AsyncRelayCommand(DoLogin);
        }

        private async Task DoLogin()
        {
            var success = await _authentication.LoginAsync();
            if (success)
            {
                await _navigator.NavigateViewModelAsync<MainViewModel>(this, qualifier: Qualifiers.ClearBackStack);
            }
        }

        public string Title { get; } = "Login";

        public ICommand Login { get; }
    }
}