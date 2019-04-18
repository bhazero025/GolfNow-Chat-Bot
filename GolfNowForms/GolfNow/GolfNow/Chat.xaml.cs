using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using GolfNow;
using Amazon.Lex;
using Amazon.Lex.Model;

using Xamarin.Forms;

namespace GolfNow
{
    public partial class Chat : ContentPage
    {
        public static String accessKey = "ASIAR6K6Z5XVKUOZYY5A";
        public static String accessKeySecret = "MP++sjjR8uHvXb3H/wyqQOCJVqYc7qVYLaXFMN46";
        public static String accessSessionToken = "FQoGZXIvYXdzEIb//////////wEaDO2E4VfkWdekUO4LVSKGA6OJBfYGxcyqviqJi0K1HzSE/SDvTZquLYhFNI/nvi+TXUIPZ6IM8RoSV1DWa/30lg2WalOqxzt7pQTEsON1MgW+xoLTEF95ES+gCWrwBXuT9bKx82uhRZ7xNGKoavJSKE2/krx+/OJRTKfN7wWyKnANHv5U2XkJl3GoF0Q/g9TRkYWst40S6etlt+/apKqrN8sh9Jo6U0qN+a3JHoHcBlpWE10uS/V1eemxoHvO3k2HUKdPCBQqCTe0Wj8eXwoPETGrNqeBvjAKA4cSJLVo8aB/e5GG8Q+e6dPmct5pp6ZKj6/d+Sms1rj8AGdFdNLH4bqAEoXNM0YCVSi9fbKrGdkZ5XzP8IZ5U9XQ4PORjuCsb38hrYsVrCFT6+Mof2cqIWD/vLfWt953SVThVCDE1y2k7bud0O03oJP+i9QwKBOo2wWN0borDUe/0wPbs4Uy6gdBqcjFkAv+3bEJhmN9M8FO3/E/iEOS+DJdwyXHoxdb7wOqzlD0bL0Ca3kjM3T/E/YZD/853SjQxuPlBQ==";
        public static AmazonLexClient lexClient = new AmazonLexClient(accessKey, accessKeySecret, accessSessionToken, Amazon.RegionEndpoint.USEast1);
        //public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        public List<String> Messages { get; set; } = new List<String>();

        public Chat()
        {
            InitializeComponent();

            //Messages.Add(new Message() { Text = "Hi" });
            //Messages.Add(new Message() { Text = "How are you?" });

            //this.BindingContext = new[] { "a", "b", "c" };

            listView.ItemsSource = Messages;

        }

        public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return; // has been set to null, do not 'process' tapped event
            DisplayAlert("Tapped", e.SelectedItem + " row was tapped", "OK");
            ((ListView)sender).SelectedItem = null; // de-select the row
        }

        async void OnCellClicked(object sender, EventArgs e)
        {
            var b = (Xamarin.Forms.Button)sender;
            var t = b.CommandParameter;

            await ((ContentPage)((ListView)((ViewCell)((StackLayout)b.Parent).Parent).Parent).Parent).DisplayAlert("Clicked", t + " button was clicked", "OK");
            //Debug.WriteLine("clicked" + t);
        }

        async void SendMessage(object sender, EventArgs e)
        {
            listView.BeginRefresh();

            PostTextRequest request = new PostTextRequest();
            request.BotName = "GolfNowReservation";
            request.BotAlias = "GolfNowTest";
            request.UserId = "250";
            request.InputText = messageText.Text;

            var task = lexClient.PostTextAsync(request);
            Messages.Add("Me: " + messageText.Text);
            Messages.Add("GolfNow: " + task.Result.Message);

            listView.ItemsSource = Messages.ToArray();
            listView.EndRefresh();

            messageText.Text = "";
        }
    }
}
