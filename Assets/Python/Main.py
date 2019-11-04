import Unity_Controller
import Telegram_Factory

class Main:
    unity_controller = None
    telegram_factory = None

    def message_received(self, telegram, address):
        print(telegram.message + " : " + str(telegram.training_x) + " : " + str(telegram.training_y))
        # request = self.telegram_factory.Create_Print_Request("Hello from python")
        # self.unity_controller.send(request, address)

    def __init__(self):
        ip = "127.0.0.1"
        port = 11000

        self.telegram_factory = Telegram_Factory.Telegram_Factory()
        self.unity_controller = Unity_Controller.Unity_Controller(ip, port, self.message_received)


main = Main()