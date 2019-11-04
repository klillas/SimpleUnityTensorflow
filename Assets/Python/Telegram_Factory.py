import protobuf.Telegrams_pb2 as Telegrams

class Telegram_Factory:
    def Create_Print_Request(self, message):
        request = Telegrams.Request()
        request.command = Telegrams.Request.PRINT
        request.message = message
        return request