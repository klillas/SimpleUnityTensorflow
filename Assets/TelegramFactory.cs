using System.Collections.Generic;

namespace Assets
{
    public static class TelegramFactory
    {
        public static Telegrams.Request CreatePredictRequest(IEnumerable<float> predict_x)
        {
            var request = new Telegrams.Request();
            request.Command = Telegrams.Request.Types.Command.Predict;
            request.PredictX.AddRange(predict_x);

            return AddTransactionID(request);
        }

        public static Telegrams.Request CreateBeginTrainingRequest()
        {
            var request = new Telegrams.Request();
            request.Command = Telegrams.Request.Types.Command.BeginTraining;

            return AddTransactionID(request);
        }

        public static Telegrams.Request CreateAddTrainingDataRequest(IEnumerable<float> training_x, IEnumerable<float> training_y)
        {
            var request = new Telegrams.Request();
            request.Command = Telegrams.Request.Types.Command.AddTrainingData;
            request.TrainingX.AddRange(training_x);
            request.TrainingY.AddRange(training_y);

            return AddTransactionID(request);
        }

        private static Telegrams.Request AddTransactionID(Telegrams.Request request)
        {
            request.TransactionId = System.Guid.NewGuid().ToString();
            return request;
        }
    }
}
