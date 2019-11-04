import Udp_Server
import protobuf.Telegrams_pb2 as Telegrams

class Unity_Controller:
    udp_server = None
    message_received_callback = None

    def message_received(self, data, address):
        telegram = Telegrams.Request()
        telegram.ParseFromString(data)
        self.message_received_callback(telegram, address)

    def __init__(self, ip, port, message_received_callback):
        buffer_size = 1024
        self.udp_server = Udp_Server.Udp_Server(ip, port, buffer_size, self.message_received)        
        self.message_received_callback = message_received_callback
        
    def send(self, telegram, address):
        self.udp_server.send_data(telegram, address)