using Foundation;
using System;
using UIKit;

namespace GolfNow.iOS
{
    public partial class BotCell : UITableViewCell
    {
        public BotCell (IntPtr handle) : base (handle)
        {
        }

        public void update(String message)
        {
            botMessage.Text = message;
        }
    }
}