import RPi.GPIO as GPIO
import serial
import time
import os
import sys

# Find serial port in case it has changed
# this will help avoid errors if the device port has changed for the usb serial connection
tty_dir = "/dev/"  # Linux directory where usb connections will show
active_ports = []  # list to contain ports in use
for f in os.listdir(tty_dir):
    if os.path.isfile(tty_dir + f) and f.startswith("ttyUSB"):
        active_ports.append(f)
try:  # Try selecting the first item in the array
    usb_port = active_ports[0]
except Exception as ex:  # if no port is in the array, exit with error
    print(f"ERROR - An error has occured: {type(ex).__name__} - {ex} ")
    sys.exit(1)

# Serial port setup
baud_rate = 115200
# Adjust serial port as needed
serialcom = serial.Serial(usb_port, baud_rate, timeout=1)
serialcom.flush()

# GPIO pin setup
TankFill = 51
TankDrain = 49
Lights = 23
Fan = 25
TankLevel = 3  # Assuming analog to digital conversion pin, modify as needed

# Set GPIO mode to BCM or BOARD depending on your wiring. BOARD is probably the easier way
GPIO.setmode(GPIO.BOARD)
GPIO.setup(TankFill, GPIO.OUT)
GPIO.setup(TankDrain, GPIO.OUT)
GPIO.setup(Lights, GPIO.OUT)
GPIO.setup(Fan, GPIO.OUT)
GPIO.setup(TankLevel, GPIO.IN)


def send_serial_command(command):
    pass


def get_tank_level():
    pass


def open_drain():
    GPIO.output(TankDrain, GPIO.HIGH)


def close_drain():
    GPIO.output(TankDrain, GPIO.LOW)
    send_serial_command('ST<{"cmd_code":"set_text","type":"label","widget":"Water_Drain_Status","text":"Drain Closed"}>ET')
    send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"DrainClosed","visible":true}>ET')
    send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"DrainOpen","visible":false}>ET')


def open_tank_fill():
    GPIO.output(TankFill, GPIO.HIGH)
    send_serial_command('ST<{"cmd_code":"set_text","type":"label","widget":"Water_Drain_Status","text":"Drain Open"}>ET')
    send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"DrainClosed","visible":false}>ET')
    send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"DrainOpen","visible":true}>ET')


def close_tank_fill():
    GPIO.output(TankFill, GPIO.LOW)


def lights_on():
    GPIO.output(Lights, GPIO.HIGH)
    send_serial_command('ST<{"cmd_code":"set_text","type":"label","widget":"Light_Status","text":"Lights On"}>ET')
    send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"LightsOff","visible":false}>ET')
    send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"LightsOn","visible":true}>ET')


def lights_off():
    GPIO.output(Lights, GPIO.LOW)
    send_serial_command('ST<{"cmd_code":"set_text","type":"label","widget":"Light_Status","text":"Lights Off"}>ET')
    send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"LightsOff","visible":true}>ET')
    send_serial_command('ST<{"cmd_code":"set_visible","type":"widget","widget":"LightsOn","visible":false}>ET')

# Main funtion 
def main():
    while True:
        current_time = time.ctime(time.time())
        hmi_serial = serialcom.read(size=20)
        print(f"{current_time}: SERIAL READ - {hmi_serial}")


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        GPIO.cleanup()
        serialcom.close()

