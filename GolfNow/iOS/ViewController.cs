using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Foundation;
using UIKit;
using Amazon.Lex;
using Amazon.Lex.Model;

namespace GolfNow.iOS
{
    public partial class ViewController : UIViewController
    {
        public static List<string> messages = new List<string>();
        public static String accessKey = "ASIAR6K6Z5XVEBULNAGQ";
        public static String accessKeySecret = "jqrTAV2GsN/JRHs9N/RP+CdGRU7ktGotQLQsOCeF";
        public static String accessSessionToken = "FQoGZXIvYXdzELf//////////wEaDJQvTD693wjRTW82TCL4AtRsqHLh0hG8jaE1iY5VEzUh+PIfmypOoA/xkYX5SBAAKPMv8RSr+ncbIEG4qCzhA0IS+PgTH9VDYLLFPufdBW1IbGk8QuEmcVnCvtzDD2HnuCzdM9UZCuqT7ACj+4o1y3JtQbtx8eATfsyA2Cj0vKocQTFD0Po8rKKsV2gmFjPO/Xu3JE0rAPEdSIa4LCbwQ397VWDcP2vpBtp7Yx7RjgnFcLHS9s+/mQ0psiaNT5k1QW8I0o3KCApco7c1VxTgk062jdDbmnLzxpMmSVjwDqtdphufUJpVkt7262VxWvSb7dbCVGhR1xVd57UMgGBktVxs3Qz881vW7iOZFG88q7jNbRDUnWeHzf665mLAujZwTN/4pq9eJafmJW88GYRdElAGjHA2TLcBYqn1LrOmHSxWxRKULVPF6usjQl8qmPdLciBu+UisOwYSr+IA+S5sxHLpgEH9Tw4ayinV08fjxJn2qSN0OCMscBA0/jdhVMnsoE0mkQ3W0kIotsnF5AU=";

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
            //sendMessageApi(messageText.Text);
            messages.Add("You: " + messageText.Text);
            tableView.ReloadData();

            AmazonLexClient lexClient = new AmazonLexClient(accessKey, accessKeySecret, accessSessionToken, Amazon.RegionEndpoint.USEast1);

            PostTextRequest request = new PostTextRequest();
            request.BotName = "GolfNowReservation";
            request.BotAlias = "GolfNowTest";
            request.UserId = "250";
            request.InputText = messageText.Text;

            
            var task = lexClient.PostTextAsync(request);
            messages.Add("Bot: " + task.Result.Message);


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
            var text = ViewController.messages[indexPath.Row];
            Console.WriteLine("Text " + text + " contains " + text.Contains("You: ")); 
            if (text.Contains("You:"))
            {
                var cell = (MyCell) tableView.DequeueReusableCell("myCell");

                cell.update(text);
                return cell;
            }
            else
            {
                var cell = (BotCell) tableView.DequeueReusableCell("botCell");
                cell.update(text);
                return cell;
            }
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return ViewController.messages.Count;
        }
    }

}
