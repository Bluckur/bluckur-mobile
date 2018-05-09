using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BluckurWallet.Domain
{
	[JsonObject]
    public class QuizQuestion
    {
        [JsonProperty("id")]
		public int Id { get; set; }

        [JsonProperty("text")]
		public string Question { get; set; }
        
        [JsonProperty("weight")]
		public int Weight { get; set; }

        [JsonProperty("answers")]
		public Dictionary<string, string> Answers { get; set; }

		protected QuizQuestion(){}

		public QuizQuestion(int id, string question, int weight, Dictionary<string, string> answers)
        {
			this.Id = id;
			this.Question = question;
			this.Weight = weight;
			this.Answers = answers;
        }
    }
}
