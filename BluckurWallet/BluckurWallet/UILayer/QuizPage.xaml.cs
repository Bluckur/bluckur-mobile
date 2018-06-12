using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BluckurWallet.Domain;
using BluckurWallet.ServiceLayer.Rest;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace BluckurWallet.UILayer
{
    public partial class QuizPage : ContentPage
	{
        RestConsumer restConsumer;
		Uri baseUrl;
		String publicKey;
		QuizQuestion currentQuestion;
        
        public QuizPage()
        {
            InitializeComponent();
            
			restConsumer = new RestConsumer();

			getStoredValues();
   
			getNewQuestion();
        }
        
        /// <summary>
        /// Fetch rest server ip and public key from storage.
        /// </summary>
        private async void getStoredValues()
		{
            string defaultIp = "84.29.78.31:80";

			publicKey = string.Empty;
            baseUrl = new Uri(string.Format("http://{0}/QuizApp/api/Rest/", defaultIp));

            try
            {
				object quizServerIpObj = null;
                object publicKeyObj = null;

				bool fetchedQuizServerIp = Application.Current.Properties.TryGetValue("quizip", out quizServerIpObj);
                bool fetchedPublicKey = Application.Current.Properties.TryGetValue("pubkey", out publicKeyObj);

				string quizServerIp = "";

				publicKey = fetchedPublicKey ? publicKeyObj.ToString() : "";

				if (fetchedQuizServerIp)
				{
					quizServerIp = quizServerIpObj.ToString();
				}
				else
				{
					Application.Current.Properties.Add(new KeyValuePair<string, object>("quizip", defaultIp));

					quizServerIp = defaultIp;
				}

                baseUrl = new Uri(string.Format("http://{0}/QuizApp/api/Rest/", quizServerIp));
            }
            catch (Exception ex)
            {
				await DisplayAlert("Error", string.Format("{0} - {1}", ex.GetType(), ex.Message), "OK");
            }
		}

		private async void getNewQuestion()
		{
			try
			{
				RestResponse response = await restConsumer.GetAsync(new Uri(baseUrl, "getQuestion"));

				JObject json = response.JsonBody;

				QuizQuestion question = json.ToObject<QuizQuestion>();
				currentQuestion = question;

				lblQuestion.Text = question.Question;

				buttonContainer.Children.Clear();

				foreach (KeyValuePair<string, string> pair in question.Answers)
				{
					Button button = new Button();
					button.Text = pair.Value;
					button.TextColor = Color.White;
					button.BackgroundColor = Color.FromHex("e88506");
					button.Clicked += btnAnswer_click;
					button.StyleId = pair.Key;
					button.WidthRequest = 250;

					buttonContainer.Children.Add(button);
				}
				
			}
            catch(Exception ex)
			{
				await DisplayAlert("Error", "Oops, something went wrong", "OK");
			}
		}

		/// <summary>
        /// Answer question.
        /// </summary>
		private async void btnAnswer_click(object sender, EventArgs e)
        {
			try
			{
                Button senderButton = (Button)sender;
                string answer = senderButton.Text;
                string publicKey = "Value";

				Uri uri = new Uri(baseUrl, string.Format("isAnswerCorrect/{0}/{1}/{2}", currentQuestion.Id, answer, publicKey));
                RestResponse response = await restConsumer.GetAsync(new Uri(baseUrl, string.Format("isAnswerCorrect/{0}/{1}/{2}", currentQuestion.Id, answer, publicKey)));

                JObject json = response.JsonBody;

                QuizReply reply = json.ToObject<QuizReply>();

                if (reply.IsCorrect)
                {
                    await DisplayAlert("Well done!", "Your answer was correct!", "Next question");
                }
                else
                {
                    await DisplayAlert("Too bad...", string.Format("The correct answer is: {0}", reply.CorrectAnswer), "Next question");
                }

                getNewQuestion();
			}
			catch(Exception ex)
			{
				await DisplayAlert("Error", "Oops, something went wrong", "OK");            
			}
        }
    }
}
