using System;
using System.Collections.Generic;
using System.Diagnostics;
using BluckurWallet.Domain;
using BluckurWallet.ServiceLayer.Rest;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace BluckurWallet.UILayer
{
    public partial class QuizPage : ContentPage
	{
        RestConsumer restConsumer;
		readonly Uri baseUrl = new Uri("http://192.168.223.38:8080/Kwetter/api/Rest");
		QuizQuestion currentQuestion;

        public QuizPage()
        {
            InitializeComponent();

			restConsumer = new RestConsumer();

			getNewQuestion();
        }

        private async void getNewQuestion()
		{
			try
			{
				RestResponse response = await restConsumer.GetAsync(new Uri(baseUrl, "getQuestion"));

				JObject json = response.JsonBody;

				/*JObject answersArray = new JObject(
					new JProperty("text2", "2"),
					new JProperty("text3", "3"),
					new JProperty("text4", "4"),
                    new JProperty("text5", "5")
				);

				JObject json = new JObject(
					new JProperty("id", 1),
					new JProperty("text", "1+1="),
					new JProperty("weight", 100),
					new JProperty("answers", answersArray)
			    );*/

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
				await DisplayAlert("Error", ex.Message, "OK");
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

                RestResponse response = await restConsumer.GetAsync(new Uri(baseUrl, string.Format("isAnswerCorrect/{0}/{1}/{2}", currentQuestion.Id, answer, publicKey)));

                JObject json = response.JsonBody;

                /*JObject json = new JObject(
                    new JProperty("id", 12),
                    new JProperty("correct", false),
                    new JProperty("correctAnswer", 2),
                    new JProperty("secret", "afcbcedabcdadbecabedaabcbdeabcbbedabbadbcbadebadcbbfffaabc")
                );*/

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
				await DisplayAlert("Error", ex.Message, "OK");            
			}
        }
    }
}
