using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GolfNow
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var user = new User
            {
                Username = usernameEntry.Text,
                Password = passwordEntry.Text
            };

            var isValid = AreCredentialsCorrect(user);
            if (isValid)
            {
                App.IsUserLoggedIn = true;
                //Navigation.InsertPageBefore(new MainPage(), this);
                await Navigation.PushModalAsync(new Chat());
                //Application.Current.MainPage = new NavigationPage(new Chat());
                //await Navigation.PopModalAsync();
            }
            else
            {
                messageLabel.Text = "Login failed";
                passwordEntry.Text = string.Empty;
            }
        }

        bool AreCredentialsCorrect(User user)
        {
            return true;
        }


        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new SignUpPage());
        }
    }

}
