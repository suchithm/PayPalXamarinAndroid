using Android.App;
using Android.Widget;
using Android.OS;
using Paypal;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System; 

namespace Paypal
{
	[Activity (Label = "Paypal", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{ 
		string paypalemail="shirasangi-facilitator@itwinetech.com";
		string PaypalTestMode="Y" ;
		string PayPalLiveUrl="https://www.paypal.com/cgi-bin/webscr";
		string PayPalTestUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";
		string CurrencyCode = "USD";
		string FailedURL = "PaymentError.aspx";
		string SuccessURL="http://localhost:55917";
		internal static StringBuilder builder;
		long TotalAmount=Convert.ToInt64("100");
		 
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);


			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += delegate {
				RedirectToPaypalSite("Radhakrsihnan", "Madiwala", "Karnataka", "09003467461", "radha123456@gmail.com", "no 16 Bangalr", "Ebook", "Ebook", 1, CurrencyCode, TotalAmount);
 
			};
		}
		protected override void OnActivityResult (int requestCode, Result resultCode, Android.Content.Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
		}

		private string VerifyPaypalStatus()
		{
			string url = string.Empty;
			if ( PaypalTestMode == "Y")
			{
				//string strSandbox = "https://www.sandbox.paypal.com/cgi-bin/webscr";
				url =  PayPalTestUrl;
			}
			else
			{
				//string strLive = "https://www.paypal.com/cgi-bin/webscr";

				url =  PayPalLiveUrl;
			}
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

			//Set values for the request back
			req.Method = "POST";

			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
//				| SecurityProtocolType.Tls11
//				| SecurityProtocolType.Tls12
				| SecurityProtocolType.Ssl3;

			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

			req.ContentType = "application/x-www-form-urlencoded";
			//byte[] param = Request.BinaryRead(HttpContext.Current.Request.ContentLength);
			string strRequest=null; //= Encoding.ASCII.GetString(param);
			strRequest+= "&cmd=_notify-validate";
			req.ContentLength = strRequest.Length; 

			//Send the request to PayPal and get the response
			StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
			streamOut.Write(strRequest);

			streamOut.Close();
			StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
			string strResponse = streamIn.ReadToEnd();
			streamIn.Close();
			string txn_id = string.Empty; 
			if (strResponse == "VERIFIED")
			{
				// strRequest is a long string delimited by '&'
				string[] responseArray = strRequest.Split('&');

				List<KeyValuePair<string, string>> lkvp = new List<KeyValuePair<string, string>>();

				string[] temp;

				// for each key value pair
				foreach (string i in responseArray)
				{
					temp = i.Split('=');
					lkvp.Add(new KeyValuePair<string, string>(temp[0], temp[1]));
				}

				// now we have a list of key value pairs
				string payerEmail = string.Empty;
				//string firstName = string.Empty;
				//string lastName = string.Empty;
				//string address = string.Empty;
				//string city = string.Empty;
				//string state = string.Empty;
				//string zip = string.Empty; 
				//string contactPhone = string.Empty;

				foreach (KeyValuePair<string, string> kvp in lkvp)
				{
					switch (kvp.Key)
					{
					case "payer_email":
						payerEmail = kvp.Value.Replace("%40", "@");
						break;
						//case "first_name":
						//    firstName = kvp.Value;
						//    break;
						//case "last_name":
						//    lastName = kvp.Value;
						//    break;
						//case "address_city":
						//    city = kvp.Value.Replace("+", " ");
						//    break;
						//case "address_state":
						//    state = kvp.Value.Replace("+", " ");
						//    break;
						//case "address_street":
						//    address = kvp.Value.Replace("+", " ");
						//    break;
						//case "address_zip":
						//    zip = kvp.Value;
						//    break;
						//case "contact_phone":
						//    contactPhone = kvp.Value;
						//    break;
					case "txn_id":
						{
							txn_id = kvp.Value; 
							break; 
						}

					default:
						break;
					}
				}
				// need to figure out a way to catch unwanted responses here... redirect somehow
				return txn_id; 
			}
			else if (strResponse == "INVALID")
			{
				//log for manual investigation
			}
			else
			{
				//log response/ipn data for manual investigation
			}
			return txn_id;

		}
		private void RedirectToPaypalSite(string Username, string city, string state, string phone, string email, string address1, string item_name, string itemInfo, int quantity, string currency, decimal TotalAmount)
		{


			  builder = new StringBuilder();
			if ( PaypalTestMode.ToString() != "Y")
			{
				//Mention URL to redirect content to Live paypal site
				builder.Append(  PayPalLiveUrl + "?cmd=_xclick&business=" +
					"paypalemail".ToString());

			}
			else
			{   //Mention URL to redirect content to Sandbox paypal site
				builder.Append( PayPalTestUrl + "?cmd=_xclick&business=" +
					paypalemail.ToString());
			}

			//First name i assign static based on login details assign this value
			builder.Append( "&first_name=" + Username);

			//City i assign static based on login user detail you change this value
			builder.Append( "&city=" + city);

			//State i assign static based on login user detail you change this value
			builder.Append( "&state=" + state);

			//Product Name
			builder.Append( "&item_name=" + item_name);

			//Product Name
			builder.Append( "&amount=" + TotalAmount);

			//Phone No
			builder.Append( "&night_phone_a=" + phone);

			//Product Name
			builder.Append( "&item_name=" + itemInfo);

			//Address 
			builder.Append( "&address1=" + address1);

			//Address 
			builder.Append( "&email=" + email);

			//Business contact id
			//  builder.Append( "&business=k.tapankumar@gmail.com");

			//Shipping charges if any
			builder.Append( "&shipping=0");

			//Handling charges if any
			builder.Append( "&handling=0");

			//Add quatity i added one only statically 
			builder.Append( "&quantity=" + quantity);

			//Currency code 
			builder.Append( "&currency=" + currency);

			//invoice code 
			builder.Append( "&invoice=" + Guid.NewGuid().ToString());

			//Success return page url
			builder.Append( "&return=" + "http://www.appliedcodelog.com/2015/12/push-notification-using-google-cloud.html");

			//Failed return page url
			builder.Append( "&cancel_return=" + FailedURL .ToString());
			builder.Append( "&notify_url=" +  "http://www.appliedcodelog.com/2015/12/push-notification-using-google-cloud.html");
			StartActivity (typeof(WebViewActivity));
			//ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popup", "window.open('" + builder + "','_blank','resizable=no,menubar=no,toolbar=no,location=no,directories=no,status=no')", true);
			// Response.Redirect(builder.ToString());
		}

	}
}


