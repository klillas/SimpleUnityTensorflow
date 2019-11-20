import Udp_Server
import protobuf.Telegrams_pb2 as Telegrams
import queue

class Unity_Controller:
    _incoming_queue = None
    _udp_server = None

    def message_received(self, data, address):
        telegram = Telegrams.Request()
        telegram.ParseFromString(data)
        self._incoming_queue.put((telegram, address))

    def __init__(self, ip, port, _incoming_queue_size = 10000):
        self._max_queue_cached = 0
        buffer_size = 10000

        self._incoming_queue = queue.Queue(_incoming_queue_size)
        self._udp_server = Udp_Server.Udp_Server(ip, port, buffer_size, self.message_received)        
        
    def send(self, telegram, address):
        self._udp_server.send_data(telegram, address)

    def get_incoming_queue(self):
        return self._incoming_queue