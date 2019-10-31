import socket
import protobuf.Telegrams_pb2 as Telegrams


class Udp_Server:

    def __init__(self):
        UDP_IP = "127.0.0.1"
        UDP_PORT = 11000
         
        sock = socket.socket(socket.AF_INET, # Internet
        socket.SOCK_DGRAM) # UDP
        sock.bind((UDP_IP, UDP_PORT))
     
        while True:
            data, addr = sock.recvfrom(1024) # buffer size is 1024 bytes
            request = Telegrams.Request()
            request.ParseFromString(data)
            print("received message: " + request)