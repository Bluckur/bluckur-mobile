using System;
using Newtonsoft.Json;

namespace BluckurWallet.Domain
{
	[JsonObject]
    public class QuizReply
    {
		[JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("correct")]
        public bool IsCorrect { get; set; }

		[JsonProperty("correctAnswer")]
        public string CorrectAnswer { get; set; }

		[JsonProperty("secret")]
		public string Secret { get; set; }

		public QuizReply(){}

		public QuizReply(int id, bool isCorrect, string correctAnswer, string secret)
		{
			this.Id = id;
			this.IsCorrect = isCorrect;
			this.CorrectAnswer = correctAnswer;
			this.Secret = secret;
		}
    }
}
