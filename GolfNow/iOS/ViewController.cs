using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Foundation;
using UIKit;

namespace GolfNow.iOS
{
    public partial class ViewController : UIViewController
    {
        public static List<string> messages = new List<string>();

        public ViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView.DataSource = new TableViewData();

            // Perform any additional setup after loading the view, typically from a nib.
            //Button.AccessibilityIdentifier = "myButton";
            //Button.TouchUpInside += delegate
            //{
            //    var title = string.Format("{0} clicks!", count++);
            //    Button.SetTitle(title, UIControlState.Normal);
            //};
        }


        partial void sendMessage(UIButton sender)
        {
            Console.WriteLine("Message is " + messageText);
            sendMessageApi(messageText.Text);
            messages.Add("You: " + messageText.Text);
            messageText.Text = "";
            tableView.ReloadData();

            //throw new NotImplementedException();
        }

        private async void sendMessageApi(String message)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://webhook.site/");

            string jsonData = "{\"message\" : "+message+"}";

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("/92279537-4092-4935-a752-5547965db9a7", content);
            Console.WriteLine("Got here! " + message);

            // this result string should be something like: "{"token":"rgh2ghgdsfds"}"
            var result = await response.Content.ReadAsStringAsync();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.		
        }
    }

    class TableViewData : UITableViewDataSource
    {
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell("cell");
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, "cell");
            }
            cell.TextLabel.Text = ViewController.messages[indexPath.Row];
            return cell;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return ViewController.messages.Count;
        }
    }

}
