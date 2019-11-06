import protobuf.Telegrams_pb2 as Telegrams
import uuid

class Telegram_Factory:
    def Create_Print_Request(self, message, transaction_id = None):
        request = Telegrams.Request()
        request.command = Telegrams.Request.PRINT
        request.message = message
        return self.Add_Transaction_ID(request, transaction_id)

    def Create_Predict_Request(self, prediction_y, transaction_id = None):
        request = Telegrams.Request()
        request.command = Telegrams.Request.PREDICTION
        for prediction_item in prediction_y:
            request.prediction_y.append(prediction_item)
        return self.Add_Transaction_ID(request, transaction_id)

    def Add_Transaction_ID(self, request, transaction_id):
        if transaction_id == None:
            transaction_id = str(uuid.uuid4())
        request.transaction_id = transaction_id
        return request