import socket
import threading

class Udp_Server:
    _sock = None
    buffer_size = None
    message_received_callback = None
    ip = None
    port = None

    def receive_data(self):
        while True:
            data, address = self._sock.recvfrom(self.buffer_size)
            self.message_received_callback(data, address)

    def send_data(self, protobuf_message, address):
        self._sock.sendto(protobuf_message.SerializeToString(), address)

    def __init__(self, ip, port, buffer_size, message_received_callback):
        self._messages_received = 0
        self.ip = ip
        self.port = port
        self.buffer_size = buffer_size
        self.message_received_callback = message_received_callback
        self._sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self._sock.bind((ip, port))

        receive_thread = threading.Thread(target=self.receive_data)
        receive_thread.start()